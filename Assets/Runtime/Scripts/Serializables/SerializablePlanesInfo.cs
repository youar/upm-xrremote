//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializablePlanesInfo.cs" createdby="gblikas">
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
using System;

namespace XRRemote.Serializables 
{
    [Serializable]
    public class SerializablePlanesInfo
    {
        public SerializableXRPlaneNdi[] added;
        public SerializableXRPlaneNdi[] updated;
        public SerializableXRPlaneNdi[] removed;

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