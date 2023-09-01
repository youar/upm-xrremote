//-------------------------------------------------------------------------------------------------------
// <copyright file="ObjectSerializationExtension.cs" createdby="gblikas">
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
//
// <modified url="https://bitbucket.org/Unity-Technologies/unity-arkit-plugin/src/default/" modifiedby="gblikas">
//
// </modified> 
//-------------------------------------------------------------------------------------------------------
using System.IO;

namespace XRRemote
{
    using System.Runtime.Serialization.Formatters.Binary;

    //Extension class to provide serialize / deserialize methods to object.
    //src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    //NOTE: You need add [Serializable] attribute in your class to enable serialization
    public static class ObjectSerializationExtension
	{
		public static byte[] SerializeToByteArray(this object obj)
		{
			if (obj == null)
			{
				return null;
			}
			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public static T Deserialize<T>(this byte[] byteArray) where T : class
		{
			if (byteArray == null)
			{
				return null;
			}
			using (var memStream = new MemoryStream())
			{
				var binForm = new BinaryFormatter();
				memStream.Write(byteArray, 0, byteArray.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				var obj = (T)binForm.Deserialize(memStream);
				return obj;
			}
		}
	}
}