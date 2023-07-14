//-------------------------------------------------------------------------------------------------------
// <copyright file="ServerReceiver.cs" createdby="gblikas">
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
using System.Linq;
using System.Collections;
using UnityEngine;
using Klak.Ndi;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR;
using XRRemote;

namespace XRRemote
{
    public class ServerReceiver : CustomNdiReceiver
    {
        [SerializeField] 
        public ClientRemotePacket remotePacket {get; private set;} = null;
        public static ServerReceiver Instance {get; private set;} = null;
        public Text testText;
        public Text receiverNameText;

     
        private void Awake()
        {
            if (Application.isEditor)
            {
                Destroy(gameObject);
                Debug.LogError("cannot use ServerReceiver in Editor.");
                return;
            }

            ServerReceiver.Instance = this;

            targetNdiSenderName = "ClientSender";
        }

        protected override void Start()
        {
            base.Start();
            Debug.Log("ServerReceiver Start");
            //Add subscriptions hereee??!?
        }

        private void OnDisable()
        {
            Instance = null;
            //Remove subscriptions hereee??!?
        }

        protected override void ProcessPacketData(byte[] bytes) 
        {
            ClientRemotePacket remotePacket = ObjectSerializationExtension.Deserialize<ClientRemotePacket>(bytes);
            this.remotePacket = remotePacket;
            //Do other things with packet data here, UI Display method
            receiverNameText.text = ndiReceiver.ndiName;
            testText.text = remotePacket.testNumber.ToString();
        }


    }
}