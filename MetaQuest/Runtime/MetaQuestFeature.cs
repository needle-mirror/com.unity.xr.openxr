using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;

#if UNITY_EDITOR
using System.IO;
using UnityEditor.Android;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.Rendering;
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif

[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Features.OculusQuestSupport")]
[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Features.MetaQuestSupport.Editor")]
[assembly: InternalsVisibleTo("Unity.XRTesting")]
namespace UnityEngine.XR.OpenXR.Features.MetaQuestSupport
{
    /// <summary>
    /// Enables the Meta mobile OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with Quest.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Meta Quest Support",
        Desc = "Necessary to deploy a Meta Quest compatible app.",
        Company = "Unity",
        DocumentationLink = Constants.k_DocumentationManualURL + "features/metaquest.html",
        OpenxrExtensionStrings = "XR_OCULUS_android_initialize_loader",
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        FeatureId = featureId
    )]
#endif
    public class MetaQuestFeature : OpenXRFeature, ISerializationCallbackReceiver
    {
        [Serializable]
        internal struct TargetDevice
        {
            public string visibleName;
            public string manifestName;
            public bool enabled;
            [NonSerialized] public bool active;
        }
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.metaquest";

        /// <summary>
        /// The name of the ambient occlusion render feature script.
        /// Used for validation regarding ambient occlusion on meta quest devices.
        /// </summary>
        private const string ambientOcclusionScriptName = "ScreenSpaceAmbientOcclusion";

        /// <summary>OnBeforeSerialize.</summary>
        public void OnBeforeSerialize()
        {
#if UNITY_6000_1_OR_NEWER && UNITY_EDITOR
#pragma warning disable CS0618 // suppress obsolete field usage warning
            optimizeMultiviewRenderRegions = multiviewRenderRegionsOptimizationMode != OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None;
#pragma warning restore CS0618
#endif
        }
        /// <summary>OnAfterDeserialize.</summary>
        public void OnAfterDeserialize()
        {
#if UNITY_6000_1_OR_NEWER && UNITY_EDITOR
            if (!m_hasMigratedMultiviewRenderRegions)
            {
#pragma warning disable CS0618 // suppress obsolete field usage warning
                if (optimizeMultiviewRenderRegions)
                    multiviewRenderRegionsOptimizationMode = OpenXRSettings.MultiviewRenderRegionsOptimizationMode.FinalPass;
                else
                    multiviewRenderRegionsOptimizationMode = OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None;
#pragma warning restore CS0618

                m_hasMigratedMultiviewRenderRegions = true;
            }
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Adds devices to the supported devices list in the Android manifest.
        /// </summary>
        [SerializeField]
        internal List<TargetDevice> targetDevices;

        /// <summary>
        /// Forces the removal of Internet permissions added to the Android Manifest.
        /// </summary>
        [SerializeField, Tooltip("Forces the removal of Internet permissions added to the Android Manifest.")]
        internal bool forceRemoveInternetPermission = false;

        [SerializeField]
        internal bool symmetricProjection = false;

        /// <summary>
        /// Different APIs to use in the backend.
        /// On Built-in Render Pipeline, only Legacy will be used.
        /// On Scriptable Render Pipelines, it is highly recommended to use the SRPFoveation API. More textures will use FDM with the SRPFoveation API.
        /// </summary>
        [SerializeField, Tooltip("On Scriptable Render Pipelines, it is highly recommended to use the SRPFoveation API. More textures will use FDM with the SRPFoveation API.")]
        internal OpenXRSettings.BackendFovationApi foveatedRenderingApi = OpenXRSettings.BackendFovationApi.Legacy;

        /// <summary>
        /// Uses a PNG in the Assets folder as the system splash screen image. If set, the OS will display the system splash screen as a high quality compositor layer as soon as the app is starting to launch until the app submits the first frame.
        /// </summary>
        [SerializeField, Tooltip("Uses a PNG in the Assets folder as the system splash screen image. If set, the OS will display the system splash screen as a high quality compositor layer as soon as the app is starting to launch until the app submits the first frame.")]
        public Texture2D systemSplashScreen;

        [SerializeField, Tooltip("Optimization that allows 4x MSAA textures to be memoryless on Vulkan")]
        internal bool optimizeBufferDiscards = true;

        /// <summary>
        /// Caches validation rules for each build target group requested by <see cref="GetValidationChecks="/>.
        /// </summary>
        private Dictionary<BuildTargetGroup, ValidationRule[]> validationRules = new Dictionary<BuildTargetGroup, ValidationRule[]>();

        /// Holding the Late Latching mode here for the editor (so we get undo/redo functionality)
        /// </summary>
        [SerializeField]
        internal bool lateLatchingMode;

        /// <summary>
        /// Holding the Late Latching mode here for the editor (so we get undo/redo functionality)
        /// </summary>
        [SerializeField]
        internal bool lateLatchingDebug;

        /// <summary>
        /// If enabled, the application can use Multi-View Per View Viewports functionality. This feature requires Unity 6.1 or later, and usage of the Vulkan renderer.
        /// </summary>
        [SerializeField, HideInInspector]
        [Obsolete("optimizeMultiviewRenderRegions is deprecated. Use multiviewRenderRegionsOptimizationMode instead.", false)]
        internal bool optimizeMultiviewRenderRegions;
#if UNITY_6000_1_OR_NEWER
        /// <summary>
        /// Selected Multiview Render Region Optimization Mode. This feature requires Unity 6.1 or later, and usage of the Vulkan renderer.
        /// </summary>
        [SerializeField]
        internal OpenXRSettings.MultiviewRenderRegionsOptimizationMode multiviewRenderRegionsOptimizationMode;

        [SerializeField, HideInInspector]
        private bool m_hasMigratedMultiviewRenderRegions = false;
#endif

        /// <summary>
        /// Holding the Space Warp motion vector texture format
        /// </summary>
        [SerializeField]
        internal OpenXRSettings.SpaceWarpMotionVectorTextureFormat spacewarpMotionVectorTextureFormat =  OpenXRSettings.SpaceWarpMotionVectorTextureFormat.RGBA16f;

        /// <summary>
        /// Forces the removal of Internet permissions added to the Android Manifest.
        /// </summary>
        public bool ForceRemoveInternetPermission
        {
            get => forceRemoveInternetPermission;
            set => forceRemoveInternetPermission = value;
        }

        public new void OnEnable()
        {
            // add known devices
            AddTargetDevice("quest", "Quest", true);
            AddTargetDevice("quest2", "Quest 2", true);
            AddTargetDevice("cambria", "Quest Pro", true);
            AddTargetDevice("eureka", "Quest 3", true);
            AddTargetDevice("quest3s", "Quest 3S", true);
        }

        /// <summary>
        /// Adds additional target devices to the devices list in the MetaQuestFeatureEditor. Added target devices will
        /// be serialized into the settings asset and will persist across editor sessions, but will only be visible to users
        /// and the manifest if they've been added in the active editor session.
        /// </summary>
        /// <param name="manifestName">Target device name that will be added to AndroidManifest</param>
        /// <param name="visibleName">Device name that will be displayed in feature configuration UI</param>
        /// <param name="enabledByDefault">Target device should be enabled by default or not</param>
        public void AddTargetDevice(string manifestName, string visibleName, bool enabledByDefault)
        {
            if (targetDevices == null)
                targetDevices = new List<TargetDevice>();

            // don't add devices that already exist, but do mark them active for this session
            for (int i = 0; i < targetDevices.Count; ++i)
            {
                var dev = targetDevices[i];

                if (dev.manifestName == manifestName)
                {
                    dev.active = true;
                    targetDevices[i] = dev;
                    return;
                }
            }

            TargetDevice targetDevice = new TargetDevice { manifestName = manifestName, visibleName = visibleName, enabled = enabledByDefault, active = true };
            targetDevices.Add(targetDevice);
        }

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

        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            if (!validationRules.ContainsKey(targetGroup))
                validationRules.Add(targetGroup, CreateValidationRules(targetGroup));

            rules.AddRange(validationRules[targetGroup]);
        }

        private ValidationRule[] CreateValidationRules(BuildTargetGroup targetGroup) =>

            new ValidationRule[]
            {
                    new ValidationRule(this)
                    {
                        message = "Select Oculus Touch Interaction Profile, Meta Quest Touch Pro Interaction Profile, or Meta Quest Touch Plus Interaction Profile to pair with.",
                        checkPredicate = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            if (null == settings)
                                return false;

                            bool touchFeatureEnabled = false;
                            foreach (var feature in settings.GetFeatures<OpenXRInteractionFeature>())
                            {
                                if (feature.enabled)
                                {
                                    if ((feature is OculusTouchControllerProfile) || (feature is MetaQuestTouchProControllerProfile) || (feature is MetaQuestTouchPlusControllerProfile))
                                        touchFeatureEnabled = true;
                                }
                            }
                            return touchFeatureEnabled;
                        },
                        error = false,
                        fixIt = () => { SettingsService.OpenProjectSettings("Project/XR Plug-in Management/OpenXR"); },
                        fixItAutomatic = false,
                        fixItMessage = "Open Project Settings to select Oculus Touch, Meta Quest Touch Pro, or Meta Quest Touch Plus interaction profiles or select all."
                    },

                    new ValidationRule(this)
                    {
                        message = "No Quest target devices selected.",
                        checkPredicate = () =>
                        {
                            foreach (var device in targetDevices)
                            {
                                if (device.enabled)
                                    return true;
                            }

                            return false;
                        },
                        fixIt = () =>
                        {
                            var window = MetaQuestFeatureEditorWindow.Create(this);
                            window.ShowPopup();
                        },
                        error = true,
                        fixItAutomatic = false,
                    },

                    new ValidationRule(this)
                    {
                        message = "Using the Screen Space Ambient Occlusion render feature results in significant performance overhead when the application is running natively on device. Disabling or removing that render feature is recommended.",
                        helpText = "Only removing the Screen Space Ambient Occlusion render feature from all UniversalRenderer assets that may be used will make this warning go away, but just disabling the render feature will still prevent the performance overhead.",
                        checkPredicate = () =>
                        {

                            // Checks the dependencies of all configured render pipeline assets.
                            foreach(var renderPipeline in GraphicsSettings.allConfiguredRenderPipelines)
                            {
                                var dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(renderPipeline));
                                foreach(var dependency in dependencies)
                                {
                                    if (dependency.Contains(ambientOcclusionScriptName))
                                        return false;
                                }
                            }

                            return true;
                        },
                        fixItAutomatic = false,
                    },

                    new ValidationRule(this)
                    {
                        message = "System Splash Screen must be a PNG texture asset.",
                        checkPredicate = () =>
                        {
                            if (systemSplashScreen == null)
                                return true;

                            string splashScreenAssetPath = AssetDatabase.GetAssetPath(systemSplashScreen);
                            if (Path.GetExtension(splashScreenAssetPath).ToLower() != ".png")
                                return false;

                            return true;
                        },
                        fixIt = () =>
                        {
                            var window = MetaQuestFeatureEditorWindow.Create(this);
                            window.ShowPopup();
                        },
                        error = true,
                        fixItAutomatic = false,
                    },

                    // MultiviewRenderRegionsOptimizationMode (aka MVPVV) only supported on Unity 6.1 onwards
#if UNITY_6000_1_OR_NEWER
                    new ValidationRule(this)
                    {
                        message = "Multiview Render Regions Optimizations Mode requires symmetric projection setting turned on.",
                        checkPredicate = () =>
                        {
                            if (multiviewRenderRegionsOptimizationMode != OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None)
                            {
                                return symmetricProjection;
                            }
                            return true;
                        },
                        error = true,
                        fixIt = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            var feature = settings.GetFeature<MetaQuestFeature>();
                            feature.symmetricProjection = true;
                        }
                    },

                    new ValidationRule(this)
                    {
                        message = "Multiview Render Regions Optimizations Mode requires Render Mode set to \"Single Pass Instanced / Multi-view\".",
                        checkPredicate = () =>
                        {
                            if (multiviewRenderRegionsOptimizationMode != OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None)
                            {
                                var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                                return (settings.renderMode == OpenXRSettings.RenderMode.SinglePassInstanced);
                            }
                            return true;
                        },
                        error = true,
                        fixIt = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            settings.renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;
                        }
                    },

                    new ValidationRule(this)
                    {
                        message = "Multiview Render Regions Optimizations Mode needs the Vulkan Graphics API to be the default Graphics API to work at runtime.",
                        helpText = "The Multiview Render Regions Optimizations Mode feature only works with the Vulkan Graphics API, which needs to be set as the first Graphics API to be loaded at application startup. Choosing other Graphics API may require to switch to Vulkan and restart the application.",
                        checkPredicate = () =>
                        {
                            if (multiviewRenderRegionsOptimizationMode != OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None)
                            {
                                var graphicsApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                                return graphicsApis[0] == GraphicsDeviceType.Vulkan;
                            }
                            return true;
                        },
                        error = false
                    },

                    new ValidationRule(this)
                    {
                        message = "Multiview Render Regions Optimizations - All Passes mode is only supported on Unity 6.2+ versions",
                        checkPredicate = () =>
                        {
#if !UNITY_6000_2_OR_NEWER
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            if (settings.multiviewRenderRegionsOptimizationMode == OpenXRSettings.MultiviewRenderRegionsOptimizationMode.AllPasses)
                                return false;
#endif
                            return true;
                        },
                        error = true,
                        fixIt = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            var feature = settings.GetFeature<MetaQuestFeature>();
                            feature.multiviewRenderRegionsOptimizationMode = OpenXRSettings.MultiviewRenderRegionsOptimizationMode.FinalPass;
                        },
                        fixItAutomatic = true,
                        fixItMessage = "Set Multiview Render Regions Optimization Mode to Final Pass."
                    },

