#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    /// <summary>
    /// A general-purpose helper class for composition layer support.
    /// </summary>
    public static class OpenXRLayerUtility
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct SourceTextureData
        {
            public bool customRectsEnabled;
            public Rect leftEyeSourceRect;
            public Rect rightEyeSourceRect;
            public Rect leftEyeDestinationRect;
            public Rect rightEyeDestinationRect;
            public Vector2 leftEyeTextureSize;
            public Vector2 rightEyeTextureSize;
        }

        private static Camera _mainCameraCache;
        /// <summary>
        /// Calls the methods in its invocation list when a swapchain is created on the graphics thread inside the UnityOpenXR lib.
        /// </summary>
        public unsafe delegate void SwapchainCallbackDelegate(int layerId, ulong swapchainHandle);

        /// <summary>
        /// Calls the methods in its invocation list when a stereo swapchain is created on the graphics thread inside the UnityOpenXR lib.
        /// </summary>
        public unsafe delegate void StereoSwapchainCallbackDelegate(int layerId, ulong swapchainHandleLeft, ulong swapchainHandleRight);
        internal unsafe delegate void LayerCallbackDelegate(int layerId, XrCompositionLayerBaseHeader* layer);

        private static Dictionary<UInt32, RenderTexture> _textureMap = new Dictionary<UInt32, RenderTexture>();

        /// <summary>
        /// Main camera cache accessor
        /// </summary>
        public static Camera mainCameraCache
        {
            get => _mainCameraCache;
            set => _mainCameraCache = value;
        }

        /// <summary>
        /// Helper method used to gather the extension components attached to a CompositionLayer GameObject.
        /// This method chains the native extension struct pointers of those extension components to initialize an OpenXR native object's Next pointer struct chain.
        /// </summary>
        public static unsafe void* GetExtensionsChain(CompositionLayerManager.LayerInfo layer, CompositionLayerExtension.ExtensionTarget extensionTarget)
        {
            void* extensionsChainHead = null;
            void* extensionsChain = null;

            foreach (var extension in layer.Layer.Extensions)
            {
                // Skip extension if not enabled or not the intended target.
                if (!extension.enabled || extension.Target != extensionTarget)
                    continue;

                var extensionNativeStructPtr = extension.GetNativeStructPtr();

                // Skip extension if no native pointer is provided.
                if (extensionNativeStructPtr == null)
                    continue;

                // Initialize pointer chain if head has not been set.
                if (extensionsChainHead == null)
                {
                    extensionsChainHead = extensionNativeStructPtr;
                    extensionsChain = extensionsChainHead;
                }

                // Chain pointer if head has been initialized.
                else
                {
                    ((XrBaseInStructure*)extensionsChain)->Next = extensionNativeStructPtr;
                    extensionsChain = extensionNativeStructPtr;
                }
            }

            return extensionsChainHead;
        }

        /// <summary>
        /// Helper method used get the current app space for any native composition layer structs that may require an associated XrSpace.
        /// </summary>
        public static ulong GetCurrentAppSpace() => Features.OpenXRFeature.Internal_GetAppSpace(out ulong appSpaceId) ? appSpaceId : 0;

        /// <summary>
        /// Helper method used get the XR session handle for any native composition layer structs that may require an associated XrSession.
        /// </summary>
        public static ulong GetXRSession() => Features.OpenXRFeature.Internal_GetXRSession(out ulong xrSessionHandle) ? xrSessionHandle : 0;

        /// <summary>
        /// Create the <see cref="XrSwapchainCreateInfo"/> struct that is passed to OpenXR SDK to create a swapchain.
        /// </summary>
        public static void CreateSwapchain(int id, XrSwapchainCreateInfo createInfo, bool isExternalSurface = false, SwapchainCallbackDelegate callback = null)
        {
            ext_composition_layers_CreateSwapchain(id, createInfo, isExternalSurface, callback);
        }

        /// <summary>
        /// Create the <see cref="XrSwapchainCreateInfo"/> struct that is passed to OpenXR SDK to create a swapchain for stereo projection, like Projection layer type.
        /// </summary>
        public static void CreateStereoSwapchain(int id, XrSwapchainCreateInfo createInfo, StereoSwapchainCallbackDelegate callback = null)
        {
            ext_composition_layers_CreateStereoSwapchain(id, createInfo, callback);
        }

        /// <summary>
        /// Release swapchain according to the id provided.
        /// </summary>
        public static void ReleaseSwapchain(int id)
        {
            ext_composition_layers_release_swapchain(id);
        }

        /// <summary>
        /// Return swapchain supported color format.
        /// </summary>
        public static Int64 GetDefaultColorFormat()
        {
            return ext_composition_layers_GetUnityDefaultColorFormat();
        }

        /// <summary>
        /// Query the correct XR Textures for rendering and blit the layer textures.
        /// </summary>
        public static void SetActiveLayerAndGetTexture(CompositionLayerManager.LayerInfo layerInfo)
        {
            var tex = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (tex == null || tex.LeftTexture == null || tex.sourceTexture == TexturesExtension.SourceTextureEnum.AndroidSurface)
                return;

            UInt32 texId = ext_compositor_layers_SetActiveLayerAndGetTexture(layerInfo.Id);

            // texId will be 0 if swapchain has no images.
            if (texId == 0)
                return;

            if (!_textureMap.TryGetValue(texId, out var destTex))
            {
                var objs = Resources.FindObjectsOfTypeAll<RenderTexture>();
                var name = $"XR Texture [{texId}]";
                foreach (var rt in objs)
                {
                    if (rt.name == name)
                    {
                        destTex = rt;
                        _textureMap[texId] = rt;
                        break;
                    }
                }
            }

            if (tex.LeftTexture == null || destTex == null)
                return;

            if (layerInfo.Layer.LayerData.GetType() == typeof(CubeProjectionLayerData))
            {
                for (int i = 0; i < 6; i++)
                {
                    Graphics.CopyTexture(tex.LeftTexture, i, destTex, i);
                }
            }
            else
            {
                Graphics.Blit(tex.LeftTexture, destTex);
            }
        }
        /// <summary>
        /// Query the correct XR Textures for rendering and blit the layer textures (For Projection Layer type).
        /// </summary>
        public static void SetActiveLayerAndGetTexturesForProjectionLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            var tex = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (tex == null || tex.LeftTexture == null || tex.RightTexture == null)
                return;

            bool ret = ext_compositor_layers_SetActiveLayerAndGetTexturesForProjectionLayer(layerInfo.Id, out uint leftId, out uint rightId);
            if (!ret)
                return;
            if (!_textureMap.TryGetValue(leftId, out var destTexLeft))
            {
                var objs = Resources.FindObjectsOfTypeAll<RenderTexture>();
                var name = $"XR Texture [{leftId}]";
                foreach (var rt in objs)
                {
                    if (rt.name == name)
                    {
                        destTexLeft = rt;
                        _textureMap[leftId] = rt;
                        break;
                    }
                }
            }

            Graphics.Blit(tex.LeftTexture, destTexLeft);

            if (!_textureMap.TryGetValue(rightId, out var destTexRight))
            {
                var objs = Resources.FindObjectsOfTypeAll<RenderTexture>();
                var name = $"XR Texture [{rightId}]";
                foreach (var rt in objs)
                {
                    if (rt.name == name)
                    {
                        destTexRight = rt;
                        _textureMap[rightId] = rt;
                        break;
                    }
                }
            }

            Graphics.Blit(tex.RightTexture, destTexRight);
        }

        /// <summary>
        /// Layer custom rects settings support.
        /// </summary>
        internal static void SetSourceTexture(int id, SourceTextureData textureData)
        {
            ext_composition_layers_SetSourceTexture(id, textureData);
        }

        /// <summary>
        /// Add native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib - for custom layer type support
        /// </summary>
        public static unsafe void AddActiveLayersToEndFrame(void* layers, void* orders, int count, int layerByteSize)
        {
            ext_composition_layers_AddActiveLayers(layers, orders, count, layerByteSize);
        }

        /// <summary>
        /// Return the Surface object for Android External Surface support (Android only).
        /// </summary>
        public static System.IntPtr GetLayerAndroidSurfaceObject(int layerId)
        {
            IntPtr surfaceObject = IntPtr.Zero;
            if (ext_composition_layers_GetLayerAndroidSurfaceObject(layerId, ref surfaceObject))
            {
                return surfaceObject;
            }
            return IntPtr.Zero;
        }

        private const string LibraryName = "UnityOpenXR";

        [DllImport(LibraryName)]
        internal static extern UInt32 ext_compositor_layers_SetActiveLayerAndGetTexture(int id);

        [DllImport(LibraryName)]
        internal static extern bool ext_compositor_layers_SetActiveLayerAndGetTexturesForProjectionLayer(int id, out UInt32 leftId, out UInt32 rightId);

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_CreateSwapchain(int id, XrSwapchainCreateInfo createInfo, bool isExternalSurface = false, SwapchainCallbackDelegate callback = null);

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_CreateStereoSwapchain(int id, XrSwapchainCreateInfo createInfo, StereoSwapchainCallbackDelegate callback = null);

        [DllImport(LibraryName)]
        internal static extern Int64 ext_composition_layers_GetUnityDefaultColorFormat();

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_release_swapchain(int id);

        [DllImport(LibraryName)]
        internal static extern unsafe void ext_composition_layers_AddActiveLayers(void* layers, void* orders, int count, int size);

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_SetSourceTexture(int id, SourceTextureData textureData);

        [DllImport(LibraryName)]
        internal static extern bool ext_composition_layers_GetLayerAndroidSurfaceObject(int layerId, ref IntPtr surfaceObject);

    }
}

#endif
