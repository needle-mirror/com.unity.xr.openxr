---
uid: openxr-starter-assets
---
# Starter Assets

This sample includes prefabs that are commonly shared between OpenXR samples, such as the Player Rig, Controllers, and various UI elements. This sample also has an Input Action Asset that contains Actions with typical Input Bindings for use across all OpenXR Samples.

> [!IMPORTANT]
> This sample must be imported into the project for other OpenXR samples to function. This sample contains key scripts and prefabs used in the other samples.

## Scenes

The sample includes the **Starter Scene**. This scene contains a player rig, as well as a menu that allows you to navigate to other OpenXR Samples that are included in the build settings of the project.
The purpose of this sample is to provide a common set of functionality for other samples, as well as serve as a unifying point to allow you to navigate between multiple samples with the appropriate configuration.

## Enabling Navigation to Combined Samples

After importing samples into the project via the Package Manager, add scenes in those samples to the build settings, via **File > Build Profiles > Scene List**. The **Sample Scene Navigator** Prefab allows you to navigate to all scenes added to the Scene List which are located in the default samples path, `Assets/Samples/OpenXR Plugin/<version #>`.

## Prefabs

The sample includes the following notable prefabs:

| Prefab | Description |
| --- | --- |
| `Prefabs/Controller.prefab` | Root controller visualization. Contains child objects for each tracked pose marker and for the control-display panel. |
| `Prefabs/PoseMarker.prefab` | A small visual marker positioned and oriented according to a controller pose action. |
| `Prefabs/PlayerRig.prefab` | A prefab that represents the player. This prefab contains the camera used to navigate the scene, and enables the Input Action Asset to allow for player input. |
| `Prefabs/Sample Scene Navigator.prefab` | A prefab which allows the user to navigate to other OpenXR sample scenes that are included in the project's build settings. |

## Scripts

The sample implements the following scripts:

| Script | Description |
| --- | --- |
| `Scripts/UI/SceneNavigationUI.cs` | Enables functionality for the Sample Scene Navigator prefab. Uses the singleton design pattern, so only 1 Scene Navigator GameObject will be active at any given time|
| `Scripts/Player Rig/PlayerRigSingleton.cs` | Enables functionality for the Player Rig prefab. Uses the singleton design pattern, so only 1 Player Rig GameObject will be active at any given time|
| `Scripts/ActionToHaptics.cs` | Sends a haptic impulse to the controller via `OpenXRInput.SendHapticImpulse` whenever a trigger action fires, with configurable amplitude, duration, and frequency. |
| `Scripts/ActionToVisibility.cs` | Shows or hides a `GameObject` by polling `OpenXRInput.TryGetInputSourceName` in a coroutine; the target is made visible once the action has a bound OpenXR source. |
