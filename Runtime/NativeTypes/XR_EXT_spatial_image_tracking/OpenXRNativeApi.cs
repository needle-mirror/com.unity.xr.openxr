using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using XrFutureEXT = System.UInt64;
using XrInstance = System.UInt64;
using XrSession = System.UInt64;
using XrSpatialImageTrackingDatabaseEXT = System.UInt64;
using XrSystemId = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    public static unsafe partial class OpenXRNativeApi
    {
        /// <summary>
        /// Begins asynchronous creation of an `XrSpatialImageTrackingDatabaseEXT`.
        /// Use `xrCreateSpatialImageTrackingDatabaseCompleteEXT` with the returned future to retrieve the database
        /// handle once it is ready.
        /// Provided by `XR_EXT_spatial_image_tracking`.
        /// </summary>
        /// <param name="session">The session in which to create the database.</param>
        /// <param name="createInfo">The information used to specify the reference images and options.</param>
        /// <param name="future">The future that resolves the created database once ready.</param>
        /// <returns>The result of the operation.\
        /// \
        /// Success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        ///   <item><description><see cref="XrResult.LossPending"/></description></item>
        /// </list>
        /// Failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.SessionLost"/></description></item>
        ///   <item><description><see cref="XrResult.PermissionInsufficient"/></description></item>
        ///   <item><description><see cref="XrResult.LimitReached"/></description></item>
        ///   <item><description><see cref="XrResult.OutOfMemory"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialImageSizeMissingEXT"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialImageInvalidEXT"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialImageFormatUnsupportedEXT"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        [DllImport(InternalConstants.openXRLibrary,
            EntryPoint = "EXT_spatial_image_tracking_xrCreateSpatialImageTrackingDatabaseAsyncEXT")]
        public static extern XrResult xrCreateSpatialImageTrackingDatabaseAsyncEXT(
            XrSession session,
            in XrSpatialImageTrackingDatabaseCreateInfoEXT createInfo,
            out XrFutureEXT future);

        /// <summary>
        /// Begins asynchronous creation of an `XrSpatialImageTrackingDatabaseEXT` in the current session.
        /// Provided by `XR_EXT_spatial_image_tracking`.
        /// </summary>
        /// <param name="createInfo">The information used to specify the reference images and options.</param>
        /// <param name="future">The future that resolves the created database once ready.</param>
        /// <returns>The result of the operation.\
        /// \
        /// `nativeStatusCode` success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        ///   <item><description><see cref="XrResult.LossPending"/></description></item>
        /// </list>
        /// `nativeStatusCode` failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.SessionLost"/></description></item>
        ///   <item><description><see cref="XrResult.PermissionInsufficient"/></description></item>
        ///   <item><description><see cref="XrResult.LimitReached"/></description></item>
        ///   <item><description><see cref="XrResult.OutOfMemory"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialImageSizeMissingEXT"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialImageInvalidEXT"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialImageFormatUnsupportedEXT"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        [DllImport(InternalConstants.openXRLibrary,
            EntryPoint = "EXT_spatial_image_tracking_xrCreateSpatialImageTrackingDatabaseAsyncEXT_usingContext")]
        public static extern OpenXRResultStatus xrCreateSpatialImageTrackingDatabaseAsyncEXT(
            in XrSpatialImageTrackingDatabaseCreateInfoEXT createInfo,
            out XrFutureEXT future);

        /// <summary>
        /// Completes the asynchronous database creation initiated by
        /// `xrCreateSpatialImageTrackingDatabaseAsyncEXT`.
        /// Call once the corresponding future reports <see cref="XrFutureStateEXT.Ready"/>.
        /// Provided by `XR_EXT_spatial_image_tracking`.
        /// </summary>
        /// <param name="session">The session previously passed to
        /// `xrCreateSpatialImageTrackingDatabaseAsyncEXT`.</param>
        /// <param name="future">The future received from
        /// `xrCreateSpatialImageTrackingDatabaseAsyncEXT`.</param>
        /// <param name="completion">The output completion struct. Its
        /// <see cref="XrCreateSpatialImageTrackingDatabaseCompletionEXT.futureResult"/> carries the real result of the
        /// creation operation.</param>
        /// <returns>The result of the operation.\
        /// \
        /// Success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        ///   <item><description><see cref="XrResult.LossPending"/></description></item>
        /// </list>
        /// Failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.SessionLost"/></description></item>
        ///   <item><description><see cref="XrResult.FuturePendingEXT"/></description></item>
        ///   <item><description><see cref="XrResult.FutureInvalidEXT"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        public static XrResult xrCreateSpatialImageTrackingDatabaseCompleteEXT(
            XrSession session, XrFutureEXT future, out XrCreateSpatialImageTrackingDatabaseCompletionEXT completion)
        {
            completion = XrCreateSpatialImageTrackingDatabaseCompletionEXT.defaultValue;
            return xrCreateSpatialImageTrackingDatabaseCompleteEXT_native(session, future, ref completion);
        }

        [DllImport(InternalConstants.openXRLibrary,
            EntryPoint = "EXT_spatial_image_tracking_xrCreateSpatialImageTrackingDatabaseCompleteEXT")]
        static extern XrResult xrCreateSpatialImageTrackingDatabaseCompleteEXT_native(
            XrSession session, XrFutureEXT future, ref XrCreateSpatialImageTrackingDatabaseCompletionEXT completion);

        /// <summary>
        /// Completes the asynchronous database creation initiated by
        /// `xrCreateSpatialImageTrackingDatabaseAsyncEXT`
        /// in the current session. Call once the corresponding future reports <see cref="XrFutureStateEXT.Ready"/>.
        /// Provided by `XR_EXT_spatial_image_tracking`.
        /// </summary>
        /// <param name="future">The future received from `xrCreateSpatialImageTrackingDatabaseAsyncEXT`.</param>
        /// <param name="completion">The output completion struct. Its
        /// <see cref="XrCreateSpatialImageTrackingDatabaseCompletionEXT.futureResult"/> carries the real result of the creation operation.</param>
        /// <returns>The result of the operation.\
        /// \
        /// Success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        ///   <item><description><see cref="XrResult.LossPending"/></description></item>
        /// </list>
        /// Failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.SessionLost"/></description></item>
        ///   <item><description><see cref="XrResult.FuturePendingEXT"/></description></item>
        ///   <item><description><see cref="XrResult.FutureInvalidEXT"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        public static OpenXRResultStatus xrCreateSpatialImageTrackingDatabaseCompleteEXT(
            XrFutureEXT future, out XrCreateSpatialImageTrackingDatabaseCompletionEXT completion)
        {
            completion = XrCreateSpatialImageTrackingDatabaseCompletionEXT.defaultValue;
            return xrCreateSpatialImageTrackingDatabaseCompleteEXT_usingContext(future, ref completion);
        }

        [DllImport(InternalConstants.openXRLibrary,
            EntryPoint = "EXT_spatial_image_tracking_xrCreateSpatialImageTrackingDatabaseCompleteEXT_usingContext")]
        static extern OpenXRResultStatus xrCreateSpatialImageTrackingDatabaseCompleteEXT_usingContext(
            XrFutureEXT future, ref XrCreateSpatialImageTrackingDatabaseCompletionEXT completion);

        /// <summary>
        /// Releases an `XrSpatialImageTrackingDatabaseEXT` handle and its underlying resources.
        /// Provided by `XR_EXT_spatial_image_tracking`.
        /// </summary>
        /// <param name="database">The database to destroy.</param>
        /// <returns>The result of the operation.\
        /// \
        /// Success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        /// </list>
        /// Failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        /// </list>
        /// </returns>
        [DllImport(InternalConstants.openXRLibrary,
            EntryPoint = "EXT_spatial_image_tracking_xrDestroySpatialImageTrackingDatabaseEXT")]
        public static extern XrResult xrDestroySpatialImageTrackingDatabaseEXT(
            XrSpatialImageTrackingDatabaseEXT database);

        /// <summary>
        /// Enumerates the supported reference image formats for the given system and capability.
        /// Pass `formatCapacityInput=0` to first query the required size.
        /// Provided by `XR_EXT_spatial_image_tracking`.
        /// </summary>
        /// <param name="instance">The OpenXR instance.</param>
        /// <param name="systemId">The system whose supported formats will be enumerated.</param>
        /// <param name="capability">The capability for which the formats will be enumerated.</param>
        /// <param name="formatCapacityInput">The capacity of <paramref name="formats"/>, or `0` to indicate a request
        /// to retrieve the required capacity.</param>
        /// <param name="formatCountOutput">The count of elements in <paramref name="formats"/>, or the required
        /// capacity if <paramref name="formatCapacityInput"/> is insufficient.</param>
        /// <param name="formats">Pointer to an array of formats. Can be null if <paramref name="formatCapacityInput"/>
        /// is `0`.</param>
        /// <returns>The result of the operation.\
        /// \
        /// Success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        /// </list>
        /// Failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.SizeInsufficient"/></description></item>
        ///   <item><description><see cref="XrResult.SystemInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialCapabilityUnsupportedEXT"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        /// </remarks>
        [DllImport(InternalConstants.openXRLibrary,
            EntryPoint = "EXT_spatial_image_tracking_xrEnumerateSpatialReferenceImageFormatsEXT")]
        public static extern XrResult xrEnumerateSpatialReferenceImageFormatsEXT(
            XrInstance instance,
            XrSystemId systemId,
            XrSpatialCapabilityEXT capability,
            uint formatCapacityInput,
            out uint formatCountOutput,
            XrSpatialReferenceImageFormatEXT* formats);

        [DllImport(InternalConstants.openXRLibrary,
            EntryPoint = "EXT_spatial_image_tracking_xrEnumerateSpatialReferenceImageFormatsEXT_usingContext")]
        static extern OpenXRResultStatus xrEnumerateSpatialReferenceImageFormatsEXT_usingContext(
            XrSpatialCapabilityEXT capability,
            uint formatCapacityInput,
            out uint formatCountOutput,
            XrSpatialReferenceImageFormatEXT* formats);

        /// <summary>
        /// Enumerates the supported reference image formats for the current system and the given capability, performing
        /// the two-call enumeration idiom for you.
        /// Provided by `XR_EXT_spatial_image_tracking`.
        /// </summary>
        /// <param name="capability">The capability for which the formats will be enumerated.</param>
        /// <param name="allocator">The allocation strategy to use for <paramref name="formats"/>.</param>
        /// <param name="formats">The array of supported formats.</param>
        /// <returns>The result of the operation.\
        /// \
        /// `nativeStatusCode` success codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.Success"/></description></item>
        /// </list>
        /// `nativeStatusCode` failure codes:
        /// <list type="bullet">
        ///   <item><description><see cref="XrResult.FunctionUnsupported"/></description></item>
        ///   <item><description><see cref="XrResult.ValidationFailure"/></description></item>
        ///   <item><description><see cref="XrResult.RuntimeFailure"/></description></item>
        ///   <item><description><see cref="XrResult.HandleInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.InstanceLost"/></description></item>
        ///   <item><description><see cref="XrResult.SizeInsufficient"/></description></item>
        ///   <item><description><see cref="XrResult.SystemInvalid"/></description></item>
        ///   <item><description><see cref="XrResult.SpatialCapabilityUnsupportedEXT"/></description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > Output parameters are only valid if the returned result `.IsSuccess()`.
        /// > Don't read the output if an error is returned.
        ///
        /// You are responsible to `Dispose` the output native array if you pass `Allocator.Persistent` as the
        /// <paramref name="allocator"/> value.
        /// </remarks>
        /// <exception cref="OverflowException">Thrown if the enumerated count would exceed
        /// <see cref="Int32.MaxValue"/>.</exception>
        public static OpenXRResultStatus xrEnumerateSpatialReferenceImageFormatsEXT(
            XrSpatialCapabilityEXT capability,
            Allocator allocator,
            out NativeArray<XrSpatialReferenceImageFormatEXT> formats)
        {
            var result = xrEnumerateSpatialReferenceImageFormatsEXT_usingContext(
                capability, 0, out var formatCountOutput, null);
            if (result.IsError())
            {
                formats = default;
                return result;
            }

            formats = new NativeArray<XrSpatialReferenceImageFormatEXT>(checked((int)formatCountOutput), allocator);
            return xrEnumerateSpatialReferenceImageFormatsEXT_usingContext(
                capability, formatCountOutput, out _, (XrSpatialReferenceImageFormatEXT*)formats.GetUnsafePtr());
        }
    }
}
