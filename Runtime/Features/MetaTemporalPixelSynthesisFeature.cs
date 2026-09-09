// Copyright (c) Unity Technologies. All rights reserved.
// Licensed under the Unity Companion License for Unity-dependent projects.
// See LICENSE.md for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR;
using UnityEditor.XR.OpenXR.Features;
#if UNITY_RENDER_PIPELINES_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif // UNITY_RENDER_PIPELINES_UNIVERSAL
#endif

namespace UnityEngine.XR.OpenXR.Features.Meta
{
    /// <summary>
    /// Enables the <c>XR_META_temporal_pixel_synthesis</c> OpenXR extension.
    ///
    /// This feature allows applications to provide per-view motion vector, depth, and stencil
    /// data so that the Meta runtime can apply temporal anti-aliasing, upscaling, or other
    /// temporal reconstruction techniques to improve the visual quality of the current frame's
    /// projection layer content.
    ///
    /// Enable this feature in <b>Project Settings &gt; XR Plug-in Management &gt; OpenXR &gt;
    /// OpenXR Feature Groups</b>. Call <see cref="SetPixelSynthesis"/> to supply parameters;
    /// values are cached and reused every frame until updated. Default parameters are applied
    /// automatically when the session starts if <see cref="SetPixelSynthesis"/> has not been called.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(
        UiName = "Meta: Temporal Pixel Synthesis",
        Desc = "Enables temporal anti-aliasing and upscaling via XR_META_temporal_pixel_synthesis. " +
               "Requires motion vector data rendered to a separate swapchain each frame.",
        Company = "Unity",
        DocumentationLink = Constants.k_DocumentationManualURL + "features/temporalpixelsynthesis.html",
        OpenxrExtensionStrings = ExtensionString,
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        FeatureId = FeatureId
    )]
