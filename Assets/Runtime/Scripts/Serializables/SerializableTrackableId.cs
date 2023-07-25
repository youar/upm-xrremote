//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableTrackableId.cs" createdby="gblikas">
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
using System.Runtime.InteropServices;

namespace XRRemote.Serializables 
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SerializableTrackableId : IEquatable<SerializableTrackableId>
    {
        public ulong subId1;
        public ulong subId2;

        public SerializableTrackableId(ulong subId1, ulong subId2)
        {
            this.subId1 = subId1;
            this.subId2 = subId2;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = subId1.GetHashCode();
                return hash * 486187739 + subId2.GetHashCode();
            }
        }

        public bool Equals(SerializableTrackableId o)
        {
            return (subId1 == o.subId1) && (subId2 == o.subId2);
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}",
                subId1.ToString("X16"),
                subId2.ToString("X16"));
        }

        public static implicit operator UnityEngine.XR.ARSubsystems.TrackableId(SerializableTrackableId id)
        {
            return new UnityEngine.XR.ARSubsystems.TrackableId(id.subId1, id.subId2);
        }

        public static implicit operator SerializableTrackableId(UnityEngine.XR.ARSubsystems.TrackableId id)
        {
            return new SerializableTrackableId(id.subId1, id.subId2);
        }
    }
}