using System;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Rendering;

namespace UnityEngine.XR.OpenXR.Features
{
    /// <summary>
    /// This <see cref="OpenXRFeature"/> enables the use of foveated rendering in OpenXR.
    /// </summary>
#if UNITY_EDITOR && UNITY_2023_2_OR_NEWER
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "Foveated Rendering",
        BuildTargetGroups = new []{BuildTargetGroup.Standalone, BuildTargetGroup.WSA, BuildTargetGroup.Android},
        Company = "Unity",
        Desc = "Add foveated rendering.",
        DocumentationLink = Constants.k_DocumentationManualURL + "features/foveatedrendering.html",
        OpenxrExtensionStrings = "XR_UNITY_foveation XR_FB_foveation XR_FB_foveation_configuration XR_FB_swapchain_update_state XR_FB_foveation_vulkan XR_META_foveation_eye_tracked XR_META_vulkan_swapchain_create_info",
        Version = "1",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Feature,
        FeatureId = featureId)]
#endif
    public class FoveatedRenderingFeature : OpenXRFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.foveatedrendering";

        /// <inheritdoc />
        protected internal override bool OnInstanceCreate(ulong instance)
        {
            // If using BiRP, the feature must know not to use the newer API for FSR/FDM
            Internal_Unity_SetUseFoveatedRenderingLegacyMode(GraphicsSettings.defaultRenderPipeline == null);
            return base.OnInstanceCreate(instance);
        }

        /// <inheritdoc />
        protected internal override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            return Internal_Unity_intercept_xrGetInstanceProcAddr(func);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        private const string Library = "UnityOpenXR";

        [DllImport(Library, EntryPoint = "UnityFoveation_intercept_xrGetInstanceProcAddr")]
        private static extern IntPtr Internal_Unity_intercept_xrGetInstanceProcAddr(IntPtr func);

        [DllImport(Library, EntryPoint = "UnityFoveation_SetUseFoveatedRenderingLegacyMode")]
        private static extern void Internal_Unity_SetUseFoveatedRenderingLegacyMode([MarshalAs(UnmanagedType.I1)] bool value);

        [DllImport(Library, EntryPoint = "UnityFoveation_GetUseFoveatedRenderingLegacyMode")]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool Internal_Unity_GetUseFoveatedRenderingLegacyMode();
    }
}