#endif
    public class MetaTemporalPixelSynthesisFeature : OpenXRFeature
    {
        /// <summary>The unique feature ID used to identify this feature in OpenXR settings.</summary>
        public const string FeatureId = "com.unity.openxr.feature.meta.temporalpixelsynthesis";

        /// <summary>The OpenXR extension string required by this feature.</summary>
        public const string ExtensionString = "XR_META_temporal_pixel_synthesis";

        // ------------------------------------------------------------------
        // Inspector-serialized default settings
        // ------------------------------------------------------------------

        /// <summary>
        /// Quality requested for the fovea (central) region, applied when
        /// <see cref="SetPixelSynthesis"/> is called without an explicit override.
        /// </summary>
        [Tooltip("Quality requested for the fovea (central) region of the image. The device may adjust this based on available performance.")]
        public PixelSynthesisQualityLevel defaultFoveaQuality = PixelSynthesisQualityLevel.High;

        /// <summary>
        /// Quality requested for the periphery (outer) region, applied when
        /// <see cref="SetPixelSynthesis"/> is called without an explicit override.
        /// </summary>
        [Tooltip("Quality requested for the periphery (outer) region of the image. The device may adjust this based on available performance.")]
        public PixelSynthesisQualityLevel defaultPeripheryQuality = PixelSynthesisQualityLevel.Medium;

        /// <summary>Size of the fovea region, controlling where the central region ends and the periphery begins.</summary>
        [Tooltip("Size of the fovea region, controlling where the central region ends and the periphery begins. A larger fovea favors quality, a smaller one favors performance.")]
        public PixelSynthesisFoveaRadius defaultFoveaRadius = PixelSynthesisFoveaRadius.Medium;

        /// <summary>Default setting for whether upscaling is requested.</summary>
        [Tooltip("Let the device upscale the rendered image. Requires a device that supports upscaling.")]
        public bool defaultEnableUpscaling = false;

        /// <summary>Quality requested for upscaling. Only used when <see cref="defaultEnableUpscaling"/> is <c>true</c>.</summary>
        [Tooltip("Quality requested when upscaling. The device may adjust this based on available performance.")]
        public PixelSynthesisQualityLevel defaultUpscalingQuality = PixelSynthesisQualityLevel.Medium;

        // ------------------------------------------------------------------
        // Runtime state
        // ------------------------------------------------------------------

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static MetaTemporalPixelSynthesisFeature s_Instance;
#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static bool s_UserHasSetParams;

        /// <summary>
        /// Singleton accessor. Returns <c>null</c> when the feature is not enabled.
        /// </summary>
        public static MetaTemporalPixelSynthesisFeature Instance => s_Instance;

        /// <summary>
        /// Whether the device runtime reports support for this extension.
        /// Populated after session creation.
        /// </summary>
        public bool IsSupported { get; private set; }

        /// <summary>
        /// Bitmask of optional features reported by the runtime via
        /// <c>XrSystemPixelSynthesisPropertiesMETA.supportedFeatureFlags</c>.
        /// </summary>
        public PixelSynthesisSupportedFeatureFlags SupportedFeatureFlags { get; private set; }

        /// <summary>
        /// The recommended motion-vector image width (pixels) for the first view,
        /// queried from <c>XrViewPixelSynthesisConfigurationViewMETA</c>. Zero until
        /// <see cref="OnSessionCreate"/> runs.
        /// </summary>
        public uint RecommendedMotionVectorWidth { get; private set; }

        /// <summary>
        /// The recommended motion-vector image height (pixels) for the first view.
        /// </summary>
        public uint RecommendedMotionVectorHeight { get; private set; }

        // ------------------------------------------------------------------
        // OpenXRFeature lifecycle
        // ------------------------------------------------------------------

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();
            s_Instance = this;
        }

        /// <inheritdoc/>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (s_Instance == this)
                s_Instance = null;
        }

        /// <inheritdoc/>
        protected internal override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!base.OnInstanceCreate(xrInstance))
                return false;

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
            {
                Debug.LogError($"[{nameof(MetaTemporalPixelSynthesisFeature)}] " +
                               "Temporal Pixel Synthesis is only supported on the Vulkan graphics API. " +
                               "The application is running on OpenGL ES 3, so Temporal Pixel Synthesis has been disabled.");
                return false;
            }

            if (!OpenXRRuntime.IsExtensionEnabled(ExtensionString))
            {
                Debug.LogWarning($"[{nameof(MetaTemporalPixelSynthesisFeature)}] " +
                                 $"Extension '{ExtensionString}' is not available on this runtime.");
                return true; // Non-fatal: feature is simply inactive at runtime.
            }

            NativeApi.Create(xrInstance);

            return true;
        }

        /// <inheritdoc/>
        protected internal override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);

            if (!OpenXRRuntime.IsExtensionEnabled(ExtensionString))
                return;

            NativeApi.OnSessionCreate(xrSession,
                out bool supported,
                out uint supportedFlags,
                out uint mvWidth,
                out uint mvHeight);

            IsSupported = supported;
            SupportedFeatureFlags = (PixelSynthesisSupportedFeatureFlags)supportedFlags;
            RecommendedMotionVectorWidth = mvWidth;
            RecommendedMotionVectorHeight = mvHeight;

            if (!IsSupported)
            {
                Debug.LogWarning($"[{nameof(MetaTemporalPixelSynthesisFeature)}] " +
                                 "Runtime reports pixel synthesis is not supported on this system.");
            }
            else
            {
                Debug.Log($"[{nameof(MetaTemporalPixelSynthesisFeature)}] " +
                          $"Pixel synthesis supported. Feature flags: {SupportedFeatureFlags}. " +
                          $"Recommended MV size: {mvWidth}x{mvHeight}.");

                if (!s_UserHasSetParams)
                {
                    var defaultParams = PixelSynthesisFrameParams.Default;
                    defaultParams.enableUpscaling = defaultEnableUpscaling;
                    SendParamsToNative(defaultParams);
                }
            }
        }

        /// <inheritdoc/>
        protected internal override void OnSessionDestroy(ulong xrSession)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionString))
                NativeApi.OnSessionDestroy(xrSession);

            IsSupported = false;
            SupportedFeatureFlags = 0;
            RecommendedMotionVectorWidth = 0;
            RecommendedMotionVectorHeight = 0;
            s_UserHasSetParams = false;

            base.OnSessionDestroy(xrSession);
        }

        /// <inheritdoc/>
        protected internal override void OnInstanceDestroy(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionString))
                NativeApi.Destroy();

            base.OnInstanceDestroy(xrInstance);
        }

        /// <inheritdoc/>
        protected internal override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            // Hand our xrGetInstanceProcAddr wrapper to the native layer so it can
            // intercept xrEndFrame and inject XrPixelSynthesisInfoMETA into the
            // next-chain of each XrCompositionLayerProjectionView.
            return NativeApi.HookGetInstanceProcAddr(func);
        }

        // ------------------------------------------------------------------
        // Public API
        // ------------------------------------------------------------------

        /// <summary>
        /// Configures pixel synthesis parameters. Values are cached and reused every frame
        /// until this method is called again. Call before <c>xrEndFrame</c> is submitted
        /// (typically in a camera callback or just before <c>XRDisplaySubsystem.Stop()</c>).
        /// </summary>
        /// <param name="params">Pixel synthesis parameters to apply.</param>
        public static void SetPixelSynthesis(in PixelSynthesisFrameParams @params)
        {
            if (s_Instance == null || !s_Instance.IsSupported)
                return;

            s_UserHasSetParams = true;
            SendParamsToNative(@params);
        }

        /// <summary>
        /// Resets pixel synthesis parameters to their defaults. Pixel synthesis remains
        /// active; use <see cref="SetPixelSynthesisEnabled"/> to enable or disable it.
        /// </summary>
        public static void ClearPixelSynthesis()
        {
            if (s_Instance == null || !s_Instance.IsSupported)
                return;

            s_UserHasSetParams = false;
            var defaultParams = PixelSynthesisFrameParams.Default;
            defaultParams.enableUpscaling = s_Instance.defaultEnableUpscaling;
            SendParamsToNative(defaultParams);
        }

        /// <summary>
        /// Enables or disables pixel synthesis at runtime without discarding the cached
        /// parameters. When re-enabled, the previously cached parameters resume immediately.
        /// </summary>
        /// <param name="enabled">
        /// <c>true</c> to enable pixel synthesis; <c>false</c> to disable it.
        /// </param>
        public static void SetPixelSynthesisEnabled(bool enabled)
        {
            if (s_Instance == null || !s_Instance.IsSupported)
                return;

            NativeApi.SetEnabled(enabled);
        }

        static void SendParamsToNative(PixelSynthesisFrameParams @params)
        {
            PixelSynthesisFlags flags = PixelSynthesisFlags.None;
            if (@params.enableUpscaling)
                flags |= PixelSynthesisFlags.EnableUpscaling;
            if (s_Instance.SupportedFeatureFlags.HasFlag(PixelSynthesisSupportedFeatureFlags.EyeTrackedFoveation))
                flags |= PixelSynthesisFlags.UseEyeTrackedFoveation;

            var mainCamera = Camera.main;
            float nearZ = @params.nearZ >= 0f ? @params.nearZ : (mainCamera != null ? mainCamera.nearClipPlane : 0.1f);
            float farZ  = @params.farZ  >= 0f ? @params.farZ  : (mainCamera != null ? mainCamera.farClipPlane  : 1000f);

            uint mvW = s_Instance.RecommendedMotionVectorWidth;
            uint mvH = s_Instance.RecommendedMotionVectorHeight;

            var native = new NativeApi.NativeParams
            {
                motionVectorImageArrayIndex = 0,
                motionVectorImageMipLevel   = 0,
                motionVectorImageRect       = new RectInt(0, 0, (int)mvW, (int)mvH),
                motionVectorScale           = @params.motionVectorScale,
                motionVectorOffset          = @params.motionVectorOffset,
                appSpaceDeltaPosition       = @params.appSpaceDeltaPosition,
                appSpaceDeltaOrientation    = @params.appSpaceDeltaOrientation,
                depthImageArrayIndex        = 0,
                depthImageMipLevel          = 0,
                depthImageRect              = default,
                minDepth                    = 0f,
                maxDepth                    = 1f,
                nearZ                       = nearZ,
                farZ                        = farZ,
                stencilImageArrayIndex      = 0,
                stencilImageMipLevel        = 0,
                stencilImageRect            = default,
                stencilBitMask              = @params.stencilBitMask,
                stencilValue                = @params.stencilValue,
                foveaQuality                = (int)s_Instance.defaultFoveaQuality,
                peripheryQuality            = (int)s_Instance.defaultPeripheryQuality,
                foveaRadius                 = (int)s_Instance.defaultFoveaRadius,
                upscalingQuality            = (int)s_Instance.defaultUpscalingQuality,
                layerFlags                  = (ulong)flags,
                resetAggregations           = @params.resetAggregations ? 1 : 0,
            };

            NativeApi.SetPixelSynthesisParams(in native);
        }

        // ------------------------------------------------------------------
        // Validation Rules
        // ------------------------------------------------------------------

