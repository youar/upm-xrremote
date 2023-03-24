using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRRemote
{
    public class XRRemoteInputReader : MonoBehaviour
    {
        private Vector3 lastTouchPosition = Vector3.zero;
        private bool hasBeenRead = true;

        public bool TryGetLastInput(out Vector3 touchPosition)
        {
            if (hasBeenRead) {
                touchPosition = Vector3.zero;
                return false;
            }
            touchPosition = lastTouchPosition;
            hasBeenRead = true;

            return true;
        }

        // Called Unity's Input System when touch input is recieved
        public void OnTouch(InputAction.CallbackContext context)
        {
            var touchPosition = context.ReadValue<Vector2>();
            lastTouchPosition = touchPosition;
            hasBeenRead = false;
        }
    }
}
