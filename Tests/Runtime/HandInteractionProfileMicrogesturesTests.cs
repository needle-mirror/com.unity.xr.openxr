using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.ConformanceAutomation;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.XR.OpenXR.TestTooling;

namespace UnityEngine.XR.OpenXR.Tests
{
    class HandInteractionProfileMicrogesturesTests
    {
        static readonly string[] k_Hands =
        {
            OpenXRInteractionFeature.UserPaths.leftHand,
            OpenXRInteractionFeature.UserPaths.rightHand,
        };

        static readonly (string controlName, string interactionPath)[] k_Gestures =
        {
            ("swipeLeft", HandInteractionProfileMicrogestures.swipeLeft),
            ("swipeRight", HandInteractionProfileMicrogestures.swipeRight),
            ("swipeForward", HandInteractionProfileMicrogestures.swipeForward),
            ("swipeBackward", HandInteractionProfileMicrogestures.swipeBackward),
            ("tapThumb", HandInteractionProfileMicrogestures.tapThumb),
        };

        static readonly bool[] k_ExtensionStates = { true, false };

        readonly List<OpenXRFeature> m_EnabledFeaturesInProject = new List<OpenXRFeature>();

        MockOpenXREnvironment m_Environment;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            foreach (var feature in OpenXRSettings.Instance.features)
            {
                if (feature.enabled)
                    m_EnabledFeaturesInProject.Add(feature);

                feature.enabled = false;
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            foreach (var feature in m_EnabledFeaturesInProject)
                feature.enabled = true;

            m_EnabledFeaturesInProject.Clear();
        }

        // A fresh environment per test: Dispose() resets the native test API, so the supported extensions
        // and interceptors of one case can't leak into the next.
        [SetUp]
        public void SetUp()
        {
            m_Environment = MockOpenXREnvironment.CreateEnvironment();

            // The mock always advertises XR_ANDROID_enumerate_system_extension_properties with an empty list, and
            // the Hand Interaction Profile gates on IsSystemExtensionEnabled. Without this, the base profile never
            // registers the action map the additive gestures splice into.
            m_Environment.SetSystemExtensionEnabled(HandInteractionProfile.extensionString, true);

            EnableFeature<MockRuntime>();
            EnableFeature<HandInteractionProfile>();
            EnableFeature<HandInteractionProfileMicrogestures>();
            EnableFeature<ConformanceAutomationFeature>();
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            m_Environment.Dispose();

            // Gives the Input System an update to process the device removals before the next case.
            yield return null;
        }

        void EnableFeature<TFeature>() where TFeature : OpenXRFeature
        {
            Assert.That(m_Environment.Settings.EnableFeature<TFeature>(true), Is.True, $"Failed to enable {typeof(TFeature).Name}");
        }

        void StartEnvironment(bool microgesturesSupported) => StartEnvironment(microgesturesSupported, microgesturesSupported);

        void StartEnvironment(bool instanceExtensionSupported, bool systemExtensionEnabled)
        {
            if (instanceExtensionSupported)
                m_Environment.AddSupportedExtension(HandInteractionProfileMicrogestures.extensionString, 1);

            // Set false explicitly rather than omitting it: the mock's system-extension list persists across
            // instances in the editor process, so a prior supported case would otherwise leak a true.
            m_Environment.SetSystemExtensionEnabled(HandInteractionProfileMicrogestures.extensionString, systemExtensionEnabled);

            m_Environment.Start();
        }

