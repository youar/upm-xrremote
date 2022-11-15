//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteServer.cs" createdby="gblikas">
// 
// XR Remote
// Copyright(C) 2020  YOUAR, INC.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// https://www.gnu.org/licenses/agpl-3.0.html
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see
// <http://www.gnu.org/licenses/>.
//
// </copyright>
//
// <modified url="https://bitbucket.org/Unity-Technologies/unity-arkit-plugin/src/default/" modifiedby="gblikas">
//
// This class connects you to the editor for client image and pose sendoff.
//
// </modified> 
//-------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.Serialization;

namespace XRRemote
{
    using System;
    using UnityEngine.SpatialTracking;
    using UnityEngine.UI;
    using UnityEngine.XR.ARFoundation;

    public static class XRSessionInfoStates
    {
        public static readonly string initialized = "Initialized";
        public static readonly string running = "Running";
        public static readonly string stopped = "Stopped";
    }


    public class XRRemoteServer : Server
    {
        /// <summary>
        /// device reference to the DEVICE camera
        /// this will send out the frame to
        /// the XREditorClient script to
        /// recieve. 
        /// </summary>
        public ARCameraManager cameraManager;

        /// <summary>
        /// Reference to the thing we care about. 
        /// </summary>
        public ARPoseDriver arPoseDriver;

        public Text connectionText;

        public Text arSystemBarText;

        public Button testConnectionButton;

        /// <summary>
        /// The actual thing to send. 
        /// </summary>
        XRRemotePacket xrRemotePacket = new XRRemotePacket();

        /// <summary>
        /// necessary to copy out textures
        /// from GPU
        /// </summary>
        [SerializeField] RenderTexture renderTexture;

        /// <summary>
        /// 
        /// </summary>
        Texture2D texture2D;

        /// <summary>
        /// material with the shader needed to conver the
        /// YUV420 system over. 
        /// </summary>
        public Material senderMaterial;

        /// <summary>
        /// 
        /// </summary>
        public int renderTextureWidth = 640;

        /// <summary>
        /// 
        /// </summary>
        public int renderTextureHeight = 480;

        /// <summary>
        /// 
        /// </summary>
        public XRTextureExtractor textureExtractor;

        /// <summary>
        /// A "handshake" for when the editor is 
        /// ready to recieve another frame, false otherwise
        /// </summary>
        private bool readyForFrame; 

        /// <summary>
        /// reference to try and load, if the one is not set in editor.
        /// </summary>
        string defaultMaterial = "YUV420Material"; 

        /// <summary>
        /// Has the ar session started
        /// </summary>
        bool arSessionInitialized = false;


        private string ConnectionMessage(string baseMessage)
        {
            return $"XRRemoteServer.cs {baseMessage}";
        }

        private void DisplayLogConnectionMessage(string message)
        {
            if (DebugFlags.displayEditorConnectionStats)
            {
                Debug.LogFormat(
                    ConnectionMessage(message));
            }
        }

        private void DisplayLogWarningConnectionMessage(string message)
        {
            if (DebugFlags.displayEditorConnectionStats)
            {
                Debug.LogWarningFormat(
                    ConnectionMessage(message));
            }
        }

        private void DisplayLogErrorConnectionMessage(string message)
        {
            if (DebugFlags.displayEditorConnectionStats)
            {
                Debug.LogErrorFormat(
                    ConnectionMessage(message));
            }
        }

        private string SessionInfoMessage(string baseMessage)
        {
            return string.Format("AR Session: {0}", baseMessage);
        }


        #region MONOBEHAVIOUR
        private void OnEnable()
        {
            if (connectionText == null)
            {
                DisplayLogErrorConnectionMessage("OnEnable Event: no connectionText");
                return;
            }
            if (arSystemBarText == null)
            {
                DisplayLogErrorConnectionMessage("OnEnable Event: no arSystemBarText");
                return;
            }
            if (testConnectionButton == null)
            {
                DisplayLogErrorConnectionMessage("OnEnable Event: no testConnectionButton");
                return;
            }
            base.onConnection = FireOnConnection;
            base.onDisconnection = () => { connectionText.text = ConnectionMessage("disconnected"); };

            base.Initialize();

            testConnectionButton.onClick.RemoveAllListeners();
            testConnectionButton.onClick.AddListener(EditorSendTest);

        }

