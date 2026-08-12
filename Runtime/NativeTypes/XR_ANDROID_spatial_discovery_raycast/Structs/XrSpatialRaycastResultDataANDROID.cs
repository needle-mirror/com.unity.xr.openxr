using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// The raycast result component, which describes where a ray hit a spatial entity.
    /// Provided by `XR_ANDROID_spatial_discovery_raycast`.
    /// </summary>
    public readonly struct XrSpatialRaycastResultDataANDROID : IEquatable<XrSpatialRaycastResultDataANDROID>
    {
        /// <summary>
        /// The pose defining the point where the ray intersects with the spatial entity.
        /// The positive Z axis is the normal of the hit surface, and the negative Y axis roughly points
        /// toward the ray origin.
        /// </summary>
        /// <remarks>
        /// The space and time this pose is located in depend on how the snapshot was created:
        ///
        /// <list type="bullet">
        /// <item><description>For a snapshot from `OpenXRNativeApi.xrCreateSpatialDiscoverySnapshotAsyncEXT`,
        /// the pose is located in `XrCreateSpatialDiscoverySnapshotCompletionInfoEXT.baseSpace` at that
        /// struct's `time`.</description></item>
        /// <item><description>For a snapshot from `OpenXRNativeApi.xrCreateSpatialRaycastSnapshotANDROID`,
        /// there is no completion info. The pose is located in
        /// <see cref="XrSpatialRaycastInfoANDROID.space"/> at
        /// <see cref="XrSpatialRaycastInfoANDROID.time"/>.</description></item>
        /// </list>
        /// </remarks>
        public XrPosef hitPose { get; }

        /// <summary>
        /// The square of the distance from the origin of the ray to the intersection point.
        /// </summary>
        public float distanceSquared { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="hitPose">The pose defining the point where the ray intersects with the
        /// spatial entity.</param>
        /// <param name="distanceSquared">The square of the distance from the origin of the ray to the
        /// intersection point.</param>
        public XrSpatialRaycastResultDataANDROID(XrPosef hitPose, float distanceSquared)
        {
            this.hitPose = hitPose;
            this.distanceSquared = distanceSquared;
        }

        /// <summary>
        /// Compares for equality.
        /// Two instances are equal if their `hitPose` and `distanceSquared` properties are exactly equal.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>`true` if the instances are equal, otherwise `false`.</returns>
        public bool Equals(XrSpatialRaycastResultDataANDROID other)
        {
            return hitPose.Equals(other.hitPose) && distanceSquared.Equals(other.distanceSquared);
        }

        /// <summary>
        /// Compares for equality.
        /// Two instances are equal if their `hitPose` and `distanceSquared` properties are exactly equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>`true` if `obj` is an `XrSpatialRaycastResultDataANDROID` and equal to this instance.
        /// Otherwise, `false`.</returns>
        public override bool Equals(object obj)
        {
            return obj is XrSpatialRaycastResultDataANDROID other && Equals(other);
        }

        /// <summary>
        /// Generates a unique hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(hitPose, distanceSquared);
        }
    }
}
