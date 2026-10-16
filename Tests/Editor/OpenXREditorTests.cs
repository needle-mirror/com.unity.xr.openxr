using System;
using NUnit.Framework;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR;

namespace UnityEditor.XR.OpenXR.Tests
{
    [Category("Level.Unit")]
    [Property("Covers", "Settings")]
    internal class OpenXREditorTests
    {
        [Test]
        [Description("A feature's InternalDocumentationLink embeds the package's current major.minor " +
                     "version, so documentation links track the installed package.")]
        public void DocumentationVersion()
        {
            var version = PackageManager.PackageInfo.FindForAssembly(typeof(OpenXREditorTests).Assembly)?.version;
            var majorminor = "@" + OpenXRFeatureAttribute.k_PackageVersionRegex.Match(version).Groups[1].Value + "/";
            UnityEngine.Debug.Log(typeof(KHRSimpleControllerProfile).GetCustomAttribute<OpenXRFeatureAttribute>().InternalDocumentationLink);
            Assert.IsTrue(typeof(KHRSimpleControllerProfile).GetCustomAttribute<OpenXRFeatureAttribute>().InternalDocumentationLink.Contains(majorminor));
        }
    }
}
