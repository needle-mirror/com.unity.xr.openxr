---
uid: openxr-meshing-subsystem-feature-sample
---
# Meshing subsystem feature sample

Demonstrates how to author an OpenXR Feature that registers and manages a custom [XR SDK Meshing Subsystem](https://docs.unity3d.com/2023.2/Documentation/Manual/xrsdk-meshing.html).


The [XR SDK Meshing Subsystem](https://docs.unity3d.com/2023.3/Documentation/Manual/xrsdk-meshing.html) allows for one to surface procedurally-generated meshes within Unity.  Within OpenXR this functionality can be exposed by creating an `OpenXRFeature` to manage the subsystem.  This sample uses a native plugin to provide a teapot mesh through a [XRMeshingSubsystem](https://docs.unity3d.com/ScriptReference/XR.XRMeshSubsystem.html) that is managed by an OpenXR feature.

For more information, refer to the [XR SDK Meshing Subsystem](https://docs.unity3d.com/Manual/xrsdk-meshing.html).

For more information on developing a custom feature, refer to the [Integrate OpenXR features](https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest?subfolder=manual/features/feature-integration.html).

## Scenes

The sample includes the **MeshingFeature** scene. This scene contains a `MeshingBehaviour` component that drives the mesh subsystem, and physics objects to demonstrate collision behaviour.

## Prefabs

The sample includes the following Prefabs:

| Prefab | Description |
| --- | --- |
| `DynamicMeshPrefab.prefab` | Instantiated at runtime for each mesh chunk reported by the subsystem. Contains a `MeshFilter` (for visual geometry), a `MeshRenderer` (with a white material), and a `MeshCollider` (for physics). |

## Scripts

The sample implements the following scripts:

| Script | Description |
| --- | --- |
| `MeshingBehaviour.cs` | Polls [XRMeshSubsystem.TryGetMeshInfos](https://docs.unity3d.com/ScriptReference/XR.XRMeshSubsystem.TryGetMeshInfos.html) each frame. For `Added`/`Updated` chunks it instantiates a `DynamicMeshPrefab` (or reuses an existing one) and calls [GenerateMeshAsync](https://docs.unity3d.com/ScriptReference/XR.XRMeshSubsystem.GenerateMeshAsync.html) to populate its `MeshFilter` and `MeshCollider` in the background. |
| `MeshingTeapotFeature.cs` | Custom `OpenXRFeature` that manages the lifecycle of the [XRMeshSubsystem](https://docs.unity3d.com/ScriptReference/XR.XRMeshSubsystem.html). |