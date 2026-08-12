using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Creation info struct passed to `xrCreateSpatialImageTrackingDatabaseAsyncEXT`.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialImageTrackingDatabaseCreateInfoEXT"/>.
    /// </remarks>
    /// <seealso cref="XrSpatialImageSizeEXT"/>
    /// <seealso cref="XrSpatialImageStaticOptimizationEXT"/>
    public readonly unsafe struct XrSpatialImageTrackingDatabaseCreateInfoEXT
    {
        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.SpatialImageTrackingDatabaseCreateInfoEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        /// <seealso cref="XrSpatialImageSizeEXT"/>
        /// <seealso cref="XrSpatialImageStaticOptimizationEXT"/>
        public void* next { get; }

        /// <summary>
        /// The number of elements in <see cref="spatialReferenceImages"/>. Must be greater than `0`.
        /// </summary>
        public uint spatialReferenceImageCount { get; }

        /// <summary>
        /// Pointer to an array of `XrSpatialReferenceImageEXT`. Must be non-null.
        /// </summary>
        public XrSpatialReferenceImageEXT* spatialReferenceImages { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="spatialReferenceImageCount">The number of elements in
        /// <paramref name="spatialReferenceImages"/>. Must be greater than `0`.</param>
        /// <param name="spatialReferenceImages">Pointer to an array of reference image descriptors. Must be
        /// non-null.</param>
        public XrSpatialImageTrackingDatabaseCreateInfoEXT(
            void* next, uint spatialReferenceImageCount, XrSpatialReferenceImageEXT* spatialReferenceImages)
        {
            Assert.IsTrue(spatialReferenceImageCount > 0);
            Assert.IsTrue(spatialReferenceImages != null);

            type = XrStructureType.SpatialImageTrackingDatabaseCreateInfoEXT;
            this.next = next;
            this.spatialReferenceImageCount = spatialReferenceImageCount;
            this.spatialReferenceImages = spatialReferenceImages;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="spatialReferenceImageCount">The number of elements in
        /// <paramref name="spatialReferenceImages"/>. Must be greater than `0`.</param>
        /// <param name="spatialReferenceImages">Pointer to an array of reference image descriptors. Must be
        /// non-null.</param>
        public XrSpatialImageTrackingDatabaseCreateInfoEXT(
            uint spatialReferenceImageCount, XrSpatialReferenceImageEXT* spatialReferenceImages)
            : this(null, spatialReferenceImageCount, spatialReferenceImages) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="spatialReferenceImages">The reference image descriptors. Must contain at least one
        /// element.</param>
        public XrSpatialImageTrackingDatabaseCreateInfoEXT(
            void* next, NativeArray<XrSpatialReferenceImageEXT> spatialReferenceImages)
            : this(
                next,
                (uint)spatialReferenceImages.Length,
                (XrSpatialReferenceImageEXT*)spatialReferenceImages.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="spatialReferenceImages">The reference image descriptors. Must contain at least one
        /// element.</param>
        public XrSpatialImageTrackingDatabaseCreateInfoEXT(
            NativeArray<XrSpatialReferenceImageEXT> spatialReferenceImages)
            : this(null, spatialReferenceImages) { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="spatialReferenceImages">The reference image descriptors. Must contain at least one
        /// element.</param>
        public XrSpatialImageTrackingDatabaseCreateInfoEXT(
            void* next, NativeArray<XrSpatialReferenceImageEXT>.ReadOnly spatialReferenceImages)
            : this(
                next,
                (uint)spatialReferenceImages.Length,
                (XrSpatialReferenceImageEXT*)spatialReferenceImages.GetUnsafeReadOnlyPtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array.
        /// </summary>
        /// <param name="spatialReferenceImages">The reference image descriptors. Must contain at least one
        /// element.</param>
        public XrSpatialImageTrackingDatabaseCreateInfoEXT(
            NativeArray<XrSpatialReferenceImageEXT>.ReadOnly spatialReferenceImages)
            : this(null, spatialReferenceImages) { }
    }
}
