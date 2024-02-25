#if XR_COMPOSITION_LAYERS

using System;
using System.Runtime.InteropServices;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.CompositionLayers;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    //OpenXR Composition Layer - Equirect Layer support : only the interior of the mesh surface will be visible
    internal class OpenXREquirectLayer : OpenXRLayerProvider.ILayerHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct EquirectLayerParam
        {
            internal int id;
            internal void* next;
            internal Vector3 position;
            internal Quaternion rotation;
            internal float radius;
            internal Vector2 scale;
            internal Vector2 bias;
            internal int width;
            internal int height;
            internal BlendType blendType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct Equirect2LayerParam
        {
            internal int id;
            internal void* next;
            internal Vector3 position;
            internal Quaternion rotation;
            internal float radius;
            internal float centralHorizontalAngle;
            internal float upperVerticalAngle;
            internal float lowerVerticalAngle;
            internal int width;
            internal int height;
            internal BlendType blendType;
        }

        private bool isEquirect2LayerExtensionEnabled = OpenXRRuntime.IsExtensionEnabled("XR_KHR_composition_layer_equirect2");
        private bool isEquirectLayerExtensionEnabled = OpenXRRuntime.IsExtensionEnabled("XR_KHR_composition_layer_equirect");

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a new <see cref="LayerData"/>
        /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
        /// </summary>
        public unsafe void CreateLayer(CompositionLayerManager.LayerInfo layer)
        {
            var transform = layer.Layer.GetComponent<Transform>();
            var data = layer.Layer.LayerData as EquirectMeshLayerData;
            TexturesExtension texturesExtension  = layer.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
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
                Width = (uint) (texturesExtension.LeftTexture.width),
                Height = (uint) (texturesExtension.LeftTexture.height),
                FaceCount = 1,
                ArraySize = 1,
                MipCount = (uint)texturesExtension.LeftTexture.mipmapCount,
            });
            // Create Layers if available on runtime ...
            if (isEquirect2LayerExtensionEnabled)
            {
                ext_composition_layers_createEquirect2Layer(new Equirect2LayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = transform.position,
                    rotation = transform.rotation,
                    radius = data.Radius,
                    centralHorizontalAngle = data.CentralHorizontalAngle,
                    upperVerticalAngle = data.UpperVerticalAngle,
                    lowerVerticalAngle = -data.LowerVerticalAngle,
                    width = texturesExtension.LeftTexture.width,
                    height = texturesExtension.LeftTexture.height,
                    blendType = layer.Layer.LayerData.BlendType
                });
            }

            if (isEquirectLayerExtensionEnabled)
            {
                Vector2 scaleCalculated = CalculateScale(data.CentralHorizontalAngle, data.UpperVerticalAngle, data.LowerVerticalAngle);
                Vector2 biasCalculated = CalculateBias(scaleCalculated, data.UpperVerticalAngle);
                ext_composition_layers_createEquirectLayer(new EquirectLayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = transform.position,
                    rotation = transform.rotation,
                    radius = data.Radius,
                    scale = scaleCalculated,
                    bias = biasCalculated,
                    width = texturesExtension.LeftTexture.width,
                    height = texturesExtension.LeftTexture.height,
                    blendType = layer.Layer.LayerData.BlendType
                });
            }
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// of the type registered to this <c>ILayerHandler</c> instance has been destroyed or disabled.
        /// </summary>
        public void RemoveLayer(int id)
        {
            ext_composition_layers_removeEquirectLayer(id);
            OpenXRLayerUtility.ReleaseSwapchain(id);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// or any attached extension components have had a member modified.
        /// </summary>
        public unsafe void ModifyLayer(CompositionLayerManager.LayerInfo layer)
        {
            var transform = layer.Layer.GetComponent<Transform>();
            var data = layer.Layer.LayerData as EquirectMeshLayerData;
            var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
                return;
            if (texturesExtension.TextureAdded)
            {
                texturesExtension.TextureAdded = false;
                CreateLayer(layer);
                return;
            }
            if (isEquirect2LayerExtensionEnabled)
            {
                ext_composition_layers_modifyEquirect2Layer(new Equirect2LayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = transform.position,
                    rotation = transform.rotation,
                    radius = data.Radius,
                    centralHorizontalAngle = data.CentralHorizontalAngle,
                    upperVerticalAngle = data.UpperVerticalAngle,
                    lowerVerticalAngle = -data.LowerVerticalAngle,
                    width = texturesExtension.LeftTexture.width,
                    height = texturesExtension.LeftTexture.height,
                });
            }

            if (isEquirectLayerExtensionEnabled)
            {
                Vector2 scaleCalculated = CalculateScale(data.CentralHorizontalAngle, data.UpperVerticalAngle, data.LowerVerticalAngle);
                Vector2 biasCalculated = CalculateBias(scaleCalculated, data.UpperVerticalAngle);

                ext_composition_layers_modifyEquirectLayer(new EquirectLayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = transform.position,
                    rotation = transform.rotation,
                    radius = data.Radius,
                    scale = scaleCalculated,
                    bias = biasCalculated,
                    width = texturesExtension.LeftTexture.width,
                    height = texturesExtension.LeftTexture.height,
                });
            }
        }

        private Vector2 CalculateScale(float centralHorizontalAngle, float upperVerticalAngle, float lowerVerticalAngle)
        {
            return new Vector2((2.0f * (float)Math.PI) / centralHorizontalAngle, (float)Math.PI / (upperVerticalAngle - lowerVerticalAngle));
        }

        private Vector2 CalculateBias(Vector2 scaleCalculated, float upperVerticalAngle)
        {
            return new Vector2((1.0f - scaleCalculated.x) * 0.5f, (upperVerticalAngle / (float)Math.PI - 0.5f) * scaleCalculated.y);
        }

        /// <summary>
        /// Called every frame by the <see cref="OpenXRLayerProvider"/> for all currently active <see cref="LayerData"/> objects
        /// of the type registered to this <c>ILayerHandler</c> instance.
        /// </summary>
        public void SetActiveLayer(CompositionLayerManager.LayerInfo layer)
        {
            OpenXRLayerUtility.SetActiveLayerAndGetTexture(layer);
            ext_composition_layers_SetActiveEquirectLayer(layer.Id, layer.Layer.Order);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> during the Unity Update loop.
        /// All implementations must call <see cref="OpenXRLayerUtility.AddActiveLayersToEndFrame(void*,void*,int,int)"/> every frame
        /// to add their native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib.
        /// </summary>
        public void OnUpdate() { }

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_createEquirectLayer(EquirectLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_modifyEquirectLayer(EquirectLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_createEquirect2Layer(Equirect2LayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_modifyEquirect2Layer(Equirect2LayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_removeEquirectLayer(int id);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_SetActiveEquirectLayer(int id, int order);
    }
}
#endif
