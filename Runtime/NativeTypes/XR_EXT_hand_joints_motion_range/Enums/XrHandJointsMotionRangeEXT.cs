namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Specifies the motion range constraint that the runtime should apply
    /// when returning hand joint poses. Provided by `XR_EXT_hand_joints_motion_range`.
    /// </summary>
    public enum XrHandJointsMotionRangeEXT
    {
        /// <summary>
        /// Joint poses are not constrained by a held controller and reflect
        /// the full natural range of hand motion.
        /// Equivalent to the OpenXR value `XR_HAND_JOINTS_MOTION_RANGE_UNOBSTRUCTED_EXT`.
        /// </summary>
        Unobstructed = 1,

        /// <summary>
        /// Joint poses are constrained to conform to the shape of a held
        /// controller, reflecting how the hand wraps around the device.
        /// Equivalent to the OpenXR value `XR_HAND_JOINTS_MOTION_RANGE_CONFORMING_TO_CONTROLLER_EXT`.
        /// </summary>
        ConformingToController = 2,
    }
}
