---
uid: openxr-android-mouse-interaction
---
# Android Mouse Interaction Profile

Unity OpenXR provides support for the Android Mouse Interaction extension. This extension provides OpenXR support for mouse and trackpad devices in Android XR applications, enabling 3D pointer ray interactions such as using a virtual laser pointer to target objects.

Enables the OpenXR interaction profile for Android Mouse Interaction and exposes the `<AndroidMouseInteraction>` device layout within the [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/).

For more information about the Android Mouse Interaction extension, refer to the [Android XR Documentation](https://developer.android.com/develop/xr/openxr/extensions/XR_ANDROID_mouse_interaction).

## Aim Pose Behavior

The aim pose enables 3D pointer ray interactions with the following characteristics:

- **Position**: Typically positioned at the user's head location, calculated when mouse movement is detected. The last known position is retained until the next movement.
- **Orientation**: The **-Z direction** is the forward aiming direction. Relative X,Y mouse movement computes movement along a sphere around the user's head.
- **Depth Movement**: Position supports depth movement via scroll input:
  - **Positive scroll**: Moves position forward (positive Z-direction)
  - **Negative scroll**: Moves position backward (negative Z-direction)

## Available controls

The following table outlines the mapping between the OpenXR paths and Unity's implementation:

| OpenXR Path | Unity Control Name | Type |
|----|----|-----|
| `/input/aim/pose` | aim | Pose |
| `/input/select/click` | click | Boolean |
| `/input/secondary_android/click` | secondaryClick | Boolean |
| `/input/tertiary_android/click` | tertiaryClick | Boolean |
| `/input/scroll_android/value` | scroll | Vector2 |

### Control Descriptions

- **aim**: A 3D pointer ray pose for interaction with position at the user's head and -Z as the forward aiming direction.
- **click**: Primary mouse button (typically left-click). Returns `true` when pressed.
- **secondaryClick**: Secondary mouse button (typically right-click). Returns `true` when pressed.
- **tertiaryClick**: Tertiary mouse button (typically scroll wheel button or middle button). Returns `true` when pressed.
- **scroll**: Mouse scroll wheel input as a 2D vector with X and Y components in range -1.0 to 1.0. The Y component represents vertical scrolling (where -1.0 is scroll down and 1.0 is scroll up). The X component represents horizontal scrolling. Used for depth movement and interaction scrolling.
