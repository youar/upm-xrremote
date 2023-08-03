//-------------------------------------------------------------------------------------------------------
// <copyright file="ServerReceiver.cs" createdby="cSustrich">
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

using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using System.IO;


namespace XRRemote
{
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public static class ImageLibraryAssetBundler
    { 

        //[review] needs to be refined to not include the given name of library to enable more generic reconstruction
        //[review] will this have issues if the user has other asset bundles qued to be built
        public static void BuildAssetBundle(this XRReferenceImageLibrary imageLibrary)
        {
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            buildMap[0].assetBundleName = "imagelibrarybundle";

            string[] library = new string[1];
            library[0] = AssetDatabase.GetAssetPath(imageLibrary);

            buildMap[0].assetNames = library;

            string outputDirectory = "Assets/StreamingAssets/AssetBundles/";

            //[review] This is a directory path used frequently.......refine this before shipping or risk ANGRY USERS with deleted bundlesssss >=0
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            //[review] should this be done differently? need to remove previous bundle if it exists, and different files may be included, depending on target build platform
            System.IO.DirectoryInfo di = new DirectoryInfo(outputDirectory);
            foreach(FileInfo file in di.GetFiles())
            {   
                file.Delete();
            }

            BuildPipeline.BuildAssetBundles(outputDirectory, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            string outputPath = Path.Combine(outputDirectory, "imagelibrarybundle");   
        }
    } 
   
}

/*
Must review compression methods:

BuildAssetBundleOptions.None: This bundle option uses LZMA Format compression, which is a single compressed LZMA stream of serialized data files. LZMA compression requires that the entire bundle is decompressed before it’s used. This results in the smallest possible file size but a slightly longer load time due to the decompression. It is worth noting that when using this BuildAssetBundleOptions, in order to use any assets from the bundle the entire bundle must be uncompressed initially.
Once the bundle has been decompressed, it will be recompressed on disk using LZ4 compression which doesn’t require the entire bundle be decompressed before using assets from the bundle. This is best used when a bundle contains assets such that to use one asset from the bundle would mean all assets are going to be loaded. Packaging all assets for a character or scene are some examples of bundles that might use this.
Using LZMA compression is only recommended for the initial download of an AssetBundle from an off-site host due to the smaller file size. LZMA compressed asset bundles loaded through UnityWebRequestAssetBundle are automatically recompressed to LZ4 compression and cached on the local file system. If you download and store the bundle through other means, you can recompress it with the AssetBundle.RecompressAssetBundleAsync API.

BuildAssetBundleOptions.UncompressedAssetBundle: This bundle option builds the bundles in such a way that the data is completely uncompressed. The downside to being uncompressed is the larger file download size. However, the load times once downloaded will be much faster. Uncompressed AssetBundles are 16-byte aligned.

BuildAssetBundleOptions.ChunkBasedCompression: This bundle option uses a compression method known as LZ4, which results in larger compressed file sizes than LZMA but does not require that entire bundle is decompressed, unlike LZMA, before it can be used. LZ4 uses a chunk based algorithm which allows the AssetBundle be loaded in pieces or “chunks.” Decompressing a single chunk allows the contained assets to be used even if the other chunks of the AssetBundle are not decompressed.
*/
