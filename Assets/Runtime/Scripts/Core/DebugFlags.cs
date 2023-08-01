//-------------------------------------------------------------------------------------------------------
// <copyright file="DebugFlags.cs" createdby="gblikas">
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    public static class DebugFlags
    {
        public static readonly bool globalDisplay = true;

        public static readonly bool displayCompressionStats = globalDisplay;
        public static readonly bool displayEditorConnectionStats = globalDisplay;
        public static readonly bool displayXRRemotePacketStats = globalDisplay;
        public static readonly bool displayXRRemoteConnectionStats = globalDisplay;
        public static readonly bool displayXRExtractTextureStats = globalDisplay;
        public static readonly bool displayXRRemotePlaneStats = globalDisplay;

    }
}