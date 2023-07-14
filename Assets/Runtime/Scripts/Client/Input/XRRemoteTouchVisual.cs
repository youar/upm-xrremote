//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteTouchVisual.cs" createdby="gblikas">
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
using XRRemote;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemoteTouchVisual : MonoBehaviour
    {
        private float timeToShowVisual = 0.1f;
        private float fadeScale = 10f;
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

            rectTransform.SetAsLastSibling();
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
