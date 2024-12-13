using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Features.MockRuntime")]
[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Features.ConformanceAutomation")]

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Runtime XR Environment Blend Mode. <see cref="Features.OpenXRFeature.SetEnvironmentBlendMode"/>
    /// </summary>
    public enum XrEnvironmentBlendMode
    {
        /// <summary>
        /// Select XR_ENVIRONMENT_BLEND_MODE_OPAQUE for runtime.
        /// </summary>
        Opaque = 1,
        /// <summary>
        /// Select XR_ENVIRONMENT_BLEND_MODE_ADDITIVE for runtime.
        /// </summary>
        Additive = 2,
        /// <summary>
        /// Select XR_ENVIRONMENT_BLEND_MODE_ALPHA_BLEND for runtime.
        /// </summary>
        AlphaBlend = 3
    }

    /// <summary>
    /// XR Results returned by XR subsystem callbacks
    /// </summary>
    public enum XrResult
    {
        /// <summary>
        /// Function successfully completed.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The specified timeout time occurred before the operation could complete.
        /// </summary>
        [Obsolete("This value is misspelled and therefore deprecated in OpenXR Plug-in version 1.14.0. Use TimeoutExpired instead.", false)]
        TimeoutExpored = 1,

        /// <summary>
        /// The specified timeout time occurred before the operation could complete.
        /// </summary>
        TimeoutExpired = 1,

        /// <summary>
        /// The session will be lost soon.
        /// </summary>
        LossPending = 3,

        /// <summary>
        /// No event was available.
        /// </summary>
        EventUnavailable = 4,

        /// <summary>
        /// The space’s bounds are not known at the moment.
        /// </summary>
        SpaceBoundsUnavailable = 7,

        /// <summary>
        /// The session is not in the focused state.
        /// </summary>
        SessionNotFocused = 8,

        /// <summary>
        /// A frame has been discarded from composition.
        /// </summary>
        FrameDiscarded = 9,

        /// <summary>
        /// The function usage was invalid in some way.
        /// </summary>
        ValidationFailure = -1,

        /// <summary>
        /// The runtime failed to handle the function in an unexpected way that is not covered by another error result.
        /// </summary>
        RuntimeFailure = -2,

        /// <summary>
        /// A memory allocation has failed.
        /// </summary>
        OutOfMemory = -3,

        /// <summary>
        /// The runtime does not support the requested API version.
        /// </summary>
        ApiVersionUnsupported = -4,

        /// <summary>
        /// Initialization of object could not be completed.
        /// </summary>
        InitializationFailed = -6,

        /// <summary>
        /// The requested function was not found or is otherwise unsupported.
        /// </summary>
        FunctionUnsupported = -7,

        /// <summary>
        /// The requested feature is not supported.
        /// </summary>
        FeatureUnsupported = -8,

        /// <summary>
        /// A requested extension is not supported.
        /// </summary>
        ExtensionNotPresent = -9,

        /// <summary>
        /// The runtime supports no more of the requested resource.
        /// </summary>
        LimitReached = -10,

        /// <summary>
        /// The supplied size was smaller than required.
        /// </summary>
        SizeInsufficient = -11,

        /// <summary>
        /// A supplied object handle was invalid.
        /// </summary>
        HandleInvalid = -12,

        /// <summary>
        /// The `XrInstance` was lost or could not be found. It will need to be destroyed and optionally recreated.
        /// </summary>
        InstanceLost = -13,

        /// <summary>
        /// The session is already running.
        /// </summary>
        SessionRunning = -14,

        /// <summary>
        /// The session is not yet running.
        /// </summary>
        SessionNotRunning = -16,

        /// <summary>
        /// The `XrSession` was lost. It will need to be destroyed and optionally recreated.
        /// </summary>
        SessionLost = -17,

        /// <summary>
        /// The provided `XrSystemId` was invalid.
        /// </summary>
        SystemInvalid = -18,

        /// <summary>
        /// The provided `XrPath` was not valid.
        /// </summary>
        PathInvalid = -19,

        /// <summary>
        /// The maximum number of supported semantic paths has been reached.
        /// </summary>
        PathCountExceeded = -20,

        /// <summary>
        /// The semantic path character format is invalid.
        /// </summary>
        PathFormatInvalid = -21,

        /// <summary>
        /// The semantic path is unsupported.
        /// </summary>
        PathUnsupported = -22,

        /// <summary>
        /// The layer was NULL or otherwise invalid.
        /// </summary>
        LayerInvalid = -23,

        /// <summary>
        /// The number of specified layers is greater than the supported number.
        /// </summary>
        LayerLimitExceeded = -24,

        /// <summary>
        /// The image rect was negatively sized or otherwise invalid.
        /// </summary>
        SwapchainRectInvalid = -25,

        /// <summary>
        /// The image format is not supported by the runtime or platform.
        /// </summary>
        SwapchainFormatUnsupported = -26,

        /// <summary>
        /// The API used to retrieve an action’s state does not match the action’s type.
        /// </summary>
        ActionTypeMismatch = -27,

        /// <summary>
        /// The session is not in the ready state.
        /// </summary>
        SessionNotReady = -28,

        /// <summary>
        /// The session is not in the stopping state.
        /// </summary>
        SessionNotStopping = -29,

        /// <summary>
        /// The provided `XrTime` was zero, negative, or out of range.
        /// </summary>
        TimeInvalid = -30,

        /// <summary>
        /// The specified reference space is not supported by the runtime or system.
        /// </summary>
        ReferenceSpaceUnsupported = -31,

        /// <summary>
        /// The file could not be accessed.
        /// </summary>
        FileAccessError = -32,

        /// <summary>
        /// The file’s contents were invalid.
        /// </summary>
        FileContentsInvalid = -33,

        /// <summary>
        /// The specified form factor is not supported by the current runtime or platform.
        /// </summary>
        FormFactorUnsupported = -34,

        /// <summary>
        /// The specified form factor is supported, but the device is currently not available, e.g. not plugged in or powered off.
        /// </summary>
        FormFactorUnavailable = -35,

        /// <summary>
        /// A requested API layer is not present or could not be loaded.
        /// </summary>
        ApiLayerNotPresent = -36,

        /// <summary>
        /// The call was made without having made a previously required call.
        /// </summary>
        CallOrderInvalid = -37,

        /// <summary>
        /// The given graphics device is not in a valid state. The graphics device could be lost or initialized without meeting graphics requirements.
        /// </summary>
        GraphicsDeviceInvalid = -38,

        /// <summary>
        /// The supplied pose was invalid with respect to the requirements.
        /// </summary>
        PoseInvalid = -39,

        /// <summary>
        /// The supplied index was outside the range of valid indices.
        /// </summary>
        IndexOutOfRange = -40,

        /// <summary>
        /// The specified view configuration type is not supported by the runtime or platform.
        /// </summary>
        ViewConfigurationTypeUnsupported = -41,

        /// <summary>
        /// The specified environment blend mode is not supported by the runtime or platform.
        /// </summary>
        EnvironmentBlendModeUnsupported = -42,

        /// <summary>
        /// The name provided was a duplicate of an already-existing resource.
        /// </summary>
        NameDuplicated = -44,

        /// <summary>
        /// The name provided was invalid.
        /// </summary>
        NameInvalid = -45,

        /// <summary>
        /// A referenced action set is not attached to the session.
        /// </summary>
        ActionsetNotAttached = -46,

        /// <summary>
        /// The session already has attached action sets.
        /// </summary>
        ActionsetsAlreadyAttached = -47,

        /// <summary>
        /// The localized name provided was a duplicate of an already-existing resource.
        /// </summary>
        LocalizedNameDuplicated = -48,

        /// <summary>
        /// The localized name provided was invalid.
        /// </summary>
        LocalizedNameInvalid = -49,

        /// <summary>
        /// The `xrGetGraphicsRequirements`* call was not made before calling `xrCreateSession`.
        /// </summary>
        GraphicsRequirementsCallMissing = -50,

        /// <summary>
        /// The loader was unable to find or load a runtime.
        /// </summary>
        RuntimeUnavailable = -51,

        /// <summary>
        /// One or more of the extensions being enabled has dependency on extensions that are not enabled.
        /// </summary>
        ExtensionDependencyNotEnabled = -1000710001,

        /// <summary>
        /// Insufficient permissions. This error is included for use by vendor extensions. The precise definition of
        /// `PermissionInsufficient` and actions possible by the developer or user to resolve it can vary by platform,
        /// extension or function. The developer should refer to the documentation of the function that returned the
        /// error code and extension it was defined.
        /// </summary>
        PermissionInsufficient = -1000710000,

        /// <summary>
        /// `xrSetAndroidApplicationThreadKHR` failed as thread id is invalid. (Added by the `XR_KHR_android_thread_settings` extension)
        /// </summary>
        AndroidThreadSettingsIdInvalidKHR = -1000003000,

        /// <summary>
        /// `xrSetAndroidApplicationThreadKHR` failed setting the thread attributes/priority. (Added by the `XR_KHR_android_thread_settings` extension)
        /// </summary>
        [Obsolete("This enum value is misspelled and therefore deprecated in OpenXR Plug-in version 1.14.0. Use AndroidThreadSettingsFailureKHR instead.", false)]
        AndroidThreadSettingsdFailureKHR = -1000003001,

        /// <summary>
        /// `xrSetAndroidApplicationThreadKHR` failed setting the thread attributes/priority. (Added by the `XR_KHR_android_thread_settings` extension)
        /// </summary>
        AndroidThreadSettingsFailureKHR = -1000003001,

        /// <summary>
        /// Spatial anchor could not be created at that location. (Added by the `XR_MSFT_spatial_anchor` extension)
        /// </summary>
        CreateSpatialAnchorFailedMSFT = -1000039001,

        /// <summary>
        /// The secondary view configuration was not enabled when creating the session. (Added by the `XR_MSFT_secondary_view_configuration` extension)
        /// </summary>
        SecondaryViewConfigurationTypeNotEnabledMSFT = -1000053000,

        /// <summary>
        /// The controller model key is invalid. (Added by the `XR_MSFT_controller_model` extension)
        /// </summary>
        ControllerModelKeyInvalidMSFT = -1000055000,

        /// <summary>
        /// The reprojection mode is not supported. (Added by the `XR_MSFT_composition_layer_reprojection` extension)
        /// </summary>
        ReprojectionModeUnsupportedMSFT = -1000066000,

        /// <summary>
        /// Compute new scene not completed. (Added by the `XR_MSFT_scene_understanding` extension)
        /// </summary>
        ComputeNewSceneNotCompletedMSFT = -1000097000,

        /// <summary>
        /// Scene component id invalid. (Added by the `XR_MSFT_scene_understanding` extension)
        /// </summary>
        SceneComponentIdInvalidMSFT = -1000097001,

        /// <summary>
        /// Scene component type mismatch. (Added by the `XR_MSFT_scene_understanding` extension)
        /// </summary>
        SceneComponentTypeMismatchMSFT = -1000097002,

        /// <summary>
        /// Scene mesh buffer id invalid. (Added by the `XR_MSFT_scene_understanding` extension)
        /// </summary>
        SceneMeshBufferIdInvalidMSFT = -1000097003,

        /// <summary>
        /// Scene compute feature incompatible. (Added by the `XR_MSFT_scene_understanding` extension)
        /// </summary>
        SceneComputeFeatureIncompatibleMSFT = -1000097004,

        /// <summary>
        /// Scene compute consistency mismatch. (Added by the `XR_MSFT_scene_understanding` extension)
        /// </summary>
        SceneComputeConsistencyMismatchMSFT = -1000097005,

        /// <summary>
        /// The display refresh rate is not supported by the platform. (Added by the `XR_FB_display_refresh_rate` extension)
        /// </summary>
        DisplayRefreshRateUnsupportedFB = -1000101000,

        /// <summary>
        /// The color space is not supported by the runtime. (Added by the `XR_FB_color_space` extension)
        /// </summary>
        ColorSpaceUnsupportedFB = -1000108000,

        /// <summary>
        /// The component type is not supported for this space. (Added by the `XR_FB_spatial_entity` extension)
        /// </summary>
        SpaceComponentNotSupportedFB = -1000113000,

        /// <summary>
        /// The required component is not enabled for this space. (Added by the `XR_FB_spatial_entity` extension)
        /// </summary>
        SpaceComponentNotEnabledFB = -1000113001,

        /// <summary>
        /// A request to set the component’s status is currently pending. (Added by the `XR_FB_spatial_entity` extension)
        /// </summary>
        SpaceComponentStatusPendingFB = -1000113002,

        /// <summary>
        /// The component is already set to the requested value. (Added by the `XR_FB_spatial_entity` extension)
        /// </summary>
        SpaceComponentStatusAlreadySetFB = -1000113003,

        /// <summary>
        /// The object state is unexpected for the issued command. (Added by the `XR_FB_passthrough` extension)
        /// </summary>
        UnexpectedStatePassthroughFB = -1000118000,

        /// <summary>
        /// Trying to create an MR feature when one was already created and only one instance is allowed. (Added by the `XR_FB_passthrough` extension)
        /// </summary>
        FeatureAlreadyCreatedPassthroughFB = -1000118001,

        /// <summary>
        /// Requested functionality requires a feature to be created first. (Added by the `XR_FB_passthrough` extension)
        /// </summary>
        FeatureRequiredPassthroughFB = -1000118002,

        /// <summary>
        /// Requested functionality is not permitted - application is not allowed to perform the requested operation. (Added by the `XR_FB_passthrough` extension)
        /// </summary>
        NotPermittedPassthroughFB = -1000118003,

        /// <summary>
        /// There were insufficient resources available to perform an operation. (Added by the `XR_FB_passthrough` extension)
        /// </summary>
        InsufficientResourcesPassthroughFB = -1000118004,

        /// <summary>
        /// Unknown Passthrough error (no further details provided). (Added by the `XR_FB_passthrough` extension)
        /// </summary>
        UnknownPassthroughFB = -1000118050,

        /// <summary>
        /// The model key is invalid. (Added by the `XR_FB_render_model` extension)
        /// </summary>
        RenderModelKeyInvalidFB = -1000119000,

        /// <summary>
        /// The model is unavailable. (Added by the `XR_FB_render_model` extension)
        /// </summary>
        RenderModelUnavailableFB = 1000119020,

        /// <summary>
        /// Marker tracking is disabled or the specified marker is not currently tracked. (Added by the `XR_VARJO_marker_tracking` extension)
        /// </summary>
        MarkerNotTrackedVARJO = -1000124000,

        /// <summary>
        /// The specified marker ID is not valid. (Added by the `XR_VARJO_marker_tracking` extension)
        /// </summary>
        MarkerIdInvalidVARJO = -1000124001,

        /// <summary>
        /// The com.magicleap.permission.MARKER_TRACKING permission was denied. (Added by the `XR_ML_marker_understanding` extension)
        /// </summary>
        MarkerDetectorPermissionDeniedML = -1000138000,

        /// <summary>
        /// The specified marker could not be located spatially. (Added by the `XR_ML_marker_understanding` extension)
        /// </summary>
        MarkerDetectorLocateFailedML = -1000138001,

        /// <summary>
        /// The marker queried does not contain data of the requested type. (Added by the `XR_ML_marker_understanding` extension)
        /// </summary>
        MarkerDetectorInvalidDataQueryML = -1000138002,

        /// <summary>
        /// `createInfo` contains mutually exclusive parameters, such as setting `XR_MARKER_DETECTOR_CORNER_REFINE_METHOD_APRIL_TAG_ML` with `XR_MARKER_TYPE_ARUCO_ML`. (Added by the `XR_ML_marker_understanding` extension)
        /// </summary>
        MarkerDetectorInvalidCreateInfoML = -1000138003,

        /// <summary>
        /// The marker id passed to the function was invalid. (Added by the `XR_ML_marker_understanding` extension)
        /// </summary>
        MarkerInvalidML = -1000138004,

        /// <summary>
        /// The localization map being imported is not compatible with current OS or mode. (Added by the `XR_ML_localization_map` extension)
        /// </summary>
        LocalizationMapIncompatibleML = -1000139000,

        /// <summary>
        /// The localization map requested is not available. (Added by the `XR_ML_localization_map` extension)
        /// </summary>
        LocalizationMapUnavailableML = -1000139001,

        /// <summary>
        /// The map localization service failed to fulfill the request, retry later. (Added by the `XR_ML_localization_map` extension)
        /// </summary>
        LocalizationMapFailML = -1000139002,

        /// <summary>
        /// The com.magicleap.permission.SPACE_IMPORT_EXPORT permission was denied. (Added by the `XR_ML_localization_map` extension)
        /// </summary>
        LocalizationMapImportExportPermissionDeniedML = -1000139003,

        /// <summary>
        /// The com.magicleap.permission.SPACE_MANAGER permission was denied. (Added by the `XR_ML_localization_map` extension)
        /// </summary>
        LocalizationMapPermissionDeniedML = -1000139004,

        /// <summary>
        /// The map being imported already exists in the system. (Added by the `XR_ML_localization_map` extension)
        /// </summary>
        LocalizationMapAlreadyExistsML = -1000139005,

        /// <summary>
        /// The map localization service cannot export cloud based maps. (Added by the `XR_ML_localization_map` extension)
        /// </summary>
        LocalizationMapCannotExportCloudMapML = -1000139006,

        /// <summary>
        /// A spatial anchor was not found associated with the spatial anchor name provided (Added by the `XR_MSFT_spatial_anchor_persistence` extension)
        /// </summary>
        SpatialAnchorNameNotFoundMSFT = -1000142001,

        /// <summary>
        /// The spatial anchor name provided was not valid (Added by the `XR_MSFT_spatial_anchor_persistence` extension)
        /// </summary>
        SpatialAnchorNameInvalidMSFT = -1000142002,

        /// <summary>
        /// Marker does not encode a string. (Added by the `XR_MSFT_scene_marker` extension)
        /// </summary>
        SceneMarkerDataNotStringMSFT = 1000147000,

        /// <summary>
        /// Anchor import from cloud or export from device failed. (Added by the `XR_FB_spatial_entity_sharing` extension)
        /// </summary>
        SpaceMappingInsufficientFB = -1000169000,

        /// <summary>
        /// Anchors were downloaded from the cloud but failed to be imported/aligned on the device. (Added by the `XR_FB_spatial_entity_sharing` extension)
        /// </summary>
        SpaceLocalizationFailedFB = -1000169001,

        /// <summary>
        /// Timeout occurred while waiting for network request to complete. (Added by the `XR_FB_spatial_entity_sharing` extension)
        /// </summary>
        SpaceNetworkTimeoutFB = -1000169002,

        /// <summary>
        /// The network request failed. (Added by the `XR_FB_spatial_entity_sharing` extension)
        /// </summary>
        SpaceNetworkRequestFailedFB = -1000169003,

        /// <summary>
        /// Cloud storage is required for this operation but is currently disabled. (Added by the `XR_FB_spatial_entity_sharing` extension)
        /// </summary>
        SpaceCloudStorageDisabledFB = -1000169004,

        /// <summary>
        /// The provided data buffer did not match the required size. (Added by the `XR_META_passthrough_color_lut` extension)
        /// </summary>
        PassthroughColorLutBufferSizeMismatchMETA = -1000266000,

        /// <summary>
        /// Warning: The requested depth image is not yet available. (Added by the `XR_META_environment_depth` extension)
        /// </summary>
        EnvironmentDepthNotAvailableMETA = 1000291000,

        /// <summary>
        /// Tracking optimization hint is already set for the domain. (Added by the `XR_QCOM_tracking_optimization_settings` extension)
        /// </summary>
        HintAlreadySetQCOM = -1000306000,

        /// <summary>
        /// The provided space is valid but not an anchor. (Added by the `XR_HTC_anchor` extension)
        /// </summary>
        NotAnAnchorHTC = -1000319000,

        /// <summary>
        /// The space passed to the function was not locatable. (Added by the `XR_EXT_plane_detection` extension)
        /// </summary>
        SpaceNotLocatableEXT = -1000429000,

        /// <summary>
        /// The permission for this resource was not granted. (Added by the `XR_EXT_plane_detection` extension)
        /// </summary>
        PlaneDetectionPermissionDeniedEXT = -1000429001,

        /// <summary>
        /// Returned by completion function to indicate future is not ready. (Added by the `XR_EXT_future` extension)
        /// </summary>
        FuturePendingEXT = -1000469001,

        /// <summary>
        /// Returned by completion function to indicate future is not valid. (Added by the `XR_EXT_future` extension)
        /// </summary>
        FutureInvalidEXT = -1000469002,

        /// <summary>
        /// Provided by `XR_KHR_maintenance1`
        /// </summary>
        ExtensionDependencyNotEnabledKHR = ExtensionDependencyNotEnabled,

        /// <summary>
        /// Provided by `XR_KHR_maintenance1`
        /// </summary>
        PermissionInsufficientKHR = PermissionInsufficient,

        /// <summary>
        /// Max XR Result Value
        /// </summary>
        MaxResult = 0x7FFFFFFF
    }

    /// <summary>
    /// Extension methods for <see cref="XrResult"/>.
    /// </summary>
    public static class XrResultExtensions
    {
        /// <summary>
        /// Indicates whether the `XrResult` represents a success code, inclusive of <see cref="XrResult.Success"/>.
        /// </summary>
        /// <param name="xrResult">The `XrResult`.</param>
        /// <returns><see langword="true"/> if the `XrResult` represents a success code. Otherwise, <see langword="false"/>.</returns>
        public static bool IsSuccess(this XrResult xrResult)
        {
            return xrResult >= 0;
        }

        /// <summary>
        /// Indicates whether the `XrResult` represents an unqualified success, ie <see cref="XrResult.Success"/>.
        /// </summary>
        /// <param name="xrResult">The `XrResult`.</param>
        /// <returns><see langword="true"/> if the `XrResult` represents an unqualified success. Otherwise, <see langword="false"/>.</returns>
        public static bool IsUnqualifiedSuccess(this XrResult xrResult)
        {
            return xrResult == 0;
        }

        /// <summary>
        /// Indicates whether the `XrResult` represents an error code.
        /// </summary>
        /// <param name="xrResult">The `XrResult`.</param>
        /// <returns><see langword="true"/> if the `XrResult` represents an error code. Otherwise, <see langword="false"/>.</returns>
        public static bool IsError(this XrResult xrResult)
        {
            return xrResult < 0;
        }
    }

    /// <summary>
    /// Runtime View Configuration. <see cref="Features.Mock.MockRuntime.SetViewState"/>
    /// </summary>
    public enum XrViewConfigurationType
    {
        /// <summary>
        /// Select Mono view for runtime.
        /// </summary>
        PrimaryMono = 1,

        /// <summary>
        /// Select Stereo view for runtime.
        /// </summary>
        PrimaryStereo = 2,

        /// <summary>
        /// Select Quad view for runtime.
        /// </summary>
        PrimaryQuadVarjo = 1000037000,

        /// <summary>
        /// Select first person mono view for runtime.
        /// </summary>
        SecondaryMonoFirstPersonObserver = 1000054000,

        /// <summary>
        /// Select third person mono view for runtime.
        /// </summary>
        SecondaryMonoThirdPersonObserver = 1000145000
    }

    /// <summary>
    /// Space Location bit flags. <see cref="Features.Mock.MockRuntime.SetSpace"/>
    /// </summary>
    [Flags]
    public enum XrSpaceLocationFlags
    {
        /// <summary>
        /// Default space location flag.
        /// </summary>
        None = 0,

        /// <summary>
        /// Orientation Valid bit.
        /// </summary>
        OrientationValid = 1,

        /// <summary>
        /// Position Valid bit.
        /// </summary>
        PositionValid = 2,

        /// <summary>
        /// Oriention Tracked bit.
        /// </summary>
        OrientationTracked = 4,

        /// <summary>
        /// Position Tracked bit.
        /// </summary>
        PositionTracked = 8
    }

    /// <summary>
    /// Runtime view state flags. <see cref="Features.Mock.MockRuntime.SetViewState"/>
    /// </summary>
    [Flags]
    public enum XrViewStateFlags
    {
        /// <summary>
        /// Default view state flag.
        /// </summary>
        None = 0,

        /// <summary>
        /// Orientation Valid bit.
        /// </summary>
        OrientationValid = 1,

        /// <summary>
        /// Position Valid bit.
        /// </summary>
        PositionValid = 2,

        /// <summary>
        /// Oriention Tracked bit.
        /// </summary>
        OrientationTracked = 4,

        /// <summary>
        /// Position Tracked bit.
        /// </summary>
        PositionTracked = 8
    }

    /// <summary>
    /// Runtime XR Reference Space. <see cref="Features.Mock.MockRuntime.SetSpace"/>
    /// </summary>
    [Flags]
    public enum XrReferenceSpaceType
    {
        /// <summary>
        /// View space.
        /// </summary>
        View = 1,

        /// <summary>
        /// Local space.
        /// </summary>
        Local = 2,

        /// <summary>
        /// Stage space.
        /// </summary>
        Stage = 3,

        /// <summary>
        /// Unbounded (Microsoft specific).
        /// </summary>
        UnboundedMsft = 1000038000,

        /// <summary>
        /// Combined Eye (Varjo specific).
        /// </summary>
        CombinedEyeVarjo = 1000121000
    }

    /// <summary>
    /// Runtime XR Session State. <see cref="Features.Mock.MockRuntime.TransitionToState"/>
    /// </summary>
    public enum XrSessionState
    {
        /// <summary>
        /// Session State Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Session State Idle.
        /// </summary>
        Idle = 1,

        /// <summary>
        /// Session State Ready.
        /// </summary>
        Ready = 2,

        /// <summary>
        /// Session State Synchronized.
        /// </summary>
        Synchronized = 3,

        /// <summary>
        /// Session State Visible.
        /// </summary>
        Visible = 4,

        /// <summary>
        /// Session State Focused.
        /// </summary>
        Focused = 5,

        /// <summary>
        /// Session State Stopping.
        /// </summary>
        Stopping = 6,

        /// <summary>
        /// Session State Loss Pending.
        /// </summary>
        LossPending = 7,

        /// <summary>
        /// Session State Exiting.
        /// </summary>
        Exiting = 8,
    }

    /// <summary>
    /// Values for type members of structs.
    /// </summary>
    public enum XrStructureType : uint
    {
        /// <summary>
        /// Unknown struct type.
        /// </summary>
        XR_TYPE_UNKNOWN = 0,

        /// <summary>
        /// Struct type for creating a swapchain.
        /// </summary>
        XR_TYPE_SWAPCHAIN_CREATE_INFO = 9,

        /// <summary>
        /// Struct type for creating a projection composition layer.
        /// </summary>
        XR_TYPE_COMPOSITION_LAYER_PROJECTION = 35,

        /// <summary>
        /// Struct type for creating a quad composition layer.
        /// </summary>
        XR_TYPE_COMPOSITION_LAYER_QUAD = 36,

        /// <summary>
        /// Struct type that a projection composition layer comprises of.
        /// </summary>
        XR_TYPE_COMPOSITION_LAYER_PROJECTION_VIEW = 48,

        /// <summary>
        /// Struct type for creating a cube composition layer.
        /// </summary>
        XR_TYPE_COMPOSITION_LAYER_CUBE_KHR = 1000006000,

        /// <summary>
        /// Struct type for creating a cylinder composition layer.
        /// </summary>
        XR_TYPE_COMPOSITION_LAYER_CYLINDER_KHR = 1000017000,

        /// <summary>
        /// Struct type for creating an equirect composition layer.
        /// </summary>
        XR_TYPE_COMPOSITION_LAYER_EQUIRECT_KHR = 1000018000,

        /// <summary>
        /// Struct type for creating an equirect2 composition layer.
        /// </summary>
        XR_TYPE_COMPOSITION_LAYER_EQUIRECT2_KHR = 1000091000,
    }


    /// <summary>
    /// Values to specify the intended usage of swapchain images.
    /// </summary>
    [Flags]
    public enum XrSwapchainUsageFlags : ulong
    {
        /// <summary>
        /// Specifies that the image may be a color rendering target.
        /// </summary>
        XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT = 0x00000001,

        /// <summary>
        ///  Specifies that the image may be a depth/stencil rendering target.
        /// </summary>
        XR_SWAPCHAIN_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT = 0x00000002,

        /// <summary>
        /// Specifies that the image may be accessed out of order and that access may be via atomic operations.
        /// </summary>
        XR_SWAPCHAIN_USAGE_UNORDERED_ACCESS_BIT = 0x00000004,

        /// <summary>
        /// Specifies that the image may be used as the source of a transfer operation.
        /// </summary>
        XR_SWAPCHAIN_USAGE_TRANSFER_SRC_BIT = 0x00000008,

        /// <summary>
        /// Specifies that the image may be used as the destination of a transfer operation.
        /// </summary>
        XR_SWAPCHAIN_USAGE_TRANSFER_DST_BIT = 0x00000010,

        /// <summary>
        /// Specifies that the image may be sampled by a shader.
        /// </summary>
        XR_SWAPCHAIN_USAGE_SAMPLED_BIT = 0x00000020,

        /// <summary>
        /// Specifies that the image may be reinterpreted as another image format
        /// </summary>
        XR_SWAPCHAIN_USAGE_MUTABLE_FORMAT_BIT = 0x00000040,

        /// <summary>
        /// Specifies that the image may be used as a input attachment. (Added by the XR_MND_swapchain_usage_input_attachment_bit extension)
        /// </summary>
        XR_SWAPCHAIN_USAGE_INPUT_ATTACHMENT_BIT_MND = 0x00000080,

        /// <summary>
        ///  Specifies that the image may be used as a input attachment. (Added by the XR_KHR_swapchain_usage_input_attachment_bit extension)
        /// </summary>
        XR_SWAPCHAIN_USAGE_INPUT_ATTACHMENT_BIT_KHR = 0x00000080
    }


    /// <summary>
    /// Convenience type for iterating (read only).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XrBaseInStructure
    {
        /// <summary>
        /// The XrStructureType of this structure. This base structure itself has no associated XrStructureType value.
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;
    }

    /// <summary>
    /// Two-dimensional vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrVector2f
    {
        /// <summary>
        /// The x coordinate of the vector.
        /// </summary>
        public float X;

        /// <summary>
        /// The y coordinate of the vector.
        /// </summary>
        public float Y;

        /// <summary>
        /// Constructor for two float values.
        /// </summary>
        /// <param name="x">The x coordinate of the vector.</param>
        /// <param name="y">The y coordinate of the vector.</param>
        public XrVector2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Initializes and returns an instance of XrVector2f with the provided parameters.
        /// </summary>
        /// <param name="value">Vector2 struct coming from Unity that is translated into the OpenXR XrVector2f struct.</param>
        public XrVector2f(Vector2 value)
        {
            X = value.x;
            Y = value.y;
        }
    };

    /// <summary>
    /// Three-dimensional vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrVector3f
    {
        /// <summary>
        /// The x coordinate of the vector.
        /// </summary>
        public float X;

        /// <summary>
        /// The y coordinate of the vector.
        /// </summary>
        public float Y;

        /// <summary>
        /// The z coordinate of the vector.
        /// </summary>
        public float Z;

        /// <summary>
        /// Initializes and returns an instance of XrVector3f with the provided parameters.
        /// </summary>
        /// <param name="x">The x coordinate of the vector.</param>
        /// <param name="y">The y coordinate of the vector.</param>
        /// <param name="z">The z coordinate of the vector.</param>
        public XrVector3f(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = -z;
        }

        /// <summary>
        /// Initializes and returns an instance of XrVector3f with the provided parameters.
        /// </summary>
        /// <param name="value">Vector3 struct coming from Unity that is translated into the OpenXR XrVector3f struct.</param>
        public XrVector3f(Vector3 value)
        {
            X = value.x;
            Y = value.y;
            Z = -value.z;
        }
    };

    /// <summary>
    /// Unit Quaternion.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrQuaternionf
    {
        /// <summary>
        /// The x coordinate of the quaternion.
        /// </summary>
        public float X;

        /// <summary>
        /// The y coordinate of the quaternion.
        /// </summary>
        public float Y;

        /// <summary>
        /// The z coordinate of the quaternion.
        /// </summary>
        public float Z;

        /// <summary>
        /// The w coordinate of the quaternion.
        /// </summary>
        public float W;

        /// <summary>
        /// Initializes and returns an instance of XrQuaternionf with the provided parameters.
        /// </summary>
        /// <param name="x">The x coordinate of the quaternion.</param>
        /// <param name="y">The y coordinate of the quaternion.</param>
        /// <param name="z">The z coordinate of the quaternion.</param>
        /// <param name="w">The w coordinate of the quaternion.</param>
        public XrQuaternionf(float x, float y, float z, float w)
        {
            this.X = -x;
            this.Y = -y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Initializes and returns an instance of XrQuaternionf with the provided parameters.
        /// </summary>
        /// <param name="quaternion">Quaternion struct coming from Unity that is translated into the OpenXR XrQuaternionf struct.</param>
        public XrQuaternionf(Quaternion quaternion)
        {
            this.X = -quaternion.x;
            this.Y = -quaternion.y;
            this.Z = quaternion.z;
            this.W = quaternion.w;
        }
    };

    /// <summary>
    /// A construct representing a position and orientation within a space, with position expressed in meters, and orientation represented as a unit quaternion.
    /// <a href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrPosef.html">OpenXR Spec</a>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrPosef
    {
        /// <summary>
        /// The orientation/rotation of the pose.
        /// </summary>
        public XrQuaternionf Orientation;

        /// <summary>
        /// The position of the pose.
        /// </summary>
        public XrVector3f Position;

        /// <summary>
        /// Initializes and returns an instance of XrPosef with the provided parameters.
        /// </summary>
        /// <param name="vec3">vector3 position.</param>
        /// <param name="quaternion">quaternion orientation.</param>
        public XrPosef(Vector3 vec3, Quaternion quaternion)
        {
            this.Position = new XrVector3f(vec3);
            this.Orientation = new XrQuaternionf(quaternion);
        }
    };

    /// <summary>
    /// Creation info for a swapchain.
    /// <a href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrSwapchainCreateInfo.html">OpenXR Spec</a>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrSwapchainCreateInfo
    {
        /// <summary>
        /// The XrStructureType of this structure.
        /// <a href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrStructureType.html">OpenXR Spec</a>
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain. Can be null.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrSwapchainCreateFlagBits describing additional properties of the swapchain.
        /// <a href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrSwapchainCreateFlagBits.html">OpenXR Spec</a>
        /// </summary>
        public ulong CreateFlags;

        /// <summary>
        /// Bitmask of XrSwapchainUsageFlagBits describing the intended usage of the swapchain�s images.
        /// The usage flags define how the corresponding graphics API objects are created.
        /// A mismatch may result in swapchain images that do not support the application�s usage.
        /// <a href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrSwapchainUsageFlagBits.html">OpenXR Spec</a>
        /// </summary>
        public ulong UsageFlags;

        /// <summary>
        /// The graphics API-specific texture format identifier.
        /// Can use OpenXRLayerUtility.GetDefaultColorFormat() to get the default format.
        /// </summary>
        public long Format;

        /// <summary>
        /// The number of sub-data element samples in the image, must not be 0 or greater than the graphics API�s maximum limit.
        /// </summary>
        public uint SampleCount;

        /// <summary>
        /// The width of the image, must not be 0 or greater than the graphics API�s maximum limit.
        /// </summary>
        public uint Width;

        /// <summary>
        /// The height of the image, must not be 0 or greater than the graphics API�s maximum limit.
        /// </summary>
        public uint Height;

        /// <summary>
        /// The number of faces, which can be either 6 (for cubemaps) or 1.
        /// </summary>
        public uint FaceCount;

        /// <summary>
        /// The number of array layers in the image or 1 for a 2D image, must not be 0 or greater than the graphics API�s maximum limit.
        /// </summary>
        public uint ArraySize;

        /// <summary>
        /// Describes the number of levels of detail available for minified sampling of the image, must not be 0 or greater than the graphics APIs maximum limit.
        /// </summary>
        public uint MipCount;
    }

#if XR_COMPOSITION_LAYERS

    /// <summary>
    /// Specifies options for individual composition layers, and contains a bitwise-OR of zero or more of the bits.
    /// </summary>
    [Flags]
    public enum XrCompositionLayerFlags : ulong
    {
        /// <summary>
        /// Enables chromatic aberration correction when not done by default. This flag has no effect on any known conformant runtime, and is planned for deprecation for OpenXR 1.1
        /// </summary>
        CorrectChromaticAberration = 1,

        /// <summary>
        /// Enables the layer texture alpha channel.
        /// </summary>
        SourceAlpha = 2,

        /// <summary>
        /// Indicates the texture color channels have not been premultiplied by the texture alpha channel.
        /// </summary>
        UnPremultipliedAlpha = 4
    }

    /// <summary>
    /// Offset in two dimensions
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrOffset2Di
    {
        /// <summary>
        /// The integer offset in the x direction.
        /// </summary>
        public int X;

        /// <summary>
        /// The integer offset in the y direction.
        /// </summary>
        public int Y;
    }

    /// <summary>
    /// Extent  in two dimensions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrExtent2Di
    {
        /// <summary>
        /// The integer width of the extent.
        /// </summary>
        public int Width;

        /// <summary>
        /// The integer height of the extent.
        /// </summary>
        public int Height;
    }

    /// <summary>
    /// Extent  in two dimensions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrExtent2Df
    {
        /// <summary>
        /// The floating-point width of the extent.
        /// </summary>
        public float Width;

        /// <summary>
        /// The floating-point height of the extent.
        /// </summary>
        public float Height;
    }

    /// <summary>
    /// Rect in two dimensions, integer values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrRect2Di
    {
        /// <summary>
        /// The XrOffset2Di specifying the integer rectangle offset.
        /// </summary>
        public XrOffset2Di Offset;

        /// <summary>
        /// The XrExtent2Di specifying the integer rectangle extent.
        /// </summary>
        public XrExtent2Di Extent;
    }


    /// <summary>
    /// Field of view.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrFovf
    {
        /// <summary>
        /// The angle of the left side of the field of view. For a symmetric field of view this value is negative.
        /// </summary>
        public float AngleLeft;

        /// <summary>
        /// The angle of the right side of the field of view.
        /// </summary>
        public float AngleRight;

        /// <summary>
        /// The angle of the top part of the field of view.
        /// </summary>
        public float AngleUp;

        /// <summary>
        /// The angle of the bottom part of the field of view. For a symmetric field of view this value is negative.
        /// </summary>
        public float AngleDown;
    }

    /// <summary>
    /// Composition layer data describing the associated swapchain.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrSwapchainSubImage
    {
        /// <summary>
        /// The XrSwapchain to be displayed.
        /// </summary>
        public ulong Swapchain;

        /// <summary>
        /// An XrRect2Di representing the valid portion of the image to use, in pixels.
        /// It also implicitly defines the transform from normalized image coordinates into pixel coordinates.
        /// The coordinate origin depends on which graphics API is being used.
        /// See the graphics API extension details for more information on the coordinate origin definition. Note that the compositor may bleed in pixels from outside the bounds in some cases, for instance due to mipmapping.
        /// </summary>
        public XrRect2Di ImageRect;

        /// <summary>
        /// Composition layer data describing the associated swapchain.
        /// </summary>
        public uint ImageArrayIndex;
    }

    /// <summary>
    /// Composition layer base header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerBaseHeader
    {
        /// <summary>
        /// The XrStructureType of this structure. This base structure itself has no associated XrStructureType value
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrCompositionLayerFlags describing flags to apply to the layer.
        /// </summary>
        public XrCompositionLayerFlags LayerFlags;

        /// <summary>
        /// The XrSpace in which the layer will be kept stable over time.
        /// </summary>
        public ulong Space;
    }

    /// <summary>
    /// Quad composition layer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerQuad
    {
        /// <summary>
        /// The XrStructureType of this structure.
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrCompositionLayerFlags describing flags to apply to the layer.
        /// </summary>
        public XrCompositionLayerFlags LayerFlags;

        /// <summary>
        /// The XrSpace in which the layer will be kept stable over time.
        /// </summary>
        public ulong Space;

        /// <summary>
        /// The XrEyeVisibility for this layer.
        /// </summary>
        public uint EyeVisibility;

        /// <summary>
        /// The image layer XrSwapchainSubImage to use. The swapchain must have been created with a XrSwapchainCreateInfo.faceCount of 1.
        /// </summary>
        public XrSwapchainSubImage SubImage;

        /// <summary>
        /// An XrPosef defining the position and orientation of the quad in the reference frame of the space.
        /// </summary>
        public XrPosef Pose;

        /// <summary>
        /// The width and height of the quad in meters.
        /// </summary>
        public XrExtent2Df Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerCylinderKHR
    {
        /// <summary>
        /// The XrStructureType of this structure.
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrCompositionLayerFlags describing flags to apply to the layer.
        /// </summary>
        public XrCompositionLayerFlags LayerFlags;

        /// <summary>
        /// The XrSpace in which the layer will be kept stable over time.
        /// </summary>
        public ulong Space;

        /// <summary>
        /// The XrEyeVisibility for this layer.
        /// </summary>
        public uint EyeVisibility;

        /// <summary>
        /// The image layer XrSwapchainSubImage to use. The swapchain must have been created with a XrSwapchainCreateInfo.faceCount of 1.
        /// </summary>
        public XrSwapchainSubImage SubImage;

        /// <summary>
        /// An XrPosef defining the position and orientation of the center point of the view of the cylinder within the reference frame of the space.
        /// </summary>
        public XrPosef Pose;

        /// <summary>
        /// The non-negative radius of the cylinder. Values of zero or floating point positive infinity are treated as an infinite cylinder.
        /// </summary>
        public float Radius;

        /// <summary>
        /// The angle of the visible section of the cylinder, based at 0 radians, in the range of [0, 2π). It grows symmetrically around the 0 radian angle.
        /// </summary>
        public float CentralAngle;

        /// <summary>
        /// The ratio of the visible cylinder section width / height. The height of the cylinder is given by: (cylinder radius × cylinder angle) / aspectRatio.
        /// </summary>
        public float AspectRatio;
    }

    /// <summary>
    /// Struct containing view projection state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrView
    {
        /// <summary>
        /// The XrStructureType of this structure.
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// An XrPosef defining the location and orientation of the view in the space specified by the xrLocateViews function.
        /// </summary>
        public XrPosef Pose;

        /// <summary>
        /// The XrFovf for the four sides of the projection.
        /// </summary>
        public XrFovf Fov;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerProjection
    {
        /// <summary>
        /// The XrStructureType of this structure.
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrCompositionLayerFlags describing flags to apply to the layer.
        /// </summary>
        public XrCompositionLayerFlags LayerFlags;

        /// <summary>
        /// The XrSpace in which the layer will be kept stable over time.
        /// </summary>
        public ulong Space;

        /// <summary>
        /// The count of views in the views array. This must be equal to the number of view poses returned by xrLocateViews.
        /// </summary>
        public uint ViewCount;

        /// <summary>
        /// The array of type XrCompositionLayerProjectionView containing each projection layer view.
        /// </summary>
        public XrCompositionLayerProjectionView* Views;
    }

    /// <summary>
    /// Projection layer element
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerProjectionView
    {
        /// <summary>
        /// The XrStructureType of this structure.
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// An XrPosef defining the location and orientation of this projection element in the space of the corresponding XrCompositionLayerProjectionView.
        /// </summary>
        public XrPosef Pose;

        /// <summary>
        /// The XrFovf for this projection element.
        /// </summary>
        public XrFovf Fov;

        /// <summary>
        /// The image layer XrSwapchainSubImage to use. The swapchain must have been created with a XrSwapchainCreateInfo.faceCount of 1.
        /// </summary>
        public XrSwapchainSubImage SubImage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerCubeKHR
    {
        /// <summary>
        /// The XrStructureType of this structure. This base structure itself has no associated XrStructureType value
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrCompositionLayerFlags describing flags to apply to the layer.
        /// </summary>
        public XrCompositionLayerFlags LayerFlags;

        /// <summary>
        /// The XrSpace in which the layer will be kept stable over time.
        /// </summary>
        public ulong Space;

        /// <summary>
        /// The XrEyeVisibility for this layer.
        /// </summary>
        public uint EyeVisibility;

        /// <summary>
        /// The swapchain, which must have been created with a XrSwapchainCreateInfo.faceCount of 6.
        /// </summary>
        public ulong Swapchain;

        /// <summary>
        /// The image array index, with 0 meaning the first or only array element.
        /// </summary>
        public uint ImageArrayIndex;

        /// <summary>
        /// The orientation of the environment map in the space.
        /// </summary>
        public XrQuaternionf Orientation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerEquirectKHR
    {
        /// <summary>
        /// The XrStructureType of this structure. This base structure itself has no associated XrStructureType value
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrCompositionLayerFlags describing flags to apply to the layer.
        /// </summary>
        public XrCompositionLayerFlags LayerFlags;

        /// <summary>
        /// The XrSpace in which the layer will be kept stable over time.
        /// </summary>
        public ulong Space;

        /// <summary>
        /// The eye represented by this layer.
        /// </summary>
        public uint EyeVisibility;

        /// <summary>
        /// Identifies the image XrSwapchainSubImage to use. The swapchain must have been created with a XrSwapchainCreateInfo.faceCount of 1.
        /// </summary>
        public XrSwapchainSubImage SubImage;

        /// <summary>
        /// An XrPosef defining the position and orientation of the center point of the sphere onto which the equirect image data is mapped, relative to the reference frame of the space.
        /// </summary>
        public XrPosef Pose;

        /// <summary>
        /// The non-negative radius of the sphere onto which the equirect image data is mapped. Values of zero or floating point positive infinity are treated as an infinite sphere.
        /// </summary>
        public float Radius;

        /// <summary>
        /// An XrVector2f indicating a scale of the texture coordinates after the mapping to 2D.
        /// </summary>
        public XrVector2f Scale;

        /// <summary>
        /// An XrVector2f indicating a bias of the texture coordinates after the mapping to 2D.
        /// </summary>
        public XrVector2f Bias;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrCompositionLayerEquirect2KHR
    {
        /// <summary>
        /// The XrStructureType of this structure. This base structure itself has no associated XrStructureType value
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrCompositionLayerFlags describing flags to apply to the layer.
        /// </summary>
        public XrCompositionLayerFlags LayerFlags;

        /// <summary>
        /// The XrSpace in which the layer will be kept stable over time.
        /// </summary>
        public ulong Space;

        /// <summary>
        /// The eye represented by this layer.
        /// </summary>
        public uint EyeVisibility;

        /// <summary>
        /// Identifies the image XrSwapchainSubImage to use. The swapchain must have been created with a XrSwapchainCreateInfo.faceCount of 1.
        /// </summary>
        public XrSwapchainSubImage SubImage;

        /// <summary>
        /// An XrPosef defining the position and orientation of the center point of the sphere onto which the equirect image data is mapped, relative to the reference frame of the space.
        /// </summary>
        public XrPosef Pose;

        /// <summary>
        /// The non-negative radius of the sphere onto which the equirect image data is mapped. Values of zero or floating point positive infinity are treated as an infinite sphere.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Defines the visible horizontal angle of the sphere, based at 0 radians, in the range of [0, 2π]. It grows symmetrically around the 0 radian angle.
        /// </summary>
        public float CentralHorizontalAngle;

        /// <summary>
        /// Defines the upper vertical angle of the visible portion of the sphere, in the range of [-π/2, π/2].
        /// </summary>
        public float UpperVerticalAngle;

        /// <summary>
        /// Defines the lower vertical angle of the visible portion of the sphere, in the range of [-π/2, π/2].
        /// </summary>
        public float LowerVerticalAngle;

    }

#endif
}
