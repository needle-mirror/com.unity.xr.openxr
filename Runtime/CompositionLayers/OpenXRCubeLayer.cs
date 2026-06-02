#if XR_COMPOSITION_LAYERS
using System.Collections.Generic;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    internal class OpenXRCubeLayer : OpenXRCustomLayerHandler<XrCompositionLayerCubeKHR>
    {
        public static bool ExtensionEnabled = OpenXRRuntime.IsExtensionEnabled("XR_KHR_composition_layer_cube");

        Dictionary<int, OpenXRStereoLayerData.RightEyeData<XrCompositionLayerCubeKHR>> m_StereoData = new();

        protected override unsafe bool CreateSwapchain(CompositionLayerManager.LayerInfo layerInfo, out SwapchainCreateInfo swapchainCreateInfo)
        {
            TexturesExtension texture = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texture == null || texture.enabled == false || texture.LeftTexture == null)
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
                Width = (uint)(texture.LeftTexture.width),
                Height = (uint)(texture.LeftTexture.height),
                FaceCount = 6,
                ArraySize = 1,
                MipCount = (uint)texture.LeftTexture.mipmapCount,
            };

            swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isExternalSurface: false, isStereo: OpenXRStereoLayerData.IsStereoRequested(texture));
            return true;
        }

        protected override unsafe bool CreateNativeLayer(CompositionLayerManager.LayerInfo layerInfo, SwapchainCreatedOutput swapchainOutput, out XrCompositionLayerCubeKHR nativeLayer)
        {
            var data = layerInfo.Layer.LayerData as CubeProjectionLayerData;
            var transform = layerInfo.Layer.GetComponent<Transform>();
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();

            bool isStereo = OpenXRStereoLayerData.IsStereoRequested(texturesExtension)
                && swapchainOutput.secondStereoHandle != 0;

            var layerFlags = data.BlendType == BlendType.Premultiply ? XrCompositionLayerFlags.SourceAlpha : XrCompositionLayerFlags.SourceAlpha | XrCompositionLayerFlags.UnPremultipliedAlpha;
            var orientation = new XrQuaternionf(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);

            nativeLayer = new XrCompositionLayerCubeKHR()
            {
                Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_CUBE_KHR,
                Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer),
                LayerFlags = layerFlags,
                Space = OpenXRLayerUtility.GetCurrentAppSpace(),
                EyeVisibility = OpenXRStereoLayerData.GetEyeVisibility(texturesExtension, isStereo),
                Swapchain = swapchainOutput.handle,
                ImageArrayIndex = 0,
                Orientation = orientation
            };

            if (isStereo)
            {
                m_StereoData[layerInfo.Id] = new OpenXRStereoLayerData.RightEyeData<XrCompositionLayerCubeKHR>
                {
                    RightNativeLayer = new XrCompositionLayerCubeKHR()
                    {
                        Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_CUBE_KHR,
                        Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer),
                        LayerFlags = layerFlags,
                        Space = OpenXRLayerUtility.GetCurrentAppSpace(),
                        EyeVisibility = XrEyeVisibility.Right,
                        Swapchain = swapchainOutput.secondStereoHandle,
                        ImageArrayIndex = 0,
                        Orientation = orientation
                    },
                    LeftTexture = texturesExtension?.LeftTexture,
                    RightTexture = texturesExtension?.RightTexture,
                };
            }
            else
            {
                m_StereoData.Remove(layerInfo.Id);
            }

            return true;
        }

        protected override bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerCubeKHR nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null)
                return false;

            var transform = layerInfo.Layer.GetComponent<Transform>();
            nativeLayer.Space = OpenXRLayerUtility.GetCurrentAppSpace();
            nativeLayer.Orientation = new XrQuaternionf(OpenXRUtility.ComputePoseToWorldSpace(transform, CompositionLayerManager.mainCameraCache).rotation);

            if (m_StereoData.TryGetValue(layerInfo.Id, out var stereoData))
            {
                stereoData.RightNativeLayer.Space = nativeLayer.Space;
                stereoData.RightNativeLayer.Orientation = nativeLayer.Orientation;
                stereoData.RightNativeLayer.LayerFlags = nativeLayer.LayerFlags;
            }

            return true;
        }

        protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerCubeKHR nativeLayer)
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
#endif
