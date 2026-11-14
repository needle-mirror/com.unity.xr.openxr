using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
    /// <summary>
    /// An additive interaction feature that binds to the microgesture inputs the `XR_META_hand_tracking_microgestures` OpenXR extension adds to the Hand Interaction Profile.
    /// </summary>
    /// <remarks>
    /// <para>This feature requires the <see cref="HandInteractionProfile"/> feature. When the runtime supports the extension, Unity creates a <see cref="HandInteractionMicrogestures"/> device for each hand, which exposes the microgesture controls.</para>
    /// <para>For more information, refer to [Hand Interaction Profile Microgestures](xref:openxr-hand-interaction-profile-microgestures).</para>
    /// </remarks>
    /// <seealso cref="HandInteractionProfile"/>
#if UNITY_EDITOR
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "Hand Interaction Profile Microgestures",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android },
        Company = "Unity",
        Desc = "Binds to the swipe and tap microgesture inputs that the XR_META_hand_tracking_microgestures extension adds to the Hand Interaction Profile. Requires the Hand Interaction Profile.",
        DocumentationLink = Constants.k_DocumentationManualURL + "features/handinteractionprofilemicrogestures.html",
        OpenxrExtensionStrings = extensionString,
        Version = "0.0.1",
        FeatureId = featureId)]
