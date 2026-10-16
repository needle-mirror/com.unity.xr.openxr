using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;

namespace UnityEngine.XR.OpenXR.CodeSamples.Editor.Tests
{
    public static class PerformanceSettingsSnippets
    {
        static void ExampleHolder()
        { // This method is a placeholder to hold the code snippets.
            #region PowerSavingsHintExample
            // using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;

            // Set power savings hint for CPU and GPU
            XrPerformanceSettingsFeature.SetPerformanceLevelHint(
                PerformanceDomain.Cpu,
                PerformanceLevelHint.PowerSavings);
            XrPerformanceSettingsFeature.SetPerformanceLevelHint(
                PerformanceDomain.Gpu,
                PerformanceLevelHint.PowerSavings);

            // Show a picture sequence and continue with app loading
            #endregion

            #region SustainedLowHintExample
            // using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;

            // Set sustained low hint for CPU and GPU
            XrPerformanceSettingsFeature.SetPerformanceLevelHint(
                PerformanceDomain.Cpu,
                PerformanceLevelHint.SustainedLow);
            XrPerformanceSettingsFeature.SetPerformanceLevelHint(
                PerformanceDomain.Gpu,
                PerformanceLevelHint.SustainedLow);

            // Run regular app process
            #endregion

            #region SustainedHighHintExample
            // using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;

            // Set sustained high hint for GPU
            XrPerformanceSettingsFeature.SetPerformanceLevelHint(
                PerformanceDomain.Gpu,
                PerformanceLevelHint.SustainedHigh);

            // Load and process action scene
            #endregion

            #region BoostHintExample
            // using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;

            // Set boost hint for CPU
            XrPerformanceSettingsFeature.SetPerformanceLevelHint(
                PerformanceDomain.Cpu,
                PerformanceLevelHint.Boost);

            // Run complex logic
            #endregion
        }
    }
}
// Used in Documentation~/features/performance-settings.md
// This file contains code snippets for performance level hints.
