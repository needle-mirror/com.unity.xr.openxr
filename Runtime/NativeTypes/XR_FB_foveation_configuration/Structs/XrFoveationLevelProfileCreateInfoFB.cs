namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Extends `XrFoveationProfileCreateInfoFB` to request a foveation profile that applies a
    /// fixed foveation pattern at a chosen level. Provided by `XR_FB_foveation_configuration`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.FoveationLevelProfileCreateInfoFB"/>.
    /// </remarks>
    public readonly unsafe struct XrFoveationLevelProfileCreateInfoFB
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrFoveationLevelProfileCreateInfoFB defaultValue => new(null, default, default, default);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.FoveationLevelProfileCreateInfoFB"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The maximum desired foveation level.
        /// </summary>
        public XrFoveationLevelFB level { get; }

        /// <summary>
        /// The vertical offset of the foveation pattern, in degrees.
        /// </summary>
        public float verticalOffset { get; }

        /// <summary>
        /// Whether the runtime may vary the applied foveation level up to <see cref="level"/>.
        /// </summary>
        public XrFoveationDynamicFB dynamic { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="level">The maximum desired foveation level.</param>
        /// <param name="verticalOffset">The vertical offset of the foveation pattern, in degrees.</param>
        /// <param name="dynamic">Whether the runtime may vary the applied foveation level.</param>
        public XrFoveationLevelProfileCreateInfoFB(
            void* next, XrFoveationLevelFB level, float verticalOffset, XrFoveationDynamicFB dynamic)
        {
            type = XrStructureType.FoveationLevelProfileCreateInfoFB;
            this.next = next;
            this.level = level;
            this.verticalOffset = verticalOffset;
            this.dynamic = dynamic;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="level">The maximum desired foveation level.</param>
        /// <param name="verticalOffset">The vertical offset of the foveation pattern, in degrees.</param>
        /// <param name="dynamic">Whether the runtime may vary the applied foveation level.</param>
        public XrFoveationLevelProfileCreateInfoFB(
            XrFoveationLevelFB level, float verticalOffset, XrFoveationDynamicFB dynamic)
            : this(null, level, verticalOffset, dynamic) { }
    }
}
