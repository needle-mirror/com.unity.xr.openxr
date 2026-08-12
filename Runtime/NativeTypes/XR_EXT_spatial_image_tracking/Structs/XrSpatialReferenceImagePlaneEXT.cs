using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes a single plane of a reference image's pixel data.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// The <see cref="buffer"/> data must be supplied in a left-to-right and top-to-bottom pixel ordering by the
    /// application. Set <see cref="rowStride"/> and <see cref="pixelStride"/> such that only meaningful pixels of this
    /// plane are extracted by the runtime.
    /// </remarks>
    public readonly unsafe struct XrSpatialReferenceImagePlaneEXT
    {
        /// <summary>
        /// The size of the <see cref="buffer"/> array. Must be greater than `0`.
        /// </summary>
        public uint bufferSize { get; }

        /// <summary>
        /// An array of `byte` data representing the plane data. Must be non-null.
        /// </summary>
        public byte* buffer { get; }

        /// <summary>
        /// The byte distance between two starting positions of consecutive pixel data rows.
        /// </summary>
        public uint rowStride { get; }

        /// <summary>
        /// The byte distance between two consecutive pixels.
        /// </summary>
        public uint pixelStride { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="bufferSize">The size of the <paramref name="buffer"/> array. Must be greater than `0`.</param>
        /// <param name="buffer">An array of `byte` data representing the plane data. Must be non-null.</param>
        /// <param name="rowStride">The byte distance between two starting positions of consecutive pixel data rows.</param>
        /// <param name="pixelStride">The byte distance between two consecutive pixels.</param>
        public XrSpatialReferenceImagePlaneEXT(uint bufferSize, byte* buffer, uint rowStride, uint pixelStride)
        {
            Assert.IsTrue(bufferSize > 0);
            Assert.IsTrue(buffer != null);

            this.bufferSize = bufferSize;
            this.buffer = buffer;
            this.rowStride = rowStride;
            this.pixelStride = pixelStride;
        }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="buffer">The plane data.</param>
        /// <param name="rowStride">The byte distance between two starting positions of consecutive pixel data rows.</param>
        /// <param name="pixelStride">The byte distance between two consecutive pixels.</param>
        public XrSpatialReferenceImagePlaneEXT(NativeArray<byte> buffer, uint rowStride, uint pixelStride)
            : this((uint)buffer.Length, (byte*)buffer.GetUnsafePtr(), rowStride, pixelStride) { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="buffer">The plane data.</param>
        /// <param name="rowStride">The byte distance between two starting positions of consecutive pixel data rows.</param>
        /// <param name="pixelStride">The byte distance between two consecutive pixels.</param>
        public XrSpatialReferenceImagePlaneEXT(NativeArray<byte>.ReadOnly buffer, uint rowStride, uint pixelStride)
            : this((uint)buffer.Length, (byte*)buffer.GetUnsafeReadOnlyPtr(), rowStride, pixelStride) { }
    }
}
