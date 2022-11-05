//-------------------------------------------------------------------------------------------------------
// <copyright file="ShellHelper.cs" createdby="gblikas">
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

//-----------------------------------------------------------------------
// <copyright file="ShellHelper.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//
// <modified url="https://bitbucket.org/Unity-Technologies/unity-arkit-plugin/src/default/" modifiedby="gblikas">
//
// </modified> 
//-----------------------------------------------------------------------

using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ShellHelper
{

    /// <summary>
    /// Run a shell command.
    /// </summary>
    /// <param name="fileName">File name for the executable.</param>
    /// <param name="arguments">Command line arguments, space delimited.</param>
    /// <param name="output">Filled out with the result as printed to stdout.</param>
    /// <param name="error">Filled out with the result as printed to stderr.</param>
    public static void RunCommand(
        string fileName, string arguments, out string output, out string error)
    {
        using (var process = new System.Diagnostics.Process())
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo(fileName, arguments);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            process.OutputDataReceived += (sender, ef) => outputBuilder.AppendLine(ef.Data);
            process.ErrorDataReceived += (sender, ef) => errorBuilder.AppendLine(ef.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();

            // Trims the output strings to make comparison easier.
            output = outputBuilder.ToString().Trim();
            error = errorBuilder.ToString().Trim();
        }
    }

    /// <summary>
    /// Gets the path to adb in the Android SDK defined in the Unity Editor preferences.
    /// </summary>
    /// <remarks>
    /// This function only works while in the Unity editor and returns null otherwise.
    /// </remarks>
    /// <returns> String that contains the path to adb that the Unity editor uses. </returns>
    public static string GetAdbPath()
    {
        string sdkRoot = null;
#if UNITY_EDITOR
        // Gets adb path and starts instant preview server.
        sdkRoot = EditorPrefs.GetString("AndroidSdkRoot");
#endif // UNITY_EDITOR

        if (string.IsNullOrEmpty(sdkRoot))
        {
            return null;
        }

        // Gets adb path from known directory.
        var adbPath = Path.Combine(Path.GetFullPath(sdkRoot),
                                   Path.Combine("platform-tools", GetAdbFileName()));

        return adbPath;
    }

    /// <summary>
    /// Returns adb's executable name based on platform.
    /// On macOS this function will return "adb" and on Windows it will return "adb.exe".
    /// </summary>
    /// <returns> Returns adb's executable name based on platform.
    static string GetAdbFileName()
    {
        var adbName = "adb";

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            adbName = Path.ChangeExtension(adbName, "exe");
        }

        return adbName;
    }

    /// <summary>
    /// Gets the path to aapt in the Android SDK defined in the Unity Editor preferences
    /// </summary>
    /// <remarks>
    /// This function only works while in the Unity editor and returns null otherwise.
    /// </remarks>
    /// <returns></returns>
    public static string GetAaptPath(string buildToolsVersion = "28.0.3")
    {
        string sdkRoot = null;
#if UNITY_EDITOR
        // Gets adb path and starts instant preview server.
        sdkRoot = EditorPrefs.GetString("AndroidSdkRoot");
#endif // UNITY_EDITOR

        if (string.IsNullOrEmpty(sdkRoot))
        {
            return null;
        }

        // Gets adb path from known directory.
        var aaptPath = Path.Combine(
            new string[] { Path.GetFullPath(sdkRoot), "build-tools", buildToolsVersion, GetAaptName() });

        return aaptPath;
    }

    static string GetAaptName()
    {
        var aaptName = "aapt";

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            aaptName = Path.ChangeExtension(aaptName, "exe");
        }

        return aaptName;

    }
}
