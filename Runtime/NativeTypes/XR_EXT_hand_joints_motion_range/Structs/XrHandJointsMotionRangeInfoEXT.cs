namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Extends `XrHandJointsLocateInfoEXT` to constrain the returned hand joint
    /// poses to either natural movement or movement that conforms to a held
    /// controller. Provided by `XR_EXT_hand_joints_motion_range`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandJointsMotionRangeInfoEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrHandJointsMotionRangeInfoEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandJointsMotionRangeInfoEXT defaultValue => new(null, default);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandJointsMotionRangeInfoEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The motion range constraint that the runtime should apply when
        /// returning hand joint poses for the next `xrLocateHandJointsEXT` call.
        /// </summary>
        public XrHandJointsMotionRangeEXT handJointsMotionRange { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="handJointsMotionRange">The motion range constraint to apply.</param>
        public XrHandJointsMotionRangeInfoEXT(void* next, XrHandJointsMotionRangeEXT handJointsMotionRange)
        {
            type = XrStructureType.HandJointsMotionRangeInfoEXT;
            this.next = next;
            this.handJointsMotionRange = handJointsMotionRange;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="handJointsMotionRange">The motion range constraint to apply.</param>
        public XrHandJointsMotionRangeInfoEXT(XrHandJointsMotionRangeEXT handJointsMotionRange)
            : this(null, handJointsMotionRange) { }
    }
}
