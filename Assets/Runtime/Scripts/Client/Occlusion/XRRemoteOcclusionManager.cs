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
using System.Linq;
using System.Collections.Generic;
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
        public Camera mainCamera = null;
        public Plane[] planes = null;   
        

        private void OnEnable()
        {
            if (ClientReceiver.Instance == null) return;

            GameObject arCameraGO = GameObject.Find("AR Camera");
            mainCamera = arCameraGO != null ? arCameraGO.GetComponent<Camera>() : Camera.main;

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
        /// Checks if any of the GameObjects in the Occlusion Layer are visible
        /// </summary>
        /// <returns></returns>
        private bool GameObjectVisible()
            {
            return OcclusionHelper.occlusionRenderers.Any(kvp => GeometryUtility.TestPlanesAABB(planes, kvp.Value.bounds));
            }

        /// <summary>
        /// Sets the visibility of GameObjects in the Occlusion Layer
        /// </summary>
        private void SetGameObjectVisibility()
            {
                foreach (KeyValuePair<GameObject, Renderer> kvp in OcclusionHelper.occlusionRenderers)
                {
                    kvp.Value.enabled = GeometryUtility.TestPlanesAABB(planes, kvp.Value.bounds);
                }
            }

        /// <summary>
        /// Takes raw DepthTexture info from the received packet, Checks for the visibility of GameObjects and if they are visible,
        /// generates the depthTexture and applies it to the occlusionMaterial
        /// </summary>
        /// <param name="img"></param>
        private void PopulateAndApplyDepthTextureInfo(SerializableDepthImage img)
        {
            if (img.texData == null) return;
            if (depthTexture == null && img.width > 0 && img.height > 0)
            {
                depthTexture = new Texture2D(img.width, img.height, TextureFormat.RFloat, false);
            }

            //defines camera frustum planes
            planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            
            SetGameObjectVisibility();

            if (GameObjectVisible()) 
            {
                TextureHelper.PopulateTexture2DFromRBytes(depthTexture, img.texData, out var maxDepthValue);
                OcclusionHelper.UpdateOcclusionMaterialOnRenderers(maxDepthValue, occlusionMaterial, depthTexture, planes);
            }
        }

        public void CustomNdiReceiver_OnDepthImageInfoReceived(object sender, EventArgs e)
        {
            PopulateAndApplyDepthTextureInfo(ClientReceiver.Instance.remotePacket.depthImage);
        }
    }
}
#endif
