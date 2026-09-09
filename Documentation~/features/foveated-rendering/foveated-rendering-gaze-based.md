---
uid: openxr-foveated-rendering-gaze-based
---
# Configure gaze-based foveated rendering

Enable eye tracking so that the device centers the high-resolution area where the user looks.

Devices that provide eye tracking can support gaze-based foveated rendering, where the device centers the high-resolution area on the point the user looks at. Devices without eye tracking, and devices on which the user denies permission, use fixed foveated rendering instead.

## Prerequisites

Before you begin, make sure your project meets the conditions in [Foveated rendering requirements reference](xref:openxr-foveated-rendering-requirements).

Configure a foveation method first, using either the scriptable render pipeline (SRP) method described in [Use the SRP foveation API](xref:openxr-foveated-rendering-srp-api), or [Use the legacy foveation API](xref:openxr-foveated-rendering-legacy-api).

## Enable eye tracking

To enable eye tracking for gaze-based foveated rendering:

1. Open the **Project Settings** window.
1. Under **XR Plug-in Management**, select the **OpenXR** settings.
1. Select the tab for the platform you want to configure.
1. In the list of **OpenXR Feature Groups**, select **All Features**.
1. Under **OpenXR Feature Groups**, enable the **Foveated Rendering** feature.
1. Under **OpenXR Feature Groups**, select the gear icon next to the **Foveated Rendering** feature.
1. Enable the **Use Eye Tracking** setting.

After you enable **Use Eye Tracking**, the build requests the eye tracking OpenXR extension. Unity enables this setting by default. Disable it if you want to use foveated rendering, including [Quad views](xref:openxr-quad-views), without the eye tracking extension.

Next, [request eye-tracking permission on Android](xref:openxr-foveated-rendering-eye-tracking-permission), then set the foveation level at runtime as described in [Use the SRP foveation API](xref:openxr-foveated-rendering-srp-api). If you use the legacy API with the Meta XR Core SDK, enable the **Meta XR Eye Tracked Foveation** OpenXR feature instead. Other platforms might have similar permission requirements.

## Additional resources

* [Request eye-tracking permission on Android](xref:openxr-foveated-rendering-eye-tracking-permission)
* [Use the SRP foveation API](xref:openxr-foveated-rendering-srp-api)
* [Use dynamic foveation](xref:openxr-foveated-rendering-dynamic)
* [Eye Tracking Interaction](xref:openxr-eye-gaze-interaction)
