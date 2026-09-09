---
uid: openxr-foveated-rendering
---
# Foveated rendering in OpenXR

Understand the foveated rendering methods that the Unity OpenXR provider plug-in supports, and configure the one your project needs.

Foveated rendering is an optimization technique that can speed up rendering with minimal perceived impact on visual quality. It works by lowering the resolution of areas in the user's peripheral vision.

The plug-in provides two foveation methods: the Unity scriptable render pipeline (SRP) foveation API, and a legacy API that uses the Meta XR Core SDK. You can also add gaze-based foveated rendering, which follows the user's eyes, and dynamic foveation, which varies the amount of foveation as GPU load changes.

These pages cover the aspects of foveated rendering that are specific to the Unity OpenXR provider plug-in. For general information about foveated rendering in Unity XR, refer to [Foveated rendering](xref:um-xr-foveated-rendering) in the Unity Manual.

| **Topic** | **Description** |
| :-------- | :-------------- |
| [Introduction to foveated rendering in OpenXR](xref:openxr-foveated-rendering-introduction) | Learn how OpenXR devices implement foveated rendering, and how the two foveation methods differ. |
| [Foveated rendering requirements reference](xref:openxr-foveated-rendering-requirements) | Find the Unity versions, packages, render pipelines, and graphics APIs that each foveation method requires. |
| [Use the SRP foveation API](xref:openxr-foveated-rendering-srp-api) | Configure the SRP foveation method, then set the foveation level at runtime with `XRDisplaySubsystem`. |
| [Use the legacy foveation API](xref:openxr-foveated-rendering-legacy-api) | Configure the legacy method with the Meta XR Core SDK, then set the foveation level at runtime with `OVRManager`. |
| [Configure gaze-based foveated rendering](xref:openxr-foveated-rendering-gaze-based) | Enable eye tracking so the device centers the high-resolution area where the user looks. |
| [Request eye-tracking permission on Android](xref:openxr-foveated-rendering-eye-tracking-permission) | Declare and request the Android permission that gaze-based foveated rendering requires. |
| [Use dynamic foveation](xref:openxr-foveated-rendering-dynamic) | Let the device vary the amount of foveation it applies, up to the level you set. |

## Additional resources

* [Subsampled layout](xref:openxr-subsampled-layout)
* [Quad views](xref:openxr-quad-views)
* [OpenXR settings reference](xref:openxr-settings)
* [Foveated rendering](xref:um-xr-foveated-rendering) (Unity Manual)
