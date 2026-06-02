using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Space velocity bit flags. Equivalent to the OpenXR `XrSpaceVelocityFlags` flag bits.
    /// </summary>
    [Flags]
    public enum XrSpaceVelocityFlags : ulong
    {
        /// <summary>
        /// No flags set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Linear velocity is valid.
        /// Equivalent to the OpenXR value `XR_SPACE_VELOCITY_LINEAR_VALID_BIT`.
        /// </summary>
        LinearValid = 0x00000001,

        /// <summary>
        /// Angular velocity is valid.
        /// Equivalent to the OpenXR value `XR_SPACE_VELOCITY_ANGULAR_VALID_BIT`.
        /// </summary>
        AngularValid = 0x00000002,
    }
}
