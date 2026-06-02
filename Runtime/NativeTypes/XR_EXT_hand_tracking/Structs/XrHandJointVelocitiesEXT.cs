using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Contains the velocities of hand joints. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandJointVelocitiesEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrHandJointVelocitiesEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandJointVelocitiesEXT defaultValue => new(0, null);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandJointVelocitiesEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// The count of elements in <see cref="jointVelocities"/>.
        /// </summary>
        public uint jointCount { get; }

        /// <summary>
        /// Pointer to an array of <see cref="XrHandJointVelocityEXT"/> structures.
        /// </summary>
        public XrHandJointVelocityEXT* jointVelocities { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="jointCount">The count of elements in <paramref name="jointVelocities"/>.</param>
        /// <param name="jointVelocities">Pointer to an array of joint velocities.</param>
        public XrHandJointVelocitiesEXT(
            void* next, uint jointCount, XrHandJointVelocityEXT* jointVelocities)
        {
            type = XrStructureType.HandJointVelocitiesEXT;
            this.next = next;
            this.jointCount = jointCount;
            this.jointVelocities = jointVelocities;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="jointCount">The count of elements in <paramref name="jointVelocities"/>.</param>
        /// <param name="jointVelocities">Pointer to an array of joint velocities.</param>
        public XrHandJointVelocitiesEXT(uint jointCount, XrHandJointVelocityEXT* jointVelocities)
            : this(null, jointCount, jointVelocities) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="jointVelocities">Native array of joint velocities.</param>
        public XrHandJointVelocitiesEXT(
            void* next, NativeArray<XrHandJointVelocityEXT> jointVelocities)
            : this(next, (uint)jointVelocities.Length,
                   (XrHandJointVelocityEXT*)jointVelocities.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="jointVelocities">Native array of joint velocities.</param>
        public XrHandJointVelocitiesEXT(
            NativeArray<XrHandJointVelocityEXT> jointVelocities)
            : this(null, (uint)jointVelocities.Length,
                   (XrHandJointVelocityEXT*)jointVelocities.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="jointVelocities">Read-only native array of joint velocities.</param>
        public XrHandJointVelocitiesEXT(
            void* next, NativeArray<XrHandJointVelocityEXT>.ReadOnly jointVelocities)
            : this(next, (uint)jointVelocities.Length,
                   (XrHandJointVelocityEXT*)jointVelocities.GetUnsafeReadOnlyPtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array.
        /// </summary>
        /// <param name="jointVelocities">Read-only native array of joint velocities.</param>
        public XrHandJointVelocitiesEXT(
            NativeArray<XrHandJointVelocityEXT>.ReadOnly jointVelocities)
            : this(null, (uint)jointVelocities.Length,
                   (XrHandJointVelocityEXT*)jointVelocities.GetUnsafeReadOnlyPtr())
        { }
    }
}
