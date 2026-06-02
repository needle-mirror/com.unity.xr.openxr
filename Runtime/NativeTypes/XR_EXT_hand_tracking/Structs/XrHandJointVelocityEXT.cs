namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes the velocity of a hand joint. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    public readonly struct XrHandJointVelocityEXT
    {
        /// <summary>
        /// A bitfield of <see cref="XrSpaceVelocityFlags"/> describing the validity of the velocities.
        /// </summary>
        public XrSpaceVelocityFlags velocityFlags { get; }

        /// <summary>
        /// The linear velocity of the hand joint in meters per second.
        /// </summary>
        public XrVector3f linearVelocity { get; }

        /// <summary>
        /// The angular velocity of the hand joint in radians per second.
        /// </summary>
        public XrVector3f angularVelocity { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="velocityFlags">A bitfield describing the validity of the velocities.</param>
        /// <param name="linearVelocity">The linear velocity of the hand joint in meters per second.</param>
        /// <param name="angularVelocity">The angular velocity of the hand joint in radians per second.</param>
        public XrHandJointVelocityEXT(
            XrSpaceVelocityFlags velocityFlags, XrVector3f linearVelocity, XrVector3f angularVelocity)
        {
            this.velocityFlags = velocityFlags;
            this.linearVelocity = linearVelocity;
            this.angularVelocity = angularVelocity;
        }
    }
}
