using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Output list struct for the Image 2D component, chained to
    /// <see cref="XrSpatialComponentDataQueryResultEXT.next">XrSpatialComponentDataQueryResultEXT.next</see> when
    /// querying image tracking entities.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialComponentImage2DListEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialComponentImage2DListEXT
    {
        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.SpatialComponentImage2DListEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The number of elements in <see cref="images"/>.
        /// </summary>
        public uint imageCount { get; }

        /// <summary>
        /// Pointer to an array of <see cref="XrSpatialImage2DDataEXT"/> the runtime will populate. Must be non-null.
        /// </summary>
        public XrSpatialImage2DDataEXT* images { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="imageCount">The number of elements in <paramref name="images"/>. Must be greater than
        /// `0`.</param>
        /// <param name="images">Pointer to an array of <see cref="XrSpatialImage2DDataEXT"/> the runtime will populate.
        /// Must be non-null.</param>
        public XrSpatialComponentImage2DListEXT(void* next, uint imageCount, XrSpatialImage2DDataEXT* images)
        {
            Assert.IsTrue(imageCount > 0);
            Assert.IsTrue(images != null);

            type = XrStructureType.SpatialComponentImage2DListEXT;
            this.next = next;
            this.imageCount = imageCount;
            this.images = images;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="imageCount">The number of elements in <paramref name="images"/>. Must be greater than
        /// `0`.</param>
        /// <param name="images">Pointer to an array of <see cref="XrSpatialImage2DDataEXT"/> the runtime will populate.
        /// Must be non-null.</param>
        public XrSpatialComponentImage2DListEXT(uint imageCount, XrSpatialImage2DDataEXT* images)
            : this(null, imageCount, images) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="images">The array the runtime will populate with image data.</param>
        public XrSpatialComponentImage2DListEXT(void* next, NativeArray<XrSpatialImage2DDataEXT> images)
            : this(next, (uint)images.Length, (XrSpatialImage2DDataEXT*)images.GetUnsafePtr()) { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="images">The array the runtime will populate with image data.</param>
        public XrSpatialComponentImage2DListEXT(NativeArray<XrSpatialImage2DDataEXT> images)
            : this(null, images) { }
    }
}
