using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrFutureEXT = System.UInt64;
using XrSpatialPersistenceContextEXT = System.UInt64;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    /// <summary>
    /// Delegate signature for `xrPersistSpatialEntityAsyncEXT`.
    /// Provided by `XR_EXT_spatial_persistence_operations`.
    /// </summary>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="persistInfo">The persist info.</param>
    /// <param name="future">The output future.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrPersistSpatialEntityAsyncEXT_delegate(
        XrSpatialPersistenceContextEXT persistenceContext,
        in XrSpatialEntityPersistInfoEXT persistInfo,
        out XrFutureEXT future);

    /// <summary>
    /// Delegate signature for `xrPersistSpatialEntityCompleteEXT`.
    /// Provided by `XR_EXT_spatial_persistence_operations`.
    /// </summary>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="future">The future.</param>
    /// <param name="completion">The completion.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrPersistSpatialEntityCompleteEXT_delegate(
        XrSpatialPersistenceContextEXT persistenceContext,
        XrFutureEXT future,
        ref XrPersistSpatialEntityCompletionEXT completion);

    /// <summary>
    /// Delegate signature for `xrUnpersistSpatialEntityAsyncEXT`.
    /// Provided by `XR_EXT_spatial_persistence_operations`.
    /// </summary>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="unpersistInfo">The unpersist info.</param>
    /// <param name="future">The output future.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrUnpersistSpatialEntityAsyncEXT_delegate(
        XrSpatialPersistenceContextEXT persistenceContext,
        in XrSpatialEntityUnpersistInfoEXT unpersistInfo,
        out XrFutureEXT future);

    /// <summary>
    /// Delegate signature for `xrUnpersistSpatialEntityCompleteEXT`.
    /// Provided by `XR_EXT_spatial_persistence_operations`.
    /// </summary>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="future">The future.</param>
    /// <param name="completion">The completion.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrUnpersistSpatialEntityCompleteEXT_delegate(
        XrSpatialPersistenceContextEXT persistenceContext,
        XrFutureEXT future,
        ref XrUnpersistSpatialEntityCompletionEXT completion);

    static class EXTSpatialPersistenceOperationsMocks
    {
        internal static XrResult xrPersistSpatialEntityAsyncEXT(
            XrSpatialPersistenceContextEXT persistenceContext,
            in XrSpatialEntityPersistInfoEXT persistInfo,
            out XrFutureEXT future)
        {
            future = 123456;
            return XrResult.Success;
        }

        // Rooted so the delegate is not collected while native holds its pointer.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrPersistSpatialEntityAsyncEXT_delegate
            xrPersistSpatialEntityAsyncEXT_Delegate = xrPersistSpatialEntityAsyncEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrPersistSpatialEntityAsyncEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrPersistSpatialEntityAsyncEXT_Delegate);

        internal static XrResult xrPersistSpatialEntityCompleteEXT(
            XrSpatialPersistenceContextEXT persistenceContext,
            XrFutureEXT future,
            ref XrPersistSpatialEntityCompletionEXT completion)
        {
            completion.futureResult = XrResult.Success;
            completion.persistResult = XrSpatialPersistenceContextResultEXT.Success;
            completion.persistUuid = new(123, 456);
            return XrResult.Success;
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrPersistSpatialEntityCompleteEXT_delegate
            xrPersistSpatialEntityCompleteEXT_Delegate = xrPersistSpatialEntityCompleteEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrPersistSpatialEntityCompleteEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrPersistSpatialEntityCompleteEXT_Delegate);

        internal static XrResult xrUnpersistSpatialEntityAsyncEXT(
            XrSpatialPersistenceContextEXT persistenceContext,
            in XrSpatialEntityUnpersistInfoEXT unpersistInfo,
            out XrFutureEXT future)
        {
            future = 123456;
            return XrResult.Success;
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrUnpersistSpatialEntityAsyncEXT_delegate
            xrUnpersistSpatialEntityAsyncEXT_Delegate = xrUnpersistSpatialEntityAsyncEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrUnpersistSpatialEntityAsyncEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrUnpersistSpatialEntityAsyncEXT_Delegate);

        internal static XrResult xrUnpersistSpatialEntityCompleteEXT(
            XrSpatialPersistenceContextEXT persistenceContext,
            XrFutureEXT future,
            ref XrUnpersistSpatialEntityCompletionEXT completion)
        {
            completion.futureResult = XrResult.Success;
            completion.unpersistResult = XrSpatialPersistenceContextResultEXT.Success;
            return XrResult.Success;
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrUnpersistSpatialEntityCompleteEXT_delegate
            xrUnpersistSpatialEntityCompleteEXT_Delegate = xrUnpersistSpatialEntityCompleteEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrUnpersistSpatialEntityCompleteEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrUnpersistSpatialEntityCompleteEXT_Delegate);
    }
}