#if UNITY_EDITOR
        protected internal override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
#if UNITY_RENDER_PIPELINES_UNIVERSAL
            rules.Add(new ValidationRule(this)
            {
                message = "Temporal Pixel Synthesis should not be used with URP Upscaling as it results in a performance degradation.",
                checkPredicate = () =>
                {
                    var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    var feature = openXRSettings?.GetFeature<MetaTemporalPixelSynthesisFeature>();
                    if (feature == null || !feature.enabled)
                        return true;

                    var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if (urpAsset == null)
                        return true;

                    return urpAsset.upscalingFilter == UpscalingFilterSelection.Auto;
                },
                fixIt = () =>
                {
                    var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if (urpAsset != null)
                        urpAsset.upscalingFilter = UpscalingFilterSelection.Auto;
                },
                error = false,
                fixItAutomatic = true,
                fixItMessage = "Set Upscaling Filter to Automatic."
            });

            rules.Add(new ValidationRule(this)
            {
                message = "Temporal Pixel Synthesis should not be used with URP Anti-Aliasing as it results in a performance degradation.",
                checkPredicate = () =>
                {
                    var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    var feature = openXRSettings?.GetFeature<MetaTemporalPixelSynthesisFeature>();
                    if (feature == null || !feature.enabled)
                        return true;

                    var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if (urpAsset == null)
                        return true;

                    return urpAsset.msaaSampleCount == 1;
                },
                fixIt = () =>
                {
                    var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if (urpAsset != null)
                        urpAsset.msaaSampleCount = 1;
                },
                error = false,
                fixItAutomatic = true,
                fixItMessage = "Set MSAA to disabled."
            });

            // Camera anti-aliasing is per-camera rather than per-asset, so this can only inspect the
            // cameras in the loaded scenes. URP's camera inspector shows a matching note that links here.
            rules.Add(new ValidationRule(this)
            {
                message = "Temporal Pixel Synthesis should not be used with per-camera Anti-Aliasing. The compositor performs the temporal resolve, so URP's anti-aliasing is redundant and its jitter is not wanted.",
                checkPredicate = () =>
                {
                    var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    var feature = openXRSettings?.GetFeature<MetaTemporalPixelSynthesisFeature>();
                    if (feature == null || !feature.enabled)
                        return true;

                    return FindCamerasWithAntialiasing().Count == 0;
                },
                fixIt = () =>
                {
                    foreach (var cameraData in FindCamerasWithAntialiasing())
                    {
                        Undo.RecordObject(cameraData, "Disable camera anti-aliasing for Temporal Pixel Synthesis");
                        cameraData.antialiasing = AntialiasingMode.None;
                        EditorUtility.SetDirty(cameraData);
                    }
                },
                error = false,
                fixItAutomatic = true,
                fixItMessage = "Set Anti-aliasing to None on cameras in the loaded scenes."
            });
