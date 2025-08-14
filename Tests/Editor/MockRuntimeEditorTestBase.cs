using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.TestTooling;

namespace UnityEditor.XR.OpenXR.Tests
{
    abstract class MockRuntimeEditorTestBase
    {
        protected MockOpenXREnvironment m_Environment;
        protected Dictionary<Type, OpenXRFeature> m_EnabledFeaturesInProject = new();

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            // MockEnvironment settings make destructive changes to OpenXRSettings in Edit mode tests,
            // so this is a temporary(?) workaround until the MockEnvironment cleans itself up correctly
            foreach (var feature in OpenXRSettings.Instance.features)
            {
                if (feature.enabled)
                    m_EnabledFeaturesInProject.Add(feature.GetType(), feature);

                feature.enabled = false;
            }

            m_Environment = MockOpenXREnvironment.CreateEnvironment();
            m_Environment.Settings.EnableFeature<MockRuntime>(true);
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            m_Environment.Settings.EnableFeature<MockRuntime>(false);

            foreach (var feature in m_EnabledFeaturesInProject.Values)
            {
                feature.enabled = true;
            }

            if (!m_EnabledFeaturesInProject.ContainsKey(typeof(MockRuntime)))
                OpenXRSettings.Instance.GetFeature<MockRuntime>().enabled = false;

            m_EnabledFeaturesInProject.Clear();
            m_Environment.Dispose();
        }
    }
}
