---
uid: openxr-palm-pose-interaction
---
# Palm Pose Interaction

Unity OpenXR provides support for the Palm Pose extension specified by Khronos. Use this layout to retrieve the pose data that the extension returns. This extension also adds a new input component path using this "palm_ext" pose identifier to existing interaction profiles when active.

Enables the OpenXR interaction feature for Palm Pose Interaction and exposes the `<PalmPose>` layout within the [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/).

For more information about the Palm Pose extension, refer to the [OpenXR Specification](https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_palm_pose).

## Available controls

The following table outlines the mapping between the OpenXR paths and Unity's implementation:

| OpenXR Path | Unity Control Name | Type |
|----|----|----|
| `/input/palm_ext/pose` | palmPose | Pose |

> [!NOTE]
> OpenXR 1.1 introduced a standardized grip surface pose path at /input/grip_surface/pose. Palm Pose bindings use /input/grip_surface/pose when the runtime reports API version 1.1 or newer, and fall back to /input/palm_ext/pose on older runtimes. If you see only /input/palm_ext/pose in tooling or logs, you are likely running on an OpenXR runtime earlier than 1.1. This feature automatically selects the appropriate path at runtime based on the OpenXR API version.