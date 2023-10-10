using System.Globalization;
//-------------------------------------------------------------------------------------------------------
// <copyright file="OcclusionHelper.cs" createdby="Razieleron">
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
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    public static class OcclusionHelper
    {
        public static List<GameObject> occlusionGameObjects = null;
        public static Dictionary<GameObject, Renderer> occlusionRenderers = null;

        
        /// <summary>
        /// Populate the list of GameObjects in the XRRemote-Occlusion layer
        /// </summary>
        public static void PopulateOcclusionGameObjectList()
        {           
            if (occlusionGameObjects == null || occlusionGameObjects.Count == 0)
            {
                occlusionGameObjects = new List<GameObject>();
                foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                {
                    if (go.layer == LayerMask.NameToLayer("XRRemote-Occlusion"))
                    {
                        occlusionGameObjects.Add(go);
                    }
                }
            }
        }

    /// <summary>
    /// Populate the dictionary of GameObjects and their Renderers in the XRRemote-Occlusion layer
    /// </summary>
        public static void PopulateOcclusionRenderersDict()
        {
            if (occlusionRenderers == null || occlusionRenderers.Count == 0)
            {
                occlusionRenderers = new Dictionary<GameObject, Renderer>();
                foreach (GameObject go in occlusionGameObjects)
                {
                    Renderer renderer = go.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        occlusionRenderers.Add(go, renderer);
                    }
                }
            }
        }

        /// <summary>
        /// Update the Occlusion Material on Visible GameObjects in the XRRemote-Occlusion layer
        /// </summary>
        /// <param name="maxDepthValue"></param>
        /// <param name="occlusionMaterial"></param>
        /// <param name="depthTexture"></param>
        /// <param name="planes"></param>
        public static void UpdateOcclusionMaterialOnRenderers(float maxDepthValue, Material occlusionMaterial, Texture2D depthTexture, Plane[] planes)
        {

            foreach (KeyValuePair<GameObject, Renderer> kvp in occlusionRenderers)
            {
                if (GeometryUtility.TestPlanesAABB(planes, kvp.Value.bounds))
                {
                    SetMaterialProperties(kvp, maxDepthValue, occlusionMaterial, depthTexture);
                }
            }
        }

        /// <summary>
        /// Updates the Occlusion Material on GameObjects
        /// </summary>
        /// <param name="kvp"></param>
        /// <param name="maxDepthValue"></param>
        /// <param name="occlusionMaterial"></param>
        /// <param name="depthTexture"></param>
        private static void SetMaterialProperties(KeyValuePair<GameObject, Renderer> kvp, float maxDepthValue, Material occlusionMaterial, Texture2D depthTexture)
        {
            Renderer renderer = kvp.Value;
            if (renderer != null)
            {
                if (renderer.material == null)
                {
                    renderer.material = occlusionMaterial;
                }
                renderer.material.SetTexture("_MainTex", depthTexture);
                renderer.material.SetFloat("_MaxDistance", maxDepthValue); 
            }
        }
    }
}