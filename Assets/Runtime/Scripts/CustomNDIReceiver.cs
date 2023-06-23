using System;
using System.Linq;
using UnityEngine;
using Klak.Ndi;

using KlakNDI_Test.Assets.Scripts.ObjectSerializationExtension;

public class CustomNdiReceiver : MonoBehaviour
{
    [SerializeField] 
    private NdiResources resources = null;

    private NdiReceiver ndiReceiver = null;

    public UnityEngine.UI.RawImage rawImage = null;
    public static CustomNdiReceiver Instance { get; private set; } = null;
    public RemotePacket remotePacket { get; private set; } = null;

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
            ndiReceiver.metadata = null;
        }
    }

    private void Release(RenderTexture tex)
    {
        if (tex == null)
        {
            return;
        }

        tex.Release();
        Destroy(tex);
    }

    private void Release(Texture tex)
    {
        if (tex == null)
        {
            return;
        }

        Destroy(tex);
    }

    private void InitTexture(Texture source)
    {
        int width = source.width;
        int height = source.height / 2;

        var renderTexFormat = new RenderTextureFormat[]
        {
            RenderTextureFormat.R8, // Camera Y
            RenderTextureFormat.RG16, // Camera CbCr
            RenderTextureFormat.R8, // Stencil
            RenderTextureFormat.RHalf, // Depth
        };
        var texFormat = new TextureFormat[]
        {
            TextureFormat.R8,
            TextureFormat.RG16,
            TextureFormat.R8,
            TextureFormat.RHalf,
        };
    }

    private static string FindNdiName()
    {
        return NdiFinder.sourceNames.FirstOrDefault();
    }
}
