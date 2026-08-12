namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// The desired dynamic foveation setting, which determines whether the runtime can vary the
    /// applied foveation level while the app is running.
    /// Provided by `XR_FB_foveation_configuration`.
    /// </summary>
    public enum XrFoveationDynamicFB
    {
        /// <summary>
        /// Static foveation at the maximum desired level.
        /// Equivalent to the OpenXR value `XR_FOVEATION_DYNAMIC_DISABLED_FB`.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Dynamic changing foveation based on performance headroom available, up to the maximum
        /// desired level.
        /// Equivalent to the OpenXR value `XR_FOVEATION_DYNAMIC_LEVEL_ENABLED_FB`.
        /// </summary>
        LevelEnabled = 1
    }
}
