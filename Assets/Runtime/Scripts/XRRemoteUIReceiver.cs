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
            fragmentReceiver = GetComponent<FragmentReceiver>();
        }

        private void OnEnable()
        {
            fragmentReceiver.OnDataCompletelyReceived += FragmentReceiver_OnDataCompletelyReceived;
        }

        private void OnDisable()
        {
            fragmentReceiver.OnDataCompletelyReceived -= FragmentReceiver_OnDataCompletelyReceived;
        }

        public void ReceiveStartFragmentPacket(TestStartFragmentPacket startFragmentPacket)
        {
            Debug.LogError($"XRRemoteUIReceiver: received START fragment. id = {startFragmentPacket.id}, expectedLength = {startFragmentPacket.expectedLength}");
            fragmentReceiver.PrepareToReceiveBytes(startFragmentPacket.id, startFragmentPacket.expectedLength);
        }

        public void ReceiveDataFragmentPacket(TestDataFragmentPacket dataFragmentPacket)
        {
            Debug.LogError($"XRRemoteUIReceiver: received DATA fragment. id = {dataFragmentPacket.id}, dataLength = {dataFragmentPacket.data.Length}");
            fragmentReceiver.ReceiveBytes(dataFragmentPacket.id, dataFragmentPacket.data);
        }

        public void ReceiveFragmentPacket(BaseFragmentPacket fragmentPacket)
        {
            switch (fragmentPacket)
            {
                case StartFragmentPacket startFragmentPacket:
                    Debug.LogError($"XRRemoteUIReceiver: received START fragment");
                    fragmentReceiver.PrepareToReceiveBytes(startFragmentPacket.transmissionId, startFragmentPacket.expectedLength);
                    break;

                case DataFragmentPacket dataFragmentPacket:
                    Debug.LogError($"XRRemoteUIReceiver: completely DATA fragment");
                    fragmentReceiver.ReceiveBytes(dataFragmentPacket.transmissionId, dataFragmentPacket.data);
                    break;

                default:
                    Debug.LogError($"XRRemoteUIReceiver: unknown fragment type");
                    break;
            }
        }

        private void FragmentReceiver_OnDataCompletelyReceived(object sender, EventArgs e)
        {
            OnDataCompletelyReceivedEventArgs args = e as OnDataCompletelyReceivedEventArgs;

            Debug.LogError($"XRRemoteUIReceiver: completely received data {args.data.ToString()}");

            //Read data as Texture2d
            Texture2D remoteCanvasTexture = new Texture2D(100, 100, TextureFormat.RGBA32, false);
            remoteCanvasTexture.LoadImage(args.data);
            remoteCanvasTexture.Apply();

            //Show remote canvas
            XRRemoteServer.Instance.remoteCanvas.texture = remoteCanvasTexture;
            XRRemoteServer.Instance.remoteCanvas.enabled = true;
        }
    }
}
