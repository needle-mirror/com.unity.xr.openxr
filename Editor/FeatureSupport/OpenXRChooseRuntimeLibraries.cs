using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace UnityEditor.XR.OpenXR.Features
{
    internal class OpenXRChooseRuntimeLibraries : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private const string K_openXrLoaderLibName = "openxr_loader";

        public static string DefineEditorLoaderLibraryPath()
        {
            var allFeatureInfo = FeatureHelpersInternal.GetAllFeatureInfo(BuildTargetGroup.Standalone);
            var editorLoaderImporters = PluginImporter.GetAllImporters()
                .Where(importer => importer.GetCompatibleWithEditor() && importer.assetPath.Contains(K_openXrLoaderLibName))
#if UNITY_EDITOR_WIN
                .Where(importer => importer.GetCompatibleWithPlatform(BuildTarget.StandaloneWindows64) && importer.assetPath.EndsWith(".dll"))
#elif UNITY_EDITOR_OSX
                .Where(importer => importer.GetCompatibleWithPlatform(BuildTarget.StandaloneOSX) && importer.assetPath.EndsWith(".dylib"))
#endif
                ;

            Debug.Assert(editorLoaderImporters.Count() > 0, "No Editor-compatible openxr_loader library found");

            var openXrSettings = OpenXRSettings.Instance;
            IEnumerable<PluginImporter> loaderImporters = null;
            if (allFeatureInfo.ActiveCustomLoaderFeature.HasValue)
            {
                var activeCustomLoaderFeature = allFeatureInfo.ActiveCustomLoaderFeature.Value;
                loaderImporters = FindImportersForLoader(editorLoaderImporters, activeCustomLoaderFeature);
            }
            else
            {
                loaderImporters = FindDefaultOpenXRLoaderImporters(editorLoaderImporters, allFeatureInfo.Features);
            }
            return AssetPathToAbsolutePath(loaderImporters.First().assetPath);
        }

        private static string AssetPathToAbsolutePath(string assetPath)
        {
            var path = assetPath.Replace('/', Path.DirectorySeparatorChar);
            if (assetPath.StartsWith("Packages"))
            {
                path = String.Join("" + Path.DirectorySeparatorChar, path.Split(Path.DirectorySeparatorChar).Skip(2));

                return Path.Combine(PackageManager.PackageInfo.FindForAssetPath(assetPath).resolvedPath, path);
            }

            return path;
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            var enabled = BuildHelperUtils.HasActiveLoader(report.summary.platformGroup, typeof(OpenXRLoaderBase));

            var extensions = FeatureHelpersInternal.GetAllFeatureInfo(report.summary.platformGroup);

            // Keep set of seen plugins, only disable plugins that haven't been seen.
            HashSet<string> seenPlugins = new HashSet<string>();

            // Loop over all the native plugin importers and only include the enabled ones in the build
            var importers = PluginImporter.GetAllImporters()
                .Where(importer => importer.GetCompatibleWithPlatform(report.summary.platform));

            var openXrPluginImporters = importers
                .Where(importer => importer.assetPath.Contains(K_openXrLoaderLibName));
            if (enabled)
            {
                // Only include OpenXR loader if OpenXR plugin is active in XR Management
                IncludeOpenXRLoader(openXrPluginImporters, extensions);
            }
            else
            {
                RemoveLoadersFromBuild(openXrPluginImporters);
            }

            foreach (var importer in importers)
            {
                if (importer.assetPath.Contains(K_openXrLoaderLibName))
                {
                    // Ignore OpenXR loaders, these are already processed
                    continue;
                }

                if (importer.assetPath.Contains("UnityOpenXR"))
                {
                    importer.SetIncludeInBuildDelegate(path => enabled);
                }

                var root = Path.GetDirectoryName(importer.assetPath);
                foreach (var extInfo in extensions.Features)
                {
                    if (root != null && root.Contains(extInfo.PluginPath))
                    {
                        if (extInfo.Feature.enabled)
                        {
                            importer.SetIncludeInBuildDelegate(path => enabled);
                        }
                        else if (!seenPlugins.Contains(importer.assetPath))
                        {
                            importer.SetIncludeInBuildDelegate(path => false);
                        }
                        seenPlugins.Add(importer.assetPath);
                    }
                }
            }
        }

        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            var importers = PluginImporter.GetAllImporters();

            // fixes asset bundle building since IPreProcessBuildWithReport isn't called
            foreach (var importer in importers)
            {
                if (importer.assetPath.Contains(K_openXrLoaderLibName))
                {
                    importer.SetIncludeInBuildDelegate(path => false);
                }
            }
        }

        private void IncludeOpenXRLoader(
            IEnumerable<PluginImporter> openXrPluginImporters,
            FeatureHelpersInternal.AllFeatureInfo allFeatureInfo)
        {
            // By default, don't include any OpenXR loader, the correct one will be included later
            RemoveLoadersFromBuild(openXrPluginImporters);

            IEnumerable<PluginImporter> loaderImporters;
            if (allFeatureInfo.ActiveCustomLoaderFeature.HasValue)
            {
                loaderImporters = FindImportersForLoader(openXrPluginImporters, allFeatureInfo.ActiveCustomLoaderFeature.Value);
            }
            else
            {
                loaderImporters = FindDefaultOpenXRLoaderImporters(openXrPluginImporters, allFeatureInfo.Features);
            }

            foreach (var loader in loaderImporters)
            {
                // Include all plugins from features that override multiple architectures (ie Android arm64 and x64)
                loader.SetIncludeInBuildDelegate(path => true);
            }
        }

        private static IEnumerable<PluginImporter> FindDefaultOpenXRLoaderImporters(
            IEnumerable<PluginImporter> openXrPluginImporters,
            IEnumerable<FeatureHelpersInternal.FeatureInfo> customLoaderFeatures)
        {
            var importers = openXrPluginImporters
                .Where(importer => !customLoaderFeatures // Reject any importer that belongs to a feature
                    .Select(feature => feature.PluginPath)
                    .Where(path =>
                    {
                        return Path.GetDirectoryName(importer.assetPath).Contains(path);
                    })
                    .Any());

            if (!importers.Any())
            {
                throw new BuildFailedException("No OpenXR loader library found. Make sure the Unity OpenXR plug-in package is installed properly.");
            }

            return importers;
        }

        private static IEnumerable<PluginImporter> FindImportersForLoader(IEnumerable<PluginImporter> openXrLoaderImporters, FeatureHelpersInternal.FeatureInfo customLoaderFeature)
        {
            var importers = openXrLoaderImporters
                .Where(importer => importer != null && Path.GetDirectoryName(importer.assetPath).Contains(customLoaderFeature.PluginPath));

            if (!importers.Any())
            {
                throw new Exception(
                    $"Build process couldn't find the Plugin Importer for the OpenXR Loader library of the feature \"{customLoaderFeature.Attribute.UiName}\", " +
                    $"the file was expected to be in path: {customLoaderFeature.PluginPath}");
            }

            return importers;
        }

        private void RemoveLoadersFromBuild(IEnumerable<PluginImporter> openXrPluginImporters)
        {
            foreach (var pluginImporter in openXrPluginImporters)
            {
                pluginImporter.SetIncludeInBuildDelegate(path => false);
            }
        }
    }
}
