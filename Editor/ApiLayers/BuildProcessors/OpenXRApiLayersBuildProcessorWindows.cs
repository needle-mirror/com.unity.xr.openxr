using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UnityEditor.XR.OpenXR
{
    /// <summary>
    /// This processor handles platform-specific deployment of API layer libraries and manifest files
    /// during the build process, ensuring they are correctly integrated with the target platform's OpenXR runtime.
    /// </summary>
    internal class OpenXRApiLayersBuildProcessorWindows : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            var target = report.summary.platform;
            if (target != BuildTarget.StandaloneWindows64 && target != BuildTarget.StandaloneWindows)
                return;

            // Get the corresponding BuildTargetGroup
            var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
            var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);

            if (openXRSettings == null)
                return;

            var apiLayersFeature = openXRSettings.GetFeature<UnityEngine.XR.OpenXR.Features.ApiLayersFeature>();
            if (apiLayersFeature == null || !apiLayersFeature.enabled)
                return;

            // Construct the runtime path for explicit layers within StreamingAssets
            string apiLayersDir = new ApiLayers.WindowsPlatformSupport().GetApiLayersDir();

            // Copy library files to the build root if they were listed by filename only in the manifest where the OpenXR runtime will find them.
            // Otherwise, they will be copied into StreamingAssets where the OpenXR runtime will still be able find them.
            foreach (var apiLayer in apiLayersFeature.apiLayers.collection)
            {
                if (!apiLayer.isEnabled)
                    continue;

                // OpenXR only expects libraries that are listed as a file name in their libraryPath to be copied into Gradle.
                var isPath = apiLayer.libraryPath.Contains(Path.DirectorySeparatorChar) || apiLayer.libraryPath.Contains(Path.AltDirectorySeparatorChar);
                if (isPath)
                    continue;

                // Copy these libraries into the standalone build directory.
                try
                {
                    var archName = Enum.GetName(typeof(Architecture), apiLayer.libraryArchitecture);
                    if (string.IsNullOrEmpty(archName))
                        continue;

                    var libraryPath = Path.Combine(apiLayersDir, archName.ToLower(), apiLayer.libraryPath.TrimStart(ApiLayers.k_PathTrimChars));
                    if (!File.Exists(libraryPath))
                        continue;

                    string libraryName = Path.GetFileName(libraryPath);

                    // For Windows, OpenXR will expect this kind of library to be next to the final .exe.
                    string buildDir = Path.GetDirectoryName(report.summary.outputPath);
                    string targetPath = Path.Combine(buildDir, libraryName);
                    File.Copy(libraryPath, targetPath, true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error processing layer '{apiLayer.name}': {e.Message}");
                }
            }
        }
    }
}
