#if SAMPLE_IMPORT_EVENT_AVAILABLE
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;

namespace UnityEditor.XR.OpenXR
{
    static class OpenXRSampleDependencyImporter
    {
        const string k_PackageName = "com.unity.xr.openxr";
        const string k_StarterAssetsSampleName = "Starter Assets";

        static readonly string[] k_SamplesRequiringStarterAssets =
        {
            "Controller",
            "Graphics",
            "Tracking Origin"
        };

        [InitializeOnLoadMethod]
        static void RegisterSampleImportHandler()
        {
            Sample.OnBeforeImportFinish -= OnBeforeImportFinish;
            Sample.OnBeforeImportFinish += OnBeforeImportFinish;
        }

        static void OnBeforeImportFinish(IReadOnlyList<SampleImportEventData> importedSamples)
        {
            var starterAssetsRequired = false;
            foreach (var importedSample in importedSamples)
            {
                if (importedSample.packageTechnicalName == k_PackageName &&
                    Array.IndexOf(k_SamplesRequiringStarterAssets, importedSample.sampleDisplayName) >= 0)
                {
                    starterAssetsRequired = true;
                    break;
                }
            }

            if (!starterAssetsRequired)
                return;

            foreach (var sample in Sample.FindByPackage(k_PackageName, string.Empty))
            {
                if (sample.displayName != k_StarterAssetsSampleName)
                    continue;

                if (!sample.isImported)
                    sample.Import(Sample.ImportOptions.SkipAssetDatabaseRefresh);

                return;
            }
        }
    }
}
#endif
