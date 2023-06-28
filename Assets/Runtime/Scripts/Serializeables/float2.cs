//-------------------------------------------------------------------------------------------------------
// <copyright file="float2.cs" createdby="gblikas">
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote.Serializables 
{
    [Serializable]
    public class float2 : IEquatable<float2>, IFormattable
    {
        public float x;
        public float y;

        /// <summary>float2 zero value.</summary>
        public static readonly float2 zero;

        public float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float2(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public bool Equals(float2 rhs) { return x == rhs.x && y == rhs.y; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("float2({0}f, {1}f", x.ToString(format, formatProvider), y.ToString(format, formatProvider));
        }

        public Vector3 ToVector2()
        {
            return new Vector2(x, y);
        }
    }
}

