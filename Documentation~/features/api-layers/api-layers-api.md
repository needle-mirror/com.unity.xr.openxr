---
uid: openxr-api-layers-api
---
# Manage API Layers

This page describes how to import, remove, enable, disable, and reorder OpenXR API layers in your Unity project.

You can perform the following operations to manage an OpenXR layer for your project:

* [Import](#import): Copies the layer library and manifest files into the `XR/APILayers~` folder and adds the layer to the OpenXR settings.
* [Remove](#remove): Reverses the import operation, removing the files copied to the `XR/APILayers~` folder.
* [Enable](#enable-disable): Updates the OpenXR settings so that the API layer is included in a build and loaded at runtime.
* [Disable](#enable-disable): Updates the OpenXR settings so that the API layer is NOT included in a build and NOT loaded at runtime.
* [Reorder](#reorder): API layers are loaded in the order in which they appear in the [APILayers.collection](xref:UnityEngine.XR.OpenXR.ApiLayers.collection) property. You can change the order of the members of this collection in case one layer must load before or after another layer.

You can perform these operations using the [APILayersFeature](xref:UnityEngine.XR.OpenXR.Features.ApiLayersFeature) and [APILayers](xref:UnityEngine.XR.OpenXR.ApiLayers) classes.

> [!TIP]
> You perform all API layer management in the Unity Editor. These operations modify your Unity project's OpenXR settings and project structure.

If your application or OpenXR API layer has complex setup requirements, you can implement the [ISupport](xref:UnityEngine.XR.OpenXR.ApiLayers.ISupport) interface to handle tasks like configuring environment variables, setting up native callbacks, and integrating custom tooling.
You can also use an ISupport implementation for platform-specific setup and cleaning up resources at shutdown. Refer to [Extend API layer functionality with ISupport](#extend-api-layer-isupport).

## Import an API layer {#import}

To import an API layer into your project, you need the layer's JSON manifest file and its associated native library file (`.dll` on Windows, `.so` on Android).

When you import an API layer, Unity validates:

* The JSON file exists and has a .json extension.
* The JSON contains required fields: `name`, `api_version`, and `library_path`.
* The library file exists at the specified path.
* A layer with the same name and architecture doesn't already exist.
* If any import validation fails, `TryAdd` returns `false` and logs an error message.

Use the [ApiLayers.TryAdd](xref:UnityEngine.XR.OpenXR.ApiLayers.TryAdd*) method to import a layer:

[!code-csharp[ApiLayersFeatureExample.AddLayer](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#AddLayer)]

## Remove an API layer {#remove}

To remove an imported API layer, use the [ApiLayers.TryRemove](xref:UnityEngine.XR.OpenXR.ApiLayers.TryRemove(System.Int32)) method:

[!code-csharp[ApiLayersFeatureExample.RemoveLayer](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#RemoveLayer)]

> [!NOTE]
> Removing a layer deletes both the JSON manifest and library files from the project's API layers directory.

## Enable or disable API layers {#enable-disable}

Control which imported layers are active using the [ApiLayers.SetEnabled](xref:UnityEngine.XR.OpenXR.ApiLayers.SetEnabled(System.String,System.Boolean)) method:

[!code-csharp[ApiLayersFeatureExample.SetLayerEnabled](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#SetLayerEnabled)]

> [!NOTE]
> Api Layers are a part of OpenXRSettings, so any state changes you make made such as enable or disable will persist until that layer is removed.

## Enable or disable for all architectures {#enable-disable-all-architectures}

You can enable or disable a layer for all imported architectures:

[!code-csharp[ApiLayersFeatureExample.SetLayerEnabledAllArchitectures](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#SetLayerEnabledAllArchitectures)]

## Enable by ApiLayer instance {#enable-by-instance}

You can also enable or disable using an [ApiLayer](xref:UnityEngine.XR.OpenXR.ApiLayers.ApiLayer) instance:

[!code-csharp[ApiLayersFeatureExample.SetLayerEnabledWithInstance](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#SetLayerEnabledWithInstance)]

## Reorder API layers {#reorder}

The order of API layers determines the sequence in which they intercept OpenXR calls. Layers earlier in the collection intercept calls first.

You can access layer properties and their indices through the read-only [ApiLayers.collection](xref:UnityEngine.XR.OpenXR.ApiLayers.collection) property:

```csharp
#if UNITY_EDITOR
var apiLayersFeature = settings.GetFeature<UnityEngine.XR.OpenXR.Features.ApiLayersFeature>();
for (int i = 0; i < apiLayersFeature.apiLayers.collection.Count; i++)
{
var layer = apiLayersFeature.apiLayers.collection[i];
Debug.Log($"Index {i}: {layer.name}");
}
#endif
```

> [!NOTE]
> The collection maintains continuous indices from 0 to Count-1. When you move a layer, other layers shift positions automatically to maintain this continuous sequence.
> For example, if you start with a collection of layers, `[A, B, C, D]`, and call `SetIndex(0, 2)` to move layer A to index 2, the final order is: `[B, C, A, D]`.

## Reorder by index {#reorder-by-index}

Use the [ApiLayers.SetIndex](xref:UnityEngine.XR.OpenXR.ApiLayers.SetIndex(System.Int32,System.Int32)) method to change layer ordering:

[!code-csharp[ApiLayersFeatureExample.SetLayerIndex](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#SetLayerIndex)]

## Reorder by name and architecture {#reorder-by-name-architecture}

You can also reorder by specifying a layer name and architecture:

[!code-csharp[ApiLayersFeatureExample.SetLayerIndexWithNameAndArchitecture](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#SetLayerIndexWithNameAndArchitecture)]

## Reorder by ApiLayer instance {#reorder-by-instance}

You can also reorder by using the object instance of a layer:

[!code-csharp[ApiLayersFeatureExample.SetLayerIndexWithInstance](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#SetLayerIndexWithInstance)]

## Access the API layers collection {#}

Access the collection of imported API layers through the [ApiLayers.collection](xref:UnityEngine.XR.OpenXR.ApiLayers.collection) property.

The collection is read-only.
Use the management methods:
* [TryAdd](xref:UnityEngine.XR.OpenXR.ApiLayers.TryAdd(System.String,System.Runtime.InteropServices.Architecture,UnityEditor.BuildTargetGroup,UnityEngine.XR.OpenXR.ApiLayers.ApiLayer@))
* [TryRemove](xref:UnityEngine.XR.OpenXR.ApiLayers.TryRemove(System.Int32))
* [SetEnabled](xref:UnityEngine.XR.OpenXR.ApiLayers.SetEnabled(System.String,System.Boolean))
* [SetIndex](xref:UnityEngine.XR.OpenXR.ApiLayers.SetIndex(System.Int32,System.Int32)) to modify the collection.

[!code-csharp[ApiLayersFeatureExample.ListLayers](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#ListLayers)]

## Extend API layer functionality with ISupport {#extend-api-layer-isupport}

The [ApiLayers.ISupport](xref:UnityEngine.XR.OpenXR.ApiLayers.ISupport) interface allows you to create custom support logic that integrates with the API layers lifecycle. This is useful for:

* Configuring environment variables before layer initialization.
* Setting up debug callbacks or logging systems.
* Performing platform-specific setup required by certain layers

Implement this interface when you need to:

* Perform setup operations before the OpenXR instance is created.
* Configure layer-specific logging or debugging features.
* Clean up resources when the OpenXR instance is destroyed.
* Integrate custom tooling with the API layers workflow.

[!code-csharp[ApiLayersFeatureExample.LayerSupportExample](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#LayerSupportExample)]

## Register an ISupport implementation {#register}

Register your [ISupport](xref:UnityEngine.XR.OpenXR.ApiLayers.ISupport) implementation during subsystem registration to ensure it's available when needed.

The [RuntimeInitializeOnLoadMethod] attribute ensures your support class is registered before the OpenXR subsystem initializes.

[!code-csharp[ApiLayersFeatureExample.RegisterSupport](../../../Tests/Editor/CodeSamples/ApiLayersFeatureExample.cs#RegisterSupport)]
