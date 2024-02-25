#if XR_COMPOSITION_LAYERS

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.Rendering;
using UnityEngine.XR.OpenXR.NativeTypes;
using static Unity.XR.CompositionLayers.Services.CompositionLayerManager;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    //Default OpenXR Composition Layer - Projection Layer support
    internal class OpenXRProjectionLayer : OpenXRLayerProvider.ILayerHandler, IDisposable
    {
        private Dictionary<int, List<RenderTexture>> m_ProjectionRenderTextureMap = new Dictionary<int, List<RenderTexture>>();
        private Dictionary<int, List<Camera>> m_ProjectionRigCameraMap = new Dictionary<int, List<Camera>>();
        private Dictionary<int, LayerInfo> m_activeProjectionRigsMap = new Dictionary<int, LayerInfo>();
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
        public void Dispose()
        {
            Application.onBeforeRender -= UpdateProjectionRigs;
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a new <see cref="LayerData"/>
        /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
        /// </summary>
        public unsafe void CreateLayer(CompositionLayerManager.LayerInfo layer)
        {
            bool result = ext_composition_layers_GetViewConfigurationData(out uint width, out uint height, out uint sampleCount);
            if (!result)
                return;

            var isProjectionRig = layer.Layer.LayerData.GetType() == typeof(ProjectionLayerRigData);
            if (!isProjectionRig)
            {
                TexturesExtension texturesExtension = layer.Layer.GetComponent<TexturesExtension>();
                if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null || texturesExtension.RightTexture == null)
                    return;
            }


            // CreateSwapchain ...
            OpenXRLayerUtility.CreateStereoSwapchain(layer.Id, new XrSwapchainCreateInfo()
            {
             Type = 9,
             Next = OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Swapchain),
             CreateFlags = 0,
             UsageFlags = 0x00000020 | 0x00000001,
             Format = OpenXRLayerUtility.GetDefaultColorFormat(),
             SampleCount = sampleCount,
             Width = width,
             Height = height,
             FaceCount = 1,
             ArraySize = 1,
             MipCount = 1,
            });

            // CreateLayer ...
            ext_composition_layers_createProjectionLayer(layer.Id, OpenXRLayerUtility.GetExtensionsChain(layer, CompositionLayerExtension.ExtensionTarget.Layer), width, height, layer.Layer.LayerData.BlendType);

            // Create and Set Projection Layer Render Textures for Projection Eye Rig
            if (!isProjectionRig)
                return;

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

            m_activeProjectionRigsMap.Add(layer.Id, layer);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// of the type registered to this <c>ILayerHandler</c> instance has been destroyed or disabled.
        /// </summary>
        public void RemoveLayer(int id)
        {
            ext_composition_layers_removeProjectionLayer(id);
            OpenXRLayerUtility.ReleaseSwapchain(id);
            if (m_ProjectionRenderTextureMap.ContainsKey(id))
            {
                m_ProjectionRenderTextureMap[id][0].Release();
                m_ProjectionRenderTextureMap[id][1].Release();
                m_ProjectionRenderTextureMap.Remove(id);
            }

            m_ProjectionRigCameraMap.Remove(id);
            m_activeProjectionRigsMap.Remove(id);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
        /// or any attached extension components have had a member modified.
        /// </summary>
        public unsafe void ModifyLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            // ProjectionRig layers are updated every frame in Application.onBeforeRender due to the left and right cameras changing poses every frame.
            if (layerInfo.Layer.LayerData.GetType() == typeof(ProjectionLayerRigData))
                return;
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.enabled == false || texturesExtension.LeftTexture == null || texturesExtension.RightTexture == null)
                return;
            if (texturesExtension.TextureAdded)
            {
                texturesExtension.TextureAdded = false;
                CreateLayer(layerInfo);
                return;
            }
            ext_composition_layers_modifyProjectionLayer(layerInfo.Id, OpenXRLayerUtility.GetExtensionsChain(layerInfo, CompositionLayerExtension.ExtensionTarget.Layer));
        }

        /// <summary>
        /// Called every frame by the <see cref="OpenXRLayerProvider"/> for all currently active <see cref="LayerData"/> objects
        /// of the type registered to this <c>ILayerHandler</c> instance.
        /// </summary>
        public void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            // ProjectionRig layers are updated every frame in Application.onBeforeRender due to the left and right cameras changing poses every frame.
            if (layerInfo.Layer.LayerData.GetType() == typeof(ProjectionLayerRigData))
                return;

            OpenXRLayerUtility.SetActiveLayerAndGetTexturesForProjectionLayer(layerInfo);
            ext_composition_layers_SetActiveProjectionLayer(layerInfo.Id, layerInfo.Layer.Order);
        }

        /// <summary>
        /// Called by the <see cref="OpenXRLayerProvider"/> during the Unity Update loop.
        /// All implementations must call <see cref="OpenXRLayerUtility.AddActiveLayersToEndFrame(void*,void*,int,int)"/> every frame
        /// to add their native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib.
        /// </summary>
        public void OnUpdate() { }

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

            foreach (var projectionRigLayerInfo in m_activeProjectionRigsMap.Values)
            {
                if (projectionRigLayerInfo.Layer != null)
                {
                    unsafe { ext_composition_layers_modifyProjectionLayer(projectionRigLayerInfo.Id, OpenXRLayerUtility.GetExtensionsChain(projectionRigLayerInfo, CompositionLayerExtension.ExtensionTarget.Layer)); }
                    OpenXRLayerUtility.SetActiveLayerAndGetTexturesForProjectionLayer(projectionRigLayerInfo);
                    ext_composition_layers_SetActiveProjectionLayer(projectionRigLayerInfo.Id, projectionRigLayerInfo.Layer.Order);
                }
            }
        }

        [DllImport("UnityOpenXR")]
        internal static extern unsafe void ext_composition_layers_createProjectionLayer(int id, void* next, uint width, uint height, BlendType blendType);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_removeProjectionLayer(int id);

        [DllImport("UnityOpenXR")]
        internal static extern unsafe void ext_composition_layers_modifyProjectionLayer(int id, void* next);

        [DllImport("UnityOpenXR")]
        internal static extern void ext_composition_layers_SetActiveProjectionLayer(int id, int order);

        [DllImport("UnityOpenXR")]
        internal static extern bool ext_composition_layers_GetViewConfigurationData(out uint width, out uint height, out uint sampleCount);
    }
}


#endif