#endif // UNITY_RENDER_PIPELINES_UNIVERSAL

            // TPS is only supported on Vulkan
            rules.Add(new ValidationRule(this)
            {
                message = "Temporal Pixel Synthesis is only supported on the Vulkan graphics API. Change to Vulkan or disable Temporal Pixel Synthesis",
                checkPredicate = () =>
                {
                    if (enabled && !SettingsUseVulkan())
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
                fixItMessage = "Set Vulkan as Graphics API"
            });
        }

#if UNITY_RENDER_PIPELINES_UNIVERSAL
        // Includes inactive cameras: a camera disabled at edit time can still be enabled at runtime.
        static List<UniversalAdditionalCameraData> FindCamerasWithAntialiasing()
        {
            var result = new List<UniversalAdditionalCameraData>();
#if UNITY_6000_5_OR_NEWER
            foreach (var cameraData in FindObjectsByType<UniversalAdditionalCameraData>(FindObjectsInactive.Include))
#else
            foreach (var cameraData in FindObjectsByType<UniversalAdditionalCameraData>(FindObjectsInactive.Include, FindObjectsSortMode.None))
#endif
            {
                if (cameraData != null && cameraData.antialiasing != AntialiasingMode.None)
                    result.Add(cameraData);
            }
            return result;
        }
