// The Android generic loader supports neither MockOpenXREnvironment.AddSupportedExtension, which the DPad and palm
// pose cases need, nor SetFunctionForInterceptor, which the profile switch needs.
#if !OPENXR_USE_KHRONOS_LOADER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.TestTooling;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "AdditiveInteraction")]
    class AdditiveParentInteractionProfileTests : MockRuntimeTestBase
    {
        const string k_GetCurrentInteractionProfile = "xrGetCurrentInteractionProfile";
        const int k_EventDataBufferSize = 4016;
        const string k_HandInteractionDeviceName = "Hand Interaction OpenXR";
        const string k_TouchDeviceName = "Oculus Touch Controller OpenXR";
        const string k_DPadDeviceName = "DPad Interaction OpenXR";
        const string k_PalmPoseDeviceName = "Palm Pose Interaction OpenXR";
        const string k_HandCommonPosesDeviceName = "Hand Interaction Poses OpenXR";

        [StructLayout(LayoutKind.Sequential)]
        unsafe struct MockXrInteractionProfileState
        {
            public XrStructureType type;
            public void* next;
            public ulong interactionProfile;
        }

        unsafe delegate XrResult GetCurrentInteractionProfileDelegate(ulong session, ulong topLevelUserPath, MockXrInteractionProfileState* interactionProfileState);

        // The mock enables XR_ANDROID_enumerate_system_extension_properties, so IsSystemExtensionEnabled reads the
        // mock's system-extension map, which starts empty, instead of falling back to the enabled extensions.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly string[] k_SystemExtensions =
        {
            "XR_EXT_hand_interaction",
            "XR_KHR_binding_modification",
            "XR_EXT_dpad_binding",
            "XR_EXT_palm_pose"
        };

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly (string profile, string hostDeviceName)[] k_Steps =
        {
            (HandInteractionProfile.profile, k_HandInteractionDeviceName),
            (OculusTouchControllerProfile.profile, k_TouchDeviceName),
            (HandInteractionProfile.profile, k_HandInteractionDeviceName)
        };

        // Rooted so the delegate is not collected while native holds its pointer.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly unsafe GetCurrentInteractionProfileDelegate k_GetCurrentInteractionProfileDelegate = GetCurrentInteractionProfile;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly IntPtr k_GetCurrentInteractionProfilePtr = Marshal.GetFunctionPointerForDelegate(k_GetCurrentInteractionProfileDelegate);

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static ulong s_CurrentInteractionProfile;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static ulong s_LeftHandPath;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static ulong s_RightHandPath;

        protected override void CustomOneTimeSetup()
        {
            Assert.That(m_Environment.Settings.EnableFeature<OculusTouchControllerProfile>(true), Is.True);
            Assert.That(m_Environment.Settings.EnableFeature<HandInteractionProfile>(true), Is.True);

            m_Environment.AddSupportedExtension("XR_KHR_binding_modification", 1);
            m_Environment.AddSupportedExtension("XR_EXT_dpad_binding", 1);
            m_Environment.AddSupportedExtension("XR_EXT_palm_pose", 3);
        }

        // NUnit runs a derived one-time teardown before the base one. The base restores the project's enabled features
        // and then disposes the environment, so these must be disabled before it runs.
        [OneTimeTearDown]
        public void DisableProfileFeatures()
        {
            m_Environment.Settings.EnableFeature<HandInteractionProfile>(false);
            m_Environment.Settings.EnableFeature<OculusTouchControllerProfile>(false);
        }

        protected override void TearDown()
        {
            // Runs before the base Stop because on trampoline builds the mock API call is a no-op without a live instance.
            SetSystemExtensionsEnabled(false);
            base.TearDown();
        }

        protected override void CustomTearDown()
        {
            MockRuntime.ClearFunctionCallbacks();
            m_Environment.SetFunctionForInterceptor(k_GetCurrentInteractionProfile, IntPtr.Zero);

            m_Environment.Settings.EnableFeature<SingleHostAdditiveTestFeature>(false);
            m_Environment.Settings.EnableFeature<DPadInteraction>(false);
            m_Environment.Settings.EnableFeature<PalmPoseInteraction>(false);
            m_Environment.Settings.EnableFeature<HandCommonPosesInteraction>(false);

            s_CurrentInteractionProfile = 0;
            s_LeftHandPath = 0;
            s_RightHandPath = 0;
        }

        [UnityTest]
        [Description("An additive feature that declares the hand interaction profile as its only parent connects " +
                     "its companions under hand interaction and disconnects them under Touch.")]
        public IEnumerator SingleHostAdditive_ConnectsOnlyUnderDeclaredParent()
        {
            Assert.That(m_Environment.Settings.EnableFeature<SingleHostAdditiveTestFeature>(true), Is.True);

            yield return RunProfileSwitchSteps(SingleHostAdditiveTestFeature.localizedDeviceName, new[] { 2, 0, 2 });
        }

        [UnityTest]
        [Description("DPad declares no parent profiles, so its companions stay connected under any current " +
                     "interaction profile, including hand interaction where nothing writes to them.")]
        public IEnumerator DPad_FollowsAnyProfile()
        {
            Assert.That(m_Environment.Settings.EnableFeature<DPadInteraction>(true), Is.True);

            yield return RunProfileSwitchSteps(k_DPadDeviceName, new[] { 2, 2, 2 });
        }

        [UnityTest]
        [Description("Palm pose declares no parent profiles, so its companions stay connected under any current " +
                     "interaction profile.")]
        public IEnumerator PalmPose_FollowsAnyProfile()
        {
            Assert.That(m_Environment.Settings.EnableFeature<PalmPoseInteraction>(true), Is.True);

            yield return RunProfileSwitchSteps(k_PalmPoseDeviceName, new[] { 2, 2, 2 });
        }

        [UnityTest]
        [Description("Hand common poses declares no parent profiles, so its companions stay connected under any " +
                     "current interaction profile.")]
        public IEnumerator HandCommonPoses_FollowsAnyProfile()
        {
            Assert.That(m_Environment.Settings.EnableFeature<HandCommonPosesInteraction>(true), Is.True);

            yield return RunProfileSwitchSteps(k_HandCommonPosesDeviceName, new[] { 2, 2, 2 });
        }

        // The mock sets each user path's profile once, from the first suggested binding, so the interceptor stands in
        // for xrGetCurrentInteractionProfile and a profile-changed event makes the plugin re-query it.
        IEnumerator RunProfileSwitchSteps(string companionDeviceName, int[] expectedCompanionCounts)
        {
            m_Environment.SetFunctionForInterceptor(k_GetCurrentInteractionProfile, k_GetCurrentInteractionProfilePtr);
            MockRuntime.SetFunctionCallback("xrCreateInstance", null, (_, _) => SetSystemExtensionsEnabled(true));
            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            s_LeftHandPath = SingleHostAdditiveTestFeature.ConvertStringToPath(OpenXRInteractionFeature.UserPaths.leftHand);
            s_RightHandPath = SingleHostAdditiveTestFeature.ConvertStringToPath(OpenXRInteractionFeature.UserPaths.rightHand);

            for (var stepIndex = 0; stepIndex < k_Steps.Length; ++stepIndex)
            {
                var step = k_Steps[stepIndex];
                s_CurrentInteractionProfile = SingleHostAdditiveTestFeature.ConvertStringToPath(step.profile);
                EnqueueInteractionProfileChanged(m_Environment);
                m_Environment.ProcessEventQueue();
                yield return new WaitForXrFrame(1);

                Assert.That(CountConnectedDevices(step.hostDeviceName), Is.EqualTo(2), $"Host devices at step {stepIndex} ({step.profile})");
                Assert.That(CountConnectedDevices(companionDeviceName), Is.EqualTo(expectedCompanionCounts[stepIndex]), $"Companion devices at step {stepIndex} ({step.profile})");
            }
        }

        static void SetSystemExtensionsEnabled(bool enabled)
        {
            for (var extensionIndex = 0; extensionIndex < k_SystemExtensions.Length; ++extensionIndex)
                MockRuntime.AndroidEnumerateSystemExtensionProperties_SetSystemExtensionEnabled(k_SystemExtensions[extensionIndex], enabled);
        }

        static int CountConnectedDevices(string deviceName)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            var count = 0;
            for (var deviceIndex = 0; deviceIndex < devices.Count; ++deviceIndex)
            {
                if (string.CompareOrdinal(devices[deviceIndex].name, deviceName) == 0)
                    ++count;
            }

            return count;
        }

        static unsafe void EnqueueInteractionProfileChanged(MockOpenXREnvironment environment)
        {
            // The mock copies a whole XrEventDataBuffer from this pointer, so back the header with a full-size buffer.
            var eventBuffer = stackalloc byte[k_EventDataBufferSize];
            *(XrEventDataBaseHeader*)eventBuffer = new XrEventDataBaseHeader(XrStructureType.EventDataInteractionProfileChanged);
            environment.EnqueueMockEventData(new IntPtr(eventBuffer));
        }

        [MonoPInvokeCallback(typeof(GetCurrentInteractionProfileDelegate))]
        static unsafe XrResult GetCurrentInteractionProfile(ulong session, ulong topLevelUserPath, MockXrInteractionProfileState* interactionProfileState)
        {
            var isHand = topLevelUserPath == s_LeftHandPath || topLevelUserPath == s_RightHandPath;
            interactionProfileState->interactionProfile = isHand ? s_CurrentInteractionProfile : 0ul;
            return XrResult.Success;
        }
    }
}
#endif
