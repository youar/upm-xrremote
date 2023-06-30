using System;

namespace XRRemote.Serializables 
{
    [Serializable]
    public class PlanesInfo
    {
        public XRPlaneNdi[] added;
        public XRPlaneNdi[] updated;
        public XRPlaneNdi[] removed;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("PlanesInfo");
            foreach (var f in added)
            {
                sb.AppendLine($"added: {f}");
            }
            foreach (var f in updated)
            {
                sb.AppendLine($"updated: {f}");
            }
            foreach (var f in removed)
            {
                sb.AppendLine($"removed: {f}");
            }
            return sb.ToString();
        }
    }
}
