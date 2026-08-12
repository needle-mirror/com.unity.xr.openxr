namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Filter that excludes subsumed entities from a discovery snapshot. Chain to the
    /// <see cref="XrSpatialDiscoverySnapshotCreateInfoEXT.next"/> pointer.
    /// Provided by `XR_ANDROID_spatial_component_subsumed_by`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that
    /// > <see cref="type"/> is correctly initialized to
    /// > <see cref="XrStructureType.SpatialDiscoveryUniqueEntitiesFilterANDROID"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialDiscoveryUniqueEntitiesFilterANDROID
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property and a `null` next pointer.
        /// </summary>
        public static XrSpatialDiscoveryUniqueEntitiesFilterANDROID defaultValue => new(null);

        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.SpatialDiscoveryUniqueEntitiesFilterANDROID"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        public XrSpatialDiscoveryUniqueEntitiesFilterANDROID(void* next)
        {
            type = XrStructureType.SpatialDiscoveryUniqueEntitiesFilterANDROID;
            this.next = next;
        }
    }
}
