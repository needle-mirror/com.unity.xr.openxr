using System;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace UnityEngine.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "ApiVersioning")]
    internal class TargetOpenXRApiVersionTest : OpenXRLoaderSetup
    {
        private OpenXRFeature EnableFeatureWithApiVersion(Type featureType, string ApiVersion, bool enable)
        {
            var feature = GetFeature(featureType);
            Assert.IsNotNull(feature);
            feature.targetOpenXRApiVersion = ApiVersion;
            feature.enabled = enable;
            return feature;
        }

        private T EnableFeatureWithApiVersion<T>(string ApiVersion, bool enable) where T : OpenXRFeature => EnableFeatureWithApiVersion(typeof(T), ApiVersion, enable) as T;

        private string ToString(OpenXRApiVersion apiVersion)
        {
            return $"{apiVersion.Major}.{apiVersion.Minor}.{apiVersion.Patch}";
        }

#if !OPENXR_USE_KHRONOS_LOADER
        // Only enabled features version being considered
        [UnityTest]
        [Description("Only enabled features contribute their targetOpenXRApiVersion; a disabled feature's " +
                     "higher version is ignored.")]
        public IEnumerator SelectEnabledFeature()
        {
            string enabledVersion = "1.100.1000"; // enabled feature with lower api version
            string disabledVersion = "1.100.2000"; // disabled feature with higher api version

            EnableFeatureWithApiVersion<OculusTouchControllerProfile>(enabledVersion, true);
            EnableFeatureWithApiVersion<MetaQuestTouchProControllerProfile>(disabledVersion, false);

            base.InitializeAndStart();
            yield return null;
            Assert.AreEqual(OpenXRRuntime.apiVersion, enabledVersion);
        }

        // Only valid version being added
        [UnityTest]
        [Description("Malformed target versions (missing patch, non-numeric parts) are ignored in favour of " +
                     "a well-formed one.")]
        public IEnumerator SelectValidApiVersion()
        {
            string validVersion = "1.100.1000";
            string invalidVersion1 = "1.100";
            string invalidVersion2 = "1.100a.2000b";

            EnableFeatureWithApiVersion<OculusTouchControllerProfile>(validVersion, true);
            EnableFeatureWithApiVersion<MetaQuestTouchProControllerProfile>(invalidVersion1, true);
            EnableFeatureWithApiVersion<MetaQuestTouchPlusControllerProfile>(invalidVersion2, true);

            base.InitializeAndStart();
            yield return null;
            Assert.AreEqual(OpenXRRuntime.apiVersion, validVersion);
        }

        // Ignore versions lower than default
        [UnityTest]
        [Description("Target versions lower than the package's current OpenXR API version are ignored and " +
                     "the current version is used.")]
        public IEnumerator IgnoreLowerApiVersion()
        {
            string invalidVersion1 = "1.1.42";
            string invalidVersion2 = "1.0.43";
            string invalidVersion3 = "1.0.100";

            EnableFeatureWithApiVersion<MetaQuestTouchProControllerProfile>(invalidVersion1, true);
            EnableFeatureWithApiVersion<MetaQuestTouchPlusControllerProfile>(invalidVersion2, true);
            EnableFeatureWithApiVersion<OculusTouchControllerProfile>(invalidVersion3, true);

            base.InitializeAndStart();
            yield return null;
            Assert.AreEqual(OpenXRRuntime.apiVersion, ToString(OpenXRApiVersion.Current));
        }

        // Ignore major version > 1 cases
        [UnityTest]
        [Description("Target versions with a major version other than 1 are ignored in favour of a valid " +
                     "1.x version.")]
        public IEnumerator IgnoreMajorApiVersion()
        {
            string invalidVersion1 = "2.0.100";
            string invalidVersion2 = "2.1.1";
            string validVersion = "1.2.100";

            EnableFeatureWithApiVersion<MetaQuestTouchProControllerProfile>(invalidVersion1, true);
            EnableFeatureWithApiVersion<MetaQuestTouchPlusControllerProfile>(invalidVersion2, true);
            EnableFeatureWithApiVersion<OculusTouchControllerProfile>(validVersion, true);

            base.InitializeAndStart();
            yield return null;
            Assert.AreEqual(OpenXRRuntime.apiVersion, validVersion);
        }

        // Ignore versions not following patch version incremental policy
        [UnityTest]
        [Description("Target versions whose patch number does not follow the incremental policy are ignored " +
                     "and the current version is used.")]
        public IEnumerator IgnorePatchApiVersionNotIncremental()
        {
            string invalidVersion1 = "1.2.5";
            string invalidVersion2 = "1.3.20";

            EnableFeatureWithApiVersion<MetaQuestTouchProControllerProfile>(invalidVersion1, true);
            EnableFeatureWithApiVersion<MetaQuestTouchPlusControllerProfile>(invalidVersion2, true);

            base.InitializeAndStart();
            yield return null;
            Assert.AreEqual(OpenXRRuntime.apiVersion, ToString(OpenXRApiVersion.Current));
        }

        // Select the highest version
        [UnityTest]
        [Description("When several enabled features request valid target versions, the highest is used.")]
        public IEnumerator SelectHighestVersion()
        {
            string version1 = "1.2.100";
            string version2 = "1.1.200";
            string version3 = "1.0.300";

            EnableFeatureWithApiVersion<OculusTouchControllerProfile>(version1, true);
            EnableFeatureWithApiVersion<MetaQuestTouchPlusControllerProfile>(version2, true);
            EnableFeatureWithApiVersion<MetaQuestTouchProControllerProfile>(version3, true);

            base.InitializeAndStart();
            yield return null;
            Assert.AreEqual(OpenXRRuntime.apiVersion, version1);
        }

        // Make sure default version is set as expected
        [UnityTest]
        [Description("With no enabled feature requesting a version, the runtime targets the package's " +
                     "current OpenXR API version.")]
        public IEnumerator TestVersionFallback()
        {
            string version1 = "";
            string version2 = "";
            string version3 = "";

            EnableFeatureWithApiVersion<OculusTouchControllerProfile>(version1, false);
            EnableFeatureWithApiVersion<MetaQuestTouchPlusControllerProfile>(version2, false);
            EnableFeatureWithApiVersion<MetaQuestTouchProControllerProfile>(version3, false);

            base.InitializeAndStart();
            yield return null;
            Assert.AreEqual(OpenXRRuntime.apiVersion, ToString(OpenXRApiVersion.Current));
        }
#endif
    }
}
