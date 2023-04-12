using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemoteInputSystem : MonoBehaviour
    {
        public static XRRemoteInputSystem Instance { get; private set; }

        public event EventHandler OnTouch;

        /// <summary>
        /// Calculated touch position using normalized position sent by device
        /// </summary>
        public Vector2 touchPosition = Vector2.zero;

        private Vector2 canvasSize = Vector2.one;

        private GraphicRaycaster m_Raycaster;
        private EventSystem m_EventSystem;
        private PointerEventData m_PointerEventData;

        private void Awake()
        {
            //Set up singleton
            if (Instance != null) {
                if (DebugFlags.displayEditorConnectionStats) {
                    Debug.LogError($"There's more than one XRRemoteInputSystem! " + transform + " = " + Instance);
                }
                Destroy(gameObject);
                return;
            }
            
            Instance = this;

            //Fetch the Raycaster from the GameObject (the Canvas)
            Canvas canvas = FindObjectOfType<Canvas>();

            if (canvas != null) {
                canvasSize.x = Screen.currentResolution.height;//canvas.GetComponent<RectTransform>().rect.height;
                canvasSize.y = Screen.currentResolution.width;//canvas.GetComponent<RectTransform>().rect.width;

                Debug.Log($"Screen size: {Screen.currentResolution.width} x {Screen.currentResolution.height}");
                Debug.Log($"Canvas size: {canvas.GetComponent<RectTransform>().rect.height} x {canvas.GetComponent<RectTransform>().rect.width}");
            }

            if (canvas == null) {
                if (DebugFlags.displayEditorConnectionStats) {
                    Debug.LogError($"XRRemoteInputSystem: Unable to find Canvas. Please make sure there is one in the scene.");
                }
                return;
            }

            //Fetch the Raycaster from the GameObject (the Canvas) and Event System from the Scene
            m_Raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
            m_EventSystem = GameObject.FindObjectOfType<EventSystem>();

            if (canvas == null) {
                if (DebugFlags.displayEditorConnectionStats) {
                    Debug.LogError($"XRRemoteInputSystem: Unable to find GraphicRaycaster or EventSystem. Please make sure there is one in the scene.");
                }
                return;
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
                XREditorClient.Instance.touchPositionNormalized.x * canvasSize.x * 2,
                XREditorClient.Instance.touchPositionNormalized.y * canvasSize.y / 2
            );

            //Create an input event on the new Input System
            //InputSystem.QueueStateEvent(Touchscreen.current, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Began, position = touchPosition });

            //Handle touch hits on UI objects
            InteractWithUiObjects();
        }

        private void InteractWithUiObjects()
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = touchPosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, check if we hit a UI element
            foreach (RaycastResult result in results)
            {
                //On button, invoke its onClick event
                if (result.gameObject.TryGetComponent(out Button button)) {
                    Debug.Log("Hit " + result.gameObject.name);
                    button.onClick?.Invoke();
                }
            }
        }
    }
}
#endif
