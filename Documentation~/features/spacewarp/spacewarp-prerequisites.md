---
uid: openxr-spacewarp-prerequisites
---

# Prerequisites

To use Application SpaceWarp, your project must meet the following prerequisites:

* Unity 6.1+
* OpenXR 1.11.0+  
* Universal Render Pipeline (URP) 17.0.3+  
* Rendergraph (SpaceWarp is not supported in [URP compatibility mode](xref:urp-compatibility-mode))  
* Vulkan graphics API  
* An OpenXR device package, such as [Unity OpenXR Meta](https://docs.unity3d.com/Packages/com.unity.xr.meta-openxr@latest) or [Unity OpenXR Android XR](https://docs.unity3d.com/Packages/com.unity.xr.androidxr-openxr@latest), that supports the OpenXR Application SpaceWarp feature.
* XR Headset that supports the Khronos OpenXR Application SpaceWarp extension.

  > [!NOTE]
  > Refer to your device’s documentation and specifications to determine whether it supports the OpenXR [XR\_FB\_space\_warp](https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_FB_space_warp) extension. (You can also look at the Android logs from a device to make sure that the extension is enabled successfully when you test your app.)