---
uid: openxr-feature-integration
---
# Integrate OpenXR features

OpenXR is an extensible API that you can extend with new features. To facilitate this within the Unity ecosystem, the Unity OpenXR provider supports a feature extension mechanism.

> [!IMPORTANT]
> This section of the OpenXR package documentation is for developers integrating the OpenXR features provided by an XR device. If you are developing an XR application, refer to [Feature Management](xref:openxr-features) for information about how to enable features provided by the XR packages you have installed in your project. App developers typically don't create features or feature groups.

Examples of what you can do with a custom feature include:

* **Intercepting OpenXR function calls**: To intercept OpenXR function calls, override `OpenXRFeature.HookGetInstanceProcAddr`. Returning a different function pointer allows intercepting any OpenXR method. For an example, refer to the `Intercept Feature` sample.

* **Calling OpenXR functions from a feature**: To call an OpenXR function within a feature you first need to retrieve a pointer to the function. To do this use the `OpenXRFeature.xrGetInstanceProcAddr` function pointer to request a pointer to the function you want to call. Using  `OpenXRFeature.xrGetInstanceProcAddr` to retrieve the function pointer ensures that any intercepted calls set up by features using `OpenXRFeature.HookGetInstanceProcAddr` will be included.

* **Providing a Unity subsystem implementation**: `OpenXRFeature` provides several XR Loader callbacks where you can manage the lifecycle of Unity subsystems. For an example meshing subsystem feature, refer to the `Meshing Subsystem Feature` sample.

   > [!NOTE]
   > A `UnitySubsystemsManifest.json` file is required in order for Unity to discover any subsystems you define. At the moment, there are several restrictions around this file:
   >
   > * It must be only 1 subfolder deep in the project or package.
   > * The native library it refers to must be only 1-2 subfolders deeper than the `UnitySubsystemsManfiest.json` file.
   > * The native library it refers to must be only 1-2 subfolders deeper than the `UnitySubsystemsManifest.json` file.
* **Supporting OpenXR interaction profiles**: To support OpenXR interaction profiles that aren't currently supported by the Unity OpenXR settings, you can implement a `OpenXRInteractionFeature` subclass.

* **Requiring a more recent or custom OpenXR loader library**: you can specify that your feature requires a minimum version of the standard OpenXR loader library or that it requires a custom loader library that you supply.

Refer to the following topics for information about implementing OpenXR features:

| Topic | Description |
| :---  | :---------- |
| [Create OpenXR features](xref:openxr-create-features) | How to add features. |
| [Define OpenXR feature groups](xref:openxr-feature-groups) | How to define groups to organize OpenXR features. |
| [Create OpenXR interaction features](xref:openxr-interaction-feature) | How to add support for interaction profiles. |
| [Require a custom loader library](xref:openxr-loader-library) | How to require a custom loader library for features. |
