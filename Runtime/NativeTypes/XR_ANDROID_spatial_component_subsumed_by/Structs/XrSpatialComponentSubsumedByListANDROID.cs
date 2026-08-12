using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using XrSpatialEntityIdEXT = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Subsumed-by component list structure, used to query component data.
    /// Provided by `XR_ANDROID_spatial_component_subsumed_by`.
    /// </summary>
    /// <remarks>
    /// Chain an instance to the <see cref="XrSpatialComponentDataQueryResultEXT.next"/> pointer to query
    /// the ID of the entity that subsumes each queried entity.
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialComponentSubsumedByListANDROID"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialComponentSubsumedByListANDROID
    {
        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.SpatialComponentSubsumedByListANDROID"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The count of elements in <see cref="subsumedUniqueIds"/>. Must be greater than `0`.
        /// </summary>
        public uint subsumedUniqueIdCount { get; }

        /// <summary>
        /// Pointer to an array of subsuming entity IDs. Must be non-null.
        /// </summary>
        public XrSpatialEntityIdEXT* subsumedUniqueIds { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="subsumedUniqueIdCount">The count of elements in <paramref name="subsumedUniqueIds"/>.</param>
        /// <param name="subsumedUniqueIds">Pointer to an array of subsuming entity IDs.</param>
        public XrSpatialComponentSubsumedByListANDROID(
            void* next, uint subsumedUniqueIdCount, XrSpatialEntityIdEXT* subsumedUniqueIds)
        {
            Assert.IsTrue(subsumedUniqueIdCount > 0);
            Assert.IsTrue(subsumedUniqueIds != null);

            type = XrStructureType.SpatialComponentSubsumedByListANDROID;
            this.next = next;
            this.subsumedUniqueIdCount = subsumedUniqueIdCount;
            this.subsumedUniqueIds = subsumedUniqueIds;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="subsumedUniqueIdCount">The count of elements in <paramref name="subsumedUniqueIds"/>.</param>
        /// <param name="subsumedUniqueIds">Pointer to an array of subsuming entity IDs.</param>
        public XrSpatialComponentSubsumedByListANDROID(
            uint subsumedUniqueIdCount, XrSpatialEntityIdEXT* subsumedUniqueIds)
            : this(null, subsumedUniqueIdCount, subsumedUniqueIds) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="subsumedUniqueIds">Native array of subsuming entity IDs. Must be non-empty.</param>
        public XrSpatialComponentSubsumedByListANDROID(void* next, NativeArray<XrSpatialEntityIdEXT> subsumedUniqueIds)
            : this(next, (uint)subsumedUniqueIds.Length, (XrSpatialEntityIdEXT*)subsumedUniqueIds.GetUnsafePtr()) { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="subsumedUniqueIds">Read-only native array of subsuming entity IDs. Must be non-empty.</param>
        public XrSpatialComponentSubsumedByListANDROID(void* next, NativeArray<XrSpatialEntityIdEXT>.ReadOnly subsumedUniqueIds)
            : this(next, (uint)subsumedUniqueIds.Length, (XrSpatialEntityIdEXT*)subsumedUniqueIds.GetUnsafeReadOnlyPtr()) { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="subsumedUniqueIds">Native array of subsuming entity IDs. Must be non-empty.</param>
        public XrSpatialComponentSubsumedByListANDROID(NativeArray<XrSpatialEntityIdEXT> subsumedUniqueIds)
            : this(null, (uint)subsumedUniqueIds.Length, (XrSpatialEntityIdEXT*)subsumedUniqueIds.GetUnsafePtr()) { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array.
        /// </summary>
        /// <param name="subsumedUniqueIds">Read-only native array of subsuming entity IDs. Must be non-empty.</param>
        public XrSpatialComponentSubsumedByListANDROID(NativeArray<XrSpatialEntityIdEXT>.ReadOnly subsumedUniqueIds)
            : this(null, (uint)subsumedUniqueIds.Length, (XrSpatialEntityIdEXT*)subsumedUniqueIds.GetUnsafeReadOnlyPtr()) { }
    }
}
