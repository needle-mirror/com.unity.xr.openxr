#if ENABLE_CODE_SAMPLE_COMPILATION
#region AdditiveFeatureSample
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Input;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR.CodeSamples.Editor.Tests
{
#if UNITY_EDITOR
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(
        UiName = "Example User Controller Profile",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android },
        Company = "Unity",
        Desc = "An example to demonstrate how to create an additive profile.",
        Version = "0.0.1",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Feature,
        FeatureId = "com.unity.openxr.feature.input.user.additive")]
#endif
    /// <summary>
    /// Example feature showing how additive profiles are used
    /// </summary>
    public class AdditiveFeatureSample : OpenXRInteractionFeature
    {
        /// <summary>
        /// Name of the device layout associated with this mock feature.
        /// </summary>
        /// <returns> Returns string with device layout name.</returns>
        protected override string GetDeviceLayoutName() => "Sample Layout Name";

        /// <summary>
        /// The path representing the sample thumb pose action.
        /// </summary>
        public const string sampleThumbPosePath = "/input/thumb_ext/pose";

        /// <summary>
        /// The additive action map configuration for this sample feature.
        /// </summary>
        protected ActionMapConfig m_AdditiveMap { get; set; }

        /// <summary>
        /// A flag to mark this feature as additive.
        /// </summary>
        protected override bool IsAdditive => true;

        /// <summary>
        /// Registers thumb pose targeting the enabled profiles.
        /// </summary>
        protected override void RegisterActionMapsWithRuntime()
        {
            m_AdditiveMap = new ActionMapConfig
            {
                name = "sample_user_additive_map",
                localizedName = "Sample User Additive Map",
                desiredInteractionProfile = "/interaction_profiles/sample/controller",

                deviceInfos = new List<DeviceConfig>
                {
                    new DeviceConfig
                    {
                        characteristics = InputDeviceCharacteristics.TrackedDevice |
                                          InputDeviceCharacteristics.Controller |
                                          InputDeviceCharacteristics.Left,
                        userPath = UserPaths.leftHand
                    },
                    new DeviceConfig
                    {
                        characteristics = InputDeviceCharacteristics.TrackedDevice |
                                          InputDeviceCharacteristics.Controller |
                                          InputDeviceCharacteristics.Right,
                        userPath = UserPaths.rightHand
                    }
                },

                actions = new List<ActionConfig>
                {
                    new ActionConfig
                    {
                        name = "thumbpose",
                        localizedName = "Thumb Pose",
                        type = ActionType.Pose,
                        isAdditive = true,
                        bindings = new List<ActionBinding>
                        {
                            new ActionBinding
                            {
                                interactionPath = sampleThumbPosePath,
                                interactionProfileName = "/interaction_profiles/sample/controller"
                            }
                        }
                    }
                }
            };
            AddActionMap(m_AdditiveMap);
        }

        /// <summary>
        /// Adds sample binding actions to enabled left/right hand controllers that include a thumbpose
        /// </summary>
        /// <param name="actionMaps">The set of action maps from all enabled non-additive interaction profiles that can be augmented.</param>
        /// <param name="additiveMap">Sample feature's additive map containg the extra sample binding to append.</param>
        protected override void AddAdditiveActions(List<ActionMapConfig> actionMaps, ActionMapConfig additiveMap)
        {
            foreach (var actionMap in actionMaps)
            {
                var validUserPath = actionMap.deviceInfos.Where(d => d.userPath != null && ((String.CompareOrdinal(d.userPath, OpenXRInteractionFeature.UserPaths.leftHand) == 0) ||
                    (String.CompareOrdinal(d.userPath, OpenXRInteractionFeature.UserPaths.rightHand) == 0)));
                if (!validUserPath.Any())
                    continue;

                foreach (var additiveAction in additiveMap.actions.Where(a => a.isAdditive))
                {
                    actionMap.actions.Add(additiveAction);
                }
            }
        }
    }
}
#endregion
#endif
// Used in Features/OpenXRInteractionFeature.cs
// This example demonstrates how to use the AddAdditiveActions API to augment an existing interaction profile.
