using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using XrBool32 = System.UInt32;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Contains the output of a hand joint locate operation. Provided by `XR_EXT_hand_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandJointLocationsEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrHandJointLocationsEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandJointLocationsEXT defaultValue => new(false, 0, null);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandJointLocationsEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// Indicates whether the hand tracker is actively tracking.
        /// </summary>
        public XrBool32 isActive { get; }

        /// <summary>
        /// The count of elements in <see cref="jointLocations"/>.
        /// </summary>
        public uint jointCount { get; }

        /// <summary>
        /// Pointer to an array of <see cref="XrHandJointLocationEXT"/> structures.
        /// </summary>
        public XrHandJointLocationEXT* jointLocations { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="isActive">`true` if the hand tracker is actively tracking. Otherwise, `false`.</param>
        /// <param name="jointCount">The count of elements in <paramref name="jointLocations"/>.</param>
        /// <param name="jointLocations">Pointer to an array of joint locations.</param>
        public XrHandJointLocationsEXT(
            void* next, bool isActive, uint jointCount, XrHandJointLocationEXT* jointLocations)
        {
            type = XrStructureType.HandJointLocationsEXT;
            this.next = next;
            this.isActive = isActive ? 1u : 0;
            this.jointCount = jointCount;
            this.jointLocations = jointLocations;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="isActive">`true` if the hand tracker is actively tracking. Otherwise, `false`.</param>
        /// <param name="jointCount">The count of elements in <paramref name="jointLocations"/>.</param>
        /// <param name="jointLocations">Pointer to an array of joint locations.</param>
        public XrHandJointLocationsEXT(
            bool isActive, uint jointCount, XrHandJointLocationEXT* jointLocations)
            : this(null, isActive, jointCount, jointLocations) { }

        /// <summary>
        /// Construct an instance from a native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="isActive">`true` if the hand tracker is actively tracking. Otherwise, `false`.</param>
        /// <param name="jointLocations">Native array of joint locations.</param>
        public XrHandJointLocationsEXT(
            void* next, bool isActive, NativeArray<XrHandJointLocationEXT> jointLocations)
            : this(next, isActive, (uint)jointLocations.Length,
                   (XrHandJointLocationEXT*)jointLocations.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a native array.
        /// </summary>
        /// <param name="isActive">`true` if the hand tracker is actively tracking. Otherwise, `false`.</param>
        /// <param name="jointLocations">Native array of joint locations.</param>
        public XrHandJointLocationsEXT(
            bool isActive, NativeArray<XrHandJointLocationEXT> jointLocations)
            : this(null, isActive, (uint)jointLocations.Length,
                   (XrHandJointLocationEXT*)jointLocations.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance from a read-only native array.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="isActive">`true` if the hand tracker is actively tracking. Otherwise, `false`.</param>
        /// <param name="jointLocations">Read-only native array of joint locations.</param>
        public XrHandJointLocationsEXT(
            void* next, bool isActive, NativeArray<XrHandJointLocationEXT>.ReadOnly jointLocations)
            : this(next, isActive, (uint)jointLocations.Length,
                   (XrHandJointLocationEXT*)jointLocations.GetUnsafeReadOnlyPtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from a read-only native array.
        /// </summary>
        /// <param name="isActive">`true` if the hand tracker is actively tracking. Otherwise, `false`.</param>
        /// <param name="jointLocations">Read-only native array of joint locations.</param>
        public XrHandJointLocationsEXT(
            bool isActive, NativeArray<XrHandJointLocationEXT>.ReadOnly jointLocations)
            : this(null, isActive, (uint)jointLocations.Length,
                   (XrHandJointLocationEXT*)jointLocations.GetUnsafeReadOnlyPtr())
        { }
    }
}
