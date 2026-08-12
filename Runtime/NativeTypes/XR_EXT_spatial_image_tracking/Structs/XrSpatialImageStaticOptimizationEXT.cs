using XrBool32 = System.UInt32;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Optional next-chain struct that tells the runtime an image (or database of images) is not
    /// expected to move. Chain to
    /// <see cref="XrSpatialReferenceImageEXT.next">XrSpatialReferenceImageEXT.next</see> for per-image optimization or
    /// to <see cref="XrSpatialImageTrackingDatabaseCreateInfoEXT.next">XrSpatialImageTrackingDatabaseCreateInfoEXT.next</see>
    /// for database-wide optimization. Per-image chaining takes precedence.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialImageStaticOptimizationEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialImageStaticOptimizationEXT
    {
        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.SpatialImageStaticOptimizationEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// Indicates whether images move or not. A value of `1` indicates that images do not move.
        /// </summary>
        public XrBool32 optimizeForStaticImage { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="optimizeForStaticImage">`true` if images do not move.</param>
        public XrSpatialImageStaticOptimizationEXT(void* next, bool optimizeForStaticImage)
        {
            type = XrStructureType.SpatialImageStaticOptimizationEXT;
            this.next = next;
            this.optimizeForStaticImage = optimizeForStaticImage ? 1u : 0;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="optimizeForStaticImage">`true` if images do not move.</param>
        public XrSpatialImageStaticOptimizationEXT(bool optimizeForStaticImage)
            : this(null, optimizeForStaticImage) { }
    }
}
