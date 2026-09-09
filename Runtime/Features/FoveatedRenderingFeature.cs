using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Rendering;
using UnityEngine.XR.OpenXR.NativeTypes;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.OpenXR.Features
{
    /// <summary>
    /// This <see cref="OpenXRFeature"/> enables the use of foveated rendering in OpenXR.
    /// </summary>
#if UNITY_EDITOR && UNITY_2023_2_OR_NEWER
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "Foveated Rendering",
        BuildTargetGroups = new []{BuildTargetGroup.Standalone, BuildTargetGroup.Android},
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

        /// <summary>
        /// Get whether Vulkan subsampled layout is currently enabled.
        /// </summary>
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        public static bool isSubsampledLayoutEnabled { get; private set; }

        [SerializeField]
        bool enableSubsampledLayout;

        /// <summary>
        /// When enabled, the eye tracking OpenXR extension (<c>XR_META_foveation_eye_tracked</c>) is
        /// requested and eye tracking android permissions / manifest flags are written at build time, if the device supports it
        /// Disable this if you want foveated rendering without eye tracking.
        /// Enabled by default so existing projects are unaffected.
        /// </summary>
        [SerializeField]
        bool useEyeTracking = true;

        /// <summary>
        /// Gets or sets whether eye tracking is used for foveated rendering.
        /// When <see langword="false"/>, the eye tracking extension is not requested and
        /// eye tracking android permissions are not added to the manifest.
        /// </summary>
        public bool UseEyeTracking => useEyeTracking;

        private const string k_EyeTrackingExtension = "XR_META_foveation_eye_tracked";

        [SerializeField]
        bool m_SkipFDMForFinalPassesSRP;

        [SerializeField]
        bool m_SkipFDMForFinalPassesSRPOverridden;

        /// <summary>
        /// Controls whether a Fragment Density Map (FDM) is attached to final render passes such as URP's FinalBlit
        /// and UberPost, when using the SRP Foveation API. This has no effect on the Legacy foveated rendering API or
        /// on Quad Views.
        /// </summary>
        /// <remarks>
        /// On Meta Quest, attaching an FDM to a render pass forces the driver into binning mode, which has a lot of
        /// setup overhead regardless of how complex the pass actually is. Skipping it lets the driver use direct
        /// rendering mode for those passes instead. AndroidXR has its own approach to foveating the UberPost
        /// pass and wants FDM kept attached there, so unless explicitly set via script, this defaults to
        /// <see langword="true"/> only when the Meta Quest build profile is the confirmed active build profile, and
        /// <see langword="false"/> otherwise - including for AndroidXR, Standalone, and the generic Android build
        /// profile, since there is no reliable way to confirm a Meta device is in use in that last case.
        /// Set this via script if you need to override the default.
        /// Setting this immediately forwards the value to the XR Display Subsystem if it has already been created; if
        /// it has not (for example, if this is set before the OpenXR loader has started), the native call safely
        /// no-ops and the value is (re)applied once the subsystem is created, so it still takes effect for that run.
        /// Note that Meta Quest's driver may still use binning mode for these passes regardless of this setting if MSAA is enabled.
        /// </remarks>
        public bool skipFDMForFinalPassesSRP
        {
            get => m_SkipFDMForFinalPassesSRPOverridden
                ? m_SkipFDMForFinalPassesSRP
                : DefaultSkipFDMForFinalPassesSRP;
            set
            {
                m_SkipFDMForFinalPassesSRP = value;
                m_SkipFDMForFinalPassesSRPOverridden = true;
                Internal_SetSkipFDMForFinalPasses(value);
            }
        }

        // FDM is skipped on final passes specifically for Meta Quest devices.
        // AndroidXR wants FDM kept attached for its own UberPost foveation approach.
        // Users can still override this via script on any platform.
#if UNITY_META_QUEST
        static bool DefaultSkipFDMForFinalPassesSRP => true;
#else
        static bool DefaultSkipFDMForFinalPassesSRP => false;
#endif

        [SerializeField]
        internal bool m_EnableDynamicFoveation;

        /// <summary>
        /// Gets or sets whether the OpenXR runtime is allowed to vary the amount of foveation it
        /// applies, up to the foveation level your application sets.
        /// </summary>
        /// <remarks>
        /// The foveation level itself is not controlled by this property. Set it with
        /// <see cref="UnityEngine.XR.XRDisplaySubsystem.foveatedRenderingLevel"/>. When dynamic
        /// foveation is disabled, the runtime applies that level constantly. When it is enabled,
        /// the level becomes a maximum and the runtime applies less foveation when there is GPU
        /// headroom to spare.
        ///
        /// Because the level acts as a maximum, dynamic foveation has no effect while the
        /// foveation level is <c>0</c>, which is the default.
        ///
        /// Dynamic foveation requires the Vulkan graphics API, an OpenXR runtime that supports it,
        /// and the <b>Foveated Rendering Method</b> set to <b>Foveated rendering (SRP API)</b>. It
        /// has no effect with the Legacy API or Quad Views.
        /// </remarks>
        public bool DynamicFoveationEnabled
        {
            get
            {
                if (OpenXRLoaderBase.Instance == null)
                    return m_EnableDynamicFoveation;

                var result = Internal_GetFbFoveationDynamic(out var useFoveationDynamic);
                if (result != XrResult.Success)
                {
                    Debug.LogWarning($"Failed to read the dynamic foveation state ({result}).");
                    return false;
                }

                return useFoveationDynamic == XrFoveationDynamicFB.LevelEnabled;
            }
            set
            {
                if (OpenXRLoaderBase.Instance != null)
                    ApplyDynamicFoveation(value);
                else
                    m_EnableDynamicFoveation = value;
            }
        }

        ulong m_XrSession;

#if UNITY_EDITOR
        private bool SettingsUseVulkan()
        {
            if (!PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android))
            {
                GraphicsDeviceType[] apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                if (apis.Length >= 1 && apis[0] == GraphicsDeviceType.Vulkan)
                {
                    return true;
                }
                return false;
            }

            return true;
        }

        protected internal override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
