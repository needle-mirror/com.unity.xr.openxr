using XrBool32 = System.UInt32;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Extends `XrHandJointLocationsEXT` to report which hand tracking data source
    /// the runtime used to produce the joint locations for the most recent
    /// `xrLocateHandJointsEXT` call. Provided by `XR_EXT_hand_tracking_data_source`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.HandTrackingDataSourceStateEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrHandTrackingDataSourceStateEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrHandTrackingDataSourceStateEXT defaultValue => new(null, false, default);

        /// <summary>
        /// The `XrStructureType` of this struct: <see cref="XrStructureType.HandTrackingDataSourceStateEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// Indicates whether <see cref="dataSource"/> contains a valid value reported by the runtime.
        /// </summary>
        public XrBool32 isActive { get; }

        /// <summary>
        /// The data source the runtime used to produce the joint locations for the most recent
        /// `xrLocateHandJointsEXT` call. Only valid when <see cref="isActive"/> is non-zero.
        /// </summary>
        public XrHandTrackingDataSourceEXT dataSource { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">The next pointer.</param>
        /// <param name="isActive">`true` if <paramref name="dataSource"/> contains a valid value
        /// reported by the runtime. Otherwise, `false`.</param>
        /// <param name="dataSource">The data source the runtime used to produce the joint locations.
        /// Only valid when <paramref name="isActive"/> is `true`.</param>
        public XrHandTrackingDataSourceStateEXT(
            void* next, bool isActive, XrHandTrackingDataSourceEXT dataSource)
        {
            type = XrStructureType.HandTrackingDataSourceStateEXT;
            this.next = next;
            this.isActive = isActive ? 1u : 0;
            this.dataSource = dataSource;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="isActive">`true` if <paramref name="dataSource"/> contains a valid value
        /// reported by the runtime. Otherwise, `false`.</param>
        /// <param name="dataSource">The data source the runtime used to produce the joint locations.
        /// Only valid when <paramref name="isActive"/> is `true`.</param>
        public XrHandTrackingDataSourceStateEXT(
            bool isActive, XrHandTrackingDataSourceEXT dataSource)
            : this(null, isActive, dataSource) { }
    }
}