#endif

#if UNITY_ANDROID
                new ValidationRule(this)
                    {
                        message = "Symmetric Projection is only supported on Vulkan graphics API",
                        checkPredicate = () =>
                        {
                            if (symmetricProjection && !SettingsUseVulkan())
                            {
                                return false;
                            }
                            return true;
                        },
                        fixIt = () =>
                        {
                            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
                        },
                        fixItAutomatic = true,
                        fixItMessage = "Set Vulkan as Graphics API"
                    },

                    new ValidationRule(this)
                    {
                        message = "Symmetric Projection is only supported when using Multi-view",
                        checkPredicate = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            if (null == settings)
                                return false;

                            if (symmetricProjection && (settings.renderMode != OpenXRSettings.RenderMode.SinglePassInstanced))
                            {
                                return false;
                            }
                            return true;
                        },
                        fixIt = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            if (null != settings)
                            {
                                settings.renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;
                            }
                        },
                        error = true,
                        fixItAutomatic = true,
                        fixItMessage = "Set Render Mode to Multi-view"
                    },

                    new ValidationRule(this)
                    {
                        message = "Only Legacy Foveated Rendering API usage is possible on Built-in Render Pipeline",
                        checkPredicate = () =>
                        {
                            return GraphicsSettings.defaultRenderPipeline != null || foveatedRenderingApi == OpenXRSettings.BackendFovationApi.Legacy;
                        },
                        fixIt = () =>
                        {
                            foveatedRenderingApi = OpenXRSettings.BackendFovationApi.Legacy;
                        },
                        error = true,
                        fixItAutomatic = true,
                        fixItMessage = "Set Foveated Rendering API to Legacy"
                    },

                    new ValidationRule(this)
                    {
                        message = "Symmetric Projection is only available on Quest 2 or higher",
                        checkPredicate = () =>
                        {
                            if (symmetricProjection)
                            {
                                foreach (var device in targetDevices)
                                {
                                    if (device.enabled && device.manifestName == "quest")
                                    {
                                        return false;
                                    }
                                }
                            }
                            return true;
                        },
                        fixIt = () =>
                        {
                            var window = MetaQuestFeatureEditorWindow.Create(this);
                            window.ShowPopup();
                        },
                        error = true,
                        fixItAutomatic = false,
                    },

                    new ValidationRule(this)
                    {
                        message = "Optimize Buffer Discards is only supported on Vulkan graphics API",
                        checkPredicate = () =>
                        {
                            if (optimizeBufferDiscards && !SettingsUseVulkan())
                            {
                                return false;
                            }

                            return true;
                        },
                        fixIt = () =>
                        {
                            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
                        },
                        fixItAutomatic = true,
                        fixItMessage = "Set Vulkan as Graphics API"
                    },
