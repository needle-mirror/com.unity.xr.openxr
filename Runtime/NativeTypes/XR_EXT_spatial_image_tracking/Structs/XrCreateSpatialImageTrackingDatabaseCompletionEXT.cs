using XrSpatialImageTrackingDatabaseEXT = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Completion struct populated by
    /// `xrCreateSpatialImageTrackingDatabaseCompleteEXT`
    /// when the future returned by
    /// `xrCreateSpatialImageTrackingDatabaseAsyncEXT`
    /// is ready.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use either <see cref="defaultValue"/> or a constructor with parameters to ensure that <see cref="type"/>
    /// > is correctly initialized to <see cref="XrStructureType.CreateSpatialImageTrackingDatabaseCompletionEXT"/>.
    /// </remarks>
    /// <seealso cref="M:UnityEngine.XR.OpenXR.NativeTypes.OpenXRNativeApi.xrCreateSpatialImageTrackingDatabaseCompleteEXT(System.UInt64,System.UInt64,UnityEngine.XR.OpenXR.NativeTypes.XrCreateSpatialImageTrackingDatabaseCompletionEXT@)"/>
    /// <seealso cref="M:UnityEngine.XR.OpenXR.NativeTypes.OpenXRNativeApi.xrCreateSpatialImageTrackingDatabaseCompleteEXT(System.UInt64,UnityEngine.XR.OpenXR.NativeTypes.XrCreateSpatialImageTrackingDatabaseCompletionEXT@)"/>
    public unsafe struct XrCreateSpatialImageTrackingDatabaseCompletionEXT
    {
        /// <summary>
        /// Get a default instance with an initialized <see cref="type"/> property.
        /// </summary>
        public static XrCreateSpatialImageTrackingDatabaseCompletionEXT defaultValue => new(default, default, default);

        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.CreateSpatialImageTrackingDatabaseCompletionEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; set; }

        /// <summary>
        /// The result of the asynchronous database creation operation.
        /// </summary>
        /// <remarks>
        /// Success codes:
        /// * <see cref="XrResult.Success"/>
        /// * <see cref="XrResult.LossPending"/>
        ///
        /// Failure codes:
        /// * <see cref="XrResult.RuntimeFailure"/>
        /// * <see cref="XrResult.InstanceLost"/>
        /// * <see cref="XrResult.SessionLost"/>
        /// * <see cref="XrResult.OutOfMemory"/>
        /// * <see cref="XrResult.LimitReached"/>
        /// * <see cref="XrResult.SpatialImageFormatUnsupportedEXT"/>
        /// * <see cref="XrResult.SpatialImageInvalidEXT"/>
        /// </remarks>
        public XrResult futureResult { get; set; }

        /// <summary>
        /// The newly created database handle, if `futureResult.IsSuccess()`.
        /// </summary>
        /// <remarks>
        /// If `futureResult.IsSuccess()`, the database is valid within the lifecycle of the session passed to
        /// `xrCreateSpatialImageTrackingDatabaseAsyncEXT`
        /// or until you destroy it with
        /// <see cref="OpenXRNativeApi.xrDestroySpatialImageTrackingDatabaseEXT"/>, whichever comes first.
        /// </remarks>
        public XrSpatialImageTrackingDatabaseEXT database { get; set; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="futureResult">The result of the asynchronous database creation operation.</param>
        /// <param name="database">The newly created database handle.</param>
        public XrCreateSpatialImageTrackingDatabaseCompletionEXT(
            void* next, XrResult futureResult, XrSpatialImageTrackingDatabaseEXT database)
        {
            type = XrStructureType.CreateSpatialImageTrackingDatabaseCompletionEXT;
            this.next = next;
            this.futureResult = futureResult;
            this.database = database;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="futureResult">The result of the asynchronous database creation operation.</param>
        /// <param name="database">The newly created database handle.</param>
        public XrCreateSpatialImageTrackingDatabaseCompletionEXT(
            XrResult futureResult, XrSpatialImageTrackingDatabaseEXT database)
            : this(null, futureResult, database) { }
    }
}
