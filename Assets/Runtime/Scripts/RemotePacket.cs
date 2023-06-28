using System;
using XRRemote;
// using System.Runtime.InteropServices;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine;
// using System.ComponentModel;

namespace KlakNDI_Test.Assets.Scripts.ObjectSerializationExtension 
{
    [Serializable]
    public partial class RemotePacket
    {
        public CameraFrameEvent cameraFrame;
        // todo make frameInfo and timestamp their own object classes
        public int frameInfo;
        public long? timestamp;
        public int bytesSent;
        // public FaceInfo face;
        // public Pose trackedPose;
        public PlanesInfo planesInfo;
        // public HumanBodyInfo humanBody;
    }


}

