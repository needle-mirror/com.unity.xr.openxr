#if XR_HANDS_1_9_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Meta;
using Unity.XR.Management.AndroidManifest.Editor;

namespace UnityEditor.XR.OpenXR.Features.Meta
{
    class ModifyAndroidManifestHandMeshData : OpenXRFeatureBuildHooks
    {
        public override int callbackOrder => 1;
        public override Type featureType => typeof(MetaOpenXRHandMeshData);

        protected override void OnPreprocessBuildExt(BuildReport report) { }
        protected override void OnPostGenerateGradleAndroidProjectExt(string path) { }
        protected override void OnPostprocessBuildExt(BuildReport report) { }

        protected override ManifestRequirement ProvideManifestRequirementExt()
        {
            var androidOpenXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            var elementsToAdd = new List<ManifestElement>();

            var handMeshDataFeature = androidOpenXRSettings.GetFeature<MetaOpenXRHandMeshData>();
            if (handMeshDataFeature != null && handMeshDataFeature.enabled)
            {
                elementsToAdd.Add(
                    new ManifestElement
                    {
                        ElementPath = new List<string> { "manifest", "uses-permission" },
                        Attributes = new Dictionary<string, string>
                        {
                            { "name", "com.oculus.permission.HAND_TRACKING" },
                        }
                    }
                );

                elementsToAdd.Add(
                    new ManifestElement
                    {
                        ElementPath = new List<string> { "manifest", "uses-permission" },
                        Attributes = new Dictionary<string, string>
                        {
                            { "name", "android.permission.HAND_TRACKING" },
                        }
                    }
                );

                elementsToAdd.Add(
                    new ManifestElement
                    {
                        ElementPath = new List<string> { "manifest", "uses-feature" },
                        Attributes = new Dictionary<string, string>
                        {
                            { "name", "oculus.software.handtracking" },
                            { "required", "false" },
                        }
                    }
                );
            }

            return new ManifestRequirement
            {
                SupportedXRLoaders = new HashSet<Type>()
                {
                    typeof(OpenXRLoader)
                },
                NewElements = elementsToAdd
            };
        }
    }
}
#endif // XR_HANDS_1_9_OR_NEWER
