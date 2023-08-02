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

        //needs to be refined to not include the given name of library to enable more generic reconstruction
        public static void BuildAssetBundle(this XRReferenceImageLibrary imageLibrary)
        {
            // Create the array of bundle build details.
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            buildMap[0].assetBundleName = "imagelibrarybundle";

            string[] library = new string[1];
            library[0] = AssetDatabase.GetAssetPath(imageLibrary);

            buildMap[0].assetNames = library;

            string outputDirectory = "Assets/StreamingAssets/AssetBundles/";

            //This is a directory path used frequently.......refine this before shipping or risk ANGRY USERS with deleted bundlesssss >=0
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            //should this be done differently? need to remove previous bundle if it exists, and different files may be included, depending on target build platform
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
