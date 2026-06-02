#if XR_HANDS_1_9_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.TestTooling;
using UnityEngine.XR.Hands.OpenXR;
using UnityEngine.XR.Hands.OpenXR.Meshing;

namespace UnityEngine.XR.OpenXR.Tests
{
    class MetaOpenXRHandMeshDataTests
    {
        [Test]
        public void FeatureId_IsCorrect()
        {
            Assert.AreEqual("com.unity.openxr.feature.metahandmeshdata", MetaOpenXRHandMeshData.featureId);
        }

#if UNITY_EDITOR
        [Test]
        public void Feature_HasOpenXRFeatureAttribute()
        {
            var attrs = typeof(MetaOpenXRHandMeshData).GetCustomAttributes(
                typeof(UnityEditor.XR.OpenXR.Features.OpenXRFeatureAttribute), false);
            Assert.IsTrue(attrs.Length > 0, "MetaOpenXRHandMeshData should have OpenXRFeature attribute");
        }

        [Test]
        public void Feature_RequestsHandTrackingMeshExtension()
        {
            var attrs = typeof(MetaOpenXRHandMeshData).GetCustomAttributes(
                typeof(UnityEditor.XR.OpenXR.Features.OpenXRFeatureAttribute), false);
            Assert.IsTrue(attrs.Length > 0);

            var attr = (UnityEditor.XR.OpenXR.Features.OpenXRFeatureAttribute)attrs[0];
            Assert.IsTrue(attr.OpenxrExtensionStrings.Contains("XR_FB_hand_tracking_mesh"),
                "Feature should request XR_FB_hand_tracking_mesh extension");
        }
#endif
    }

