namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// The maximum desired foveation level, trading periphery visual fidelity for performance.
    /// Provided by `XR_FB_foveation_configuration`.
    /// </summary>
    public enum XrFoveationLevelFB
    {
        /// <summary>
        /// No foveation.
        /// Equivalent to the OpenXR value `XR_FOVEATION_LEVEL_NONE_FB`.
        /// </summary>
        None = 0,

        /// <summary>
        /// Less foveation (higher periphery visual fidelity, lower performance).
        /// Equivalent to the OpenXR value `XR_FOVEATION_LEVEL_LOW_FB`.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium foveation (medium periphery visual fidelity, medium performance).
        /// Equivalent to the OpenXR value `XR_FOVEATION_LEVEL_MEDIUM_FB`.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// High foveation (lower periphery visual fidelity, higher performance).
        /// Equivalent to the OpenXR value `XR_FOVEATION_LEVEL_HIGH_FB`.
        /// </summary>
        High = 3
    }
}
