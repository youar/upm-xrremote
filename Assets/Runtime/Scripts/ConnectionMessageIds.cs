//-------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageIds.cs" createdby="gblikas">
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

namespace XRRemote
{
	public static class ConnectionMessageIds
	{
		public static Guid fromEditorARKitSessionMsgId { get { return new Guid("523bb5dd-163b-4e5b-9271-d18a50e8897e"); } }
        public static Guid readyForFrameEventMsgId { get { return new Guid("f32fdde8-916d-4304-80aa-3d9a49b88096"); } }
        public static Guid updateCameraFrameMsgId { get { return new Guid("6d8c39bf-279a-46cf-91f4-9827a44443af"); } }
        public static Guid updateSerializableRenderTextureFrameMsgId { get { return new Guid("84d5ad8d-e7f9-432c-ae5d-40717790a12f"); } }


        public static Guid editorInitARKit { get { return new Guid("2e5d7c45-daef-474d-bf55-1f02f0a10b69"); } }
        public static Guid editorInitARKitFaceTracking { get { return new Guid("3e86ccf6-93c6-4b07-b78f-0a60f6ed4a7a"); } }


        public static Guid updateTrackedPoseDriverMsgId { get { return new Guid("a435cdb9-fa85-4d3c-9d3f-57fa85f62da3"); } }
		public static Guid removePlaneAnchorMsgeId { get { return new Guid("b07750a2-8825-4e86-9483-0b22b07df800"); } }
		public static Guid screenCaptureYMsgId { get { return new Guid("25c3d26f-72c5-4f3e-9a1f-c8c9b859453b"); } }
		public static Guid screenCaptureUVMsgId { get { return new Guid("d7f4d3cd-2d12-4ab7-b755-932fe7ab744d"); } }
		public static Guid addFaceAnchorMsgeId { get { return new Guid("7d7531e9-28b8-40b3-9afd-b6e7baa8e630"); } }
		public static Guid updateFaceAnchorMsgeId { get { return new Guid("80880c6e-d3f5-449a-9c8b-55c95b188563"); } }
		public static Guid removeFaceAnchorMsgeId { get { return new Guid("ba429c59-067e-4548-ab01-d7129f060872"); } }

        public static class TestingIds
        {
            public static readonly Guid kMsgSendEditorToPlayer = new Guid("34d9b47f923142ff847c0d1f8b0554d9");
            public static readonly Guid kMsgSendPlayerToEditor = new Guid("12871ffeaf0c489189579946d8e0840f");
        }
    };

    
}