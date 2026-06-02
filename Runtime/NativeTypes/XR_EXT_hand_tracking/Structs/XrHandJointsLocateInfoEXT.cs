using XrSpace = System.UInt64;
using XrTime = System.Int64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes the information to locate hand joints. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandJointsLocateInfoEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrHandJointsLocateInfoEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandJointsLocateInfoEXT defaultValue => new(null, 0, 0);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandJointsLocateInfoEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The base space in which to locate the hand joints.
        /// </summary>
        public XrSpace baseSpace { get; }

        /// <summary>
        /// The time at which to locate the hand joints.
        /// </summary>
        public XrTime time { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="baseSpace">The base space in which to locate the hand joints.</param>
        /// <param name="time">The time at which to locate the hand joints.</param>
        public XrHandJointsLocateInfoEXT(void* next, XrSpace baseSpace, XrTime time)
        {
            type = XrStructureType.HandJointsLocateInfoEXT;
            this.next = next;
            this.baseSpace = baseSpace;
            this.time = time;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="baseSpace">The base space in which to locate the hand joints.</param>
        /// <param name="time">The time at which to locate the hand joints.</param>
        public XrHandJointsLocateInfoEXT(XrSpace baseSpace, XrTime time)
            : this(null, baseSpace, time) { }
    }
}
