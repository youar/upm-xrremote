using System;

namespace KlakNDI_Test.Assets.Scripts.ObjectSerializationExtension 
{
    [Serializable]
    public partial class RemotePacket
    {
        public CameraFrameEvent cameraFrame;
        // public FaceInfo face;
        // public Pose trackedPose;
        public PlaneInfo plane;
        // // public HumanBodyInfo humanBody;

    }

}