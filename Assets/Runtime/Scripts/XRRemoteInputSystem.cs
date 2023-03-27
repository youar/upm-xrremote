using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemoteInputSystem : MonoBehaviour
    {
        public static XRRemoteInputSystem Instance { get; private set; }

        public event EventHandler OnTouch;

        public Vector2 touchPosition = Vector2.zero;

        private Vector2 canvasSize = Vector2.one;

        private void Awake()
        {
            if (Instance != null) {
                Debug.LogError($"There's more than one XRRemoteInputSystem! " + transform + " = " + Instance);
                Destroy(gameObject);
                return;
            }
            
            Instance = this;

            Canvas canvas = FindObjectOfType<Canvas>();

            if (canvas != null) {
                canvasSize.x = canvas.GetComponent<RectTransform>().rect.height;
                canvasSize.y = canvas.GetComponent<RectTransform>().rect.width;

                //Debug.LogError($"canvas size: {canvasSize.x} x {canvasSize.y}");
            }
        }

        private void OnEnable()
        {
            XREditorClient.Instance.OnInputDataReceived += XREditorClient_OnInputDataReceived;
        }

        private void OnDisable()
        {
            XREditorClient.Instance.OnInputDataReceived -= XREditorClient_OnInputDataReceived;
        }

        private void XREditorClient_OnInputDataReceived(object sender, EventArgs e)
        {
            OnTouch?.Invoke(this, EventArgs.Empty);

            if (XREditorClient.Instance.touchPositionNormalized == null) return;

            //Update touch position
            touchPosition = new Vector2(
                XREditorClient.Instance.touchPositionNormalized.x * canvasSize.x,
                XREditorClient.Instance.touchPositionNormalized.y * canvasSize.y
            );

            //Create an input event on the new Input System
            //InputSystem.QueueStateEvent(Touchscreen.current, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Began, position = touchPosition });
        }
    }
}
#endif
