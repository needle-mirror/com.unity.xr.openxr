using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Tests;

namespace UnityEditor.XR.OpenXR.Tests
{
    // Level carried as a Property, not a Category: the only test here is [Explicit], and a
    // Level.Build category sweep would count as an explicit match and run it.
    [NUnit.Framework.Property("PyramidLevel", "Build")]
    [NUnit.Framework.Property("Covers", "Features")]
    [NUnit.Framework.Property("Covers", "ProjectValidation")]
    class FeatureModifyingTests : OpenXRLoaderSetup
    {
        // Override AfterTest to prevent OpenXRSettings.Instance.features from getting reset.
        // This test suite destroys and restores OpenXRSettings.Instance.features manually.
        public override void AfterTest()
        { }

        [Test]
        [Category("BuildUnitTest"), Explicit]
        [Description("Swapping in a package-settings asset that holds duplicate feature entries is flagged " +
                     "by AssetHasNoDuplicates, and RegenerateXRPackageSettingsAsset yields a clean asset.")]
        public void DuplicateSettingAssetTest()
        {
            // Store original file paths and backup
            string settingsFilePath = OpenXRPackageSettings.OpenXRPackageSettingsAssetPath();
            string metaFilePath = settingsFilePath + ".meta";

            string backupSettingsPath = settingsFilePath + ".backup";
            string backupMetaPath = metaFilePath + ".backup";

            // Backup original files
            if (File.Exists(settingsFilePath))
                File.Copy(settingsFilePath, backupSettingsPath, true);
            if (File.Exists(metaFilePath))
                File.Copy(metaFilePath, backupMetaPath, true);

            try
            {
                string openXRFolder = Path.GetFullPath("Packages/com.unity.xr.openxr");
                string testAssetName = "OpenXR Package Settings With Duplicates.testasset";
                string testAssetPath = Path.Combine(openXRFolder, "Tests", "Editor", testAssetName);
                string testMetaAssetPath = testAssetPath + ".meta";

                // Copy in the test files
                File.Delete(settingsFilePath);
                File.Delete(metaFilePath);
                File.Copy(testAssetPath, settingsFilePath);
                File.Copy(testMetaAssetPath, metaFilePath);

                // Force AssetDatabase to recognize the new file
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(settingsFilePath, ImportAssetOptions.ForceUpdate);

                // Manually register the copied asset as a config object
                var copiedAsset = AssetDatabase.LoadAssetAtPath<OpenXRPackageSettings>(settingsFilePath);
                if (copiedAsset != null)
                {
                    EditorBuildSettings.AddConfigObject(Constants.k_SettingsKey, copiedAsset, true);
                }

                // Force refresh again to ensure everything is loaded
                AssetDatabase.Refresh();

                // Now the assertions should work
                Assert.IsFalse(OpenXRProjectValidation.AssetHasNoDuplicates(),
                    "The duplicate settings on the bad asset should be detected.");

                // Regenerate the asset
                OpenXRProjectValidation.RegenerateXRPackageSettingsAsset();

                // Verify no duplicates after regeneration
                Assert.IsTrue(OpenXRProjectValidation.AssetHasNoDuplicates(),
                    "After regenerating the asset, the duplicate settings should be removed.");
            }
            finally
            {
                // Always restore original files
                if (File.Exists(backupSettingsPath))
                {
                    File.Copy(backupSettingsPath, settingsFilePath, true);
                    File.Delete(backupSettingsPath);
                }
                if (File.Exists(backupMetaPath))
                {
                    File.Copy(backupMetaPath, metaFilePath, true);
                    File.Delete(backupMetaPath);
                }

                // Restore the original config object registration
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(settingsFilePath, ImportAssetOptions.ForceUpdate);

                var originalAsset = AssetDatabase.LoadAssetAtPath<OpenXRPackageSettings>(settingsFilePath);
                if (originalAsset != null)
                {
                    EditorBuildSettings.AddConfigObject(Constants.k_SettingsKey, originalAsset, true);
                }

                AssetDatabase.Refresh();
            }
        }
    }
}
