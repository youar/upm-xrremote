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
    public event EventHandler OnDebugModeChanged;
    [SerializeField] private Canvas debugCanvas;

    [HideInInspector][SerializeField] private bool _debugMode;
    [SerializeField] public bool debugMode;

    private void Update()
    {
        if (_debugMode != debugMode)
        {
            _debugMode = debugMode;
            OnDebugModeChanged?.Invoke(this, EventArgs.Empty);
        } 
    }

    private void Awake()
    {
        if (UIRenderer.Instance != null)
        {
            Debug.LogError("UIRenderer must be only one in the scene.");
            // need to destroy one or return here??
        }

        UIRenderer.Instance = this;
    }

    private void Start()
    {
        OnDebugModeChanged += UIRenderer_OnDebugModeChanged;
        UIRenderer_OnDebugModeChanged(this, EventArgs.Empty);
    }

    private void OnDisable()
    {
        OnDebugModeChanged -= UIRenderer_OnDebugModeChanged;
    }

    private void UIRenderer_OnDebugModeChanged(object sender, EventArgs e)
    {
        debugCanvas.gameObject.SetActive(_debugMode);
    }
}
}
