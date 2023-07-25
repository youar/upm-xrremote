using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class UIRemoteServer : MonoBehaviour
{
    public TMP_Text tmpTextIP;

    private void Start()
    {
        tmpTextIP.text = GetLocalIPv4();
    }

    string GetLocalIPv4()
    {
        List<IPAddress> ips = getIPAddresses().ToList();
        IPAddress preferredIP = Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        string message = preferredIP + "\n";
        foreach (IPAddress ip in ips)
        {
            if (ip.Equals(preferredIP)) continue;
            message += ip.ToString()+"\n";
        }

        string title = (ips.Count>1)?"IP addresses\n\n":"ip address\n\n";
        return title + message;
    }

    IEnumerable<IPAddress> getIPAddresses() {
            return NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(_ => _.GetIPProperties().UnicastAddresses)
                .Select(_ => _.Address)
                .Where(_ => _.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(_))
                .Distinct();
    }   
    
}
