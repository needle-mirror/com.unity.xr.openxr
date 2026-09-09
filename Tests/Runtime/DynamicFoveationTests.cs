using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using NUnit.Framework;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.TestTooling;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif
using XrFoveationProfileFB = System.UIntPtr;
using XrSession = System.UIntPtr;
using XrSwapchain = System.UIntPtr;

namespace UnityEngine.XR.OpenXR.Tests
{
    /// <summary>
    /// Verifies that toggling <see cref="FoveatedRenderingFeature.DynamicFoveationEnabled"/> preserves
    /// the foveation level, which is owned by the SRP Foveation and Legacy APIs.
    /// </summary>
    /// <remarks>
    /// The FB foveation profile carries a single level plus a separate dynamic flag, and both travel
    /// through one native cache. These tests intercept `xrCreateFoveationProfileFB` so they can assert
    /// on what actually reached the runtime rather than on what the C# layer cached.
    /// </remarks>
    class DynamicFoveationTests
    {
        // All four are required before the plugin creates its foveation extension. The Mock Runtime
        // advertises the first three, but not XR_FB_foveation_vulkan.
        static readonly string[] k_FoveationExtensions =
        {
            "XR_FB_foveation",
            "XR_FB_foveation_configuration",
            "XR_FB_swapchain_update_state",
            "XR_FB_foveation_vulkan",
        };

        // Any non-null handle will do; the plugin only checks that the call succeeded.
        static readonly XrFoveationProfileFB k_MockProfileHandle = new(0xF0EA7104);

        // What the plugin submitted in the most recent xrCreateFoveationProfileFB call.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static XrFoveationLevelFB s_LastLevel;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static XrFoveationDynamicFB s_LastDynamic;

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static int s_CreateProfileCallCount;

        // Set if any profile submitted during a test enabled dynamic foveation. The native layer
        // decides for itself how many profiles to create, so tests assert on what those profiles
        // contained rather than on how many of them there were.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static bool s_SawDynamicEnabled;

        // Anything the interceptor threw, so it can be reported instead of aborting the process.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static Exception s_InterceptorException;

        MockOpenXREnvironment m_Environment;
        readonly Dictionary<Type, OpenXRFeature> m_EnabledFeaturesInProject = new();

        bool m_OriginalEnableDynamicFoveation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Creating the environment deinitializes any loader that is already running, so it has
            // to happen before anything writes to OpenXRFeature.enabled, which logs an error and
            // keeps its old value while a loader is running.
            m_Environment = MockOpenXREnvironment.CreateEnvironment();

            foreach (var feature in OpenXRSettings.Instance.features)
            {
                if (feature.enabled)
                    m_EnabledFeaturesInProject.Add(feature.GetType(), feature);

                feature.enabled = false;
            }

            Assert.IsTrue(
                m_Environment.Settings.EnableFeature<MockRuntime>(true),
                "The Mock Runtime feature could not be enabled on the mock environment.");
            Assert.IsTrue(
                m_Environment.Settings.EnableFeature<FoveatedRenderingFeature>(true),
                "The Foveated Rendering feature could not be enabled on the mock environment.");

            foreach (var extension in k_FoveationExtensions)
            {
                m_Environment.Settings.RequestUseExtension(extension);
                m_Environment.AddSupportedExtension(extension, 1);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            m_Environment.Settings.EnableFeature<FoveatedRenderingFeature>(false);
            m_Environment.Settings.EnableFeature<MockRuntime>(false);

            // Dispose stops the environment, so no loader is running when the project's own feature
            // states are put back.
            m_Environment.Dispose();

            foreach (var feature in m_EnabledFeaturesInProject.Values)
            {
                feature.enabled = true;
            }

            m_EnabledFeaturesInProject.Clear();
        }

        [SetUp]
        public void Setup()
        {
            s_LastLevel = XrFoveationLevelFB.None;
            s_LastDynamic = XrFoveationDynamicFB.Disabled;
            s_CreateProfileCallCount = 0;
            s_SawDynamicEnabled = false;
            s_InterceptorException = null;

            // OnSessionCreate applies this setting, so each test needs a known starting point.
            // The feature asset belongs to the project, so the value has to be put back.
            var feature = GetFoveatedRenderingFeature();
            m_OriginalEnableDynamicFoveation = feature.DynamicFoveationEnabled;
            feature.DynamicFoveationEnabled = false;

            m_Environment.SetFunctionForInterceptor("xrCreateFoveationProfileFB", k_CreateFoveationProfilePtr);
            m_Environment.SetFunctionForInterceptor("xrDestroyFoveationProfileFB", k_DestroyFoveationProfilePtr);
            m_Environment.SetFunctionForInterceptor("xrUpdateSwapchainFB", k_UpdateSwapchainPtr);
        }

        [TearDown]
        public void TearDown()
        {
            m_Environment.Stop();

            m_Environment.SetFunctionForInterceptor("xrCreateFoveationProfileFB", IntPtr.Zero);
            m_Environment.SetFunctionForInterceptor("xrDestroyFoveationProfileFB", IntPtr.Zero);
            m_Environment.SetFunctionForInterceptor("xrUpdateSwapchainFB", IntPtr.Zero);

            var feature = GetFoveatedRenderingFeature();
            feature.DynamicFoveationEnabled = m_OriginalEnableDynamicFoveation;
        }

