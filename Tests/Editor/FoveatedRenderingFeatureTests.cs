using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEditor.XR.OpenXR.Tests
{
    [Category("Level.Unit")]
    [NUnit.Framework.Property("Covers", "Features")]
    [NUnit.Framework.Property("Covers", "Rendering")]
    class FoveatedRenderingFeatureTests
    {
        FoveatedRenderingFeature m_Feature;
        bool m_OriginalEnableDynamicFoveation;

        [SetUp]
        public void SetUp()
        {
            Assert.IsNull(
                OpenXRLoaderBase.Instance,
                "These tests cover the behavior of the feature while no OpenXR loader is running.");

            m_Feature = OpenXRSettings.ActiveBuildTargetInstance.GetFeature<FoveatedRenderingFeature>();
            Assert.IsNotNull(m_Feature, "The Foveated Rendering feature was not found in the OpenXR settings.");

            // These tests mutate the project's own feature asset, so the value has to be put back.
            m_OriginalEnableDynamicFoveation = m_Feature.DynamicFoveationEnabled;
        }

        [TearDown]
        public void TearDown()
        {
            m_Feature.DynamicFoveationEnabled = m_OriginalEnableDynamicFoveation;
        }

        [Test]
        [Description("A freshly created FoveatedRenderingFeature has dynamic foveation disabled.")]
        public void DynamicFoveationIsDisabledByDefault()
        {
            // Checked on a fresh instance rather than the project's, so the result is the actual
            // default and does not depend on what another test left behind.
            var feature = ScriptableObject.CreateInstance<FoveatedRenderingFeature>();
            try
            {
                Assert.IsFalse(feature.DynamicFoveationEnabled);
            }
            finally
            {
                Object.DestroyImmediate(feature);
            }
        }

        [Description("Setting DynamicFoveationEnabled on the project's feature asset reads back the same " +
                     "value while no OpenXR loader is running.")]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicFoveationEnabledRoundTrips(bool enableDynamicFoveation)
        {
            m_Feature.DynamicFoveationEnabled = enableDynamicFoveation;

            Assert.AreEqual(enableDynamicFoveation, m_Feature.DynamicFoveationEnabled);
        }

        // Dynamic foveation used to be applied from OnInstanceCreate, before an OpenXR session
        // existed, which logged a warning on every startup. Reading or writing the property
        // without a session must be silent.
        [Test]
        [Description("Reading or writing DynamicFoveationEnabled with no OpenXR session logs nothing, " +
                     "guarding against the old per-startup warning.")]
        public void AccessingDynamicFoveationWithoutSessionDoesNotLog()
        {
            var messages = new List<string>();

            void OnLogMessageReceived(string condition, string stackTrace, LogType type)
                => messages.Add($"{type}: {condition}");

            Application.logMessageReceived += OnLogMessageReceived;
            try
            {
                m_Feature.DynamicFoveationEnabled = true;
                _ = m_Feature.DynamicFoveationEnabled;
            }
            finally
            {
                Application.logMessageReceived -= OnLogMessageReceived;
            }

            Assert.IsEmpty(messages, "Accessing dynamicFoveationEnabled without an OpenXR session must not log.");
        }
    }
}
