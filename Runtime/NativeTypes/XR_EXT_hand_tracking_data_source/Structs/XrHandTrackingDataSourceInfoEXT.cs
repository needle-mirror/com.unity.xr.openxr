using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Extends `XrHandTrackerCreateInfoEXT` to specify which hand tracking data
    /// sources the application prefers. Provided by `XR_EXT_hand_tracking_data_source`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.HandTrackingDataSourceInfoEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrHandTrackingDataSourceInfoEXT
    {
        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandTrackingDataSourceInfoEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The count of elements in <see cref="requestedDataSources"/>. Must be greater than `0`.
        /// </summary>
        public uint requestedDataSourceCount { get; }

        /// <summary>
        /// Pointer to an array of requested data sources, in priority order.
        /// Must be non-null.
        /// </summary>
        public XrHandTrackingDataSourceEXT* requestedDataSources { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="requestedDataSourceCount">The count of elements in
        /// <paramref name="requestedDataSources"/>. Must be greater than `0`.</param>
        /// <param name="requestedDataSources">Pointer to an array of requested data sources,
        /// in priority order. Must be non-null.</param>
        public XrHandTrackingDataSourceInfoEXT(
            void* next, uint requestedDataSourceCount, XrHandTrackingDataSourceEXT* requestedDataSources)
        {
            Assert.IsTrue(requestedDataSourceCount > 0);
            Assert.IsTrue(requestedDataSources != null);

            type = XrStructureType.HandTrackingDataSourceInfoEXT;
            this.next = next;
            this.requestedDataSourceCount = requestedDataSourceCount;
            this.requestedDataSources = requestedDataSources;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="requestedDataSourceCount">The count of elements in
        /// <paramref name="requestedDataSources"/>. Must be greater than `0`.</param>
        /// <param name="requestedDataSources">Pointer to an array of requested data sources,
        /// in priority order. Must be non-null.</param>
        public XrHandTrackingDataSourceInfoEXT(
            uint requestedDataSourceCount, XrHandTrackingDataSourceEXT* requestedDataSources)
            : this(null, requestedDataSourceCount, requestedDataSources) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="requestedDataSources">Native array of requested data sources, in priority order.
        /// Must be non-empty.</param>
        public XrHandTrackingDataSourceInfoEXT(
            void* next, NativeArray<XrHandTrackingDataSourceEXT> requestedDataSources)
            : this(
                next,
                (uint)requestedDataSources.Length,
                (XrHandTrackingDataSourceEXT*)requestedDataSources.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="requestedDataSources">Native array of requested data sources, in priority order.
        /// Must be non-empty.</param>
        public XrHandTrackingDataSourceInfoEXT(
            NativeArray<XrHandTrackingDataSourceEXT> requestedDataSources)
            : this(
                null,
                (uint)requestedDataSources.Length,
                (XrHandTrackingDataSourceEXT*)requestedDataSources.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="requestedDataSources">Read-only native array of requested data sources,
        /// in priority order. Must be non-empty.</param>
        public XrHandTrackingDataSourceInfoEXT(
            void* next, NativeArray<XrHandTrackingDataSourceEXT>.ReadOnly requestedDataSources)
            : this(
                next,
                (uint)requestedDataSources.Length,
                (XrHandTrackingDataSourceEXT*)requestedDataSources.GetUnsafeReadOnlyPtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array.
        /// </summary>
        /// <param name="requestedDataSources">Read-only native array of requested data sources,
        /// in priority order. Must be non-empty.</param>
        public XrHandTrackingDataSourceInfoEXT(
            NativeArray<XrHandTrackingDataSourceEXT>.ReadOnly requestedDataSources)
            : this(
                null,
                (uint)requestedDataSources.Length,
                (XrHandTrackingDataSourceEXT*)requestedDataSources.GetUnsafeReadOnlyPtr())
        { }
    }
}
