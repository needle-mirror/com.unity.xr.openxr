namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Identifies the source of hand tracking data reported by the runtime.
    /// Provided by `XR_EXT_hand_tracking_data_source`.
    /// </summary>
    public enum XrHandTrackingDataSourceEXT
    {
        /// <summary>
        /// Hand tracking data is derived from optical (camera-based) hand tracking
        /// with no controller present.
        /// Equivalent to the OpenXR value `XR_HAND_TRACKING_DATA_SOURCE_UNOBSTRUCTED_EXT`.
        /// </summary>
        Unobstructed = 1,

        /// <summary>
        /// Hand tracking data is derived from a held controller's sensors.
        /// Equivalent to the OpenXR value `XR_HAND_TRACKING_DATA_SOURCE_CONTROLLER_EXT`.
        /// </summary>
        Controller = 2,
    }
}
