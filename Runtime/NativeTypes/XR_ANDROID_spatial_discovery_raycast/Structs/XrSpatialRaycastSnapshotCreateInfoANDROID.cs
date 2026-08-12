using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Information used to synchronously create a snapshot handle via raycast.
    /// Provided by `XR_ANDROID_spatial_discovery_raycast`.
    /// </summary>
    /// <remarks>
    /// Unlike `xrCreateSpatialDiscoverySnapshotAsyncEXT`, a raycast snapshot is created synchronously, so you can
    /// query raycast results immediately.
    ///
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialRaycastSnapshotCreateInfoANDROID"/>.
    ///
    /// > [!WARNING]
    /// > <see cref="raycastInfo"/> stores the address of the `raycastInfo` passed to the constructor, and this
    /// > struct does not keep that memory alive. The referent must remain valid and at a stable address for as
    /// > long as you use this struct.
    /// >
    /// > In particular, don't pass a temporary. Because the constructors take `raycastInfo` by `in`, an
    /// > expression such as `new XrSpatialRaycastSnapshotCreateInfoANDROID(new XrSpatialRaycastInfoANDROID(...))`
    /// > compiles, but stores the address of a value that is destroyed immediately. Assign the
    /// > <see cref="XrSpatialRaycastInfoANDROID"/> to a local or field first, then pass that.
    /// </remarks>
    public readonly unsafe struct XrSpatialRaycastSnapshotCreateInfoANDROID
    {
        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.SpatialRaycastSnapshotCreateInfoANDROID"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The count of elements in <see cref="componentTypes"/>. May be `0`.
        /// </summary>
        public uint componentTypeCount { get; }

        /// <summary>
        /// Pointer to an array of component types to include in the snapshot. May be `null`.
        /// </summary>
        /// <remarks>
        /// If you don't provide a list of component types, the runtime includes all spatial entities in the
        /// snapshot that have the set of components enumerated in
        /// <see cref="XrSpatialCapabilityConfigurationBaseHeaderEXT.enabledComponents"/> for the raycast-supported
        /// capabilities configured for the spatial context.
        /// </remarks>
        public XrSpatialComponentTypeEXT* componentTypes { get; }

        /// <summary>
        /// A reference to the struct describing the ray.
        /// </summary>
        public XrSpatialRaycastInfoANDROID* raycastInfo { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="componentTypeCount">The count of elements in <paramref name="componentTypes"/>.
        /// May be `0`.</param>
        /// <param name="componentTypes">Pointer to an array of component types to include in the snapshot.
        /// May be `null`.</param>
        /// <param name="raycastInfo">A reference to the struct describing the ray.</param>
        public XrSpatialRaycastSnapshotCreateInfoANDROID(
            void* next,
            uint componentTypeCount,
            XrSpatialComponentTypeEXT* componentTypes,
            in XrSpatialRaycastInfoANDROID raycastInfo)
        {
            type = XrStructureType.SpatialRaycastSnapshotCreateInfoANDROID;
            this.next = next;
            this.componentTypeCount = componentTypeCount;
            this.componentTypes = componentTypes;

            fixed (XrSpatialRaycastInfoANDROID* raycastInfoPtr = &raycastInfo)
            {
                this.raycastInfo = raycastInfoPtr;
            }
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="componentTypeCount">The count of elements in <paramref name="componentTypes"/>.
        /// May be `0`.</param>
        /// <param name="componentTypes">Pointer to an array of component types to include in the snapshot.
        /// May be `null`.</param>
        /// <param name="raycastInfo">A reference to the struct describing the ray.</param>
        public XrSpatialRaycastSnapshotCreateInfoANDROID(
            uint componentTypeCount,
            XrSpatialComponentTypeEXT* componentTypes,
            in XrSpatialRaycastInfoANDROID raycastInfo)
            : this(null, componentTypeCount, componentTypes, in raycastInfo) { }

        /// <summary>
        /// Construct an instance with a `null` next pointer and no component type filter, which includes all
        /// spatial entities hit by the ray that have the components enabled for the raycast-supported
        /// capabilities of the spatial context.
        /// </summary>
        /// <param name="raycastInfo">A reference to the struct describing the ray.</param>
        public XrSpatialRaycastSnapshotCreateInfoANDROID(in XrSpatialRaycastInfoANDROID raycastInfo)
            : this(null, 0, null, in raycastInfo) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="componentTypes">Native array of component types to include in the snapshot.</param>
        /// <param name="raycastInfo">A reference to the struct describing the ray.</param>
        public XrSpatialRaycastSnapshotCreateInfoANDROID(
            void* next,
            NativeArray<XrSpatialComponentTypeEXT> componentTypes,
            in XrSpatialRaycastInfoANDROID raycastInfo)
            : this(
                next,
                (uint)componentTypes.Length,
                (XrSpatialComponentTypeEXT*)componentTypes.GetUnsafePtr(),
                in raycastInfo)
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="componentTypes">Native array of component types to include in the snapshot.</param>
        /// <param name="raycastInfo">A reference to the struct describing the ray.</param>
        public XrSpatialRaycastSnapshotCreateInfoANDROID(
            NativeArray<XrSpatialComponentTypeEXT> componentTypes, in XrSpatialRaycastInfoANDROID raycastInfo)
            : this(
                null,
                (uint)componentTypes.Length,
                (XrSpatialComponentTypeEXT*)componentTypes.GetUnsafePtr(),
                in raycastInfo)
        { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="componentTypes">Read-only native array of component types to include in the
        /// snapshot.</param>
        /// <param name="raycastInfo">A reference to the struct describing the ray.</param>
        public XrSpatialRaycastSnapshotCreateInfoANDROID(
            void* next,
            NativeArray<XrSpatialComponentTypeEXT>.ReadOnly componentTypes,
            in XrSpatialRaycastInfoANDROID raycastInfo)
            : this(
                next,
                (uint)componentTypes.Length,
                (XrSpatialComponentTypeEXT*)componentTypes.GetUnsafeReadOnlyPtr(),
                in raycastInfo)
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array.
        /// </summary>
        /// <param name="componentTypes">Read-only native array of component types to include in the
        /// snapshot.</param>
        /// <param name="raycastInfo">A reference to the struct describing the ray.</param>
        public XrSpatialRaycastSnapshotCreateInfoANDROID(
            NativeArray<XrSpatialComponentTypeEXT>.ReadOnly componentTypes,
            in XrSpatialRaycastInfoANDROID raycastInfo)
            : this(
                null,
                (uint)componentTypes.Length,
                (XrSpatialComponentTypeEXT*)componentTypes.GetUnsafeReadOnlyPtr(),
                in raycastInfo)
        { }
    }
}
