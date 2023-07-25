//-------------------------------------------------------------------------------------------------------
// <copyright file="UIRenderer.cs" createdby="gblikas">
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
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
using UnityEngine.UI;
// using XRRemote.Serializables;

namespace XRRemote
{   
    [DisallowMultipleComponent]
    public class UIRenderer : MonoBehaviour
    {
        public static UIRenderer Instance { get; private set; } = null;
        [SerializeField] private Canvas debugCanvas;
        [HideInInspector][SerializeField] private bool _debugMode;
        [SerializeField] public bool debugMode;

        private void Update()
        {
            if (_debugMode != debugMode)
            {
                _debugMode = debugMode;
                UpdateDebugUI();
            } 
        }

        private void OnDisable()
        {
            Instance = null;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("UIRenderer must be only one in the scene.");
            }

            Instance = this;
        }

        private void UpdateDebugUI()
        {
            debugCanvas.gameObject.SetActive(_debugMode);
        }
    }
}
