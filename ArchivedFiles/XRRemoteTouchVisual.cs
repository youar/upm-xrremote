using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRRemote;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemoteTouchVisual : MonoBehaviour
    {
        private float timeToShowVisual = 0.1f;
        private float fadeScale = 5f;
        private float timeSinceVisible = float.PositiveInfinity;

        [SerializeField] private RectTransform imageRectTransform;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            XRRemoteInputSystem.Instance.OnTouch += XRRemoteInputSystem_OnTouch;
        }

        private void OnDisable()
        {
            XRRemoteInputSystem.Instance.OnTouch -= XRRemoteInputSystem_OnTouch;
        }

        private void Update()
        {
            if (timeSinceVisible > timeToShowVisual) {
                imageRectTransform.gameObject.SetActive(false);
                return;
            }

            imageRectTransform.localScale = Vector3.one * Mathf.Lerp(1f, fadeScale, timeSinceVisible / timeToShowVisual);

            timeSinceVisible += Time.deltaTime;
        }

        private void XRRemoteInputSystem_OnTouch(object sender, EventArgs e)
        {
            Vector2 touchPosition = XRRemoteInputSystem.Instance.touchPosition;

            rectTransform.anchoredPosition = new Vector2(touchPosition.x, touchPosition.y);
            imageRectTransform.localScale = Vector3.one;
            imageRectTransform.gameObject.SetActive(true);
            timeSinceVisible = 0;
        }
    }
}
#endif
