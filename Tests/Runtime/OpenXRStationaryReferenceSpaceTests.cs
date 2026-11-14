using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.Features.Extensions.StationaryReferenceSpace;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Tests
{
    internal class OpenXRStationaryReferenceSpaceTests : OpenXRLoaderSetup
    {
        XRInputSubsystem m_InputSubsystem;
        static readonly XrUuid s_TestGenerationId = new XrUuid(0x3141592653589793UL, 0x2718281828459045UL);

        const XrSpaceLocationFlags k_AllLocationFlags =
            XrSpaceLocationFlags.OrientationValid | XrSpaceLocationFlags.PositionValid |
            XrSpaceLocationFlags.OrientationTracked | XrSpaceLocationFlags.PositionTracked;

        public override void AfterTest()
        {
            base.AfterTest();
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            feature?.DestroyStationaryReferenceSpace();
        }

        IEnumerator CommonStationarySpaceTestSetup()
        {
            EnableFeature<XrStationaryReferenceSpaceFeature>();
            InitializeAndStart();
            yield return new WaitForXrFrame(2);

            // Query for the current input subsystem
            var subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            m_InputSubsystem = subsystems.Count > 0 ? subsystems[0] : null;

            Assert.That(m_InputSubsystem, Is.Not.Null, "No XRInputSubsystem found");

            // Set the tracking origin to device so the reference space used by these tests is deterministic.
            m_InputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
        }

        [UnityTest]
        public IEnumerator FeatureEnabledIfExtensionAvailable()
        {
            yield return CommonStationarySpaceTestSetup();

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);
        }

        [UnityTest]
        public IEnumerator StationarySpaceLifetimeTest()
        {
            yield return CommonStationarySpaceTestSetup();

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            feature.DestroyStationaryReferenceSpace();
            Assert.That(feature.IsStationarySpaceValid(), Is.False);

            // BeforeEndSession nulls the native session handle but leaves the pfn set, so a post-shutdown
            // call must report ProviderUninitialized. The Unsupported leg (pfn null with a live session) is
            // untestable through the mock: proc-addr resolution for xrGetStationaryReferenceSpaceGenerationIdEXT and
            // extension enablement are coupled in the mock runtime, so the pfn can't be withheld without also
            // disabling the extension.
            StopAndShutdown();
            var status = feature.GetStationarySpaceGenerationId(out _);
            Assert.That(status.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.ProviderUninitialized));
        }

        [Test]
        public void CallsBeforeStartReportProviderUninitializedTest()
        {
            // After the loader initializes but before it starts, no session has begun, so the provider isn't ready
            // yet. That must not be reported as the runtime lacking support.
            EnableFeature<XrStationaryReferenceSpaceFeature>();
            Initialize();

            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.ProviderUninitialized));
            Assert.That(feature.IsStationarySpaceValid(), Is.False);

            var locateStatus = feature.LocateStationarySpaceInAppSpace(out var pose, out var locationFlags);
            Assert.That(locateStatus.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.ProviderUninitialized));
            Assert.That(pose, Is.EqualTo(Pose.identity));
            Assert.That(locationFlags, Is.EqualTo(XrSpaceLocationFlags.None));

            var generationIdStatus = feature.GetStationarySpaceGenerationId(out var generationId);
            Assert.That(generationIdStatus.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.ProviderUninitialized));
            Assert.That(generationId, Is.EqualTo(XrUuid.empty));
        }

        [UnityTest]
        public IEnumerator StationarySpaceMultipleDestroyTest()
        {
            yield return CommonStationarySpaceTestSetup();

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            feature.DestroyStationaryReferenceSpace();

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            feature.DestroyStationaryReferenceSpace();
            Assert.That(feature.IsStationarySpaceValid(), Is.False);

            feature.DestroyStationaryReferenceSpace();
        }

        [UnityTest]
        public IEnumerator MultipleInitializeTest()
        {
            yield return CommonStationarySpaceTestSetup();

            var destroySpaceInvokeCount = 0;
            void DestroySpaceHook(string functionName, XrResult result)
            {
                Assert.That(functionName, Is.EqualTo("xrDestroySpace"));
                Assert.That(result, Is.EqualTo(XrResult.Success));
                destroySpaceInvokeCount++;
            }
            MockRuntime.SetFunctionCallback("xrDestroySpace", DestroySpaceHook);

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace(new Pose(new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity)).IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            Assert.That(destroySpaceInvokeCount, Is.EqualTo(1), "xrDestroySpace should have been called once as part of reinitialization of an existing space.");
        }

        [UnityTest]
        public IEnumerator LocateSpaceTest()
        {
            yield return CommonStationarySpaceTestSetup();

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            Vector3 expectedPosition = new Vector3(1.0f, 2.0f, 3.0f);
            Quaternion expectedRotation = new Quaternion(4.0f, 5.0f, 6.0f, 7.0f);
            MockRuntime.SetSpace(XrReferenceSpaceType.StationaryReferenceSpaceExt, expectedPosition, expectedRotation, k_AllLocationFlags);
            Assert.That(MockRuntime.StationaryReferenceSpace_TrySetGenerationId(s_TestGenerationId), Is.True);

            var status = feature.LocateStationarySpaceInAppSpace(out var pose, out var locationFlags);
            Assert.That(status.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.UnqualifiedSuccess));
            Assert.That(locationFlags, Is.EqualTo(k_AllLocationFlags));
            Assert.That(pose.position, Is.EqualTo(expectedPosition));
            Assert.That(pose.rotation, Is.EqualTo(expectedRotation));

            Assert.That(feature.GetStationarySpaceGenerationId(out XrUuid generationId).IsSuccess(), Is.True);
            Assert.That(generationId, Is.EqualTo(s_TestGenerationId));

            expectedPosition = new Vector3(3.0f, 2.0f, 1.0f);
            expectedRotation = new Quaternion(7.0f, 6.0f, 5.0f, 4.0f);
            MockRuntime.SetSpace(XrReferenceSpaceType.Local, expectedPosition, expectedRotation, k_AllLocationFlags);

            status = feature.LocateAppSpaceInStationarySpace(out pose, out locationFlags);
            Assert.That(status.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.UnqualifiedSuccess));
            Assert.That(locationFlags, Is.EqualTo(k_AllLocationFlags));
            Assert.That(pose.position, Is.EqualTo(expectedPosition));
            Assert.That(pose.rotation, Is.EqualTo(expectedRotation));
        }

        [UnityTest]
        public IEnumerator SpaceUpdateTest()
        {
            yield return CommonStationarySpaceTestSetup();

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            bool callbackInvoked = false;
            Action<XrStationaryReferenceSpaceFeature.StationarySpaceChangedEventArgs> onChanged = (args) =>
            {
                callbackInvoked = true;
                Assert.That(args.TryGetPoseInPreviousSpace(out Pose poseInPreviousSpace), Is.True);
                Assert.That(poseInPreviousSpace.position, Is.EqualTo(new Vector3(1.0f, 2.0f, 3.0f)));
                Assert.That(poseInPreviousSpace.rotation, Is.EqualTo(new Quaternion(4.0f, 5.0f, 6.0f, 7.0f)));
                Assert.That(args.featureSender.GetStationarySpaceGenerationId(out XrUuid generationId).IsSuccess(), Is.True);
                Assert.That(generationId, Is.EqualTo(s_TestGenerationId));
            };
            // The feature instance is shared across tests, so the handler must be removed even if a
            // callback assertion throws — otherwise a leaked handler fires during later tests.
            feature.stationarySpaceChanged += onChanged;
            try
            {
                MockRuntime.SetSpace(
                    XrReferenceSpaceType.StationaryReferenceSpaceExt,
                    new Vector3(1.0f, 2.0f, 3.0f), new Quaternion(4.0f, 5.0f, 6.0f, 7.0f),
                    k_AllLocationFlags,
                    MockRuntime.SetReferenceSpaceQueueEventBehavior.QueueWithPose);

                Assert.That(MockRuntime.StationaryReferenceSpace_TrySetGenerationId(s_TestGenerationId), Is.True);

                yield return new WaitForXrFrame(2);
            }
            finally
            {
                feature.stationarySpaceChanged -= onChanged;
            }

            Assert.That(callbackInvoked, Is.True);
        }

        [UnityTest]
        public IEnumerator SpaceUpdateWithoutPoseTest()
        {
            yield return CommonStationarySpaceTestSetup();

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            bool callbackInvoked = false;
            Action<XrStationaryReferenceSpaceFeature.StationarySpaceChangedEventArgs> onChanged = (args) =>
            {
                callbackInvoked = true;
                Assert.That(args.TryGetPoseInPreviousSpace(out Pose poseInPreviousSpace), Is.False);
                Assert.That(poseInPreviousSpace, Is.EqualTo(Pose.identity));
            };
            // The feature instance is shared across tests, so the handler must be removed even if a
            // callback assertion throws — otherwise a leaked handler fires during later tests.
            feature.stationarySpaceChanged += onChanged;
            try
            {
                MockRuntime.SetSpace(
                    XrReferenceSpaceType.StationaryReferenceSpaceExt,
                    new Vector3(1.0f, 2.0f, 3.0f), new Quaternion(4.0f, 5.0f, 6.0f, 7.0f),
                    k_AllLocationFlags,
                    MockRuntime.SetReferenceSpaceQueueEventBehavior.QueueWithoutPose);

                yield return new WaitForXrFrame(2);
            }
            finally
            {
                feature.stationarySpaceChanged -= onChanged;
            }

            Assert.That(callbackInvoked, Is.True);
        }

        [UnityTest]
        public IEnumerator SpaceUpdateWithNoListenerTest()
        {
            yield return CommonStationarySpaceTestSetup();

            // Query for the XrStationaryReferenceSpaceFeature
            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            MockRuntime.SetSpace(
                XrReferenceSpaceType.StationaryReferenceSpaceExt,
                new Vector3(1.0f, 2.0f, 3.0f),
                new Quaternion(4.0f, 5.0f, 6.0f, 7.0f),
                k_AllLocationFlags,
                MockRuntime.SetReferenceSpaceQueueEventBehavior.QueueWithPose);

            yield return new WaitForXrFrame(2);
        }

        [UnityTest]
        public IEnumerator SpaceUpdateAfterReinitializeNotifiesOnceTest()
        {
            yield return CommonStationarySpaceTestSetup();

            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            // Initializing again destroys the first space and creates a second one. A change to the reference space
            // type must still be reported once, not once per space created.
            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            var callbackCount = 0;
            Action<XrStationaryReferenceSpaceFeature.StationarySpaceChangedEventArgs> onChanged = _ => callbackCount++;

            // The feature instance is shared across tests, so the handler must be removed even if the test fails.
            feature.stationarySpaceChanged += onChanged;
            try
            {
                MockRuntime.SetSpace(
                    XrReferenceSpaceType.StationaryReferenceSpaceExt,
                    new Vector3(1.0f, 2.0f, 3.0f), new Quaternion(4.0f, 5.0f, 6.0f, 7.0f),
                    k_AllLocationFlags,
                    MockRuntime.SetReferenceSpaceQueueEventBehavior.QueueWithPose);

                yield return new WaitForXrFrame(2);
            }
            finally
            {
                feature.stationarySpaceChanged -= onChanged;
            }

            Assert.That(callbackCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator SpaceUpdateWhileVisibleTest()
        {
            yield return CommonStationarySpaceTestSetup();

            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            // A VISIBLE session is still rendering, so the change must be reported without waiting for focus.
            Assert.That(MockRuntime.TransitionToState(XrSessionState.Visible, false), Is.True);
            yield return new WaitForXrFrame(1);

            var callbackInvoked = false;
            Action<XrStationaryReferenceSpaceFeature.StationarySpaceChangedEventArgs> onChanged = _ => callbackInvoked = true;

            // The feature instance is shared across tests, so the handler must be removed even if the test fails.
            feature.stationarySpaceChanged += onChanged;
            try
            {
                MockRuntime.SetSpace(
                    XrReferenceSpaceType.StationaryReferenceSpaceExt,
                    new Vector3(1.0f, 2.0f, 3.0f), new Quaternion(4.0f, 5.0f, 6.0f, 7.0f),
                    k_AllLocationFlags,
                    MockRuntime.SetReferenceSpaceQueueEventBehavior.QueueWithoutPose);

                yield return new WaitForXrFrame(2);
            }
            finally
            {
                feature.stationarySpaceChanged -= onChanged;
            }

            Assert.That(callbackInvoked, Is.True);
        }

        [UnityTest]
        public IEnumerator LocateBeforeCreateTest()
        {
            yield return CommonStationarySpaceTestSetup();

            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            // Locating without a stationary space is a caller error, not a runtime one.
            var status = feature.LocateStationarySpaceInAppSpace(out var pose, out var locationFlags);
            Assert.That(status.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.ValidationFailure));
            Assert.That(pose, Is.EqualTo(Pose.identity));
            Assert.That(locationFlags, Is.EqualTo(XrSpaceLocationFlags.None));
        }

        [UnityTest]
        public IEnumerator LocateUntrackedSpaceTest()
        {
            yield return CommonStationarySpaceTestSetup();

            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);
            Assert.That(feature.IsStationarySpaceValid(), Is.True);

            // While the runtime relocates the space, the call succeeds, but the flags say the pose can't be used.
            var invalidPosition = new Vector3(1.0f, 2.0f, 3.0f);
            var invalidRotation = new Quaternion(4.0f, 5.0f, 6.0f, 7.0f);
            MockRuntime.SetSpace(XrReferenceSpaceType.StationaryReferenceSpaceExt, invalidPosition, invalidRotation, XrSpaceLocationFlags.None);
            Assert.That(MockRuntime.StationaryReferenceSpace_TrySetGenerationId(XrUuid.empty), Is.True);

            var status = feature.LocateStationarySpaceInAppSpace(out var pose, out var locationFlags);
            Assert.That(status.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.UnqualifiedSuccess));
            Assert.That(locationFlags, Is.EqualTo(XrSpaceLocationFlags.None));
            Assert.That(pose, Is.EqualTo(Pose.identity));
            Assert.That(feature.GetStationarySpaceGenerationId(out var generationId).IsSuccess(), Is.True);
            Assert.That(generationId, Is.EqualTo(XrUuid.empty));

            MockRuntime.SetSpace(XrReferenceSpaceType.Local, invalidPosition, invalidRotation, XrSpaceLocationFlags.None);
            status = feature.LocateAppSpaceInStationarySpace(out pose, out locationFlags);
            Assert.That(status.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.UnqualifiedSuccess));
            Assert.That(locationFlags, Is.EqualTo(XrSpaceLocationFlags.None));
            Assert.That(pose, Is.EqualTo(Pose.identity));
        }

        [UnityTest]
        public IEnumerator LocateSpaceWithOnlyOrientationValidTest()
        {
            yield return CommonStationarySpaceTestSetup();

            var feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            Assert.That(feature, Is.Not.Null);
            Assert.That(feature.enabled, Is.True);

            Assert.That(feature.InitializeStationaryReferenceSpace().IsSuccess(), Is.True);

            // Each part of the pose is returned only while its valid flag is set.
            var expectedRotation = new Quaternion(4.0f, 5.0f, 6.0f, 7.0f);
            const XrSpaceLocationFlags orientationOnly = XrSpaceLocationFlags.OrientationValid | XrSpaceLocationFlags.OrientationTracked;
            MockRuntime.SetSpace(XrReferenceSpaceType.StationaryReferenceSpaceExt, new Vector3(1.0f, 2.0f, 3.0f), expectedRotation, orientationOnly);

            var status = feature.LocateStationarySpaceInAppSpace(out var pose, out var locationFlags);
            Assert.That(status.statusCode, Is.EqualTo(OpenXRResultStatus.StatusCode.UnqualifiedSuccess));
            Assert.That(locationFlags, Is.EqualTo(orientationOnly));
            Assert.That(pose.position, Is.EqualTo(Vector3.zero));
            Assert.That(pose.rotation, Is.EqualTo(expectedRotation));
        }
    }
}
