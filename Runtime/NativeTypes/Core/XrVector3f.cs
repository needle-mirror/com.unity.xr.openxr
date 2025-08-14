using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Represents a three-dimensional vector in OpenXR coordinates.
    /// </summary>
    public struct XrVector3f : IEquatable<XrVector3f>
    {
        /// <summary>
        /// The x coordinate of the vector, in OpenXR coordinates.
        /// </summary>
        public float X;

        /// <summary>
        /// The y coordinate of the vector, in OpenXR coordinates.
        /// </summary>
        public float Y;

        /// <summary>
        /// The z coordinate of the vector, in OpenXR coordinates.
        /// </summary>
        public float Z;

        /// <summary>
        /// Construct an instance from the given Unity coordinates.
        /// </summary>
        /// <param name="x">The x coordinate from Unity's coordinate system.</param>
        /// <param name="y">The y coordinate from Unity's coordinate system.</param>
        /// <param name="z">The z coordinate from Unity's coordinate system.</param>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > This constructor negates the <paramref name="z"/> value to convert from Unity's left-handed coordinate
        /// > system to the right-handed system used by OpenXR.
        /// </remarks>
        public XrVector3f(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = -z;
        }

        /// <summary>
        /// Construct an instance from the given `Vector3`.
        /// </summary>
        /// <param name="value">The input `Vector3` from Unity's coordinate system.</param>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > This constructor negates the z value of the input `Vector3` to convert from Unity's left-handed coordinate
        /// > system to the right-handed system used by OpenXR.
        /// </remarks>
        public XrVector3f(Vector3 value)
        {
            X = value.x;
            Y = value.y;
            Z = -value.z;
        }

        /// <summary>
        /// Convert this instance in OpenXR coordinates to a `Vector3` in Unity coordinates.
        /// </summary>
        /// <returns>The output `Vector3`.</returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > This method negates the <see cref="Z"/> value to convert from OpenXR's right-handed
        /// > coordinate system to the left-handed system used by Unity.
        /// </remarks>
        public Vector3 AsVector3()
        {
            return new Vector3(X, Y, -Z);
        }

        /// <summary>
        /// Compares for equality.
        /// Two instances are equal if their bits are identical for the `X`, `Y`, and `Z` fields.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>`true` if the instances are equal. Otherwise, `false`.</returns>
        public bool Equals(XrVector3f other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <summary>
        /// Compares for equality.
        /// Two instances are equal if their bits are identical for the `X`, `Y`, and `Z` fields.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>`true` if `obj` is an `XrVector3f` and equal to this instance. Otherwise, `false`.</returns>
        public override bool Equals(object obj)
        {
            return obj is XrVector3f other && Equals(other);
        }

        /// <summary>
        /// Generates a unique hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }
}
