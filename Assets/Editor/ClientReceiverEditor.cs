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

namespace XRRemote
{
    [CustomEditor(typeof(ClientReceiver))]
    public class ClientReceiverEditor : Editor
    {
        private SerializedProperty cameraManagersProperty;

        private void OnEnable()
        {
            cameraManagersProperty = serializedObject.FindProperty("cameraManagerList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Receiving Cameras", "List of AR Cameras that will render the NDI video. Video automatically directed to the active/enabled camera."));
                if (GUILayout.Button("+"))
                {
                    ClientReceiver clientReceiver = (ClientReceiver)target;
                    clientReceiver.AddCameraManager();            
                }
                if (GUILayout.Button("-") && cameraManagersProperty.arraySize > 0)
                {
                    cameraManagersProperty.DeleteArrayElementAtIndex(cameraManagersProperty.arraySize - 1);
                }
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
                for (int i = 0; i < cameraManagersProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = cameraManagersProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(elementProperty);
                }
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
