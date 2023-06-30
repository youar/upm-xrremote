//-------------------------------------------------------------------------------------------------------
// <copyright file="ARRemoteConnection.cs" createdby="gblikas">
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
using UnityEditor;
using System.Text;

namespace XRRemote
{

#if UNITY_EDITOR

    using UnityEditor.Networking.PlayerConnection;
    using UnityEngine.Networking.PlayerConnection;

    public class ARRemoteConnectionDiagnostics : EditorWindow
    {
        private string ARRemoteConnectionDiagnosticsMessage(string baseMessage)
        {
            return string.Format("ARRemoteConnectionDiagnostics {0}", baseMessage);
        }

        [MenuItem("Lockar/AR Remote/Diagnostics")]
        static void Init()
        {
            ARRemoteConnectionDiagnostics window = (ARRemoteConnectionDiagnostics)EditorWindow.GetWindow(typeof(ARRemoteConnectionDiagnostics));
            window.Show();
            window.titleContent = new GUIContent("AR Remote Connection(s)");
        }

        void OnEnable()
        {
            EditorConnection.instance.Initialize();
            EditorConnection.instance.Register(ConnectionMessageIds.TestingIds.kMsgSendPlayerToEditor, OnMessageEvent);
        }

        void OnDisable()
        {
            EditorConnection.instance.Unregister(ConnectionMessageIds.TestingIds.kMsgSendPlayerToEditor, OnMessageEvent);
            EditorConnection.instance.DisconnectAll();
        }

        private void OnMessageEvent(MessageEventArgs args)
        {
            string receivedText = Encoding.ASCII.GetString(args.data);
            ARRemoteConnectionDiagnosticsMessage(
                string.Format("OnMessageEvent: received {0}", receivedText));
        }

        void OnGUI()
        {
            var playerCount = EditorConnection.instance.ConnectedPlayers.Count;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("{0} players connected.", playerCount));
            int i = 0;
            foreach (var p in EditorConnection.instance.ConnectedPlayers)
            {
                builder.AppendLine(string.Format("[{0}] - {1} {2}", i++, p.name, p.playerId));
            }
            EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);

            if (GUILayout.Button("Send message to player"))
            {
                try
                {
                    byte[] data = Encoding.ASCII.GetBytes("Editor: ARRemoteConnectionDiagnostics");
                    EditorConnection.instance.Send(ConnectionMessageIds.TestingIds.kMsgSendEditorToPlayer, data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"EXCEPTION reason: {e.Message}");
                }
            }
        }
    }
#endif
}