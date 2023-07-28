//------------------------------------------------------------------------------------------------------- 
// <copyright file="SerializableVector2Int.cs" createdby="razieleron"> //  
// XR Remote // Copyright(C) 2020  YOUAR, INC. // 
// This program is free software: you can redistribute it and/or modify 
// it under the terms of the GNU Affero General Public License as published by 
// the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version. // 
// https://www.gnu.org/licenses/agpl-3.0.html // 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the 
// GNU Affero General Public License for more details. 
// You should have received a copy of the GNU Affero General Public License 
// along with this program. If not, see // <http://www.gnu.org/licenses/>. // 
// </copyright> 
//-------------------------------------------------------------------------------------------------------  

using System;
using UnityEngine;
namespace XRRemote.Serializables

{
[Serializable]
    public class SerializableVector2Int : IEquatable<SerializableVector2Int>, IFormattable
    {
        public int x;
        public int y;

        public SerializableVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public SerializableVector2Int(Vector2Int vector)
        {
            this.x = vector.x;
            this.y = vector.y;
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(x, y);
        }

        public bool Equals(SerializableVector2Int rhs) 
        { 
            return x == rhs.x && y == rhs.y; 
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Vector2Int({0}, {1})", x.ToString(format, formatProvider), y.ToString(format, formatProvider));
        }

        public Vector2Int ConvertFromSerializableVector2IntToVector2Int()
        {
            return new Vector2Int(x, y);
        }
    }
}
