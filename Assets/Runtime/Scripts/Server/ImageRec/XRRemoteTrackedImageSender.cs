//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemotePlaneSender.cs" createdby="gblikas">
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using XRRemote.Serializables;

namespace XRRemote
{
    public class XRRemoteTrackedImageSender : MonoBehaviour
    {

        [SerializeField] private ARTrackedImageManager arTrackedImageManager;

        private void OnEnable()
        {
            ServerReceiver.Instance.OnNewImageLibraryReceived += XRRemoteTrackedImageSender_OnNewImageLibraryReceived;
        }

        private void OnDisable()
        {
            ServerReceiver.Instance.OnNewImageLibraryReceived -= XRRemoteTrackedImageSender_OnNewImageLibraryReceived;
        }

        private void XRRemoteTrackedImageSender_OnNewImageLibraryReceived(object sender, EventArgs e)
        {
            if (ReconstructLibraryFromBytes(ServerReceiver.Instance.referenceImageLibrary, out XRReferenceImageLibrary loadedLibrary))
            {
                arTrackedImageManager.enabled = false;
                arTrackedImageManager.referenceLibrary = loadedLibrary;
                arTrackedImageManager.enabled = true;
                Debug.Log("Successfully changed reference image library!");
            }
            else
            {
                Debug.LogError("Failed to reconstruct reference image library from bytes!");
            }
        }

        private bool ReconstructLibraryFromBytes (byte[] bytes, out XRReferenceImageLibrary loadedLibrary)
        {
            AssetBundle reconstructedBundle = AssetBundle.LoadFromMemory(bytes);

            if (reconstructedBundle == null)
            {
                Debug.LogError("Failed to load AssetBundle!");
                loadedLibrary = null;
                return false;
            }            
            
            //[review]this can be refined after changing bundling method to not include the user given name for the reference library
            loadedLibrary = reconstructedBundle.LoadAsset(reconstructedBundle.GetAllAssetNames()[0]) as XRReferenceImageLibrary;
            reconstructedBundle.Unload(false);

            if (loadedLibrary != null) return true;
            else return false;
        }
        
    }
}
