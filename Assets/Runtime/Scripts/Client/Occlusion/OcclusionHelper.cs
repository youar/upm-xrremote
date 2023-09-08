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
using UnityEngine;

namespace XRRemote
{
    public static class OcclusionHelper
    {

        public static void UpdateOcclusionMaterialOnGameObjects(float maxDepthValue, Material occlusionMaterial, Texture2D depthTexture)
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.layer == LayerMask.NameToLayer("XRRemote-Occlusion"))
                {
                    SetMaterialProperties(go, maxDepthValue, occlusionMaterial, depthTexture);
                }
            }
        }
        private static void SetMaterialProperties(GameObject go, float maxDepthValue, Material occlusionMaterial, Texture2D depthTexture)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = occlusionMaterial;
                renderer.material.SetTexture("_MainTex", depthTexture);
                renderer.material.SetFloat("_MaxDistance", maxDepthValue); 
            }
        }
    }
}