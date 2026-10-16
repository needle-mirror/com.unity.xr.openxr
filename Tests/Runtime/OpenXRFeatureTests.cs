using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Mock;

namespace UnityEngine.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "Features")]
    class OpenXRFeatureTests : OpenXRLoaderSetup
    {
        class FakeFeature : OpenXRFeature
        { }

        [Test]
        [Description("A feature with the highest priority value sorts first in OpenXRSettings.features when " +
                     "the loader starts.")]
        public void HighPriority()
        {
            MockRuntime.Instance.priority = Int32.MaxValue;

            InitializeAndStart();

            if (OpenXRSettings.Instance.features.Length == 1)
                Assert.IsTrue(OpenXRSettings.Instance.features[0] == MockRuntime.Instance);

            for (int i = 1; i < OpenXRSettings.Instance.features.Length; ++i)
            {
                Assert.IsTrue(OpenXRSettings.Instance.features[i - 1].priority >= OpenXRSettings.Instance.features[i].priority);
            }
        }

        [Test]
        [Description("A feature with the lowest priority value sorts last in OpenXRSettings.features when " +
                     "the loader starts.")]
        public void LowPriority()
        {
            MockRuntime.Instance.priority = Int32.MinValue;

            InitializeAndStart();

            Assert.IsTrue(OpenXRSettings.Instance.features[^1] == MockRuntime.Instance);
        }

        [Test]
        [Description("Changing a feature's enabled flag while OpenXR is running logs an error.")]
        public void ChangeEnabledAtRuntime()
        {
            InitializeAndStart();

            MockRuntime.Instance.enabled = false;
            LogAssert.Expect(LogType.Error, "OpenXRFeature.enabled cannot be changed while OpenXR is running");
        }

        [Test]
        [Description("After a feature's OnInstanceCreate fails, re-assigning its enabled flag to its current " +
                     "value is accepted and leaves the flag unchanged.")]
        public void FeatureFailedInitialization()
        {
            //Force OnInstanceCreate returning false so that failedInitialization is true.
            MockRuntime.Instance.TestCallback = (methodName, _) => methodName != nameof(OpenXRFeature.OnInstanceCreate);
            InitializeAndStart();
            var enableStatus = MockRuntime.Instance.enabled;
            MockRuntime.Instance.enabled = enableStatus;
            Assert.IsTrue(MockRuntime.Instance.enabled == enableStatus);
        }

        [Test]
        [Description("featureCount equals the number of features returned by GetFeatures(typeof(OpenXRFeature)).")]
        public void GetFeatureCount()
        {
            Assert.IsTrue(OpenXRSettings.Instance.featureCount == OpenXRSettings.Instance.GetFeatures(typeof(OpenXRFeature)).Length);
        }

        [Test]
        [Description("GetFeature with a type that is not an OpenXRFeature returns null.")]
        public void GetFeatureByTypeBadType()
        {
            Assert.IsNull(OpenXRSettings.Instance.GetFeature(typeof(OpenXRLoader)));
        }

        [Test]
        [Description("GetFeature<T> for a feature type that is not in the settings returns null.")]
        public void GetFeatureByTypeNotFound()
        {
            Assert.IsNull(OpenXRSettings.Instance.GetFeature<FakeFeature>());
        }

        [Test]
        [Description("GetFeature resolves the Mock Runtime feature both through the generic overload and " +
                     "by Type.")]
        public void GetFeatureByType()
        {
            var feature = OpenXRSettings.Instance.GetFeature<MockRuntime>();
            Assert.IsNotNull(feature);
            Assert.IsNotNull(OpenXRSettings.Instance.GetFeature(typeof(MockRuntime)) as MockRuntime);
        }

        [Test]
        [Description("GetFeatures<T> and GetFeatures(Type) both return exactly the one Mock Runtime feature.")]
        public void GetFeaturesByTypeArray()
        {
            var features = OpenXRSettings.Instance.GetFeatures<MockRuntime>();
            Assert.IsNotNull(features);
            Assert.IsTrue(features.Length == 1);
            Assert.IsTrue(features[0] is MockRuntime);

            features = OpenXRSettings.Instance.GetFeatures(typeof(MockRuntime));
            Assert.IsNotNull(features);
            Assert.IsTrue(features.Length == 1);
            Assert.IsTrue(features[0] is MockRuntime);
        }

        [Test]
        [Description("GetFeatures(List<T>) fills the list with the one matching feature and returns 1.")]
        public void GetFeaturesByGenericList()
        {
            var features = new List<MockRuntime>();
            Assert.IsTrue(OpenXRSettings.Instance.GetFeatures(features) == 1);
            Assert.IsNotNull(features[0]);
        }

        [Test]
        [Description("GetFeatures(Type, List<OpenXRFeature>) fills the list with the one matching feature " +
                     "and returns 1.")]
        public void GetFeaturesByTypeList()
        {
            var features = new List<OpenXRFeature>();
            Assert.IsTrue(OpenXRSettings.Instance.GetFeatures(typeof(MockRuntime), features) == 1);
            Assert.IsNotNull(features[0]);
            Assert.IsTrue(features[0] is MockRuntime);
        }

        [Test]
        [Description("GetFeatures() returns an array whose length equals featureCount.")]
        public void GetFeaturesArray()
        {
            var features = OpenXRSettings.Instance.GetFeatures();
            Assert.IsNotNull(features);
            Assert.IsTrue(features.Length == OpenXRSettings.Instance.featureCount);
        }

        [Test]
        [Description("GetFeatures(List<OpenXRFeature>) fills the list with every feature and returns " +
                     "featureCount.")]
        public void GetFeaturesList()
        {
            var features = new List<OpenXRFeature>();
            Assert.IsTrue(OpenXRSettings.Instance.GetFeatures(features) == OpenXRSettings.Instance.featureCount);
        }
    }
}