        /// <summary>
        /// Validate that each of the 5 gesture controls on a hand is backed by a bound OpenXR action, is driven
        /// only by its own gesture path, and clears when the gesture is released.
        /// </summary>
        [UnityTest]
        public IEnumerator GesturesReceiveConformanceAutomationData([ValueSource(nameof(k_Hands))] string userPath)
        {
            StartEnvironment(true);
            yield return new WaitForXrFrame(2);

            var usage = userPath == OpenXRInteractionFeature.UserPaths.leftHand ? "{LeftHand}" : "{RightHand}";
            var actions = new InputAction[k_Gestures.Length];
            var failures = new List<string>();

            try
            {
                for (var gestureIdx = 0; gestureIdx < k_Gestures.Length; gestureIdx++)
                {
                    actions[gestureIdx] = new InputAction(
                        null,
                        InputActionType.Value,
                        $"<{nameof(HandInteractionProfileMicrogestures.HandInteractionMicrogestures)}>{usage}/{k_Gestures[gestureIdx].controlName}",
                        null,
                        null,
                        "Button");
                    actions[gestureIdx].Enable();
                }

                // Update and wait a frame so the actions are bound before driving them.
                InputSystem.InputSystem.Update();
                yield return new WaitForXrFrame(1);

                // Record failures rather than asserting per gesture so one broken binding doesn't mask the rest.
                for (var gestureIdx = 0; gestureIdx < k_Gestures.Length; gestureIdx++)
                {
                    var (controlName, interactionPath) = k_Gestures[gestureIdx];
                    var fullPath = $"{userPath}{interactionPath}";

                    // Exercises the managed-to-native action lookup that haptics and source-name queries share,
                    // which data flow alone doesn't cover for an additive device.
                    if (!OpenXRInput.TryGetInputSourceName(actions[gestureIdx], 0, out _))
                        failures.Add($"{controlName}: no OpenXR action or bound source behind the control");

                    ConformanceAutomationFeature.ConformanceAutomationSetBool(userPath, fullPath, true);
                    yield return new WaitForXrFrame(2);

                    var controlValueAfterPress = actions[gestureIdx].ReadValue<float>();
                    var otherActiveControls = new List<string>();
                    for (var otherGestureIdx = 0; otherGestureIdx < k_Gestures.Length; otherGestureIdx++)
                    {
                        if (otherGestureIdx != gestureIdx && actions[otherGestureIdx].ReadValue<float>() > 0f)
                            otherActiveControls.Add(k_Gestures[otherGestureIdx].controlName);
                    }

                    ConformanceAutomationFeature.ConformanceAutomationSetBool(userPath, fullPath, false);
                    yield return new WaitForXrFrame(2);

                    var controlValueAfterRelease = actions[gestureIdx].ReadValue<float>();

                    if (controlValueAfterPress <= 0f)
                        failures.Add($"{controlName}: never pressed (read {controlValueAfterPress} after press)");

                    if (otherActiveControls.Count > 0)
                        failures.Add($"{controlName}: press also drove [{string.Join(", ", otherActiveControls)}]");

                    if (controlValueAfterRelease > 0f)
                        failures.Add($"{controlName}: stuck after release (read {controlValueAfterRelease})");
                }
            }
            finally
            {
                foreach (var action in actions)
                {
                    action?.Disable();
                    action?.Dispose();
                }
            }

            if (failures.Count > 0)
                Assert.Fail($"Microgesture checks failed on '{userPath}':\n  {string.Join("\n  ", failures)}");
        }

        /// <summary>
        /// Validate that the feature only initializes and creates its per-hand devices when the runtime enables
        /// the XR_META_hand_tracking_microgestures extension and the system supports it, and that the Hand
        /// Interaction Profile it augments keeps working either way.
        /// </summary>
        [UnityTest]
        public IEnumerator MicrogestureDevicesGatedOnExtensionSupport([ValueSource(nameof(k_ExtensionStates))] bool supported)
        {
            StartEnvironment(supported);
            yield return new WaitForXrFrame(2);

            InputSystem.InputSystem.Update();

            Assert.That(OpenXRRuntime.IsExtensionEnabled(HandInteractionProfileMicrogestures.extensionString), Is.EqualTo(supported));
            Assert.That(OpenXRRuntime.IsSystemExtensionEnabled(HandInteractionProfileMicrogestures.extensionString), Is.EqualTo(supported));
            Assert.That(OpenXRSettings.Instance.GetFeature<HandInteractionProfileMicrogestures>().enabled, Is.EqualTo(supported));
            Assert.That(CountDevices<HandInteractionProfileMicrogestures.HandInteractionMicrogestures>(), Is.EqualTo(supported ? k_Hands.Length : 0), "Microgesture device count");
            Assert.That(CountDevices<HandInteractionProfile.HandInteraction>(), Is.EqualTo(k_Hands.Length), "Hand Interaction device count");
        }

