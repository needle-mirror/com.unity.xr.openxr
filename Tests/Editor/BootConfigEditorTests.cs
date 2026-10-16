using System;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using UnityEngine.XR.OpenXR.Tests;

namespace UnityEditor.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "Settings")]
    class BootConfigEditorTests : OpenXRLoaderSetup
    {
        [Test]
        [Description("Boot config entries written for the Android build target round-trip " +
                     "through a re-read, and unset keys report absent.")]
        public void TestCanCreateBootConfigAndroid()
        {
            TestBuildTarget(BuildTarget.Android);
        }

        [Test]
        [Description("Boot config entries written for both 32- and 64-bit standalone Windows " +
                     "build targets round-trip through a re-read.")]
        public void TestCanCreateBootConfigWindows()
        {
            TestBuildTarget(BuildTarget.StandaloneWindows);
            TestBuildTarget(BuildTarget.StandaloneWindows64);
        }

        static void TestBuildTarget(BuildTarget buildTarget)
        {
            var bootConfig = new BootConfig(buildTarget);
            bootConfig.ReadBootConfig();

            // Check to see that we do not have the following key in the boot config
            Assert.IsFalse(bootConfig.TryGetValue("xr-sample-bootconfig-key01", out string value));
            Assert.AreEqual(value, null);

            // Check to see that we can store a key and retrieve it.
            bootConfig.SetValueForKey("xr-sample-bootconfig-key02", "primary value");
            Assert.IsTrue(bootConfig.TryGetValue("xr-sample-bootconfig-key02", out string key02value));
            Assert.AreEqual(key02value, "primary value");
            Assert.IsTrue(bootConfig.CheckValuePairExists("xr-sample-bootconfig-key02", "primary value"));

            // check to see that we can write the keys to the boot config and ensure that we can
            // retrieve the stored values
            bootConfig.WriteBootConfig();

            var cloneBootConfig = new BootConfig(buildTarget);
            cloneBootConfig.ReadBootConfig();
            Assert.IsTrue(cloneBootConfig.TryGetValue("xr-sample-bootconfig-key02", out key02value));
            Assert.AreEqual(key02value, "primary value");
            Assert.IsTrue(cloneBootConfig.CheckValuePairExists("xr-sample-bootconfig-key02", "primary value"));
        }
    }
}
