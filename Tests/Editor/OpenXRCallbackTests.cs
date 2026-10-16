using NUnit.Framework;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.Tests;
using Assert = UnityEngine.Assertions.Assert;

namespace UnityEditor.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [Property("Covers", "Features")]
    class OpenXRCallbackTests : OpenXRLoaderSetup
    {
        [Test]
        [Description("OnInstanceCreate fires on features during loader start and receives the runtime's " +
                     "instance handle.")]
        public void InstanceCreated()
        {
            bool instanceCreated = false;
            MockRuntime.Instance.TestCallback = (methodName, param) =>
            {
                if (methodName == nameof(OpenXRFeature.OnInstanceCreate))
                {
                    instanceCreated = true;
                    Assert.AreEqual(10, (ulong)param);
                }
                return true;
            };

            AddExtension(MockRuntime.XR_UNITY_mock_test);

            InitializeAndStart();

            Assert.IsTrue(instanceCreated);
        }

        [Test]
        [Description("OnSessionCreate fires on features during loader start and receives the runtime's " +
                     "session handle.")]
        public void SessionCreated()
        {
            bool sessionCreated = false;
            MockRuntime.Instance.TestCallback = (methodName, param) =>
            {
                if (methodName == nameof(OpenXRFeature.OnSessionCreate))
                {
                    sessionCreated = true;
                    Assert.AreEqual(3, (ulong)param);
                }
                return true;
            };

            InitializeAndStart();

            Assert.IsTrue(sessionCreated);
        }
    }
}
