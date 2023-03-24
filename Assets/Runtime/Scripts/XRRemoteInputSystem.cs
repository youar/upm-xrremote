using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemoteInputSystem : MonoBehaviour
    {
        public event EventHandler OnTouch;

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
            Debug.Log("Input happened! " + XREditorClient.Instance.touchPosition);
            OnTouch?.Invoke(this, EventArgs.Empty);
        }
    }
}
#endif