#if UNITY_ANDROID
            rules.Add(new ValidationRule(this)
            {
                message = "Subsampled Layout is only supported on Vulkan graphics API",
                checkPredicate = () =>
                {
                    if (enableSubsampledLayout && !SettingsUseVulkan())
                    {
                        return false;
                    }
                    return true;
                },
                fixIt = () =>
                {
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
                },
                error = true,
                fixItAutomatic = true,
                fixItMessage = "Set Vulkan as Graphics API"
            });
#endif
#if UNITY_6000_0_OR_NEWER
            rules.Add(new ValidationRule(this)
            {
                message = "Only Legacy Foveated Rendering API usage is possible on Built-in Render Pipeline",
                checkPredicate = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    return GraphicsSettings.currentRenderPipeline != null || currentSettings.foveatedRenderingApi == OpenXRSettings.BackendFovationApi.Legacy;
                },
                fixIt = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    currentSettings.foveatedRenderingApi = OpenXRSettings.BackendFovationApi.Legacy;
                },
                error = true,
                fixItAutomatic = true,
                fixItMessage = "Set Foveated Rendering Method to Foveated Rendering (Legacy)"
            });
#endif

#if UNITY_ANDROID_XR && UNITY_2023_2_OR_NEWER
            // Have a rule that gives the user a warning if they're using the AndroidXR Build Profile.
            rules.Add(new ValidationRule(this)
            {
                message = "Quad Views is not supported for the AndroidXR Build Profile.",
                checkPredicate = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    return currentSettings.foveatedRenderingApi != OpenXRSettings.BackendFovationApi.QuadViews;
                },
                fixIt = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    currentSettings.foveatedRenderingApi = OpenXRSettings.BackendFovationApi.SRPFoveation;
                },
                error = true,
                fixItAutomatic = true,
                fixItMessage = "Set Foveated Rendering Method to Foveated Rendering (SRP API)"
            });
