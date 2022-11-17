//-------------------------------------------------------------------------------------------------------
// <copyright file="XREditorClient.cs" createdby="gblikas">
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
//-------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.XR.ARFoundation;

namespace XRRemote
{
    using UnityEngine.XR.ARSubsystems;

    //
    // The following script only applies inside of the
    // editor environment as this is the script that will actually
    // communicate with the connected device and is not built onto it. 
#if UNITY_EDITOR
    using UnityEngine.SpatialTracking;

    public class XREditorClient : Client
    {

        private static XREditorClient instance = null;
        public static XREditorClient Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<XREditorClient>();
                if (instance == null)
                {
                    if (DebugFlags.displayXRRemoteConnectionStats)
                    {
                        Debug.LogErrorFormat("XREditorClient failure");
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Reference to the remote video which
        /// will be displayed on screen. 
        /// </summary>
        XRRemoteVideo remoteVideo;

        /////////////////////////////////////////////////////////////////////////

        // CONNECTIONS
        string connectionMessage;

        Rect connectionStartMessageRect = new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 200, 400, 100);
        Rect connectionMessageRect = new Rect((Screen.width / 2) - 200, (Screen.height / 2) + 100, 400, 50);
        private string XRRemoteConnectionMessage(string baseMessage)
        {
            return string.Format("XREditorClient ({0}) {1}", name, baseMessage);
        }

        /////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////

        // XR REMOTE DATA.

        XRRemotePacket xrRemotePacketReceived;

        [HideInInspector]
        public TrackingState xrRemoteTrackingState;

        public XRRemote.CameraFrameEvent xrRemoteCameraFrameEvent { get { return xrRemotePacketReceived.cameraFrame; } }

        public XRRemote.Pose xrRemoteTrackedPose
        {
            get
            {
                return xrRemotePacketReceived == null ? null : xrRemotePacketReceived.trackedPose; 
            }
        }

        public UnityEngine.Texture2D xrRemoteScreenRGBTex
        {
            get
            {
                return
                    xrRemotePacketReceived == null ? null :
                    XRRemote.SerializeableTexture2D.FromSerializeableTexture2D(xrRemotePacketReceived.texture);
            }
        }
        /////////////////////////////////////////////////////////////////////////

        bool isXRPlayerInitialized = false;

        bool readyForNewFrame = false;

        private void OnEnable()
        {
            base.Initialize();
        }

        // Use this for initialization
        void Start()
        {
            //
            // initialize the pose deliever system from
            // the player. 
            TrySetupTrackedPoseDriver();

            //
            // make sure the XRRemoteVideo player
            // is properly set up to recieve and then
            // push the image feed from device, to editor. 
            TrySetUpXRRemoteVideo();
        }

        void OnGUI()
        {
            if (!isXRPlayerInitialized)
            {
                if (connectionState == ConnectionState.CONNECTED)
                {
                    connectionMessage = string.Format("XRRemote: ({0}) connected", name);
                }
                else
                { 
                    connectionMessage = string.Format("XRRemote ({0}): please, connect to player", name);
                }

                if (GUI.Button(connectionStartMessageRect, "Start XR Remote Session"))
                {
                    InitializeXRPlayer();
                }

                GUI.Box(connectionMessageRect, connectionMessage);
            }

        }

        private void Update() 
        {
            base.Update();
            if (readyForNewFrame)
            {
                ReadyForFrame();
                readyForNewFrame = false;
            }
        }

        public new void OnDisable()
        {
            base.OnDisable();
        }
        
        private void InitializeXRPlayer()
        {
            InitializeXRSession();
        }
        private void InitializeXRSession()
        {
            SendToPlayer(new EditorARKitSessionInitialized{value = true});
            readyForNewFrame = true;
        }

        /// <summary>
        /// We have caught an incoming frame from the connected player. 
        /// </summary>
        /// <param name="data"></param>
        private void OnXRRemotePacketReceived(XRRemotePacket data)
        {
            readyForNewFrame = false; 

            //if (DebugFlags.displayXRRemoteConnectionStats)
            //{
            //    Debug.LogFormat(XRRemoteConnectionMessage("OnXRRemotePacketReceived"));
            //}

            xrRemotePacketReceived = data;
            if (xrRemotePacketReceived == null)
            {
                if (DebugFlags.displayXRRemoteConnectionStats)
                    Debug.LogWarningFormat("XREditorClient Event ({0}): empty XRRemotePacket");
                return;
            }

            xrRemoteTrackingState = TrackingState.Tracking;

#if UNITY_IOS
            SetXRRemoteVideoFrameRGB(xrRemoteScreenRGBTex);
#elif UNITY_ANDROID
            SetXRRemoteVideoFrameRGB(xrRemoteScreenRGBTex);
#endif
            readyForNewFrame = true;
        }

        private void SetXRRemoteVideoFrameRGB(UnityEngine.Texture2D texture2D)
        {
            remoteVideo.SetRGBTexture(texture2D);
        }

        private void ReadyForFrame() {
            SendToPlayer(new XRFrameReadyPacket{value = true});
        }
        

    #region SETUP_XR_REMOTE_CONNECTION_GO
        private void TrySetupTrackedPoseDriver()
        {
            TrackedPoseDriver trackedPoseDriver = FindObjectOfType<TrackedPoseDriver>();
            if (trackedPoseDriver == null)
            {
                if (DebugFlags.displayXRRemoteConnectionStats)
                {
                    Debug.LogErrorFormat(
                        XRRemoteConnectionMessage(
                            string.Format("TrySetupTrackedPoseDriver Event: null aRPoseDriver")));
                    return;
                }
            }

            XRRemotePoseProvider remotePoseProvider = GetComponent<XRRemotePoseProvider>();
            if (remotePoseProvider == null)
            {
                remotePoseProvider = gameObject.AddComponent<XRRemotePoseProvider>();
            }
            trackedPoseDriver.poseProviderComponent = remotePoseProvider;
        }

        private void TrySetUpXRRemoteVideo()
        {
            remoteVideo = Camera.main.GetComponent<XRRemoteVideo>();
            if (remoteVideo == null)
            {
                // add a XRRemote video to the camera
                // add ites texture. 
                remoteVideo = Camera.main.gameObject.AddComponent<XRRemoteVideo>();
                Material defaultMaterial = Resources.Load("XRVideoMaterial") as Material;
                if (defaultMaterial == null)
                {
                    if (DebugFlags.displayXRRemoteConnectionStats)
                    {
                        Debug.LogErrorFormat(
                            XRRemoteConnectionMessage(
                                "TrySetUpXRRemoteVideo Error: defaultMaterial == null"));
                    }
                    return;
                }

                remoteVideo.remoteMaterial = defaultMaterial;
                remoteVideo.materialType = XRRemoteVideo.MaterialType.RGB;
            }

            if (remoteVideo == null)
            {
                if (DebugFlags.displayXRRemoteConnectionStats)
                {
                    Debug.LogErrorFormat(
                        XRRemoteConnectionMessage(
                            "TrySetUpXRRemoteVideo Error: remoteVideo == null"));
                }
            }
        }
    #endregion

        /// <summary>
        /// When the client responds that the session has been properly initialized. 
        /// </summary>
        /// <param name="messageEventArgs"></param>
        private void OnARSessionHandShakeAck(ARSessionHandShakePacket messageEventArgs)
        {
            isXRPlayerInitialized = true;
        }


        /// <summary>
        /// proc function for making sure that data moves from
        /// EDITOR -> PLAYER
        /// </summary>
        /// <param name="socketChannel"></param>
        /// <param name="serializableObject"></param>
        public void SendToPlayer(object serializableObject)
        {
            base.Send(serializableObject);
        }

        public override void MessageReceived(object obj)
        {
            if (obj is XRRemotePacket) OnXRRemotePacketReceived(obj as XRRemotePacket);
            if (obj is ARSessionHandShakePacket) OnARSessionHandShakeAck(obj as ARSessionHandShakePacket);
        }
    }
#endif
}
