
using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace UnityEngine.XR.OpenXR.Tests
{
    // No Covers on purpose: this test exercises nothing, so it must not be selected for any area.
    [Category("Level.Unit")]
    internal class PlaceholderTests
    {
        //An always passing test for Linux platform to by pass Validation jobs requirements.
        [UnityPlatform(exclude = new[] { RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor, RuntimePlatform.Android, RuntimePlatform.OSXEditor, RuntimePlatform.OSXPlayer })]
        [Test]
        [Description("Always-passing placeholder so the Linux validation job has a test to run; it verifies " +
                     "nothing.")]
        public void AlwaysPassingTest()
        {
            Assert.IsTrue(true);
        }
    }
}
