#if XR_COMPOSITION_LAYERS
using System.Runtime.InteropServices;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Services;
using Unity.XR.CompositionLayers;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    /// <summary>
    /// Demonstrates custom handler for the custom quad layer.
    /// </summary>
    public class CustomLayerHandler : OpenXRCustomLayerHandler<XrCompositionLayerQuad>
    {
        protected override XrSwapchainCreateInfo CreateSwapchain(CompositionLayerManager.LayerInfo layer)
        {
            unsafe
            {
                return new XrSwapchainCreateInfo()
                {
                    Type = 9,
                    Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                    CreateFlags = 0,
                    UsageFlags = 0x00000020 | 0x00000001,
                    Format = OpenXRLayerUtility.GetDefaultColorFormat(),
                    SampleCount = 1,
                    Width = (uint)layer.Layer.GetComponent<TexturesExtension>().LeftTexture.width,
                    Height = (uint)layer.Layer.GetComponent<TexturesExtension>().LeftTexture.height,
                    FaceCount = 1,
                    ArraySize = 1,
                    MipCount = (uint)layer.Layer.GetComponent<TexturesExtension>().LeftTexture.mipmapCount,
                };
            }
        }

        protected override XrCompositionLayerQuad CreateNativeLayer(CompositionLayerManager.LayerInfo layer, SwapchainCreatedOutput swapchainOutput)
        {
            unsafe
            {
                var data = layer.Layer.LayerData as CustomQuadLayerData;

                return new XrCompositionLayerQuad()
                {
                    Type = 36,
                    Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                    LayerFlags = 0x00000002,
                    Space = OpenXRLayerUtility.GetCurrentAppSpace(),
                    EyeVisibility = 0,
                    SubImage = new SwapchainSubImage()
                    {
                        Swapchain = swapchainOutput.handle,
                        ImageRect = new XrRect2Di()
                        {
                            offset = new XrOffset2Di() { x = 0, y = 0 },
                            extent = new XrExtent2Di()
                            {
                                width = layer.Layer.GetComponent<TexturesExtension>().LeftTexture.width,
                                height = layer.Layer.GetComponent<TexturesExtension>().LeftTexture.height
                            }
                        },
                        ImageArrayIndex = 0
                    },
                    Pose = new XrPosef(layer.Layer.GetComponent<Transform>().position, layer.Layer.GetComponent<Transform>().rotation),
                    Size = new XrExtend2Df()
                    {
                        width = data.GetScaledSize(layer.Layer.GetComponent<Transform>().lossyScale).x,
                        height = data.GetScaledSize(layer.Layer.GetComponent<Transform>().lossyScale).y
                    }
                };
            }
        }

        protected override void ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerQuad nativeLayer)
        {
            var data = layerInfo.Layer.LayerData as CustomQuadLayerData;

            nativeLayer.Pose = new XrPosef(layerInfo.Layer.GetComponent<Transform>().position, layerInfo.Layer.GetComponent<Transform>().rotation);
            nativeLayer.Size = new XrExtend2Df()
            {
                width = data.GetScaledSize(layerInfo.Layer.GetComponent<Transform>().lossyScale).x,
                height = data.GetScaledSize(layerInfo.Layer.GetComponent<Transform>().lossyScale).y
            };

            unsafe
            {
                nativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
            }
        }
    }
}

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XrOffset2Di
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrExtent2Di
    {
        public int width;
        public int height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrRect2Di
    {
        public XrOffset2Di offset;
        public XrExtent2Di extent;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SwapchainSubImage
    {
        public ulong Swapchain;
        public XrRect2Di ImageRect;
        public uint ImageArrayIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrExtend2Df
    {
        public float width;
        public float height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerQuad
    {
        public uint Type;
        public void* Next;
        public ulong LayerFlags;
        public ulong Space;
        public uint EyeVisibility;
        public SwapchainSubImage SubImage;
        public XrPosef Pose;
        public XrExtend2Df Size;
    }
}
#endif
