---
uid: openxr-custom-composition-layer-feature-sample
---
# Custom Composition Layer feature sample

Demonstrates how to add support for a customized composition layer type using an OpenXR Feature and the [XR Composition Layers](https://docs.unity3d.com/Packages/com.unity.xr.compositionlayers@latest/) package.

This sample does not include a scene. It contains three scripts that together form a minimal but complete example of defining a new layer geometry type, registering a runtime handler for it, and managing the OpenXR swapchain and native layer lifecycle.

> [!NOTE]
> This sample requires the `XR Composition Layers` package (`com.unity.xr.compositionlayers`). The scripts are wrapped in `#if XR_COMPOSITION_LAYERS` guards and are inactive unless that package is present.

## Scripts

The sample implements the following scripts:

| Script | Description |
| --- | --- |
| `CustomFeature.cs` | An `OpenXRFeature` that instantiates `CustomLayerHandler` and registers it with `OpenXRLayerProvider` when the feature is enabled. Takes care of the timing edge case where the layer provider may already be started before the feature enables. |
| `CustomLayerHandler.cs` | Implements `OpenXRCustomLayerHandler<CustomNativeCompositionLayerQuad>`. Overrides `CreateSwapchain` to build an `XrSwapchainCreateInfo` from the layer's `TexturesExtension`, and overrides `CreateNativeLayer` to populate the `XrCompositionLayerQuad`-equivalent native struct with pose, size, and sub-image data. |
| `CustomQuadLayerData.cs` | Defines a custom `LayerData` subclass representing a quad-shaped composition layer with a configurable width/height size and an option to scale with the GameObject's transform. Decorated with the `[CompositionLayerData]` attribute so it appears in the **Composition Layer** component's type dropdown. |

## Key classes and methods

The following sections list the important Unity and OpenXR types and members used by the sample, organized by namespace.

### `Unity.XR.CompositionLayers.Layers`

| Class / Method | Description |
| --- | --- |
| [`[CompositionLayerData]` attribute](xref:Unity.XR.CompositionLayers.Layers.CompositionLayerDataAttribute) | Applied to a `LayerData` subclass to register it with the Composition Layers system. The `Name` parameter controls what appears in the type dropdown of the **Composition Layer** component inspector. |
| [`LayerData`](xref:Unity.XR.CompositionLayers.Layers.LayerData) | Abstract base class for composition layer data assets. Subclass this to define the geometry and properties of a new layer type. Override `CopyFrom` to support layer duplication. |

### `Unity.XR.CompositionLayers.Extensions`

| Class / Method | Description |
| --- | --- |
| [`TexturesExtension`](xref:Unity.XR.CompositionLayers.Extensions.TexturesExtension) | A standard composition layer extension component that holds the source texture. `CreateSwapchain` reads the texture dimensions and format from this extension when building the `XrSwapchainCreateInfo`. |

### `UnityEngine.XR.OpenXR.CompositionLayers`

| Class / Method | Description |
| --- | --- |
| [`OpenXRCustomLayerHandler<TNativeLayer>`](xref:UnityEngine.XR.OpenXR.CompositionLayers.OpenXRCustomLayerHandler`1) | Abstract generic base class for custom layer handlers. Override `CreateSwapchain` to define the swapchain parameters and `CreateNativeLayer` to build the native OpenXR layer struct that will be submitted to the compositor. |
| [`OpenXRLayerProvider.isStarted`](xref:UnityEngine.XR.OpenXR.CompositionLayers.OpenXRLayerProvider.isStarted) | Returns `true` when the layer provider has been started by the runtime. Used to decide whether to register the handler immediately or to wait for the `Started` event. |
| [`OpenXRLayerProvider.RegisterLayerHandler(Type, ILayerHandler)`](xref:UnityEngine.XR.OpenXR.CompositionLayers.OpenXRLayerProvider.RegisterLayerHandler(System.Type,UnityEngine.XR.OpenXR.CompositionLayers.OpenXRLayerProvider.ILayerHandler)) | Registers a custom layer handler for a specific `LayerData` type. The handler will be invoked whenever the system needs to create or update a layer of that type. |
| [`OpenXRLayerProvider.Started`](xref:UnityEngine.XR.OpenXR.CompositionLayers.OpenXRLayerProvider.Started) | Event raised when the layer provider starts. Subscribe to this event when the feature is enabled before the provider has started to defer handler registration until the provider is ready. |
| [`OpenXRLayerUtility.GetCurrentAppSpace()`](xref:UnityEngine.XR.OpenXR.CompositionLayers.OpenXRLayerUtility.GetCurrentAppSpace) | Returns the `XrSpace` handle for the current application reference space, needed when populating the `Space` field of native layer structs. |
| [`OpenXRLayerUtility.GetDefaultColorFormat()`](xref:UnityEngine.XR.OpenXR.CompositionLayers.OpenXRLayerUtility.GetDefaultColorFormat) | Returns the default swapchain color format negotiated with the runtime. |
| [`OpenXRLayerUtility.GetExtensionsChain`](xref:UnityEngine.XR.OpenXR.CompositionLayers.OpenXRLayerUtility.GetExtensionsChain(Unity.XR.CompositionLayers.Services.CompositionLayerManager.LayerInfo,Unity.XR.CompositionLayers.CompositionLayerExtension.ExtensionTarget)) | Builds the OpenXR `next` extension chain pointer from any extension components attached to the layer. Pass the result to the `Next` field of native structs. |
