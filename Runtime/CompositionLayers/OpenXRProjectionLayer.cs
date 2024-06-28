#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.Rendering;
using UnityEngine.XR.OpenXR.NativeTypes;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    //Default OpenXR Composition Layer - Projection Layer support
    internal class OpenXRProjectionLayer : OpenXRCustomLayerHandler<XrCompositionLayerProjection>, IDisposable
    {
        private Dictionary<int, NativeArray<XrCompositionLayerProjectionView>> m_ProjectionNativeViews = new Dictionary<int, NativeArray<XrCompositionLayerProjectionView>>();
        private Dictionary<int, List<RenderTexture>> m_ProjectionRenderTextureMap = new Dictionary<int, List<RenderTexture>>();
        private Dictionary<int, List<Camera>> m_ProjectionRigCameraMap = new Dictionary<int, List<Camera>>();
        private Camera m_mainCameraCache;
        private bool mainCameraRendered = false;

        /// <summary>
        /// Constructor for OpenXR Projection Layer.
        /// </summary>
        public OpenXRProjectionLayer()
        {
            Application.onBeforeRender += UpdateProjectionRigs;
            RenderPipelineManager.endCameraRendering += OnCameraEndRendering;
            Camera.onPostRender += OnCameraPostRender;
        }

        /// <summary>
        /// Used to clean up.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Application.onBeforeRender -= UpdateProjectionRigs;
            base.Dispose(disposing);
        }

        protected override bool CreateSwapchain(CompositionLayerManager.LayerInfo layer, out SwapchainCreateInfo swapchainCreateInfo)
        {
            bool result = ext_composition_layers_GetViewConfigurationData(out uint width, out uint height, out uint sampleCount);
            if (!result)
            {
                swapchainCreateInfo = default;
                return false;
            }

            var isProjectionRig = layer.Layer.LayerData.GetType() == typeof(ProjectionLayerRigData);
            if (!isProjectionRig)
            {
                TexturesExtension texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null || texturesExtension.RightTexture == null)
                {
                    swapchainCreateInfo = default;
                    return false;
                }
            }

            unsafe
            {
                var xrCreateInfo = new XrSwapchainCreateInfo()
                {
                    Type = (uint)XrStructureType.XR_TYPE_SWAPCHAIN_CREATE_INFO,
                    Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
                    CreateFlags = 0,
                    UsageFlags = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT),
                    Format = OpenXRLayerUtility.GetDefaultColorFormat(),
                    SampleCount = sampleCount,
                    Width = width,
                    Height = height,
                    FaceCount = 1,
                    ArraySize = 1,
                    MipCount = 1,
                };

                swapchainCreateInfo = new SwapchainCreateInfo(xrCreateInfo, isStereo: true);
                return true;
            }
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a new <see cref="LayerData"/>
        /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
        /// </summary>
        protected override unsafe bool CreateNativeLayer(CompositionLayerManager.LayerInfo layer, SwapchainCreatedOutput swapchainCreatedOutput, out XrCompositionLayerProjection nativeLayer)
        {
            bool result = ext_composition_layers_GetViewConfigurationData(out uint width, out uint height, out uint sampleCount);
            if (!result)
            {
                nativeLayer = default;
                return false;
            }

            var isProjectionRig = layer.Layer.LayerData.GetType() == typeof(ProjectionLayerRigData);
            if (!isProjectionRig)
            {
                TexturesExtension texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null || texturesExtension.RightTexture == null)
                {
                    nativeLayer = default;
                    return false;
                }
            }

            XrView xrView1 = default;
            XrView xrView2 = default;
            bool validViews = ext_composition_layers_GetStereoViews(&xrView1, &xrView2);
            XrView[] xrViews = new XrView[2] { xrView1, xrView2 };

            var nativeView1 = new XrCompositionLayerProjectionView()
            {
                Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_PROJECTION_VIEW,
                Next = null,
                Pose = validViews ? xrViews[0].Pose : default,
                Fov = validViews ? xrViews[0].Fov : default,
                SubImage = new XrSwapchainSubImage()
                {
                    Swapchain = swapchainCreatedOutput.handle,
                    ImageRect = new XrRect2Di()
                    {
                        Offset = new XrOffset2Di() { X = 0, Y = 0 },
                        Extent = new XrExtent2Di()
                        {
                            Width = (int)width,
                            Height = (int)height
                        }
                    },
                    ImageArrayIndex = 0
                },
            };

            var nativeView2 = new XrCompositionLayerProjectionView()
            {
                Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_PROJECTION_VIEW,
                Next = null,
                Pose = validViews ? xrViews[1].Pose : default,
                Fov = validViews ? xrViews[1].Fov : default,
                SubImage = new XrSwapchainSubImage()
                {
                    Swapchain = swapchainCreatedOutput.secondStereoHandle,
                    ImageRect = new XrRect2Di()
                    {
                        Offset = new XrOffset2Di() { X = 0, Y = 0 },
                        Extent = new XrExtent2Di()
                        {
                            Width = (int)width,
                            Height = (int)height
                        }
                    },
                    ImageArrayIndex = 0
                },
            };


            var nativeViews = new NativeArray<XrCompositionLayerProjectionView>(2, Allocator.Persistent);
            nativeViews[0] = nativeView1;
            nativeViews[1] = nativeView2;
            m_ProjectionNativeViews[layer.Id] = nativeViews;

            var v = (XrCompositionLayerProjectionView*)nativeViews.GetUnsafePtr();

            nativeLayer = new XrCompositionLayerProjection()
            {
                Type = (uint)XrStructureType.XR_TYPE_COMPOSITION_LAYER_PROJECTION,
                Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer),
                LayerFlags = XrCompositionLayerFlags.SourceAlpha,
                Space = OpenXRLayerUtility.GetCurrentAppSpace(),
                ViewCount = 2,
                Views = v
            };

            // Create and Set Projection Layer Render Textures for Projection Eye Rig
            if (!isProjectionRig)
                return true;

            m_mainCameraCache = Camera.main;

            Camera leftCamera = null;
            Camera rightCamera = null;
            if (layer.Layer.transform.childCount >= 2)
            {
                leftCamera = layer.Layer.transform.GetChild(0).GetComponent<Camera>();
                rightCamera = layer.Layer.transform.GetChild(1).GetComponent<Camera>();
                m_ProjectionRigCameraMap[layer.Id] = new List<Camera>() { leftCamera, rightCamera };
            }

            String layerName = String.Format("projectLayer_{0}", layer.Id);
            RenderTexture leftRT = new RenderTexture((int)width, (int)height, 0, RenderTextureFormat.ARGB32) {name = layerName + "_left", depthStencilFormat = Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt };
            leftRT.Create();
            RenderTexture rightRT = new RenderTexture((int)width, (int)height, 0, RenderTextureFormat.ARGB32) {name = layerName + "_right", depthStencilFormat = Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt };
            rightRT.Create();
            m_ProjectionRenderTextureMap[layer.Id] = new List<RenderTexture>() {leftRT, rightRT};

            TexturesExtension texExt = layer.Layer.GetComponent<TexturesExtension>();
            texExt.LeftTexture = leftRT;
            texExt.RightTexture = rightRT;

            if (leftCamera)
            {
                leftCamera.targetTexture = leftRT;
                leftCamera.enabled = false;
            }
            if (rightCamera != null)
            {
                rightCamera.targetTexture = rightRT;
                rightCamera.enabled = false;
            }
            return true;
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// of the type registered to this <c>ILayerHandler</c> instance has been destroyed or disabled.
        /// </summary>
        public override void RemoveLayer(int id)
        {
            if (m_ProjectionRenderTextureMap.ContainsKey(id))
            {
                m_ProjectionRenderTextureMap[id][0].Release();
                m_ProjectionRenderTextureMap[id][1].Release();
                m_ProjectionRenderTextureMap.Remove(id);
            }

            m_ProjectionRigCameraMap.Remove(id);

            foreach(var nativeArray in m_ProjectionNativeViews.Values)
            {
                nativeArray.Dispose();
            }

            base.RemoveLayer(id);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// or any attached extension components have had a member modified.
        /// </summary>
        protected override unsafe bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerProjection nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null || texturesExtension.RightTexture == null)
                return false;

            XrView view1 = default;
            XrView view2 = default;
            bool validViews = ext_composition_layers_GetStereoViews(&view1, &view2);
            if (validViews)
            {
                XrView[] views = new XrView[2] { view1, view2 };
                nativeLayer.Views[0].Pose = views[0].Pose;
                nativeLayer.Views[0].Fov = views[0].Fov;
                nativeLayer.Views[1].Pose = views[1].Pose;
                nativeLayer.Views[1].Fov = views[1].Fov;
            }
            nativeLayer.Next = OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer);
            return true;
        }

        /// <summary>
        /// Called every frame by the <see cref="OpenXRLayerProvider"/> for all currently active <see cref="LayerData"/> objects
        /// of the type registered to this <c>ILayerHandler</c> instance.
        /// </summary>
        protected override unsafe bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref XrCompositionLayerProjection nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension != null || texturesExtension.enabled && texturesExtension.CustomRects && texturesExtension.LeftTexture != null || texturesExtension.RightTexture != null)
            {
                nativeLayer.Views[0].SubImage.ImageRect = GetSubImageOffsetAndExtent(texturesExtension.LeftEyeSourceRect, new Vector2(texturesExtension.LeftTexture?.width ?? 0, texturesExtension.LeftTexture?.height ?? 0));
                nativeLayer.Views[1].SubImage.ImageRect = GetSubImageOffsetAndExtent(texturesExtension.RightEyeSourceRect, new Vector2(texturesExtension.RightTexture?.width ?? 0, texturesExtension.RightTexture?.height ?? 0));
            }

            nativeLayer.Space = OpenXRLayerUtility.GetCurrentAppSpace();
            OpenXRLayerUtility.SetActiveLayerAndGetTexturesForProjectionLayer(layerInfo);
            return true;
        }

        // Used to hault calls to GetStereoProjectionMatrix before the main camera's projection matrix has been initialized
        private void OnCameraPostRender(Camera cam)
        {
            if (mainCameraRendered || m_mainCameraCache == null || cam != m_mainCameraCache)
                return;

            mainCameraRendered = true;
            Camera.onPostRender -= OnCameraPostRender;
            RenderPipelineManager.endCameraRendering -= OnCameraEndRendering;
        }

        // Used to hault calls to GetStereoProjectionMatrix before the main camera's projection matrix has been initialized
        private void OnCameraEndRendering(ScriptableRenderContext cxt, Camera cam) => OnCameraPostRender(cam);

        private void UpdateProjectionRigs()
        {
            foreach (var cameras in m_ProjectionRigCameraMap.Values)
            {
                var leftCamera = cameras[0];
                var rightCamera = cameras[1];

                if (mainCameraRendered && leftCamera != null && rightCamera != null)
                {
                    leftCamera.projectionMatrix = m_mainCameraCache.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                    rightCamera.projectionMatrix = m_mainCameraCache.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

                    leftCamera.Render();
                    rightCamera.Render();
                }
            }
        }

        XrRect2Di GetSubImageOffsetAndExtent(Rect sourceRect, Vector2 textureSize)
        {
            var sourceTextureOffset = new XrOffset2Di()
            {
                X = (int)(sourceRect.x * textureSize.x),
                Y = (int)(sourceRect.y * textureSize.y)
            };

            var sourceTextureExtent = new XrExtent2Di()
            {
                Width = (int)(sourceRect.width * textureSize.x),
                Height = (int)(sourceRect.height * textureSize.y)
            };

            return new XrRect2Di { Offset = sourceTextureOffset, Extent = sourceTextureExtent };
        }

        [DllImport("UnityOpenXR")]
        internal static extern bool ext_composition_layers_GetViewConfigurationData(out uint width, out uint height, out uint sampleCount);

        [DllImport("UnityOpenXR")]
        internal unsafe static extern bool ext_composition_layers_GetStereoViews(XrView* view1, XrView* view2);
    }
}


#endif
