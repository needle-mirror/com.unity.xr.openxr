namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Identifies a hand. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    public enum XrHandEXT
    {
        /// <summary>
        /// Specifies the hand tracker will be tracking the user's left hand.
        /// Equivalent to the OpenXR value `XR_HAND_LEFT_EXT`.
        /// </summary>
        Left = 1,

        /// <summary>
        /// Specifies the hand tracker will be tracking the user's right hand.
        /// Equivalent to the OpenXR value `XR_HAND_RIGHT_EXT`.
        /// </summary>
        Right = 2,
    }
}
