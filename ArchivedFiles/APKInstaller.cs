//-------------------------------------------------------------------------------------------------------
// <copyright file="APKInstaller.cs" createdby="gblikas">
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

using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// A static class with functionality to install an apk.
///
/// This class should be used by the APK class, in order
/// to install, run and check version information about a
/// given plugin.
/// </summary>
static class APKInstaller
{
    private const string noDevicesFoundAdbResult = "error: no devices/emulators found";
    private const string adbNotRunningResuilt = "daemon not running"; 

    private static string _adbPath;
    public static string adbPath
    {
        get
        {
            if (string.IsNullOrEmpty(_adbPath))
            {
                _adbPath = ShellHelper.GetAdbPath();
                if (_adbPath == null)
                {
#if UNITY_ANDROID
                    Debug.LogError("APKInstaller requires your Unity Android SDK path to be set. " +
                        "Please set it under 'Preferences > External Tools > Android'. " +
                        "You may need to install the Android SDK first.");
#endif
                    throw new System.ArgumentNullException(nameof(_adbPath)); 
                }
                else if (!File.Exists(_adbPath))
                {
                    Debug.LogErrorFormat(
                        "adb not found at \"{0}\". Please verify that 'Preferences > External Tools " +
                        "> Android' has the correct Android SDK path that the Android Platform Tools " +
                        "are installed, and that \"{0}\" exists. You may need to install the Android " +
                        "SDK first.", _adbPath);
                    throw new System.Exception($"FILE_NOT_FOUND reason: {_adbPath} could not be found");
                }
            }
            return _adbPath;
        }
    }


    private static string _aaptPath;
    public static string aaptPath
    {
        get
        {
            if (string.IsNullOrEmpty(_aaptPath))
            {
                _aaptPath = ShellHelper.GetAaptPath();
                if (_aaptPath == null)
                {
#if UNITY_ANDROID
                    Debug.LogError("APKInstaller requires your Unity Android SDK path to be set. " +
                        "Please set it under 'Preferences > External Tools > Android'. " +
                        "You may need to install the Android SDK first.");
#endif
                    throw new System.ArgumentNullException(nameof(_aaptPath));
                }
                else if (!File.Exists(_aaptPath))
                {
                    Debug.LogErrorFormat(
                        "aapt not found at \"{0}\". Please verify that 'Preferences > External Tools " +
                        "> Android' has the correct Android SDK path that the Android Build Tools " +
                        "are installed, and that \"{0}\" exists. You may need to install the Android " +
                        "SDK first.", _aaptPath);
                    throw new System.Exception($"FILE_NOT_FOUND reason: {_aaptPath} could not be found");
                }
            }
            return _aaptPath;
        }
    }

    static bool ConnectedDevices(string errors)
    {
        if (string.Compare(errors, noDevicesFoundAdbResult) == 0)
        {
            Debug.LogError("DEVICE_NOT_FOUND reason: there are no connected devices");
            return false;
        }

        return true;
    }

    public static bool InstallAPK(string apkPath, bool displayOutput = true)
    {
        // Early outs if set to install but the apk can't be found.
        if (!File.Exists(apkPath))
        {
            Debug.LogErrorFormat($"FILE_NOT_FOUND reason: apkPath {apkPath} does not exits. perhaps apk guid, path, or name was incorrect");
            return false;
        }

        string output, errors; 

        ShellHelper.RunCommand(adbPath, $"install \"{apkPath}\"", out output, out errors);

        ConnectedDevices(errors); 

        if (!string.IsNullOrEmpty(errors))
        {
            Debug.LogError(errors);
            return false;
        }

        if (displayOutput && !string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }

        return true;
    }

    public static bool RunAPK(string bundleIdentifier, bool displayOutput = true)
    {
        string output, errors;

        // Gets version of installed apk.
        ShellHelper.RunCommand(adbPath, $"shell monkey \"-p\" {bundleIdentifier} \"-c\" \"android.intent.category.LAUNCHER\" 1", out output, out errors);

        ConnectedDevices(errors);

        if (!string.IsNullOrEmpty(errors))
        {
            if (!errors.Contains("android.intent.category.LAUNCHER"))
            {
                Debug.LogError(errors);
                return false;
            }
        }

        if (displayOutput && !string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }

        return true;

    }

