#if XR_COMPOSITION_LAYERS
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    /// <summary>
    /// A layer's live native swapchain and the description it was created from.
    /// </summary>
    readonly struct OpenXRLayerSwapchain
    {
        internal readonly XrSwapchainCreateInfo CreateInfo;
        internal readonly bool IsExternalSurface;
        internal readonly bool IsStereo;
        internal readonly ulong Handle;
        internal readonly ulong SecondStereoHandle;

        /// <summary>
        /// Distinguishes this swapchain from earlier ones of the same layer, whose handles the runtime may reuse.
        /// </summary>
        internal readonly int Version;

        internal OpenXRLayerSwapchain(
            XrSwapchainCreateInfo createInfo,
            bool isExternalSurface,
            bool isStereo,
            ulong handle,
            ulong secondStereoHandle,
            int version)
        {
            CreateInfo = createInfo;
            IsExternalSurface = isExternalSurface;
            IsStereo = isStereo;
            Handle = handle;
            SecondStereoHandle = secondStereoHandle;
            Version = version;
        }

        /// <summary>
        /// Whether this swapchain's images match a swapchain created from the given description. The extension chain
        /// in <c>Next</c> is not compared.
        /// </summary>
        internal bool CanBeReusedFor(XrSwapchainCreateInfo createInfo, bool isExternalSurface, bool isStereo)
        {
            return IsExternalSurface == isExternalSurface
                && IsStereo == isStereo
                && CreateInfo.CreateFlags == createInfo.CreateFlags
                && CreateInfo.UsageFlags == createInfo.UsageFlags
                && CreateInfo.Format == createInfo.Format
                && CreateInfo.SampleCount == createInfo.SampleCount
                && CreateInfo.Width == createInfo.Width
                && CreateInfo.Height == createInfo.Height
                && CreateInfo.FaceCount == createInfo.FaceCount
                && CreateInfo.ArraySize == createInfo.ArraySize
                && CreateInfo.MipCount == createInfo.MipCount;
        }
    }
}
#endif