#endif
            rules.Add(new ValidationRule(this)
            {
                message = "Quad Views is only supported with the \"Single-Pass Instanced \\ Multi-view\" Render Mode.",
                helpText = "Use a different foveation method, switch to \"Single-Pass Instanced \\ Multi-view\" Render Mode, or disable the Foveated Rendering feature.",
                checkPredicate = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    if (currentSettings == null) return true;

                    if (currentSettings.foveatedRenderingApi == OpenXRSettings.BackendFovationApi.QuadViews)
                    {
                        return currentSettings.renderMode == OpenXRSettings.RenderMode.SinglePassInstanced;
                    }

                    return true;
                },
                error = true,
                fixIt = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    currentSettings.renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;
                }
            });

            rules.Add(new ValidationRule(this)
            {
                message = "Dynamic foveation is only supported with the \"Foveated rendering (SRP API)\" foveation method.",
                helpText = "Use the \"Foveated rendering (SRP API)\" foveation method, or disable dynamic foveation.",
                checkPredicate = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    if (currentSettings == null) return true;

                    if (DynamicFoveationEnabled)
                        return currentSettings.foveatedRenderingApi == OpenXRSettings.BackendFovationApi.SRPFoveation;

                    return true;
                },
                fixIt = () =>
                {
                    var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    if (currentSettings != null)
                        currentSettings.foveatedRenderingApi = OpenXRSettings.BackendFovationApi.SRPFoveation;
                }
            });

        }

        [CustomEditor(typeof(FoveatedRenderingFeature))]
        internal class FoveatedRenderingFeatureEditor : Editor
        {
            private SerializedProperty subsampledLayout;
            private SerializedProperty useEyeTracking;
            SerializedProperty m_EnableDynamicFoveationProperty;
#if LIFECYCLE_APIS_AVAILABLE
            // Static UI label caches; content never changes at runtime.
            [NoAutoStaticsCleanup]
#endif
            static GUIContent s_SubsampledLayout = EditorGUIUtility.TrTextContent("Subsampled Layout (Vulkan)", "An optimization technique that can improve foveated rendering performance by optimizing eye texture sampling.");
#if LIFECYCLE_APIS_AVAILABLE
            [NoAutoStaticsCleanup]
#endif
            static GUIContent s_UseEyeTracking = EditorGUIUtility.TrTextContent("Use Eye Tracking", "When enabled, the eye tracking OpenXR extension is requested and eye tracking Android permissions are added to the manifest. Disable to use foveated rendering without eye tracking.");
#if LIFECYCLE_APIS_AVAILABLE
            [NoAutoStaticsCleanup]
#endif
            static GUIContent s_DynamicFoveationContent = EditorGUIUtility.TrTextContent("Dynamic Foveation (Vulkan)", "Allows the device to vary the amount of foveation it applies, up to the foveation level your application sets, to improve performance. Has no effect while the foveation level is 0.");

            private SerializedObject openXRSettings;
            private BuildTargetGroup selectedBuildSettings;
#if UNITY_6000_0_OR_NEWER
            private SerializedProperty foveatedRenderingApi;

#if LIFECYCLE_APIS_AVAILABLE
            [NoAutoStaticsCleanup]
#endif
            private static readonly GUIContent[] k_foveatedRenderingApiOptions = new GUIContent[2]
            {
                new GUIContent("Legacy"),
                new GUIContent("SRP Foveation"),
            };

#if LIFECYCLE_APIS_AVAILABLE
            [NoAutoStaticsCleanup]
#endif
#if UNITY_6000_5_OR_NEWER
            private static readonly GUIContent[] k_androidfoveatedRenderingApiOptions = new GUIContent[3]
            {
                new GUIContent("Foveated rendering (Legacy API)"),
                new GUIContent("Foveated rendering (SRP API)"),
                new GUIContent("Quad Views"),
            };
#else
            private static readonly GUIContent[] k_androidfoveatedRenderingApiOptions = new GUIContent[2]
            {
                new GUIContent("Foveated rendering (Legacy API)"),
                new GUIContent("Foveated rendering (SRP API)"),
            };
#endif

#if LIFECYCLE_APIS_AVAILABLE
            [NoAutoStaticsCleanup]
#endif
            private static readonly GUIContent k_foveatedRenderingApiLabel = new GUIContent("Foveated Rendering Method", "Choose the foveated rendering api.");
#endif

            void OnEnable()
            {
                subsampledLayout = serializedObject.FindProperty("enableSubsampledLayout");
                useEyeTracking = serializedObject.FindProperty("useEyeTracking");
                m_EnableDynamicFoveationProperty = serializedObject.FindProperty(nameof(m_EnableDynamicFoveation));

#if UNITY_6000_0_OR_NEWER
                selectedBuildSettings = EditorUserBuildSettings.selectedBuildTargetGroup;
                var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                openXRSettings = new SerializedObject(currentSettings);
                if (openXRSettings != null)
                {
                    foveatedRenderingApi = openXRSettings.FindProperty("m_foveatedRenderingApi");
                }
#endif
            }

            public override void OnInspectorGUI()
            {
#if UNITY_6000_0_OR_NEWER
                if (selectedBuildSettings != EditorUserBuildSettings.selectedBuildTargetGroup)
                {
                    selectedBuildSettings = EditorUserBuildSettings.selectedBuildTargetGroup;
                    openXRSettings = new SerializedObject(OpenXRSettings.GetSettingsForBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
                    if (openXRSettings != null)
                    {
                        foveatedRenderingApi = openXRSettings.FindProperty("m_foveatedRenderingApi");
                    }
                }
                openXRSettings.Update();
#endif

                EditorGUIUtility.labelWidth = 300.0f;
                serializedObject.Update();
                EditorGUILayout.PropertyField(subsampledLayout, s_SubsampledLayout);
                EditorGUILayout.PropertyField(useEyeTracking, s_UseEyeTracking);
#if UNITY_6000_0_OR_NEWER

                var currentSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                int newFoveatedRenderingApi;
                GUILayout.BeginHorizontal();
                if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Android)
                {
                    newFoveatedRenderingApi = EditorGUILayout.Popup(
                        k_foveatedRenderingApiLabel,
                        (int)currentSettings.foveatedRenderingApi,
                        k_androidfoveatedRenderingApiOptions
                    );
                }
                else
                {
                    newFoveatedRenderingApi = EditorGUILayout.Popup(
                        k_foveatedRenderingApiLabel,
                        (int)currentSettings.foveatedRenderingApi,
                        k_foveatedRenderingApiOptions
                    );
                }

                if (newFoveatedRenderingApi != (int)currentSettings.foveatedRenderingApi)
                {
                    currentSettings.foveatedRenderingApi = (OpenXRSettings.BackendFovationApi)newFoveatedRenderingApi;
                }

                GUILayout.EndHorizontal();

                var isSrpFoveation = currentSettings.foveatedRenderingApi == OpenXRSettings.BackendFovationApi.SRPFoveation;
                using (new EditorGUI.DisabledScope(!isSrpFoveation))
                {
                    EditorGUILayout.PropertyField(m_EnableDynamicFoveationProperty, s_DynamicFoveationContent);
                }

                if (!isSrpFoveation)
                {
                    EditorGUILayout.HelpBox("Dynamic foveation is only supported with SRP Foveation. Select SRP Foveation as the Foveated Rendering Method to use dynamic foveation.", MessageType.Info);
                }
#else
                EditorGUILayout.PropertyField(m_EnableDynamicFoveationProperty, s_DynamicFoveationContent);
#endif

                openXRSettings.ApplyModifiedProperties();
                serializedObject.ApplyModifiedProperties();

                EditorGUIUtility.labelWidth = 0.0f;
            }
        }