    public static string GetUninstalledApkVersion(string apkPath, bool displayOutput = true)
    {
        string output, errors;

        ShellHelper.RunCommand(aaptPath, $"dump badging \"{apkPath}\"", out output, out errors);

        if (!string.IsNullOrEmpty(errors))
        {
            throw new System.Exception(errors);
        }

        string version = string.Empty;

        if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(errors))
        {
            if (output.Contains("versionName"))
            {
                output = output.Substring(output.IndexOf("versionName"));
                output = output.Split(new[] { '\r', '\n' }).FirstOrDefault();
                version = GetVersionNumber(output);
            }
        }

        if (displayOutput && !string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }

        return version.Replace("'", "");
    }

    public static string GetInstalledApkVersion(string bundleIdentifier, bool displayOutput = true)
    {
        string output, errors; 

        Debug.Log("delete me : "+adbPath);
        Debug.Log("delete me : "+bundleIdentifier);
        ShellHelper.RunCommand(adbPath, $"shell dumpsys package \"{bundleIdentifier}\"", out output, out errors);

        ConnectedDevices(errors);

        if (!string.IsNullOrEmpty(errors))
        {
            throw new System.Exception(errors);
        }

        string version = string.Empty;

        if (!string.IsNullOrEmpty(output) && !string.IsNullOrWhiteSpace(output) && string.IsNullOrEmpty(errors))
        {
            if (output.Contains("versionName"))
            {
                output = output.Substring(output.IndexOf("versionName"));
                output = output.Split(new[] { '\r', '\n', ' ' }, System.StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                version = GetVersionNumber(output);
            }
        }

        if (displayOutput && !string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }
        
        return version.Replace("'", "");
    }

    static string GetVersionNumber(string versionString)
    {
        return versionString.Substring(versionString.IndexOf('=') + 1).Split(new[] { ' ' }).FirstOrDefault();
    }

    public static bool NeedsInstall(string bundleIdentifier, string targetVersion)
    {
        string version = GetInstalledApkVersion(bundleIdentifier);

        targetVersion = GetVersionNumber(targetVersion);

        if (targetVersion != version) return true; 

        return false;
    }

    /// <summary>
    /// Start the adb server to port-forward to the correct development
    /// build application, specified by bundleIdentifier.
    /// <remark>
    /// If this is not done,
    /// unplugging the device will cause a disconnection that will not
    /// automagically work.
    /// </remark>
    /// </summary>
    /// <param name="bundleIdentifier"></param>
    /// <param name="displayOutput"></param>
    /// <returns> whether or not he the server was properly started with
    /// the correct port forwarding.</returns>
    public static bool StartADBServer(string bundleIdentifier, bool displayOutput = true)
    {
        string output, errors;

        Debug.Log("delete me : adbPath == "+adbPath); 
        
        /*
        ShellHelper.RunCommand(adbPath, $"kill-server", out output, out errors);

        if (!string.IsNullOrEmpty(errors))
        {
            Debug.LogError($"ADB_ERROR reason: {errors}");
            return false;
        }

        if (displayOutput && !string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }

        ShellHelper.RunCommand(adbPath, $"start-server", out output, out errors);

        if (!errors.Contains(adbNotRunningResuilt))
        {
            if (!string.IsNullOrEmpty(errors))
            {
                Debug.LogError($"ADB_ERROR reason: {errors}");
                return false;
            }
        }
        else
        {
            Debug.Log($"ADB_ERROR reason: {errors}");
        }

        if (displayOutput && !string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }
*/
        ShellHelper.RunCommand(adbPath, $"forward tcp:34999 localabstract:Unity-{bundleIdentifier}", out output, out errors);

        if (!string.IsNullOrEmpty(errors))
        {
            Debug.LogError($"ADB_ERROR reason: {errors}");
            return false;
        }

        if (displayOutput && !string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }

        return true; 
    }


}
