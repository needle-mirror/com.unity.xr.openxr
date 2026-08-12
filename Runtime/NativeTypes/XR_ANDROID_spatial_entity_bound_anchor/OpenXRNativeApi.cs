using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using XrSystemId = System.UInt64;
using XrInstance = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    public static partial class OpenXRNativeApi
    {
        /// <summary>
        /// Enumerates the spatial component types that support anchor attachment on the current system.
        /// Provided by `XR_ANDROID_spatial_entity_bound_anchor`.
        /// </summary>
        /// <param name="instance">The `XrInstance` handle.</param>
        /// <param name="systemId">The `XrSystemId` of the system.</param>
        /// <param name="attachableComponentCapacityInput">
        /// The capacity of the <paramref name="attachableComponents"/> array,
        /// or `0` to request the required capacity.
        /// </param>
        /// <param name="attachableComponentCountOutput">
        /// The number of components written, or the required capacity if
        /// <paramref name="attachableComponentCapacityInput"/> is `0`.
        /// </param>
        /// <param name="attachableComponents">
        /// A pointer to the output array of component types, or `null` if
        /// <paramref name="attachableComponentCapacityInput"/> is `0`.
        /// </param>
        /// <returns>The result of the operation.</returns>
        [DllImport("UnityOpenXR", EntryPoint = "EXT_spatial_entity_bound_anchor_xrEnumerateSpatialAnchorAttachableComponentsANDROID")]
        public static extern unsafe XrResult xrEnumerateSpatialAnchorAttachableComponentsANDROID(
            XrInstance instance,
            XrSystemId systemId,
            uint attachableComponentCapacityInput,
            out uint attachableComponentCountOutput,
            XrSpatialComponentTypeEXT* attachableComponents
        );

        /// <summary>
        /// Enumerates the spatial component types that support anchor attachment on the current system
        /// using the two-step enumeration pattern.
        /// Provided by `XR_ANDROID_spatial_entity_bound_anchor`.
        /// </summary>
        /// <param name="instance">The `XrInstance` handle.</param>
        /// <param name="systemId">The `XrSystemId` of the system.</param>
        /// <param name="allocator">The allocation strategy to use for the output array.</param>
        /// <param name="attachableComponents">
        /// Receives the enumerated component types allocated with the given <paramref name="allocator"/>.
        /// Only valid if the returned result indicates success.
        /// </param>
        /// <returns>The result of the operation.</returns>
        public static unsafe XrResult xrEnumerateSpatialAnchorAttachableComponentsANDROID(
            XrInstance instance,
            XrSystemId systemId,
            Allocator allocator,
            out NativeArray<XrSpatialComponentTypeEXT> attachableComponents
        )
        {
            var result = xrEnumerateSpatialAnchorAttachableComponentsANDROID(instance, systemId, 0, out var count, null);
            if (result.IsError())
            {
                attachableComponents = default;
                return result;
            }

            attachableComponents = new NativeArray<XrSpatialComponentTypeEXT>(checked((int)count), allocator);
            result = xrEnumerateSpatialAnchorAttachableComponentsANDROID(
                instance, systemId, count, out _, (XrSpatialComponentTypeEXT*)attachableComponents.GetUnsafePtr());

            if (result.IsError())
            {
                attachableComponents.Dispose();
                attachableComponents = default;
            }

            return result;
        }

        /// <summary>
        /// Enumerates the spatial component types that support anchor attachment on the current system.
        /// Resolves `XrInstance` and `XrSystemId` from the current context.
        /// Provided by `XR_ANDROID_spatial_entity_bound_anchor`.
        /// </summary>
        /// <param name="attachableComponentCapacityInput">
        /// The capacity of the <paramref name="attachableComponents"/> array,
        /// or `0` to request the required capacity.
        /// </param>
        /// <param name="attachableComponentCountOutput">
        /// The number of components written, or the required capacity if
        /// <paramref name="attachableComponentCapacityInput"/> is `0`.
        /// </param>
        /// <param name="attachableComponents">
        /// A pointer to the output array of component types, or `null` if
        /// <paramref name="attachableComponentCapacityInput"/> is `0`.
        /// </param>
        /// <returns>The result of the operation.</returns>
        [DllImport("UnityOpenXR", EntryPoint = "EXT_spatial_entity_bound_anchor_xrEnumerateSpatialAnchorAttachableComponentsANDROID_UsingContext")]
        public static extern unsafe OpenXRResultStatus xrEnumerateSpatialAnchorAttachableComponentsANDROID(
            uint attachableComponentCapacityInput,
            out uint attachableComponentCountOutput,
            XrSpatialComponentTypeEXT* attachableComponents
        );

        /// <summary>
        /// Enumerates the spatial component types that support anchor attachment on the current system
        /// using the two-step enumeration pattern.
        /// Resolves `XrInstance` and `XrSystemId` from the current context.
        /// Provided by `XR_ANDROID_spatial_entity_bound_anchor`.
        /// </summary>
        /// <param name="allocator">The allocation strategy to use for the output array.</param>
        /// <param name="attachableComponents">
        /// Receives the enumerated component types allocated with the given <paramref name="allocator"/>.
        /// Only valid if the returned result indicates success.
        /// </param>
        /// <returns>The result of the operation.</returns>
        public static unsafe OpenXRResultStatus xrEnumerateSpatialAnchorAttachableComponentsANDROID(
            Allocator allocator, out NativeArray<XrSpatialComponentTypeEXT> attachableComponents
        )
        {
            var result = xrEnumerateSpatialAnchorAttachableComponentsANDROID(0, out var count, null);
            if (result.IsError())
            {
                attachableComponents = default;
                return result;
            }

            attachableComponents = new NativeArray<XrSpatialComponentTypeEXT>(checked((int)count), allocator);
            result = xrEnumerateSpatialAnchorAttachableComponentsANDROID(count, out _, (XrSpatialComponentTypeEXT*)attachableComponents.GetUnsafePtr());

            if (result.IsError())
            {
                attachableComponents.Dispose();
                attachableComponents = default;
            }

            return result;
        }
    }
}
