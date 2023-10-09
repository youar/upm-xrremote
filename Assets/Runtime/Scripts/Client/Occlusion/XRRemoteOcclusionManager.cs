//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteOcclusionManager.cs" createdby="Razieleron">
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
using UnityEngine;
using XRRemote.Serializables;

#if UNITY_EDITOR
namespace XRRemote
{
    /// <summary>
    /// Manages the interpretation and application of XR Occlusion Data
    /// </summary>
    public class XRRemoteOcclusionManager : MonoBehaviour
    {

        public Texture2D depthTexture = null;
        [Tooltip("The Occlusion capable Material that will be applied to objects in the XRRemote-Occlusion layer")]
        public Material occlusionMaterial;

        private void OnEnable()
        {
            if (ClientReceiver.Instance == null) return;

            OcclusionHelper.PopulateOcclusionGameObjectList();
            OcclusionHelper.PopulateOcclusionRenderersDict();            
            ClientReceiver.Instance.OnDepthImageInfoReceived += CustomNdiReceiver_OnDepthImageInfoReceived;
        }

        private void OnDisable()
        {
            if (ClientReceiver.Instance == null) return;
            ClientReceiver.Instance.OnDepthImageInfoReceived -= CustomNdiReceiver_OnDepthImageInfoReceived;
        }

        /// <summary>
        /// Takes raw DepthTexture info from the received packet and applies it to the depthTexture and occlusionMaterial
        /// </summary>
        /// <param name="img"></param>
        private void PopulateAndApplyDepthTextureInfo(SerializableDepthImage img)
        {
            if (img.texData == null) return;
            if (depthTexture == null && img.width > 0 && img.height > 0)
            {
                depthTexture = new Texture2D(img.width, img.height, TextureFormat.RFloat, false);
            }

            TextureHelper.PopulateTexture2DFromRBytes(depthTexture, img.texData, out var maxDepthValue);
            OcclusionHelper.UpdateOcclusionMaterialOnRenderers(maxDepthValue, occlusionMaterial, depthTexture);
        }

        public void CustomNdiReceiver_OnDepthImageInfoReceived(object sender, EventArgs e)
        {
            PopulateAndApplyDepthTextureInfo(ClientReceiver.Instance.remotePacket.depthImage);
        }
    }
}
#endif