---
uid: openxr-hand-interaction-profile-microgestures
---
# Hand Interaction Profile Microgestures

Use the Hand Interaction Profile Microgestures feature to add swipe and tap microgesture input.

Unity OpenXR provides support for the `XR_META_hand_tracking_microgestures` extension through the Hand Interaction Profile Microgestures feature. This extension adds swipe and tap microgesture inputs to the [Hand Interaction Profile](xref:openxr-hand-interaction-profile). Microgestures are small thumb motions, such as swiping or tapping the thumb against the side of the index finger.

Enable this OpenXR feature to expose the `<HandInteractionMicrogestures>` device layout within the [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest). Unity creates one device for each hand, and each device has one usage, `LeftHand` or `RightHand`, so you can bind to each hand separately. For example, bind to `<HandInteractionMicrogestures>{LeftHand}/tapThumb` for the left hand.

This feature is additive. It binds to the microgesture inputs that the extension adds to the Hand Interaction Profile, so it requires that profile. Unity exposes the microgesture controls on a separate `HandInteractionMicrogestures` device for each hand, not on the Hand Interaction device. For more information about additive features, refer to [Create an Interaction Feature](xref:openxr-interaction-feature).

For more information about the microgestures extension, refer to the [OpenXR Specification](https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html#XR_META_hand_tracking_microgestures).

## Requirements

To use the Hand Interaction Profile Microgestures feature, your project must meet the following requirements:

* Hand Interaction Profile: Add [Hand Interaction Profile](xref:openxr-hand-interaction-profile) to **Enabled Interaction Profiles**. If you enable this feature without the Hand Interaction Profile, [project validation](xref:openxr-project-validation) reports an error that blocks builds and Play mode. Select **Fix** to add the Hand Interaction Profile to your project's enabled interaction profiles.
* Runtime support: Target an OpenXR runtime that supports the `XR_META_hand_tracking_microgestures` extension, and the `XR_EXT_hand_interaction` extension that the Hand Interaction Profile uses. If the runtime doesn't support the extension, Unity disables the feature when OpenXR initializes.
* Build target: The **Android** and desktop (**Windows, Mac, and Linux**) build targets support this feature.

## Available controls

The following table outlines the mapping between the OpenXR paths and Unity's implementation.

| OpenXR Path | Unity Control Name | Type | Description |
|----|----|----|----|
| `/input/swipe_left_meta/click` | swipeLeft | Boolean | `true` when the runtime recognizes a swipe left microgesture. |
| `/input/swipe_right_meta/click` | swipeRight | Boolean | `true` when the runtime recognizes a swipe right microgesture. |
| `/input/swipe_forward_meta/click` | swipeForward | Boolean | `true` when the runtime recognizes a swipe forward microgesture. |
| `/input/swipe_backward_meta/click` | swipeBackward | Boolean | `true` when the runtime recognizes a swipe backward microgesture. |
| `/input/tap_thumb_meta/click` | tapThumb | Boolean | `true` when the runtime recognizes a thumb tap microgesture. |

## Additional resources

* [Hand Interaction Profile](xref:openxr-hand-interaction-profile)
* [Feature management](xref:openxr-feature-management)
* [Validate your project](xref:openxr-project-validation)
* [Create an Interaction Feature](xref:openxr-interaction-feature)
