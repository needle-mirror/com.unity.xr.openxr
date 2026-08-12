using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes a single input reference image for image tracking.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialReferenceImageEXT"/>.
    /// </remarks>
    /// <seealso cref="XrSpatialImageSizeEXT"/>
    /// <seealso cref="XrSpatialImageStaticOptimizationEXT"/>
    public readonly unsafe struct XrSpatialReferenceImageEXT
    {
        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.SpatialReferenceImageEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        /// <seealso cref="XrSpatialImageSizeEXT"/>
        /// <seealso cref="XrSpatialImageStaticOptimizationEXT"/>
        public void* next { get; }

        /// <summary>
        /// The pixel width of the reference image.
        /// </summary>
        public uint width { get; }

        /// <summary>
        /// The pixel height of the reference image.
        /// </summary>
        public uint height { get; }

        /// <summary>
        /// The format of the image.
        /// </summary>
        public XrSpatialReferenceImageFormatEXT format { get; }

        /// <summary>
        /// The number of elements in <see cref="planes"/>. The application must provide one plane per color
        /// channel of <see cref="format"/>: `4` for <see cref="XrSpatialReferenceImageFormatEXT.RGBA_8888"/>,
        /// `3` for <see cref="XrSpatialReferenceImageFormatEXT.RGB_888"/>, and `3` for
        /// <see cref="XrSpatialReferenceImageFormatEXT.YUV_420_888"/>.
        /// </summary>
        public uint planeCount { get; }

        /// <summary>
        /// Pointer to an array of `XrSpatialReferenceImagePlaneEXT` structures representing the planes of the image data. Must be non-null.
        /// </summary>
        public XrSpatialReferenceImagePlaneEXT* planes { get; }

        /// <summary>
        /// The number of planes expected for the given <paramref name="format"/> (one per color channel), or `0`
        /// if the format is not a known value.
        /// </summary>
        static uint ExpectedPlaneCount(XrSpatialReferenceImageFormatEXT format) => format switch
        {
            XrSpatialReferenceImageFormatEXT.RGBA_8888 => 4,
            XrSpatialReferenceImageFormatEXT.RGB_888 => 3,
            XrSpatialReferenceImageFormatEXT.YUV_420_888 => 3,
            _ => 0
        };

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="width">The pixel width of the reference image.</param>
        /// <param name="height">The pixel height of the reference image.</param>
        /// <param name="format">The format of the image.</param>
        /// <param name="planeCount">The number of elements in <paramref name="planes"/>. Must provide one plane per
        /// color channel of <paramref name="format"/> (`4` for `RGBA_8888`, `3` for `RGB_888`/`YUV_420_888`).</param>
        /// <param name="planes">Pointer to an array of `XrSpatialReferenceImagePlaneEXT` structures representing the planes of the image data. Must be non-null.</param>
        public XrSpatialReferenceImageEXT(
            void* next,
            uint width,
            uint height,
            XrSpatialReferenceImageFormatEXT format,
            uint planeCount,
            XrSpatialReferenceImagePlaneEXT* planes)
        {
            Assert.IsTrue(planeCount > 0);
            Assert.IsTrue(planes != null);

            var expectedPlaneCount = ExpectedPlaneCount(format);
            Assert.IsTrue(expectedPlaneCount == 0 || planeCount == expectedPlaneCount);

            type = XrStructureType.SpatialReferenceImageEXT;
            this.next = next;
            this.width = width;
            this.height = height;
            this.format = format;
            this.planeCount = planeCount;
            this.planes = planes;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="width">The pixel width of the reference image.</param>
        /// <param name="height">The pixel height of the reference image.</param>
        /// <param name="format">The format of the image.</param>
        /// <param name="planeCount">The number of elements in <paramref name="planes"/>.</param>
        /// <param name="planes">Pointer to an array of `XrSpatialReferenceImagePlaneEXT` structures representing the planes of the image data. Must be non-null.</param>
        public XrSpatialReferenceImageEXT(
            uint width,
            uint height,
            XrSpatialReferenceImageFormatEXT format,
            uint planeCount,
            XrSpatialReferenceImagePlaneEXT* planes)
            : this(null, width, height, format, planeCount, planes) { }

        /// <summary>
        /// Construct an instance from a native array of planes.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="width">The pixel width of the reference image.</param>
        /// <param name="height">The pixel height of the reference image.</param>
        /// <param name="format">The format of the image.</param>
        /// <param name="planes">The planes of the image data.</param>
        public XrSpatialReferenceImageEXT(
            void* next,
            uint width,
            uint height,
            XrSpatialReferenceImageFormatEXT format,
            NativeArray<XrSpatialReferenceImagePlaneEXT> planes)
            : this(
                next, width, height, format,
                (uint)planes.Length, (XrSpatialReferenceImagePlaneEXT*)planes.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array of planes.
        /// </summary>
        /// <param name="width">The pixel width of the reference image.</param>
        /// <param name="height">The pixel height of the reference image.</param>
        /// <param name="format">The format of the image.</param>
        /// <param name="planes">The planes of the image data.</param>
        public XrSpatialReferenceImageEXT(
            uint width,
            uint height,
            XrSpatialReferenceImageFormatEXT format,
            NativeArray<XrSpatialReferenceImagePlaneEXT> planes)
            : this(null, width, height, format, planes) { }

        /// <summary>
        /// Construct an instance from a read-only native array of planes.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="width">The pixel width of the reference image.</param>
        /// <param name="height">The pixel height of the reference image.</param>
        /// <param name="format">The format of the image.</param>
        /// <param name="planes">The planes of the image data.</param>
        public XrSpatialReferenceImageEXT(
            void* next,
            uint width,
            uint height,
            XrSpatialReferenceImageFormatEXT format,
            NativeArray<XrSpatialReferenceImagePlaneEXT>.ReadOnly planes)
            : this(
                next, width, height, format,
                (uint)planes.Length, (XrSpatialReferenceImagePlaneEXT*)planes.GetUnsafeReadOnlyPtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array of planes.
        /// </summary>
        /// <param name="width">The pixel width of the reference image.</param>
        /// <param name="height">The pixel height of the reference image.</param>
        /// <param name="format">The format of the image.</param>
        /// <param name="planes">The planes of the image data.</param>
        public XrSpatialReferenceImageEXT(
            uint width,
            uint height,
            XrSpatialReferenceImageFormatEXT format,
            NativeArray<XrSpatialReferenceImagePlaneEXT>.ReadOnly planes)
            : this(null, width, height, format, planes) { }
    }
}
