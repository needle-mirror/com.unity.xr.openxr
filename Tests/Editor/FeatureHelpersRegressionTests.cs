using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Tests;
using Object = UnityEngine.Object;

namespace UnityEditor.XR.OpenXR.Tests
{
    class FeatureHelpersRegressionTests
    {
        internal enum NullFeatureEntryKind
        {
            TrueNull,

            // A destroyed object that the array still references. This is the analogue of a feature sub-asset
            // whose script can no longer be resolved, and it is the only case that pins the UnityEngine.Object
            // != operator: replacing the guards in FeatureHelpers with "feature is not null" would still pass
            // a TrueNull-only test while reintroducing the regression.
            DestroyedSubAsset
        }

        class OrphanedFeature : OpenXRFeature
        {
        }

        const string k_TestFolderName = "TempFeatureHelpersRegressionTests";
        const string k_TestSettingsAssetName = "TestOpenXRSettings.asset";
        const string k_OrphanedFeatureName = "Orphaned Feature";
        const string k_UnknownFeatureId = "com.unity.openxr.tests.feature.unknown";
        const BuildTargetGroup k_TestBuildTargetGroup = BuildTargetGroup.Standalone;

        static readonly BuildTargetGroup[] k_BuildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS
        };

        // Mirrors the BuildTargetGroups declared by MockAdditiveFeature. If that feature ever declares a
        // different set, this array is what needs to be updated alongside it.
        static readonly BuildTargetGroup[] k_MockAdditiveFeatureBuildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android
        };

        static readonly NullFeatureEntryKind[] k_NullFeatureEntryKinds =
        {
            NullFeatureEntryKind.TrueNull,
            NullFeatureEntryKind.DestroyedSubAsset
        };

        OpenXRPackageSettings m_PackageSettings;
        OpenXRSettings m_TestSettings;
        string m_TestFolderPath;

        [SetUp]
        public void SetUp()
        {
            m_TestFolderPath = OpenXRPackageSettings
                .GetAssetPathForComponents(new[] { k_TestFolderName })
                .Replace('\\', '/');

            m_TestSettings = ScriptableObject.CreateInstance<OpenXRSettings>();
            AssetDatabase.CreateAsset(m_TestSettings, Path.Combine(m_TestFolderPath, k_TestSettingsAssetName).Replace('\\', '/'));

            m_PackageSettings = OpenXRPackageSettings.GetOrCreateInstance();
            Assert.IsNotNull(m_PackageSettings, "The OpenXR package settings are required to override the settings locator.");

            ((IPackageSettings2)m_PackageSettings).OverrideSettingsLocatorFunc(_ => m_TestSettings);
        }

        [TearDown]
        public void TearDown()
        {
            if (m_PackageSettings != null)
                ((IPackageSettings2)m_PackageSettings).RestoreDefaultSettingsLocatorFunc();

            m_PackageSettings = null;
            m_TestSettings = null;

            if (!AssetDatabase.DeleteAsset(m_TestFolderPath))
                Debug.LogWarning($"Could not delete the temporary test settings folder at '{m_TestFolderPath}'.");
        }

        [Test]
        public void RefreshAllFeatureInfo_CreatesFeatureThatOmitsBuildTargetGroups(
            [ValueSource(nameof(k_BuildTargetGroups))] BuildTargetGroup buildTargetGroup)
        {
            FeatureHelpersInternal.RefreshAllFeatureInfo(buildTargetGroup);

            Assert.IsNotNull(
                m_TestSettings.GetFeature<MockAllBuildTargetGroupsFeature>(),
                $"A feature that omits BuildTargetGroups must be created for {buildTargetGroup}.");
        }

        [Test]
        public void GetAllFeatureInfo_ListsFeatureThatOmitsBuildTargetGroups(
            [ValueSource(nameof(k_BuildTargetGroups))] BuildTargetGroup buildTargetGroup)
        {
            FeatureHelpersInternal.RefreshAllFeatureInfo(buildTargetGroup);

            var allFeatureInfo = FeatureHelpersInternal.GetAllFeatureInfo(buildTargetGroup);

            Assert.IsTrue(
                ContainsFeature<MockAllBuildTargetGroupsFeature>(allFeatureInfo),
                $"A feature that omits BuildTargetGroups must be listed for {buildTargetGroup}.");
        }

        [Test]
        public void GetAllFeatureInfo_HonorsExplicitBuildTargetGroups(
            [ValueSource(nameof(k_BuildTargetGroups))] BuildTargetGroup buildTargetGroup)
        {
            var expectedToBeListed = false;
            foreach (var targetGroup in k_MockAdditiveFeatureBuildTargetGroups)
            {
                if (targetGroup == buildTargetGroup)
                {
                    expectedToBeListed = true;
                    break;
                }
            }

            // The Standalone refresh puts MockAdditiveFeature into the settings array. The test settings are
            // shared across build target groups, so it is still in the array when the other groups are
            // queried, and the build target group filter is the only thing that can keep it out.
            FeatureHelpersInternal.RefreshAllFeatureInfo(BuildTargetGroup.Standalone);
            FeatureHelpersInternal.RefreshAllFeatureInfo(buildTargetGroup);

            var allFeatureInfo = FeatureHelpersInternal.GetAllFeatureInfo(buildTargetGroup);

            Assert.AreEqual(
                expectedToBeListed,
                ContainsFeature<MockAdditiveFeature>(allFeatureInfo),
                $"MockAdditiveFeature declares Standalone and Android, but {buildTargetGroup} disagreed.");
        }

        [Test]
        public void RefreshAllFeatureInfo_DropsNullFeatureEntriesAndKeepsRealFeatures(
            [ValueSource(nameof(k_NullFeatureEntryKinds))] NullFeatureEntryKind nullFeatureEntryKind)
        {
            FeatureHelpersInternal.RefreshAllFeatureInfo(k_TestBuildTargetGroup);
            var featureCountBeforeInjection = m_TestSettings.features.Length;
            InjectNullFeatureEntry(nullFeatureEntryKind);

            Assert.DoesNotThrow(() => FeatureHelpersInternal.RefreshAllFeatureInfo(k_TestBuildTargetGroup));

            Assert.IsFalse(
                ContainsNullFeature(m_TestSettings.features),
                "A refresh must leave no null or destroyed entries in OpenXRSettings.features.");
            Assert.AreEqual(
                featureCountBeforeInjection,
                m_TestSettings.features.Length,
                "A refresh must remove only the injected entry and preserve every real feature.");
            Assert.IsNotNull(
                m_TestSettings.GetFeature<MockAdditiveFeature>(),
                "A real feature next to the null entry must survive the refresh.");
        }

        [Test]
        public void GetAllFeatureInfo_SkipsNullFeatureEntries(
            [ValueSource(nameof(k_NullFeatureEntryKinds))] NullFeatureEntryKind nullFeatureEntryKind)
        {
            FeatureHelpersInternal.RefreshAllFeatureInfo(k_TestBuildTargetGroup);
            InjectNullFeatureEntry(nullFeatureEntryKind);

            FeatureHelpersInternal.AllFeatureInfo allFeatureInfo = null;
            Assert.DoesNotThrow(() => allFeatureInfo = FeatureHelpersInternal.GetAllFeatureInfo(k_TestBuildTargetGroup));

            Assert.IsFalse(
                ContainsNullFeatureInfo(allFeatureInfo),
                "No FeatureInfo may be produced for a null or destroyed feature entry.");
            Assert.IsTrue(
                ContainsFeature<MockAdditiveFeature>(allFeatureInfo),
                "A real feature next to the null entry must still be listed.");
        }

        [Test]
        public void GetFeatureWithIdForBuildTarget_ReturnsFeatureThatFollowsANullEntry(
            [ValueSource(nameof(k_NullFeatureEntryKinds))] NullFeatureEntryKind nullFeatureEntryKind)
        {
            FeatureHelpersInternal.RefreshAllFeatureInfo(k_TestBuildTargetGroup);
            InjectNullFeatureEntry(nullFeatureEntryKind, prependEntry: true);

            OpenXRFeature feature = null;
            Assert.DoesNotThrow(() => feature = FeatureHelpers.GetFeatureWithIdForBuildTarget(
                k_TestBuildTargetGroup, MockAllBuildTargetGroupsFeature.k_FeatureId));

            Assert.IsInstanceOf<MockAllBuildTargetGroupsFeature>(
                feature,
                "The lookup must skip the null entry and return the matching feature.");
        }

        [Test]
        public void GetFeatureWithIdForBuildTarget_ReturnsNullForAnUnknownIdAfterANullEntry(
            [ValueSource(nameof(k_NullFeatureEntryKinds))] NullFeatureEntryKind nullFeatureEntryKind)
        {
            FeatureHelpersInternal.RefreshAllFeatureInfo(k_TestBuildTargetGroup);
            InjectNullFeatureEntry(nullFeatureEntryKind, prependEntry: true);

            OpenXRFeature feature = null;
            Assert.DoesNotThrow(() => feature = FeatureHelpers.GetFeatureWithIdForBuildTarget(
                k_TestBuildTargetGroup, k_UnknownFeatureId));

            Assert.IsNull(feature, "An unknown feature id must resolve to null rather than throw.");
        }

        static bool ContainsFeature<TFeature>(FeatureHelpersInternal.AllFeatureInfo allFeatureInfo)
            where TFeature : OpenXRFeature
        {
            foreach (var featureInfo in allFeatureInfo.Features)
            {
                if (featureInfo.Feature is TFeature)
                    return true;
            }

            return false;
        }

        static bool ContainsNullFeatureInfo(FeatureHelpersInternal.AllFeatureInfo allFeatureInfo)
        {
            foreach (var featureInfo in allFeatureInfo.Features)
            {
                if (featureInfo.Feature == null)
                    return true;
            }

            return false;
        }

        static bool ContainsNullFeature(OpenXRFeature[] features)
        {
            foreach (var feature in features)
            {
                if (feature == null)
                    return true;
            }

            return false;
        }

        void InjectNullFeatureEntry(NullFeatureEntryKind nullFeatureEntryKind, bool prependEntry = false)
        {
            var features = new List<OpenXRFeature>(m_TestSettings.features);
            OpenXRFeature injectedEntry = null;

            if (nullFeatureEntryKind == NullFeatureEntryKind.DestroyedSubAsset)
            {
                injectedEntry = ScriptableObject.CreateInstance<OrphanedFeature>();
                injectedEntry.name = k_OrphanedFeatureName;
                AssetDatabase.AddObjectToAsset(injectedEntry, m_TestSettings);
            }

            if (prependEntry)
                features.Insert(0, injectedEntry);
            else
                features.Add(injectedEntry);

            m_TestSettings.features = features.ToArray();

            // Destroyed after the array is assigned, so that the array keeps the managed wrapper.
            if (nullFeatureEntryKind == NullFeatureEntryKind.DestroyedSubAsset)
                Object.DestroyImmediate(injectedEntry, true);
        }
    }
}