        [Test]
        public void EnablingDynamicFoveationPreservesTheFoveationLevel()
        {
            m_Environment.Start();

            var feature = GetFoveatedRenderingFeature();
            SeedFoveationLevel(XrFoveationLevelFB.High, XrFoveationDynamicFB.Disabled);
            var createCallsBeforeToggle = s_CreateProfileCallCount;

            feature.DynamicFoveationEnabled = true;

            AssertInterceptorDidNotThrow();
            Assert.AreEqual(
                createCallsBeforeToggle + 1, s_CreateProfileCallCount,
                "Toggling dynamic foveation should submit a new foveation profile.");
            Assert.AreEqual(
                XrFoveationLevelFB.High, s_LastLevel,
                "Enabling dynamic foveation must not change the foveation level.");
            Assert.AreEqual(XrFoveationDynamicFB.LevelEnabled, s_LastDynamic);
        }

        [Test]
        public void DisablingDynamicFoveationPreservesTheFoveationLevel()
        {
            m_Environment.Start();

            var feature = GetFoveatedRenderingFeature();
            SeedFoveationLevel(XrFoveationLevelFB.High, XrFoveationDynamicFB.LevelEnabled);
            var createCallsBeforeToggle = s_CreateProfileCallCount;

            feature.DynamicFoveationEnabled = false;

            AssertInterceptorDidNotThrow();
            Assert.AreEqual(
                createCallsBeforeToggle + 1, s_CreateProfileCallCount,
                "Toggling dynamic foveation should submit a new foveation profile.");
            Assert.AreEqual(
                XrFoveationLevelFB.High, s_LastLevel,
                "Disabling dynamic foveation must not change the foveation level.");
            Assert.AreEqual(XrFoveationDynamicFB.Disabled, s_LastDynamic);
        }

        [Test]
        public void DynamicFoveationSettingIsAppliedWhenTheSessionIsCreated()
        {
            var feature = GetFoveatedRenderingFeature();
            feature.DynamicFoveationEnabled = true;

            m_Environment.Start();

            AssertInterceptorDidNotThrow();
            Assert.IsTrue(
                feature.DynamicFoveationEnabled,
                "The project setting should be applied once the OpenXR session exists.");
            Assert.AreEqual(XrFoveationDynamicFB.LevelEnabled, s_LastDynamic);
        }

        /// <remarks>
        /// This asserts on what reached the runtime rather than on the number of
        /// `xrCreateFoveationProfileFB` calls. `OnSessionCreate` applies the setting whether it is
        /// on or off, the native layer decides for itself whether that turns into a new profile,
        /// and it creates profiles of its own while rendering frames, so a profile count is not a
        /// contract this feature keeps.
        /// </remarks>
        [Test]
        public void DisabledDynamicFoveationSettingDoesNotEnableDynamicFoveation()
        {
            // CustomSetup already left the setting disabled, and no loader is running yet.
            m_Environment.Start();

            AssertInterceptorDidNotThrow();
            Assert.IsFalse(
                s_SawDynamicEnabled,
                "Starting a session must not enable dynamic foveation while the setting is off.");

            Assert.AreEqual(
                XrResult.Success, NativeApi.Internal_GetFbFoveationDynamic(out var useFoveationDynamic));
            Assert.AreEqual(XrFoveationDynamicFB.Disabled, useFoveationDynamic);
        }

        /// <summary>
        /// Fails with the interceptor's exception rather than leaving a bare "the profile was not
        /// submitted" assertion, which would not say why.
        /// </summary>
        static void AssertInterceptorDidNotThrow()
        {
            if (s_InterceptorException != null)
            {
                Assert.Fail($"The xrCreateFoveationProfileFB interceptor threw: {s_InterceptorException}");
            }
        }

        FoveatedRenderingFeature GetFoveatedRenderingFeature()
        {
            var feature = m_Environment.Settings.GetFeature<FoveatedRenderingFeature>();
            Assert.IsNotNull(feature, "The Foveated Rendering feature was not enabled by the environment.");
            return feature;
        }

        /// <summary>
        /// Sets a foveation level the way the SRP Foveation and Legacy APIs do, then verifies the
        /// native foveation extension actually accepted it.
        /// </summary>
        static void SeedFoveationLevel(XrFoveationLevelFB level, XrFoveationDynamicFB useFoveationDynamic)
        {
            var session = MockRuntime.Instance.XrSession;

            Assert.AreEqual(
                XrResult.Success,
                NativeApi.Internal_SetFbFoveationLevel(session, level, 0f, useFoveationDynamic));

            AssertInterceptorDidNotThrow();

            // FBSetFoveationLevel returns XR_SUCCESS even when the foveation extension was never
            // created, in which case it does nothing and every assertion that follows would pass
            // vacuously. Fail here instead, with a message that says why.
            Assert.AreEqual(XrResult.Success, NativeApi.Internal_GetFbFoveationLevel(out var seededLevel));
            Assert.AreEqual(
                level, seededLevel,
                "The seeded foveation level was not applied, so the FB foveation extension is not "
                + "active and this test cannot verify anything.");
        }

