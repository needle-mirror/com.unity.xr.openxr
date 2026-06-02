#if XR_COMPOSITION_LAYERS
using System.Collections.Generic;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    internal class OpenXRCylinderLayer : OpenXRCustomLayerHandler<XrCompositionLayerCylinderKHR>
    {
        public static bool ExtensionEnabled = OpenXRRuntime.IsExtensionEnabled("XR_KHR_composition_layer_cylinder");

        float savedDelta;
        bool layerDataChanged = false;

        Dictionary<int, OpenXRStereoLayerData.RightEyeData<XrCompositionLayerCylinderKHR>> m_StereoData = new();

        struct CylinderLayerSize
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

        protected override bool CreateSwapchain(CompositionLayerManager.LayerInfo layer, out SwapchainCreateInfo swapchainCreateInfo)
        {
            if (layer.Layer == null)
            {
                swapchainCreateInfo = default;
                return false;
            }

            unsafe
            {
                var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                if (texturesExtension == null || texturesExtension.enabled == false)
                {
                    swapchainCreateInfo = default;
                    return false;
                }

                switch (texturesExtension.sourceTexture)
                {
                    case TexturesExtension.SourceTextureEnum.LocalTexture:
                    {
                        if (texturesExtension.LeftTexture == null)
                            goto default;

                        var xrCreateInfo = new XrSwapchainCreateInfo()
                        {
                            Type = (uint)XrStructureType.XR_TYPE_SWAPCHAIN_CREATE_INFO,
                            Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                            CreateFlags = 0,
                            UsageFlags = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT),
                            Format = OpenXRLayerUtility.GetDefaultColorFormat(),
                            SampleCount = 1,
                            Width = (uint)texturesExtension.LeftTexture.width,
                            Height = (uint)texturesExtension.LeftTexture.height,
                            FaceCount = 1,
                            ArraySize = 1,
                            MipCount = (uint)texturesExtension.LeftTexture.mipmapCount,
                        };

                        swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: false, isStereo: OpenXRStereoLayerData.IsStereoRequested(texturesExtension));
                        return true;
                    }

                    case TexturesExtension.SourceTextureEnum.AndroidSurface:
                    {
#if UNITY_ANDROID
                        var xrCreateInfo = new XrSwapchainCreateInfo()
                        {
                            Type = (uint)XrStructureType.XR_TYPE_SWAPCHAIN_CREATE_INFO,
                            Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                            CreateFlags = 0,
                            UsageFlags = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT),
                            Format = 0,
                            SampleCount = 0,
                            Width = (uint)texturesExtension.Resolution.x,
                            Height = (uint)texturesExtension.Resolution.y,
                            FaceCount = 0,
                            ArraySize = 0,
                            MipCount = 0,
                        };
                        swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: true);
                        return true;
#else
                        goto default;
#endif
                    }

                    default:
                        swapchainCreateInfo = default;
                        return false;
                }
            }
        }

        protected override bool CreateNativeLayer(CompositionLayerManager.LayerInfo layer, SwapchainCreatedOutput swapchainOutput, out XrCompositionLayerCylinderKHR nativeLayer)
        {
            unsafe
            {
                var data = layer.Layer.LayerData as CylinderLayerData;
                var transform = layer.Layer.GetComponent<Transform>();
                var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                int subImageWidth = 0;
                int subImageHeight = 0;

                switch (texturesExtension.sourceTexture)
                {
                    case TexturesExtension.SourceTextureEnum.LocalTexture:
                    {
                        if (texturesExtension.LeftTexture != null)
                        {
                            subImageWidth = texturesExtension.LeftTexture.width;
                            subImageHeight = texturesExtension.LeftTexture.height;
                        }
                        break;
                    }

                    case TexturesExtension.SourceTextureEnum.AndroidSurface:
                    {
                        subImageWidth = (int)texturesExtension.Resolution.x;
                        subImageHeight = (int)texturesExtension.Resolution.y;
                        break;
                    }
                }

                CylinderLayerSize scaledSize = data.GetScaledSize(transform.lossyScale);
                if (texturesExtension.CropToAspect)
                {
                    scaledSize = FixAspectRatio(data, scaledSize, subImageWidth, subImageHeight);
                }

                bool isStereo = OpenXRStereoLayerData.IsStereoRequested(texturesExtension)
                    && swapchainOutput.secondStereoHandle != 0;

                var layerFlags = data.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha;
                var pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
                float radius = data.ApplyTransformScale ? scaledSize.radius : data.Radius;
                float centralAngle = data.ApplyTransformScale ? scaledSize.centralAngle : data.CentralAngle;
                float aspectRatio = data.ApplyTransformScale ? scaledSize.aspectRatio : data.AspectRatio;

                nativeLayer = new XrCompositionLayerCylinderKHR()
                {
                    Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_CYLINDER_KHR,
                    Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    LayerFlags = layerFlags,
                    Space = OpenXRLayerUtility.GetCurrentAppSpace(),
                    EyeVisibility = OpenXRStereoLayerData.GetEyeVisibility(texturesExtension, isStereo),
                    SubImage = new XrSwapchainSubImage()
                    {
                        Swapchain = swapchainOutput.handle,
                        ImageRect = new XrRect2Di()
                        {
                            Offset = new XrOffset2Di() { X = 0, Y = 0 },
                            Extent = new XrExtent2Di()
                            {
                                Width = subImageWidth,
                                Height = subImageHeight
                            }
                        },
                        ImageArrayIndex = 0
                    },
                    Pose = pose,
                    Radius = radius,
                    CentralAngle = centralAngle,
                    AspectRatio = aspectRatio,
                };

                if (isStereo)
                {
                    m_StereoData[layer.Id] = new OpenXRStereoLayerData.RightEyeData<XrCompositionLayerCylinderKHR>
                    {
                        RightNativeLayer = new XrCompositionLayerCylinderKHR()
                        {
                            Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_CYLINDER_KHR,
                            Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                            LayerFlags = layerFlags,
                            Space = OpenXRLayerUtility.GetCurrentAppSpace(),
                            EyeVisibility = XrEyeVisibility.Right,
                            SubImage = new XrSwapchainSubImage()
                            {
                                Swapchain = swapchainOutput.secondStereoHandle,
                                ImageRect = new XrRect2Di()
                                {
                                    Offset = new XrOffset2Di() { X = 0, Y = 0 },
                                    Extent = new XrExtent2Di()
                                    {
                                        Width = subImageWidth,
                                        Height = subImageHeight
                                    }
                                },
                                ImageArrayIndex = 0
                            },
                            Pose = pose,
                            Radius = radius,
                            CentralAngle = centralAngle,
                            AspectRatio = aspectRatio,
                        },
                        LeftTexture = texturesExtension.LeftTexture,
                        RightTexture = texturesExtension.RightTexture,
                    };
                }
                else
                {
                    m_StereoData.Remove(layer.Id);
                }

                layerDataChanged = true;
                return true;
            }
        }

        protected override bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerCylinderKHR nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var data = layerInfo.Layer.LayerData as CylinderLayerData;
            OpenXRStereoLayerData.GetSubImageDimensions(texturesExtension, out int subImageWidth, out int subImageHeight);
            nativeLayer.SubImage.ImageRect.Extent = new XrExtent2Di()
            {
                Width = subImageWidth,
                Height = subImageHeight
            };

            var transform = layerInfo.Layer.GetComponent<Transform>();
            CylinderLayerSize scaledSize = data.GetScaledSize(transform.lossyScale);
            if (texturesExtension.CropToAspect)
            {
                scaledSize = FixAspectRatio(data, scaledSize, subImageWidth, subImageHeight);
            }
            nativeLayer.Radius = data.ApplyTransformScale ? scaledSize.radius : data.Radius;
            nativeLayer.CentralAngle = data.ApplyTransformScale ? scaledSize.centralAngle : data.CentralAngle;
            nativeLayer.AspectRatio = data.ApplyTransformScale ? scaledSize.aspectRatio : data.AspectRatio;

            unsafe
            {
                nativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
            }

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Pose = nativeLayer.Pose;
                stereoData.RightNativeLayer.Radius = nativeLayer.Radius;
                stereoData.RightNativeLayer.CentralAngle = nativeLayer.CentralAngle;
                stereoData.RightNativeLayer.AspectRatio = nativeLayer.AspectRatio;
                stereoData.RightNativeLayer.LayerFlags = nativeLayer.LayerFlags;
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
                stereoData.RightNativeLayer.SubImage.ImageRect.Extent = nativeLayer.SubImage.ImageRect.Extent;

                unsafe
                {
                    stereoData.RightNativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
                }
            }

            layerDataChanged = true;
            return true;
        }

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerCylinderKHR nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var transform = layerInfo.Layer.GetComponent<Transform>();

            // Special treatment for cylinder type based on destination rects.
            if (texturesExtension != null && texturesExtension.CustomRects)
            {
                var cylinderLayer = layerInfo.Layer.LayerData as CylinderLayerData;
                float rotationDelta = (texturesExtension.LeftEyeDestinationRect.x + (0.5f * texturesExtension.LeftEyeDestinationRect.width) - 0.5f) * cylinderLayer.CentralAngle / (float)System.Math.PI * 180.0f;

                if (rotationDelta != savedDelta)
                {
                    Quaternion savedDeltaQuaternion = Quaternion.AngleAxis(savedDelta, Vector3.up);
                    Quaternion deltaQuaternion = Quaternion.AngleAxis(rotationDelta, Vector3.up);
                    Quaternion difference = deltaQuaternion * Quaternion.Inverse(savedDeltaQuaternion);

                    savedDelta = rotationDelta;
                    transform.rotation *= difference;
                }
            }

            nativeLayer.Pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
            nativeLayer.Space = OpenXRLayerUtility.GetCurrentAppSpace();

            if (texturesExtension.CustomRects && layerDataChanged)
            {
                OpenXRStereoLayerData.GetSubImageDimensions(texturesExtension, out int subImageWidth, out int subImageHeight);

                nativeLayer.SubImage.ImageRect = OpenXRStereoLayerData.ToSubImageRect(texturesExtension.LeftEyeSourceRect, subImageWidth, subImageHeight);

                var worldPose = OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache);
                var currentPosition = worldPose.position;

                float cylinderHeight = nativeLayer.Radius * nativeLayer.CentralAngle / nativeLayer.AspectRatio;
                float transformedY = currentPosition.y + (((texturesExtension.LeftEyeDestinationRect.y + (0.5f * texturesExtension.LeftEyeDestinationRect.height) - 0.5f)) * (-1.0f * cylinderHeight));
                nativeLayer.Pose = new XrPosef(new Vector3(currentPosition.x, transformedY, currentPosition.z), worldPose.rotation);

                nativeLayer.CentralAngle = nativeLayer.CentralAngle * texturesExtension.LeftEyeDestinationRect.width;
                nativeLayer.AspectRatio = nativeLayer.AspectRatio * texturesExtension.LeftEyeDestinationRect.width / texturesExtension.LeftEyeDestinationRect.height;

                if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
                {
                    stereoData.RightNativeLayer.SubImage.ImageRect = OpenXRStereoLayerData.ToSubImageRect(texturesExtension.RightEyeSourceRect, subImageWidth, subImageHeight);

                    // Apply right-eye destination rect with cylinder-specific rotation
                    var cylinderLayer = layerInfo.Layer.LayerData as CylinderLayerData;
                    float rightRotationDelta = (texturesExtension.RightEyeDestinationRect.x + (0.5f * texturesExtension.RightEyeDestinationRect.width) - 0.5f) * cylinderLayer.CentralAngle / (float)System.Math.PI * 180.0f;

                    float rightCylinderHeight = stereoData.RightNativeLayer.Radius * stereoData.RightNativeLayer.CentralAngle / stereoData.RightNativeLayer.AspectRatio;
                    float rightTransformedY = currentPosition.y + (((texturesExtension.RightEyeDestinationRect.y + (0.5f * texturesExtension.RightEyeDestinationRect.height) - 0.5f)) * (-1.0f * rightCylinderHeight));

                    Quaternion rightDeltaQuaternion = Quaternion.AngleAxis(rightRotationDelta, Vector3.up);
                    stereoData.RightNativeLayer.Pose = new XrPosef(new Vector3(currentPosition.x, rightTransformedY, currentPosition.z), worldPose.rotation * rightDeltaQuaternion);
                    stereoData.RightNativeLayer.CentralAngle = stereoData.RightNativeLayer.CentralAngle * texturesExtension.RightEyeDestinationRect.width;
                    stereoData.RightNativeLayer.AspectRatio = stereoData.RightNativeLayer.AspectRatio * texturesExtension.RightEyeDestinationRect.width / texturesExtension.RightEyeDestinationRect.height;
                }

                layerDataChanged = false;
            }
            else if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Pose = nativeLayer.Pose;
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
                stereoData.RightNativeLayer.Radius = nativeLayer.Radius;
                stereoData.RightNativeLayer.CentralAngle = nativeLayer.CentralAngle;
                stereoData.RightNativeLayer.AspectRatio = nativeLayer.AspectRatio;
            }

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereo))
            {
                if (!ValidateAndUpdateRenderInfo(layerInfo, texturesExtension))
                    return false;

                stereo.IsActive = true;
                stereo.LeftTexture = texturesExtension.LeftTexture;
                stereo.RightTexture = texturesExtension.RightTexture;
                return true;
            }

            return base.ActiveNativeLayer(layerInfo, ref nativeLayer);
        }

        public override void RemoveLayer(int id)
        {
            m_StereoData.Remove(id);

            base.RemoveLayer(id);
        }

        public override void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            base.SetActiveLayer(layerInfo);

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData) && stereoData.IsActive)
                AppendActiveNativeLayer(stereoData.RightNativeLayer, layerInfo.Layer.Order);
        }

        public override void OnUpdate()
        {
            OpenXRStereoLayerData.WriteStereoTextures(m_StereoData);
            base.OnUpdate();
        }

        static CylinderLayerSize FixAspectRatio(CylinderLayerData data, CylinderLayerSize scaledSize, int texWidth, int texHeight)
        {
            // because we're cropping and trying to maintain the same other parameters, we don't
            // need to consider data.MaintainAspectRatio here. That's mostly an editor concern, anyway.
            float texRatio = (float)texWidth / (float)texHeight;
            if (scaledSize.aspectRatio > texRatio)
            {
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


    }

}
#endif
