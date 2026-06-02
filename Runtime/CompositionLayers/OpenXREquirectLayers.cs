#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Generic;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{

    //OpenXR Composition Layer - Equirect Layer support : only the interior of the mesh surface will be visible
    internal class OpenXREquirectLayer : OpenXRCustomLayerHandler<XrCompositionLayerEquirectKHR>
    {
        public static bool ExtensionEnabled = OpenXRRuntime.IsExtensionEnabled("XR_KHR_composition_layer_equirect");

        Dictionary<int, OpenXRStereoLayerData.RightEyeData<XrCompositionLayerEquirectKHR>> m_StereoData = new();

        protected override unsafe bool CreateSwapchain(CompositionLayerManager.LayerInfo layerInfo, out SwapchainCreateInfo swapchainCreateInfo)
        {
            TexturesExtension texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
            {
                swapchainCreateInfo = default;
                return false;
            }

            var xrCreateInfo = new XrSwapchainCreateInfo()
            {
                Type = (uint)XrStructureType.XR_TYPE_SWAPCHAIN_CREATE_INFO,
                Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Swapchain),
                CreateFlags = 0,
                UsageFlags = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT),
                Format = OpenXRLayerUtility.GetDefaultColorFormat(),
                SampleCount = 1,
                Width = (uint)(texturesExtension.LeftTexture.width),
                Height = (uint)(texturesExtension.LeftTexture.height),
                FaceCount = 1,
                ArraySize = 1,
                MipCount = (uint)texturesExtension.LeftTexture.mipmapCount,
            };

            swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: false, isStereo: OpenXRStereoLayerData.IsStereoRequested(texturesExtension));
            return true;
        }

        protected override unsafe bool CreateNativeLayer(CompositionLayerManager.LayerInfo layerInfo, SwapchainCreatedOutput swapchainOutput, out XrCompositionLayerEquirectKHR nativeLayer)
        {
            TexturesExtension texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
            {
                nativeLayer = default;
                return false;
            }

            var transform = layerInfo.Layer.GetComponent<Transform>();
            var data = layerInfo.Layer.LayerData as EquirectMeshLayerData;
            Vector2 scaleCalculated = CalculateScale(data.CentralHorizontalAngle, data.UpperVerticalAngle, data.LowerVerticalAngle);
            Vector2 biasCalculated = CalculateBias(scaleCalculated, data.UpperVerticalAngle);

            OpenXRStereoLayerData.GetSubImageDimensions(texturesExtension, out int subImageWidth, out int subImageHeight);

            bool isStereo = OpenXRStereoLayerData.IsStereoRequested(texturesExtension)
                && swapchainOutput.secondStereoHandle != 0;

            var layerFlags = data.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha;
            var pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);

            nativeLayer = new XrCompositionLayerEquirectKHR()
            {
                Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_EQUIRECT_KHR,
                Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer),
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
                Radius = data.Radius,
                Scale = new XrVector2f(scaleCalculated),
                Bias = new XrVector2f(biasCalculated),
            };

            if (isStereo)
            {
                m_StereoData[layerInfo.Id] = new OpenXRStereoLayerData.RightEyeData<XrCompositionLayerEquirectKHR>
                {
                    RightNativeLayer = new XrCompositionLayerEquirectKHR()
                    {
                        Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_EQUIRECT_KHR,
                        Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer),
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
                        Radius = data.Radius,
                        Scale = new XrVector2f(scaleCalculated),
                        Bias = new XrVector2f(biasCalculated),
                    },
                    LeftTexture = texturesExtension.LeftTexture,
                    RightTexture = texturesExtension.RightTexture,
                };
            }
            else
            {
                m_StereoData.Remove(layerInfo.Id);
            }

            return true;
        }

        protected override unsafe bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerEquirectKHR nativeLayer)
        {
            TexturesExtension texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
                return false;

            var transform = layerInfo.Layer.GetComponent<Transform>();
            var data = layerInfo.Layer.LayerData as EquirectMeshLayerData;
            Vector2 scaleCalculated = CalculateScale(data.CentralHorizontalAngle, data.UpperVerticalAngle, data.LowerVerticalAngle);
            Vector2 biasCalculated = CalculateBias(scaleCalculated, data.UpperVerticalAngle);

            nativeLayer.SubImage.ImageRect.Extent = new XrExtent2Di()
            {
                Width = texturesExtension.LeftTexture.width,
                Height = texturesExtension.LeftTexture.height
            };
            nativeLayer.Pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
            nativeLayer.Radius = data.Radius;
            nativeLayer.Scale = new XrVector2f(scaleCalculated);
            nativeLayer.Bias = new XrVector2f(biasCalculated);
            nativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Pose = nativeLayer.Pose;
                stereoData.RightNativeLayer.Radius = nativeLayer.Radius;
                stereoData.RightNativeLayer.Scale = nativeLayer.Scale;
                stereoData.RightNativeLayer.Bias = nativeLayer.Bias;
                stereoData.RightNativeLayer.LayerFlags = nativeLayer.LayerFlags;
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
                stereoData.RightNativeLayer.SubImage.ImageRect.Extent = nativeLayer.SubImage.ImageRect.Extent;

                unsafe
                {
                    stereoData.RightNativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
                }
            }

            return true;
        }

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerEquirectKHR nativeLayer)
        {
            nativeLayer.Space = OpenXRLayerUtility.GetCurrentAppSpace();

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
            }

            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereo))
            {
                if (!ValidateAndUpdateRenderInfo(layerInfo, texturesExtension))
                    return false;

                stereo.IsActive = true;
                stereo.LeftTexture = texturesExtension?.LeftTexture;
                stereo.RightTexture = texturesExtension?.RightTexture;
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

        Vector2 CalculateScale(float centralHorizontalAngle, float upperVerticalAngle, float lowerVerticalAngle)
        {
            return new Vector2((2.0f * (float)Math.PI) / centralHorizontalAngle, (float)Math.PI / (upperVerticalAngle - lowerVerticalAngle));
        }

        Vector2 CalculateBias(Vector2 scaleCalculated, float upperVerticalAngle)
        {
            return new Vector2((1.0f - scaleCalculated.x) * 0.5f, (upperVerticalAngle / (float)Math.PI - 0.5f) * scaleCalculated.y);
        }
    }


    internal class OpenXREquirect2Layer : OpenXRCustomLayerHandler<XrCompositionLayerEquirect2KHR>
    {
        public static bool ExtensionEnabled = OpenXRRuntime.IsExtensionEnabled("XR_KHR_composition_layer_equirect2");

        Dictionary<int, OpenXRStereoLayerData.RightEyeData<XrCompositionLayerEquirect2KHR>> m_StereoData = new();

        protected override unsafe bool CreateSwapchain(CompositionLayerManager.LayerInfo layerInfo, out SwapchainCreateInfo swapchainCreateInfo)
        {
            TexturesExtension texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
            {
                swapchainCreateInfo = default;
                return false;
            }

            var xrCreateInfo = new XrSwapchainCreateInfo()
            {
                Type = (uint)XrStructureType.XR_TYPE_SWAPCHAIN_CREATE_INFO,
                Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Swapchain),
                CreateFlags = 0,
                UsageFlags = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT),
                Format = OpenXRLayerUtility.GetDefaultColorFormat(),
                SampleCount = 1,
                Width = (uint)(texturesExtension.LeftTexture.width),
                Height = (uint)(texturesExtension.LeftTexture.height),
                FaceCount = 1,
                ArraySize = 1,
                MipCount = (uint)texturesExtension.LeftTexture.mipmapCount,
            };

            swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: false, isStereo: OpenXRStereoLayerData.IsStereoRequested(texturesExtension));
            return true;
        }

        protected override unsafe bool CreateNativeLayer(CompositionLayerManager.LayerInfo layerInfo, SwapchainCreatedOutput swapchainOutput, out XrCompositionLayerEquirect2KHR nativeLayer)
        {
            TexturesExtension texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
            {
                nativeLayer = default;
                return false;
            }

            var transform = layerInfo.Layer.GetComponent<Transform>();
            var data = layerInfo.Layer.LayerData as EquirectMeshLayerData;

            OpenXRStereoLayerData.GetSubImageDimensions(texturesExtension, out int subImageWidth, out int subImageHeight);

            bool isStereo = OpenXRStereoLayerData.IsStereoRequested(texturesExtension)
                && swapchainOutput.secondStereoHandle != 0;

            var layerFlags = data.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha;
            var pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);

            nativeLayer = new XrCompositionLayerEquirect2KHR()
            {
                Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_EQUIRECT2_KHR,
                Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer),
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
                Radius = data.Radius,
                CentralHorizontalAngle = data.CentralHorizontalAngle,
                UpperVerticalAngle = data.UpperVerticalAngle,
                LowerVerticalAngle = -data.LowerVerticalAngle
            };

            if (isStereo)
            {
                m_StereoData[layerInfo.Id] = new OpenXRStereoLayerData.RightEyeData<XrCompositionLayerEquirect2KHR>
                {
                    RightNativeLayer = new XrCompositionLayerEquirect2KHR()
                    {
                        Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_EQUIRECT2_KHR,
                        Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer),
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
                        Radius = data.Radius,
                        CentralHorizontalAngle = data.CentralHorizontalAngle,
                        UpperVerticalAngle = data.UpperVerticalAngle,
                        LowerVerticalAngle = -data.LowerVerticalAngle
                    },
                    LeftTexture = texturesExtension.LeftTexture,
                    RightTexture = texturesExtension.RightTexture,
                };
            }
            else
            {
                m_StereoData.Remove(layerInfo.Id);
            }

            return true;
        }

        protected override unsafe bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerEquirect2KHR nativeLayer)
        {
            TexturesExtension texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
                return false;

            var transform = layerInfo.Layer.GetComponent<Transform>();
            var data = layerInfo.Layer.LayerData as EquirectMeshLayerData;

            nativeLayer.SubImage.ImageRect.Extent = new XrExtent2Di()
            {
                Width = texturesExtension.LeftTexture.width,
                Height = texturesExtension.LeftTexture.height
            };
            nativeLayer.Pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
            nativeLayer.Radius = data.Radius;
            nativeLayer.CentralHorizontalAngle = data.CentralHorizontalAngle;
            nativeLayer.UpperVerticalAngle = data.UpperVerticalAngle;
            nativeLayer.LowerVerticalAngle = -data.LowerVerticalAngle;

            nativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Pose = nativeLayer.Pose;
                stereoData.RightNativeLayer.Radius = nativeLayer.Radius;
                stereoData.RightNativeLayer.CentralHorizontalAngle = nativeLayer.CentralHorizontalAngle;
                stereoData.RightNativeLayer.UpperVerticalAngle = nativeLayer.UpperVerticalAngle;
                stereoData.RightNativeLayer.LowerVerticalAngle = nativeLayer.LowerVerticalAngle;
                stereoData.RightNativeLayer.LayerFlags = nativeLayer.LayerFlags;
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
                stereoData.RightNativeLayer.SubImage.ImageRect.Extent = nativeLayer.SubImage.ImageRect.Extent;

                unsafe
                {
                    stereoData.RightNativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
                }
            }

            return true;
        }

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerEquirect2KHR nativeLayer)
        {
            nativeLayer.Space = OpenXRLayerUtility.GetCurrentAppSpace();

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
            }

            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereo))
            {
                if (!ValidateAndUpdateRenderInfo(layerInfo, texturesExtension))
                    return false;

                stereo.IsActive = true;
                stereo.LeftTexture = texturesExtension?.LeftTexture;
                stereo.RightTexture = texturesExtension?.RightTexture;
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

        Vector2 CalculateScale(float centralHorizontalAngle, float upperVerticalAngle, float lowerVerticalAngle)
        {
            return new Vector2((2.0f * (float)Math.PI) / centralHorizontalAngle, (float)Math.PI / (upperVerticalAngle - lowerVerticalAngle));
        }
    }

}
#endif
