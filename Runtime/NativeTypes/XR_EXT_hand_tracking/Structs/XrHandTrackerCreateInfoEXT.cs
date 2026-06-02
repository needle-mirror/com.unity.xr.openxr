namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes the information to create a hand tracker. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandTrackerCreateInfoEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrHandTrackerCreateInfoEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandTrackerCreateInfoEXT defaultValue => new(null, default, default);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandTrackerCreateInfoEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The hand for which the tracker will be created.
        /// </summary>
        public XrHandEXT hand { get; }

        /// <summary>
        /// The set of hand joints to track.
        /// </summary>
        public XrHandJointSetEXT handJointSet { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="hand">The hand for which the tracker will be created.</param>
        /// <param name="handJointSet">The set of hand joints to track.</param>
        public XrHandTrackerCreateInfoEXT(void* next, XrHandEXT hand, XrHandJointSetEXT handJointSet)
        {
            type = XrStructureType.HandTrackerCreateInfoEXT;
            this.next = next;
            this.hand = hand;
            this.handJointSet = handJointSet;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="hand">The hand for which the tracker will be created.</param>
        /// <param name="handJointSet">The set of hand joints to track.</param>
        public XrHandTrackerCreateInfoEXT(XrHandEXT hand, XrHandJointSetEXT handJointSet)
            : this(null, hand, handJointSet) { }
    }
}
