using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemoteInputSystem : MonoBehaviour
    {
        public static XRRemoteInputSystem Instance { get; private set; }

        public event EventHandler OnTouch;

        /// <summary>
        /// Calculated touch position using normalized position sent by device from the bottom-left corner of the screen
        /// </summary>
        public Vector2 touchPosition = Vector2.zero;

        private Vector2 canvasSize = Vector2.one;

        private GraphicRaycaster raycaster;
        private EventSystem eventSystem;
        private PointerEventData pointerEventData;
        private CustomRawImage customRawImage;

        private void Awake()
        {
            //Set up singleton
            if (Instance != null) {
                // if (DebugFlags.displayEditorConnectionStats) {
                //     Debug.LogError($"There's more than one XRRemoteInputSystem! " + transform + " = " + Instance);
                // }
                Destroy(gameObject);
                return;
            }
            Instance = this;

            customRawImage = FindObjectOfType<CustomRawImage>();
            // if (m_Raycaster == null) {
            //     if (DebugFlags.displayEditorConnectionStats) {
            //         Debug.LogError($"XRRemoteInputSystem: Unable to find GraphicRaycaster or EventSystem. Please make sure there is one in the scene.");
            //     }
            //     return;
            // }

            raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
            // if (m_Raycaster == null) {
            //     if (DebugFlags.displayEditorConnectionStats) {
            //         Debug.LogError($"XRRemoteInputSystem: Unable to find GraphicRaycaster or EventSystem. Please make sure there is one in the scene.");
            //     }
            //     return;
            // }

            eventSystem = GameObject.FindObjectOfType<EventSystem>();
            // if (m_Raycaster == null) {
            //     if (DebugFlags.displayEditorConnectionStats) {
            //         Debug.LogError($"XRRemoteInputSystem: Unable to find GraphicRaycaster or EventSystem. Please make sure there is one in the scene.");
            //     }
            //     return;
            // }
        }

        private void Start()
        {
            XRRemoteTouchVisual touchVisual = FindObjectOfType<XRRemoteTouchVisual>(true);
            if (touchVisual != null) {
                touchVisual.gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            ClientReceiver.Instance.OnInputDataReceived += ClientReceiver_OnInputDataReceived;
        }

        private void OnDisable()
        {
            if (ClientReceiver.Instance == null) return;
            ClientReceiver.Instance.OnInputDataReceived -= ClientReceiver_OnInputDataReceived;
        }

        private void ClientReceiver_OnInputDataReceived(object sender, EventArgs e)
        {
            OnTouch?.Invoke(this, EventArgs.Empty);

            if (ClientReceiver.Instance.remotePacket.touchPositionNormalized == null) return;

            UpdateCanvasSize();

            //Update touch position
            touchPosition = new Vector2(
                ClientReceiver.Instance.remotePacket.touchPositionNormalized.x * canvasSize.x,
                ClientReceiver.Instance.remotePacket.touchPositionNormalized.y * canvasSize.y
            );

            //Handle touch hits on UI objects
            InteractWithUiObjects();
        }

        private void UpdateCanvasSize()
        {
            canvasSize.x = customRawImage.rectTransform.rect.width;
            canvasSize.y = customRawImage.rectTransform.rect.height;
        }

        private void InteractWithUiObjects()
        {
            //Set up the new Pointer Event
            pointerEventData = new PointerEventData(eventSystem);

            //Set the Pointer Event Position to that of the mouse position
            pointerEventData.position = touchPosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            raycaster.Raycast(pointerEventData, results);

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
