using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    public class OnDataCompletelyReceivedEventArgs : EventArgs
    {
        public byte[] data;
    }

    public class FragmentReceiver : MonoBehaviour
    {
        private int currentTransmissionId;
        private int currentDataIndex = 0;
        private byte[] dataReceived;

        private int currentPacketCount = 0;

        public event EventHandler OnDataFragmentReceived;
        public event EventHandler OnDataCompletelyReceived;

        public void PrepareToReceiveBytes(int transmissionId, int expectedSize)
        {
            //Prepare data array which will be filled chunk by chunk by the received data
            currentTransmissionId = transmissionId;
            dataReceived = new byte[expectedSize];
            currentDataIndex = 0;
            currentPacketCount = 0;
        }

        public void ReceiveBytes(int transmissionId, byte[] recBuffer)
        {
            //Make sure receiving data from current transmittion
            if (transmissionId != currentTransmissionId) {
                if (DebugFlags.displayXRFragmentSender) {
                    Debug.LogErrorFormat("FragmentReciever: recieved data for id:{0} but expecting data for id:{1}", transmissionId, currentTransmissionId);
                }
                return;
            }

            //copy received data into prepared array and remember current dataposition
            System.Array.Copy(recBuffer, 0, dataReceived, currentDataIndex, recBuffer.Length);
            currentDataIndex += recBuffer.Length;

            currentPacketCount++;
            OnDataFragmentReceived?.Invoke(this, EventArgs.Empty);

            //Check if completely received data
            if (currentDataIndex < dataReceived.Length - 1) {
                return;
            }

            OnDataCompletelyReceived?.Invoke(this, 
                new OnDataCompletelyReceivedEventArgs {
                    data = dataReceived
                }
            );
        }
        
    }
}
