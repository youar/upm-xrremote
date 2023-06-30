using System;

namespace XRRemote.Serializables 
{
    [Serializable]
    public class CameraFrameEvent : IEquatable<CameraFrameEvent>
    {
        // public ARLightEstimationData lightEstimation;
        public long timestampNs;
        public Pose projectionMatrix;
        public Pose displayMatrix;

        public bool Equals(CameraFrameEvent o)
        {
            return timestampNs.Equals(o.timestampNs)
                && projectionMatrix.Equals(o.projectionMatrix)
                && displayMatrix.Equals(o.displayMatrix);
        }

        public override string ToString()
        {
            return $"[time: {timestampNs}, projection: {projectionMatrix}, display: {displayMatrix}]";
        }

        // public static int DataSize => sizeof(long) + Marshal.SizeOf(typeof(Matrix4x4)) * 2;
    }
}
