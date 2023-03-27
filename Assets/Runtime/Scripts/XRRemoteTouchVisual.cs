using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRRemote;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemoteTouchVisual : MonoBehaviour
    {
        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            XRRemoteInputSystem.Instance.OnTouch += XRRemoteInputSystem_OnTouch;
        }

        private void XRRemoteInputSystem_OnTouch(object sender, EventArgs e)
        {
            Vector2 touchPosition = XRRemoteInputSystem.Instance.touchPosition;

            m_RectTransform.anchoredPosition = new Vector2(touchPosition.x, touchPosition.y);
        }
    }
}
#endif
