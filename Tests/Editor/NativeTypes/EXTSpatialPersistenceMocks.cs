using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrFutureEXT = System.UInt64;
using XrInstance = System.UInt64;
using XrSession = System.UInt64;
using XrSpatialPersistenceContextEXT = System.UInt64;
using XrSystemId = System.UInt64;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    static class EXTSpatialPersistenceMocks
    {
        internal delegate XrResult xrCreateSpatialPersistenceContextAsyncEXT_delegate(
            XrSession session, in XrSpatialPersistenceContextCreateInfoEXT createInfo, out XrFutureEXT future);

        internal static IntPtr xrCreateSpatialPersistenceContextAsyncEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrCreateSpatialPersistenceContextAsyncEXT_delegate)xrCreateSpatialPersistenceContextAsyncEXT);

        internal static XrResult xrCreateSpatialPersistenceContextAsyncEXT(
            XrSession session, in XrSpatialPersistenceContextCreateInfoEXT createInfo, out XrFutureEXT future)
        {
            future = 123456;
            return XrResult.Success;
        }

        internal unsafe delegate XrResult xrEnumerateSpatialPersistenceScopesEXT_delegate(
            XrInstance instance,
            XrSystemId systemId,
            uint persistenceScopeCapacityInput,
            out uint persistenceScopeCountOutput,
            XrSpatialPersistenceScopeEXT* persistenceScopes);

        internal static unsafe IntPtr xrEnumerateSpatialPersistenceScopesEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrEnumerateSpatialPersistenceScopesEXT_delegate)xrEnumerateSpatialPersistenceScopesEXT);

        internal static unsafe XrResult xrEnumerateSpatialPersistenceScopesEXT(
            XrInstance instance,
            XrSystemId systemId,
            uint persistenceScopeCapacityInput,
            out uint persistenceScopeCountOutput,
            XrSpatialPersistenceScopeEXT* persistenceScopes)
        {
            persistenceScopeCountOutput = 2;
            if (persistenceScopeCapacityInput < 2)
                return XrResult.Success;

            persistenceScopes[0] = XrSpatialPersistenceScopeEXT.SystemManaged;
            persistenceScopes[1] = XrSpatialPersistenceScopeEXT.LocalAnchors;
            return XrResult.Success;
        }

        internal delegate XrResult xrCreateSpatialPersistenceContextCompleteEXT_delegate(
            XrSession session,
            XrFutureEXT future,
            ref XrCreateSpatialPersistenceContextCompletionEXT completion);

        internal static IntPtr xrCreateSpatialPersistenceContextCompleteEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrCreateSpatialPersistenceContextCompleteEXT_delegate)xrCreateSpatialPersistenceContextCompleteEXT);

        internal static XrResult xrCreateSpatialPersistenceContextCompleteEXT(
            XrSession session,
            XrFutureEXT future,
            ref XrCreateSpatialPersistenceContextCompletionEXT completion)
        {
            completion.futureResult = XrResult.Success;
            completion.createResult = XrSpatialPersistenceContextResultEXT.Success;
            completion.persistenceContext = 123456;
            return XrResult.Success;
        }

        internal delegate XrResult xrDestroySpatialPersistenceContextEXT_delegate(
            XrSpatialPersistenceContextEXT persistenceContext);

        internal static IntPtr xrDestroySpatialPersistenceContextEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrDestroySpatialPersistenceContextEXT_delegate)xrDestroySpatialPersistenceContextEXT);

        internal static XrResult xrDestroySpatialPersistenceContextEXT(
            XrSpatialPersistenceContextEXT persistenceContext)
        {
            return XrResult.Success;
        }
    }
}