        static class NativeApi
        {
            const string LibraryName = "UnityOpenXR";

            [DllImport(LibraryName, EntryPoint = "FBSetFoveationLevel")]
            internal static extern XrResult Internal_SetFbFoveationLevel(
                ulong session,
                XrFoveationLevelFB level,
                float verticalOffset,
                XrFoveationDynamicFB useFoveationDynamic);

            [DllImport(LibraryName, EntryPoint = "FBGetFoveationLevel")]
            internal static extern XrResult Internal_GetFbFoveationLevel(out XrFoveationLevelFB level);

            [DllImport(LibraryName, EntryPoint = "FBGetFoveationDynamic")]
            internal static extern XrResult Internal_GetFbFoveationDynamic(
                out XrFoveationDynamicFB useFoveationDynamic);
        }

        /// <summary>
        /// The common header of every OpenXR structure, used to walk a next chain.
        /// </summary>
        /// <remarks>
        /// Marshalling the header as a struct rather than reading the type field as a bare enum
        /// matters: <c>Marshal.PtrToStructure</c> of an enum type works under Mono but throws under
        /// IL2CPP, which aborts the process from inside a reverse P/Invoke.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        internal struct XrBaseInStructure
        {
            internal XrStructureType type;
            internal IntPtr next;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XrFoveationLevelProfileCreateInfoFB
        {
            internal XrStructureType type;
            internal IntPtr next;
            internal XrFoveationLevelFB level;
            internal float verticalOffset;
            internal XrFoveationDynamicFB useFoveationDynamic;
        }

        internal delegate XrResult CreateFoveationProfile_Delegate(
            XrSession session, IntPtr createInfo, out XrFoveationProfileFB profile);

        internal delegate XrResult DestroyFoveationProfile_Delegate(XrFoveationProfileFB profile);

        internal delegate XrResult UpdateSwapchain_Delegate(XrSwapchain swapchain, IntPtr state);

        // Held in static fields so the delegates are not collected while native holds the pointers.
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly CreateFoveationProfile_Delegate k_CreateFoveationProfile = CreateFoveationProfile_MockCallback;
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly DestroyFoveationProfile_Delegate k_DestroyFoveationProfile = DestroyFoveationProfile_MockCallback;
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly UpdateSwapchain_Delegate k_UpdateSwapchain = UpdateSwapchain_MockCallback;

        static readonly IntPtr k_CreateFoveationProfilePtr = Marshal.GetFunctionPointerForDelegate(k_CreateFoveationProfile);
        static readonly IntPtr k_DestroyFoveationProfilePtr = Marshal.GetFunctionPointerForDelegate(k_DestroyFoveationProfile);
        static readonly IntPtr k_UpdateSwapchainPtr = Marshal.GetFunctionPointerForDelegate(k_UpdateSwapchain);

        [MonoPInvokeCallback(typeof(CreateFoveationProfile_Delegate))]
        static XrResult CreateFoveationProfile_MockCallback(
            XrSession session, IntPtr createInfo, out XrFoveationProfileFB profile)
        {
            profile = k_MockProfileHandle;

            // A managed exception must never escape a reverse P/Invoke. The native caller in
            // libUnityOpenXR cannot unwind one, so IL2CPP turns it into std::terminate and the
            // whole test run aborts. Record it and report a failure to the runtime instead, so it
            // surfaces as a test failure with a usable message.
            try
            {
                // XrFoveationProfileCreateInfoFB carries no payload of its own. The level, vertical
                // offset and dynamic flag arrive chained onto its next pointer.
                var currentInfo = Marshal.PtrToStructure<XrBaseInStructure>(createInfo).next;

                while (currentInfo != IntPtr.Zero)
                {
                    var header = Marshal.PtrToStructure<XrBaseInStructure>(currentInfo);

                    if (header.type == XrStructureType.FoveationLevelProfileCreateInfoFB)
                    {
                        var levelProfile = Marshal.PtrToStructure<XrFoveationLevelProfileCreateInfoFB>(currentInfo);
                        s_LastLevel = levelProfile.level;
                        s_LastDynamic = levelProfile.useFoveationDynamic;
                        s_SawDynamicEnabled |=
                            levelProfile.useFoveationDynamic == XrFoveationDynamicFB.LevelEnabled;
                    }

                    currentInfo = header.next;
                }

                s_CreateProfileCallCount++;

                return XrResult.Success;
            }
            catch (Exception exception)
            {
                s_InterceptorException = exception;
                return XrResult.RuntimeFailure;
            }
        }

        [MonoPInvokeCallback(typeof(DestroyFoveationProfile_Delegate))]
        static XrResult DestroyFoveationProfile_MockCallback(XrFoveationProfileFB profile)
            => XrResult.Success;

        [MonoPInvokeCallback(typeof(UpdateSwapchain_Delegate))]
        static XrResult UpdateSwapchain_MockCallback(XrSwapchain swapchain, IntPtr state)
            => XrResult.Success;
    }
}
