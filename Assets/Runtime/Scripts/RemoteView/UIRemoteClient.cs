using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class UIRemoteClient : MonoBehaviour
{
    public class TestLocationService : MonoBehaviour
    {
        public List<TMP_Text> tmpIP;
        private List<string> connectedIPs;
        
        private bool _shouldUpdateUI;
        private void Start()
        {
            connectedIPs = new List<string>();
            GetLocalIPAddress();
        }
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> ips = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    // INCOMPLETE
                    ips.Add(ip.ToString());
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
 
    }
}
