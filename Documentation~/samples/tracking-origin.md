---
uid: openxr-tracking-origin-sample
---
# Tracking Origin sample

This sample demonstrates how to use different tracking origin modes to position the camera perspective as needed, and how a stationary reference space stays fixed in the physical world when the tracking origin moves.

## Scenes

The sample includes the **Tracking Origin Sample** scene. This scene allows users to set the tracking origin mode and adjust the camera height in real time.

## Stationary reference space

The **Stationary space** section of the scene places a marker in a stationary reference space next to a marker relative to the XR Origin. When you recenter or change the camera height, the stationary marker stays fixed in your physical room, while the XR Origin marker moves with the rest of the virtual scene.

To use this section, enable the **Stationary Reference Space** feature in **Project Settings** > **XR Plug-in Management** > **OpenXR**, and run the sample on a runtime that supports the `XR_EXT_stationary_reference_space` extension.
