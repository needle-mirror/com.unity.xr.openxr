using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrSpatialContextEXT = System.UInt64;
using XrSpatialSnapshotEXT = System.UInt64;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    /// <summary>
    /// Delegate signature for `xrCreateSpatialRaycastSnapshotANDROID`.
    /// Provided by `XR_ANDROID_spatial_discovery_raycast`.
    /// </summary>
    /// <param name="spatialContext">The spatial context.</param>
    /// <param name="createInfo">The create info.</param>
    /// <param name="snapshot">The output snapshot handle.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrCreateSpatialRaycastSnapshotANDROID_delegate(
        XrSpatialContextEXT spatialContext,
        in XrSpatialRaycastSnapshotCreateInfoANDROID createInfo,
        out XrSpatialSnapshotEXT snapshot);

    static class ANDROIDSpatialDiscoveryRaycastMocks
    {
        internal const XrSpatialSnapshotEXT snapshotHandle = 789;

        /// <summary>
        /// The `componentTypeCount` the runtime last received, so tests can verify what the managed and
        /// `_usingContext` overloads forwarded.
        /// </summary>
        internal static uint lastComponentTypeCount;

        /// <summary>
        /// The raycast info the runtime last received, so tests can verify what the `_usingContext` overload
        /// built natively.
        /// </summary>
        internal static XrSpatialRaycastInfoANDROID lastRaycastInfo;

        [MonoPInvokeCallback(typeof(xrCreateSpatialRaycastSnapshotANDROID_delegate))]
        internal static unsafe XrResult xrCreateSpatialRaycastSnapshotANDROID(
            XrSpatialContextEXT spatialContext,
            in XrSpatialRaycastSnapshotCreateInfoANDROID createInfo,
            out XrSpatialSnapshotEXT snapshot)
        {
            lastComponentTypeCount = createInfo.componentTypeCount;
            lastRaycastInfo = *createInfo.raycastInfo;
            snapshot = snapshotHandle;
            return XrResult.Success;
        }

        internal static IntPtr xrCreateSpatialRaycastSnapshotANDROID_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrCreateSpatialRaycastSnapshotANDROID_delegate)xrCreateSpatialRaycastSnapshotANDROID);
    }
}
