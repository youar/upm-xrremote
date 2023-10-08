using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRRemote;

public class PacketReceiverStats : MonoBehaviour
{
    public static PacketReceiverStats Instance { get; private set; } = null;

    private int firstPacketReceived = 100000000;
    private int lastPacketReceived = -100000000;
    private int countPacketsReceived = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("PacketReceiverStats must be only one in the scene.");
            enabled = false;
            return;
        }
        Instance = this;
    }

    public void ReceivePacket(ClientRemotePacket packet)
    {
        if (packet.packetId < firstPacketReceived) {
            firstPacketReceived = packet.packetId;
            countPacketsReceived++;
            Debug.Log($"count: {countPacketsReceived}, first: {firstPacketReceived}, last: {lastPacketReceived}");
        } else if (packet.packetId > lastPacketReceived) {
            lastPacketReceived = packet.packetId;
            countPacketsReceived++;
            Debug.Log($"count: {countPacketsReceived}, first: {firstPacketReceived}, last: {lastPacketReceived}");
        }
    }
}
