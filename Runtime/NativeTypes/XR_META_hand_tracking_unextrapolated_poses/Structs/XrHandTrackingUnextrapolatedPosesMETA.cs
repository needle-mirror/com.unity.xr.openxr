using XrTime = System.Int64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Extends `XrHandJointLocationsEXT` to report when the tracking system captured the source
    /// data used to generate the returned unextrapolated hand joint poses. Provided by
    /// `XR_META_hand_tracking_unextrapolated_poses`.
    /// </summary>
    /// <remarks>
    /// The runtime populates this struct only when
    /// <see cref="XrHandTrackingUnextrapolatedPosesRequestMETA"/> is chained to the
    /// `XrHandJointsLocateInfoEXT` input, and leaves it unmodified otherwise.
    ///
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandTrackingUnextrapolatedPosesMETA"/>.
    /// </remarks>
    public readonly unsafe struct XrHandTrackingUnextrapolatedPosesMETA
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandTrackingUnextrapolatedPosesMETA defaultValue => new(null, 0);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandTrackingUnextrapolatedPosesMETA"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The `XrTime` at which the tracking system captured the source data used to generate the
        /// returned unextrapolated joint poses, or zero if there are no valid poses.
        /// </summary>
        public XrTime captureTime { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="captureTime">The time at which the source data was captured.</param>
        public XrHandTrackingUnextrapolatedPosesMETA(void* next, XrTime captureTime)
        {
            type = XrStructureType.HandTrackingUnextrapolatedPosesMETA;
            this.next = next;
            this.captureTime = captureTime;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="captureTime">The time at which the source data was captured.</param>
        public XrHandTrackingUnextrapolatedPosesMETA(XrTime captureTime)
            : this(null, captureTime) { }
    }
}
