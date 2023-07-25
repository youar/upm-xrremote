//-------------------------------------------------------------------------------------------------------
// <copyright file="APK.cs" createdby="gblikas">
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
using System.IO;
using System.Threading;
using UnityEngine;

public class APK
{
    string apk;
    string companyName;
    string _bundleIdentifier;
    public string bundleIdentifier
    {
        get
        {
            if (string.IsNullOrEmpty(_bundleIdentifier))
            {
                _bundleIdentifier = $"com.{companyName}.{Path.GetFileNameWithoutExtension(apk)}";
            }
            return _bundleIdentifier;
        }
        set
        {
            _bundleIdentifier = value;
        }
    }

    APK(string name)
    {
        apk = name;
        companyName = Application.companyName;
    }

#if UNITY_EDITOR
    public static APK FromGUID(string apkGUID)
    {
        APK apk = new APK(UnityEditor.AssetDatabase.GUIDToAssetPath(apkGUID));
        return apk;
    }
#endif

    public static APK FromPath(string apkPath)
    {
        if (!File.Exists(apkPath))
        {
            throw new FileNotFoundException($"FILE_NOT_FOUND reason: file {apkPath} does not exist");
        }

        APK apk = new APK(apkPath);

        return apk;
    }

    bool NeedsInstall(bool displayOutput = true)
    {
        string installedVersion = APKInstaller.GetInstalledApkVersion(bundleIdentifier, displayOutput);
        string localVersion = APKInstaller.GetUninstalledApkVersion(apk, displayOutput);

        if (installedVersion != localVersion)
        {
            Debug.LogFormat($"XR Remote Preview installed version {installedVersion} does not match local version {localVersion}");
            return true;
        }

        return false;
    }

    public IEnumerator InstallIfNeeded(bool displayOutput = true)
    {
        if (NeedsInstall(displayOutput))
        {
            Thread installThread = new Thread(() =>
            {
                APKInstaller.InstallAPK(apk, displayOutput);

            });
            installThread.Start();

            while (!installThread.Join(0))
            {
                yield return 0;
            };
        }
    }

    public bool StartServerToDebugingApplication(string debugAppBundleIdentifier, bool displayOutput = true)
    {
        if (string.IsNullOrEmpty(debugAppBundleIdentifier))
        {
            Debug.LogError($"INVALID_BUNDLE_IDENTIFIER reason: \"debugAppBundleIdentifier\" is null or empty");
            return false;
        }

        return APKInstaller.StartADBServer(bundleIdentifier, displayOutput);
    }

    public bool Run(bool displayOutput = true)
    {
        if (NeedsInstall(displayOutput))
        {
            Debug.LogError($"RUN_ERROR reason: apk {apk} needs installation.");
            return false;
        }

        if (!APKInstaller.RunAPK(bundleIdentifier, displayOutput))
        {
            Debug.LogError(
                $"FAILED_START_APK reason: your apk might need to have the adb server started. " +
                $"try again calling StartServerToDebugingApplication first");
            return false;
        }

        return true;
    }
}