        private void OnApplicationQuit()
        {
            base.Disconnect();
        }

        private void OnDisable()
        {
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnARCameraFrameRecieved;
            }
            base.Disconnect();
        }

        // Start is called before the first frame update
        void FireOnConnection(ConnectionState connectionState)
        {
            if (log) { Debug.Log(ConnectionMessage("start")); }

            connectionText.text = ConnectionMessage("connected");

            if (!SetUpTextureSystem(renderTextureWidth, renderTextureHeight, defaultMaterial)) return;
            if (!SetUpCameraManager(this.cameraManager)) return;
            if (!SetUpTrackedPoseDriver(this.arPoseDriver)) return;

#if UNITY_ANDROID
            //
            // make sure we have a way to get acceess to the
            // texture that we need for passing video frames. 
            if (!SetUpXRTextureExtractor(this.textureExtractor)) return;
#endif
            if (connectionState == ConnectionState.DISCONNECTED)
            {
                connectionText.text = ConnectionMessage("disconnected");
                arSystemBarText.text = SessionInfoMessage(XRSessionInfoStates.stopped);
            }
        }
        #endregion


        /// <summary>
        /// Register all player messages to make sure that they system actualy
        /// is hooked up properly. 
        /// </summary>
        
        //private void RegisterPlayerMethods()
        //{
        //    DisplayLogConnectionMessage("RegisterPlayerConnectionMethods");

        //    base.RegisterMethod(ConnectionMessageIds.fromEditorARKitSessionMsgId, OnARKitSessionInitializationMessage);
        //    base.RegisterMethod(ConnectionMessageIds.readyForFrameEventMsgId, OnReadyForFrameEvent);
        //    base.RegisterMethod(ConnectionMessageIds.TestingIds.kMsgSendEditorToPlayer, OnTextMessageRecieved);
        //}

        #region SCENE_SETUP
        private bool SetUpTextureSystem(int width, int height, string pathToMaterial)
        {
            if (!SetUpMaterial(senderMaterial, pathToMaterial)) return false;

            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            }

