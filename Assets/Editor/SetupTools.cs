using System;
using System.Runtime.CompilerServices;
//-------------------------------------------------------------------------------------------------------
// <copyright file="SetupTools.cs" createdby="Razieleron">
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
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

    //Class to create Planes/XRRemote Debug layers / set those layers to active in all AR cameras.
    //partial src: https://forum.unity.com/threads/adding-layer-by-script.41970
public static class SetupTools
{
    [MenuItem("XRRemote/Layer Utilities/Create Layers and Setup Camera Culling Mask")]
    // [InitializeOnLoadMethod]
    public static void SetupLayers()
    {
        
        Debug.Log("Adding Layers.");
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

        if (asset != null && asset.Length > 0)
        {
            SerializedObject serializedObject = new SerializedObject(asset[0]);
            SerializedProperty layers = serializedObject.FindProperty("layers");
         
           // Add your layers here, these are just examples. Keep in mind: indices below 6 are the built in layers.
            AddLayerAt(layers,  6, "Planes");
            AddLayerAt(layers,  7, "XRRemote-Debug");
 
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        Debug.Log("end of setup layers.");
        SetCameraCullingMask();
    }

    static void SetCameraCullingMask()
    {
        Camera[] cameras = UnityEngine.Object.FindObjectsOfType<Camera>();

        foreach (Camera camera in cameras)
        {
            if (camera.TryGetComponent<ARCameraManager>(out ARCameraManager cameraManager))
            {
                camera.cullingMask |= 1 << LayerMask.NameToLayer("Planes");
                camera.cullingMask |= 1 << LayerMask.NameToLayer("XRRemote-Debug");
            }
        }
    }
 
    static void AddLayerAt(SerializedProperty layers, int index, string layerName, bool tryOtherIndex = true)
   {
       // Skip if a layer with the name already exists.
       for (int i = 0; i < layers.arraySize; ++i)
       {
           if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
           {
               Debug.Log("Skipping layer '" + layerName + "' because it already exists.");
               return;
           }
       }
 
       // Extend layers if necessary
       if (index >= layers.arraySize)
           layers.arraySize = index + 1;
 
       // set layer name at index
       var element = layers.GetArrayElementAtIndex(index);
       if (string.IsNullOrEmpty(element.stringValue))
       {
           element.stringValue = layerName;
           Debug.Log("Added layer '" + layerName + "' at index " + index + ".");
       }
       else
       {
           Debug.LogWarning("Could not add layer at index " + index + " because there already is another layer '" + element.stringValue + "'." );
 
           if (tryOtherIndex)
           {
               // Go up in layer indices and try to find an empty spot.
               for (int i = index + 1; i < 32; ++i)
               {
                   // Extend layers if necessary
                   if (i >= layers.arraySize)
                       layers.arraySize = i + 1;
 
                   element = layers.GetArrayElementAtIndex(i);
                   if (string.IsNullOrEmpty(element.stringValue))
                   {
                       element.stringValue = layerName;
                       Debug.Log("Added layer '" + layerName + "' at index " + i + " instead of " + index + ".");
                       return;
                   }
               }
               Debug.LogError("Could not add layer " + layerName + " because there is no space left in the layers array.");
           }
       }
   }
}

 

