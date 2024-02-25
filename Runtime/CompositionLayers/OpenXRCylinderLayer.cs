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
    //OpenXR Composition Layer - Cylinder Layer support : only the interior of the cylinder surface will be visible
    internal class OpenXRCylinderLayer : OpenXRLayerProvider.ILayerHandler
    {
        private float savedDelta;

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CylinderLayerParam
        {
            internal int id;
            internal void* next;
            internal Vector3 position;
            internal Quaternion rotation;
            internal float radius;
            internal float centralAngle;
            internal float aspectRatio;
            internal int width;
            internal int height;
            internal BlendType blendType;
        }

        internal struct CylinderLayerSize
        {
            public float radius;
            public float centralAngle;
            public float aspectRatio;

            public static implicit operator CylinderLayerSize(Vector3 v) => new CylinderLayerSize
            {
                radius = v.x,
                centralAngle = v.y,
                aspectRatio = v.z
            };
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a new <see cref="LayerData"/>
        /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
        /// </summary>
        public unsafe void CreateLayer(CompositionLayerManager.LayerInfo layer)
        {
            var data = layer.Layer.LayerData as CylinderLayerData;
            TexturesExtension texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return;

            if (texturesExtension.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texturesExtension.LeftTexture != null)
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
                    Width = (uint) (texturesExtension.LeftTexture.width),
                    Height = (uint) (texturesExtension.LeftTexture.height),
                    FaceCount = 1,
                    ArraySize = 1,
                    MipCount = (uint)texturesExtension.LeftTexture.mipmapCount,
                });

                var transform = layer.Layer.GetComponent<Transform>();
                CylinderLayerSize scaledSize = data.GetScaledSize(transform.lossyScale);
                if (texturesExtension.CropToAspect)
                {
                    scaledSize =
                        FixAspectRatio(
                            data,
                            scaledSize,
                            texturesExtension.LeftTexture.width,
                            texturesExtension.LeftTexture.height);
                }
                // CreateLayer ...
                ext_composition_layers_createCylinderLayer(new CylinderLayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position,
                    rotation = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation,
                    radius = data.ApplyTransformScale ? scaledSize.radius : data.Radius,
                    centralAngle = data.ApplyTransformScale ? scaledSize.centralAngle : data.CentralAngle,
                    aspectRatio = data.ApplyTransformScale ? scaledSize.aspectRatio :data.AspectRatio,
                    width = texturesExtension.LeftTexture.width,
                    height = texturesExtension.LeftTexture.height,
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
                    Width = (uint) (texturesExtension.Resolution.x),
                    Height = (uint) (texturesExtension.Resolution.y),
                    FaceCount = 0,
                    ArraySize = 0,
                    MipCount = 0,
                }, true);

                var transform = layer.Layer.GetComponent<Transform>();
                CylinderLayerSize scaledSize = data.GetScaledSize(transform.lossyScale);
                if (texturesExtension.CropToAspect)
                {
                    scaledSize =
                        FixAspectRatio(
                            data,
                            scaledSize,
                            (int)texturesExtension.Resolution.x,
                            (int)texturesExtension.Resolution.y);
                }
                ext_composition_layers_createCylinderLayer(new CylinderLayerParam()
                {
                    id = layer.Id,
                    next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    position = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position,
                    rotation = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation,
                    radius = data.ApplyTransformScale ? scaledSize.radius : data.Radius,
                    centralAngle = data.ApplyTransformScale ? scaledSize.centralAngle : data.CentralAngle,
                    aspectRatio = data.ApplyTransformScale ? scaledSize.aspectRatio : data.AspectRatio,
                    width = (int)texturesExtension.Resolution.x,
                    height = (int)texturesExtension.Resolution.y,
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
            ext_composition_layers_removeCylinderLayer(id);
            OpenXRLayerUtility.ReleaseSwapchain(id);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// or any attached extension components have had a member modified.
        /// </summary>
        public unsafe void ModifyLayer(CompositionLayerManager.LayerInfo layer)
        {
            var data = layer.Layer.LayerData as CylinderLayerData;
            var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return;
            if (texturesExtension.TextureAdded)
            {
                texturesExtension.TextureAdded = false;
                CreateLayer(layer);
                return;
            }
            int width = 0;
            int height = 0;
            if (texturesExtension.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texturesExtension.LeftTexture != null)
            {
                width = texturesExtension.LeftTexture.width;
                height = texturesExtension.LeftTexture.height;
            }
#if UNITY_ANDROID
            else
            {
                width = (int)texturesExtension.Resolution.x;
                height = (int)texturesExtension.Resolution.y;
            }
#endif
            var transform = layer.Layer.GetComponent<Transform>();

            CylinderLayerSize scaledSize = data.GetScaledSize(transform.lossyScale);
            if (texturesExtension.CropToAspect)
            {
                scaledSize = FixAspectRatio(data, scaledSize, width, height);
            }
            ext_composition_layers_modifyCylinderLayer(new CylinderLayerParam()
            {
                id = layer.Id,
                next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                position = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position,
                rotation = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation,
                radius = data.ApplyTransformScale ? scaledSize.radius : data.Radius,
                centralAngle = data.ApplyTransformScale ? scaledSize.centralAngle : data.CentralAngle,
                aspectRatio = data.ApplyTransformScale ? scaledSize.aspectRatio :data.AspectRatio,
                width = width,
                height = height
            });
        }

        private static CylinderLayerSize FixAspectRatio(CylinderLayerData data, CylinderLayerSize scaledSize, int texWidth, int texHeight)
        {
            // because we're cropping and trying to maintain the same other parameters, we don't
            // need to consider data.MaintainAspectRatio here. That's mostly an editor concern, anyway.
            float texRatio = (float)texWidth / (float)texHeight;
            if (scaledSize.aspectRatio > texRatio){
                // too wide
                float width = scaledSize.radius * scaledSize.centralAngle;
                float height = width / scaledSize.aspectRatio;
                scaledSize.centralAngle = height * texRatio / scaledSize.radius;
                scaledSize.aspectRatio = texRatio;
            }
            else if (scaledSize.aspectRatio < texRatio)
            {
                // too narrow
                scaledSize.aspectRatio = texRatio;
            }

            return scaledSize;
        }

        /// <summary>
        /// Called every frame by the <see cref="OpenXRLayerProvider"/> for all currently active <see cref="LayerData"/> objects
        /// of the type registered to this <c>ILayerHandler</c> instance.
        /// </summary>
        public void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            OpenXRLayerUtility.SetActiveLayerAndGetTexture(layerInfo);
            var transform = layerInfo.Layer.GetComponent<Transform>();
            var adjustedPos = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).position;
            var adjustedRot = OpenXRLayerUtility.ComputePoseTrackingSpace(transform).rotation;

            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();

            // Special treatment for cylinder type based on destination rects.
            if (texturesExtension != null && texturesExtension.CustomRects)
            {
                var cylinderLayer = layerInfo.Layer.LayerData as CylinderLayerData;
                float rotationDelta = (texturesExtension.LeftEyeDestinationRect.x + (0.5f * texturesExtension.LeftEyeDestinationRect.width) - 0.5f) * cylinderLayer.CentralAngle / (float)Math.PI * 180.0f;

                Transform layerTransform = layerInfo.Layer.GetComponent<Transform>();

                if(rotationDelta != savedDelta)
                {
                    Quaternion savedDeltaQuaternion = Quaternion.AngleAxis(savedDelta, Vector3.up);
                    Quaternion deltaQuaternion = Quaternion.AngleAxis(rotationDelta, Vector3.up);
                    Quaternion difference = deltaQuaternion * Quaternion.Inverse(savedDeltaQuaternion);

                    savedDelta = rotationDelta;
                    layerTransform.rotation *= difference;
                }
            }

            ext_composition_layers_SetActiveCylinderLayer(layerInfo.Id, layerInfo.Layer.Order, adjustedPos, adjustedRot);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> during the Unity Update loop.
        /// All implementations must call <see cref="OpenXRLayerUtility.AddActiveLayersToEndFrame(void*,void*,int,int)"/> every frame
        /// to add their native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib.
        /// </summary>
        public void OnUpdate() { }

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_createCylinderLayer(CylinderLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_removeCylinderLayer(int id);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_modifyCylinderLayer(CylinderLayerParam layer);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_SetActiveCylinderLayer(int id, int order, Vector3 adjustedPos, Quaternion adjustedRot);
    }
}


#endif
