#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    /// <summary>
    /// A general-purpose helper class for composition layer support.
    /// </summary>
    public static class OpenXRLayerUtility
    {
        internal unsafe delegate void LayerCallbackDelegate(int layerId, XrCompositionLayerBaseHeader* layer);

        static Dictionary<UInt32, RenderTexture> _textureMap = new Dictionary<UInt32, RenderTexture>();

        /// <summary>
        /// Calls the methods in its invocation list when a swapchain is created on the graphics thread inside the UnityOpenXR lib.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="swapchainHandle">The handle to the native swapchain that was just created.</param>
        public unsafe delegate void SwapchainCallbackDelegate(int layerId, ulong swapchainHandle);

        /// <summary>
        /// Calls the methods in its invocation list when a stereo swapchain is created on the graphics thread inside the UnityOpenXR lib.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="swapchainHandleLeft">The handle to one of the stereo swapchains that was just created.</param>
        /// <param name="swapchainHandleRight">The handle to one of the stereo swapchains that was just created.</param>
        public unsafe delegate void StereoSwapchainCallbackDelegate(int layerId, ulong swapchainHandleLeft, ulong swapchainHandleRight);

        /// <summary>
        /// Helper method used to gather the extension components attached to a CompositionLayer GameObject.
        /// This method chains the native extension struct pointers of those extension components to initialize an OpenXR native object's Next pointer struct chain.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer.</param>
        /// <param name="extensionTarget"> Represents what part of the composition layer to retrieve extensions for.</param>
        /// <returns>A pointer to the head of an array of native extension objects that will be associated with a composition layer.</returns>
        public static unsafe void* GetExtensionsChain(CompositionLayerManager.LayerInfo layerInfo, CompositionLayerExtension.ExtensionTarget extensionTarget)
        {
            void* extensionsChainHead = null;
            void* extensionsChain = null;

            foreach (var extension in layerInfo.Layer.Extensions)
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
        /// <returns>A handle to the current app space.</returns>
        /// <remarks>Normally used when creating native composition layers.</remarks>
        public static ulong GetCurrentAppSpace() => Features.OpenXRFeature.Internal_GetAppSpace(out ulong appSpaceId) ? appSpaceId : 0;

        /// <summary>
        /// Helper method used get the XR session handle for any native composition layer structs that may require an associated XrSession.
        /// </summary>
        /// <returns>A handle to the current xr session.</returns>
        public static ulong GetXRSession() => Features.OpenXRFeature.Internal_GetXRSession(out ulong xrSessionHandle) ? xrSessionHandle : 0;

        /// <summary>
        /// Create the <see cref="XrSwapchainCreateInfo"/> struct that is passed to OpenXR SDK to create a swapchain.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="createInfo">The struct used to create the swapchain.</param>
        /// <param name="isExternalSurface"> Optional parameter that can be used when an external surface will be used, like when using the Android Surface feature.</param>
        /// <param name="callback"> Optional parameter that can be used if your composition layer needs to know the handle after swapchain creation.</param>
        public static void CreateSwapchain(int layerId, XrSwapchainCreateInfo createInfo, bool isExternalSurface = false, SwapchainCallbackDelegate callback = null)
        {
            ext_composition_layers_CreateSwapchain(layerId, createInfo, isExternalSurface, callback);
        }

        /// <summary>
        /// Create the <see cref="XrSwapchainCreateInfo"/> struct that is passed to OpenXR SDK to create a swapchain for stereo projection, like Projection layer type.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="createInfo">The struct used to create the swapchain.</param>
        /// <param name="callback"> Optional parameter that can be used if your composition layer needs to know the handles after swapchain creation.</param>
        public static void CreateStereoSwapchain(int layerId, XrSwapchainCreateInfo createInfo, StereoSwapchainCallbackDelegate callback = null)
        {
            ext_composition_layers_CreateStereoSwapchain(layerId, createInfo, callback);
        }

        /// <summary>
        /// Release swapchain according to the id provided.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        public static void ReleaseSwapchain(int layerId)
        {
            ext_composition_layers_ReleaseSwapchain(layerId);
        }

        /// <summary>
        /// Return swapchain supported color format.
        /// </summary>
        /// <returns>The color format the swapchains will be using.</returns>
        public static Int64 GetDefaultColorFormat()
        {
            return ext_composition_layers_GetUnityDefaultColorFormat();
        }

        /// <summary>
        /// Finds the render texture of the give texture id.
        /// </summary>
        /// <param name="texId">The id of the render texture to find.</param>
        /// <returns>The render texture with the provided id or null if no render textrue with that id was found.</returns>
        public static RenderTexture FindRenderTexture(UInt32 texId)
        {
            // texId will be 0 if swapchain has no images.
            if (texId == 0)
                return null;

            if (!_textureMap.TryGetValue(texId, out var renderTexture))
            {
                var objs = Resources.FindObjectsOfTypeAll<RenderTexture>();
                var name = $"XR Texture [{texId}]";
                foreach (var rt in objs)
                {
                    if (rt.name == name)
                    {
                        renderTexture = rt;
                        _textureMap[texId] = rt;
                        break;
                    }
                }
            }

            return renderTexture;
        }

        /// <summary>
        /// Finds the render texture of the layer id.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer.</param>
        /// <returns>The render texture with the provided id or null if no render textrue with that id was found.</returns>
        public static RenderTexture FindRenderTexture(CompositionLayerManager.LayerInfo layerInfo)
        {
            UInt32 texId = ext_compositor_layers_CreateOrGetRenderTextureId(layerInfo.Id);
            return FindRenderTexture(texId);
        }


        /// <summary>
        /// Handles transfering texture data to a render texture.
        /// </summary>
        /// <param name="texture">The source texture that will be written into the provided render texture.</param>
        /// <param name="renderTexture">The render texture that will be written to.</param>
        public static void WriteToRenderTexture(Texture texture, RenderTexture renderTexture)
        {
            if (texture == null || renderTexture == null)
                return;

            if (renderTexture.dimension == Rendering.TextureDimension.Cube && texture.dimension == Rendering.TextureDimension.Cube)
                for (int i = 0; i < 6; i++)
                    Graphics.CopyTexture(texture, i, renderTexture, i);
            else
                Graphics.Blit(texture, renderTexture);
        }

        /// <summary>
        /// Query the correct XR Textures for rendering and blit the layer textures.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer.</param>
        /// <param name="texture">The source texture that will be written into the provided render texture.</param>
        /// <param name="renderTexture">The render texture that will be searched for and written to.
        /// Will be null if no render texture can be found for the provided layerInfo object.</param>
        /// <returns>True if a render texture was found and written to, false if the provided texture is null or if no render texture was found for the provided layerInfo object.</returns>
        public static bool FindAndWriteToRenderTexture(CompositionLayerManager.LayerInfo layerInfo, Texture texture, out RenderTexture renderTexture)
        {
            if (texture == null)
            {
                renderTexture = null;
                return false;
            }

            renderTexture = FindRenderTexture(layerInfo);
            WriteToRenderTexture(texture, renderTexture);
            return renderTexture != null;
        }

        /// <summary>
        /// Query the correct XR Textures for rendering and blit the layer textures (For Projection Layer type).
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer.</param>
        /// <param name="renderTextureLeft">The left stereo render texture that will be searched for and written to.
        /// Will be null if no render textures can be found for the provided layerInfo object.</param>
        /// <param name="renderTextureRight">The right stereo render texture that will be searched for and written to.
        /// Will be null if no render textures can be found for the provided layerInfo object.</param>
        /// <returns>True if both render textures were found and written to, false if no texture was found on the TexturesExtension component of the layerInfo object or if no render texture was found for the provided layerInfo object.</returns>

        public static bool FindAndWriteToStereoRenderTextures(CompositionLayerManager.LayerInfo layerInfo, out RenderTexture renderTextureLeft, out RenderTexture renderTextureRight)
        {
            var tex = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (tex == null || tex.LeftTexture == null || tex.RightTexture == null)
            {
                renderTextureLeft = null;
                renderTextureRight = null;
                return false;
            }

            ext_compositor_layers_CreateOrGetStereoRenderTextureIds(layerInfo.Id, out uint leftId, out uint rightId);
            renderTextureLeft = FindRenderTexture(leftId);
            renderTextureRight = FindRenderTexture(rightId);
            WriteToRenderTexture(tex.LeftTexture, renderTextureLeft);
            WriteToRenderTexture(tex.RightTexture, renderTextureRight);
            return renderTextureLeft != null && renderTextureRight != null;
        }

        /// <summary>
        /// Add native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib - for custom layer type support
        /// </summary>
        /// <param name="layers">Pointer to the native array of currently active composition layers.</param>
        /// <param name="orders">Pointer to the native array of order values for the currently active composition layers.</param>
        /// <param name="count">Indicates the size of the layers and orders arrays.</param>
        /// <param name="layerByteSize">Indicates the size in bytes of a single element of the given array of composition layers.</param>
        /// <remarks>Layers sent must all be of the same type.Demonstrated in the OpenXRCustomLayerHandler class.</remarks>
        public static unsafe void AddActiveLayersToEndFrame(void* layers, void* orders, int count, int layerByteSize)
        {
            ext_composition_layers_AddActiveLayers(layers, orders, count, layerByteSize);
        }

        /// <summary>
        /// Return the Surface object for Android External Surface support (Android only).
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <returns>Pointer to the android surface object.</returns>
        public static System.IntPtr GetLayerAndroidSurfaceObject(int layerId)
        {
            IntPtr surfaceObject = IntPtr.Zero;
            if (ext_composition_layers_GetLayerAndroidSurfaceObject(layerId, ref surfaceObject))
            {
                return surfaceObject;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Sends an array of extensions to be attached to the native default compostion layer.
        /// </summary>
        /// <param name="extensions">Pointer to the array of extensions to attach to the default compostion layer.</param>
        /// <remarks>Currently only called by the OpenXRDefautLayer class.</remarks>
        public static unsafe void SetDefaultSceneLayerExtensions(void* extensions)
        {
            ext_composition_layers_SetDefaultSceneLayerExtensions(extensions);
        }

        /// <summary>
        /// Sends what flags are to be added to the native default compostion layer.
        /// </summary>
        /// <param name="flags">Flags to be added to the native default compostion layer.</param>
        /// <remarks>Currently only called by the OpenXRDefautLayer class.</remarks>
        public static unsafe void SetDefaultLayerFlags(XrCompositionLayerFlags flags)
        {
            ext_composition_layers_SetDefaultSceneLayerFlags(flags);
        }

        const string LibraryName = "UnityOpenXR";

        [DllImport(LibraryName)]
        internal static extern UInt32 ext_compositor_layers_CreateOrGetRenderTextureId(int id);

        [DllImport(LibraryName)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool ext_compositor_layers_CreateOrGetStereoRenderTextureIds(int id, out UInt32 leftId, out UInt32 rightId);

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_CreateSwapchain(int id, XrSwapchainCreateInfo createInfo, [MarshalAs(UnmanagedType.I1)]bool isExternalSurface = false, SwapchainCallbackDelegate callback = null);

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_CreateStereoSwapchain(int id, XrSwapchainCreateInfo createInfo, StereoSwapchainCallbackDelegate callback = null);

        [DllImport(LibraryName)]
        internal static extern Int64 ext_composition_layers_GetUnityDefaultColorFormat();

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_ReleaseSwapchain(int id);

        [DllImport(LibraryName)]
        internal static extern unsafe void ext_composition_layers_AddActiveLayers(void* layers, void* orders, int count, int size);

        [DllImport(LibraryName)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool ext_composition_layers_GetLayerAndroidSurfaceObject(int layerId, ref IntPtr surfaceObject);

        [DllImport(LibraryName)]
        internal static extern unsafe void ext_composition_layers_SetDefaultSceneLayerExtensions(void* extensions);

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_SetDefaultSceneLayerFlags(XrCompositionLayerFlags flags);
    }
}

#endif
