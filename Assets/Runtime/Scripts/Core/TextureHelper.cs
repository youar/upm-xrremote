//-------------------------------------------------------------------------------------------------------
// <copyright file="TextureHelper.cs" createdby="gblikas">
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


namespace XRRemote
{
    public static class TextureHelper
    {

        private static Color[] FromRFloatBytesToColorArray(byte[] rFloat, int dstWidth, int dstHeight, out float maxValue)
        {
            maxValue = 0.0f;
            if (rFloat.Length != 4 * dstWidth * dstHeight)
            {
                Debug.LogError($"rFloat is most-likely not RFloat: rFloat.Length != 4*{dstWidth}*{dstHeight}");
                return null;
            }

                // if (depthTexture == null)
                // {
                // depthTexture = new Texture2D(height, width, TextureFormat.RFloat, false);
                // }
                // maxValue = 0.0f;

            Color[] pixels = new Color[dstWidth * dstHeight];

            for (int y = 0; y < dstHeight; y++)
            {
                int newY = dstHeight - y - 1; // flips

                for (int x = 0; x < dstWidth; x++)
                {
                    int index = (y * dstWidth + x) * 4;
                    float depthValue = BitConverter.ToSingle(rFloat, index);
                    if (depthValue >= maxValue) maxValue = depthValue;
                    pixels[newY * dstWidth + x] = new Color(depthValue, depthValue, depthValue, 1.0f);
                }
            }

            return pixels;
        }

        private static void RotateTextureClockwise90Degrees()
        {

        }

        private static void RotateTextureClockwise180Degrees()
        {

        }

        private static void RotateTextureClockwise270Degrees()
        {

        }

        public static void PopulateTexture2DFromRBytes(Texture2D inTex, byte[] inRawData, out float fdsa)
        {
            fdsa = 0.0f;
            if (inRawData.Length != 4 * inTex.width * inTex.height)
            {
                Debug.LogError($"array is most-likely not RFloat: array.Length != 4*{inTex.width}*{inTex.height}");
                // maxPixelValue = 0.0f;
                return;
            }
            
            Color[] pixels = FromRFloatBytesToColorArray(inRawData, inTex.width, inTex.height, out fdsa);

            inTex.SetPixels(pixels); 
            inTex.Apply();
        }


        // public static void PopulateTexture2DFromBytes(Texture2D inTex, byte[] inRawData, out float fdsa){
            
        //     fdsa = 0.0f;
        //     // ⚠️ we have no idea how large "inTex" is, nor how large "inRawData" is. 
        //     if (inTex.format != TextureFormat.RFloat) {
        //         return;
        //     }
            
        //     // fill the rest of this out. And also refactor the code so that the naming conventions for variables is more in line with
        //     // what we talked about yesterday.   why did 
    }
}