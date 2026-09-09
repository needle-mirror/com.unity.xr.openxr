using UnityEngine.XR.OpenXR.Features;

namespace UnityEngine.XR.OpenXR.Tests
{
#if UNITY_EDITOR
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(
        UiName = "Mock All Build Target Groups Feature",
        Company = "Unity",
        Desc = "Mock feature that omits BuildTargetGroups, so that it is supported on every build target group.",
        Version = "0.0.1",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Feature,
        Hidden = true,
        FeatureId = MockAllBuildTargetGroupsFeature.k_FeatureId)]
#endif
    class MockAllBuildTargetGroupsFeature : OpenXRFeature
    {
        internal const string k_FeatureId = "com.unity.openxr.tests.feature.allbuildtargetgroups";
    }
}
