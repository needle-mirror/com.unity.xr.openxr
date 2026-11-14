using System.Collections.Generic;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR.Tests
{
#if UNITY_EDITOR
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(
        UiName = "Single Host Additive Test Feature",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android },
        Company = "Unity",
        Desc = "Additive interaction feature that declares the hand interaction profile as its only host.",
        Version = "0.0.1",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction,
        Hidden = true,
        FeatureId = featureId)]
#endif
    class SingleHostAdditiveTestFeature : OpenXRInteractionFeature
    {
        public const string featureId = "com.unity.openxr.feature.input.test.singlehostadditive";

        public const string profile = "/interaction_profiles/unity/single_host_additive_test";

        public const string localizedDeviceName = "Single Host Additive Test OpenXR";

        const string k_ReadyPath = "/input/pinch_ext/ready_ext";

        protected internal override bool IsAdditive => true;

        internal static ulong ConvertStringToPath(string path) => StringToPath(path);

        protected override string GetDeviceLayoutName() => "Single Host Additive Test Layout";

        protected override void RegisterActionMapsWithRuntime()
        {
            var actionMap = new ActionMapConfig
            {
                name = "singlehostadditivetest",
                localizedName = localizedDeviceName,
                desiredInteractionProfile = profile,
                manufacturer = "Unity",
                parentInteractionProfiles = new List<string> { HandInteractionProfile.profile },
                deviceInfos = new List<DeviceConfig>
                {
                    new DeviceConfig
                    {
                        characteristics = InputDeviceCharacteristics.HandTracking |
                                          InputDeviceCharacteristics.TrackedDevice |
                                          InputDeviceCharacteristics.Left,
                        userPath = UserPaths.leftHand
                    },
                    new DeviceConfig
                    {
                        characteristics = InputDeviceCharacteristics.HandTracking |
                                          InputDeviceCharacteristics.TrackedDevice |
                                          InputDeviceCharacteristics.Right,
                        userPath = UserPaths.rightHand
                    }
                },
                actions = new List<ActionConfig>
                {
                    new ActionConfig
                    {
                        name = "singlehostready",
                        localizedName = "Single Host Ready",
                        type = ActionType.Binary,
                        isAdditive = true,
                        bindings = new List<ActionBinding>
                        {
                            new ActionBinding
                            {
                                interactionPath = k_ReadyPath,
                                interactionProfileName = HandInteractionProfile.profile
                            }
                        }
                    }
                }
            };

            AddActionMap(actionMap);
        }

        protected internal override void AddAdditiveActions(List<ActionMapConfig> actionMaps, ActionMapConfig additiveMap)
        {
            for (var mapIndex = 0; mapIndex < actionMaps.Count; ++mapIndex)
            {
                var actionMap = actionMaps[mapIndex];
                if (actionMap == null || string.CompareOrdinal(actionMap.desiredInteractionProfile, HandInteractionProfile.profile) != 0)
                    continue;

                for (var actionIndex = 0; actionIndex < additiveMap.actions.Count; ++actionIndex)
                {
                    var action = additiveMap.actions[actionIndex];
                    if (action.isAdditive)
                        actionMap.actions.Add(action);
                }
            }
        }
    }
}
