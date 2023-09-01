//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteDepthImageSender.cs" createdby="razieleron">
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
    private AROcclusionManager occlusionManager;
    // private RawImage rawImage;
    Texture2D texture = null;

    private void Awake()
    {
      occlusionManager = FindObjectOfType<AROcclusionManager>();

      if(occlusionManager == null) {
        UnityEngine.Debug.LogError($"XRRemoteDepthImageSender: Unable to find AROcclusionManager. Please make sure there is one in the scene.");
      }

      return;
    }

    public bool TryGetDepthImage(out SerializableDepthImage depthImage, RawImage rawImage)
    {
        if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage xrCpuImage))
        {
            texture = UpdateToXRCpuImage(xrCpuImage);
            var byteArray = texture.GetRawTextureData();
            depthImage = new SerializableDepthImage(xrCpuImage, byteArray);
            rawImage.texture = texture;

            xrCpuImage.Dispose();
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
        xRCpuImage.Dispose();
        return texture; 
        }
    }
}


            
            
  