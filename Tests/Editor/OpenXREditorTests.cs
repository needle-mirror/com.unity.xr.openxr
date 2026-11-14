using System;
using NUnit.Framework;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.Management;
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
            var version = PackageManager.PackageInfo.FindForPackageName("com.unity.xr.openxr")?.version;
            Assert.That(version, Is.Not.Null.And.Not.Empty);
            var majorminor = "@" + OpenXRFeatureAttribute.k_PackageVersionRegex.Match(version).Groups[1].Value + "/";
            UnityEngine.Debug.Log(typeof(KHRSimpleControllerProfile).GetCustomAttribute<OpenXRFeatureAttribute>().InternalDocumentationLink);
            Assert.IsTrue(typeof(KHRSimpleControllerProfile).GetCustomAttribute<OpenXRFeatureAttribute>().InternalDocumentationLink.Contains(majorminor));
        }

        [TestCase(BuildTargetGroup.Standalone, true)]
        [TestCase(BuildTargetGroup.Android, true)]
        [TestCase(BuildTargetGroup.VisionOS, false)]
        [TestCase(BuildTargetGroup.iOS, false)]
        [Description("IsBuildTargetGroupSupported returns true only for build target groups listed in the OpenXR " +
                     "loader metadata, so builds skip the OpenXR loader on build target groups it does not support.")]
        public void IsBuildTargetGroupSupported(BuildTargetGroup buildTargetGroup, bool expected)
        {
            Assert.AreEqual(expected, OpenXRManagementSettings.IsBuildTargetGroupSupported(buildTargetGroup));
        }

        [Test]
        [Description("RemoveActiveLoaders removes every OpenXR loader, including OpenXRLoaderNoPreInit, from an " +
                     "XR Plug-in Management loader list, so builds can drop the OpenXR loader from unsupported " +
                     "build target groups.")]
        public void RemoveActiveLoadersRemovesOpenXRLoaders()
        {
            var manager = UnityEngine.ScriptableObject.CreateInstance<XRManagerSettings>();
            var loader = UnityEngine.ScriptableObject.CreateInstance<OpenXRLoader>();
            var noPreInitLoader = UnityEngine.ScriptableObject.CreateInstance<OpenXRLoaderNoPreInit>();
            try
            {
                Assert.IsTrue(manager.TryAddLoader(loader));
                Assert.IsTrue(manager.TryAddLoader(noPreInitLoader));

                BuildHelperUtils.RemoveActiveLoaders(manager, typeof(OpenXRLoaderBase));

                Assert.AreEqual(0, manager.activeLoaders.Count);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(noPreInitLoader);
                UnityEngine.Object.DestroyImmediate(loader);
                UnityEngine.Object.DestroyImmediate(manager);
            }
        }
    }
}
