namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Extends `XrHandJointsLocateInfoEXT` to request the runtime's latest calculated hand joint
    /// poses without temporal extrapolation. Provided by `XR_META_hand_tracking_unextrapolated_poses`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandTrackingUnextrapolatedPosesRequestMETA"/>.
    /// </remarks>
    public readonly unsafe struct XrHandTrackingUnextrapolatedPosesRequestMETA
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandTrackingUnextrapolatedPosesRequestMETA defaultValue => new(null);

        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.HandTrackingUnextrapolatedPosesRequestMETA"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        public XrHandTrackingUnextrapolatedPosesRequestMETA(void* next)
        {
            type = XrStructureType.HandTrackingUnextrapolatedPosesRequestMETA;
            this.next = next;
        }
    }
}