#endif
    public class HandInteractionProfileMicrogestures : OpenXRInteractionFeature
    {
        /// <summary>
        /// The unique ID of this feature.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.input.handinteractionmicrogestures";

        /// <summary>
        /// Whether this feature adds its actions to other enabled interaction profiles instead of acting as an interaction profile on its own.
        /// </summary>
        /// <remarks>
        /// Always <c>true</c>. This feature adds its actions to the Hand Interaction Profile.
        /// </remarks>
        protected internal override bool IsAdditive => true;

        /// <summary>
        /// An Input System device that exposes the microgesture controls for one hand.
        /// </summary>
        /// <remarks>
        /// Unity creates one device for each hand and assigns it the <c>LeftHand</c> or <c>RightHand</c> usage. To bind a control for one hand, use a path such as <c>&lt;HandInteractionMicrogestures&gt;{LeftHand}/tapThumb</c>.
        /// </remarks>
        [Preserve, InputControlLayout(displayName = "Hand Interaction Microgestures (OpenXR)", commonUsages = new[] { "LeftHand", "RightHand" })]
        public class HandInteractionMicrogestures : OpenXRDevice
        {
            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="HandInteractionProfileMicrogestures.swipeLeft"/> OpenXR binding.
            /// </summary>
            /// <remarks>
            /// The OpenXR input is <c>true</c> when the runtime recognizes a swipe left microgesture.
            /// </remarks>
            [Preserve, InputControl()]
            public ButtonControl swipeLeft { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="HandInteractionProfileMicrogestures.swipeRight"/> OpenXR binding.
            /// </summary>
            /// <remarks>
            /// The OpenXR input is <c>true</c> when the runtime recognizes a swipe right microgesture.
            /// </remarks>
            [Preserve, InputControl()]
            public ButtonControl swipeRight { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="HandInteractionProfileMicrogestures.swipeForward"/> OpenXR binding.
            /// </summary>
            /// <remarks>
            /// The OpenXR input is <c>true</c> when the runtime recognizes a swipe forward microgesture.
            /// </remarks>
            [Preserve, InputControl()]
            public ButtonControl swipeForward { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="HandInteractionProfileMicrogestures.swipeBackward"/> OpenXR binding.
            /// </summary>
            /// <remarks>
            /// The OpenXR input is <c>true</c> when the runtime recognizes a swipe backward microgesture.
            /// </remarks>
            [Preserve, InputControl()]
            public ButtonControl swipeBackward { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="HandInteractionProfileMicrogestures.tapThumb"/> OpenXR binding.
            /// </summary>
            /// <remarks>
            /// The OpenXR input is <c>true</c> when the runtime recognizes a thumb tap microgesture.
            /// </remarks>
            [Preserve, InputControl()]
            public ButtonControl tapThumb { get; private set; }

            /// <summary>
            /// Assigns each microgesture control property after the Input System creates the device.
            /// </summary>
            protected override void FinishSetup()
            {
                base.FinishSetup();
                swipeLeft = GetChildControl<ButtonControl>("swipeLeft");
                swipeRight = GetChildControl<ButtonControl>("swipeRight");
                swipeForward = GetChildControl<ButtonControl>("swipeForward");
                swipeBackward = GetChildControl<ButtonControl>("swipeBackward");
                tapThumb = GetChildControl<ButtonControl>("tapThumb");
            }
        }

        /// <summary>
        /// The OpenXR component path of the swipe left microgesture input.
        /// </summary>
        public const string swipeLeft = "/input/swipe_left_meta/click";

        /// <summary>
        /// The OpenXR component path of the swipe right microgesture input.
        /// </summary>
        public const string swipeRight = "/input/swipe_right_meta/click";

        /// <summary>
        /// The OpenXR component path of the swipe forward microgesture input.
        /// </summary>
        public const string swipeForward = "/input/swipe_forward_meta/click";

        /// <summary>
        /// The OpenXR component path of the swipe backward microgesture input.
        /// </summary>
        public const string swipeBackward = "/input/swipe_backward_meta/click";

        /// <summary>
        /// The OpenXR component path of the thumb tap microgesture input.
        /// </summary>
        public const string tapThumb = "/input/tap_thumb_meta/click";

        /// <summary>
        /// The OpenXR interaction profile path that the microgesture bindings use. Equal to <see cref="HandInteractionProfile.profile"/>.
        /// </summary>
        public const string profile = HandInteractionProfile.profile;

        private const string kDeviceLocalizedName = "Hand Interaction Microgestures OpenXR";

        /// <summary>
        /// The name of the OpenXR extension that this feature requires.
        /// </summary>
        public const string extensionString = "XR_META_hand_tracking_microgestures";

#if UNITY_EDITOR
        /// <summary>
        /// Adds a validation rule that requires the Hand Interaction Profile feature.
        /// </summary>
        /// <param name="results">List to add the validation rules to.</param>
        /// <param name="target">Build target group to validate.</param>
        protected internal override void GetValidationChecks(List<OpenXRFeature.ValidationRule> results, BuildTargetGroup target)
        {
            results.Add(new ValidationRule(this)
            {
                message = "Hand Interaction Profile Microgestures requires the Hand Interaction Profile. Add it to Enabled Interaction Profiles.",
                error = true,
                errorEnteringPlaymode = true,
                checkPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(target);
                    if (null == settings)
                        return false;

                    var handInteractionProfile = settings.GetFeature<HandInteractionProfile>();
                    return handInteractionProfile != null && handInteractionProfile.enabled;
                },
                fixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(target);
                    var handInteractionProfile = settings != null ? settings.GetFeature<HandInteractionProfile>() : null;
                    if (handInteractionProfile != null)
                        handInteractionProfile.enabled = true;
                },
                fixItAutomatic = true,
                fixItMessage = "Enable the Hand Interaction Profile."
            });

            base.GetValidationChecks(results, target);
        }
#endif

        /// <summary>
        /// Defers instance setup until <see cref="OnExtensionsReady"/>.
        /// </summary>
        /// <param name="instance">Handle of the OpenXR instance.</param>
        /// <returns>Always <c>true</c>.</returns>
        protected internal override bool OnInstanceCreate(ulong instance)
        {
            // no-op; we want to delay the call to base.OnInstanceCreate() until OnExtensionsReady().
            return true;
        }

        /// <summary>
        /// Registers the <see cref="HandInteractionMicrogestures"/> layout with the Input System if the runtime supports the `XR_META_hand_tracking_microgestures` extension.
        /// </summary>
        /// <param name="instance">Handle of the OpenXR instance.</param>
        /// <returns><c>true</c> if the runtime supports the extension. Otherwise, <c>false</c>, and Unity disables this feature.</returns>
        protected internal override bool OnExtensionsReady(ulong instance)
        {
            // Requires the microgesture extension
            if (!OpenXRRuntime.IsSystemExtensionEnabled(extensionString))
                return false;

            // calling OnInstanceCreate here is intentional; the operation simply happens at a slightly later time.
            return base.OnInstanceCreate(instance);
        }

        /// <summary>
        /// Registers the <see cref="HandInteractionMicrogestures"/> layout with the Input System.
        /// </summary>
        protected override void RegisterDeviceLayout()
        {
#if UNITY_EDITOR
            if (!OpenXRLoaderEnabledForSelectedBuildTarget(EditorUserBuildSettings.selectedBuildTargetGroup))
                return;
#endif
            InputSystem.InputSystem.RegisterLayout(typeof(HandInteractionMicrogestures),
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(kDeviceLocalizedName));
        }

        /// <summary>
        /// Removes the <see cref="HandInteractionMicrogestures"/> layout from the Input System.
        /// </summary>
        protected override void UnregisterDeviceLayout()
        {
#if UNITY_EDITOR
            if (!OpenXRLoaderEnabledForSelectedBuildTarget(EditorUserBuildSettings.selectedBuildTargetGroup))
                return;
#endif
            InputSystem.InputSystem.RemoveLayout(nameof(HandInteractionMicrogestures));
        }

        /// <summary>
        /// Returns <see cref="InteractionProfileType.Device"/>, because <see cref="HandInteractionMicrogestures"/> isn't an
        /// <see cref="XRController"/>.
        /// </summary>
        /// <returns>The interaction profile type.</returns>
        protected override InteractionProfileType GetInteractionProfileType() => InteractionProfileType.Device;

        /// <summary>
        /// Obtains the name of the device layout that this feature registers with the Input System.
        /// </summary>
        /// <returns>The name of the <see cref="HandInteractionMicrogestures"/> layout.</returns>
        protected override string GetDeviceLayoutName()
        {
            return nameof(HandInteractionMicrogestures);
        }

        /// <summary>
        /// Registers the microgesture action map with the runtime.
        /// </summary>
        protected override void RegisterActionMapsWithRuntime()
        {
            ActionMapConfig actionMap = new ActionMapConfig()
            {
                name = "handinteractionmicrogestures",
                localizedName = kDeviceLocalizedName,
                desiredInteractionProfile = profile,
                manufacturer = "",
                serialNumber = "",
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Left),
                        userPath = UserPaths.leftHand
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Right),
                        userPath = UserPaths.rightHand
                    }
                },
                actions = new List<ActionConfig>()
                {
                    new ActionConfig()
                    {
                        name = "swipeLeft",
                        localizedName = "Swipe Left",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "swipeLeft"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = swipeLeft,
                                interactionProfileName = profile,
                            }
                        },
                        isAdditive = true
                    },
                    new ActionConfig()
                    {
                        name = "swipeRight",
                        localizedName = "Swipe Right",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "swipeRight"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = swipeRight,
                                interactionProfileName = profile,
                            }
                        },
                        isAdditive = true
                    },
                    new ActionConfig()
                    {
                        name = "swipeForward",
                        localizedName = "Swipe Forward",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "swipeForward"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = swipeForward,
                                interactionProfileName = profile,
                            }
                        },
                        isAdditive = true
                    },
                    new ActionConfig()
                    {
                        name = "swipeBackward",
                        localizedName = "Swipe Backward",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "swipeBackward"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = swipeBackward,
                                interactionProfileName = profile,
                            }
                        },
                        isAdditive = true
                    },
                    new ActionConfig()
                    {
                        name = "tapThumb",
                        localizedName = "Tap Thumb",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "tapThumb"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = tapThumb,
                                interactionProfileName = profile,
                            }
                        },
                        isAdditive = true
                    }
                }
            };

            AddActionMap(actionMap);
        }

        /// <summary>
        /// Adds the microgesture actions to each enabled Hand Interaction Profile action map.
        /// </summary>
        /// <remarks>
        /// Unity calls this method during OpenXR initialization when this feature is enabled. Action maps for other interaction profiles are unchanged.
        /// </remarks>
        /// <param name="actionMaps">Action maps from all enabled non-additive interaction profiles.</param>
        /// <param name="additiveMap">This feature's action map, which contains the microgesture actions to add.</param>
        protected internal override void AddAdditiveActions(List<OpenXRInteractionFeature.ActionMapConfig> actionMaps, ActionMapConfig additiveMap)
        {
            foreach (var actionMap in actionMaps)
            {
                if (!string.Equals(actionMap.desiredInteractionProfile, HandInteractionProfile.profile, StringComparison.Ordinal))
                    continue;

                foreach (var additiveAction in additiveMap.actions)
                {
                    if (additiveAction.isAdditive)
                        actionMap.actions.Add(additiveAction);
                }
            }
        }
    }
}
