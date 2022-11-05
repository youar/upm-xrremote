//-------------------------------------------------------------------------------------------------------
// <copyright file="IConnection.cs" createdby="gblikas">
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    using UnityEngine.Networking.PlayerConnection;

    public enum LogLevel
    {
        QUIET,
        MINIMAL,
        VERBOSE
    }

    public enum ConnectionState
    {
        DISCONNECTED = 0,
        CONNECTED = 1
    }

    public interface IConnection
	{
        bool Initialize();

        void RegisterMethod(System.Guid socketChannel, UnityEngine.Events.UnityAction<MessageEventArgs> callback);

		void OnConnection(int playerID);
		void OnDisconnection(int playerID);

        void Disconnect();
		void DisconnectAll();

		bool Send(System.Guid socketChannel, object serializeableObject);
		bool Send(System.Guid socketChannel, byte[] data);

        void ToggleLogLevel(LogLevel logLevel); 
        string FormatConnectionMessage(string baseMessage);
	}
}