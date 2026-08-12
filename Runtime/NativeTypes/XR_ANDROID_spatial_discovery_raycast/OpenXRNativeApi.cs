using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using XrSpatialContextEXT = System.UInt64;
using XrSpatialSnapshotEXT = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    public static partial class OpenXRNativeApi
    {
        /// <summary>
        /// Synchronously creates a snapshot of the spatial entities that intersect a ray, allowing you to query
        /// raycast results immediately. Provided by `XR_ANDROID_spatial_discovery_raycast`.
        /// </summary>
        /// <param name="spatialContext">A spatial context previously created using
        /// `OpenXRNativeApi.xrCreateSpatialContextAsyncEXT`.</param>
        /// <param name="createInfo">The creation info, including the ray to cast.</param>
        /// <param name="snapshot">The created snapshot handle.</param>
        /// <returns>The result of the operation.\
        /// \
        /// Success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        ///   <item><description><see cref="XrResult.LossPending"/></description></item>
        /// </list>
        /// Failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.LimitReached"/></description></item>
        ///   <item><description><see cref="XrResult.OutOfMemory"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.SessionLost"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialComponentNotEnabledEXT"/></description></item>
        ///   <item><description><see cref="XrResult.TimeInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        [DllImport(
            InternalConstants.openXRLibrary,
            EntryPoint = "ANDROID_spatial_discovery_raycast_xrCreateSpatialRaycastSnapshotANDROID")]
        public static extern XrResult xrCreateSpatialRaycastSnapshotANDROID(
            XrSpatialContextEXT spatialContext,
            in XrSpatialRaycastSnapshotCreateInfoANDROID createInfo,
            out XrSpatialSnapshotEXT snapshot);

        /// <summary>
        /// Synchronously creates a snapshot of the spatial entities that intersect a ray, using your app space and
        /// frame's predicted display time. Provided by `XR_ANDROID_spatial_discovery_raycast`.
        /// </summary>
        /// <param name="spatialContext">A spatial context previously created using
        /// `OpenXRNativeApi.xrCreateSpatialContextAsyncEXT`.</param>
        /// <param name="origin">The origin of the ray, in session space relative to your app space
        /// (XR Origin), not Unity world space.</param>
        /// <param name="direction">The direction of the ray, in session space relative to your app space
        /// (XR Origin), not Unity world space.</param>
        /// <param name="maxDistance">The maximum distance in meters, or `0` for an unbounded ray.</param>
        /// <param name="componentTypeCount">The count of elements in <paramref name="componentTypes"/>. May be
        /// `0`.</param>
        /// <param name="componentTypes">Pointer to an array of component types to include in the snapshot. May be
        /// `null` to include all hit entities.</param>
        /// <param name="snapshot">The created snapshot handle.</param>
        /// <returns>The result of the operation.\
        /// \
        /// `nativeStatusCode` success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        ///   <item><description><see cref="XrResult.LossPending"/></description></item>
        /// </list>
        /// `nativeStatusCode` failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.LimitReached"/></description></item>
        ///   <item><description><see cref="XrResult.OutOfMemory"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.SessionLost"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialComponentNotEnabledEXT"/></description></item>
        ///   <item><description><see cref="XrResult.TimeInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        [DllImport(
            InternalConstants.openXRLibrary,
            EntryPoint = "ANDROID_spatial_discovery_raycast_xrCreateSpatialRaycastSnapshotANDROID_usingContext")]
        public static extern unsafe OpenXRResultStatus xrCreateSpatialRaycastSnapshotANDROID(
            XrSpatialContextEXT spatialContext,
            in XrVector3f origin,
            in XrVector3f direction,
            float maxDistance,
            uint componentTypeCount,
            XrSpatialComponentTypeEXT* componentTypes,
            out XrSpatialSnapshotEXT snapshot);

        /// <summary>
        /// Synchronously creates a snapshot of the spatial entities that intersect a ray, using your app space and
        /// frame's predicted display time. Provided by `XR_ANDROID_spatial_discovery_raycast`.
        /// </summary>
        /// <param name="spatialContext">A spatial context previously created using
        /// `OpenXRNativeApi.xrCreateSpatialContextAsyncEXT`.</param>
        /// <param name="origin">The origin of the ray, in session space relative to your app space
        /// (XR Origin), not Unity world space.</param>
        /// <param name="direction">The direction of the ray, in session space relative to your app space
        /// (XR Origin), not Unity world space.</param>
        /// <param name="maxDistance">The maximum distance in meters, or `0` for an unbounded ray.</param>
        /// <param name="componentTypes">Native array of component types to include in the snapshot. Pass an empty
        /// array to include all hit entities.</param>
        /// <param name="snapshot">The created snapshot handle.</param>
        /// <returns>The result of the operation.\
        /// \
        /// `nativeStatusCode` success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        ///   <item><description><see cref="XrResult.LossPending"/></description></item>
        /// </list>
        /// `nativeStatusCode` failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.LimitReached"/></description></item>
        ///   <item><description><see cref="XrResult.OutOfMemory"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.SessionLost"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialComponentNotEnabledEXT"/></description></item>
        ///   <item><description><see cref="XrResult.TimeInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        public static unsafe OpenXRResultStatus xrCreateSpatialRaycastSnapshotANDROID(
            XrSpatialContextEXT spatialContext,
            in XrVector3f origin,
            in XrVector3f direction,
            float maxDistance,
            NativeArray<XrSpatialComponentTypeEXT> componentTypes,
            out XrSpatialSnapshotEXT snapshot)
            => xrCreateSpatialRaycastSnapshotANDROID(
                spatialContext,
                origin,
                direction,
                maxDistance,
                (uint)componentTypes.Length,
                (XrSpatialComponentTypeEXT*)componentTypes.GetUnsafePtr(),
                out snapshot);
    }
}
