using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEngine.XR.OpenXR
{
    public partial class OpenXRSettings
    {
        /// <summary>
        /// Stereo rendering mode.
        /// </summary>
        public enum RenderMode
        {
            /// <summary>
            /// Submit separate draw calls for each eye.
            /// </summary>
            MultiPass,

            /// <summary>
            /// Submit one draw call for both eyes.
            /// </summary>
            SinglePassInstanced,
        };

        /// <summary>
        /// Stereo rendering mode.
        /// </summary>
        [SerializeField]
        private RenderMode m_renderMode = RenderMode.SinglePassInstanced;

        /// <summary>
        /// Runtime Stereo rendering mode.
        /// </summary>
        public RenderMode renderMode
        {
            get
            {
                if (OpenXRLoaderBase.Instance != null)
                    return Internal_GetRenderMode();
                else
                    return m_renderMode;
            }
            set
            {
                if (OpenXRLoaderBase.Instance != null)
                    Internal_SetRenderMode(value);
                else
                    m_renderMode = value;
            }
        }

        [SerializeField]
        private bool m_autoColorSubmissionMode = true;

        /// <summary>
        /// Automatically select the default color submission mode.
        /// </summary>
        public bool autoColorSubmissionMode
        {
            get => m_autoColorSubmissionMode;
            set => m_autoColorSubmissionMode = value;
        }

        [SerializeField]
        private ColorSubmissionModeList m_colorSubmissionModes = new ColorSubmissionModeList();

        /// <summary>
        /// Gets or sets the preference for what color format the OpenXR display swapchain should use.
        /// </summary>
        public ColorSubmissionModeGroup[] colorSubmissionModes
        {
            get
            {
                if (m_autoColorSubmissionMode)
                    return new[] { OpenXRSettings.kDefaultColorMode };

                if (OpenXRLoaderBase.Instance != null)
                {
                    int arraySize = Internal_GetColorSubmissionModes(null, 0);
                    int[] intModes = new int[arraySize];
                    Internal_GetColorSubmissionModes(intModes, arraySize);
                    return intModes.Select(i => (ColorSubmissionModeGroup)i).ToArray();
                }
                else
                    return m_colorSubmissionModes.m_List;
            }
            set
            {
                if (OpenXRLoaderBase.Instance != null)
                    Internal_SetColorSubmissionModes(
                        value.Select(e => (int)e).ToArray(),
                        value.Length
                    );
                else
                    m_colorSubmissionModes.m_List = value;
            }
        }

        /// <summary>
        /// Runtime Depth submission mode.
        /// </summary>
        public enum DepthSubmissionMode
        {
            /// <summary>
            /// No depth is submitted to the OpenXR compositor.
            /// </summary>
            None,

            /// <summary>
            /// 16-bit depth is submitted to the OpenXR compositor.
            /// </summary>
            Depth16Bit,

            /// <summary>
            /// 24-bit depth is submitted to the OpenXR compositor.
            /// </summary>
            Depth24Bit,
        }

        /// <summary>
        /// Different Internal Unity APIs to use in the backend.
        /// </summary>
        public enum BackendFovationApi : byte
        {
            /// <summary>
            /// Use the legacy Built-in API for Foveation. This is the only option for Built-in.
            /// </summary>
            Legacy = 0,

            /// <summary>
            /// Use the Foveation for SRP API.
            /// </summary>
            SRPFoveation = 1,
        }

#if UNITY_ANDROID

        private string[] m_eyeTrackingPermissionsToRequest = new string[]
        {
            "com.oculus.permission.EYE_TRACKING",
            "android.permission.EYE_TRACKING",
            "android.permission.EYE_TRACKING_FINE"
        };
#endif

        /// <summary>
        /// Enables XR_KHR_composition_layer_depth if possible and resolves or submits depth to OpenXR runtime.
        /// </summary>
        [SerializeField]
        private DepthSubmissionMode m_depthSubmissionMode = DepthSubmissionMode.None;

        static void PermissionGrantedCallback(string permissionName)
        {
#if UNITY_ANDROID
            foreach (var permission in GetInstance(true).m_eyeTrackingPermissionsToRequest)
            {
                if (permissionName == permission)
                {
                    Internal_SetHasEyeTrackingPermissions(true);
                }
            }
#endif
        }

        /// <summary>
        /// Enables XR_KHR_composition_layer_depth if possible and resolves or submits depth to OpenXR runtime.
        /// </summary>
        public DepthSubmissionMode depthSubmissionMode
        {
            get
            {
                if (OpenXRLoaderBase.Instance != null)
                    return Internal_GetDepthSubmissionMode();
                else
                    return m_depthSubmissionMode;
            }
            set
            {
                if (OpenXRLoaderBase.Instance != null)
                    Internal_SetDepthSubmissionMode(value);
                else
                    m_depthSubmissionMode = value;
            }
        }

        [SerializeField]
        private bool m_optimizeBufferDiscards = false;

        /// <summary>
        /// Optimization that allows 4x MSAA textures to be memoryless on Vulkan
        /// </summary>
        public bool optimizeBufferDiscards
        {
            get { return m_optimizeBufferDiscards; }
            set
            {
                if (OpenXRLoaderBase.Instance != null)
                    Internal_SetOptimizeBufferDiscards(value);
                else
                    m_optimizeBufferDiscards = value;
            }
        }

        private void ApplyRenderSettings()
        {
            Internal_SetSymmetricProjection(m_symmetricProjection);
#if UNITY_2023_2_OR_NEWER
            Internal_SetUsedFoveatedRenderingApi(m_foveatedRenderingApi);
#endif
            Internal_SetRenderMode(m_renderMode);
            Internal_SetColorSubmissionModes(
                m_colorSubmissionModes.m_List.Select(e => (int)e).ToArray(),
                m_colorSubmissionModes.m_List.Length
            );
            Internal_SetDepthSubmissionMode(m_depthSubmissionMode);
            Internal_SetOptimizeBufferDiscards(m_optimizeBufferDiscards);

#if UNITY_ANDROID // Only Android need specific permissions for eye tracking, for now
            var foveatedRenderingFreature = GetFeature<FoveatedRenderingFeature>();
            if (m_eyeTrackingPermissionsToRequest != null && m_eyeTrackingPermissionsToRequest.Length > 0
                && (Internal_HasRequestedEyeTrackingPermissions()
                    || (foveatedRenderingFreature != null && foveatedRenderingFreature.enabled)))
            {
                var permissionCallbacks = new PermissionCallbacks();
                permissionCallbacks.PermissionGranted += PermissionGrantedCallback;
                Permission.RequestUserPermissions(m_eyeTrackingPermissionsToRequest, permissionCallbacks);
            }
#endif
        }


        [SerializeField]
        private bool m_symmetricProjection = false;

#if UNITY_2023_2_OR_NEWER
        [SerializeField]
        private BackendFovationApi m_foveatedRenderingApi = BackendFovationApi.Legacy;
#endif

        /// <summary>
        /// If enabled, when the application begins it will create a stereo symmetric view that has the eye buffer resolution change based on the IPD.
        /// Provides a performance benefit across all IPDs.
        /// </summary>
        public bool symmetricProjection
        {
            get { return m_symmetricProjection; }
            set
            {
                if (OpenXRLoaderBase.Instance != null)
                    Internal_SetSymmetricProjection(value);
                else
                    m_symmetricProjection = value;
            }
        }

        /// <summary>
        /// Different APIs to use in the backend.
        /// On Built-in Render Pipeline, only Legacy will be used.
        /// On Scriptable Render Pipelines, it is highly recommended to use the SRPFoveation API. More textures will use FDM with the SRPFoveation API.
        /// </summary>
#if UNITY_2023_2_OR_NEWER
        public BackendFovationApi foveatedRenderingApi
        {
            get
            {
                if (OpenXRLoaderBase.Instance != null)
                    return Internal_GetUsedFoveatedRenderingApi();
                else
                    return m_foveatedRenderingApi;
            }
            set
            {
                if (OpenXRLoaderBase.Instance != null)
                    Internal_SetUsedFoveatedRenderingApi(value);
                else
                    m_foveatedRenderingApi = value;
            }
        }
#else
        public BackendFovationApi foveatedRenderingApi => BackendFovationApi.Legacy;
#endif

        private const string LibraryName = "UnityOpenXR";

        [DllImport(LibraryName, EntryPoint = "NativeConfig_SetRenderMode")]
        private static extern void Internal_SetRenderMode(RenderMode renderMode);

        [DllImport(LibraryName, EntryPoint = "NativeConfig_GetRenderMode")]
        private static extern RenderMode Internal_GetRenderMode();

        [DllImport(LibraryName, EntryPoint = "NativeConfig_SetDepthSubmissionMode")]
        private static extern void Internal_SetDepthSubmissionMode(
            DepthSubmissionMode depthSubmissionMode
        );

        [DllImport(LibraryName, EntryPoint = "NativeConfig_GetDepthSubmissionMode")]
        private static extern DepthSubmissionMode Internal_GetDepthSubmissionMode();

        [DllImport(LibraryName, EntryPoint = "NativeConfig_SetSymmetricProjection")]
        private static extern void Internal_SetSymmetricProjection([MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(LibraryName, EntryPoint = "NativeConfig_SetOptimizeBufferDiscards")]
        private static extern void Internal_SetOptimizeBufferDiscards([MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(LibraryName, EntryPoint = "OculusFoveation_SetUsedApi")]
        private static extern void Internal_SetUsedFoveatedRenderingApi(BackendFovationApi api);

        [DllImport(LibraryName, EntryPoint = "OculusFoveation_GetUsedApi")]
        internal static extern BackendFovationApi Internal_GetUsedFoveatedRenderingApi();

        [DllImport(LibraryName, EntryPoint = "OculusFoveation_HasRequestedEyeTrackingPermissions")]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool Internal_HasRequestedEyeTrackingPermissions();

        [DllImport(LibraryName, EntryPoint = "OculusFoveation_GetHasEyeTrackingPermissions")]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool Internal_GetHasEyeTrackingPermissions();

        [DllImport(LibraryName, EntryPoint = "OculusFoveation_SetHasEyeTrackingPermissions")]
        internal static extern void Internal_SetHasEyeTrackingPermissions([MarshalAs(UnmanagedType.I1)] bool value);

        [DllImport(LibraryName, EntryPoint = "NativeConfig_SetColorSubmissionMode")]
        private static extern void Internal_SetColorSubmissionMode(
            ColorSubmissionModeGroup[] colorSubmissionMode
        );

        [DllImport(LibraryName, EntryPoint = "NativeConfig_SetColorSubmissionModes")]
        private static extern void Internal_SetColorSubmissionModes(
            int[] colorSubmissionMode,
            int arraySize
        );

        [DllImport(LibraryName, EntryPoint = "NativeConfig_GetColorSubmissionModes")]
        private static extern int Internal_GetColorSubmissionModes(
            [Out] int[] colorSubmissionMode,
            int arraySize
        );

        [DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetIsUsingLegacyXRDisplay")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Internal_GetIsUsingLegacyXRDisplay();
    }
}
