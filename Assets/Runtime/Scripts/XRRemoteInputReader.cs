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
        public Vector2 lastTouchPositionNormalized 
        {
            get
            {
                return new Vector2(lastTouchPosition.x / canvasSize.x, lastTouchPosition.y / canvasSize.y);
            }
        }
        private Vector2 canvasSize = Vector2.one;
        private bool hasBeenRead = true;

        private void Awake()
        {
            Canvas canvas = FindObjectOfType<Canvas>();

            if (canvas != null) {
                canvasSize.x = canvas.GetComponent<RectTransform>().rect.height;
                canvasSize.y = canvas.GetComponent<RectTransform>().rect.width;

                //Debug.LogError($"canvas size: {canvasSize.x} x {canvasSize.y}");
            }
        }

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