            if (texture2D == null)
            {
                if (logLevel != LogLevel.QUIET && textureExtractor != null)
                {
                    DisplayLogConnectionMessage(
                        string.Format("Start: create texture2D [{0},{1}]", width, height));
                }
                texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
            }
            return true;
        }

        private bool SetUpMaterial(Material material, string pathToMaterial)
        {
            material = Resources.Load(pathToMaterial) as Material;
            if (material == null)
            {
                DisplayLogErrorConnectionMessage("Start: failure to load YUV420 default mat");
                return false;
            }

            return true;
        }

        private bool SetUpTrackedPoseDriver(ARPoseDriver driver)
        {
            if (driver != null) return true;

            arPoseDriver = FindObjectOfType<ARPoseDriver>();
            if (arPoseDriver == null)
            {
                if (log)
                {
                    Debug.LogErrorFormat(
                        ConnectionMessage(
                            string.Format("Event: RegisterPlayerMethods, trackedposedriver not foudn")));
                }
                return false;
            }

            return true; 
        }
        private bool SetUpXRTextureExtractor(XRTextureExtractor extractor)
        {
            if (extractor != null) return true;
            textureExtractor = GetComponent<XRTextureExtractor>();
            if (textureExtractor == null)
            {
                textureExtractor = cameraManager.gameObject.AddComponent<XRTextureExtractor>();
            }

            if (textureExtractor == null)
            {
                if (log)
                {
                    Debug.LogFormat(
                        ConnectionMessage(
                            string.Format("TrySetUpXRTextureExtractor: not added")));
                }
                return false;
            }

            return true;
        }
        private bool SetUpCameraManager(ARCameraManager manager)
        {
            if (manager == null)
            {
                cameraManager = FindObjectOfType<ARCameraManager>();
                if (cameraManager == null)
                {
                    if (log)
                    {
                        Debug.LogErrorFormat(
                            ConnectionMessage(
                                string.Format("Event: null camera manager")));
                    }
                    return false;
                }
            }

            manager.frameReceived += OnARCameraFrameRecieved;
            return true;

        }
        #endregion


        private void OnARCameraFrameRecieved(ARCameraFrameEventArgs obj)
        {
            if (connectionState == ConnectionState.DISCONNECTED) return;

            if (log) Debug.Log(ConnectionMessage($"OnARCameraFrameRecieved"));

            if (!arSessionInitialized)
            {
                if (log)
                {
                    Debug.LogFormat(
                        ConnectionMessage(
                            "OnARCameraFrameRecieved Event: ARSession not initialized"));
                }
                return;
            }

            if (!readyForFrame) return;
            
            if (xrRemotePacket == null) xrRemotePacket = new XRRemotePacket();

            XRRemote.Pose pose = arPoseDriver;

            xrRemotePacket.cameraFrame.timestampNs = obj.timestampNs.Value;
            xrRemotePacket.cameraFrame.projectionMatrix = pose;
            xrRemotePacket.cameraFrame.displayMatrix = pose;

            xrRemotePacket.trackedPose = pose;

            xrRemotePacket.face = new XRRemote.FaceInfo();

#if UNITY_IOS
            if(senderMaterial == null) senderMaterial = Resources.Load(defaultMaterial) as Material;
            if (!XRTextureHelper.CompositeYUV420(obj.textures[0], obj.textures[1], ref texture2D, ref renderTexture, senderMaterial))
            {
                if (DebugFlags.displayEditorConnectionStats)
                {
                    Debug.LogWarningFormat(
                        ConnectionMessage(
                            string.Format("OnARCameraFrameRecieved Event: issue composite420")));
                }
                return;
            }

#elif UNITY_ANDROID

            if (!XRTextureHelper.TryGetCPUTextureFromTexture(textureExtractor.texture, ref texture2D, renderTexture))
            {
                if (log)
                {
                    Debug.LogWarningFormat(
                        ConnectionMessage(
                            string.Format("OnARCameraFrameRecieved Event: issue TryGetCPUTextureFromTexture")));
                }
                return;
            }
#endif

            if (xrRemotePacket.texture == null)
            {
                xrRemotePacket.texture = new SerializeableTexture2D(renderTexture.width, renderTexture.height, texture2D.format);
            }

            xrRemotePacket.texture.rawTextureData = texture2D.EncodeToPNG();
            xrRemotePacket.texture.width = texture2D.width;
            xrRemotePacket.texture.height = texture2D.height;
            xrRemotePacket.texture.format = texture2D.format;

            readyForFrame = false;

            //
            // send down the relevant pose information for the
            // system. 
            SendToEditor(xrRemotePacket);
        }

        /// <summary>
        /// todo initialize with the incoming session data embedded into the messageEventArgs
        /// </summary>
        /// <param name="env"></param>
        private void OnARKitSessionInitializationMessage(EditorARKitSessionInitialized env)
        {
            string successMesage = "XRRemoteServer Event: initialized";
            arSystemBarText.text = SessionInfoMessage(XRSessionInfoStates.initialized);
            arSessionInitialized = true;
        }

        private void OnReadyForFrameEvent(XRFrameReadyPacket packet)
        {
            readyForFrame = packet.value;
        }

#region EDITOR_CONNECTION_TESTING
        public void EditorSendTest()
        {
            if (connectionState == ConnectionState.DISCONNECTED) return;
            EditorSendTestMessage(string.Format("EditorSendTest ({0}): hello, world!", name)); 
        }
        public void EditorSendTestMessage(string message)
        {
            //SendToEditor( ConnectionMessageIds.TestingIds.kMsgSendPlayerToEditor, message);
        }
        private void OnTextMessageRecieved(MessageEventArgs messageEventArgs)
        {
            try
            {
                string editorTestString = messageEventArgs.data.Deserialize<string>();
                Debug.LogFormat(
                    ConnectionMessage(
                        string.Format("OnTextMessageRecieved Event: {0}", editorTestString)));
            }
            catch(Exception e)
            {
                Debug.LogError($"EXCEPTION reason: {e.Message}");
            }
        }
        #endregion

        /// <summary>
        /// Helper method to help understand that this script is sending events
        /// from DEVICE -> EDITOR
        /// </summary>
        /// <param name="socketChannel"></param>
        /// <param name="serializableObject"></param>
        private void SendToEditor(object serializableObject)
        {
            base.Send(serializableObject);
        }

        public override void MessageReceived(object obj) {
            if (obj is XRFrameReadyPacket) OnReadyForFrameEvent(obj as XRFrameReadyPacket);
            if (obj is EditorARKitSessionInitialized) OnARKitSessionInitializationMessage(obj as EditorARKitSessionInitialized);
        }
    }
}
