//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteInputReader.cs" createdby="gblikas">
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRRemote.Input
{
    public class XRRemoteInputReader : MonoBehaviour
    {
        public Vector2 LastTouchPositionNormalized 
        {
            get
            {
                return new Vector2(lastTouchPosition.x / canvasSize.x, lastTouchPosition.y / canvasSize.y);
            }
        }

        private Vector2 lastTouchPosition = Vector2.zero;
        private Vector2 canvasSize = Vector2.one;
        private bool hasBeenRead = true;

        private void Awake()
        {
            canvasSize.x = Screen.width;
            canvasSize.y = Screen.height;
            // Debug.LogError($"canvas size: {canvasSize.x} x {canvasSize.y}");
        }

        public bool TryGetLastInput(out Vector2 touchPosition)
        {
            if (hasBeenRead) {
                touchPosition = Vector2.zero;
                return false;
            }
            touchPosition = lastTouchPosition;
            hasBeenRead = true;

            return true;
        }

        public bool TryGetLastInputNormalized(out Vector2 touchPositionNormalized)
        {
            if (hasBeenRead) {
                touchPositionNormalized = Vector2.zero;
                return false;
            }
            touchPositionNormalized = LastTouchPositionNormalized;
            hasBeenRead = true;

            return true;
        }

        // Called Unity's Input System when touch input is recieved
        public void OnTouch(InputAction.CallbackContext context)
        {
            if (! context.performed) return;

            var touchPosition = context.ReadValue<Vector2>();
            lastTouchPosition = touchPosition;
            hasBeenRead = false;
        }
    }
}
