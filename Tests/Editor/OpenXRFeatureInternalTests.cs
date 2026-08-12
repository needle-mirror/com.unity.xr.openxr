using NUnit.Framework;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEditor.XR.OpenXR.Tests
{
    class OpenXRFeatureInternalTests : MockRuntimeEditorTestBase
    {
        // The app space is assigned when the input provider starts; the predicted display time is cached from
        // xrWaitFrame on the render path. EditMode never renders a frame, so the success path is not reachable
        // here and is covered on device instead. These tests pin the failure contract for both states that are
        // reachable: the exports must report failure rather than hand back a zeroed space or time, which the
        // runtime would reject as an invalid handle or time.

        [Test]
        [TestCase(false, TestName = "Internal_GetPredictedDisplayTime_Fails_BeforeStart")]
        [TestCase(true, TestName = "Internal_GetPredictedDisplayTime_Fails_StartedButNoFrameRendered")]
        public void Internal_GetPredictedDisplayTime_FailsWhenStateIsUnavailable(bool start)
        {
            if (start)
                m_Environment.Start();

            var succeeded = OpenXRFeature.Internal_GetPredictedDisplayTime(out var predictedDisplayTime);

            Assert.IsFalse(succeeded, "The export must not report success without a usable predicted display time.");
            Assert.AreEqual(0, predictedDisplayTime, "A failed call must leave the time at zero.");
        }

        [Test]
        [TestCase(false, TestName = "Internal_GetAppSpaceAndPredictedDisplayTime_Fails_BeforeStart")]
        [TestCase(true, TestName = "Internal_GetAppSpaceAndPredictedDisplayTime_Fails_StartedButNoFrameRendered")]
        public void Internal_GetAppSpaceAndPredictedDisplayTime_FailsWhenStateIsUnavailable(bool start)
        {
            if (start)
                m_Environment.Start();

            var succeeded = OpenXRFeature.Internal_GetAppSpaceAndPredictedDisplayTime(
                out var appSpace, out var predictedDisplayTime);

            Assert.IsFalse(succeeded, "The export must not report success without a usable app space and time.");
            Assert.AreEqual(0, appSpace, "A failed call must leave the app space at zero.");
            Assert.AreEqual(0, predictedDisplayTime, "A failed call must leave the time at zero.");
        }
    }
}