    // --- Mock OpenXR struct mirrors ---

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct MockXrHandTrackingMeshFB
    {
        public uint type;
        public void* next;
        public uint jointCapacityInput;
        public uint jointCountOutput;
        public void* jointBindPoses;
        public float* jointRadii;
        public int* jointParents;
        public uint vertexCapacityInput;
        public uint vertexCountOutput;
        public void* vertexPositions;
        public void* vertexNormals;
        public void* vertexUVs;
        public void* vertexBlendIndices;
        public void* vertexBlendWeights;
        public uint indexCapacityInput;
        public uint indexCountOutput;
        public short* indices;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct MockXrHandTrackerCreateInfoEXT
    {
        public uint type;
        public void* next;
        public uint hand;
        public uint handJointSet;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct MockXrHandJointsLocateInfoEXT
    {
        public uint type;
        public void* next;
        public ulong baseSpace;
        public long time;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct MockXrHandJointLocationEXT
    {
        public ulong locationFlags;
        public MockXrPosef pose;
        public float radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MockXrVector3f { public float x, y, z; }

    [StructLayout(LayoutKind.Sequential)]
    struct MockXrQuaternionf { public float x, y, z, w; }

    [StructLayout(LayoutKind.Sequential)]
    struct MockXrPosef
    {
        public MockXrQuaternionf orientation;
        public MockXrVector3f position;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct MockXrHandJointLocationsEXT
    {
        public uint type;
        public void* next;
        public uint isActive;
        public uint jointCount;
        public MockXrHandJointLocationEXT* jointLocations;
    }

    // --- Delegate types ---

    unsafe delegate XrResult MockXrGetHandMeshFB_Delegate(ulong handTracker, MockXrHandTrackingMeshFB* mesh);
    unsafe delegate XrResult MockXrCreateHandTrackerEXT_Delegate(ulong session, MockXrHandTrackerCreateInfoEXT* createInfo, ulong* handTracker);
    unsafe delegate XrResult MockXrDestroyHandTrackerEXT_Delegate(ulong handTracker);
    unsafe delegate XrResult MockXrLocateHandJointsEXT_Delegate(ulong handTracker, MockXrHandJointsLocateInfoEXT* locateInfo, MockXrHandJointLocationsEXT* locations);

    // --- MockRuntime lifecycle tests ---

    class MetaOpenXRHandMeshDataMockRuntimeTests
    {
        MockOpenXREnvironment m_Environment;
        Dictionary<Type, OpenXRFeature> m_EnabledFeaturesInProject = new();

        internal static int s_XrGetHandMeshFBCallCount;

        static readonly IntPtr s_TrackCallsPtr = MockXrGetHandMeshFBInterceptors.TrackCallsPtr;
        static readonly IntPtr s_ReturnErrorPtr = MockXrGetHandMeshFBInterceptors.ReturnErrorPtr;
        static readonly IntPtr s_ReturnCountsPtr = MockXrGetHandMeshFBInterceptors.ReturnCountsPtr;
#if !OPENXR_USE_KHRONOS_LOADER
        static readonly IntPtr s_CreateHandTrackerPtr = MockHandTrackingInterceptors.CreateHandTrackerPtr;
        static readonly IntPtr s_DestroyHandTrackerPtr = MockHandTrackingInterceptors.DestroyHandTrackerPtr;
        static readonly IntPtr s_LocateHandJointsPtr = MockHandTrackingInterceptors.LocateHandJointsPtr;
#endif

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            foreach (var feature in OpenXRSettings.Instance.features)
            {
                if (feature.enabled)
                    m_EnabledFeaturesInProject.Add(feature.GetType(), feature);

                feature.enabled = false;
            }

            m_Environment = MockOpenXREnvironment.CreateEnvironment();
            Assert.IsTrue(m_Environment.Settings.EnableFeature<MockRuntime>(true),
                "Failed to enable MockRuntime");
            Assert.IsTrue(m_Environment.Settings.EnableFeature<HandTracking>(true),
                "Failed to enable HandTracking - feature not found in mock settings");
            Assert.IsTrue(m_Environment.Settings.EnableFeature<MetaOpenXRHandMeshData>(true),
                "Failed to enable MetaOpenXRHandMeshData - feature not found in mock settings");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            m_Environment.Settings.EnableFeature<MockRuntime>(false);
            m_Environment.Settings.EnableFeature<HandTracking>(false);
            m_Environment.Settings.EnableFeature<MetaOpenXRHandMeshData>(false);

            foreach (var feature in m_EnabledFeaturesInProject.Values)
                feature.enabled = true;

            m_EnabledFeaturesInProject.Clear();
            m_Environment.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            s_XrGetHandMeshFBCallCount = 0;
        }

        [TearDown]
        public void TearDown()
        {
            m_Environment.SetFunctionForInterceptor("xrGetHandMeshFB", IntPtr.Zero);
            m_Environment.SetFunctionForInterceptor("xrCreateHandTrackerEXT", IntPtr.Zero);
            m_Environment.SetFunctionForInterceptor("xrDestroyHandTrackerEXT", IntPtr.Zero);
            m_Environment.SetFunctionForInterceptor("xrLocateHandJointsEXT", IntPtr.Zero);
            m_Environment.Stop();
        }

#if !OPENXR_USE_KHRONOS_LOADER
        [UnityTest]
        [UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer })]
        public IEnumerator Extension_IsEnabled_WhenSupported()
        {
            IgnoreIfHandTrackingDescriptorUnavailable();

            m_Environment.AddSupportedExtension("XR_FB_hand_tracking_mesh", 3);
            m_Environment.Settings.RequestUseExtension("XR_FB_hand_tracking_mesh");

            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            Assert.IsTrue(OpenXRRuntime.IsExtensionEnabled("XR_FB_hand_tracking_mesh"),
                "XR_FB_hand_tracking_mesh should be enabled when runtime supports it");
        }

        [UnityTest]
        [UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer })]
        public IEnumerator Feature_RemainsEnabled_WhenExtensionUnsupported()
        {
            IgnoreIfHandTrackingDescriptorUnavailable();

            // Don't add XR_FB_hand_tracking_mesh as a supported extension.
            // The feature should still be enabled — it just won't have data.
            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            var feature = OpenXRSettings.Instance.GetFeature<MetaOpenXRHandMeshData>();
            Assert.IsNotNull(feature);
            Assert.IsTrue(feature.enabled,
                "Feature should remain enabled even when the extension is not supported by the runtime");
        }

        [UnityTest]
        [UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer })]
        public IEnumerator Feature_RemainsEnabled_AfterStart()
        {
            IgnoreIfHandTrackingDescriptorUnavailable();

            m_Environment.AddSupportedExtension("XR_FB_hand_tracking_mesh", 3);
            m_Environment.Settings.RequestUseExtension("XR_FB_hand_tracking_mesh");

            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            var feature = OpenXRSettings.Instance.GetFeature<MetaOpenXRHandMeshData>();
            Assert.IsNotNull(feature);
            Assert.IsTrue(feature.enabled, "Feature should remain enabled after runtime start");
        }

        // OpenXRHandProvider.Register runs once at SubsystemRegistration and only registers
        // the 'OpenXR Hands' descriptor if HandTracking was enabled in OpenXRSettings at app
        // startup. CI projects ship with HandTracking disabled, so in standalone player builds
        // the descriptor is never registered and these tests can't validate their assertions.
        void IgnoreIfHandTrackingDescriptorUnavailable()
        {
            if (!m_EnabledFeaturesInProject.ContainsKey(typeof(HandTracking)))
                Assert.Ignore(
                    "HandTracking must be enabled in OpenXRSettings at app startup so the " +
                    "'OpenXR Hands' subsystem descriptor is registered. Enable the HandTracking " +
                    "feature in the test project's OpenXR settings to run this test.");
        }

        void ExpectHandTrackingStartupErrors()
        {
            // When XR_EXT_hand_tracking is supported with interceptors, HandTracking
            // still can't find its subsystem descriptor in the mock environment.
            LogAssert.Expect(LogType.Error,
                "Failed to find descriptor 'OpenXR Hands' - HandTracking OpenXR feature will not do anything!");
            LogAssert.Expect(LogType.Warning,
                "Hand Tracking Subsystem feature is not enabled - subsystem APIs for hand mesh data will fail.");
        }

        void InstallAllInterceptors()
        {
            m_Environment.AddSupportedExtension("XR_EXT_hand_tracking", 4);
            m_Environment.AddSupportedExtension("XR_FB_hand_tracking_mesh", 3);
            m_Environment.Settings.RequestUseExtension("XR_EXT_hand_tracking");
            m_Environment.Settings.RequestUseExtension("XR_FB_hand_tracking_mesh");
            m_Environment.SetFunctionForInterceptor("xrCreateHandTrackerEXT", s_CreateHandTrackerPtr);
            m_Environment.SetFunctionForInterceptor("xrDestroyHandTrackerEXT", s_DestroyHandTrackerPtr);
            m_Environment.SetFunctionForInterceptor("xrLocateHandJointsEXT", s_LocateHandJointsPtr);

            ExpectHandTrackingStartupErrors();
        }

        [UnityTest]
        [UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer })]
        public IEnumerator Interceptor_XrGetHandMeshFB_ErrorDoesNotCrashRuntime()
        {
            InstallAllInterceptors();
            m_Environment.SetFunctionForInterceptor("xrGetHandMeshFB", s_ReturnErrorPtr);

            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            Assert.IsNotNull(OpenXRLoaderBase.Instance,
                "Runtime should not crash when xrGetHandMeshFB returns errors");
            Assert.IsTrue(OpenXRRuntime.IsExtensionEnabled("XR_FB_hand_tracking_mesh"),
                "Extension should still be enabled even when calls return errors");
        }

        [UnityTest]
        [UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer })]
        public IEnumerator Interceptor_BothExtensions_AreEnabled()
        {
            InstallAllInterceptors();
            m_Environment.SetFunctionForInterceptor("xrGetHandMeshFB", s_TrackCallsPtr);

            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            Assert.IsTrue(OpenXRRuntime.IsExtensionEnabled("XR_EXT_hand_tracking"),
                "XR_EXT_hand_tracking should be enabled");
            Assert.IsTrue(OpenXRRuntime.IsExtensionEnabled("XR_FB_hand_tracking_mesh"),
                "XR_FB_hand_tracking_mesh should be enabled");
        }

        [UnityTest]
        [UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer })]
        public IEnumerator Interceptor_XrGetHandMeshFB_CountsInterceptorDoesNotCrash()
        {
            InstallAllInterceptors();
            m_Environment.SetFunctionForInterceptor("xrGetHandMeshFB", s_ReturnCountsPtr);

            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            Assert.IsNotNull(OpenXRLoaderBase.Instance,
                "Runtime should not crash with count-returning interceptor");
            Assert.IsTrue(OpenXRRuntime.IsExtensionEnabled("XR_FB_hand_tracking_mesh"),
                "Extension should be enabled with count-returning interceptor");
        }
