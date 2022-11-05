# Getting Started

Welcome! :wave: 

If you're reading this, you should be thinking about creating a plugin for the Unity Package Manager. This template repository is designed to get you started building a package for UPM. Its documentation is always a work-in-progress, but there are some pre-requisites that you should familiarize yourself with: 
- **Code duplication** To reduce code duplication, please take a look at other packages online, or in your repositories, for a similar package you're looking to create: 
    - :link: [@youar/unity](https://github.com/orgs/youar/teams/unity/discussions/1)
- **Creating Custom Packages** This repository is boilerplate, and before you develop any further, please take a look at Unity Documentation on Creating Custom Packages: 
    - :link: https://docs.unity3d.com/Manual/CustomPackages.html
    - :link: [Package Layout](https://docs.unity3d.com/Manual/cus-layout.html)
- **Publishing** Before you publish, make sure you've included Tests, Samples, Documentation, _and most importantly, LICENSE.md, as well as individual script license headers_. 

## Using upm-Template

- Follow the pre-requisites listed, above.
- Make sure all instances of `upm-Template` are replaced with your package identifiers.
- Make sure to iterate on your project version appropriately, using MAJOR.MINOR.PATCH symantic versioning
- Make sure you've included a high-level changelog
    - This means a changelog that any person can read in plain English, without much knowledge of code needed;
        - Avoid code usage in a changelog, if possible
    - This means consistent changelog formating: https://github.com/youar/changelog

## System Requirements and Usage

Below is a list of requirements for getting started wiht the UPM repo system. It covers how packages should be used, and what version(s) of software you might need, in order to insure that they work properly. 

#### Windows
* Unity 2019.4+
* Git Client v2.14.0+
* Git LFS
* PuTTY SSH Client with a valid key for this repository
* Git, Git LFS, and PuTTY exposed in Environmental Variables

#### MacOS
* TBD

Additional Information: [Unity Docs - Git Dependencies](https://docs.unity3d.com/2019.4/Documentation/Manual/upm-git.html#Git-SSH)

##### Importing
1. Open Unity 2019.4+
2. Open the Package Manager (Window > Package Manager)
3. Add a package by selecting "Add package from git URL..."

![2021-10-12 17_16_55-](https://user-images.githubusercontent.com/8175698/137046043-d86f1b65-314c-461e-a942-86f2a966548c.png)

4. Enter `ssh://git@github.com/youar/<repoName>?path=/Assets/` into the field 
     * To install a specific version, include the version tag at the end of the path preceeded by a `#`
     * Ex. `ssh://git@github.com/youar/<repoName>?path=/Assets/`

##### Change Versions
1. Open Unity 2019.4+
2. Open the Package Manager (Window > Package Manager)
3. Add a package by selecting "Add package from git URL..."

![2021-10-12 17_16_55-](https://user-images.githubusercontent.com/8175698/137046043-d86f1b65-314c-461e-a942-86f2a966548c.png)

4. Enter `ssh://git@github.com/youar/<repoName>?path=/Assets/` into the field

#### Removing
1. Open Unity 2019.4+
2. Open the Package Manager (Window > Package Manager)
3. Select `Cadence UI` under the `Other` category
4. Press `Remove`

## Meeting Legal Requirements

Please, make sure that the YOUAR license agreement is at the top of every file, you create:

```
// Copyright © <currentYear> YOUAR Inc. All rights reserved.
// Any use of this software is subject to a separate written agreement between YOUAR, Inc. and you or the entity for which you work.
// If no such agreement exists, you may not use this software.
```

## Contributing
[Development Practices](https://github.com/youar/upm-Template/blob/main/.github/DEVELOPMENT_PRACTICES.md)