using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrFutureEXT = System.UInt64;
using XrSpatialPersistenceContextEXT = System.UInt64;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    static class EXTSpatialPersistenceOperationsMocks
    {
        internal delegate XrResult xrPersistSpatialEntityAsyncEXT_delegate(
            XrSpatialPersistenceContextEXT persistenceContext,
            in XrSpatialEntityPersistInfoEXT persistInfo,
            out XrFutureEXT future);

        internal static IntPtr xrPersistSpatialEntityAsyncEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrPersistSpatialEntityAsyncEXT_delegate)xrPersistSpatialEntityAsyncEXT);

        internal static XrResult xrPersistSpatialEntityAsyncEXT(
            XrSpatialPersistenceContextEXT persistenceContext,
            in XrSpatialEntityPersistInfoEXT persistInfo,
            out XrFutureEXT future)
        {
            future = 123456;
            return XrResult.Success;
        }

        internal delegate XrResult xrPersistSpatialEntityCompleteEXT_delegate(
            XrSpatialPersistenceContextEXT persistenceContext,
            XrFutureEXT future,
            ref XrPersistSpatialEntityCompletionEXT completion);

        internal static IntPtr xrPersistSpatialEntityCompleteEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrPersistSpatialEntityCompleteEXT_delegate)xrPersistSpatialEntityCompleteEXT);

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

        internal delegate XrResult xrUnpersistSpatialEntityAsyncEXT_delegate(
            XrSpatialPersistenceContextEXT persistenceContext,
            in XrSpatialEntityUnpersistInfoEXT unpersistInfo,
            out XrFutureEXT future);

        internal static IntPtr xrUnpersistSpatialEntityAsyncEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrUnpersistSpatialEntityAsyncEXT_delegate)xrUnpersistSpatialEntityAsyncEXT);

        internal static XrResult xrUnpersistSpatialEntityAsyncEXT(
            XrSpatialPersistenceContextEXT persistenceContext,
            in XrSpatialEntityUnpersistInfoEXT unpersistInfo,
            out XrFutureEXT future)
        {
            future = 123456;
            return XrResult.Success;
        }

        internal delegate XrResult xrUnpersistSpatialEntityCompleteEXT_delegate(
            XrSpatialPersistenceContextEXT persistenceContext,
            XrFutureEXT future,
            ref XrUnpersistSpatialEntityCompletionEXT completion);

        internal static IntPtr xrUnpersistSpatialEntityCompleteEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrUnpersistSpatialEntityCompleteEXT_delegate)xrUnpersistSpatialEntityCompleteEXT);

        internal static XrResult xrUnpersistSpatialEntityCompleteEXT(
            XrSpatialPersistenceContextEXT persistenceContext,
            XrFutureEXT future,
            ref XrUnpersistSpatialEntityCompletionEXT completion)
        {
            completion.futureResult = XrResult.Success;
            completion.unpersistResult = XrSpatialPersistenceContextResultEXT.Success;
            return XrResult.Success;
        }
    }
}
