---
uid: openxr-multiview-render-regions
---
# Multiview Render Regions

## Overview

The Multiview Render Regions feature is an optimization technique that prevents processing on areas of the screen that are not visible to the user.

For a detailed explanation of Multiview Render Regions, refer to [Multiview Render Regions](https://docs.unity3d.com/6000.1/Documentation/Manual/xr-multiview-render-regions.html) in the Unity Manual.

## Prerequisites

To enable this feature, you will need the following:

* Unity 6.1 or newer.
* Ensure that the Vulkan API is enabled. This feature is not available on other graphics APIs at this point in time.

## Enable Multiview Render Regions

To enable the Multiview Render Regions feature:
1. Open the **OpenXR** section of **XR Plug-in Management** (menu: **Edit** > **Project Settings** > **XR Plug-in Management** > **OpenXR**.
2. Under **All Features**, enable **Meta Quest Support**.
3. Use the **Gear** icon to open **Meta Quest Support** settings.
4. Under **Rendering Settings**, enable **Optimize Multiview Render Regions**.

![The Optimize Multiview Render Regions feature is enabled in the OpenXR Rendering Settings.](images/multiview-render-regions.png)<br/>*Enable the Optimize Multiview Render Regions feature in Rendering Settings.*
