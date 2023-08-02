//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableFloat4.cs" createdby="gblikas">
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
    public class SerializableFloat4 : IEquatable<SerializableFloat4>, IFormattable
    {
        public float x;
        public float y;
        public float z;
        public float w;

        /// <summary>float3 zero value.</summary>
        public static readonly SerializableFloat4 zero;

        public SerializableFloat4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool Equals(SerializableFloat4 rhs) { return x == rhs.x && y == rhs.y && z == rhs.z && w == rhs.w; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("float3({0}f, {1}f, {2}f), {3}f", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
        }
    }
}
