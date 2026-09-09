---
uid: openxr-foveated-rendering-introduction
---
# Introduction to foveated rendering in OpenXR

Learn how OpenXR devices implement foveated rendering, and how the two foveation methods that the plug-in provides differ.

Foveated rendering is an optimization technique that can speed up rendering with minimal perceived impact on visual quality. Foveated rendering works by lowering the resolution of areas in the user's peripheral vision. On headsets that support both eye tracking and foveated rendering, the device can center the high-resolution area where the user looks. Without eye tracking, the high-resolution area stays fixed near the middle of the screen. Users are more likely to notice fixed foveated rendering, because they can shift their eyes to look at the peripheral areas.

> [!NOTE]
> Quad views is a type of foveated rendering that uses a different rendering technique. You can't use both quad views and foveated rendering in your project. For more information about quad views, refer to [Compare quad views and foveated rendering](xref:openxr-quad-views#compare-quad-views-and-foveated-rendering).

## Shader compatibility

OpenXR platforms use the variable rate shading (VRS) technique for foveated rendering, which doesn't require you to change custom shaders. If you're creating assets that must work on all XR platforms, refer to [Adapt custom shaders for foveated rendering](xref:um-xr-foveated-rendering-custom-shaders). That page covers how to write shaders and shader graphs that work with every supported foveated rendering method.

## How the plug-in selects a technique

OpenXR devices can implement VRS with several different techniques. A single device can support more than one implementation. The Unity OpenXR provider plug-in chooses from the following techniques, in this order, depending on what the current device supports:

1. Gaze-based fragment density map (FDM) from the provider.
1. Fixed fragment density map from the provider.
1. Fragment shading rate from a provider's texture.
1. Fragment shading rate, calculated in a compute shader from the asymmetric fields of view (FOV) that the provider supplies.

> [!NOTE]
> On Vulkan, Unity automatically disables FDM foveated rendering at runtime if the physical device running OpenXR doesn't support fragment density maps (`VK_EXT_fragment_density_map`).

## Foveation methods

The plug-in provides two foveation methods, and you choose between them with the **Foveated Rendering Method** setting. One uses the scriptable render pipeline (SRP), and the other uses the Meta XR Core SDK:

* The Unity SRP foveation API works on every platform that supports foveated rendering, so you can share code across device types. It doesn't support the Built-In Render Pipeline.
* The legacy API works only on Meta Quest devices, and it supports the Built-In Render Pipeline. It doesn't support foveated rendering when you use intermediate render targets. For example, post-processing, tone mapping, and camera stacking use intermediate render targets, and other rendering features might use them too.

The recommended best practice is to use the SRP foveation API where possible, because it works across more platforms. You can still use other features from the Meta XR Core SDK with the SRP foveation API.

## Additional resources

* [Foveated rendering requirements reference](xref:openxr-foveated-rendering-requirements)
* [Use the SRP foveation API](xref:openxr-foveated-rendering-srp-api)
* [Use the legacy foveation API](xref:openxr-foveated-rendering-legacy-api)
* [Introduction to foveated rendering](xref:um-xr-foveated-rendering-introduction) (Unity Manual)
