            
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XRRemote.Serializables;
using System;

namespace XRRemote
{
  public class XRRemoteDepthImageSender : MonoBehaviour
  {
    private SerializableDepthImage xrDepthImage = null;
    private AROcclusionManager occlusionManager;
    private RawImage rawImage = null;

    Texture2D texture = null;

    private void Awake()
    {
      occlusionManager = FindObjectOfType<AROcclusionManager>();

      if(occlusionManager == null) {
        UnityEngine.Debug.LogError($"XRRemoteDepthImageSender: Unable to find AROcclusionManager. Please make sure there is one in the scene.");
      }

      return;
    }

    public bool TryGetDepthImage(out SerializableDepthImage depthImage)
    {
        if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage xrCpuImage))
        {
            var byteArray = UpdateToXRCpuImage(xrCpuImage).GetRawTextureData();
            depthImage = new SerializableDepthImage(xrCpuImage, byteArray);
            return true;
        }
        depthImage = null;
        return false;
    }


    public Texture2D UpdateToXRCpuImage(XRCpuImage xRCpuImage){
        if(texture == null || texture.width != xRCpuImage.width || texture.height != xRCpuImage.height){
            if(texture != null) Destroy(texture); 
            texture = new Texture2D(xRCpuImage.width, xRCpuImage.height, xRCpuImage.format.AsTextureFormat(), false);
        }

        UnityEngine.Debug.Log($"[UpdateToXRCpuImage] xRCpuImage.format.AsTextureFormat(): {xRCpuImage.format.AsTextureFormat()}");
        
        var conversionParams = new XRCpuImage.ConversionParams(xRCpuImage, xRCpuImage.format.AsTextureFormat(),XRCpuImage.Transformation.MirrorX);
        
        var textureData = texture.GetRawTextureData<byte>(); 
        var convertedDataSize = xRCpuImage.GetConvertedDataSize(conversionParams);
        if(textureData.Length != convertedDataSize){
            UnityEngine.Debug.LogError($"failed to convert: size-mismatch: convertedDataSize {convertedDataSize}, textureData.Length {textureData.Length}");
            Destroy(texture);
            return null;
        }

        xRCpuImage.Convert(conversionParams, textureData);
        texture.Apply();
        return texture; 
        }
    }
}


            
            
  