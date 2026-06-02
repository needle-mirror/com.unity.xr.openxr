# Meshing Subsystem Feature Sample

Demonstrates how to author an OpenXR Feature which sets up a custom XR Meshing Subsystem.

The [XR Meshing Subsystem](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/XR.XRMeshSubsystem.html) allows for one to surface procedurally-generated meshes within Unity.  Within OpenXR this functionality can be exposed by creating an `OpenXRFeature` to manage the subsystem.  This sample uses a native plugin to provide a teapot mesh through a [XRMeshingSubsystem](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/XR.XRMeshSubsystem.html) that is managed by an OpenXR feature.

For more documentation about this sample, refer to: https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest?subfolder=samples/meshing-subsystem-feature.html

For more information about the meshing subsystem, refer to the [XR Meshing Subsystem](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/XR.XRMeshSubsystem.html).

For more information on developing a custom feature, refer to the [Integrate OpenXR features](https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest?subfolder=manual/features/feature-integration.html).
