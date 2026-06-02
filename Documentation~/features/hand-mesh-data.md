---
uid: openxr-hand-mesh-data
---

# Hand Mesh Data

This feature provides access to hand mesh geometry through the [XR\_FB\_hand\_tracking\_mesh](https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_FB_hand_tracking_mesh) OpenXR extension. Hand mesh data includes vertex positions, normals, UVs, triangle indices, joint bind poses, and bone weights for skinned hand mesh rendering.

This feature works in conjunction with the [Hand Tracking Subsystem](https://docs.unity3d.com/Packages/com.unity.xr.hands@latest) feature. When you enable both features, hand mesh data is available through the [`XRHandSubsystem.TryGetMeshData`](xref:UnityEngine.XR.Hands.XRHandSubsystem.TryGetMeshData(UnityEngine.XR.Hands.Meshing.XRHandMeshDataQueryResult@,UnityEngine.XR.Hands.Meshing.XRHandMeshDataQueryParams@)) API.

In **OpenXR Project Settings**, this feature appears as **Meta Quest: Hand Mesh Data** and is available for **Android** and **Standalone** build targets.

> [!NOTE]
> The target XR platform must support the `XR_FB_hand_tracking_mesh` OpenXR extension. This extension is currently supported on Meta Quest and Android XR devices. The hand mesh data is immutable for the lifetime of the OpenXR instance. Once retrieved, the geometry doesn't change between frames.

## Requirements

- Install the [XR Hands](https://docs.unity3d.com/Packages/com.unity.xr.hands@latest) package (version 1.9.0 or later).
- Enable the **Hand Tracking Subsystem** OpenXR feature.
- On Android, the application must have hand tracking permissions. When you enable this feature, the build hook configures the Android manifest automatically by adding:
    - `<uses-permission android:name="android.permission.HAND_TRACKING" />`
    - `<uses-permission android:name="com.oculus.permission.HAND_TRACKING" />`
    - `<uses-feature android:name="oculus.software.handtracking" android:required="false" />`
At runtime, request the permission that matches the target platform: `android.permission.HAND_TRACKING` on Android XR devices, or `com.oculus.permission.HAND_TRACKING` on Meta Quest devices.

## Enable the Hand Mesh Data feature

To use hand mesh data, enable the feature in the **OpenXR Project Settings**:

1. Open the **Project Settings** window (menu: **Edit > Project Settings**).
2. Expand the **XR Plug-in Management** section, if necessary.
3. Select the **OpenXR** area under **XR Plug-in Management**.
4. Choose the **Platform Build Target** from the tabs along the top of the **OpenXR** settings page.
5. Check the box next to **Meta Quest: Hand Mesh Data** (in the **All Features** group) to enable the feature.
6. Ensure the **Hand Tracking Subsystem** feature is also enabled.

## Retrieve hand mesh data

Use [`XRHandSubsystem.TryGetMeshData`](xref:UnityEngine.XR.Hands.XRHandSubsystem.TryGetMeshData(UnityEngine.XR.Hands.Meshing.XRHandMeshDataQueryResult@,UnityEngine.XR.Hands.Meshing.XRHandMeshDataQueryParams@)) to retrieve mesh data for both hands:

```csharp
var queryParams = new XRHandMeshDataQueryParams
{
    allocator = Allocator.Temp
};

if (subsystem.TryGetMeshData(out var result, ref queryParams))
{
    using (result)
    {
        // Access left hand mesh data
        var leftHand = result.leftHand;
        if (leftHand.positions.IsCreated)
        {
            // Use leftHand.positions, leftHand.normals, leftHand.uvs,
            // leftHand.indices, leftHand.bonesPerVertex, leftHand.boneWeights
        }

        // Access right hand mesh data
        var rightHand = result.rightHand;
        // ...
    }
}
```

> [!IMPORTANT]
> If [`TryGetMeshData`](xref:UnityEngine.XR.Hands.XRHandSubsystem.TryGetMeshData(UnityEngine.XR.Hands.Meshing.XRHandMeshDataQueryResult@,UnityEngine.XR.Hands.Meshing.XRHandMeshDataQueryParams@)) returns `true`, you're responsible for disposing the result. Use a `using` statement or call `Dispose()` on the [`XRHandMeshDataQueryResult`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshDataQueryResult) when done.

## Available data

The [`XRHandMeshData`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData) struct provides the following data when available:

| Property | Type | Description |
|---|---|---|
| [`positions`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.positions) | `NativeArray<Vector3>` | Vertex positions in session space. |
| [`normals`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.normals) | `NativeArray<Vector3>` | Vertex normals. |
| [`uvs`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.uvs) | `NativeArray<Vector2>` | Texture UV coordinates. |
| [`indices`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.indices) | `NativeArray<int>` | Triangle indices into the vertex arrays. |
| [`bonesPerVertex`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.bonesPerVertex) | `NativeArray<byte>` | Number of bones influencing each vertex. |
| [`boneWeights`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.boneWeights) | `NativeArray<BoneWeight1>` | Bone weights for skinned mesh rendering. |

Use [`TryGetRootPose`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.TryGetRootPose(UnityEngine.Pose@)) to retrieve the root pose for positioning the mesh, and [`TryGetJointBindPoseMatrix`](xref:UnityEngine.XR.Hands.Meshing.XRHandMeshData.TryGetJointBindPoseMatrix(UnityEngine.Matrix4x4@,UnityEngine.XR.Hands.XRHandJointID)) to retrieve bind poses for individual joints.

Check each array's `IsCreated` property before use, because some data is unavailable in certain cases.

## Validation

When enabled, the feature includes a project validation rule that checks whether the **Hand Tracking Subsystem** feature is also enabled. If the Hand Tracking Subsystem isn't enabled, the validation system displays a warning with an automatic fix option.
