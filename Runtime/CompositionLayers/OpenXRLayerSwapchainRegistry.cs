#if XR_COMPOSITION_LAYERS
using System.Collections.Generic;
using UnityEngine.XR.OpenXR.NativeTypes;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    /// <summary>
    /// The live swapchain of each layer, shared by every layer handler. The native plugin keys swapchains by layer id,
    /// so a layer whose type changes keeps one swapchain across the handlers of each type.
    /// </summary>
    static partial class OpenXRLayerSwapchainRegistry
    {
#if LIFECYCLE_APIS_AVAILABLE
        [AutoStaticsCleanup]
#endif
        static readonly Dictionary<int, OpenXRLayerSwapchain> s_Swapchains = new();

#if LIFECYCLE_APIS_AVAILABLE
        [AutoStaticsCleanup]
#endif
        static readonly Dictionary<int, object> s_SwapchainUsers = new();

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static int s_NextVersion;

        internal static OpenXRLayerSwapchain Record(
            int layerId,
            XrSwapchainCreateInfo createInfo,
            bool isExternalSurface,
            bool isStereo,
            ulong handle,
            ulong secondStereoHandle)
        {
            var swapchain = new OpenXRLayerSwapchain(
                createInfo, isExternalSurface, isStereo, handle, secondStereoHandle, ++s_NextVersion);
            s_Swapchains[layerId] = swapchain;
            return swapchain;
        }

        internal static bool TryGet(int layerId, out OpenXRLayerSwapchain swapchain) =>
            s_Swapchains.TryGetValue(layerId, out swapchain);

        /// <summary>
        /// Records the handler using the layer's swapchain, and returns whether a different handler used it last.
        /// </summary>
        internal static bool SetUser(int layerId, object user)
        {
            var hadOtherUser = s_SwapchainUsers.TryGetValue(layerId, out var previousUser) && previousUser != user;
            s_SwapchainUsers[layerId] = user;
            return hadOtherUser;
        }

        internal static void Remove(int layerId)
        {
            s_Swapchains.Remove(layerId);
            s_SwapchainUsers.Remove(layerId);
        }

        internal static void Clear()
        {
            s_Swapchains.Clear();
            s_SwapchainUsers.Clear();
        }
    }
}
#endif
