using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

namespace UnityEngine.XR.OpenXR.CodeSamples.Editor.Tests
{
    public class ApiLayersFeatureExample
    {
#if UNITY_EDITOR
        #region AddLayer
        public void AddLayer(string layerJsonPath, Architecture architecture, BuildTargetGroup targetGroup)
        {
            OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                if (apiLayersFeature.apiLayers.TryAdd(layerJsonPath, architecture, targetGroup, out ApiLayers.ApiLayer layer))
                {
                    Debug.Log($"Successfully imported layer: {layer.name}");
                    Debug.Log($"  Description: {layer.description}");
                    Debug.Log($"  API Version: {layer.apiVersion}");
                }
                else
                {
                    Debug.LogError("Failed to import API layer");
                }
            }
        }
        #endregion


        #region RemoveLayer
        public void RemoveLayer(string layerName, Architecture architecture)
        {
            OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                if (apiLayersFeature.apiLayers.TryRemove(layerName, architecture))
                {
                    Debug.Log($"Successfully removed layer: {layerName}");
                }
                else
                {
                    Debug.LogWarning($"Layer not found: {layerName}");
                }
            }
        }
        #endregion


        #region SetLayerEnabled
        public void SetLayerEnabled(string layerName, bool enabled)
        {
            OpenXRSettings settings = OpenXRSettings.ActiveBuildTargetInstance;
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                // Enable a specific layer for current architecture.
                Architecture arch = RuntimeInformation.ProcessArchitecture;
                apiLayersFeature.apiLayers.SetEnabled(layerName, arch, enabled);

                // Check if a layer is enabled.
                bool isEnabled = apiLayersFeature.apiLayers.IsEnabled(layerName, arch);
                Debug.Log($"Layer {layerName} is enabled: {isEnabled}");
            }
        }
        #endregion

        #region SetLayerEnabledAllArchitectures
        public void SetLayerEnabledAllArchitectures(string layerName, bool enabled)
        {
            OpenXRSettings settings = OpenXRSettings.ActiveBuildTargetInstance;
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                // Enable for all architectures.
                apiLayersFeature.apiLayers.SetEnabled(layerName, enabled);

                // Check if enabled for any architecture.
                bool isEnabledAny = apiLayersFeature.apiLayers.IsEnabled(layerName);
                Debug.Log($"Layer {layerName} is enabled for at least one architecture: {isEnabledAny}");
            }
        }
        #endregion


        #region SetLayerEnabledWithInstance
        public void SetLayerEnabledWithInstance(ApiLayers.ApiLayer layer, bool enabled)
        {
            OpenXRSettings settings = OpenXRSettings.ActiveBuildTargetInstance;
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                apiLayersFeature.apiLayers.SetEnabled(layer, enabled);
            }
        }
        #endregion


        #region SetLayerIndex
        public void SetLayerIndex(int oldIndex, int newIndex)
        {
            OpenXRSettings settings = OpenXRSettings.ActiveBuildTargetInstance;
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                if (apiLayersFeature.apiLayers.SetIndex(oldIndex, newIndex))
                {
                    Debug.Log("Successfully reordered layers");
                }
                else
                {
                    Debug.LogError("Failed to reorder layers - invalid indices");
                }
            }
        }
        #endregion

        #region SetLayerIndexWithNameAndArchitecture
        public void SetLayerIndexWithNameAndArchitecture(string layerName, Architecture architecture, int newIndex)
        {
            OpenXRSettings settings = OpenXRSettings.ActiveBuildTargetInstance;
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                if (apiLayersFeature.apiLayers.SetIndex(layerName, architecture, newIndex))
                {
                    Debug.Log($"Successfully moved {layerName} to index {newIndex}");
                }
            }
        }
        #endregion

        #region SetLayerIndexWithInstance
        public void SetLayerIndexWithInstance(ApiLayers.ApiLayer layer, int newIndex)
        {
            OpenXRSettings settings = OpenXRSettings.ActiveBuildTargetInstance;
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                apiLayersFeature.apiLayers.SetIndex(layer, newIndex);
            }
        }
        #endregion
#endif

        #region ListLayers
        void ListLayers()
        {
            OpenXRSettings settings = OpenXRSettings.ActiveBuildTargetInstance;
            ApiLayersFeature apiLayersFeature = settings.GetFeature<ApiLayersFeature>();

            if (apiLayersFeature != null)
            {
                Debug.Log($"Total layers: {apiLayersFeature.apiLayers.collection.Count}");

                for (int i = 0; i < apiLayersFeature.apiLayers.collection.Count; i++)
                {
                    ApiLayers.ApiLayer layer = apiLayersFeature.apiLayers.collection[i];
                    Debug.Log($"[{i}] {layer.name}");
                    Debug.Log($"    Enabled: {layer.isEnabled}");
                    Debug.Log($"    Architecture: {layer.libraryArchitecture}");
                    Debug.Log($"    API Version: {layer.apiVersion}");
                    Debug.Log($"    Implementation Version: {layer.implementationVersion}");
                    Debug.Log($"    Description: {layer.description}");
                    Debug.Log($"    Library Path: {layer.libraryPath}");
                }
            }
        }
        #endregion
    }

    #region LayerSupportExample
    public class LayerSupportExample : ApiLayers.ISupport
    {
        #region RegisterSupport
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterSupport()
        {
            ApiLayersFeature.AddSupport(new LayerSupportExample());
        }
        #endregion

        public void Setup(System.IntPtr hookGetInstanceProcAddr)
        {
            Debug.Log("Setting up LayerSupportExample");
#if UNITY_EDITOR
            OpenXRSettings openXRSettings = OpenXRSettings.Instance;
#else
            OpenXRSettings openXRSettings = OpenXRSettings.ActiveBuildTargetInstance;
#endif
            if (openXRSettings == null)
                return;

            ApiLayersFeature apiLayersFeature = openXRSettings.GetFeature<ApiLayersFeature>();
            if (apiLayersFeature == null || !apiLayersFeature.enabled)
                return;


            Architecture architecture = RuntimeInformation.ProcessArchitecture;
            if (!apiLayersFeature.apiLayers.IsEnabled("XR_LAYER_NAME_TO_SUPPORT", architecture))
                return;

            // Do some custom setup here to support the layer.
        }

        public void Teardown(ulong xrInstance)
        {
            Debug.Log("Tearing down LayerSupportExample");
            // Do some custom teardown here to clean resources.
        }
    }
    #endregion
}
