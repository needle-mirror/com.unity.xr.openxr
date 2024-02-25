#if XR_COMPOSITION_LAYERS
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using UnityEngine.XR.OpenXR.NativeTypes;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using System;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    //Default OpenXR Composition Layer - Quad Layer support
    internal class OpenXRQuadLayer : OpenXRLayerProvider.ILayerHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct QuadLayerParam
        {
            internal int id;
            internal void* next;
            internal Vector3 position;
            internal Quaternion rotation;
            internal Vector2 size;
            internal int width;
            internal int height;
            internal BlendType blendType;
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a new <see cref="LayerData"/>
        /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
        /// </summary>
        public unsafe void CreateLayer(CompositionLayerManager.LayerInfo layer)
        {
            var data = layer.Layer.LayerData as QuadLayerData;
            TexturesExtension texture = layer.Layer.GetComponent<TexturesExtension>();
            if (texture == null || texture.enabled == false)
                return;

            if (texture.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texture.LeftTexture != null)
            {
                // CreateSwapchain ...
                OpenXRLayerUtility.CreateSwapchain(layer.Id, new XrSwapchainCreateInfo()
                {
                    Type = 9,
                    Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                    CreateFlags = 0,
                    UsageFlags = 0x00000020 | 0x00000001,
                    Format = OpenXRLayerUtility.GetDefaultColorFormat(),
                    SampleCount = 1,
                    Width = (uint) (texture.LeftTexture.width),
                    Height = (uint) (texture.LeftTexture.height),
                    FaceCount = 1,
                    ArraySize = 1,
                    MipCount = (uint) layer.Layer.GetComponent<TexturesExtension>().LeftTexture.mipmapCount,
                });

                var transform = layer.Layer.GetComponent<Transform>();
                var correctedSize = texture.CropToAspect ?
                    FixAspectRatio(data, transform, texture.LeftTexture.width, texture.LeftTexture.height) :
                    data.GetScaledSize(transform.lossyScale);

                // CreateLayer ...
                ext_composition_layers_createQuadLayer(new QuadLayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position,
                    rotation = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation,
                    size = correctedSize,
                    width = texture.LeftTexture.width,
                    height = texture.LeftTexture.height,
                    blendType = layer.Layer.LayerData.BlendType
                });
            }
#if UNITY_ANDROID
            else
            {
                //For External surface texture, Android only
                OpenXRLayerUtility.CreateSwapchain(layer.Id, new XrSwapchainCreateInfo()
                {
                    Type = 9,
                    Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                    CreateFlags = 0,
                    UsageFlags = 0x00000020 | 0x00000001,
                    Format = 0,
                    SampleCount = 0,
                    Width = (uint) (texture.Resolution.x),
                    Height = (uint) (texture.Resolution.y),
                    FaceCount = 0,
                    ArraySize = 0,
                    MipCount = 0,
                }, true);

                var transform = layer.Layer.GetComponent<Transform>();
                var correctedSize = texture.CropToAspect ?
                    FixAspectRatio(data, transform, (int)texture.Resolution.x, (int)texture.Resolution.y) :
                    data.GetScaledSize(transform.lossyScale);

                ext_composition_layers_createQuadLayer(new QuadLayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position,
                    rotation = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation,
                    size = correctedSize,
                    width = (int)texture.Resolution.x,
                    height = (int)texture.Resolution.y,
                });
            }
#endif
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// of the type registered to this <c>ILayerHandler</c> instance has been destroyed or disabled.
        /// </summary>
        public void RemoveLayer(int id)
        {
            ext_composition_layers_removeQuadLayer(id);
            OpenXRLayerUtility.ReleaseSwapchain(id);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// or any attached extension components have had a member modified.
        /// </summary>
        public unsafe void ModifyLayer(CompositionLayerManager.LayerInfo layer)
        {
            var data = layer.Layer.LayerData as QuadLayerData;

            var texture = layer.Layer.GetComponent<TexturesExtension>();
            if (texture == null || texture.enabled == false)
                return;

            if (texture.TextureAdded)
            {
                texture.TextureAdded = false;
                CreateLayer(layer);
                return;
            }
            int width = 0;
            int height = 0;
            if (texture.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texture.LeftTexture != null)
            {
                width = texture.LeftTexture.width;
                height = texture.LeftTexture.height;
            }
#if UNITY_ANDROID
            else
            {
                width = (int)texture.Resolution.x;
                height = (int)texture.Resolution.y;
            }
#endif

            var transform = layer.Layer.GetComponent<Transform>();
            var correctedSize = texture.CropToAspect ?
                FixAspectRatio(data, transform, width, height) :
                data.GetScaledSize(transform.lossyScale);
            ext_composition_layers_modifyQuadLayer(new QuadLayerParam()
            {
                id = layer.Id,
                next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                position = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position,
                rotation = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation,
                size = correctedSize,
                width = width,
                height = height,
            });
        }

        private static Vector2 FixAspectRatio(QuadLayerData data, Transform transform, int texWidth, int texHeight)
        {
            var requestedSize = data.GetScaledSize(transform.lossyScale);
            float reqSizeRatio = (float)requestedSize.x / (float)requestedSize.y;
            float texRatio = (float)texWidth / (float)texHeight;
            if (reqSizeRatio > texRatio)
            {
                // too wide
                requestedSize.x = requestedSize.y * texRatio;
            }
            else if (reqSizeRatio < texRatio)
            {
                // too narrow
                requestedSize.y = requestedSize.x / texRatio;
            }
            return requestedSize;
        }

        /// <summary>
        /// Called every frame by the <see cref="OpenXRLayerProvider"/> for all currently active <see cref="LayerData"/> objects
        /// of the type registered to this <c>ILayerHandler</c> instance.
        /// </summary>
        public void SetActiveLayer(CompositionLayerManager.LayerInfo layer)
        {
            OpenXRLayerUtility.SetActiveLayerAndGetTexture(layer);
            var transform = layer.Layer.GetComponent<Transform>();
            var adjustedPos = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position;
            var adjustedRot = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation;
            ext_composition_layers_SetActiveQuadLayer(layer.Id, layer.Layer.Order, adjustedPos, adjustedRot);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> during the Unity Update loop.
        /// All implementations must call <see cref="OpenXRLayerUtility.AddActiveLayersToEndFrame(void*,void*,int,int)"/> every frame
        /// to add their native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib.
        /// </summary>
        public void OnUpdate() { }

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_createQuadLayer(QuadLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_removeQuadLayer(int id);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_modifyQuadLayer(QuadLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_SetActiveQuadLayer(int id, int order, Vector3 adjustedPos, Quaternion adjustedRot);

    }
}


#endif
