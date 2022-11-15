using System;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;

public class UIRemoteServer : MonoBehaviour
{
    public TMP_Text tmpTextIP;

    private void Start()
    {
        tmpTextIP.text = GetLocalIPv4();
    }

    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }   
}