#endif // UNITY_RENDER_PIPELINES_UNIVERSAL

        static bool SettingsUseVulkan()
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
#endif

        // ------------------------------------------------------------------
        // Native API bindings
        // ------------------------------------------------------------------

        private static class NativeApi
        {
            private const string LibName = "UnityOpenXR";

            [DllImport(LibName, EntryPoint = "MetaTemporalPixelSynthesis_Create")]
            public static extern void Create(ulong xrInstance);

            [DllImport(LibName, EntryPoint = "MetaTemporalPixelSynthesis_Destroy")]
            public static extern void Destroy();

            [DllImport(LibName, EntryPoint = "MetaTemporalPixelSynthesis_OnSessionCreate")]
            public static extern void OnSessionCreate(
                ulong xrSession,
                [MarshalAs(UnmanagedType.I1)] out bool supportsPixelSynthesis,
                out uint supportedFeatureFlags,
                out uint recommendedMotionVectorWidth,
                out uint recommendedMotionVectorHeight);

            [DllImport(LibName, EntryPoint = "MetaTemporalPixelSynthesis_OnSessionDestroy")]
            public static extern void OnSessionDestroy(ulong xrSession);

            [DllImport(LibName, EntryPoint = "MetaTemporalPixelSynthesis_HookGetInstanceProcAddr")]
            public static extern IntPtr HookGetInstanceProcAddr(IntPtr func);

            [DllImport(LibName, EntryPoint = "MetaTemporalPixelSynthesis_SetParams")]
            public static extern void SetPixelSynthesisParams(in NativeParams @params);

            /// <summary>
            /// Sequential layout must exactly match <c>ManagedPixelSynthesisParams</c> in the native plugin.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct NativeParams
            {
                public uint      motionVectorImageArrayIndex;
                public uint      motionVectorImageMipLevel;
                public RectInt   motionVectorImageRect;
                public Vector2   motionVectorScale;
                public Vector2   motionVectorOffset;
                public Vector3   appSpaceDeltaPosition;
                public Quaternion appSpaceDeltaOrientation;
                public uint      depthImageArrayIndex;
                public uint      depthImageMipLevel;
                public RectInt   depthImageRect;
                public float     minDepth;
                public float     maxDepth;
                public float     nearZ;
                public float     farZ;
                public uint      stencilImageArrayIndex;
                public uint      stencilImageMipLevel;
                public RectInt   stencilImageRect;
                public uint      stencilBitMask;
                public uint      stencilValue;
                public int       foveaQuality;
                public int       peripheryQuality;
                public int       foveaRadius;
                public int       upscalingQuality;
                public ulong     layerFlags;
                public int       resetAggregations; // XrBool32
            }

            [DllImport(LibName, EntryPoint = "MetaTemporalPixelSynthesis_SetEnabled")]
            public static extern void SetEnabled([MarshalAs(UnmanagedType.I1)] bool enabled);
        }
    }

    // ------------------------------------------------------------------
    // Public data types
    // ------------------------------------------------------------------

    /// <summary>
    /// All per-frame parameters needed to configure pixel synthesis.
    /// Pass an instance of this struct to <see cref="MetaTemporalPixelSynthesisFeature.SetPixelSynthesis"/>.
    /// </summary>
    public struct PixelSynthesisFrameParams
    {
        // Motion vector decode parameters
        /// <summary>Per-axis scale applied to motion vector texel values to convert to NDC.</summary>
        public Vector2 motionVectorScale;
        /// <summary>Per-axis offset applied to motion vector texel values to convert to NDC.</summary>
        public Vector2 motionVectorOffset;

        // App-space delta pose (for artificial locomotion compensation)
        /// <summary>
        /// Position component of the delta pose between the previous and current frame's
        /// app space. Set to <see cref="Vector3.zero"/> when no artificial locomotion occurred.
        /// </summary>
        public Vector3 appSpaceDeltaPosition;
        /// <summary>
        /// Orientation component of the delta pose between frames.
        /// Set to <see cref="Quaternion.identity"/> when no artificial locomotion occurred.
        /// </summary>
        public Quaternion appSpaceDeltaOrientation;

        /// <summary>
        /// Near clip distance in meters. When negative, defaults to <c>Camera.main.nearClipPlane</c>.
        /// </summary>
        public float nearZ;
        /// <summary>
        /// Far clip distance in meters. When negative, defaults to <c>Camera.main.farClipPlane</c>.
        /// </summary>
        public float farZ;

        // Stencil (optional)
        /// <summary>Bitmask of stencil bits to compare. Only bits set here are tested.</summary>
        public uint stencilBitMask;
        /// <summary>
        /// Reference stencil value. Pixels where <c>(stencilBuffer &amp; stencilBitMask) == stencilValue</c>
        /// are excluded from temporal accumulation.
        /// </summary>
        public uint stencilValue;

        // Reset
        /// <summary>
        /// When <c>true</c>, the runtime discards all accumulated temporal state and
        /// starts fresh. Set on scene transitions, camera cuts, or teleportation.
        /// </summary>
        public bool resetAggregations;

        // Upscaling
        /// <summary>
        /// Whether to request upscaling as part of pixel synthesis. When <see cref="SetPixelSynthesis"/>
        /// has not been called, <see cref="MetaTemporalPixelSynthesisFeature.defaultEnableUpscaling"/>
        /// is used instead. Requires <see cref="PixelSynthesisSupportedFeatureFlags.Upscaling"/> support.
        /// </summary>
        public bool enableUpscaling;

        /// <summary>
        /// Returns a <see cref="PixelSynthesisFrameParams"/> pre-filled with sensible defaults.
        /// <c>nearZ</c> and <c>farZ</c> are set to <c>-1</c>, which causes <see cref="MetaTemporalPixelSynthesisFeature.SetPixelSynthesis"/>
        /// to read the values from <c>Camera.main</c> at call time.
        /// <c>motionVectorScale</c> carries the conversion required for motion vectors produced by the
        /// render pipeline; override it only if you write your own.
        /// <c>enableUpscaling</c> defaults to <c>false</c>; the inspector field
        /// <see cref="MetaTemporalPixelSynthesisFeature.defaultEnableUpscaling"/> overrides this when
        /// automatic defaults are applied (i.e. before <see cref="MetaTemporalPixelSynthesisFeature.SetPixelSynthesis"/> is called).
        /// </summary>
        public static PixelSynthesisFrameParams Default => new PixelSynthesisFrameParams
        {
            // Y flip takes the motion vectors the render pipeline produces into the convention the
            // compositor expects. Constant rather than derived from SpaceWarp's useRightHandedNDC setting,
            // so Temporal Pixel Synthesis does not depend on a feature it can run without.
            motionVectorScale        = new Vector2(1f, -1f),
            motionVectorOffset       = Vector2.zero,
            appSpaceDeltaPosition    = Vector3.zero,
            appSpaceDeltaOrientation = Quaternion.identity,
            nearZ                    = -1f,
            farZ                     = -1f,
            resetAggregations        = false,
            enableUpscaling          = false,
        };
    }

    // Internal mirror of XrPixelSynthesisFlagBitsMETA. Computed by SetPixelSynthesis;
    // not part of the public API.
    [Flags]
    internal enum PixelSynthesisFlags : ulong
    {
        None                   = 0,
        UseDepth               = 1 << 0,
        UseStencil             = 1 << 1,
        DepthInBlue            = 1 << 2,
        UseEyeTrackedFoveation = 1 << 3,
        EnableUpscaling        = 1 << 4,
    }

    /// <summary>
    /// Quality requested for the fovea, periphery, and upscaling regions. The device may adjust the
    /// level it actually uses based on available performance.
    /// Mirrors <c>XrPixelSynthesisQualityLevelMETA</c>.
    /// </summary>
    public enum PixelSynthesisQualityLevel : int
    {
        /// <summary>Passes the region through unmodified — no temporal accumulation.</summary>
        Disabled = 0,
        /// <summary>Prioritizes performance over quality.</summary>
        Low = 1,
        /// <summary>Balances performance and quality.</summary>
        Medium = 2,
        /// <summary>Prioritizes quality over performance.</summary>
        High = 3,
        /// <summary>Runtime dynamically adjusts quality based on headroom. Requires <see cref="PixelSynthesisSupportedFeatureFlags.DynamicQuality"/>.</summary>
        Dynamic = 4,
    }

    /// <summary>
    /// Size of the fovea region, controlling the boundary between the high-quality central region
    /// and the periphery. Mirrors <c>XrPixelSynthesisFoveaRadiusMETA</c>.
    /// Note: values start at 1; zero is not valid.
    /// </summary>
    public enum PixelSynthesisFoveaRadius : int
    {
        /// <summary>Small fovea region — prioritizes performance.</summary>
        Small = 1,
        /// <summary>Medium fovea region — balances performance and quality.</summary>
        Medium = 2,
        /// <summary>Large fovea region — prioritizes quality.</summary>
        Large = 3,
        /// <summary>Runtime dynamically varies the radius. Requires <see cref="PixelSynthesisSupportedFeatureFlags.DynamicQuality"/>.</summary>
        Dynamic = 4,
    }

    /// <summary>
    /// Optional runtime capabilities reported in
    /// <c>XrSystemPixelSynthesisPropertiesMETA.supportedFeatureFlags</c>.
    /// </summary>
    [Flags]
    public enum PixelSynthesisSupportedFeatureFlags : uint
    {
        /// <summary>Runtime supports depth input for pixel synthesis.</summary>
        Depth = 1 << 0,
        /// <summary>Runtime supports stencil masking.</summary>
        Stencil = 1 << 1,
        /// <summary>Runtime supports reading depth from the blue channel.</summary>
        DepthInBlue = 1 << 2,
        /// <summary>Runtime supports eye-tracked foveation.</summary>
        EyeTrackedFoveation = 1 << 3,
        /// <summary>Runtime supports the upscaling flag.</summary>
        Upscaling = 1 << 4,
        /// <summary>Runtime supports <see cref="PixelSynthesisQualityLevel.Dynamic"/> and <see cref="PixelSynthesisFoveaRadius.Dynamic"/>.</summary>
        DynamicQuality = 1 << 5,
    }
}