#endif

        /// <summary>
        /// Attempts to set whether subsampled layout is enabled.
        /// </summary>
        /// <param name="enableSubsampling">Indicates whether to enable subsampling.</param>
        /// <returns>
        /// Returns <see langword="true"/> if subsampling state was updated
        /// and returns <see langword="false"/> otherwise.
        /// </returns>
        public static bool TrySetSubsampledLayoutEnabled(bool enableSubsampling)
        {
            if (enableSubsampling)
            {
                if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan)
                {
                    Debug.LogError("Could not enable subsampling. Subsampled Layout is only supported on Vulkan graphics API.");
                    return false;
                }

                if (!OpenXRRuntime.IsExtensionEnabled("XR_META_vulkan_swapchain_create_info"))
                {
                    Debug.LogError("$Could not enable Vulkan subsampling. OpenXR extension XR_META_vulkan_swapchain_create_info is not available on the current runtime.");
                    return false;
                }
            }

            var wasSuccessful = Internal_Unity_MetaSetSubsampledLayout(enableSubsampling) == XrResult.Success;

            if (wasSuccessful)
            {
                isSubsampledLayoutEnabled = enableSubsampling;
            }

            return wasSuccessful;
        }

        void ApplyDynamicFoveation(bool enableDynamicFoveation)
        {
            if (m_XrSession == 0)
                return;

            if (enableDynamicFoveation &&
                OpenXRSettings.ActiveBuildTargetInstance.foveatedRenderingApi != OpenXRSettings.BackendFovationApi.SRPFoveation)
            {
                Debug.LogWarning($"Dynamic foveation is only supported with SRP Foveation rendering method. The current set foveation rendering method is \"{OpenXRSettings.ActiveBuildTargetInstance.foveatedRenderingApi}\"");
                return;
            }

            var result = Internal_GetFbFoveationLevel(out var currentFoveationLevel);
            if (result != XrResult.Success)
            {
                Debug.LogWarning($"Failed to read the current foveation level ({result}). Dynamic foveation was not changed.");
                return;
            }

            var useFoveationDynamic = enableDynamicFoveation
                ? XrFoveationDynamicFB.LevelEnabled
                : XrFoveationDynamicFB.Disabled;

            result = Internal_SetFbFoveationLevel(m_XrSession, currentFoveationLevel, 0f, useFoveationDynamic);
            if (result != XrResult.Success)
                Debug.LogWarning($"Failed to set dynamic foveation ({result}).");
        }

        /// <inheritdoc />
        protected internal override bool ShouldUseExtension(string ext)
        {
            if (!useEyeTracking && ext == k_EyeTrackingExtension)
            {
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        protected internal override bool OnInstanceCreate(ulong instance)
        {
            // If using BiRP, the feature must know not to use the newer API for FSR/FDM
            Internal_Unity_SetUseFoveatedRenderingLegacyMode(GraphicsSettings.defaultRenderPipeline == null);
            TrySetSubsampledLayoutEnabled(enableSubsampledLayout);

            return base.OnInstanceCreate(instance);
        }

        /// <inheritdoc />
        protected internal override void OnSessionCreate(ulong xrSession)
        {
            m_XrSession = xrSession;
            ApplyDynamicFoveation(m_EnableDynamicFoveation);
        }

        /// <inheritdoc />
        protected internal override void OnSessionDestroy(ulong xrSession)
        {
            m_XrSession = 0;
        }

        /// <inheritdoc />
        protected internal override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            return Internal_Unity_intercept_xrGetInstanceProcAddr(func);
        }

        /// <inheritdoc />
        protected internal override void OnSubsystemCreate()
        {
            // The native display provider isn't created until the XR Display subsystem itself is created, which
            // happens after OnInstanceCreate runs. Applying the value here (rather than in OnInstanceCreate) is what
            // actually reaches the display subsystem instead of silently no-oping every time.
            Internal_SetSkipFDMForFinalPasses(skipFDMForFinalPassesSRP);
            base.OnSubsystemCreate();
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

        [DllImport(Library, EntryPoint = "MetaSetSubsampledLayout")]
        private static extern XrResult Internal_Unity_MetaSetSubsampledLayout([MarshalAs(UnmanagedType.U1)] bool enableSubsampling);

        [DllImport(Library, EntryPoint = "NativeConfig_SetSkipFDMForFinalPasses")]
        private static extern void Internal_SetSkipFDMForFinalPasses([MarshalAs(UnmanagedType.I1)] bool skipFDM);

        [DllImport(Library, EntryPoint = "FBSetFoveationLevel")]
        static extern XrResult Internal_SetFbFoveationLevel(ulong session, XrFoveationLevelFB level, float verticalOffset, XrFoveationDynamicFB useFoveationDynamic);

        [DllImport(Library, EntryPoint = "FBGetFoveationLevel")]
        static extern XrResult Internal_GetFbFoveationLevel(out XrFoveationLevelFB level);

        [DllImport(Library, EntryPoint = "FBGetFoveationDynamic")]
        static extern XrResult Internal_GetFbFoveationDynamic(out XrFoveationDynamicFB useFoveationDynamic);
    }
}