        /// <summary>
        /// Validate that the feature stays disabled and creates no devices when the runtime enables the
        /// XR_META_hand_tracking_microgestures instance extension but the system (hardware) doesn't support it,
        /// and that the Hand Interaction Profile keeps working.
        /// </summary>
        [UnityTest]
        public IEnumerator MicrogestureDevicesNotCreatedWhenSystemLacksSupport()
        {
            StartEnvironment(instanceExtensionSupported: true, systemExtensionEnabled: false);
            yield return new WaitForXrFrame(2);

            InputSystem.InputSystem.Update();

            Assert.That(OpenXRRuntime.IsExtensionEnabled(HandInteractionProfileMicrogestures.extensionString), Is.True);
            Assert.That(OpenXRRuntime.IsSystemExtensionEnabled(HandInteractionProfileMicrogestures.extensionString), Is.False);
            Assert.That(OpenXRSettings.Instance.GetFeature<HandInteractionProfileMicrogestures>().enabled, Is.False);
            Assert.That(CountDevices<HandInteractionProfileMicrogestures.HandInteractionMicrogestures>(), Is.EqualTo(0), "Microgesture device count");
            Assert.That(CountDevices<HandInteractionProfile.HandInteraction>(), Is.EqualTo(k_Hands.Length), "Hand Interaction device count");
        }

        static int CountDevices<TDevice>() where TDevice : InputSystem.InputDevice
        {
            var count = 0;
            foreach (var device in InputSystem.InputSystem.devices)
            {
                if (device is TDevice)
                    count++;
            }

            return count;
        }
    }

    class HandInteractionProfileMicrogesturesActionMapTests
    {
        /// <summary>
        /// Validate that the feature splices its gesture actions only into the Hand Interaction Profile's action
        /// map, not into other profiles that also bind the hand user paths.
        /// </summary>
        [Test]
        public void AdditiveActionsOnlyAugmentHandInteractionProfile()
        {
            var feature = OpenXRSettings.Instance.GetFeature<HandInteractionProfileMicrogestures>();
            var additiveMaps = new List<OpenXRInteractionFeature.ActionMapConfig>();
            feature.CreateActionMaps(additiveMaps);
            var additiveMap = additiveMaps[0];

            var expectedActionCount = 0;
            foreach (var action in additiveMap.actions)
            {
                if (action.isAdditive)
                    expectedActionCount++;
            }

            var handInteractionMap = CreateHandActionMap(HandInteractionProfile.profile);
            var controllerMap = CreateHandActionMap(OculusTouchControllerProfile.profile);

            feature.AddAdditiveActions(new List<OpenXRInteractionFeature.ActionMapConfig> { handInteractionMap, controllerMap }, additiveMap);

            Assert.That(expectedActionCount, Is.GreaterThan(0));
            Assert.That(handInteractionMap.actions.Count, Is.EqualTo(expectedActionCount));
            Assert.That(controllerMap.actions, Is.Empty);
        }

        static OpenXRInteractionFeature.ActionMapConfig CreateHandActionMap(string interactionProfile) => new OpenXRInteractionFeature.ActionMapConfig
        {
            desiredInteractionProfile = interactionProfile,
            deviceInfos = new List<OpenXRInteractionFeature.DeviceConfig>
            {
                new OpenXRInteractionFeature.DeviceConfig { userPath = OpenXRInteractionFeature.UserPaths.leftHand },
                new OpenXRInteractionFeature.DeviceConfig { userPath = OpenXRInteractionFeature.UserPaths.rightHand },
            },
            actions = new List<OpenXRInteractionFeature.ActionConfig>(),
        };
    }
}
