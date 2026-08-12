using XrSpace = System.UInt64;
using XrTime = System.Int64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Describes a ray used to discover spatial entities that intersect with it. Include this struct in the
    /// <see cref="XrSpatialDiscoverySnapshotCreateInfoEXT.next"/> chain, or reference it from
    /// <see cref="XrSpatialRaycastSnapshotCreateInfoANDROID.raycastInfo"/>.
    /// Provided by `XR_ANDROID_spatial_discovery_raycast`.
    /// </summary>
    /// <remarks>
    /// The spatial context must have <see cref="XrSpatialComponentTypeEXT.RaycastResult"/> enabled for at least
    /// one capability, otherwise snapshot creation fails with
    /// <see cref="XrResult.SpatialComponentNotEnabledEXT"/>.
    ///
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialRaycastInfoANDROID"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialRaycastInfoANDROID
    {
        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.SpatialRaycastInfoANDROID"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The `XrSpace` in which the pose of <see cref="XrSpatialRaycastResultDataANDROID"/> will be located.
        /// </summary>
        public XrSpace space { get; }

        /// <summary>
        /// The time at which the pose of <see cref="XrSpatialRaycastResultDataANDROID"/> is defined.
        /// </summary>
        public XrTime time { get; }

        /// <summary>
        /// The origin of the ray, relative to the specified space.
        /// </summary>
        public XrVector3f origin { get; }

        /// <summary>
        /// The direction of the ray, relative to the specified space.
        /// </summary>
        public XrVector3f direction { get; }

        /// <summary>
        /// The distance in meters used to limit discovery to entities within that distance from
        /// <see cref="origin"/>, along <see cref="direction"/>. If this value is `0` or negative,
        /// the runtime considers the ray unbounded.
        /// </summary>
        public float maxDistance { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="space">The space in which the raycast result pose will be located.</param>
        /// <param name="time">The time at which the raycast result pose is defined.</param>
        /// <param name="origin">The origin of the ray, relative to the specified space.</param>
        /// <param name="direction">The direction of the ray, relative to the specified space.</param>
        /// <param name="maxDistance">The maximum distance in meters, or `0` for an unbounded ray.</param>
        public XrSpatialRaycastInfoANDROID(
            void* next,
            XrSpace space,
            XrTime time,
            XrVector3f origin,
            XrVector3f direction,
            float maxDistance)
        {
            type = XrStructureType.SpatialRaycastInfoANDROID;
            this.next = next;
            this.space = space;
            this.time = time;
            this.origin = origin;
            this.direction = direction;
            this.maxDistance = maxDistance;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="space">The space in which the raycast result pose will be located.</param>
        /// <param name="time">The time at which the raycast result pose is defined.</param>
        /// <param name="origin">The origin of the ray, relative to the specified space.</param>
        /// <param name="direction">The direction of the ray, relative to the specified space.</param>
        /// <param name="maxDistance">The maximum distance in meters, or `0` for an unbounded ray.</param>
        public XrSpatialRaycastInfoANDROID(
            XrSpace space, XrTime time, XrVector3f origin, XrVector3f direction, float maxDistance)
            : this(null, space, time, origin, direction, maxDistance) { }

        /// <summary>
        /// Construct an instance with a `null` next pointer and an unbounded ray.
        /// </summary>
        /// <param name="space">The space in which the raycast result pose will be located.</param>
        /// <param name="time">The time at which the raycast result pose is defined.</param>
        /// <param name="origin">The origin of the ray, relative to the specified space.</param>
        /// <param name="direction">The direction of the ray, relative to the specified space.</param>
        public XrSpatialRaycastInfoANDROID(
            XrSpace space, XrTime time, XrVector3f origin, XrVector3f direction)
            : this(null, space, time, origin, direction, 0) { }

        /// <summary>
        /// Try to construct an instance using your app space and the next frame's predicted display time,
        /// so that you don't need to supply <see cref="space"/> and <see cref="time"/>.
        /// </summary>
        /// <param name="origin">The origin of the ray, in session space relative to your app space
        /// (XR Origin), not Unity world space.</param>
        /// <param name="direction">The direction of the ray, in session space relative to your app space
        /// (XR Origin), not Unity world space.</param>
        /// <param name="maxDistance">The maximum distance in meters, or `0` for an unbounded ray.</param>
        /// <param name="info">The constructed instance, if this method returns `true`.</param>
        /// <returns>`true` if the app space and time were both available. Otherwise, `false`.</returns>
        /// <remarks>
        /// The app space and time only become available once the OpenXR provider is running and has
        /// completed a frame. Before then this method returns `false`.
        /// </remarks>
        public static bool TryCreate(
            XrVector3f origin, XrVector3f direction, float maxDistance, out XrSpatialRaycastInfoANDROID info)
        {
            if (!Features.OpenXRFeature.Internal_GetAppSpaceAndPredictedDisplayTime(
                    out var appSpace, out var predictedDisplayTime))
            {
                info = default;
                return false;
            }

            info = new XrSpatialRaycastInfoANDROID(
                appSpace, predictedDisplayTime, origin, direction, maxDistance);
            return true;
        }
    }
}
