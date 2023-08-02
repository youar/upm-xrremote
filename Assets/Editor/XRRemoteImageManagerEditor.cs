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
                if (GUILayout.Button("Send Library To Device"))
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
                EditorGUILayout.LabelField("Send Library To Device", "Disabled in play mode");
                GUI.enabled = true;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

//move from button press to create asset bundle to something like:
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
