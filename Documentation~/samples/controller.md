---
uid: openxr-controller-sample
---
# Controller sample

Demonstrates retrieving input from an XR controller with OpenXR using the Unity [Input System](xref:input-system-index).

Controller buttons, axes, thumbstick positions, are displayed in real time within the sample scene.

## Scenes

The sample includes the **ControllerSample** scene. This scene displays left- and right-hand controller state panels driven by the Input System Action asset.

## Prefabs

The sample includes the following notable Prefabs:

| Prefab | Description |
| --- | --- |
| `Prefabs/AxisControl.prefab` | A UI row containing a slider driven by a single-axis (float) action. |
| `Prefabs/ButtonControl.prefab` | A UI row that shows a colored indicator driven by a button (bool or float) action. |
| `Prefabs/ControlRow.prefab` | A generic one-row UI container used to lay out individual control visualizations. |
| `Prefabs/Vector2Control.prefab` | A UI row that shows a 2D handle (thumbstick or touchpad) driven by a `Vector2` action. |

## Scripts

The sample implements the following notable scripts:

| Script | Description |
| --- | --- |
| `Scripts/ActionAssetEnabler.cs` | Enables an `InputActionAsset` whenever its `GameObject` is enabled, so all action maps become active without requiring explicit script-side activation. |
| `Scripts/ActionToAxis.cs` | Extends `ActionToControl`. Sets a `Slider` value from the float action value on every callback. |
| `Scripts/ActionToButton.cs` | Extends `ActionToControl`. Changes an `Image` color between a normal and pressed color in response to the action's `started` and `canceled` events. Reveals the image once the action becomes bound. |
| `Scripts/ActionToButtonISX.cs` | Standalone button visualizer that reads action value each frame via `ReadValue<bool/float>`. Handles both bool and float value types to determine the pressed state without relying on action events. |
| `Scripts/ActionToControl.cs` | Abstract base class. Subscribes to `started`, `performed`, and `canceled` events on an `InputActionReference` and polls `OpenXRInput.TryGetInputSourceName` to resolve a human-readable source name for the bound control. Subclasses override the three event callbacks to implement the specific visualization. |
| `Scripts/ActionToDeviceInfo.cs` | Reads the `InputDevice` associated with an action's first control and displays its name, device ID, and usages in a `Text` component. |
| `Scripts/ActionToSliderISX.cs` | Drives a `Slider` value from a float `InputActionReference` each frame, hiding the slider if the action is not active. |
| `Scripts/ActionToVector2.cs` | Extends `ActionToControl`. Repositions a `RectTransform` handle within a 2D area to reflect the `Vector2` action value. |
| `Scripts/ActionToVector2SliderISX.cs` | Drives two separate `Slider` components (X and Y) from a `Vector2` `InputActionReference` each frame. |
| `Scripts/ActionToVisibility.cs` | Shows or hides a `GameObject` by polling `OpenXRInput.TryGetInputSourceName` in a coroutine; the target is made visible once the action has a bound OpenXR source. |
| `Scripts/ActionToVisibilityISX.cs` | Shows or hides a `GameObject` each frame based on whether the action is enabled and has active controls, without requiring an OpenXR source-name query. |
| `Scripts/DisplayDeviceInfoFromActionISX.cs` | Displays the connected device name, ID, and usage strings in a `Text` component each frame. Toggles a root `GameObject` on or off depending on whether a device is present. |
