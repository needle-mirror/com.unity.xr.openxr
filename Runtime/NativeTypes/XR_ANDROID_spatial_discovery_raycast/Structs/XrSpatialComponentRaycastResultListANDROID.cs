using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Raycast result component list structure, used to query component data.
    /// Provided by `XR_ANDROID_spatial_discovery_raycast`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialComponentRaycastResultListANDROID"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialComponentRaycastResultListANDROID
    {
        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.SpatialComponentRaycastResultListANDROID"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The count of elements in <see cref="raycastResults"/>. Must be greater than `0`.
        /// </summary>
        public uint raycastResultCount { get; }

        /// <summary>
        /// Pointer to an array of raycast result components. Must be non-null.
        /// </summary>
        public XrSpatialRaycastResultDataANDROID* raycastResults { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="raycastResultCount">The count of elements in <paramref name="raycastResults"/>.
        /// Must be greater than `0`.</param>
        /// <param name="raycastResults">Pointer to an array of raycast result components. Must be non-null.</param>
        public XrSpatialComponentRaycastResultListANDROID(
            void* next, uint raycastResultCount, XrSpatialRaycastResultDataANDROID* raycastResults)
        {
            Assert.IsTrue(raycastResultCount > 0);
            Assert.IsTrue(raycastResults != null);

            type = XrStructureType.SpatialComponentRaycastResultListANDROID;
            this.next = next;
            this.raycastResultCount = raycastResultCount;
            this.raycastResults = raycastResults;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="raycastResultCount">The count of elements in <paramref name="raycastResults"/>.
        /// Must be greater than `0`.</param>
        /// <param name="raycastResults">Pointer to an array of raycast result components. Must be non-null.</param>
        public XrSpatialComponentRaycastResultListANDROID(
            uint raycastResultCount, XrSpatialRaycastResultDataANDROID* raycastResults)
            : this(null, raycastResultCount, raycastResults) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="raycastResults">Native array of raycast result components. Must be non-empty.</param>
        public XrSpatialComponentRaycastResultListANDROID(
            void* next, NativeArray<XrSpatialRaycastResultDataANDROID> raycastResults)
            : this(
                next,
                (uint)raycastResults.Length,
                (XrSpatialRaycastResultDataANDROID*)raycastResults.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="raycastResults">Native array of raycast result components. Must be non-empty.</param>
        public XrSpatialComponentRaycastResultListANDROID(
            NativeArray<XrSpatialRaycastResultDataANDROID> raycastResults)
            : this(
                null,
                (uint)raycastResults.Length,
                (XrSpatialRaycastResultDataANDROID*)raycastResults.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="raycastResults">Read-only native array of raycast result components.
        /// Must be non-empty.</param>
        public XrSpatialComponentRaycastResultListANDROID(
            void* next, NativeArray<XrSpatialRaycastResultDataANDROID>.ReadOnly raycastResults)
            : this(
                next,
                (uint)raycastResults.Length,
                (XrSpatialRaycastResultDataANDROID*)raycastResults.GetUnsafeReadOnlyPtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array.
        /// </summary>
        /// <param name="raycastResults">Read-only native array of raycast result components.
        /// Must be non-empty.</param>
        public XrSpatialComponentRaycastResultListANDROID(
            NativeArray<XrSpatialRaycastResultDataANDROID>.ReadOnly raycastResults)
            : this(
                null,
                (uint)raycastResults.Length,
                (XrSpatialRaycastResultDataANDROID*)raycastResults.GetUnsafeReadOnlyPtr())
        { }
    }
}
