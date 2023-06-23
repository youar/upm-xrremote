using System;
// using System.Linq;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.XR.ARSubsystems;

namespace KlakNDI_Test.Assets.Scripts.ObjectSerializationExtension
{
    [Serializable]
    public class PlaneInfo
    {
        public BoundedPlane[] added;
        public BoundedPlane[] updated;
        public TrackableId[] removed;
        public Dictionary<TrackableId, byte[]> meshes;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("PlaneInfo");

            if (added != null)
            {
                foreach (var f in added)
                {
                    sb.AppendLine($"ADD: {f}");
                }
            }

            if (updated != null)
            {
                foreach (var f in updated)
                {
                    sb.AppendLine($"UPDATE: {f}");
                }
            }

            if (removed != null)
            {
                foreach (var f in removed)
                {
                    sb.AppendLine($"REMOVE: {f}");
                }
            }

            if (meshes != null)
            {
                foreach (var m in meshes)
                {
                    sb.AppendLine($"MESHED: {m}");
                }
            }

            return sb.ToString();
        }
    }

    [Serializable]
    public struct CameraFrameEvent : IEquatable<CameraFrameEvent>
    {
        public long? timestampNs;
        public float4x4 projectionMatrix;
        public float4x4 displayMatrix;

        public bool Equals(CameraFrameEvent o)
        {
            return timestampNs == o.timestampNs
                && projectionMatrix.Equals(o.projectionMatrix)
                && displayMatrix.Equals(o.displayMatrix);
        }

        public override string ToString()
        {
            return $"[time: {timestampNs?.ToString() ?? "null"}, projection: {projectionMatrix}, display: {displayMatrix}]";
        }
    }
}
