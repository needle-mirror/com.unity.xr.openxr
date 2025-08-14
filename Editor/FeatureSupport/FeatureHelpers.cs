using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;

[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Tests")]
[assembly: InternalsVisibleTo("Unity.XR.OpenXR.TestTooling")]
namespace UnityEditor.XR.OpenXR.Features
{
    /// <summary>
    /// Editor OpenXR Feature helpers.
    /// </summary>
    public static class FeatureHelpers
    {
        /// <summary>
        /// Discovers all features in project and ensures that OpenXRSettings.Instance.features is up to date
        /// for selected build target group.
        /// </summary>
        /// <param name="group">build target group to refresh</param>
        public static void RefreshFeatures(BuildTargetGroup group)
        {
            FeatureHelpersInternal.GetAllFeatureInfo(group);
        }

        /// <summary>
        /// Given a feature id, returns the first instance of <see cref="OpenXRFeature" /> associated with that id.
        /// </summary>
        /// <param name="featureId">The unique id identifying the feature</param>
        /// <returns>The instance of the feature matching thd id, or null.</returns>
        public static OpenXRFeature GetFeatureWithIdForActiveBuildTarget(string featureId)
        {
            return GetFeatureWithIdForBuildTarget(BuildPipeline.GetBuildTargetGroup(UnityEditor.EditorUserBuildSettings.activeBuildTarget), featureId);
        }

        /// <summary>
        /// Given an array of feature ids, returns an array of matching <see cref="OpenXRFeature" /> instances.
        /// </summary>
        /// <param name="featureIds">Array of feature ids to match against.</param>
        /// <returns>An array of all matching features.</returns>
        public static OpenXRFeature[] GetFeaturesWithIdsForActiveBuildTarget(string[] featureIds)
        {
            return GetFeaturesWithIdsForBuildTarget(BuildPipeline.GetBuildTargetGroup(UnityEditor.EditorUserBuildSettings.activeBuildTarget), featureIds);
        }

        /// <summary>
        /// Given a feature id, returns the first <see cref="OpenXRFeature" /> associated with that id.
        /// </summary>
        /// <param name="buildTargetGroup">The build target group to get the feature from.</param>
        /// <param name="featureId">The unique id identifying the feature</param>
        /// <returns>The instance of the feature matching thd id, or null.</returns>
        public static OpenXRFeature GetFeatureWithIdForBuildTarget(BuildTargetGroup buildTargetGroup, string featureId)
        {
            if (String.IsNullOrEmpty(featureId))
                return null;

            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
            if (settings == null)
                return null;

            foreach (var feature in settings.features)
            {
                if (String.Compare(featureId, feature.featureIdInternal, true) == 0)
                    return feature;
            }

            return null;
        }

        /// <summary>
        /// Given an array of feature ids, returns an array of matching <see cref="OpenXRFeature" /> instances that match.
        /// </summary>
        /// <param name="buildTargetGroup">The build target group to get the feature from.</param>
        /// <param name="featureIds">Array of feature ids to match against.</param>
        /// <returns>An array of all matching features.</returns>
        public static OpenXRFeature[] GetFeaturesWithIdsForBuildTarget(BuildTargetGroup buildTargetGroup, string[] featureIds)
        {
            List<OpenXRFeature> ret = new List<OpenXRFeature>();

            if (featureIds == null || featureIds.Length == 0)
                return ret.ToArray();

            foreach (var featureId in featureIds)
            {
                var feature = GetFeatureWithIdForBuildTarget(buildTargetGroup, featureId);
                if (feature != null)
                    ret.Add(feature);
            }

            return ret.ToArray();
        }
    }


    internal static class FeatureHelpersInternal
    {
        public class AllFeatureInfo
        {
            public List<FeatureInfo> Features;
            public FeatureInfo? ActiveCustomLoaderFeature;
        }

        public enum FeatureInfoCategory
        {
            Feature,
            Interaction
        }

        public struct FeatureInfo
        {
            public string PluginPath;
            public OpenXRFeatureAttribute Attribute;
            public OpenXRFeature Feature;
            public FeatureInfoCategory Category;
            public OpenXRApiVersion LoaderVersion;
            public bool HasLoaderForBuildTarget;
        }

        private static FeatureInfoCategory DetermineExtensionCategory(string extensionCategoryString)
        {
            if (String.Compare(extensionCategoryString, FeatureCategory.Interaction) == 0)
            {
                return FeatureInfoCategory.Interaction;
            }

            return FeatureInfoCategory.Feature;
        }

        /// <summary>
        /// Gets all features for group. If serialized feature instances do not exist, creates them.
        /// </summary>
        /// <param name="group">BuildTargetGroup to get feature information for.</param>
        /// <returns>feature info</returns>
        public static AllFeatureInfo GetAllFeatureInfo(BuildTargetGroup group)
        {
            AllFeatureInfo ret = new()
            {
                Features = new List<FeatureInfo>(),
                ActiveCustomLoaderFeature = null
            };
            var openXrPackageSettings = OpenXRPackageSettings.GetOrCreateInstance();
            var openXrSettings = openXrPackageSettings.GetSettingsForBuildTargetGroup(group);
            if (openXrSettings == null)
            {
                return ret;
            }
            var assetPath = Path.Combine(OpenXRPackageSettings.GetAssetPathForComponents(OpenXRPackageSettings.s_PackageSettingsDefaultSettingsPath), openXrPackageSettings.name + ".asset");
            var openXrExtensionAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            if (openXrExtensionAssets == null || openXrExtensionAssets.Length == 0)
            {
                string[] guids = AssetDatabase.FindAssets("t:OpenXRSettings");

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var candidateAssets = AssetDatabase.LoadAllAssetsAtPath(path);

                    if (candidateAssets.Any(obj => obj != null && obj.name == openXrPackageSettings.name))
                    {
                        openXrExtensionAssets = candidateAssets;
                        break;
                    }
                }
            }


            // Find any current extensions that are already serialized
            var currentExts = new Dictionary<OpenXRFeatureAttribute, OpenXRFeature>();
            var buildGroupName = group.ToString();

            foreach (var ext in openXrExtensionAssets)
            {
                if (ext == null || !ext.name.Contains(buildGroupName))
                    continue;

                foreach (Attribute attr in Attribute.GetCustomAttributes(ext.GetType()))
                {
                    if (attr is OpenXRFeatureAttribute)
                    {
                        var extAttr = (OpenXRFeatureAttribute)attr;
                        currentExts[extAttr] = (OpenXRFeature)ext;
                        break;
                    }
                }
            }

            // Find any extensions that haven't yet been added to the feature list and create instances of them
            List<OpenXRFeature> all = new List<OpenXRFeature>();
            foreach (var extType in TypeCache.GetTypesWithAttribute<OpenXRFeatureAttribute>())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(extType))
                {
                    if (attr is OpenXRFeatureAttribute)
                    {
                        var extAttr = (OpenXRFeatureAttribute)attr;
                        if (extAttr.BuildTargetGroups != null && !((IList)extAttr.BuildTargetGroups).Contains(group))
                            continue;

                        if (!currentExts.TryGetValue(extAttr, out var extObj))
                        {
                            // Create a new one
                            extObj = (OpenXRFeature)ScriptableObject.CreateInstance(extType);
                            extObj.name = extType.Name + " " + group;
                            AssetDatabase.AddObjectToAsset(extObj, openXrSettings);
                            AssetDatabase.SaveAssets();
                        }
                        else
                        {
                            extObj.name = extType.Name + " " + group;
                        }

                        if (extObj == null)
                            continue;

                        bool enabled = (extObj.enabled);
                        var ms = MonoScript.FromScriptableObject(extObj);
                        var path = AssetDatabase.GetAssetPath(ms);

                        var dir = "";
                        if (!String.IsNullOrEmpty(path))
                            dir = Path.GetDirectoryName(path);
                        var featureInfo = new FeatureInfo()
                        {
                            PluginPath = dir,
                            Attribute = extAttr,
                            Feature = extObj,
                            Category = DetermineExtensionCategory(extAttr.Category)
                        };

                        if (extAttr.CustomRuntimeLoaderBuildTargets?.Length > 0)
                        {
                            featureInfo.HasLoaderForBuildTarget = extAttr.CustomRuntimeLoaderBuildTargets
                                .Select(target => BuildPipeline.GetBuildTargetGroup(target))
                                .Where(targetGroup => targetGroup == group)
                                .Any();

                            if (featureInfo.HasLoaderForBuildTarget)
                            {
                                featureInfo.LoaderVersion =
                                    OpenXRApiVersion.TryParse(extAttr.CustomRuntimeLoaderVersion, out var version) ? version : null;
                            }
                        }
                        ret.Features.Add(featureInfo);

                        all.Add(extObj);
                        break;
                    }
                }
            }

            // Update the feature list
            var originalFeatures = openXrSettings.features;
            var newFeatures = all
                .Where(f => f != null)
                .OrderByDescending(f => f.priority)
                .ThenBy(f => f.nameUi)
                .ToArray();

            // Populate the internal feature variables for all features
            bool fieldChanged = false;
            foreach (var feature in newFeatures)
            {
                if (feature.internalFieldsUpdated)
                    continue;

                feature.internalFieldsUpdated = true;

                foreach (var attr in feature.GetType().GetCustomAttributes<OpenXRFeatureAttribute>())
                {
                    foreach (var sourceField in attr.GetType().GetFields())
                    {
                        var copyField = sourceField.GetCustomAttribute<OpenXRFeatureAttribute.CopyFieldAttribute>();
                        if (copyField == null)
                            continue;

                        var targetField = feature.GetType().GetField(copyField.FieldName,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        if (targetField == null)
                            continue;

                        // Only set value if value is different
                        if ((targetField.GetValue(feature) == null && sourceField.GetValue(attr) != null) ||
                            targetField.GetValue(feature) == null || targetField.GetValue(feature).Equals(sourceField.GetValue(attr)) == false)
                        {
                            targetField.SetValue(feature, sourceField.GetValue(attr));
                            fieldChanged = true;
                        }

                    }
                }
            }

            // Ensure the settings are saved after the features are populated
            if (fieldChanged || originalFeatures == null || originalFeatures.SequenceEqual(newFeatures) == false)
            {
                openXrSettings.features = newFeatures;
#if UNITY_EDITOR
                EditorUtility.SetDirty(openXrSettings);
#endif
            }

            // Decide which loader to use and API version to request
            if (TryFindCustomLoaderWithHighestPriority(ret.Features, out var customLoader))
            {
                ret.ActiveCustomLoaderFeature = customLoader;
            }

            // Save all changes to the openXrSettings at the end.
            if (EditorUtility.IsDirty(openXrSettings))
            {
                AssetDatabase.SaveAssetIfDirty(openXrSettings);
            }

            return ret;
        }

        internal static bool TryFindCustomLoaderWithHighestPriority(IEnumerable<FeatureInfo> features, out FeatureInfo loaderFeatureInfo)
        {
            var activeCustomLoaderFeatures = features
                .Where(feature => feature.Feature.enabled && feature.HasLoaderForBuildTarget);

            if (!activeCustomLoaderFeatures.Any())
            {
                loaderFeatureInfo = default;
                return false;
            }

            if (TryGetForcedLoaderOverride(activeCustomLoaderFeatures, out var loader))
            {
                // Using the forced custom loader override, when a feature doesn't specify an API version
                Debug.Log($"Using forced custom loader override provided by the OpenXR Feature {loader.Feature.nameUi}, with version {loader.LoaderVersion}");
                loaderFeatureInfo = loader;
                return true;
            }

            // Find loader with highest version
            var loaderFeatureWithHighestApiVersion = activeCustomLoaderFeatures
                .Where(feature => feature.LoaderVersion > OpenXRApiVersion.Current); // Ignore custom loaders with lower version than default loader

            if (loaderFeatureWithHighestApiVersion.Any())
            {
                // Pick loader with highest version and highest Feature order priority
                loaderFeatureInfo = loaderFeatureWithHighestApiVersion
                    .GroupBy(feature => feature.LoaderVersion)
                    .OrderByDescending(group => group.First().LoaderVersion)
                    .First()
                    .OrderByDescending(feature => feature.Attribute.Priority)
                    .First();
                return true;
            }

            // Return default loader
            loaderFeatureInfo = default;
            return false;
        }

        private static bool TryGetForcedLoaderOverride(
            IEnumerable<FeatureInfo> customLoaderFeatures,
            out FeatureInfo overrideLoaderFeature)
        {
            var overrideLoaderFeatures = customLoaderFeatures
                .Where(feature => feature.LoaderVersion == null);
            if (overrideLoaderFeatures.Count() > 1)
            {
                Debug.LogError(
                    "Only one OpenXR feature may force a custom runtime loader override per platform." +
                    "Verify that only one of the following extensions doesn't specify a custom loader OpenXR API version:" +
                    $"{string.Join(",", overrideLoaderFeatures.Select(features => features.Attribute.UiName))}.");
            }

            if (overrideLoaderFeatures.Any())
            {
                overrideLoaderFeature = overrideLoaderFeatures.First();
                return true;
            }

            overrideLoaderFeature = default;
            return false;
        }

    }
}
