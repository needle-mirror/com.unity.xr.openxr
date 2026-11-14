#if XR_MGMT_3_2_0_OR_NEWER
using System.Collections.Generic;
using UnityEngine.XR.OpenXR;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.OpenXR
{
    public class OpenXRManagementSettings : IXRPackage
    {
        private class MyLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        private class MyPackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; }
        }

#if LIFECYCLE_APIS_AVAILABLE
        // Static configuration/metadata value, never mutated at runtime.
        [NoAutoStaticsCleanup]
#endif
        private static IXRPackageMetadata s_Metadata = new MyPackageMetadata()
        {
            packageName = "OpenXR XR Plugin",
            packageId = "com.unity.xr.openxr",
            settingsType = "UnityEditor.XR.OpenXR.OpenXRPackageSettings",
            loaderMetadata = new List<IXRLoaderMetadata>()
            {
                new MyLoaderMetadata()
                {
                    loaderName = "OpenXR Loader",
                    loaderType = "UnityEngine.XR.OpenXR.OpenXRLoader",
                    supportedBuildTargets = new List<BuildTargetGroup>()
                    {
                        BuildTargetGroup.Standalone,
                        BuildTargetGroup.Android
                    },
                },
            }
        };


        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            try
            {
                return OpenXRPackageSettings.RegisterOpenXRPackageSettings(obj);
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Error adding new OpenXR Settings object to build settings.\n{ex.Message}");
            }

            return false;
        }

        public IXRPackageMetadata metadata => s_Metadata;

        internal static string PackageId => s_Metadata.packageId;

        internal static bool IsBuildTargetGroupSupported(BuildTargetGroup buildTargetGroup) =>
            s_Metadata.loaderMetadata[0].supportedBuildTargets.Contains(buildTargetGroup);
    }
}
#endif
