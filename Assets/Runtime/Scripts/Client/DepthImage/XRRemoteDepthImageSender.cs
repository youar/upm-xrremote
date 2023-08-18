// //-------------------------------------------------------------------------------------------------------
// // <copyright file="XRRemoteDepthImageSender.cs" createdby="Razieleron">
// // 
// // XR Remote
// // Copyright(C) 2020  YOUAR, INC.
// //
// // This program is free software: you can redistribute it and/or modify
// // it under the terms of the GNU Affero General Public License as published by
// // the Free Software Foundation, either version 3 of the License, or
// // (at your option) any later version.
// //
// // https://www.gnu.org/licenses/agpl-3.0.html
// //
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// // GNU Affero General Public License for more details.
// // You should have received a copy of the GNU Affero General Public License
// // along with this program. If not, see
// // <http://www.gnu.org/licenses/>.
// //
// // </copyright>
// //-------------------------------------------------------------------------------------------------------
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;
// using XRRemote.Serializables;

// namespace XRRemote
// {
//     public class XRRemoteDepthImageSender : MonoBehaviour
//     {

//         [SerializeField] private readonly AROcclusionManager arOcclusionManager;

//         // private bool TryGetCpuImage(out XRCpuImage cpuImage)
//         // {
//         //     if (arOcclusionManager.TryAcquireRawEnvironmentDepthCpuImage(out cpuImage))
//         //     {
//         //         SerializableDepthImage generatedImage = new SerializableDepthImage(cpuImage);// Process the 'cpuImage' here if needed
//         //         return true;
//         //     }
//         //     return false;
//         // }

//         private void SerializeDepthImage(XRCpuImage cpuImage)
//         {
//             SerializableDepthImage image = new SerializableDepthImage(cpuImage);
//         }
//     }
// }
