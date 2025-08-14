using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// A construct representing a position and orientation within a space, with position expressed in meters, and orientation represented as a unit quaternion.
    /// <a href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/XrPosef.html">OpenXR Spec</a>
    /// </summary>
    public struct XrPosef : IEquatable<XrPosef>
    {
        /// <summary>
        /// The orientation/rotation of the pose.
        /// </summary>
        public XrQuaternionf Orientation;

        /// <summary>
        /// The position of the pose.
        /// </summary>
        public XrVector3f Position;

        /// <summary>
        /// Initializes and returns an instance of XrPosef with the provided parameters.
        /// </summary>
        /// <param name="vec3">vector3 position.</param>
        /// <param name="quaternion">quaternion orientation.</param>
        public XrPosef(Vector3 vec3, Quaternion quaternion)
        {
            Position = new XrVector3f(vec3);
            Orientation = new XrQuaternionf(quaternion);
        }

        /// <summary>
        /// Compares for equality.
        /// Two instances are equal if their bits are exactly identical for the `Orientation` and `Position` fields.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>`true` if the instances are equal. Otherwise, `false`.</returns>
        public bool Equals(XrPosef other)
        {
            return Orientation.Equals(other.Orientation) && Position.Equals(other.Position);
        }

        /// <summary>
        /// Compares for equality.
        /// Two instances are equal if their bits are exactly identical for the `Orientation` and `Position` fields.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>`true` if `obj` is an `XrPosef` and equal to this instance. Otherwise, `false`.</returns>
        public override bool Equals(object obj)
        {
            return obj is XrPosef other && Equals(other);
        }

        /// <summary>
        /// Generates a unique hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Orientation, Position);
        }
    }
}
