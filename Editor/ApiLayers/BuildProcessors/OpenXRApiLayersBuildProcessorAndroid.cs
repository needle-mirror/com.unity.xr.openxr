using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Android;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEditor.XR.OpenXR
{
    /// <summary>
    /// This processor handles platform-specific deployment of API layer libraries and manifest files
    /// during the build process, ensuring they are correctly integrated with the target platform's OpenXR runtime.
    /// Specifically for Android, it manages copying necessary native libraries into the Gradle project structure.
    /// Files are used to track copied libraries to ensure proper cleanup after the build is complete or if build fails or crashes.
    /// </summary>
    internal class OpenXRApiLayersBuildProcessorAndroid : IPreprocessBuildWithReport, IPostGenerateGradleAndroidProject, IPostprocessBuildWithReport
    {
        // File used for more robust caching of what libraries we have imported into the user's gradle project.
        const string k_TrackingFileName = "GradleLibrariesToCleanUp.txt";
        const string k_AndroidSrcPath = "src";
        const string k_AndroidMainPath = "main";
        const string k_AndroidJniLibsPath = "jniLibs";

        static string s_GradleProjectPath;

        public int callbackOrder => 0;

        /// <summary>
        /// Used to check if a given architecture is a targeted architecture for Android.
        /// </summary>
        /// <param name="architecture">The architecture in question.</param>
        public static bool IsTargetedArchitecture(Architecture architecture)
        {
            AndroidArchitecture targetArchs = PlayerSettings.Android.targetArchitectures;

            switch (architecture)
            {
                case Architecture.Arm64:
                    return (targetArchs & AndroidArchitecture.ARM64) != 0;
            }

            return false;
        }

        /// <summary>
        /// Pre-build process that cleans up and creates the tracking file.
        /// </summary>
        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            var targetGroup = report.summary.platformGroup;
            if (targetGroup != BuildTargetGroup.Android)
                return;

            var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            if (openXRSettings == null)
                return;

            var apiLayersFeature = openXRSettings.GetFeature<UnityEngine.XR.OpenXR.Features.ApiLayersFeature>();
            if (apiLayersFeature == null || !apiLayersFeature.enabled)
                return;

            // Cleanup any leftover files from previous builds
            CleanupFromTrackingFile();

            var trackedFilePath = GetTrackedFilePath();
            if (!File.Exists(trackedFilePath))
                File.WriteAllText(trackedFilePath, string.Empty);
        }

        /// <summary>
        /// Called after the Gradle project is generated. This method copies API layer native libraries
        /// to the appropriate architecture-specific directories within the Gradle project.
        /// </summary>
        /// <param name="pathToProject">The path to the generated Gradle project.</param>
        public void OnPostGenerateGradleAndroidProject(string pathToProject)
        {
            var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            if (openXRSettings == null)
                return;

            var apiLayersFeature = openXRSettings.GetFeature<UnityEngine.XR.OpenXR.Features.ApiLayersFeature>();
            if (apiLayersFeature != null && apiLayersFeature.enabled)
                CopyPathlessLibrariesToGradleProject(pathToProject, apiLayersFeature);
        }

        /// <summary>
        /// Called after the build process completes. This method cleans up any temporary library files
        /// that were copied into the Gradle project during the build.
        /// </summary>
        /// <param name="report">A build report containing information about the completed build.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            var targetGroup = report.summary.platformGroup;
            if (targetGroup != BuildTargetGroup.Android)
                return;

            if (!EditorUserBuildSettings.exportAsGoogleAndroidProject)
                CleanupFromTrackingFile();
        }

        string GetTrackedFilePath() => Path.Combine(Application.temporaryCachePath, k_TrackingFileName);

        void TrackCopiedLibrary(string path) => File.AppendAllLines(GetTrackedFilePath(), new[] { path });

        void CleanupFromTrackingFile()
        {
            var trackedFilePath = GetTrackedFilePath();
            if (!File.Exists(trackedFilePath))
                return;

            var filePaths = File.ReadAllLines(trackedFilePath);
            foreach (var filePath in filePaths)
            {
                try
                {
                    if (!File.Exists(filePath))
                        continue;

                    // Verify file is in some expected path
                    var isVerifiedFilePath = string.IsNullOrEmpty(s_GradleProjectPath) ? EndsWithJniLibsPath(filePath) : StartsWithPath(filePath, s_GradleProjectPath);
                    if (isVerifiedFilePath && string.Equals(Path.GetExtension(filePath), ApiLayers.AndroidPlatformSupport.k_SoExt))
                        File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to cleanup {filePath}: {e.Message}");
                }
            }

            File.Delete(trackedFilePath);
        }

        /// <summary>
        /// Copies API layer libraries that are specified by filename only (without a path)
        /// into the `jniLibs` folder of the Android Gradle project.
        /// </summary>
        /// <param name="pathToProject">The path to the Gradle project.</param>
        /// <param name="apiLayersFeature">The API Layers feature for the current build.</param>
        void CopyPathlessLibrariesToGradleProject(string pathToProject, ApiLayersFeature apiLayersFeature)
        {
            s_GradleProjectPath = pathToProject;
            var platformSupport = new ApiLayers.AndroidPlatformSupport();
            var apiLayersDir = platformSupport.GetApiLayersDir();

            // Copy library files to the Gradle project if they were listed by filename only in the manifest.
            // Otherwise, they will be copied into StreamingAssets where the OpenXR runtime will still be able find them.
            foreach (var apiLayer in apiLayersFeature.apiLayers.collection)
            {
                if (!apiLayer.isEnabled)
                    continue;

                // OpenXR only expects libraries that are listed as a file name in their libraryPath to be copied into Gradle.
                var isPath = apiLayer.libraryPath.Contains(Path.DirectorySeparatorChar) || apiLayer.libraryPath.Contains(Path.AltDirectorySeparatorChar);
                if (isPath)
                    continue;

                try
                {
                    if (!IsTargetedArchitecture(apiLayer.libraryArchitecture))
                        continue;

                    var archName = Enum.GetName(typeof(Architecture), apiLayer.libraryArchitecture);
                    if (string.IsNullOrEmpty(archName))
                        continue;

                    var libraryPath = Path.Combine(apiLayersDir, archName.ToLower(), apiLayer.libraryPath.TrimStart(ApiLayers.k_PathTrimChars));

                    if (!File.Exists(libraryPath))
                        continue;

                    var libraryName = Path.GetFileName(libraryPath);

                    var abi = platformSupport.GetExpectedArchitectureImportString(apiLayer.libraryArchitecture);

                    // OpenXR expects these libraries to be in the `src/main/jniLibs` path
                    var targetJniLibsDir = Path.Combine(pathToProject, k_AndroidSrcPath, k_AndroidMainPath, k_AndroidJniLibsPath, abi);
                    if (!Directory.Exists(targetJniLibsDir))
                        Directory.CreateDirectory(targetJniLibsDir);

                    var targetPath = Path.Combine(targetJniLibsDir, libraryName);
                    TrackCopiedLibrary(targetPath);
                    File.Copy(libraryPath, targetPath, true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error copying library for '{apiLayer.name}': {e.Message}");
                }
            }
        }

        bool StartsWithPath(string fullPath, string relativePath)
        {
            var fullSegments = fullPath.Replace('\\', '/').Trim('/').Split('/');
            var relativeSegments = relativePath.Replace('\\', '/').Trim('/').Split('/');

            return fullSegments.Length >= relativeSegments.Length &&
                    fullSegments.Take(relativeSegments.Length)
                                .SequenceEqual(relativeSegments,
                                            StringComparer.OrdinalIgnoreCase);
        }

        bool EndsWithPath(string fullPath, string relativePath)
        {
            var fullParts = fullPath.Split(new[] { Path.DirectorySeparatorChar,
                                    Path.AltDirectorySeparatorChar },
                                            StringSplitOptions.RemoveEmptyEntries);

            var relativeParts = relativePath.Split(new[] { Path.DirectorySeparatorChar,
                                            Path.AltDirectorySeparatorChar },
                                                    StringSplitOptions.RemoveEmptyEntries);

            return fullParts.Length >= relativeParts.Length &&
                    fullParts.Skip(fullParts.Length - relativeParts.Length)
                            .SequenceEqual(relativeParts, StringComparer.OrdinalIgnoreCase);
        }


        bool EndsWithJniLibsPath(string filePath)
        {
            var androidSupport = new ApiLayers.AndroidPlatformSupport();
            foreach (var architecture in androidSupport.GetSupportedArchitectures())
            {
                var abi = androidSupport.GetExpectedArchitectureImportString(architecture);
                var jniLibsDir = Path.Combine(k_AndroidSrcPath, k_AndroidMainPath, k_AndroidJniLibsPath, abi);
                var isEndsWithPath = EndsWithPath(Path.GetFullPath(filePath), jniLibsDir);
                if (isEndsWithPath)
                    return true;
            }

            return false;
        }
    }
}
