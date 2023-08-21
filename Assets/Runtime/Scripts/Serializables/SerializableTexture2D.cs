//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableFloat2.cs" createdby="gblikas">
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
using UnityEngine;
using Unity.Collections;

namespace XRRemote.Serializables 
{
    [Serializable]
    public class SerializableDepthImage
    {
        public float[] pixelData;
        public int width;
        public int height;
    

        public SerializableDepthImage(Texture2D tex)
        {
            pixelData = new float[tex.width * tex.height];
            for (int y = 0; y < height; y++) 
            {
                for (int x = 0; x < width; x++) 
                {
                    Color pixelColor = tex.GetPixel(x, y);
                    // Convert the RHalf value to a float or another suitable format.
                    float convertedValue = pixelColor.r; // Assuming RHalf is stored in the red channel.
                    Debug.Log($"Individual Pixel Value: {convertedValue}");
                    int index = x + y * width;
                    pixelData[index] = convertedValue;
                }
            }
            width = tex.width;
            height = tex.height;
        }

        public Texture2D ReconstructDepthImageFromSerializableDepthImage()
        {
            TextureFormat texFormat = TextureFormat.RHalf; 
            Texture2D reconstructedTexture = new Texture2D(width, height, texFormat, false);

            // Set the pixel data
            Color[] colors = new Color[pixelData.Length];

            for (int i = 0; i < pixelData.Length; i++) {
                colors[i] = new Color(pixelData[i], 0f, 0f, 1f); // Assuming RHalf data, set red channel
            }

            reconstructedTexture.SetPixels(colors);

            // Apply the changes to the texture
            reconstructedTexture.Apply();
            return reconstructedTexture;
        }        
    }
}

// // Assuming you have raw data in the format you want to reconstruct the texture from
// float[] pixelData; // Replace with your actual raw data
// int width = /* Width of your texture */;
// int height = /* Height of your texture */;
// TextureFormat format = TextureFormat.RHalf; // Replace with the appropriate format

// // Create a new Texture2D
// Texture2D reconstructedTexture = new Texture2D(width, height, format, false);

// // Set the pixel data
// Color[] colors = new Color[pixelData.Length];

// for (int i = 0; i < pixelData.Length; i++) {
//     colors[i] = new Color(pixelData[i], 0f, 0f, 1f); // Assuming RHalf data, set red channel
// }

// reconstructedTexture.SetPixels(colors);

// // Apply the changes to the texture
// reconstructedTexture.Apply();

// // Now, 'reconstructedTexture' contains your texture with the data from 'pixelData'.

// // Assuming you have an RHalf texture
// Texture2D rHalfTexture; // Replace with your actual RHalf texture
// int width = rHalfTexture.width;
// int height = rHalfTexture.height;

// // Create a new Texture2D with a common format (e.g., RGBA32)
// Texture2D convertedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

// // Set the pixels of the converted texture to match the RHalf texture
// convertedTexture.SetPixels(rHalfTexture.GetPixels());

// // Apply the changes to the converted texture
// convertedTexture.Apply();

// // Now, 'convertedTexture' contains the visual representation of your texture