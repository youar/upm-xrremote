using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    public class XRRemoteUIReceiver : MonoBehaviour
    {
        private FragmentReceiver fragmentReceiver;

        private void Awake()
        {
            if (TryGetComponent<FragmentReceiver>(out FragmentReceiver fragmentReceiver)) {
                this.fragmentReceiver = fragmentReceiver;
            } else {
                if (DebugFlags.displayXRFragmentSender) {
                    Debug.LogWarningFormat("XRRemoteUIReceiver: required FragmentReceiver component not found.");
                }
            }
        }

        private void OnEnable()
        {
            fragmentReceiver.OnDataCompletelyReceived += FragmentReceiver_OnDataCompletelyReceived;
        }

        private void OnDisable()
        {
            fragmentReceiver.OnDataCompletelyReceived -= FragmentReceiver_OnDataCompletelyReceived;
        }

        public void ReceiveStartFragmentPacket(StartFragmentPacket startFragmentPacket)
        {
            //Debug.LogError($"XRRemoteUIReceiver: received START fragment. id = {startFragmentPacket.id}, expectedLength = {startFragmentPacket.expectedLength}");
            fragmentReceiver.PrepareToReceiveBytes(startFragmentPacket.id, startFragmentPacket.expectedLength);
        }

        public void ReceiveDataFragmentPacket(DataFragmentPacket dataFragmentPacket)
        {
            //Debug.LogError($"XRRemoteUIReceiver: received DATA fragment. id = {dataFragmentPacket.id}, dataLength = {dataFragmentPacket.data.Length}");
            fragmentReceiver.ReceiveBytes(dataFragmentPacket.id, dataFragmentPacket.data);
        }

        private void SendTextureToCanvas(OnDataCompletelyReceivedEventArgs args)
        {
            //Read data as Texture2d
            Texture2D remoteCanvasTexture = new Texture2D(100, 100, TextureFormat.RGBA32, false);
            remoteCanvasTexture.LoadImage(args.data);
            remoteCanvasTexture.Apply();

            //Show remote canvas
            XRRemoteServer.Instance.remoteCanvas.texture = remoteCanvasTexture;
            XRRemoteServer.Instance.remoteCanvas.enabled = true;
        }

        private void FragmentReceiver_OnDataCompletelyReceived(object sender, EventArgs e)
        {
            SendTextureToCanvas(e as OnDataCompletelyReceivedEventArgs);
            //Debug.LogError($"XRRemoteUIReceiver: completely received data");
        }
    }
}
