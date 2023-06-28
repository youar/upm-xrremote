using System;
using XRRemote.Serializables;

namespace XRRemote 
{
    [Serializable]
    public partial class RemotePacket
    {
        public CameraFrameEvent cameraFrame;
        public PlanesInfo planesInfo;
        // todo make frameInfo and timestamp their own object classes
        public int frameInfo;
        public long? timestamp;
        public int bytesSent;
    }
}

