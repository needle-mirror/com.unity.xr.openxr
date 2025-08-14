using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrInstance = System.UInt64;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    static class EXTFutureMocks
    {
        internal delegate XrResult xrPollFutureEXT_delegate(
            XrInstance instance, in XrFuturePollInfoEXT pollInfo, ref XrFuturePollResultEXT pollResult);

        internal static IntPtr xrPollFutureEXT_Ready_Ptr =
            Marshal.GetFunctionPointerForDelegate((xrPollFutureEXT_delegate)xrPollFutureEXT_Ready);

        internal static XrResult xrPollFutureEXT_Ready(
            XrInstance instance, in XrFuturePollInfoEXT pollInfo, ref XrFuturePollResultEXT pollResult)
        {
            pollResult.state = XrFutureStateEXT.Ready;
            return XrResult.Success;
        }

        internal delegate XrResult xrCancelFutureEXT_delegate(XrInstance instance, in XrFutureCancelInfoEXT cancelInfo);

        internal static IntPtr xrCancelFutureEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate((xrCancelFutureEXT_delegate)xrCancelFutureEXT);

        internal static XrResult xrCancelFutureEXT(XrInstance instance, in XrFutureCancelInfoEXT cancelInfo)
        {
            return XrResult.Success;
        }
    }
}
