#if XR_COMPOSITION_LAYERS

using System;
using System.Runtime.InteropServices;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    //Default OpenXR Composition Layer - Cube Layer support
    internal class OpenXRCubeLayer : OpenXRLayerProvider.ILayerHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CubeLayerParam
        {
            internal int id;
            internal Quaternion rotation;
            internal BlendType blendType;
        }

        private bool IsCubemapTextureSupportedForCurrentEditorVersion;

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a new <see cref="LayerData"/>
        /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
        /// </summary>
        public unsafe void CreateLayer(CompositionLayerManager.LayerInfo layer)
        {
            // Check if current Unity Editor version supports cubemap texture rendering. Supported versions:  2023.3.0a9+, 2023.2.0b13+, 2023.1.17f1+ & 2022.3.11f1+
            IsCubemapTextureSupportedForCurrentEditorVersion = ext_composition_layers_isCubemapTextureSupportedForCurrentEditor(Application.unityVersion);
            if (!IsCubemapTextureSupportedForCurrentEditorVersion)
            {
                Debug.LogWarning("Cube Layer is not supported in current Unity Editor version. Supported minimum versions are: 2023.3.0a9, 2023.2.0b13, 2023.1.17f1 & 2022.3.11f1.\n");
                return;
            }
            var data = layer.Layer.LayerData as CubeProjectionLayerData;
            TexturesExtension texture = layer.Layer.GetComponent<TexturesExtension>();
            if (texture == null || texture.enabled == false || texture.LeftTexture == null)
                return;
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
                FaceCount = 6,
                ArraySize = 1,
                MipCount = (uint) texture.LeftTexture.mipmapCount,
            });

            // CreateLayer ...
            ext_composition_layers_createCubeLayer(new CubeLayerParam()
            {
                id = layer.Id,
                rotation = layer.Layer.GetComponent<Transform>().rotation,
                blendType = layer.Layer.LayerData.BlendType
            });
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// of the type registered to this <c>ILayerHandler</c> instance has been destroyed or disabled.
        /// </summary>
        public void RemoveLayer(int id)
        {
            if (!IsCubemapTextureSupportedForCurrentEditorVersion)
                return;
            ext_composition_layers_removeCubeLayer(id);
            OpenXRLayerUtility.ReleaseSwapchain(id);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// or any attached extension components have had a member modified.
        /// </summary>
        public void ModifyLayer(CompositionLayerManager.LayerInfo layer)
        {
            if (!IsCubemapTextureSupportedForCurrentEditorVersion)
                return;
            var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
                return;
            if (texturesExtension.TextureAdded)
            {
                texturesExtension.TextureAdded = false;
                CreateLayer(layer);
                return;
            }
            ext_composition_layers_modifyCubeLayer(new CubeLayerParam()
            {
                id = layer.Id,
                rotation = layer.Layer.GetComponent<Transform>().rotation
            });
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> during the Unity Update loop.
        /// All implementations must call <see cref="OpenXRLayerUtility.AddActiveLayersToEndFrame(void*,void*,int,int)"/> every frame
        /// to add their native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib.
        /// </summary>
        public void OnUpdate()
        {
        }

        /// <summary>
        /// Called every frame by the <see cref="OpenXRLayerProvider"/> for all currently active <see cref="LayerData"/> objects
        /// of the type registered to this <c>ILayerHandler</c> instance.
        /// </summary>
        public void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            if (!IsCubemapTextureSupportedForCurrentEditorVersion)
                return;
            OpenXRLayerUtility.SetActiveLayerAndGetTexture(layerInfo);
            ext_composition_layers_SetActiveCubeLayer(layerInfo.Id, layerInfo.Layer.Order);
        }

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_createCubeLayer(CubeLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_removeCubeLayer(int id);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_modifyCubeLayer(CubeLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_SetActiveCubeLayer(int id, int order);

        [DllImport("UnityOpenXR")]
        internal static extern bool ext_composition_layers_isCubemapTextureSupportedForCurrentEditor(string editorVersion);
    }
}
#endif
