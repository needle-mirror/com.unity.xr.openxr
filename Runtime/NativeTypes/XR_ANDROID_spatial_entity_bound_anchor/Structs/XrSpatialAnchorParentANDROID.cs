using XrSpatialEntityIdEXT = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Chains into <see cref="XrSpatialAnchorCreateInfoEXT.next"/> to specify a parent spatial entity
    /// when creating a bound anchor via `xrCreateSpatialAnchorEXT`.
    /// Provided by `XR_ANDROID_spatial_entity_bound_anchor`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialAnchorParentANDROID"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialAnchorParentANDROID
    {
        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.SpatialAnchorParentANDROID"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain. No valid next structures are defined.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The `XrSpatialEntityIdEXT` of the spatial entity to attach the anchor to.
        /// </summary>
        public XrSpatialEntityIdEXT parentId { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="XrSpatialAnchorParentANDROID"/>.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="parentId">The entity ID of the spatial entity to attach the anchor to.</param>
        public XrSpatialAnchorParentANDROID(void* next, XrSpatialEntityIdEXT parentId)
        {
            type = XrStructureType.SpatialAnchorParentANDROID;
            this.next = next;
            this.parentId = parentId;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="XrSpatialAnchorParentANDROID"/> with `next` set to `null`.
        /// </summary>
        /// <param name="parentId">The entity ID of the spatial entity to attach the anchor to.</param>
        public XrSpatialAnchorParentANDROID(XrSpatialEntityIdEXT parentId) : this(null, parentId)
        {
        }
    }
}
