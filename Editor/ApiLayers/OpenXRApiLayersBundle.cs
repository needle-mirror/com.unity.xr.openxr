using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEditor.XR.OpenXR
{
    /// <summary>
    /// Provides utility methods for discovering and importing API layers that are bundled
    /// with the Unity OpenXR package for the current platform.
    /// </summary>
    internal static class OpenXRApiLayersBundle
    {
#if UNITY_EDITOR
        /// <summary>
        /// Gets the directory containing bundled API layers for a specific build target group.
        /// </summary>
        /// <param name="targetGroup">The build target group (e.g., Standalone, Android).</param>
        /// <returns>The path to the bundled API layers directory, or an empty string if not found.</returns>
        static string GetBundleDir(BuildTargetGroup targetGroup)
        {
            ApiLayers.IPlatformSupport platformSupport = null;
            switch (targetGroup)
            {
                case BuildTargetGroup.Standalone:
                    // Currently, only Windows is supported for standalone bundled layers.
                    platformSupport = new ApiLayers.WindowsPlatformSupport();
                    break;
                case BuildTargetGroup.Android:
                    platformSupport = new ApiLayers.AndroidPlatformSupport();
                    break;
                default:
                    return string.Empty;
            }

            return platformSupport?.GetBundleDir() ?? string.Empty;
        }

        /// <summary>
        /// Adds all bundled API layers for the currently selected build target to the OpenXR settings.
        /// </summary>
        /// <param name="openXRSettings">The OpenXR settings instance to add the layers to.</param>
        /// <param name="bundleDir">The directory to the bundle.</param>
        public static void AddBundle(BuildTargetGroup targetGroup)
        {
            var bundleDir = GetBundleDir(targetGroup);
            if (!Directory.Exists(bundleDir))
                return;

            var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            if (openXRSettings == null)
                return;

            var apiLayersFeature = openXRSettings.GetFeature<ApiLayersFeature>();
            if (apiLayersFeature == null)
                return;

            ApiLayers.IPlatformSupport platformSupport = null;
            switch (targetGroup)
            {
                case BuildTargetGroup.Standalone:
                    // Currently, only Windows is supported for standalone bundled layers.
                    platformSupport = new ApiLayers.WindowsPlatformSupport();
                    break;
                case BuildTargetGroup.Android:
                    platformSupport = new ApiLayers.AndroidPlatformSupport();
                    break;
                default:
                    return;
            }

            if (platformSupport == null)
                return;

            // Get the api layers for each supported architecture.
            foreach (var architecture in platformSupport.GetSupportedArchitectures())
            {
                var architectureName = Enum.GetName(typeof(Architecture), architecture)?.ToLower();
                if (string.IsNullOrEmpty(architectureName))
                    continue;

                var jsonFilesDir = Path.Combine(bundleDir, architectureName);

                if (!Directory.Exists(jsonFilesDir))
                    continue;

                // Find all JSON manifest files in the bundle directory for this architecture
                var jsonFiles = Directory.GetFiles(jsonFilesDir, $"*{ApiLayers.k_JsonExt}", SearchOption.TopDirectoryOnly);

                foreach (var jsonPath in jsonFiles)
                {
                    apiLayersFeature.apiLayers.TryAdd(jsonPath, architecture, targetGroup, out _);
                }
            }
        }
#endif
    }
}
