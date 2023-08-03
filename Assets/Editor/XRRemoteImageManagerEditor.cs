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

using UnityEditor;
using UnityEngine;
using System.IO;

namespace XRRemote 
{
    [CustomEditor(typeof(XRRemoteImageManager))]
    public class XRRemoteImageManagerEditor : Editor
    {
        private bool isPlayMode;

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            //disable send to device button when in play mode, unity unable to build asset bundles in play mode
            isPlayMode = EditorApplication.isPlaying;
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            XRRemoteImageManager manager = (XRRemoteImageManager)target;
            DrawDefaultInspector();

            if (!isPlayMode)
            {
                if (GUILayout.Button("Bundle Image Library"))
                {                    
                    if (manager.OnClickTrySend())
                    {
                        manager.imageLibrary.BuildAssetBundle();
                    }
                }
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.LabelField("Bundle Image Library", "Disabled in play mode");
                GUI.enabled = true;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

//[review] move from button press to create asset bundle to something like:
// private void OnPlayModeStateChanged(PlayModeStateChange state)
// {
//     isPlayMode = EditorApplication.isPlaying;
//     Repaint();

//     if (state == PlayModeStateChange.ExitingEditMode)
//     {
//          CustomImageManager manager = (CustomImageManager)target;
//          manager.imageLibrary.BuildAssetBundle();
//     }
// }
