using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEditor.XR.OpenXR
{
    /// <summary>
    /// Build processor responsible for copying enabled OpenXR API Layer files into the StreamingAssets folder of a Unity build.
    /// </summary>
    internal class OpenXRApiLayersBuildProcessor : BuildPlayerProcessor
    {
        /// <summary>
        /// Prepares enabled API layer files for inclusion in the build by copying them to the StreamingAssets directory.
        /// This method processes only the API layers that are marked as enabled in the OpenXR settings and ensures
        /// their manifest and library files are available at runtime.
        /// </summary>
        /// <param name="buildPlayerContext">Context containing build configuration and options.</param>
        public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
        {
            var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildPlayerContext.BuildPlayerOptions.targetGroup);
            if (openXRSettings == null)
                return;

            var apiLayersFeature = openXRSettings.GetFeature<ApiLayersFeature>();
            if (apiLayersFeature == null || !apiLayersFeature.enabled)
                return;

            AddEnabledLayersToStreamingAssets(buildPlayerContext, apiLayersFeature);
        }

        /// <summary>
        /// Copies the manifest and library files of all enabled API layers to the appropriate StreamingAssets path.
        /// </summary>
        /// <param name="buildPlayerContext">The build context, used to add files to StreamingAssets.</param>
        /// <param name="apiLayersFeature">The API Layers feature containing the list of API layers.</param>
        protected void AddEnabledLayersToStreamingAssets(BuildPlayerContext buildPlayerContext, ApiLayersFeature apiLayersFeature)
        {
            ApiLayers.IPlatformSupport platformSupport;
            Architecture targetArchitecture = Architecture.X86;

            switch (buildPlayerContext.BuildPlayerOptions.target)
            {
                case BuildTarget.StandaloneWindows:
                    targetArchitecture = Architecture.X86;
                    platformSupport = new ApiLayers.WindowsPlatformSupport();
                    break;
                case BuildTarget.StandaloneWindows64:
                    targetArchitecture = Architecture.X64;
                    platformSupport = new ApiLayers.WindowsPlatformSupport();
                    break;
                case BuildTarget.Android:
                    platformSupport = new ApiLayers.AndroidPlatformSupport();
                    break;
                default:
                    return;
            }

            foreach (var apiLayer in apiLayersFeature.apiLayers.collection)
            {
                if (!apiLayer.isEnabled)
                    continue;

                // We don't need to verify the layer's platform because the api layer object comes from the OpenXRSettings of the build target.
                if (string.IsNullOrEmpty(apiLayer.jsonFileName))
                    continue;

                // Construct the runtime path for explicit layers within StreamingAssets
                try
                {
                    var majorApiVersion = ApiLayers.k_OpenXRApiMajorVersionFallback;

                    // OpenXR spec demands that the json apiVersion be in major.minor form here
                    if (Version.TryParse(apiLayer.apiVersion, out Version version))
                        majorApiVersion = version.Major.ToString();
                    else
                        Debug.LogWarning($"Failed to parse apiLayer.apiVersion, using fallback major version {ApiLayers.k_OpenXRApiMajorVersionFallback} instead.");

                    var explicitLayersDir = Path.Combine(ApiLayers.k_RuntimeOpenXRPath, majorApiVersion, ApiLayers.k_RuntimeApiLayersPath, ApiLayers.k_RuntimeExplicitLayersDir);

                    var archName = Enum.GetName(typeof(Architecture), apiLayer.libraryArchitecture)?.ToLower();
                    if (string.IsNullOrEmpty(archName))
                        continue;

                    var apiLayersDir = Path.Combine(platformSupport.GetApiLayersDir(), archName);
                    var jsonPath = Path.GetFullPath(Path.Combine(apiLayersDir, apiLayer.jsonFileName));
                    // Ensure the JSON manifest file exists before attempting to copy it
                    if (!File.Exists(jsonPath))
                    {
                        Debug.LogError($"Error processing layer '{apiLayer.name}': Json file '{apiLayer.jsonFileName}' does not exist.");
                        continue;
                    }

                    var isTargetArchitecture = apiLayer.libraryArchitecture == targetArchitecture;

                    // Special case for android since it can have multiple architectures targeted.
                    if (buildPlayerContext.BuildPlayerOptions.target == BuildTarget.Android)
                        isTargetArchitecture = OpenXRApiLayersBuildProcessorAndroid.IsTargetedArchitecture(apiLayer.libraryArchitecture);

                    if (!isTargetArchitecture)
                        continue;

                    // Only copy the libraries that have paths into StreamingAssets here.
                    // A library path that is just a file name (no directory info) is handled by a platform-specific mechanism (e.g., Android Gradle copy step).
                    // OpenXR will expect this behavior.
                    bool isPath = apiLayer.libraryPath.Contains(Path.DirectorySeparatorChar) || apiLayer.libraryPath.Contains(Path.AltDirectorySeparatorChar);
                    if (!isPath)
                    {
                        buildPlayerContext.AddAdditionalPathToStreamingAssets(jsonPath, Path.Combine(explicitLayersDir, apiLayer.jsonFileName));
                    }
                    else
                    {
                        var libraryFileName = Path.GetFileName(apiLayer.libraryPath);
                        var libraryPath = Path.Combine(apiLayersDir, libraryFileName);
                        if (!File.Exists(libraryPath))
                        {
                            Debug.LogError($"Error processing layer '{apiLayer.name}': Library file '{libraryPath}' does not exist.");
                            continue;
                        }
                        buildPlayerContext.AddAdditionalPathToStreamingAssets(libraryPath, Path.Combine(explicitLayersDir, libraryFileName));
                        buildPlayerContext.AddAdditionalPathToStreamingAssets(jsonPath, Path.Combine(explicitLayersDir, apiLayer.jsonFileName));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error processing layer '{apiLayer.name}': {e.Message}");
                }
            }
        }
    }
}
