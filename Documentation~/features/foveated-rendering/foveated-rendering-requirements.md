---
uid: openxr-foveated-rendering-requirements
---
# Foveated rendering requirements reference

Find the Unity versions, packages, render pipelines, and graphics APIs that each OpenXR foveation method requires.

Support for foveated rendering depends on the foveation method you choose, and on the device and its OpenXR runtime. The plug-in provides two methods: the scriptable render pipeline (SRP) method, and the legacy method. This page lists the requirements shared by both methods, the requirements specific to each one, and the extra conditions that dynamic foveation adds.

For an explanation of how the two methods differ, refer to [Introduction to foveated rendering in OpenXR](xref:openxr-foveated-rendering-introduction).

## Shared requirements

Your project must meet the following requirements to use foveated rendering with either method:

| **Requirement** | **Details** |
| :-------------- | :---------- |
| Unity version | Unity 6 or later. |
| OpenXR provider plug-in | `com.unity.xr.openxr` 1.11.0 or later. |

## SRP foveation API requirements

Your project must meet the following requirements to use the SRP Foveation API:

| **Requirement** | **Details** |
| :-------------- | :---------- |
| Render pipeline | The [Universal Render Pipeline (URP)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest?subfolder=/manual/InstallURPIntoAProject.html). |
| Devices | Every platform that supports foveated rendering. |
| Not supported | The Built-In Render Pipeline. |

## Legacy API requirements

Your project must meet the following requirements to use the Legacy API:

| **Requirement** | **Details** |
| :-------------- | :---------- |
| Package | Meta XR Core SDK 68.0 or later. |
| Render pipeline | The Built-In Render Pipeline or URP. |
| Devices | The Meta Quest family of devices only. |
| Not supported | Foveated rendering when you use intermediate render targets. |

> [!IMPORTANT]
> In Unity 6.5 and later, the Built-In Render Pipeline is deprecated, and becomes obsolete in a future release. For more information, refer to [Migrating from the Built-In Render Pipeline to URP](xref:um-upgrading-from-birp) and [Render pipeline feature comparison](xref:um-render-pipelines-feature-comparison).

## Dynamic foveation requirements

Dynamic foveation adds the following requirements to the SRP foveation API requirements:

| **Requirement** | **Details** |
| :-------------- | :---------- |
| Graphics API | Vulkan. |
| Foveation method | The SRP foveation method. |
| Not supported | The quad views and legacy foveation methods. Unity disables the **Dynamic Foveation (Vulkan)** setting when you select either one. |

The OpenXR runtime must also support the following extensions:

* [`XR_FB_foveation`](https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html#XR_FB_foveation)
* [`XR_FB_foveation_configuration`](https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html#XR_FB_foveation_configuration)
* [`XR_FB_foveation_vulkan`](https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html#XR_FB_foveation_vulkan)
* [`XR_FB_swapchain_update_state`](https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html#XR_FB_swapchain_update_state)

## Additional resources

* [Introduction to foveated rendering in OpenXR](xref:openxr-foveated-rendering-introduction)
* [Use dynamic foveation](xref:openxr-foveated-rendering-dynamic)
* [OpenXR settings reference](xref:openxr-settings)
* [Foveated rendering support reference](xref:um-xr-foveated-rendering-support) (Unity Manual)
