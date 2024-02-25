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
        /// Success
        /// </summary>
        Success = 0,

        /// <summary>
        /// Timeout
        /// </summary>
        TimeoutExpored = 1,

        /// <summary>
        /// Connection Loss Pending
        /// </summary>
        LossPending = 3,

        /// <summary>
        /// Event Unavailable
        /// </summary>
        EventUnavailable = 4,

        /// <summary>
        /// Bounds Unavailable
        /// </summary>
        SpaceBoundsUnavailable = 7,

        /// <summary>
        /// No Session Focus
        /// </summary>
        SessionNotFocused = 8,

        /// <summary>
        /// Frame Discarded
        /// </summary>
        FrameDiscarded = 9,

        /// <summary>
        /// Validation Failure
        /// </summary>
        ValidationFailure = -1,

        /// <summary>
        /// Runtime Failure
        /// </summary>
        RuntimeFailure = -2,

        /// <summary>
        /// Out of Memory
        /// </summary>
        OutOfMemory = -3,

        /// <summary>
        /// API version Unsupported
        /// </summary>
        ApiVersionUnsupported = -4,

        /// <summary>
        /// Initialization Failure
        /// </summary>
        InitializationFailed = -6,

        /// <summary>
        /// Function Unsupported
        /// </summary>
        FunctionUnsupported = -7,

        /// <summary>
        /// Feature Unsupported
        /// </summary>
        FeatureUnsupported = -8,

        /// <summary>
        /// Extension Not Available
        /// </summary>
        ExtensionNotPresent = -9,

        /// <summary>
        /// Limit Reached
        /// </summary>
        LimitReached = -10,

        /// <summary>
        /// Insufficient Size
        /// </summary>
        SizeInsufficient = -11,

        /// <summary>
        /// Handle Invalid
        /// </summary>
        HandleInvalid = -12,

        /// <summary>
        /// Instance Lost
        /// </summary>
        InstanceLost = -13,

        /// <summary>
        /// Session Running
        /// </summary>
        SessionRunning = -14,

        /// <summary>
        /// Session Not Running
        /// </summary>
        SessionNotRunning = -16,

        /// <summary>
        /// Session Lost
        /// </summary>
        SessionLost = -17,

        /// <summary>
        /// System Invalid
        /// </summary>
        SystemInvalid = -18,

        /// <summary>
        /// Path Invalid
        /// </summary>
        PathInvalid = -19,

        /// <summary>
        /// Path Count Exceeded
        /// </summary>
        PathCountExceeded = -20,

        /// <summary>
        /// Path Format Invalid
        /// </summary>
        PathFormatInvalid = -21,

        /// <summary>
        /// Path Unsupported
        /// </summary>
        PathUnsupported = -22,

        /// <summary>
        /// Layer Invalid
        /// </summary>
        LayerInvalid = -23,

        /// <summary>
        /// Layer Limit Exceeded
        /// </summary>
        LayerLimitExceeded = -24,

        /// <summary>
        /// Swapchain Rect Invalid
        /// </summary>
        SwapchainRectInvalid = -25,

        /// <summary>
        /// Swapchain Rect Unsupported
        /// </summary>
        SwapchainFormatUnsupported = -26,

        /// <summary>
        /// Action Type Mismatch
        /// </summary>
        ActionTypeMismatch = -27,

        /// <summary>
        /// Session Not Ready
        /// </summary>
        SessionNotReady = -28,

        /// <summary>
        /// Session Not Stopping
        /// </summary>
        SessionNotStopping = -29,

        /// <summary>
        /// Time Invalid
        /// </summary>
        TimeInvalid = -30,

        /// <summary>
        /// Reference Space Unsupported
        /// </summary>
        ReferenceSpaceUnsupported = -31,

        /// <summary>
        /// File Access Error
        /// </summary>
        FileAccessError = -32,

        /// <summary>
        /// File Contents Invalid
        /// </summary>
        FileContentsInvalid = -33,

        /// <summary>
        /// Form Factor Unsupported
        /// </summary>
        FormFactorUnsupported = -34,

        /// <summary>
        /// Form Factor Unavailable
        /// </summary>
        FormFactorUnavailable = -35,

        /// <summary>
        /// API Layer Not Present
        /// </summary>
        ApiLayerNotPresent = -36,

        /// <summary>
        /// Call Order Invalid
        /// </summary>
        CallOrderInvalid = -37,

        /// <summary>
        /// Graphics Device Invalid
        /// </summary>
        GraphicsDeviceInvalid = -38,

        /// <summary>
        /// Pose Invalid
        /// </summary>
        PoseInvalid = -39,

        /// <summary>
        /// Index Out of Range
        /// </summary>
        IndexOutOfRange = -40,

        /// <summary>
        /// View Configuration Type Unsupported
        /// </summary>
        ViewConfigurationTypeUnsupported = -41,

        /// <summary>
        /// Environment Blend Mode Unsupported
        /// </summary>
        EnvironmentBlendModeUnsupported = -42,

        /// <summary>
        /// Name Duplicated
        /// </summary>
        NameDuplicated = -44,

        /// <summary>
        /// Name Invalid
        /// </summary>
        NameInvalid = -45,

        /// <summary>
        /// Actionset Not Attached
        /// </summary>
        ActionsetNotAttached = -46,

        /// <summary>
        /// Actionset Already Attached
        /// </summary>
        ActionsetsAlreadyAttached = -47,

        /// <summary>
        /// Localized Name Duplicated
        /// </summary>
        LocalizedNameDuplicated = -48,

        /// <summary>
        /// Localized Name Invalid
        /// </summary>
        LocalizedNameInvalid = -49,

        /// <summary>
        /// Android Thread Settings Id Invalid
        /// </summary>
        AndroidThreadSettingsIdInvalidKHR = -1000003000,

        /// <summary>
        /// Android Thread Settings Failure
        /// </summary>
        AndroidThreadSettingsdFailureKHR = -1000003001,

        /// <summary>
        /// Create Spatial Anchor Failed
        /// </summary>
        CreateSpatialAnchorFailedMSFT = -1000039001,

        /// <summary>
        /// Seccondary View Configuration Type Not Enabled
        /// </summary>
        SecondaryViewConfigurationTypeNotEnabledMSFT = -1000053000,

        /// <summary>
        /// Max XR Result Value
        /// </summary>
        MaxResult = 0x7FFFFFFF
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

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XrBaseInStructure
    {
        public uint type;
        public void* next;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XrVector2f
    {
        float x;
        float y;

        public XrVector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public XrVector2f(Vector2 value)
        {
            x = value.x;
            y = value.y;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct XrVector3f
    {
        float x;
        float y;
        float z;

        public XrVector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = -z;
        }

        public XrVector3f(Vector3 value)
        {
            x = value.x;
            y = value.y;
            z = -value.z;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct XrQuaternionf
    {
        float x;
        float y;
        float z;
        float w;

        public XrQuaternionf(float x, float y, float z, float w)
        {
            this.x = -x;
            this.y = -y;
            this.z = z;
            this.w = w;
        }

        public XrQuaternionf(Quaternion quaternion)
        {
            this.x = -quaternion.x;
            this.y = -quaternion.y;
            this.z = quaternion.z;
            this.w = quaternion.w;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XrCompositionLayerBaseHeader
    {
        public uint Type;
        public void* Next;
        public ulong LayerFlags;
        public ulong Space;
    }

    /// <summary>
    /// A construct representing a position and orientation within a space, with position expressed in meters, and orientation represented as a unit quaternion.
    /// <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrPosef.html"></see>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrPosef
    {
        XrQuaternionf orientation;
        XrVector3f position;

        /// <summary>
        /// Initializes and returns an instance of XrPosef with the provided parameters.
        /// </summary>
        /// <param name="vec3">vector3 position.</param>
        /// <param name="quaternion">quaternion orientation.</param>
        public XrPosef(Vector3 vec3, Quaternion quaternion)
        {
            this.position = new XrVector3f(vec3);
            this.orientation = new XrQuaternionf(quaternion);
        }
    };

    /// <summary>
    /// Creation info for a swapchain.
    /// <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrSwapchainCreateInfo.html"></see>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XrSwapchainCreateInfo
    {
        /// <summary>
        /// The XrStructureType of this structure.
        /// <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrStructureType.html"></see>
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain. Can be null.
        /// </summary>
        public void* Next;

        /// <summary>
        /// Bitmask of XrSwapchainCreateFlagBits describing additional properties of the swapchain.
        /// <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrSwapchainCreateFlagBits.html"></see>
        /// </summary>
        public ulong CreateFlags;

        /// <summary>
        /// Bitmask of XrSwapchainUsageFlagBits describing the intended usage of the swapchain�s images.
        /// The usage flags define how the corresponding graphics API objects are created.
        /// A mismatch may result in swapchain images that do not support the application�s usage.
        /// <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrSwapchainUsageFlagBits.html"></see>
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
        /// Describes the number of levels of detail available for minified sampling of the image, must not be 0 or greater than the graphics API�s maximum limit.
        /// </summary>
        public uint MipCount;
    }
}
