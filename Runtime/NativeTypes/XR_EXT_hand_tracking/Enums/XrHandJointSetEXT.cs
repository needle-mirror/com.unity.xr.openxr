namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Identifies a set of hand joints to track. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    public enum XrHandJointSetEXT
    {
        /// <summary>
        /// Indicates that the created <c>XrHandTrackerEXT</c> tracks the set of hand joints described by <see cref="XrHandJointEXT"/>.
        /// Equivalent to the OpenXR value `XR_HAND_JOINT_SET_DEFAULT_EXT`.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Hand joints including forearm. Equivalent to the OpenXR value
        /// `XR_HAND_JOINT_SET_HAND_WITH_FOREARM_ULTRALEAP`.
        /// Provided by `XR_ULTRALEAP_hand_tracking_forearm`.
        /// </summary>
        HandWithForearmUltraleap = 1000149000,
    }
}