#endif
    }

    // --- No hand tracking test ---

    class MetaOpenXRHandMeshDataNoHandTrackingTests
    {
        MockOpenXREnvironment m_Environment;
        Dictionary<Type, OpenXRFeature> m_EnabledFeaturesInProject = new();

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            foreach (var feature in OpenXRSettings.Instance.features)
            {
                if (feature.enabled)
                    m_EnabledFeaturesInProject.Add(feature.GetType(), feature);

                feature.enabled = false;
            }

            m_Environment = MockOpenXREnvironment.CreateEnvironment();
            m_Environment.Settings.EnableFeature<MockRuntime>(true);
            m_Environment.Settings.EnableFeature<MetaOpenXRHandMeshData>(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            m_Environment.Settings.EnableFeature<MockRuntime>(false);
            m_Environment.Settings.EnableFeature<MetaOpenXRHandMeshData>(false);

            foreach (var feature in m_EnabledFeaturesInProject.Values)
                feature.enabled = true;

            m_EnabledFeaturesInProject.Clear();
            m_Environment.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            m_Environment.Stop();
        }

        [UnityTest]
        public IEnumerator Start_WithoutHandTracking_LogsWarning()
        {
            m_Environment.AddSupportedExtension("XR_FB_hand_tracking_mesh", 3);
            m_Environment.Start();
            yield return new WaitForXrFrame(2);

            LogAssert.Expect(LogType.Warning,
                "Hand Tracking Subsystem feature is not enabled - subsystem APIs for hand mesh data will fail.");
        }
    }

    // --- Mock interceptor implementations ---

    static unsafe class MockXrGetHandMeshFBInterceptors
    {
        static readonly MockXrGetHandMeshFB_Delegate s_TrackCalls = TrackCalls;
        static readonly MockXrGetHandMeshFB_Delegate s_ReturnError = ReturnError;
        static readonly MockXrGetHandMeshFB_Delegate s_ReturnCounts = ReturnCounts;

        public static IntPtr TrackCallsPtr => Marshal.GetFunctionPointerForDelegate(s_TrackCalls);
        public static IntPtr ReturnErrorPtr => Marshal.GetFunctionPointerForDelegate(s_ReturnError);
        public static IntPtr ReturnCountsPtr => Marshal.GetFunctionPointerForDelegate(s_ReturnCounts);

        [MonoPInvokeCallback(typeof(MockXrGetHandMeshFB_Delegate))]
        static XrResult TrackCalls(ulong handTracker, MockXrHandTrackingMeshFB* mesh)
        {
            MetaOpenXRHandMeshDataMockRuntimeTests.s_XrGetHandMeshFBCallCount++;
            mesh->jointCountOutput = 0;
            mesh->vertexCountOutput = 0;
            mesh->indexCountOutput = 0;
            return XrResult.Success;
        }

        [MonoPInvokeCallback(typeof(MockXrGetHandMeshFB_Delegate))]
        static XrResult ReturnError(ulong handTracker, MockXrHandTrackingMeshFB* mesh)
        {
            MetaOpenXRHandMeshDataMockRuntimeTests.s_XrGetHandMeshFBCallCount++;
            return XrResult.RuntimeFailure;
        }

        [MonoPInvokeCallback(typeof(MockXrGetHandMeshFB_Delegate))]
        static XrResult ReturnCounts(ulong handTracker, MockXrHandTrackingMeshFB* mesh)
        {
            MetaOpenXRHandMeshDataMockRuntimeTests.s_XrGetHandMeshFBCallCount++;

            if (mesh->jointCapacityInput == 0 && mesh->vertexCapacityInput == 0)
            {
                mesh->jointCountOutput = 26;
                mesh->vertexCountOutput = 468;
                mesh->indexCountOutput = 2736;
            }
            else
            {
                mesh->jointCountOutput = Math.Min(mesh->jointCapacityInput, 26);
                mesh->vertexCountOutput = Math.Min(mesh->vertexCapacityInput, 468);
                mesh->indexCountOutput = Math.Min(mesh->indexCapacityInput, 2736);
            }

            return XrResult.Success;
        }
    }

    static unsafe class MockHandTrackingInterceptors
    {
        static readonly MockXrCreateHandTrackerEXT_Delegate s_Create = CreateHandTracker;
        static readonly MockXrDestroyHandTrackerEXT_Delegate s_Destroy = DestroyHandTracker;
        static readonly MockXrLocateHandJointsEXT_Delegate s_Locate = LocateHandJoints;

        static ulong s_NextHandTracker = 100;

        public static IntPtr CreateHandTrackerPtr => Marshal.GetFunctionPointerForDelegate(s_Create);
        public static IntPtr DestroyHandTrackerPtr => Marshal.GetFunctionPointerForDelegate(s_Destroy);
        public static IntPtr LocateHandJointsPtr => Marshal.GetFunctionPointerForDelegate(s_Locate);

        [MonoPInvokeCallback(typeof(MockXrCreateHandTrackerEXT_Delegate))]
        static XrResult CreateHandTracker(ulong session, MockXrHandTrackerCreateInfoEXT* createInfo, ulong* handTracker)
        {
            *handTracker = s_NextHandTracker++;
            return XrResult.Success;
        }

        [MonoPInvokeCallback(typeof(MockXrDestroyHandTrackerEXT_Delegate))]
        static XrResult DestroyHandTracker(ulong handTracker)
        {
            return XrResult.Success;
        }

        [MonoPInvokeCallback(typeof(MockXrLocateHandJointsEXT_Delegate))]
        static XrResult LocateHandJoints(ulong handTracker, MockXrHandJointsLocateInfoEXT* locateInfo, MockXrHandJointLocationsEXT* locations)
        {
            const int jointCount = 26;
            locations->isActive = 1;
            locations->jointCount = jointCount;

            if (locations->jointLocations != null)
            {
                for (int i = 0; i < jointCount && i < locations->jointCount; i++)
                {
                    locations->jointLocations[i].locationFlags = 0x7;
                    locations->jointLocations[i].pose.orientation = new MockXrQuaternionf { x = 0, y = 0, z = 0, w = 1 };
                    locations->jointLocations[i].pose.position = new MockXrVector3f { x = 0, y = 0, z = 0 };
                    locations->jointLocations[i].radius = 0.01f;
                }
            }

            return XrResult.Success;
        }
    }
}
#endif // XR_HANDS_1_9_OR_NEWER
