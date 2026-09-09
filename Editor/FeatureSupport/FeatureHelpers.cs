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
using Object = UnityEngine.Object;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

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
            FeatureHelpersInternal.RefreshAllFeatureInfo(group);
        }

        /// <summary>
        /// Given a feature id, returns the first instance of <see cref="OpenXRFeature" /> associated with that id.
        /// </summary>
        /// <param name="featureId">The unique id identifying the feature</param>
        /// <returns>The instance of the feature matching thd id, or null.</returns>
        public static OpenXRFeature GetFeatureWithIdForActiveBuildTarget(string featureId)
        {
            return GetFeatureWithIdForBuildTarget(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), featureId);
        }

        /// <summary>
        /// Given an array of feature ids, returns an array of matching <see cref="OpenXRFeature" /> instances.
        /// </summary>
        /// <param name="featureIds">Array of feature ids to match against.</param>
        /// <returns>An array of all matching features.</returns>
        public static OpenXRFeature[] GetFeaturesWithIdsForActiveBuildTarget(string[] featureIds)
        {
            return GetFeaturesWithIdsForBuildTarget(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), featureIds);
        }

        /// <summary>
        /// Given a feature id, returns the first <see cref="OpenXRFeature" /> associated with that id.
        /// </summary>
        /// <param name="buildTargetGroup">The build target group to get the feature from.</param>
        /// <param name="featureId">The unique id identifying the feature</param>
        /// <returns>The instance of the feature matching thd id, or null.</returns>
        public static OpenXRFeature GetFeatureWithIdForBuildTarget(BuildTargetGroup buildTargetGroup, string featureId)
        {
            if (string.IsNullOrEmpty(featureId))
                return null;

            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
            if (settings == null || settings.features == null)
                return null;

            foreach (var feature in settings.features)
            {
                if (feature == null)
                    continue;

                if (string.Compare(featureId, feature.featureIdInternal, true) == 0)
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

    static class FeatureHelpersInternal
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
            public string CustomLoaderName;
        }

        static FeatureInfoCategory DetermineFeatureCategory(string featureCategoryString)
        {
            return string.Compare(featureCategoryString, FeatureCategory.Interaction) == 0
                ? FeatureInfoCategory.Interaction
                : FeatureInfoCategory.Feature;
        }

        static FeatureInfo GetFeatureInfo(OpenXRFeature openXRFeature, BuildTargetGroup group)
        {
            var ms = MonoScript.FromScriptableObject(openXRFeature);
            var path = AssetDatabase.GetAssetPath(ms);
            var dir = "";
            if (!string.IsNullOrEmpty(path))
                dir = Path.GetDirectoryName(path);

            OpenXRFeatureAttribute featureAttr = GetOpenXRFeatureAttribute(openXRFeature.GetType());
            bool hasLoaderForBuildTarget = featureAttr.CustomRuntimeLoaderBuildTargets?.Length > 0
                                                && featureAttr.CustomRuntimeLoaderBuildTargets
                                                .Any(target => BuildPipeline.GetBuildTargetGroup(target) == group);

            FeatureInfo featureInfo = new FeatureInfo
            {
                PluginPath = dir,
                Attribute = featureAttr,
                HasLoaderForBuildTarget = hasLoaderForBuildTarget,
                LoaderVersion = hasLoaderForBuildTarget ? OpenXRApiVersion.TryParse(featureAttr.CustomRuntimeLoaderVersion, out var version)
                                    ? version
                                    : null
                                    : null,
                CustomLoaderName = hasLoaderForBuildTarget ? featureAttr.CustomRuntimeLoaderName : null,

                Feature = openXRFeature,
                Category = DetermineFeatureCategory(featureAttr.Category)
            };
            return featureInfo;
        }

        public static AllFeatureInfo GetAllFeatureInfo(BuildTargetGroup group)
        {
            AllFeatureInfo ret = new()
            {
                Features = new List<FeatureInfo>(),
                ActiveCustomLoaderFeature = null
            };

            // Initialize the FeatureInfo from the OpenXRPackageSettings Object
            OpenXRPackageSettings openXrPackageSettings = OpenXRPackageSettings.Instance;
            if (openXrPackageSettings == null)
                return ret;

            OpenXRSettings openXrSettings = openXrPackageSettings.GetSettingsForBuildTargetGroup(group);
            if (openXrSettings == null)
                return ret;

            bool isOpenXrSettingsAMockInstance = ((IPackageSettings2)openXrPackageSettings).IsSettingsLocatorFuncOverriden();
            IEnumerable<OpenXRFeature> openXRFeatures = openXrSettings.features ?? Array.Empty<OpenXRFeature>();

            foreach (OpenXRFeature openXRFeature in openXRFeatures)
            {
                if (openXRFeature == null)
                    continue;
                OpenXRFeatureAttribute featureAttr = GetOpenXRFeatureAttribute(openXRFeature.GetType());
                if (featureAttr == null)
                    continue;
                if (featureAttr.BuildTargetGroups != null && !featureAttr.BuildTargetGroups.Contains(group))
                    continue;
                if (isOpenXrSettingsAMockInstance && !openXRFeature.name.Contains("MockRuntime"))
                    continue;

                ret.Features.Add(GetFeatureInfo(openXRFeature, group));
            }

            // Initialize the FeatureInfo's Custom Loader if applicable
            if (TryFindCustomLoaderWithHighestPriority(ret.Features, out var customLoader))
                ret.ActiveCustomLoaderFeature = customLoader;

            return ret;
        }

#if LIFECYCLE_APIS_AVAILABLE
        // Reflection cache; results never change at runtime.
        [NoAutoStaticsCleanup]
#endif
        static Dictionary<Type, OpenXRFeatureAttribute> featureAssetsMap = new();
        static OpenXRFeatureAttribute GetOpenXRFeatureAttribute(Type featureType)
        {
            return featureAssetsMap.TryGetValue(featureType, out var attr) ? attr : featureType.GetCustomAttribute<OpenXRFeatureAttribute>(true);
        }

        public static void RefreshAllFeatureInfo(BuildTargetGroup group)
        {
            var openXrPackageSettings = OpenXRPackageSettings.GetOrCreateInstance();
            if (openXrPackageSettings == null)
                return;
            var openXrSettings = openXrPackageSettings.GetSettingsForBuildTargetGroup(group);
            if (openXrSettings == null)
                return;

            bool isOpenXrSettingsAMockInstance = ((IPackageSettings2)openXrPackageSettings).IsSettingsLocatorFuncOverriden();
            string buildGroupName = isOpenXrSettingsAMockInstance ? "MockRuntime" : group.ToString();

            List<OpenXRFeature> allOpenXRFeatures = new();
            foreach(var feature in openXrSettings.features)
            {
                // A feature can be loaded as null because its class has been removed from the project
                if (feature != null)
                    allOpenXRFeatures.Add(feature);
            }
            featureAssetsMap.Clear();

            // Iterate through all types that have the OpenXRFeatureAttribute, and create ScriptableObjects for the ones that are valid for the current BuildTargetGroup
            // and not already serialized within the OpenXRSettings found on disk
            foreach (Type featureType in TypeCache.GetTypesWithAttribute<OpenXRFeatureAttribute>())
            {
                OpenXRFeatureAttribute featureAttribute = GetOpenXRFeatureAttribute(featureType);
                // Make sure that features of this type are valid for the current build target group
                if (featureAttribute != null && featureAttribute.BuildTargetGroups != null && !featureAttribute.BuildTargetGroups.Contains(group))
                    continue;

                // If the type is not currently in the settings found on Disk, create a ScriptableObject for it.
                if (!allOpenXRFeatures.Any(x => x.GetType().Equals(featureType)))
                {
                    // Create a new one
                    var featureAsset = (OpenXRFeature)ScriptableObject.CreateInstance(featureType);
                    featureAsset.name = featureType.Name + " " + buildGroupName;

                    allOpenXRFeatures.Add(featureAsset);

                    AssetDatabase.AddObjectToAsset(featureAsset, openXrSettings);
                    featureAssetsMap[featureAsset.GetType()] = featureAttribute;
                }
            }

#if UNITY_EDITOR
            // Filter out the openXRFeatures which no longer have a OpenXRFeatureAttribute and remove them from
            // the Settings asset.
            for (int i = allOpenXRFeatures.Count - 1; i >= 0; i--)
            {
                OpenXRFeature feature = allOpenXRFeatures[i];
                var type = feature.GetType();
                var attr = type.GetCustomAttribute<OpenXRFeatureAttribute>();
                if (attr == null)
                {
                    AssetDatabase.RemoveObjectFromAsset(feature);
                    allOpenXRFeatures.RemoveAt(i);
                }
            }
#endif

            foreach (var feature in allOpenXRFeatures)
            {
                if (feature.internalFieldsUpdated)
                    continue;

                feature.internalFieldsUpdated = true;
                OpenXRFeatureAttribute featureAttribute = feature.GetType().GetCustomAttribute<OpenXRFeatureAttribute>();
                if (featureAttribute == null)
                    continue;

                foreach (var sourceField in featureAttribute.GetType().GetFields())
                {
                    var copyField = sourceField.GetCustomAttribute<OpenXRFeatureAttribute.CopyFieldAttribute>();
                    if (copyField == null)
                        continue;

                    var targetField = feature.GetType().GetField(
                        copyField.FieldName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (targetField == null)
                        continue;

                    // Often, when the instance fields are strings, either the target or source values may have it
                    // assigned as null or empty string "".
                    // In this case, we default to an empty string, and only if the field is of string type.
                    // Otherwise, we may not be able to get accurate comparisons.
                    var targetFieldValue = targetField.GetValueOrTypeDefault(feature, string.Empty);
                    var sourceFieldValue = sourceField.GetValueOrTypeDefault(featureAttribute, string.Empty);

                    // Only set value if value is different
                    if (targetFieldValue == null
                        || !targetFieldValue.Equals(sourceFieldValue))
                    {
                        targetField.SetValue(feature, sourceFieldValue);
                    }
                }
            }

            var updatedOpenXRFeatures = allOpenXRFeatures.ToArray();
            if (!HasSameFeatureReferences(openXrSettings.features, updatedOpenXRFeatures))
            {
                openXrSettings.features = updatedOpenXRFeatures;
                EditorUtility.SetDirty(openXrSettings);
            }

            List<FeatureInfo> allFeatureInfo = allOpenXRFeatures.Select(x => GetFeatureInfo(x, group)).ToList();
            if (TryFindCustomLoaderWithHighestPriority(allFeatureInfo, out var customLoaderFeatureInfo) && !string.IsNullOrWhiteSpace(customLoaderFeatureInfo.CustomLoaderName))
            {
                openXrSettings.customLoaderName = customLoaderFeatureInfo.CustomLoaderName;
                EditorUtility.SetDirty(openXrSettings);
            }
            else if (!string.IsNullOrEmpty(openXrSettings.customLoaderName))
            {
                openXrSettings.customLoaderName = string.Empty;
                EditorUtility.SetDirty(openXrSettings);
            }
            if (EditorUtility.IsDirty(openXrSettings))
                AssetDatabase.SaveAssetIfDirty(openXrSettings);

            AssetDatabase.SaveAssets();
        }

        static bool HasSameFeatureReferences(OpenXRFeature[] currentFeatures, OpenXRFeature[] updatedFeatures)
        {
            if (currentFeatures == null || currentFeatures.Length != updatedFeatures.Length)
                return false;

            for (var i = 0; i < currentFeatures.Length; i++)
            {
                if (!ReferenceEquals(currentFeatures[i], updatedFeatures[i]))
                    return false;
            }

            return true;
        }

        static IEnumerable<Object> GetPackageSettingsFeatureAssets(OpenXRPackageSettings openXrPackageSettings)
        {
            var assetPath = Path.Combine(OpenXRPackageSettings.GetAssetPathForComponents(
                OpenXRPackageSettings.s_PackageSettingsDefaultSettingsPath), openXrPackageSettings.name + ".asset");
            var featureAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            if (featureAssets == null || featureAssets.Length == 0)
            {
                string[] guids = AssetDatabase.FindAssets("t:OpenXRSettings");

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var packageSettingsAssets = AssetDatabase.LoadAllAssetsAtPath(path);

                    if (packageSettingsAssets.Any(obj => obj != null && obj.name == openXrPackageSettings.name))
                    {
                        return packageSettingsAssets;
                    }
                }
            }

            return featureAssets;
        }

        internal static bool TryFindCustomLoaderWithHighestPriority(
            IEnumerable<FeatureInfo> features, out FeatureInfo loaderFeatureInfo)
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

        static bool TryGetForcedLoaderOverride(
            IEnumerable<FeatureInfo> customLoaderFeatures, out FeatureInfo overrideLoaderFeature)
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

        /// <summary>
        /// Returns the field value from the instance object.
        ///
        /// If the field value is null, and the field type is the same as the default value,
        /// then it returns the default value instead.
        /// </summary>
        /// <typeparam name="T">Type of the default value to match.</typeparam>
        /// <param name="field">Field from which retrieve the value.</param>
        /// <param name="instance">Object from which to retrieve the field value.</param>
        /// <param name="fieldDefaultValue">Default value to return, only if the field is of the same type.</param>
        /// <returns>Value of the field in the instance object, or default value only if the field type matches its type.</returns>
        static object GetValueOrTypeDefault<T>(this FieldInfo field, object instance, T fieldDefaultValue)
        {
            return field.FieldType == typeof(T) ?
                field.GetValue(instance) ?? fieldDefaultValue :
                field.GetValue(instance);
        }
    }
}
