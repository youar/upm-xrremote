//-------------------------------------------------------------------------------------------------------
// <copyright file="ServerRemotePacket.cs" createdby="gblikas">
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
using XRRemote.Serializables;

namespace XRRemote 
{
    [Serializable]
    public partial class ServerRemotePacket : RemotePacket
    {
        public SerializablePose cameraPose = null;
        public SerializablePlanesInfo planesInfo;
        public SerializableFloat2 touchPositionNormalized = null;
        public SerializableXRCameraIntrinsics cameraIntrinsics = null;
    }
}
