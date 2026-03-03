#region BuildTimeValidationExample
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEngine.XR.OpenXR.CodeSamples.Editor.Tests
{
#if UNITY_EDITOR
    public class BuildTimeValidationExample : OpenXRFeature
    {
        protected override void GetValidationChecks(
            List<OpenXRFeature.ValidationRule> results,
            BuildTargetGroup targetGroup)
        {
            results.Add(new ValidationRule(this)
            {
                message = "At least one interaction profile must be added. Please select which controllers you will be testing against in the Features menu.",
                checkPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                    return settings == null || settings.GetFeatures<OpenXRInteractionFeature>().Any(f => f.enabled);
                },
                fixIt = () => SettingsService.OpenProjectSettings("Project/XR Plug-in Management/OpenXR"),
                fixItAutomatic = false,
                fixItMessage = "Open Project Settings to select one or more interaction profiles.",
            });
        }
    }
#endif
}
#endregion
// Used in Documentation~/features.md
// This example demonstrates how to add build time validation checks in an OpenXRFeature.
