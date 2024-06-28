#if XR_COMPOSITION_LAYERS
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

                if (texturesExtension.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texturesExtension.LeftTexture != null)
                {
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

                    swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: false);
                    return true;
                }
#if UNITY_ANDROID
                else
                {
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
                }
#else
                swapchainCreateInfo = default;
                return false;
#endif
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
                if (texturesExtension.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texturesExtension.LeftTexture != null)
                {
                    subImageWidth = texturesExtension.LeftTexture.width;
                    subImageHeight = texturesExtension.LeftTexture.height;
                }
#if UNITY_ANDROID
                else
                {
                    subImageWidth = (int)texturesExtension.Resolution.x;
                    subImageHeight = (int)texturesExtension.Resolution.y;
                }
#endif
                var correctedSize = texturesExtension.CropToAspect ?
                    FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                    data.GetScaledSize(transform.lossyScale);

                nativeLayer = new XrCompositionLayerQuad()
                {
                    Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_QUAD,
                    Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    LayerFlags = data.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha,
                    Space = OpenXRLayerUtility.GetCurrentAppSpace(),
                    EyeVisibility = 0,
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
                    Pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).rotation),
                    Size = new XrExtent2Df()
                    {
                        Width = correctedSize.x,
                        Height = correctedSize.y
                    }
                };
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
            nativeLayer.Pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).rotation);


            int subImageWidth = 0;
            int subImageHeight = 0;
            if (texturesExtension.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texturesExtension.LeftTexture != null)
            {
                subImageWidth = texturesExtension.LeftTexture.width;
                subImageHeight = texturesExtension.LeftTexture.height;
            }
#if UNITY_ANDROID
            else
            {
                subImageWidth = (int)texturesExtension.Resolution.x;
                subImageHeight = (int)texturesExtension.Resolution.y;
            }
#endif
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

            return true;
        }

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerQuad nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false)
                return false;

            var data = layerInfo.Layer.LayerData as QuadLayerData;
            var transform = layerInfo.Layer.GetComponent<Transform>();
            nativeLayer.Pose = new XrPosef(OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).position, OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).rotation);
            nativeLayer.Space = OpenXRLayerUtility.GetCurrentAppSpace();

            if (texturesExtension.CustomRects)
            {
                int subImageWidth = 0;
                int subImageHeight = 0;
                if (texturesExtension.sourceTexture == TexturesExtension.SourceTextureEnum.LocalTexture && texturesExtension.LeftTexture != null)
                {
                    subImageWidth = texturesExtension.LeftTexture.width;
                    subImageHeight = texturesExtension.LeftTexture.height;
                }
#if UNITY_ANDROID
                else
                {
                    subImageWidth = (int)texturesExtension.Resolution.x;
                    subImageHeight = (int)texturesExtension.Resolution.y;
                }
#endif
                nativeLayer.SubImage.ImageRect = new XrRect2Di()
                {
                    Offset = new XrOffset2Di()
                    {
                        X = (int)(subImageWidth * texturesExtension.LeftEyeSourceRect.x),
                        Y = (int)(subImageHeight * texturesExtension.LeftEyeSourceRect.y)
                    },

                    Extent = new XrExtent2Di()
                    {
                        Width = (int)(subImageWidth * texturesExtension.LeftEyeSourceRect.width),
                        Height = (int)(subImageHeight * texturesExtension.LeftEyeSourceRect.height)
                    }
                };

                var currentPosition = OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).position;
                var correctedSize = texturesExtension.CropToAspect ?
                    FixAspectRatio(data, transform, subImageWidth, subImageHeight) :
                    data.GetScaledSize(transform.lossyScale);

                float transformedX = currentPosition.x + (((texturesExtension.LeftEyeDestinationRect.x + (0.5f * texturesExtension.LeftEyeDestinationRect.width) - 0.5f)) * correctedSize.x);
                float transformedY = currentPosition.y + (((texturesExtension.LeftEyeDestinationRect.y + (0.5f * texturesExtension.LeftEyeDestinationRect.height) - 0.5f)) * (-1.0f * correctedSize.y));
                nativeLayer.Pose = new XrPosef(new Vector3(transformedX, transformedY, currentPosition.z), OpenXRUtility.ComputePoseToWorldSpace(transform, OpenXRLayerUtility.mainCameraCache).rotation);
                nativeLayer.Size = new XrExtent2Df()
                {
                    Width = correctedSize.x * texturesExtension.LeftEyeDestinationRect.width,
                    Height = correctedSize.y * texturesExtension.LeftEyeDestinationRect.height
                };
            }

            return base.ActiveNativeLayer(layerInfo, ref nativeLayer);
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
    }
}


#endif
