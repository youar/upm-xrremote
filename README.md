# XR Remote Preview

## Who is this project for?

This project is for augmented reality developers who are looking for a convenient way to debug their AR applications, without the length build time required for Unity projects like AR Foundation. Unfortunately, AR Foundation from Unity did not ship with a "remote system", i.e. a way to pass AR session information from a device, to the editor.

If you would like to learn more about Unity's current option for a similar system, take a look at some of these links:
* https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-168773
* https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-2-0-201106

## What are the limitations of this project? 

Currently, this application is only for piping **video** and **Tracked Pose Driver** information from a remote device (a server), to the editor (a client). There are more features in the [ARKit Streamer](https://github.com/asus4/ARKitStreamer) package. However, this project is difficult to set-up. To be clear, this project currently does not support:

* Hand Tracking
* Image Tracking
* Face Tracking
* Human Pose Tracking
* Plane Detection

Though it is not impossible to have these features implemented in the near future

We hope the community can use some of the boiler plate in this project to make these other types possible. Click on each feature to look at its feature request on GitHub.

## How do I get started? 

The steps differ slightly between Android and iOS. If you are on Android, the project has a pre-built .apk in it for you to try. If you are on iOS, you will need to build the Xcode project, and install it on your device. Also, once in Play mode, you will need to connect your device. The following graphic shows where to connect your device to:

You can find a more detailed explanation [here](https://docs.unity3d.com/Manual/Console.html).

##### - Android -

Before using a device **Please, make sure that you have Developer Mode enabled, and USB Debugging enabled**!

Here is the quick start method: 

### Android Device
* Make sure you are at least `Android Version 12` (can be found here "Settings/About phone/Software information/Android version")
* Install `upm-xrremote.apk` from https://github.com/youar/upm-xrremote/releases

### Unity Editor
* Make sure you are on build platform Android
* Install `XR Remote` as a `UPM Package`
	* Open Package Manager window at "Window/Package Manager"
	* Select "Add package from git URL..." from the drop-down menu
	* Install from `ssh://git@github.com/youar/upm-xrremote.git?path=/Assets/` ([More Info](https://docs.unity3d.com/Manual/upm-git.html#syntax))

<p align="left">
        <img src="https://user-images.githubusercontent.com/8175726/202574695-af00b094-efee-44f6-84e4-d41e344fe28f.png">
</p>
<p align="left">
        <img src="https://user-images.githubusercontent.com/8175726/202574718-0f370959-d60c-4904-8bd9-6f949ca7824f.png">
</p>

* Make sure you have "ARFoundation ver 4.1.12" installed in your project
* Make sure you have "AR Session" and "AR Session Origin" in your scene
* Make sure `Tracked Pose Driver` is attached as a component on the AR Camera ("AR Session Origin > AR Camera")
* Drag the `XR Remote Connection` prefab into your scene, (`Packages/upm-xrremote/Runtime/Prefabs`)
* Open your `upm-xrremote` application on your device
* Enter your mobile device's IP address in `XR Editor Client` component attached to the `XR Remote Connection` GameObject
* Hit Play in Unity Editor
* Press GUI button "Start XR Remote Session" to connect

You can get the `upm-xrremote` apk by either:
* manually build your own `XR Remote` APK by importing the Sample scenes into the Asset folder and build the `AR Remote Server` scene
* downloading and installing the pre-built apk at found in the [release](https://github.com/youar/upm-xrremote/releases)

##### - iOS -

iOS will require you to build out the Xcode projects for the scene `Assets/XR Remote/Scenes/AR Remote Server`, with _Developer Mode_ enabled, and "Run In Xcode" as Debug set via Player Settings. Go to the section on [building XR Remote](#building_xrremote) for more information.

After the application is installed on your iOS device, follow these steps: 

* Make sure you are on build platform iOS
* Install `XR Remote` as a `UPM Package`
	* Open Package Manager window at "Window/Package Manager"
	* Select "Add package from git URL..." from the drop-down menu
	* Install from `ssh://git@github.com/youar/upm-xrremote.git?path=/Assets/` ([More Info](https://docs.unity3d.com/Manual/upm-git.html#syntax))

<p align="left">
        <img src="https://user-images.githubusercontent.com/8175726/202574695-af00b094-efee-44f6-84e4-d41e344fe28f.png">
</p>
<p align="left">
        <img src="https://user-images.githubusercontent.com/8175726/202574718-0f370959-d60c-4904-8bd9-6f949ca7824f.png">
</p>

* Make sure you have "ARFoundation ver 4.1.12" installed in your project
* Make sure you have "AR Session" and "AR Session Origin" in your scene
* Make sure "AR Pose Driver" is attached as a component on the AR Camera ("AR Session Origin > AR Camera")
* Drag the `XR Remote Connection` prefab into your scene, (`Packages/upm-xrremote/Runtime/Prefabs`)
* Open your `upm-xrremote` application on your device
* Enter your mobile device's IP address in `XR Editor Client` Script on the `XR Remote Connection` GameObject
* Hit Play in Unity Editor
* Press GUI button "Start XR Remote Session" to connect

<a name="building_xrremote"></a>
## Building XR Remote

Building XR Remote should be fairly easy, but there are a couple things to keep in mind, at a high-level:

#### Logging

There is an over all logging system that operates on the premise of "VERBOSE", "MINIMAL", and "QUITE" amounts of logging. This is due to the fact that an attached device will cause 2 streams of data to come into the Unity editor window, and this can sometimes be unruly to deal with. 

#### Features

A feature which uses the AR Foundation SDK will need to be built into the XR Remote application that lives on the device (server). That means that any feature change will need to be built into the application which is providing the information to the Editor (client). The process for doing that sounds lengthy, but doing it on editor first, can save a lot of time. 

In order to add a feature, please expand the `Assets/Runtime/Scripts/XRRemotePacket.cs` class with additional data types that can be appropriately deserialized. 

### How to Build

* Select your desired platform
* import the Sample scenes from the `upm-xrremote` package into the Asset folder
* Build the base scene `AR Remote Server` from `Assets/Samples/upm-xrremte/[upm version]/AR Remote Server`
* Select the following options in Build Settings: 
	* Development Build
	* (iOS) Run in Xcode as "Debug"
	* (Android) Target API Level at least 30
	* (Android) 64 bit architecture `Project Settings/Player/Other Settings/Configuration/Scripting backend` -> IL2CPP
* Hit Build
* Sign the application with your personal key/team (if applicable)

## FAQ:

* This project uses git lfs for every major type of binary file.

## License: 
> For the full-license, please see https://github.com/youar/upm-xrremote/blob/master/LICENSE

This file (and repository) is part of XR REMOTE.

XR Remote
Copyright(C) 2020  YOUAR, INC.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

https://www.gnu.org/licenses/agpl-3.0.html

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program. If not, see
<http://www.gnu.org/licenses/>.

## Bugs: 

* GitHub issues #6 

## Change Log:
11/15/2022
* Upgrade to AR Foundation 4.1.12
* Upgrade to ARKit XR Plugin 4.1.12
* Upgrade to ARCore XR Plugin 4.1.12
* Upgrade to Unity 2020.3.36f1
* Converted from use of PlayerConnection to TCP connection

4/19/2020
* Fixed Texture destroy issue causing a memory leak.

1/22/2020
* Upgrade to AR Foundation 3.0.1
* Upgrade to ARKit XR Plugin 3.0.1
* Upgrade to ARCore XR Plugin 3.0.1
* Upgrade to Unity 2019.2.18f1
