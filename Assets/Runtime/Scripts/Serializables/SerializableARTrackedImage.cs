//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableARTrackedImage.cs" createdby="csustrich">
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
// using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace XRRemote.Serializables 
{
    [Serializable]
    // [StructLayout(LayoutKind.Sequential)]
    public struct SerializableARTrackedImage : IEquatable<SerializableARTrackedImage>
    {
        public SerializableFloat2 extents;
        public SerializableFloat2 size;
        public SerializableTrackableId trackableId;
        public SerializablePose pose;
        public SerializableXRReferenceImage referenceImage;
        public int trackingState;
        public string name;

        public SerializableARTrackedImage(ARTrackedImage arImage)
        {
            //constructor bro
            //guid ?? how to associate with original image on client
            name = arImage.name;  //maybe arImage.referenceImage.name??
            extents = new SerializableFloat2(arImage.extents);
            size = new SerializableFloat2(arImage.size);
            pose = SerializablePose.FromTransform(arImage.transform);
            trackableId = arImage.trackableId;
            trackingState = (int)arImage.trackingState;
            referenceImage = new SerializableXRReferenceImage(arImage.referenceImage);
        }

        public bool Equals(SerializableARTrackedImage o)
        {
            //prob don't need dis either. just compare IDs
            return trackableId.Equals(o.trackableId);
        }

        // public override string ToString()
        // {
        //   do I need dis??
        // }
    }
}
