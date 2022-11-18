# XR Remote Preview

## Who is this project for?

This project is for augmented reality developers who are looking for a convenient way to debug their AR applications, without the length build time required for Unity projects like AR Foundation. Unfortunately, AR Foundation from Unity did not ship with a "remote system", i.e. a way to pass AR session information from a device, to the editor.

If you would like to learn more about Unity's option for a similar system, take a look at some of these links: 
* https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-168773
* https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-2-0-201106

## What are the limitations of this project? 

Currently, this application is only for piping **video** and **AR Pose Driver** information from a remote device (a server), to the editor (a client). There are more features in the [ARKit Streamer](https://github.com/asus4/ARKitStreamer) package. However, this project is difficult to set-up, and doesn't utilize the [EditorConnection](https://docs.unity3d.com/ScriptReference/Networking.PlayerConnection.EditorConnection.html), and [PlayerConnection](https://docs.unity3d.com/ScriptReference/Networking.PlayerConnection.PlayerConnection.html) classes from Unity. To be clear, this project currently does not support: 

* Hand Tracking
* Image Tracking
* Face Tracking
* Human Pose Tracking

Though it is not impossible to have these features implemented in the near future

We hope the community can use some of the boiler plate in this project to make these other types possible. Click on each feature to look at its feature request on GitHub. 

## How do I get started? 

The steps differ slightly between Android and iOS. If you are on Android, the project has a pre-built .apk in it for you to try. If you are on iOS, you will need to build the Xcode project, and install it on your device. Also, once in Play mode, you will need to connect your device. The following graphic shows where to connect your device to:

<p align="center">
        <img src="https://user-images.githubusercontent.com/8175726/202572291-ccbc2df5-c336-4374-a7ae-6b437aa7cfa7.png">
</p>
<p align="center">
        <em>Select the dropdown in "B" labeled "Editor" and select your device</em>
</p>

You can find a more detailed explanation [here](https://docs.unity3d.com/Manual/Console.html).

##### - Android -

Before using a device **Please, make sure that you have Developer Mode enabled, and USB Debugging enabled**!

Here is the quick start method: 

* Make sure you are on build platform Android
* Install `XR Remote` as a `UPM Package`
	* Open Package Manager window at "Window/Package Manager"
	* Select "Add package from git URL..." from the drop-down menu
	* Install from `ssh://git@github.com/youar/upm-xrremote.git?path=/Assets/`

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

* Hit Play
* Connect the device to the Editor by going to the Console window, and selecting the Android device from the dropdown. 

If you are just in the XR Remote project, itself, you can bypass this and just plug in an Android device, on the Android platform, and open the AR Remote Client scene (`Assets/XR Remote/Scenes/AR Remote Client`), and hit Play. In a moment, the application should install and launch on your phone.

##### - iOS -

iOS will require you to build out the Xcode projects for the scene `Assets/XR Remote/Scenes/AR Remote Server`, with _Developer Mode_ enabled, and "Run In Xcode" as Debug set via Player Settings. Go to the section on [building XR Remote](#building_xrremote) for more information.

After the application is installed on your iOS device, follow these steps: 

* Drop the `XR Remote` folder into your project at `Assets/XR Remote`
* Make sure you are on build platform iOS
* Make sure you have a `Main Camera` in your scene, with a `Tracked Pose Driver`, and `AR Camera Manager`
* Drag the `XR Remote Connection` prefab into your scene, (`Assets/XR Remote/XR Remote Connection.prefab`)
* Start the XRRemote application built from the above.
* Hit Play
* Connect the device to the Editor by going to the Console window, and selecting the iOS device from the dropdown. 

<a name="building_xrremote"></a>
## Building XR Remote

Building XR Remote should be fairly easy, but there are a couple things to keep in mind, at a high-level:

#### Logging

There is an over all logging system that operates on the premise of "VERBOSE", "MINIMAL", and "QUITE" amounts of logging. This is due to the fact that an attached device will cause 2 streams of data to come into the Unity editor window, and this can sometimes be unruly to deal with. 

#### Features

A feature which uses the AR Foundation SDK will need to be built into the XR Remote application that lives on the device (server). That means that any feature change will need to be built into the application which is providing the information to the Editor (client). The process for doing that sounds lengthy, but doing it on editor first, can save a lot of time. 

In order to add a feature, please expand the `Assets/XR Remote/Scripts/XRRemotePacket.cs` class with additional data types that can be appropriately deserialized. 

### How to Build

**WITHOUT DEVELOPMENT MODE ENABLED THIS WILL NOT CONNECT TO UNITY**

* Select your desired platform
* Build the base scene `Assets/XR Remote/Scenes/AR Remote Server`
* Select the following options in Build Settings: 
	* Development Build
	* Run in Xcode as "Debug" (if applicable)
* Hit Build
* Sign the application with your personal key/team (if applicable)

## FAQ:

* This project uses git lfs for every major type of binary file.

## Bugs: 

* GitHub issues #6 

## Change Log:
4/19/2020
* Fixed Texture destroy issue causing a memory leak.

1/22/2020
* Upgrade to AR Foundation 3.0.1
* Upgrade to ARKit XR Plugin 3.0.1
* Upgrade to ARCore XR Plugin 3.0.1
* Upgrade to Unity 2019.2.18f1
