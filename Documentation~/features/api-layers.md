---
uid: openxr-api-layers
---

# OpenXR API Layers support

An OpenXR API layer is a feature that allows you to intercept and modify communication between your application and the OpenXR runtime. Common uses of API layers include debugging, logging, and validation of OpenXR API calls.

The Unity API layer feature lets you import and enable OpenXR API layers implemented as pre-compiled native libraries in your project. You can use the [API Layers](xref:openxr-api-layers-overview) classes and methods to create Editor scripts to manage which API layers are included and active in your project builds.

| Topics | Description |
|:-- | :-- |
| [API layers overview](xref:openxr-api-layers-overview) | Provides an overview of the OpenXR API layers feature and requirements to use it in a Unity application.
| [Manage API layers](xref:openxr-api-layers-api) | Describes how to import, remove, enable, disable, and reorder OpenXR API layers in your Unity project.

> [!IMPORTANT]
> The API layer feature provided by the Unity OpenXR package only deals with how to manage OpenXR API layers. Implementing OpenXR API layers, including programming and compiling the required native libraries, is beyond the scope of this documentation.

## Additional resources

* [ApiLayers class](xref:UnityEngine.XR.OpenXR.ApiLayers)
* [ApiLayersFeature class](xref:UnityEngine.XR.OpenXR.Features.ApiLayersFeature)
* [Khronos OpenXR 1.0 Spec: API Layers](https://registry.khronos.org/OpenXR/specs/1.0-khr/html/xrspec.html#api-layers)
* [Khronos OpenXR Loader - Design and Operation: API Layers](https://registry.khronos.org/OpenXR/specs/1.0/loader.html#openxr-api-layers)
* [KhronosGroup OpenXR-SDK-Source: Api Layers](https://github.com/KhronosGroup/OpenXR-SDK-Source/blob/main/src/api_layers/README.md)
