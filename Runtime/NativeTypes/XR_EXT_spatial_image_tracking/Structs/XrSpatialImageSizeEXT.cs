namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Optional next-chain struct that informs the runtime of an image's physical width in meters.
    /// Chain to <see cref="XrSpatialReferenceImageEXT.next">XrSpatialReferenceImageEXT.next</see> for per-image size or
    /// to <see cref="XrSpatialImageTrackingDatabaseCreateInfoEXT.next">XrSpatialImageTrackingDatabaseCreateInfoEXT.next</see>
    /// for a database-wide default. Per-image chaining takes precedence.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialImageSizeEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialImageSizeEXT
    {
        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.SpatialImageSizeEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The physical width in meters to set. The height is implied by the aspect ratio of the reference image's
        /// `width` and `height`.
        /// </summary>
        public float physicalWidth { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="physicalWidth">The physical width in meters to set.</param>
        public XrSpatialImageSizeEXT(void* next, float physicalWidth)
        {
            type = XrStructureType.SpatialImageSizeEXT;
            this.next = next;
            this.physicalWidth = physicalWidth;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="physicalWidth">The physical width in meters to set.</param>
        public XrSpatialImageSizeEXT(float physicalWidth)
            : this(null, physicalWidth) { }
    }
}
