using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Input;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR.Tests
{
#if UNITY_EDITOR
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(
        UiName = "Mock Meta Controller",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android },
        Company = "MockUnity",
        Desc = "Adds a mock additive pose/button to a new controller profile.",
        Version = "0.0.1",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Feature,
        Hidden = true,
        FeatureId = "com.unity.openxr.feature.input.partner.additive")]
#endif
    /// <summary>
    /// Mock feature used for testing additive OpenXR interaction profiles.
    /// </summary>
    public class MockAdditiveFeature : OpenXRInteractionFeature
    {
        /// <summary>
        /// Stores merged additive actions for all features (used for testing).
        /// </summary>
        public static readonly List<(string profile, List<(string name, string type, List<string> bindPaths, bool isAdditive)>)> MergeDetails = new();

        /// <summary>
        /// Name of the device layout associated with this mock feature.
        /// </summary>
        /// <returns> Returns string with device layout name.</returns>
        protected override string GetDeviceLayoutName() => "Mock Meta Layout";

        /// <summary>
        /// The target interaction profile name.
        /// </summary>
        public const string targetProfile = "/interaction_profiles/mockmeta/controller";

        /// <summary>
        /// The OpenXR path representing the mock thumb pose action.
        /// </summary>
        public const string mockThumbPosePath = "/input/thumb_ext/pose";

        /// <summary>
        /// The OpenXR path representing the mock auxiliary button action.
        /// </summary>
        public const string mockAuxButtonPath = "/input/aux_ext/click";

        /// <summary>
        /// The additive action map configuration for this mock feature.
        /// </summary>
        protected internal ActionMapConfig m_AdditiveMap { get; set; }

        /// <summary>
        /// A flag to mark this feature as additive.
        /// </summary>
        protected internal override bool IsAdditive => true;

        /// <summary>
        /// Registers thumb pose and aux click.
        /// </summary>
        protected override void RegisterActionMapsWithRuntime()
        {
            m_AdditiveMap = new ActionMapConfig
            {
                name = "mock_partner_additive_map",
                localizedName = "Mock Partner Additive",
                desiredInteractionProfile = targetProfile,
                manufacturer = "Mock Partner",
                serialNumber = "MockAdditive01",

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
                                interactionPath = mockThumbPosePath,
                                interactionProfileName = targetProfile
                            }
                        }
                    },
                    new ActionConfig
                    {
                        name = "auxclick",
                        localizedName = "Aux Click",
                        type = ActionType.Binary,
                        isAdditive = true,
                        bindings = new List<ActionBinding>
                        {
                            new ActionBinding
                            {
                                interactionPath = mockAuxButtonPath,
                                interactionProfileName = targetProfile
                            }
                        }
                    }
                }
            };
            AddActionMap(m_AdditiveMap);
        }


        /// <summary>
        /// Processes additive actions to add to an existing controller. This function has to copy values to merge details in order to see if actions were merged correctly.
        /// </summary>
        /// <param name="actionMaps">Action maps to extend.</param>
        /// <param name="additiveMap">The additive map to merge.</param>
        protected internal override void AddAdditiveActions(List<ActionMapConfig> actionMaps, ActionMapConfig additiveMap)
        {
            if (additiveMap == null || actionMaps == null)
                return;

            // Build payload of additive actions once
            var additivePayload = new List<(string name, string type, List<string> bindPaths, bool isAdditive)>();
            if (additiveMap.actions != null)
            {
                for (int i = 0; i < additiveMap.actions.Count; i++)
                {
                    var a = additiveMap.actions[i];
                    if (a == null || !a.isAdditive)
                        continue;

                    var bindPaths = new List<string>();
                    if (a.bindings != null)
                    {
                        for (int j = 0; j < a.bindings.Count; j++)
                        {
                            var b = a.bindings[j];
                            var path = (b != null && b.interactionPath != null) ? b.interactionPath : string.Empty;
                            bindPaths.Add(path);
                        }
                    }

                    additivePayload.Add((
                        name: a.name,
                        type: a.type.ToString(),
                        bindPaths: bindPaths,
                        isAdditive: a.isAdditive
                    ));
                }
            }

            if (additivePayload.Count == 0)
                return;

            // Merge into target maps (left/right hands) and record under those profiles
            for (int m = 0; m < actionMaps.Count; m++)
            {
                var map = actionMaps[m];
                if (map == null)
                    continue;

                // Only merge into maps that represent left/right hands
                bool hasValidHand = false;
                if (map.deviceInfos != null)
                {
                    for (int i = 0; i < map.deviceInfos.Count; i++)
                    {
                        var d = map.deviceInfos[i];
                        if (d?.userPath != null &&
                            (String.CompareOrdinal(d.userPath, UserPaths.leftHand) == 0 ||
                            String.CompareOrdinal(d.userPath, UserPaths.rightHand) == 0))
                        {
                            hasValidHand = true;
                            break;
                        }
                    }
                }
                if (!hasValidHand)
                    continue;

                // Merge additive actions
                if (map.actions == null)
                    map.actions = new List<ActionConfig>();

                if (additiveMap.actions != null)
                {
                    for (int i = 0; i < additiveMap.actions.Count; i++)
                    {
                        var add = additiveMap.actions[i];
                        if (add != null && add.isAdditive)
                            map.actions.Add(add);
                    }
                }

                // Record MergeDetails under the actual target profile key
                var profileKey = map.desiredInteractionProfile ?? string.Empty;

                // Avoid duplicates for the same profile
                bool alreadyRecorded = false;
                for (int i = 0; i < MergeDetails.Count; i++)
                {
                    if (String.CompareOrdinal(MergeDetails[i].profile, profileKey) == 0)
                    {
                        alreadyRecorded = true;
                        break;
                    }
                }

                if (!alreadyRecorded)
                {
                    // Copy payload while keeping MergeDetails immutable for assertions
                    var payloadCopy = new List<(string name, string type, List<string> bindPaths, bool isAdditive)>(additivePayload.Count);
                    for (int i = 0; i < additivePayload.Count; i++)
                    {
                        var p = additivePayload[i];
                        payloadCopy.Add((p.name, p.type, new List<string>(p.bindPaths), p.isAdditive));
                    }
                    MergeDetails.Add((profileKey, payloadCopy));
                }
            }
        }
    }
}
