# upm-xrremote

## About

upm-xrremote allows users to stream AR Foundation directly to their Unity Editor. 

![demo.gif](createdemovideobeforerelease)

This project is for augmented reality developers who are looking for a convenient way to debug their AR applications, without the lengthy build times required for Unity projects, like AR Foundation. 

AR Foundation did not ship with a "remote system", i.e. a way to pass AR session information from a device, to the editor. Other systems like MARS are about recreating scenes virtually, and simulating augmented reality, but that only goes so far, especially for applications that require real world interactions. 

## Supported XR APIs

* [x] Plane Detection
  * You will need to add a user layer named **Planes** in order to view the planes. (Project Settings -> Tags and Layers)

We're currently working on support for the following APIs: 

* [ ] [Image Tracking](https://github.com/youar/upm-xrremote/issues/24)
* [ ] Hand Tracking
* [ ] Face Tracking
* [ ] Human Pose Tracking

## Getting started

There are 3 steps to using the XRRemote. 
1. Dependencies
2. Server application install
3. Client setup

#### Dependencies 

To get started using this Unity package, either include the following dependencies to your `/Packages/manifest.json` file, 

```json
{
 "dependencies": {
  //
  // some other deps
  //
  "com.youar.upm-xrremote": "ssh://git@github.com/youar/upm-xrremote.git?path=/Assets/#<releasenumber>",
 },
 "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [
        "jp.keijiro"
    }
      ]
  ]
}
```

or setup the same [Scoped Regisry](https://docs.unity3d.com/Manual/upm-scoped.html) and dependency via the Unity Package Manager. 

##### Server application install

Either build the include sample scene, [`Server`](/Assets/Samples/Scenes/Server), or install the associated app for your [release and platform](https://github.com/youar/upm-xrremote/releases). See the section on [Building the 'Server' scene](#building-the-server-scene), for manual building. 

##### Client setup

An example scene setup that uses upm-xrremote can be found in the (Client)[/Assets/Samples/Scenes/Client] scene. The prefabs and references in this scene show the general setup.

#### Building the `Server` scene

> Before reading this section, please make sure you're familiar with building AR applications for iOS and Android, via Unity. 

##### Device Requirements
* Enable Developer mode
* Enable USB Debugging

##### Server Scene Build Setting Requirements
* Ensure correct device target platform
* Enable 'Development Build'
* Ensure `Server` scene is open and added to build
* (Android) 
  * Enable ARCore
  * Target minimum API level 30
* (iOS)
  * Enable ARKit
  * iOS11 is an appropriate minimum
  * iOS support is lacking. Builds will be possible, but may require additional steps on your end.

<details><summary>⚠️ Warning ⚠️</summary>
It is critical that your server scene and client scene come from the same commit tree; if not, there can be issues with serializing and deserializing data. 
</details>

## Contributing 

> For a list of issues and features to work on, please check out our [issues page](https://github.com/youar/upm-xrremote/issues). If you'd like to maintain, please reach out to [@gblikas](https://github.com/gblikas). Keep reading for more information on lifecycle, and dependencies.

## Developer Info

The upm-xrremote project consists of two applications: a "server" and a "client". The server is what is running on your AR device, i.e. an AR-capable. The server scene pipes information to (and reads info from) the client scene, i.e. your Unity Editor.

The upm-xrremote package relies primarily on AR Foundation and [KlakNDI](https://github.com/keijiro/KlakNDI) to send and receive data from device to device. Outside of the setup for this, each sample scene should consist of a "Receiver" and a "Sender" to allow for bi-directional communication. 

The sender packs data using the [Klak metadata property](linktometadataproperty) and the receiver uses a custom package to deserialize, conversely for the receiver package. 

You can find the official dependencies in [manifest.json](Packages/manifest.json), and in [package.json](Asstes/package.json). 

## FAQ

- Q: I've built and installed the server scene on my AR device, and set up the Client scene, but don't see a video stream. 
    - A: Typically, this means that your devices aren't on the same network. Double check that both your AR device and your PC are on the same network (physical or WiFi). If the issue persists, please check our [issues page](https://github.com/youar/upm-xrremote/issues).

## License

> For the full-license, please see https://github.com/youar/upm-xrremote/blob/master/LICENSE

This file (and repository) is part of XR REMOTE.

XR Remote
Copyright(C) 2020  YOUAR, INC.

XR Remote is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

https://www.gnu.org/licenses/agpl-3.0.html

XR Remote is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program. If not, see
<http://www.gnu.org/licenses/>.
