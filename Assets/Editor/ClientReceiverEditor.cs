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
