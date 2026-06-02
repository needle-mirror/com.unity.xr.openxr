using XrBool32 = System.UInt32;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes hand tracking system properties. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.SystemHandTrackingPropertiesEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrSystemHandTrackingPropertiesEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrSystemHandTrackingPropertiesEXT defaultValue => new(false);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.SystemHandTrackingPropertiesEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// Indicates whether the system supports hand tracking.
        /// </summary>
        public XrBool32 supportsHandTracking { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="supportsHandTracking">`true` if the system supports hand tracking. Otherwise, `false`.</param>
        public XrSystemHandTrackingPropertiesEXT(void* next, bool supportsHandTracking)
        {
            type = XrStructureType.SystemHandTrackingPropertiesEXT;
            this.next = next;
            this.supportsHandTracking = supportsHandTracking ? 1u : 0;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="supportsHandTracking">`true` if the system supports hand tracking. Otherwise, `false`.</param>
        public XrSystemHandTrackingPropertiesEXT(bool supportsHandTracking)
            : this(null, supportsHandTracking) { }
    }
}
