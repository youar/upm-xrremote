//-------------------------------------------------------------------------------------------------------
// <copyright file="CompressionHelper.cs" createdby="gblikas">
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
//
// <modified url="https://bitbucket.org/Unity-Technologies/unity-arkit-plugin/src/default/" modifiedby="gblikas">
//
// This class is designed to provide some additional compression for the underlying
// byte arrays that will be passed from Editro to Player, and visa, versa. 
//
// </modified> 
//-------------------------------------------------------------------------------------------------------

using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace XRRemote
{

    public static class CompressionHelper
    { 

        /// <summary>
        /// Compress using deflate.
        /// </summary>
        /// <returns>The byte compress.</returns>
        /// <param name="source">Source.</param>
        public static byte[] ByteArrayCompress(byte[] source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream compressedDStream = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    compressedDStream.Write(source, 0, source.Length);

                    compressedDStream.Close();

                    byte[] destination = ms.ToArray();

                    if (XRRemote.DebugFlags.displayCompressionStats)
                        Debug.LogFormat("CompressionHelper (src, dst): ({0},{1})", source.Length, destination.Length); 

                    return destination;
                }
            }
        }

        /// <summary>
        /// Decompress using deflate.
        /// </summary>
        /// <returns>The byte decompress.</returns>
        /// <param name="source">Source.</param>
        public static byte[] ByteArrayDecompress(byte[] source)
        {
            using (MemoryStream input = new MemoryStream(source))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (DeflateStream decompressedDstream = new DeflateStream(input, CompressionMode.Decompress))
                    {
                        decompressedDstream.CopyTo(output);

                        byte[] destination = output.ToArray();

                        if (XRRemote.DebugFlags.displayCompressionStats)
                            Debug.LogFormat("Decompress Size (src): {0}", output.Length);

                        return destination;
                    }
                }
            }
        }

        public static long CopyTo(this Stream source, Stream destination)
        {
            byte[] buffer = new byte[2048];
            int bytesRead;
            long totalBytes = 0;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, bytesRead);
                totalBytes += bytesRead;
            }
            return totalBytes;
        }

    }
}