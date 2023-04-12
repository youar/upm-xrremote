using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    [Serializable]
    public abstract class BaseFragmentPacket 
    {
        public int transmissionId;
    }

    [Serializable]
    public class StartFragmentPacket : BaseFragmentPacket
    {
        public int expectedLength;
    }

    [Serializable]
    public class DataFragmentPacket : BaseFragmentPacket
    {
        public int index;
        public byte[] data;
    }

    [Serializable]
    public class TestFragmentPacket
    {
        public int id;
    }

    [Serializable]
    public class TestStartFragmentPacket
    {
        public int id;
        public int expectedLength;
    }

    [Serializable]
    public class TestDataFragmentPacket
    {
        public int id;
        public int index;
        public byte[] data = null;
    }

#if UNITY_EDITOR
    public class FragmentSender : MonoBehaviour
    {
        private static int defaultBufferSize = 512;
        private static float timeBetweenSendingFragment = 0.2f;

        private int transmissionId;
        private byte[] data;
        private int currentIndex;
        private bool isSending;

        public event EventHandler OnDataFragmentSent;
        public event EventHandler OnDataComepletelySent;


        public void SendBytesToClient(int transmissionId, byte[] data)
        {
            //Check if already currently sending data
            if (isSending) {
                if (DebugFlags.displayXRFragmentSender) {
                    Debug.LogErrorFormat("FragmentSender: Already sending data");
                }
                return;
            }

            //Start sending data
            StartCoroutine(SendBytesToClientsRoutine(transmissionId, data));
        }

        private IEnumerator SendBytesToClientsRoutine(int newId, byte[] dataToSend)
        {
            //Start sending
            transmissionId = newId;
            data = dataToSend;
            currentIndex = 0;
            isSending = true;

            //Tell client that it is going to receive some data and tell it how much it will be.
            // XREditorClient.Instance.Send(
            //     new StartFragmentPacket {
            //         transmissionId = transmissionId,
            //         expectedLength = data.Length
            //     }
            // );
            Debug.Log($"SendBytesToClientsRoutine: Start message: id = {newId}, dataLength = {data.Length}");
             //XREditorClient.Instance.Send(new TestFragmentPacket {id = 1}); 
             XREditorClient.Instance.Send(new TestStartFragmentPacket {id = newId, expectedLength = data.Length}); 
            yield return new WaitForSeconds(timeBetweenSendingFragment);

            //Transmit data in chunks of bufferSize
            int bufferSize = defaultBufferSize;
            while(currentIndex < data.Length - 1) {
                //Determine the remaining amount of bytes, still need to be sent.
                int remaining = data.Length - currentIndex;
                if (remaining < bufferSize) {
                    bufferSize = remaining;
                }

                //Prepare the chunk of data which will be sent in this iteration
                byte[] buffer = new byte[bufferSize];
                System.Array.Copy(data, currentIndex, buffer, 0, bufferSize);

                //Send the chunk
                // XREditorClient.Instance.Send(
                //     new DataFragmentPacket {
                //         transmissionId = transmissionId,
                //         index = currentIndex,
                //         data = buffer
                //     }
                // );
                Debug.Log($"SendBytesToClientsRoutine: Send message: id = {currentIndex}");
                //XREditorClient.Instance.Send(new TestFragmentPacket {id = currentIndex});
                XREditorClient.Instance.Send(new TestDataFragmentPacket {id = newId, index = currentIndex, data = buffer});
                currentIndex += bufferSize;

                
                yield return new WaitForSeconds(timeBetweenSendingFragment);

                //Fragment sent
                OnDataFragmentSent?.Invoke(this, EventArgs.Empty);
            }

            //Complete send
            transmissionId = -1;
            Array.Clear(data, 0, data.Length);
            isSending = false;

            //Transmission complete
            OnDataComepletelySent?.Invoke(this, EventArgs.Empty);
        }
    }
#endif
}
