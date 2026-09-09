using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrInstance = System.UInt64;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    /// <summary>
    /// Delegate signature for `xrPollFutureEXT`.
    /// Provided by `XR_EXT_future`.
    /// </summary>
    /// <param name="instance">The `XrInstance`.</param>
    /// <param name="pollInfo">The poll info.</param>
    /// <param name="pollResult">The poll result.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrPollFutureEXT_delegate(
        XrInstance instance, in XrFuturePollInfoEXT pollInfo, ref XrFuturePollResultEXT pollResult);

    /// <summary>
    /// Delegate signature for `xrCancelFutureEXT`.
    /// Provided by `XR_EXT_future`.
    /// </summary>
    /// <param name="instance">The `XrInstance`.</param>
    /// <param name="cancelInfo">The cancel info.</param>
    /// <returns>The result of the operation.</returns>
    public delegate XrResult xrCancelFutureEXT_delegate(XrInstance instance, in XrFutureCancelInfoEXT cancelInfo);

    static class EXTFutureMocks
    {
        internal static XrResult xrPollFutureEXT_Ready(
            XrInstance instance, in XrFuturePollInfoEXT pollInfo, ref XrFuturePollResultEXT pollResult)
        {
            pollResult.state = XrFutureStateEXT.Ready;
            return XrResult.Success;
        }

        // Rooted so the delegate is not collected while native holds its pointer.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrPollFutureEXT_delegate xrPollFutureEXT_Ready_Delegate = xrPollFutureEXT_Ready;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrPollFutureEXT_Ready_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrPollFutureEXT_Ready_Delegate);

        internal static XrResult xrCancelFutureEXT(XrInstance instance, in XrFutureCancelInfoEXT cancelInfo)
        {
            return XrResult.Success;
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly xrCancelFutureEXT_delegate xrCancelFutureEXT_Delegate = xrCancelFutureEXT;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        internal static IntPtr xrCancelFutureEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(xrCancelFutureEXT_Delegate);
    }
}