#endif
                    new ValidationRule(this)
                    {
                        message = "Meta Quest HMDs only support Landscape Left orientation.",
                        checkPredicate = () =>
                        {
                            if (PlayerSettings.defaultInterfaceOrientation == UIOrientation.AutoRotation)
                            {
                                if (!PlayerSettings.allowedAutorotateToLandscapeLeft)
                                    return false;
                            }
                            else
                            {
                                if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.LandscapeLeft)
                                    return false;
                            }
                            return true;
                        },
                        error = true,
                        fixIt = () =>
                        {
                            if (PlayerSettings.defaultInterfaceOrientation == UIOrientation.AutoRotation)
                            {
                                PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                            }
                            else
                            {
                                PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
                            }
                        },
                        fixItAutomatic = true,
                    },
                    new ValidationRule(this)
                    {
                        message = "[Optional] Latency Optimization is recommended to be set to <b>Prioritize Input Polling</b> when Meta Quest support is enabled.",
                        checkPredicate = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            if (settings.latencyOptimization != OpenXRSettings.LatencyOptimization.PrioritizeInputPolling)
                                return false;
                            else
                                return true;
                        },
                        fixIt = () =>
                        {
                            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                            settings.latencyOptimization = OpenXRSettings.LatencyOptimization.PrioritizeInputPolling;
                        },
                        fixItAutomatic = true,
                        fixItMessage = "Set Latency Optimization to <b>Prioritize Input Polling</b>"
                    },
