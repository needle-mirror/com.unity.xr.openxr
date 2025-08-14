using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrSpatialContextEXT = System.UInt64;
using XrSpatialEntityEXT = System.UInt64;
using XrSpatialEntityIdEXT = System.UInt64;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    static class EXTSpatialAnchorMocks
    {
        internal delegate XrResult xrCreateSpatialAnchorEXT_delegate(
            XrSpatialContextEXT spatialContext,
            in XrSpatialAnchorCreateInfoEXT createInfo,
            out XrSpatialEntityIdEXT anchorEntityId,
            out XrSpatialEntityEXT anchorEntity);

        internal static IntPtr xrCreateSpatialAnchorEXT_Ptr =
            Marshal.GetFunctionPointerForDelegate(
                (xrCreateSpatialAnchorEXT_delegate)xrCreateSpatialAnchorEXT);

        [MonoPInvokeCallback(typeof(xrCreateSpatialAnchorEXT_delegate))]
        internal static XrResult xrCreateSpatialAnchorEXT(
            XrSpatialContextEXT spatialContext,
            in XrSpatialAnchorCreateInfoEXT createInfo,
            out XrSpatialEntityIdEXT anchorEntityId,
            out XrSpatialEntityEXT anchorEntity)
        {
            anchorEntityId = 123;
            anchorEntity = 456;
            return XrResult.Success;
        }
    }
}
