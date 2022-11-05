//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteInstallation.cs" createdby="gblikas">
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
using UnityEngine;

public class XRRemoteInstallation : MonoBehaviour
{
    private static string xrRemoteApkGUID = "8c0eeb23a51e4422b932b9b6d5fd6e95"; 

    /// <summary>
    /// create an APK install type for the given apk in the
    /// projected, pointed to by the GUID. 
    /// </summary>
    private static APK xrRemoteApk;

    /// <summary>
    /// Launch the XRRemote Preview application if needed. This also automatically
    /// starts the given ADB server so that the system works when
    /// unplugging and plugging back in a device. 
    /// </summary>
    /// <returns></returns>
    public static IEnumerator LaunchApkIfNeeded()
    {

#if UNITY_ANDROID && UNITY_EDITOR
        yield return xrRemoteApk.InstallIfNeeded();

        if (!xrRemoteApk.StartServerToDebugingApplication(xrRemoteApk.bundleIdentifier))
        {
            yield break;
        }

        if (!xrRemoteApk.Run())
        {
            yield break;
        }
#else
        yield break;
#endif 
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID && UNITY_EDITOR
        xrRemoteApk = APK.FromGUID(xrRemoteApkGUID);

        StartCoroutine(LaunchApkIfNeeded());
#endif
    }
}
