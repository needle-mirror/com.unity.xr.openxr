using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEngine.XR.OpenXR;
#if XR_COMPOSITION_LAYERS
using UnityEngine.XR.OpenXR.Features.CompositionLayers;
#endif
using UnityEngine.XR.OpenXR.Features;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;


[assembly: InternalsVisibleTo("UnityEditor.XR.OpenXR.Tests")]
namespace UnityEditor.XR.OpenXR
{
    internal class OpenXRProjectValidationRulesSetup
    {
        static BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static BuildTargetGroup s_SelectedBuildTargetGroup = BuildTargetGroup.Unknown;

        internal const string OpenXRProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        [InitializeOnLoadMethod]
        static void OpenXRProjectValidationCheck()
        {
            UnityEditor.PackageManager.Events.registeredPackages += (packageRegistrationEventArgs) =>
            {
                // In the Player Settings UI we have to delay the call one frame to let OpenXRSettings constructor to get initialized
                EditorApplication.delayCall += () =>
                {
                    if (HasXRPackageVersionChanged(packageRegistrationEventArgs))
                    {
                        ShowWindowIfIssuesExist();
                    }
                };
            };

            AddOpenXRValidationRules();
        }

        private static bool HasXRPackageVersionChanged(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            bool packageChanged = packageRegistrationEventArgs.changedTo.Any(p => p.name.Equals(OpenXRManagementSettings.PackageId));
            OpenXRSettings.Instance.versionChanged = packageChanged;
            return packageRegistrationEventArgs.added.Any(p => p.name.Equals(OpenXRManagementSettings.PackageId)) || packageChanged;
        }

        private static void ShowWindowIfIssuesExist()
        {
            List<OpenXRFeature.ValidationRule> failures = new List<OpenXRFeature.ValidationRule>();
            BuildTargetGroup activeBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            OpenXRProjectValidation.GetCurrentValidationIssues(failures, activeBuildTargetGroup);

            if (failures.Count > 0)
                ShowWindow();
        }

        internal static BuildValidationRule ConvertRuleToBuildValidationRule(OpenXRFeature.ValidationRule rule, BuildTargetGroup buildTargetGroup)
        {
            Type featureType = null;
            if (rule.feature != null)
            {
                featureType = rule.feature.GetType();
            }

            return new BuildValidationRule
            {
                // This will hide the rules given a condition so that when you click "Show all" it doesn't show up as passed
                IsRuleEnabled = () =>
                {
                    // If OpenXR isn't enabled, no need to show the rule
                    if (!BuildHelperUtils.HasLoader(buildTargetGroup, typeof(OpenXRLoaderBase)))
                        return false;

                    // If a  rule specific feature isn't enabled, don't show this rule
                    if (featureType != null)
                    {
                        OpenXRSettings openXrSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
                        if (openXrSettings == null)
                            return false;

                        OpenXRFeature feature = openXrSettings.GetFeature(featureType);
                        if (feature == null || !feature.enabled)
                            return false;
                    }

                    return true;
                },
                CheckPredicate = rule.checkPredicate,
                Error = rule.error,
                FixIt = rule.fixIt,
                FixItAutomatic = rule.fixItAutomatic,
                FixItMessage = rule.fixItMessage,
                HelpLink = rule.helpLink,
                HelpText = rule.helpText,
                Message = rule.message,
                Category = rule.feature != null ? rule.feature.nameUi : "OpenXR",
                SceneOnlyValidation = false
            };
        }

        static void AddOpenXRValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                bool isOpenXRSupportedPlatform = buildTargetGroup == BuildTargetGroup.Standalone || buildTargetGroup == BuildTargetGroup.Android || buildTargetGroup == BuildTargetGroup.WSA;
                if (!isOpenXRSupportedPlatform)
                    continue;

                var coreIssues = new List<BuildValidationRule>() { GetDefaultBuildValidationRule(buildTargetGroup) };
                var issues = new List<OpenXRFeature.ValidationRule>();
                OpenXRProjectValidation.GetAllValidationIssues(issues, buildTargetGroup);

                foreach (var issue in issues)
                {
                    coreIssues.Add(ConvertRuleToBuildValidationRule(issue, buildTargetGroup));
                }

                BuildValidator.AddRules(buildTargetGroup, coreIssues);
            }
        }

        static BuildValidationRule GetDefaultBuildValidationRule(BuildTargetGroup targetGroup)
        {
            var defaultRule = new BuildValidationRule()
            {
                // This will hide the rules given a condition so that when you click "Show all" it doesn't show up as passed
                IsRuleEnabled = () =>
                {
                    // Only show this rule if there are enabled OpenXR features that aren't interaction profiles
                    return ExistsOpenXRFeaturesEnabledForBuildTarget(targetGroup);
                },
                Message = "[OpenXR] Enabled OpenXR Features require OpenXR to be selected as the active loader for this platform",
                CheckPredicate = () => BuildHelperUtils.HasLoader(targetGroup, typeof(OpenXRLoaderBase)),
                Error = false,
                FixIt = () => { SettingsService.OpenProjectSettings("Project/XR Plug-in Management"); },
                FixItAutomatic = false,
                FixItMessage = "Open Project Settings to select OpenXR as the active loader for this platform."
            };

            return defaultRule;
        }

        /// <summary>
        /// Check if there are any features enabled for this build target
        /// </summary>
        static bool ExistsOpenXRFeaturesEnabledForBuildTarget(BuildTargetGroup buildTargetGroup)
        {
            var openXrSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
            foreach (var feature in openXrSettings?.features)
            {
                if (feature != null && feature.enabled && !(feature is OpenXRInteractionFeature interactionFeature && !interactionFeature.IsAdditive))
                {
                    return true;
                }
            }
            return false;
        }

        [MenuItem("Window/XR/OpenXR/Project Validation")]
        private static void MenuItem()
        {
            ShowWindow();
        }

        internal static void SetSelectedBuildTargetGroup(BuildTargetGroup buildTargetGroup)
        {
            if (s_SelectedBuildTargetGroup == buildTargetGroup)
                return;

            s_SelectedBuildTargetGroup = buildTargetGroup;
        }

        internal static void ShowWindow(BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown)
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified
            EditorApplication.delayCall += () =>
            {
                SetSelectedBuildTargetGroup(buildTargetGroup);
                SettingsService.OpenProjectSettings(OpenXRProjectValidationSettingsPath);
            };
        }

        internal static void CloseWindow() { }
    }

    internal class OpenXRProjectValidationBuildStep : IPreprocessBuildWithReport
    {
        [OnOpenAsset(0)]
        static bool ConsoleErrorDoubleClicked(int instanceId, int line)
        {
            var objName = EditorUtility.InstanceIDToObject(instanceId).name;
            if (objName == "OpenXRProjectValidation")
            {
                OpenXRProjectValidationRulesSetup.ShowWindow();
                return true;
            }

            return false;
        }

        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!BuildHelperUtils.HasLoader(report.summary.platformGroup, typeof(OpenXRLoaderBase)))
                return;

            if (OpenXRProjectValidation.LogBuildValidationIssues(report.summary.platformGroup))
                throw new BuildFailedException("OpenXR Build Failed.");
        }
    }
}
