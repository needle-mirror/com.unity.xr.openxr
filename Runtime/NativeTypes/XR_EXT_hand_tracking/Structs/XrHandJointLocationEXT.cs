namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes the location and radius of a hand joint. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    public readonly struct XrHandJointLocationEXT
    {
        /// <summary>
        /// A bitfield of <see cref="XrSpaceLocationFlags"/> describing the validity of the pose.
        /// </summary>
        public ulong locationFlags { get; }

        /// <summary>
        /// The pose of the hand joint.
        /// </summary>
        public XrPosef pose { get; }

        /// <summary>
        /// The radius of the hand joint in meters.
        /// </summary>
        public float radius { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="locationFlags">A bitfield describing the validity of the pose.</param>
        /// <param name="pose">The pose of the hand joint.</param>
        /// <param name="radius">The radius of the hand joint in meters.</param>
        public XrHandJointLocationEXT(
            ulong locationFlags, XrPosef pose, float radius)
        {
            this.locationFlags = locationFlags;
            this.pose = pose;
            this.radius = radius;
        }
    }
}
