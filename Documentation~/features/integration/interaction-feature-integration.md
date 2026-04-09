---
uid: openxr-interaction-feature
---

# Create an OpenXR Interaction Feature

Use the [OpenXRInteractionFeature](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature) class to make an OpenXR interaction profile available to Unity apps. An `OpenXRInteractionFeature` implementation connects an OpenXR interaction profile to the Unity Input System and makes the profile available to the OpenXR settings in the Unity Editor.

You can also create additive profiles. Unlike a regular profile, an additive profile can't be selected by the OpenXR runtime as the active profile. Instead, the actions in the additive profile can be added to the active profile. For example, the [DPad interaction profile](xref:openxr-dpad-interaction) adds thumbstick and trackpad controls to the selected interaction profile, but can't be used as an interaction profile on its own. Refer to [Additive profiles](#additive-profiles) for information about implementing an additive profile.

To implement a subclass of [OpenXRInteractionFeature](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature) that hooks up an interaction profile to the Unity Input System, you need to accomplish the following tasks:

* Decorate the feature class declaration with the [OpenXRFeatureAttribute](xref:UnityEditor.XR.OpenXR.Features.OpenXRFeatureAttribute). This attribute makes your feature discoverable to the XR build system and the OpenXR settings in Unity. This declaration allows application developers to enable your feature and make the interaction profile controls available to Unity XR applications.
* Register the interaction profile paths and actions with the OpenXR runtime. Registration allows the OpenXR runtime to choose your profile when it best suits the current hardware.
* Create an Input System device layout to represent the physical controller.
* Register the Input System paths to map the Unity path strings to your device controls.

The **Category** parameter of the [OpenXRFeatureAttribute](xref:UnityEditor.XR.OpenXR.Features.OpenXRFeatureAttribute) determines how your profile is presented to developers in the Unity OpenXR settings:

* [**UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction**](xref:UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction): The feature is included in the list of interaction profiles and can be added to the list of **Enabled Interaction Profiles**. Put regular non-additive profiles in the **Interaction** category.
* [**UnityEditor.XR.OpenXR.Features.FeatureCategory.Feature**](xref:UnityEditor.XR.OpenXR.Features.FeatureCategory.Feature): The feature is included in the list of OpenXR features that developers can enable individually. Always put [Additive profiles](#additive-profiles) in the **Feature** category so that developers using the additive profile don't accidentally try to select it as a regular profile.

> [!NOTE]
> You can't create new interaction profiles. The profile that you reference in your `OpenXRInteractionFeature` must already be supported by the OpenXR runtime.

## Declare an OpenXR Interaction Feature

Your class must extend the [OpenXRInteractionFeature](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature) class and, at a minimum, implement the [RegisterActionMapsWithRuntime](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.RegisterActionMapsWithRuntime) function. Use an [OpenXRFeatureAttribute](xref:UnityEditor.XR.OpenXR.Features.OpenXRFeatureAttribute) on your class declaration. This attribute makes the class discoverable by the Unity OpenXR code in the Editor. When discovered, the Editor adds your feature to the OpenXR settings page for the build target groups declared in the attribute.

The key attribute properties that you must set include:

* **UIName**: This value is the label shown in the OpenXR settings where users of your feature can choose to enable it.
* **BuildTargetGroups**: Determines which platforms your feature can be enabled for.
* **Desc**: Helps users of your feature decide whether they need to enable it or not.
* **Category**: Determines whether your feature is included in the list of interaction profiles or in the OpenXR features list. Choose [UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction](xref:UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction) if you want the feature to be included in the **Enabled Interaction Profiles** list.
* **FeatureId**: This ID is an arbitrary string that you create to uniquely identify your feature. To help ensure uniqueness, a reverse-domain-style name string should be used (for example: "com.yourcompany.openxr.feature.input.featurename").

Refer to [OpenXRInteractionFeature class](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddAdditiveActions(System.Collections.Generic.List{UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig},UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) for an example of how the `OpenXRFeatureAttribute` should be applied to an `OpenXRInteractionFeature` implementation.

## Create and register OpenXR action maps

In your [OpenXRInteractionFeature](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature) class, you must implement the [RegisterActionMapsWithRuntime](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.RegisterActionMapsWithRuntime) function to create the interaction data that needs to be sent to the OpenXR runtime. Unity code calls this function automatically when initializing the app at runtime.

To register an OpenXR action map, create an [ActionMapConfig](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig) object and populate it with the [DeviceConfig](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.DeviceConfig) and [ActionConfig](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionConfig) objects describing your interaction profile. Finally, call [OpenXRInteractionFeature.AddActionMap](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddActionMap(UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) to send the action map data to the OpenXR runtime.

If you want to make some action bindings available to OpenXR applications even when your feature is not the interaction profile chosen at runtime, you can mark those bindings as additive by setting the ActionConfig.isAdditive property to true and implement the [OpenXRInteractionFeature.AddAdditiveActions](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddAdditiveActions(System.Collections.Generic.List{UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig},UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) function as described in [Additive actions](#additive-actions).

Refer to [OpenXRInteractionFeature class](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddAdditiveActions(System.Collections.Generic.List{UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig},UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) for an example of how to create and register action maps.

## Create an Input System device layout

A device layout describes your input device to the Unity Input System. To define a device layout that describes the controls provided by your interaction profile:

* Implement an [XR.InputDevice](https://docs.unity3d.com/ScriptReference/XR.InputDevice.html) subclass, typically based on [XRController](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/api/UnityEngine.InputSystem.XR.XRController.html).
* Use [InputControlLayoutAttribute](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/api/UnityEngine.InputSystem.Layouts.InputControlLayoutAttribute.html?q=InputControlLayoutAttribute) to define the name and common usages of the device.
* Create an [InputControl](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/api/UnityEngine.InputSystem.InputControl.html) property for each data input in the profile, such as [ButtonControl](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/api/UnityEngine.InputSystem.Controls.ButtonControl.html) for hardware buttons and [XR.PoseControl](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/api/UnityEngine.InputSystem.XR.PoseControl.html) for tracked items.
* Implement [OpenXRInteractionFeature.RegisterDeviceLayout](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature#UnityEngine_XR_OpenXR_Features_OpenXRInteractionFeature_RegisterDeviceLayout) to register the your `InputDevice` subclass with the Input System.

Refer to [OpenXRInteractionFeature class](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddAdditiveActions(System.Collections.Generic.List{UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig},UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) for an example implementation of the [RegisterDeviceLayout](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature#UnityEngine_XR_OpenXR_Features_OpenXRInteractionFeature_RegisterDeviceLayout) method.

## Additive profiles {#additive-profiles}

Create an additive interaction profile to make actions available to other interaction profiles. When an app developer enables an additive interaction profile, that profile's actions are added to the regular, non-additive profile selected by the OpenXR runtime on an app user's device. An additive profile is never chosen as the active profile and cannot be used on its own.

To add actions to other interaction profiles, you must override the following members of the [OpenXRInteractionFeature](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature) class:

* [isAdditive](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.IsAdditive) property

   This property indicates whether your interaction feature provides additive actions. Set the property to `true` in your subclass.

* [AddAdditiveActions](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddAdditiveActions(System.Collections.Generic.List{UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig},UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) method

   This method receives a list of [ActionMapConfig](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig) objects from all of the other interaction profiles enabled by the app developer for their project. Your implementation must iterate through these action maps and add the actions from your profile. In your method implementation, you should check to make sure an action doesn't already exist in the other action map before adding it.

When you create an additive profile, you should mark all of its actions as additive by setting their `ActionConfig.isAdditive` property to `true`. Any actions not marked as additive are ignored because an additive profile can never be used on its own. You should put [Additive profiles](#additive-profiles) in the **Feature** category so the Unity OpenXR settings put your profile in the list of OpenXR features instead of in the list of interaction profiles. Otherwise, app developers using the additive profile might try to use it as a regular profile.

Refer to [OpenXRInteractionFeature class](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddAdditiveActions(System.Collections.Generic.List{UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig},UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) for an example implementation of the [AddAdditiveActions](xref:UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.AddAdditiveActions(System.Collections.Generic.List{UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig},UnityEngine.XR.OpenXR.Features.OpenXRInteractionFeature.ActionMapConfig)) method.
