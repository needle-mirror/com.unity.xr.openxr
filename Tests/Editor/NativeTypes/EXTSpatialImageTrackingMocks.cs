using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrFutureEXT = System.UInt64;
using XrInstance = System.UInt64;
using XrSession = System.UInt64;
using XrSpatialImageTrackingDatabaseEXT = System.UInt64;
using XrSystemId = System.UInt64;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    /// <summary>
    /// Delegate signature for `xrCreateSpatialImageTrackingDatabaseAsyncEXT`.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <param name="session">The `XrSession`.</param>
    /// <param name="createInfo">The create info.</param>
    /// <param name="future">The output future.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrCreateSpatialImageTrackingDatabaseAsyncEXT_delegate(
        XrSession session, in XrSpatialImageTrackingDatabaseCreateInfoEXT createInfo, out XrFutureEXT future);

    /// <summary>
    /// Delegate signature for `xrCreateSpatialImageTrackingDatabaseCompleteEXT`.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <param name="session">The `XrSession`.</param>
    /// <param name="future">The future.</param>
    /// <param name="completion">The completion.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrCreateSpatialImageTrackingDatabaseCompleteEXT_delegate(
        XrSession session, XrFutureEXT future, ref XrCreateSpatialImageTrackingDatabaseCompletionEXT completion);

    /// <summary>
    /// Delegate signature for `xrDestroySpatialImageTrackingDatabaseEXT`.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrDestroySpatialImageTrackingDatabaseEXT_delegate(
        XrSpatialImageTrackingDatabaseEXT database);

    /// <summary>
    /// Delegate signature for `xrEnumerateSpatialReferenceImageFormatsEXT`.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <param name="instance">The `XrInstance`.</param>
    /// <param name="systemId">The system ID.</param>
    /// <param name="capability">The capability.</param>
    /// <param name="formatCapacityInput">The format capacity input.</param>
    /// <param name="formatCountOutput">The format count output.</param>
    /// <param name="formats">Pointer to an array of formats.</param>
    /// <returns>The result of the operation.</returns>
    public unsafe delegate XrResult xrEnumerateSpatialReferenceImageFormatsEXT_delegate(
        XrInstance instance,
        XrSystemId systemId,
        XrSpatialCapabilityEXT capability,
        uint formatCapacityInput,
        out uint formatCountOutput,
        XrSpatialReferenceImageFormatEXT* formats);

    static class EXTSpatialImageTrackingMocks
    {
        internal const XrSpatialImageTrackingDatabaseEXT k_DatabaseHandle = 0x1234;
        internal const XrFutureEXT k_Future = 0x5678;

        [MonoPInvokeCallback(typeof(xrCreateSpatialImageTrackingDatabaseAsyncEXT_delegate))]
        internal static XrResult xrCreateSpatialImageTrackingDatabaseAsyncEXT(
            XrSession session, in XrSpatialImageTrackingDatabaseCreateInfoEXT createInfo, out XrFutureEXT future)
        {
            future = k_Future;
            return XrResult.Success;
        }

        // Rooted so the delegate is not collected while native holds its pointer.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrCreateSpatialImageTrackingDatabaseAsyncEXT_delegate
            xrCreateSpatialImageTrackingDatabaseAsyncEXT_Delegate = xrCreateSpatialImageTrackingDatabaseAsyncEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrCreateSpatialImageTrackingDatabaseAsyncEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrCreateSpatialImageTrackingDatabaseAsyncEXT_Delegate);

        [MonoPInvokeCallback(typeof(xrCreateSpatialImageTrackingDatabaseCompleteEXT_delegate))]
        internal static XrResult xrCreateSpatialImageTrackingDatabaseCompleteEXT(
            XrSession session, XrFutureEXT future, ref XrCreateSpatialImageTrackingDatabaseCompletionEXT completion)
        {
            completion.futureResult = XrResult.Success;
            completion.database = k_DatabaseHandle;
            return XrResult.Success;
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrCreateSpatialImageTrackingDatabaseCompleteEXT_delegate
            xrCreateSpatialImageTrackingDatabaseCompleteEXT_Delegate = xrCreateSpatialImageTrackingDatabaseCompleteEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrCreateSpatialImageTrackingDatabaseCompleteEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrCreateSpatialImageTrackingDatabaseCompleteEXT_Delegate);

        [MonoPInvokeCallback(typeof(xrDestroySpatialImageTrackingDatabaseEXT_delegate))]
        internal static XrResult xrDestroySpatialImageTrackingDatabaseEXT(XrSpatialImageTrackingDatabaseEXT database)
        {
            return XrResult.Success;
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrDestroySpatialImageTrackingDatabaseEXT_delegate
            xrDestroySpatialImageTrackingDatabaseEXT_Delegate = xrDestroySpatialImageTrackingDatabaseEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrDestroySpatialImageTrackingDatabaseEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrDestroySpatialImageTrackingDatabaseEXT_Delegate);

        [MonoPInvokeCallback(typeof(xrEnumerateSpatialReferenceImageFormatsEXT_delegate))]
        internal static unsafe XrResult xrEnumerateSpatialReferenceImageFormatsEXT(
            XrInstance instance,
            XrSystemId systemId,
            XrSpatialCapabilityEXT capability,
            uint formatCapacityInput,
            out uint formatCountOutput,
            XrSpatialReferenceImageFormatEXT* formats)
        {
            formatCountOutput = 2;
            if (formatCapacityInput == 0)
                return XrResult.Success;

            if (formatCapacityInput < 2)
                return XrResult.SizeInsufficient;

            formats[0] = XrSpatialReferenceImageFormatEXT.RGBA_8888;
            formats[1] = XrSpatialReferenceImageFormatEXT.RGB_888;
            return XrResult.Success;
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly unsafe xrEnumerateSpatialReferenceImageFormatsEXT_delegate
            xrEnumerateSpatialReferenceImageFormatsEXT_Delegate = xrEnumerateSpatialReferenceImageFormatsEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrEnumerateSpatialReferenceImageFormatsEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrEnumerateSpatialReferenceImageFormatsEXT_Delegate);
    }
}
