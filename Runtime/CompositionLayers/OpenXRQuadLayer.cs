#if XR_COMPOSITION_LAYERS
using System.Collections.Generic;
using UnityEngine.XR.OpenXR.NativeTypes;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    //Default OpenXR Composition Layer - Quad Layer support
    internal class OpenXRQuadLayer : OpenXRCustomLayerHandler<XrCompositionLayerQuad>
    {
        Dictionary<int, OpenXRStereoLayerData.RightEyeData<XrCompositionLayerQuad>> m_StereoData = new();

        protected override bool CreateSwapchain(CompositionLayerManager.LayerInfo layer, out SwapchainCreateInfo swapchainCreateInfo)
        {
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

        protected override bool CreateNativeLayer(CompositionLayerManager.LayerInfo layer, SwapchainCreatedOutput swapchainOutput, out XrCompositionLayerQuad nativeLayer)
        {
            unsafe
            {
                var texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                if (texturesExtension == null || texturesExtension.enabled == false)
                {
                    nativeLayer = default;
                    return false;
                }

                var data = layer.Layer.LayerData as QuadLayerData;
                var transform = layer.Layer.GetComponent<Transform>();
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

                var correctedSize = texturesExtension.CropToAspect ?
                    FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                    data.GetScaledSize(transform.lossyScale);

                bool isStereo = OpenXRStereoLayerData.IsStereoRequested(texturesExtension)
                    && swapchainOutput.secondStereoHandle != 0;

                var layerFlags = data.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha;
                var pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);

                nativeLayer = new XrCompositionLayerQuad()
                {
                    Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_QUAD,
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
                    Size = new XrExtent2Df()
                    {
                        Width = correctedSize.x,
                        Height = correctedSize.y
                    }
                };

                if (isStereo)
                {
                    var rightNativeLayer = new XrCompositionLayerQuad()
                    {
                        Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_QUAD,
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
                        Size = new XrExtent2Df()
                        {
                            Width = correctedSize.x,
                            Height = correctedSize.y
                        }
                    };

                    m_StereoData[layer.Id] = new OpenXRStereoLayerData.RightEyeData<XrCompositionLayerQuad>
                    {
                        RightNativeLayer = rightNativeLayer,
                        LeftTexture = texturesExtension.LeftTexture,
                        RightTexture = texturesExtension.RightTexture,
                    };
                }
                else
                {
                    // Clean up stereo data if switching back to mono
                    m_StereoData.Remove(layer.Id);
                }

                return true;
            }
        }

        protected override bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerQuad nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var data = layerInfo.Layer.LayerData as QuadLayerData;
            var transform = layerInfo.Layer.GetComponent<Transform>();
            var pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);
            nativeLayer.Pose = pose;

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

            nativeLayer.SubImage.ImageRect.Extent = new XrExtent2Di()
            {
                Width = subImageWidth,
                Height = subImageHeight
            };

            var correctedSize = texturesExtension.CropToAspect ?
                FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                data.GetScaledSize(transform.lossyScale);
            nativeLayer.Size = new XrExtent2Df()
            {
                Width = correctedSize.x,
                Height = correctedSize.y
            };

            unsafe
            {
                nativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
            }

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Pose = pose;
                stereoData.RightNativeLayer.Size = nativeLayer.Size;
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

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerQuad nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var data = layerInfo.Layer.LayerData as QuadLayerData;
            var transform = layerInfo.Layer.GetComponent<Transform>();
            var worldPose = OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache);
            nativeLayer.Pose = new XrPosef(worldPose.position, worldPose.rotation);
            nativeLayer.Space = OpenXRLayerUtility.GetCurrentAppSpace();

            if (texturesExtension.CustomRects)
            {
                OpenXRStereoLayerData.GetSubImageDimensions(texturesExtension, out int subImageWidth, out int subImageHeight);

                nativeLayer.SubImage.ImageRect = OpenXRStereoLayerData.ToSubImageRect(texturesExtension.LeftEyeSourceRect, subImageWidth, subImageHeight);

                var currentPosition = worldPose.position;
                var correctedSize = texturesExtension.CropToAspect ?
                    FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                    data.GetScaledSize(transform.lossyScale);

                float transformedX = currentPosition.x + (((texturesExtension.LeftEyeDestinationRect.x + (0.5f * texturesExtension.LeftEyeDestinationRect.width) - 0.5f)) * correctedSize.x);
                float transformedY = currentPosition.y + (((texturesExtension.LeftEyeDestinationRect.y + (0.5f * texturesExtension.LeftEyeDestinationRect.height) - 0.5f)) * (-1.0f * correctedSize.y));
                nativeLayer.Pose = new XrPosef(new Vector3(transformedX, transformedY, currentPosition.z), worldPose.rotation);
                nativeLayer.Size = new XrExtent2Df()
                {
                    Width = correctedSize.x * texturesExtension.LeftEyeDestinationRect.width,
                    Height = correctedSize.y * texturesExtension.LeftEyeDestinationRect.height
                };

                // Apply right-eye source and destination rects for stereo
                if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
                {
                    stereoData.RightNativeLayer.SubImage.ImageRect = OpenXRStereoLayerData.ToSubImageRect(texturesExtension.RightEyeSourceRect, subImageWidth, subImageHeight);

                    float rightTransformedX = currentPosition.x + (((texturesExtension.RightEyeDestinationRect.x + (0.5f * texturesExtension.RightEyeDestinationRect.width) - 0.5f)) * correctedSize.x);
                    float rightTransformedY = currentPosition.y + (((texturesExtension.RightEyeDestinationRect.y + (0.5f * texturesExtension.RightEyeDestinationRect.height) - 0.5f)) * (-1.0f * correctedSize.y));
                    stereoData.RightNativeLayer.Pose = new XrPosef(new Vector3(rightTransformedX, rightTransformedY, currentPosition.z), worldPose.rotation);
                    stereoData.RightNativeLayer.Size = new XrExtent2Df()
                    {
                        Width = correctedSize.x * texturesExtension.RightEyeDestinationRect.width,
                        Height = correctedSize.y * texturesExtension.RightEyeDestinationRect.height
                    };
                }
            }
            else if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                // No custom rects but stereo is active so keep right layer in sync
                stereoData.RightNativeLayer.Pose = nativeLayer.Pose;
                stereoData.RightNativeLayer.Size = nativeLayer.Size;
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
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

        static Vector2 FixAspectRatio(QuadLayerData data, Transform transform, int texWidth, int texHeight)
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


    }
}
#endif
