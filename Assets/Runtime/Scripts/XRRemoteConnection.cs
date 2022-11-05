//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteConnection.cs" createdby="gblikas">
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XRRemote
{
    using System;

    using UnityEngine.XR.ARFoundation;
    using UnityEngine.Networking.PlayerConnection;
    using UnityEngine.XR.ARSubsystems;

    //
    // The following script only applies inside of the
    // editor environment as this is the script that will actually
    // communicate with the connected device and is not built onto it. 
#if UNITY_EDITOR

    using UnityEditor.Networking.PlayerConnection;
    using System.Text;
    using UnityEngine.SpatialTracking;

    public class XRRemoteConnection : RemoteConnection
    {

        private static XRRemoteConnection instance = null;
        public static XRRemoteConnection Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<XRRemoteConnection>();
                if (instance == null)
                {
                    if (DebugFlags.displayXRRemoteConnectionStats)
                    {
                        Debug.LogErrorFormat("XRRemoteConnection failure");
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
            return string.Format("XRRemoteConnection ({0}) {1}", name, baseMessage);
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

            base.onConnection = RegisterMethods;

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
                if (connected)
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
            if (readyForNewFrame)
            {
                OnReadyForFrame();
                readyForNewFrame = false;
            }
        }

        private void InitializeXRPlayer()
        {
            InitializeXRSession();
        }
        private void InitializeXRSession()
        {
            SendToPlayer(ConnectionMessageIds.fromEditorARKitSessionMsgId, "From Editor: InitializeXRSession");

            readyForNewFrame = true;
        }

        private void RegisterMethods(ConnectionState connectionState)
        {
            base.RegisterMethod(ConnectionMessageIds.updateCameraFrameMsgId, OnXRRemotePacketReceived);
            base.RegisterMethod(ConnectionMessageIds.TestingIds.kMsgSendPlayerToEditor, OnTextMessageRecieved);
            base.RegisterMethod(ConnectionMessageIds.editorInitARKit, OnARSessionHandShakeAck);
        }

        /// <summary>
        /// We have caught an incomming frame from
        /// the connected player. 
        /// </summary>
        /// <param name="messageEventArgs"></param>
        private void OnXRRemotePacketReceived(MessageEventArgs messageEventArgs)
        {
            readyForNewFrame = false; 

            if (DebugFlags.displayXRRemoteConnectionStats)
            {
                Debug.LogFormat(
                    XRRemoteConnectionMessage(
                        string.Format("OnXRRemotePacketReceived")));
            }

            int incomingPlayerID = messageEventArgs.playerId;

            xrRemotePacketReceived = messageEventArgs.data.Deserialize<XRRemotePacket>();
            if (xrRemotePacketReceived == null)
            {
                if (DebugFlags.displayXRRemoteConnectionStats)
                    Debug.LogWarningFormat("XRRemoteConnection Event ({0}): empty XRRemotePacket", incomingPlayerID);
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

        private void OnReadyForFrame()
        {
            SendToPlayer(ConnectionMessageIds.readyForFrameEventMsgId, "true");
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
                            string.Format("TrySetupTrackedPoseDriver Event: null trackedPoseDriver")));
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
                                string.Format("TrySetUpXRRemoteVideo Error: defaultMaterial == null")));
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
                            string.Format("TrySetUpXRRemoteVideo Error: remoteVideo == null")));
                }
                return;
            }
        }
    #endregion

        /// <summary>
        /// When the client responds that the session has been properly initialized. 
        /// </summary>
        /// <param name="messageEventArgs"></param>
        private void OnARSessionHandShakeAck(MessageEventArgs messageEventArgs)
        {
            isXRPlayerInitialized = true;
            OnTextMessageRecieved(messageEventArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageEventArgs"></param>
        private void OnTextMessageRecieved(MessageEventArgs messageEventArgs)
        {
            string editorTestString = messageEventArgs.data.Deserialize<string>();
            Debug.LogFormat(
                XRRemoteConnectionMessage(string.Format("OnTextMessageRecieved Event: {0}", editorTestString))); 
        }


        /// <summary>
        /// proc function for making sure that data moves from
        /// EDITOR -> PLAYER
        /// </summary>
        /// <param name="socketChannel"></param>
        /// <param name="serializableObject"></param>
        public void SendToPlayer(System.Guid socketChannel, object serializableObject)
        {
            base.Send(socketChannel, serializableObject);
        }

    }
#endif
}
