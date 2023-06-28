using System;
using System.Linq;
using UnityEngine;
using Klak.Ndi;
using XRRemote;

public class CustomNdiReceiver : MonoBehaviour
{
    [SerializeField] 
    private NdiResources resources = null;
    private NdiReceiver ndiReceiver = null;
    public CustomRawImage rawImage = null;

    public static CustomNdiReceiver Instance { get; private set; } = null;
    public RemotePacket remotePacket { get; private set; } = null;
    public event EventHandler OnPlanesInfoReceived;

    private void Awake()
    {
        // It works only in Editor!
        if (!Application.isEditor)
        {
            Destroy(gameObject);
            Debug.LogError("cannot use CustomNdiReceiver in Editor.");
            return;
        }

        if (Instance != null)
        {
            Debug.LogError("CustomNdiReceiver must be only one in the scene.");
        }

        Instance = this;
    }

    private void Start()
    {
        ndiReceiver = gameObject.AddComponent<NdiReceiver>();
        ndiReceiver.SetResources(resources);
        var ndiName = FindNdiName();
        if (!string.IsNullOrWhiteSpace(ndiName))
        {
            ndiReceiver.ndiName = ndiName;
        }
    }

    private void OnDisable()
    {
        Instance = null;
    }

    private void Update()
    {
        var rt = ndiReceiver.texture;
        if (rt == null)
        {
            var ndiName = FindNdiName();
            if (!string.IsNullOrWhiteSpace(ndiName) && ndiReceiver.ndiName != ndiName)
            {
                ndiReceiver.ndiName = ndiName;
            }
        }
        else
        {
            //add texture to rawImage
            rawImage.texture = rt;
            
            // //check metadata
            if (ndiReceiver.metadata == null)
            {
                return;
            }
            //add Metadata here
            string base64 = ndiReceiver.metadata.Substring(9, ndiReceiver.metadata.Length - 9 - 3);
            byte[] data = Convert.FromBase64String(base64);

            RemotePacket receivedData = ObjectSerializationExtension.Deserialize<RemotePacket>(data); 
            CustomNdiReceiver.Instance.remotePacket = receivedData;

            //check and add planes info
            if (receivedData.planesInfo != null) 
            {
                OnPlanesInfoReceived?.Invoke(this, EventArgs.Empty);
            }
            
            ndiReceiver.metadata = null;
        }
    }

    private static string FindNdiName()
    {
        return NdiFinder.sourceNames.FirstOrDefault();
    }
}
