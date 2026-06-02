---
uid: openxr-graphics-sample
---
# Graphics sample

Showcases and explains the effects of several graphics configurations provided by OpenXR.

Some graphic configurations are only available on certain platforms. The sample makes a best-effort attempt to only show graphics options that are available for the current platform.

## Scenes

The sample includes the **Graphics Sample** scene. This scene showcases and explains several graphics options, and allows the user to toggle between them and see the effects in real time.

## Graphics Options

The sample showcases the following Graphics Options.

| Option | Description |
| --- | --- |
| [Render Mode](xref:openxr-project-config#render-mode) | A setting which determines the OpenXR Runtime's Stereo rendering mode. MultiPass submits a separate draw calls for each eye. SinglePassInstanced submits one draw call for both eyes. Directly sets the property OpenXRSettings.RenderMode|
| [Depth Submission Mode](xref:openxr-project-config#depth-submission-mode) | A setting which determines the OpenXR Runtime's Depth submission mode. Use this setting to test project compatibility with different depth submission modes. It is typically used for sophisticated visual effects. Directly sets the property OpenXRSettings.DepthSubmissionMode |
| [Render Viewport Scale](https://docs.unity3d.com/Manual/xr-graphics-resolution-scaling.html#render-viewport-scale) | Controls how much of the allocated display texture should be used for rendering. A lower setting will result in an image with lower resolution. Directly sets the property XRDisplaySubsystem.scaleOfAllViewports |
| [Render Target Scale](https://docs.unity3d.com/ScriptReference/XR.XRDisplaySubsystem-scaleOfAllViewports.html) | Controls the size of the textures submitted to the display as a multiplier of the display's default resolution. Values less than 1.0 use lower resolution textures, which might improve performance at the expense of a less sharp image. Values greater than 1.0 use higher resolution textures, resulting in a potentially sharper image at a cost to performance and increased memory usage. Directly sets the property XRDisplaySubsystem.scaleOfAllRenderTargets |
| [Mirror Blit Mode](https://docs.unity3d.com/ScriptReference/XR.XRMirrorViewBlitMode.html) | Determines which view from the device should be mirrored to a secondary display. Mirror Blit Modes are only an option on platforms which have device mirroring, such as in the Unity Editor and standalone applications. Calls into the function XRDisplaySubsystem.SetPreferredMirrorBlitMode|

## Additional Options

This sample also provides additional options to allow for a more robust testing of the graphic options in different scenarios.

| Option | Description |
| --- | --- |
| **Time Since Opened** | Displays the time since the scene was opened. |
| **Run In Background** | Toggles whether [Application.RunInBackground](https://docs.unity3d.com/ScriptReference/Application-runInBackground.html) is enabled |
| **Frame Rate Mode** | Used to manipulate the framerate the application will run at. Useful for simulating running an application under heavy loads |
