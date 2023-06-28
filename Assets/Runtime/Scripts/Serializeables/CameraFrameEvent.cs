using System;
using UnityEngine;

namespace XRRemote 
{
    [Serializable]
    public class CameraFrameEvent : IEquatable<CameraFrameEvent>
    {
        // public ARLightEstimationData lightEstimation;
        public long timestampNs;
        public XRRemote.Pose projectionMatrix;
        public XRRemote.Pose displayMatrix;

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