using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features;
using Assert = UnityEngine.Assertions.Assert;
using UnityEngine.XR.OpenXR.Tests;
using static UnityEditor.XR.OpenXR.Tests.OpenXREditorTestHelpers;

namespace UnityEditor.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [Property("Covers", "Features")]
    class FeatureSetTests : OpenXRLoaderSetup
    {
        const string k_KnownFeatureSetName = "Known Test";
        const string k_TestFeatureSetName = "Test Feature Set";
        const string k_TestFeatureSetNameHandAndEye = "Test Feature Set Hand and Eye Tracking";
        const string k_TestFeatureSetNameHand = "Test Feature Set Hand Tracking";
        const string k_TestFeatureSetDescription = "Test feature set";
        const string k_TestFeatureSetId = "com.unity.xr.test.featureset";
        const string k_TestFeatureSetIdTwo = "com.unity.xr.test.featureset2";
        const string k_TestFeatureSetIdThree = "com.unity.xr.test.featureset3";
        const string k_TestFeatureSetIdFour = "com.unity.xr.test.featureset4";

        [OpenXRFeatureSet(
            FeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId
            },
            UiName = k_TestFeatureSetName,
            Description = k_TestFeatureSetDescription,
            FeatureSetId = k_TestFeatureSetId,
            SupportedBuildTargets = new[] { BuildTargetGroup.Standalone },
            RequiredFeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId
            }
        )]
        [OpenXRFeatureSet(
            FeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
            },
            UiName = k_TestFeatureSetNameHandAndEye,
            Description = k_TestFeatureSetDescription,
            FeatureSetId = k_TestFeatureSetIdTwo,
            SupportedBuildTargets = new[] { BuildTargetGroup.WSA },
            RequiredFeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
            }
        )]
        [OpenXRFeatureSet(
            FeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
            },
            UiName = k_TestFeatureSetNameHand,
            Description = k_TestFeatureSetDescription,
            FeatureSetId = k_TestFeatureSetIdThree,
            SupportedBuildTargets = new[] { BuildTargetGroup.WSA },
            RequiredFeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
            }
        )]
        [OpenXRFeatureSet(
            FeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
            },
            UiName = k_TestFeatureSetName,
            Description = k_TestFeatureSetDescription,
            FeatureSetId = k_TestFeatureSetIdTwo,
            SupportedBuildTargets = new[] { BuildTargetGroup.Android },
            RequiredFeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
            }
        )]
        [OpenXRFeatureSet(
            FeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
                HTCViveControllerProfile.featureId,
                OculusTouchControllerProfile.featureId,
            },
            UiName = k_TestFeatureSetName,
            Description = k_TestFeatureSetDescription,
            FeatureSetId = k_TestFeatureSetIdFour,
            SupportedBuildTargets = new[] { BuildTargetGroup.Standalone },
            RequiredFeatureIds = new[]
            {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
            },
            DefaultFeatureIds = new[]
            {
                HTCViveControllerProfile.featureId,
            }
        )]
        sealed class TestFeatureSet { }


        public override void BeforeTest()
        {
            base.BeforeTest();
            OpenXRFeature.canSetFeatureDisabled = null;

            InitializeFeatureSets(true);
        }

        /// <summary>
        /// Initialize the feature sets by disabling all features sets and all features
        /// </summary>
        /// <param name="addTestFeatures">True to include test features</param>
        static void InitializeFeatureSets(bool addTestFeatures)
        {
            // Initialize first with test feature sets so we can make sure all feature sets are disabled
            OpenXRFeatureSetManager.InitializeFeatureSets(true);

            foreach (var buildTargetGroup in GetBuildTargetGroups())
            {
                // Disable all feature sets for this build target
                foreach (var featureSetInfo in OpenXRFeatureSetManager.FeatureSetInfosForBuildTarget(buildTargetGroup))
                {
                    featureSetInfo.isEnabled = false;
                    featureSetInfo.wasEnabled = false;
                    OpenXREditorSettings.Instance.SetFeatureSetSelected(buildTargetGroup, featureSetInfo.featureSetId, false);
                }

                // Disable all features for this build target
                var extInfo = FeatureHelpersInternal.GetAllFeatureInfo(buildTargetGroup);
                foreach (var ext in extInfo.Features)
                {
                    ext.Feature.enabled = false;
                }
            }

            // If requested with no feature sets then reinitialize
            if (!addTestFeatures)
                OpenXRFeatureSetManager.InitializeFeatureSets(false);

            foreach (var buildTargetGroup in GetBuildTargetGroups())
            {
                // No feature sets should be enabled for any build target
                Assert.IsFalse(OpenXRFeatureSetManager.FeatureSetInfosForBuildTarget(buildTargetGroup).Any(f => f.isEnabled));

                // No features should be enabled
                AssertAllFeatures(buildTargetGroup, FeatureDisabled);
            }
        }

        public override void AfterTest()
        {
            base.AfterTest();
            OpenXRFeature.canSetFeatureDisabled = OpenXRFeatureSetManager.CanFeatureBeDisabled;
            ClearFeatureInfos();
        }

        [Test]
        [Description("A build target with no registered feature sets (iOS) yields an empty list.")]
        public void NoFeatureSetsReturnsEmptyList()
        {
            var featureSets = OpenXRFeatureSetManager.FeatureSetsForBuildTarget(BuildTargetGroup.iOS);
            Assert.AreEqual(0, featureSets.Count);
        }

        [Test]
        [Description("With the test feature sets excluded, the WSA build target reports no feature sets at " +
                     "all.")]
        public void FoundExpectedFeatureSets()
        {
            InitializeFeatureSets(false);

            string[] expectedFeatureSets = { };

            var featureSets = OpenXRFeatureSetManager.FeatureSetsForBuildTarget(BuildTargetGroup.WSA);
            Assert.IsNotNull(featureSets);
            Assert.AreEqual(expectedFeatureSets.Length, featureSets.Count);

            foreach (var featureSet in featureSets)
            {
                if (Array.IndexOf(expectedFeatureSets, featureSet.featureSetId) == -1)
                    Assert.IsTrue(false, $"Found unexpected feature set id {featureSet.featureSetId}!");
            }
        }

        [Test]
        [Description("GetFeatureSetWithId returns null both for a build target that lacks the set and for " +
                     "an unknown id.")]
        public void UnknownFeatureSetRerturnNull()
        {
            // For this test we do not want the test features enabled so rerun the initilization with
            InitializeFeatureSets(false);
            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.iOS, k_TestFeatureSetId);
            Assert.IsNull(foundFeatureSet);
            foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Standalone, "BAD FEATURE SET ID");
            Assert.IsNull(foundFeatureSet);
        }

        [Test]
        [Description("On Standalone, a test-registered feature set that shares an id with the built-in " +
                     "'Known Test' set replaces it, so the override's name is reported.")]
        public void OverrideKnownTestFeatureSet()
        {
            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Standalone, k_TestFeatureSetId);
            Assert.IsNotNull(foundFeatureSet);
            Assert.AreEqual(0, String.Compare(foundFeatureSet.name, k_TestFeatureSetName, true));
        }

        [Test]
        [Description("On Android, where no override is registered for the id, the built-in 'Known Test' " +
                     "feature set is still the one reported.")]
        public void NonoverrideKnownTestFeatureSet()
        {
            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Android, k_TestFeatureSetId);
            Assert.IsNotNull(foundFeatureSet);
            Assert.AreEqual(0, String.Compare(foundFeatureSet.name, k_KnownFeatureSetName, true));
        }

        [Test]
        [Description("Enabling a feature set turns on exactly the features it lists and nothing else.")]
        public void EnableFeatureSetEnablesFeatures()
        {
            EnableFeatureSet(BuildTargetGroup.Standalone, k_TestFeatureSetId, enabled: true);
            AssertOnlyFeatures(BuildTargetGroup.Standalone, new[] { KHRSimpleControllerProfile.featureId }, FeatureEnabled);
        }

        [Test]
        [Description("Disabling a previously enabled feature set turns its features back off.")]
        public void DisableFeatureSetDisabledFeatures()
        {
            // Enable the feature set and make sure only its features are enabled
            EnableFeatureSet(BuildTargetGroup.Standalone, k_TestFeatureSetId);
            AssertOnlyFeatures(BuildTargetGroup.Standalone, new[] { KHRSimpleControllerProfile.featureId }, FeatureEnabled);

            // Disable the feature set an make sure its features are disabled
            EnableFeatureSet(BuildTargetGroup.Standalone, k_TestFeatureSetId, false);
            AssertAllFeatures(BuildTargetGroup.Standalone, FeatureDisabled);
        }

        [Test]
        [Description("Disabling one of two feature sets that share a required feature keeps the shared " +
                     "feature enabled until the other set is disabled as well.")]
        public void DisableSharedFeaturesLeaveSharedFeaturesEnabled()
        {
            // Enable all WSA feature sets and make sure only the WSA feature set features are enabled
            EnableFeatureSets(BuildTargetGroup.WSA, enabled: true);

            AssertOnlyFeatures(BuildTargetGroup.WSA, new[]
            {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
            }, FeatureEnabled);

            // Disable the feature seth with both features set as required
            EnableFeatureSet(BuildTargetGroup.WSA, k_TestFeatureSetIdTwo, enabled: false);
            AssertOnlyFeatures(BuildTargetGroup.WSA, new[]
            {
                KHRSimpleControllerProfile.featureId,
            }, FeatureEnabled);

            // Disable all WSA feature sets and make sure all features are disabled
            EnableFeatureSets(BuildTargetGroup.WSA, enabled: false);
            AssertAllFeatures(BuildTargetGroup.WSA, FeatureDisabled);
        }

        [Test]
        [Description("Enabling and then disabling a feature set leaves features that were enabled " +
                     "independently of it untouched.")]
        public void DisableSharedFeaturesLeaveOthersFeaturesEnabled()
        {
            string[] allFeatureIds = {
                KHRSimpleControllerProfile.featureId,
                EyeGazeInteraction.featureId,
                MicrosoftMotionControllerProfile.featureId,
            };

            string[] otherFeatureIds = {
                MicrosoftMotionControllerProfile.featureId,
            };

            EnableFeatureInfos(BuildTargetGroup.WSA, otherFeatureIds, true);

            // Enable the second feature set and ensure that only features in the `all` list are enabled
            var featureSetToEnable = OpenXRFeatureSetManager.GetFeatureSetInfoWithId(BuildTargetGroup.WSA, k_TestFeatureSetIdTwo);
            EnableFeatureSet(BuildTargetGroup.WSA, featureSetToEnable.featureSetId);
            AssertOnlyFeatures(BuildTargetGroup.WSA, allFeatureIds, FeatureEnabled);

            // Disable the second feature set and ensure only features in the `others` list are enabled
            var featureSetToDisable = OpenXRFeatureSetManager.GetFeatureSetInfoWithId(BuildTargetGroup.WSA, k_TestFeatureSetIdTwo);
            Assert.IsNotNull(featureSetToDisable);
            EnableFeatureSet(BuildTargetGroup.WSA, featureSetToDisable.featureSetId, enabled: false);
            AssertOnlyFeatures(BuildTargetGroup.WSA, otherFeatureIds, FeatureEnabled);
        }

        [Test]
        [Description("Enabling a feature set turns on its required and default features and leaves its " +
                     "purely optional features off.")]
        public void EnablingFeatureSetEnabledDefaultFeatures()
        {
            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetInfoWithId(BuildTargetGroup.Standalone, k_TestFeatureSetIdFour);
            Assert.IsNotNull(foundFeatureSet);
            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId);

            // Ensure that only the non-optional features are enabled
            AssertOnlyFeatures(BuildTargetGroup.Standalone, foundFeatureSet.featureIds, f => f.Feature.enabled == !FeatureIsOptional(foundFeatureSet, f));
        }

        [Test]
        [Description("Optional features stay off when a set is enabled or disabled, but once the user " +
                     "enables them, re-enabling the set leaves them on.")]
        public void EnablingFeatureSetLeavesOptionFeaturesEnabled()
        {
            // Enable the feature set
            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetInfoWithId(BuildTargetGroup.Standalone, k_TestFeatureSetIdFour);
            Assert.IsNotNull(foundFeatureSet);
            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId);

            // Ensure the Optional features are all disabled
            AssertAllFeatures(BuildTargetGroup.Standalone, f => !FeatureIsOptional(foundFeatureSet, f) || !f.Feature.enabled);

            // Disable the feature set and ensure the optional features are disabled
            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId, false);
            AssertAllFeatures(BuildTargetGroup.Standalone, f => !FeatureIsOptional(foundFeatureSet, f) || !f.Feature.enabled);

            // Enable the optional features and the feature set and ensure the optional features are still enabled
            EnableFeatureInfos(BuildTargetGroup.Standalone, true, f => FeatureIsOptional(foundFeatureSet, f));
            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId, enabled: true);
            AssertAllFeatures(BuildTargetGroup.Standalone, f => !FeatureIsOptional(foundFeatureSet, f) || f.Feature.enabled);

            // Enable the feature set again and make sure the optional features are still enabled
            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId, enabled: true);
            AssertAllFeatures(BuildTargetGroup.Standalone, f => !FeatureIsOptional(foundFeatureSet, f) || f.Feature.enabled);
        }

        [Test]
        [Description("Disabling a feature set turns off its required features but leaves its default " +
                     "features enabled.")]
        public void DisablingFeatureSetLeavesDefaultFeaturesEnabled()
        {
            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetInfoWithId(BuildTargetGroup.Standalone, k_TestFeatureSetIdFour);
            Assert.IsNotNull(foundFeatureSet);
            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId, true);

            // Ensure that the only enabled features are the non optional features
            AssertAllFeatures(BuildTargetGroup.Standalone, foundFeatureSet.featureIds, f => f.Feature.enabled == !FeatureIsOptional(foundFeatureSet, f));

            // Disabling the feature set should disable the required components but not the default ones
            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId, enabled: false);
            AssertAllFeatures(BuildTargetGroup.Standalone, foundFeatureSet.requiredFeatureIds, FeatureDisabled);
            AssertAllFeatures(BuildTargetGroup.Standalone, foundFeatureSet.defaultFeatureIds, FeatureEnabled);
        }

        [Test]
        [Description("Turning a set's default features off by hand keeps them off while the set's required " +
                     "features remain enabled.")]
        public void DisablingFeatureSetLeavesDisabledDefaultFeaturesDisabled()
        {
            var buildTargetGroup = BuildTargetGroup.Standalone;
            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetInfoWithId(buildTargetGroup, k_TestFeatureSetIdFour);
            Assert.IsNotNull(foundFeatureSet);

            EnableFeatureSet(buildTargetGroup, foundFeatureSet.featureSetId, enabled: true);

            // Ensure that only the non optional features are enabled
            AssertOnlyFeatures(buildTargetGroup, foundFeatureSet.featureIds, f => f.Feature.enabled == !FeatureIsOptional(foundFeatureSet, f));

            // Disable all features in the default feature list
            EnableFeatureInfos(buildTargetGroup, foundFeatureSet.defaultFeatureIds, enable: false);
            AssertAllFeatures(buildTargetGroup, foundFeatureSet.defaultFeatureIds, FeatureDisabled);

            // Ensure that all features in the required list are enabled and that all features in the default features list are disabled
            AssertAllFeatures(buildTargetGroup, foundFeatureSet.requiredFeatureIds, FeatureEnabled);
        }

        [Test]
        [Description("While a feature set is enabled its required features cannot be disabled; before it is " +
                     "enabled and after it is disabled they toggle freely.")]
        public void CanNotChangeEnabledStateOfRequiredFeature()
        {
            OpenXRFeatureSetManager.activeBuildTarget = BuildTargetGroup.Standalone;

            var foundFeatureSet = OpenXRFeatureSetManager.GetFeatureSetInfoWithId(BuildTargetGroup.Standalone, k_TestFeatureSetIdFour);
            Assert.IsNotNull(foundFeatureSet);

            var featureInfos = GetFeatureInfos(BuildTargetGroup.Standalone, foundFeatureSet.requiredFeatureIds);
            foreach (var featureInfo in featureInfos)
            {
                AssertFeatureEnabled(featureInfo, false);
                featureInfo.Feature.enabled = true;
                AssertFeatureEnabled(featureInfo);
                featureInfo.Feature.enabled = false;
                AssertFeatureEnabled(featureInfo, false);
            }

            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId, enabled: true);

            OpenXRFeature.canSetFeatureDisabled = OpenXRFeatureSetManager.CanFeatureBeDisabled;

            foreach (var featureInfo in featureInfos)
            {
                AssertFeatureEnabled(featureInfo);
                featureInfo.Feature.enabled = false;
                AssertFeatureEnabled(featureInfo);
            }

            EnableFeatureSet(BuildTargetGroup.Standalone, foundFeatureSet.featureSetId, enabled: false);

            foreach (var featureInfo in featureInfos)
            {
                AssertFeatureEnabled(featureInfo, false);
                featureInfo.Feature.enabled = true;
                AssertFeatureEnabled(featureInfo);
                featureInfo.Feature.enabled = false;
                AssertFeatureEnabled(featureInfo, false);
            }

            OpenXRFeatureSetManager.activeBuildTarget = BuildTargetGroup.Unknown;
        }
    }
}