#if UNITY_6000_2_OR_NEWER && UNITY_META_QUEST
                    new ValidationRule(this)
                    {
                        message = "[Optional] Meta Quest supports Thin Link Time Optimization. This can optimize CPU performance and can only be done in non-development, IL2CPP enabled builds with Strip Engine Code disabled.",
                        checkPredicate = () =>
                        {
                            if (EditorUserBuildSettings.development == true)
                            {
                                return false;
                            }

                            if (PlayerSettings.GetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android) != ScriptingImplementation.IL2CPP)
                            {
                                return false;
                            }

                            if (PlayerSettings.stripEngineCode == true)
                            {
                                return false;
                            }

                            if (UserBuildSettings.linkTimeOptimization == Unity.Android.Types.AndroidLinkTimeOptimization.None)
                            {
                                return false;
                            }
                            return true;
                        },
                        error = false,
                        fixIt = () =>
                        {
                            EditorUserBuildSettings.development = false;

                            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);

                            PlayerSettings.stripEngineCode = false;

                            UserBuildSettings.linkTimeOptimization = Unity.Android.Types.AndroidLinkTimeOptimization.Thin;
                        },
                        fixItAutomatic = false,
                    }
#endif
            };

        internal class MetaQuestFeatureEditorWindow : EditorWindow
        {
            private Object feature;
            private Editor featureEditor;

            public static EditorWindow Create(Object feature)
            {
                var window = EditorWindow.GetWindow<MetaQuestFeatureEditorWindow>(true, "Meta Quest Feature Configuration", true);
                window.feature = feature;
                window.featureEditor = Editor.CreateEditor(feature);
                return window;
            }

            private void OnGUI()
            {
                featureEditor.OnInspectorGUI();
            }
        }
#endif
    }
}
