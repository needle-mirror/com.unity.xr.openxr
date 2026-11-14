using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.Extensions.StationaryReferenceSpace
{
    /// <summary>
    /// Allows creation, management, and usage of the stationary reference space on supported XR runtimes.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(
        UiName = "Stationary Reference Space",
        Desc = "Optional extension for enabling usage of the stationary reference space",
        Company = "Unity",
        DocumentationLink = Constants.k_DocumentationManualURL + "features/stationary-reference-space.html",
        OpenxrExtensionStrings = extensionString,
        Version = "0.0.1",
        FeatureId = featureId
    )]
#endif
    public class XrStationaryReferenceSpaceFeature : OpenXRFeature
    {
        /// <summary>
        /// The feature id string for stationary reference space.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.extension.stationary_reference_space";

        /// <summary>
        /// Name of the OpenXR extension for stationary reference space.
        /// </summary>
        public const string extensionString = "XR_EXT_stationary_reference_space";

        /// <inheritdoc />
        protected internal override bool OnInstanceCreate(ulong xrInstance)
        {
            return base.OnInstanceCreate(xrInstance) &&
                OpenXRRuntime.IsExtensionEnabled(extensionString);
        }

        /// <inheritdoc />
        protected internal override void OnReferenceSpaceChanged(ulong referenceSpaceType, Pose? poseInPreviousSpace)
        {
            base.OnReferenceSpaceChanged(referenceSpaceType, poseInPreviousSpace);
            if (referenceSpaceType == (ulong)XrReferenceSpaceType.StationaryReferenceSpaceExt)
            {
                stationarySpaceChanged?.Invoke(new StationarySpaceChangedEventArgs(
                    this, poseInPreviousSpace.HasValue, poseInPreviousSpace.GetValueOrDefault(Pose.identity)));
            }
        }

        /// <summary>
        /// Initializes a stationary reference space with the identity pose in space's own coordinate system.
        /// </summary>
        /// <returns>The result of the operation. Implicitly converts to `true` if and only if the space was
        /// created.\
        /// \
        /// `statusCode` values:
        /// <list type="bullet">
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.UnqualifiedSuccess"/> if the space was
        ///   created.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ProviderUninitialized"/> if the session
        ///   has not yet started.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.Unsupported"/> if the runtime does not
        ///   offer the stationary reference space.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.PlatformError"/> if the runtime call
        ///   failed. See <see cref="OpenXRResultStatus.nativeStatusCode"/> for the runtime's `XrResult`.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// If a stationary reference space already exists, it's destroyed first.
        /// </remarks>
        public OpenXRResultStatus InitializeStationaryReferenceSpace()
        {
            return InitializeStationaryReferenceSpace(Pose.identity);
        }

        /// <summary>
        /// Initializes a stationary reference space with the specified pose offset in space's own coordinate system.
        /// </summary>
        /// <param name="poseOffset">The pose offset, in the stationary reference space's own coordinate system.</param>
        /// <returns>The result of the operation. Implicitly converts to `true` if and only if the space was
        /// created.\
        /// \
        /// `statusCode` values:
        /// <list type="bullet">
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.UnqualifiedSuccess"/> if the space was
        ///   created.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ProviderUninitialized"/> if the session
        ///   has not yet started.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.Unsupported"/> if the runtime does not
        ///   offer the stationary reference space.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.PlatformError"/> if the runtime call
        ///   failed. See <see cref="OpenXRResultStatus.nativeStatusCode"/> for the runtime's `XrResult`.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// If a stationary reference space already exists, it's destroyed first.
        /// </remarks>
        public OpenXRResultStatus InitializeStationaryReferenceSpace(Pose poseOffset)
        {
            return NativeApi.xr_stationary_reference_space_TryInitializeStationaryReferenceSpace(ref poseOffset);
        }

        /// <summary>
        /// Destroys the stationary reference space, if one is currently valid.
        /// </summary>
        public void DestroyStationaryReferenceSpace()
        {
            NativeApi.xr_stationary_reference_space_DestroyStationaryReferenceSpace();
        }

        /// <summary>
        /// Checks whether the stationary reference space is currently valid.
        /// </summary>
        /// <returns>True if and only if the stationary reference space is valid, false otherwise.</returns>
        public bool IsStationarySpaceValid()
        {
            return NativeApi.xr_stationary_reference_space_IsStationaryReferenceSpaceValid();
        }

        /// <summary>
        /// Locates the current application input tracking space in the stationary reference space.
        /// </summary>
        /// <param name="appSpaceInStationarySpace">The pose of application space relative to stationary space.</param>
        /// <param name="locationFlags">Which parts of the pose are valid and tracked. <see cref="XrSpaceLocationFlags.None"/>
        /// if the call failed.</param>
        /// <returns>The result of the operation. Implicitly converts to `true` if the runtime call succeeded, even when
        /// the space isn't located. Check <paramref name="locationFlags"/> before using the pose.\
        /// \
        /// `statusCode` values:
        /// <list type="bullet">
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.UnqualifiedSuccess"/> if the runtime call
        ///   succeeded.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ProviderUninitialized"/> if the session
        ///   has not yet started.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ValidationFailure"/> if the stationary
        ///   reference space hasn't been created, or the other space handle is null.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.PlatformError"/> if the runtime call
        ///   failed. See <see cref="OpenXRResultStatus.nativeStatusCode"/> for the runtime's `XrResult`.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// While the runtime relocates the stationary reference space, the call can succeed with some location flags
        /// cleared. The position or orientation is identity while its valid flag is cleared.
        /// </remarks>
        public OpenXRResultStatus LocateAppSpaceInStationarySpace(out Pose appSpaceInStationarySpace, out XrSpaceLocationFlags locationFlags)
        {
            return LocateTargetSpaceInStationarySpace(GetCurrentAppSpace(), out appSpaceInStationarySpace, out locationFlags);
        }

        /// <summary>
        /// Locates the stationary reference space in the current application input tracking space.
        /// </summary>
        /// <param name="stationarySpaceInAppSpace">The pose of stationary space relative to application space.</param>
        /// <param name="locationFlags">Which parts of the pose are valid and tracked. <see cref="XrSpaceLocationFlags.None"/>
        /// if the call failed.</param>
        /// <returns>The result of the operation. Implicitly converts to `true` if the runtime call succeeded, even when
        /// the space isn't located. Check <paramref name="locationFlags"/> before using the pose.\
        /// \
        /// `statusCode` values:
        /// <list type="bullet">
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.UnqualifiedSuccess"/> if the runtime call
        ///   succeeded.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ProviderUninitialized"/> if the session
        ///   has not yet started.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ValidationFailure"/> if the stationary
        ///   reference space hasn't been created, or the other space handle is null.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.PlatformError"/> if the runtime call
        ///   failed. See <see cref="OpenXRResultStatus.nativeStatusCode"/> for the runtime's `XrResult`.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// While the runtime relocates the stationary reference space, the call can succeed with some location flags
        /// cleared. The position or orientation is identity while its valid flag is cleared.
        /// </remarks>
        public OpenXRResultStatus LocateStationarySpaceInAppSpace(out Pose stationarySpaceInAppSpace, out XrSpaceLocationFlags locationFlags)
        {
            return LocateStationarySpaceInBaseSpace(GetCurrentAppSpace(), out stationarySpaceInAppSpace, out locationFlags);
        }

        /// <summary>
        /// Attempts to locate a target space in the stationary reference space.
        /// </summary>
        /// <param name="targetSpace">The XrSpace handle of the space to locate in Stationary Space.</param>
        /// <param name="targetSpaceInStationarySpace">The pose of the target space relative to stationary space.</param>
        /// <param name="locationFlags">Which parts of the pose are valid and tracked. <see cref="XrSpaceLocationFlags.None"/>
        /// if the call failed.</param>
        /// <returns>The result of the operation. Implicitly converts to `true` if the runtime call succeeded, even when
        /// the space isn't located. Check <paramref name="locationFlags"/> before using the pose.\
        /// \
        /// `statusCode` values:
        /// <list type="bullet">
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.UnqualifiedSuccess"/> if the runtime call
        ///   succeeded.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ProviderUninitialized"/> if the session
        ///   has not yet started.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ValidationFailure"/> if the stationary
        ///   reference space hasn't been created, or the other space handle is null.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.PlatformError"/> if the runtime call
        ///   failed. See <see cref="OpenXRResultStatus.nativeStatusCode"/> for the runtime's `XrResult`.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// While the runtime relocates the stationary reference space, the call can succeed with some location flags
        /// cleared. The position or orientation is identity while its valid flag is cleared.
        /// </remarks>
        public OpenXRResultStatus LocateTargetSpaceInStationarySpace(ulong targetSpace, out Pose targetSpaceInStationarySpace, out XrSpaceLocationFlags locationFlags)
        {
            return NativeApi.LocateUsingStationarySpace(false, targetSpace, out targetSpaceInStationarySpace, out locationFlags);
        }

        /// <summary>
        /// Attempts to locate stationary space relative to the given base space.
        /// </summary>
        /// <param name="baseSpace">The XrSpace handle to locate stationary space relative to, as a ulong.</param>
        /// <param name="stationarySpaceInBaseSpace">The pose of stationary space relative to the given base space.</param>
        /// <param name="locationFlags">Which parts of the pose are valid and tracked. <see cref="XrSpaceLocationFlags.None"/>
        /// if the call failed.</param>
        /// <returns>The result of the operation. Implicitly converts to `true` if the runtime call succeeded, even when
        /// the space isn't located. Check <paramref name="locationFlags"/> before using the pose.\
        /// \
        /// `statusCode` values:
        /// <list type="bullet">
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.UnqualifiedSuccess"/> if the runtime call
        ///   succeeded.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ProviderUninitialized"/> if the session
        ///   has not yet started.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ValidationFailure"/> if the stationary
        ///   reference space hasn't been created, or the other space handle is null.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.PlatformError"/> if the runtime call
        ///   failed. See <see cref="OpenXRResultStatus.nativeStatusCode"/> for the runtime's `XrResult`.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// While the runtime relocates the stationary reference space, the call can succeed with some location flags
        /// cleared. The position or orientation is identity while its valid flag is cleared.
        /// </remarks>
        public OpenXRResultStatus LocateStationarySpaceInBaseSpace(ulong baseSpace, out Pose stationarySpaceInBaseSpace, out XrSpaceLocationFlags locationFlags)
        {
            return NativeApi.LocateUsingStationarySpace(true, baseSpace, out stationarySpaceInBaseSpace, out locationFlags);
        }

        /// <summary>
        /// Attempts to get the generation ID of the current stationary reference space. The XR runtime
        /// assigns a new generation ID when it can't relocate the previous stationary reference space origin and
        /// starts a new one. Recentering doesn't change the generation ID.
        /// </summary>
        /// <param name="generationId">The generation ID of the stationary reference space if successful.
        /// <see cref="XrUuid.empty"/> otherwise.</param>
        /// <returns>The result of the operation. Implicitly converts to `true` if and only if the generation ID was
        /// retrieved successfully.\
        /// \
        /// `statusCode` values:
        /// <list type="bullet">
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.UnqualifiedSuccess"/> if the generation ID
        ///   was retrieved successfully.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.Unsupported"/> if the runtime does not
        ///   support querying the generation ID.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.ProviderUninitialized"/> if the session
        ///   has not yet started.</description></item>
        ///   <item><description><see cref="OpenXRResultStatus.StatusCode.PlatformError"/> if the runtime call
        ///   failed. See <see cref="OpenXRResultStatus.nativeStatusCode"/> for the runtime's `XrResult`.</description></item>
        /// </list>
        /// </returns>
        public OpenXRResultStatus GetStationarySpaceGenerationId(out XrUuid generationId)
        {
            return NativeApi.GetStationarySpaceGenerationId(out generationId);
        }

        /// <summary>
        /// Args class for the stationary space changed event.
        /// </summary>
        public struct StationarySpaceChangedEventArgs
        {
            /// <summary>
            /// The <see cref="XrStationaryReferenceSpaceFeature"/> that sent this event.
            /// </summary>
            public XrStationaryReferenceSpaceFeature featureSender { get; private set; }

            bool m_PoseValid;

            /// <summary>
            /// Attempts to retrieve the pose of the new stationary space origin relative to the old stationary space origin.
            /// </summary>
            /// <param name="poseInPreviousSpace">The pose of the new stationary space relative to the old origin if successful. Identity otherwise.</param>
            /// <returns>True if and only if the XR runtime can determine the pose of the new origin relative to the old one.</returns>
            public bool TryGetPoseInPreviousSpace(out Pose poseInPreviousSpace)
            {
                if (m_PoseValid)
                {
                    poseInPreviousSpace = m_PoseInPreviousSpace;
                    return true;
                }
                else
                {
                    poseInPreviousSpace = Pose.identity;
                    return false;
                }
            }

            readonly Pose m_PoseInPreviousSpace;

            internal StationarySpaceChangedEventArgs(XrStationaryReferenceSpaceFeature sender, bool poseValid, Pose poseInPreviousSpace)
            {
                featureSender = sender;
                m_PoseValid = poseValid;
                m_PoseInPreviousSpace = poseInPreviousSpace;
            }
        }

        /// <summary>
        /// This Action is invoked when the underlying OpenXR runtime sends a space change event for stationary space. This
        /// is invoked on the first frame when the associated event's XrTime has passed. When stationary space and the app space
        /// are changed on the same frame, stationary space's event will be invoked before
        /// <see cref="XRInputSubsystem.trackingOriginUpdated"/>.
        /// </summary>
        public event Action<StationarySpaceChangedEventArgs> stationarySpaceChanged;

        internal static class NativeApi
        {
            internal static OpenXRResultStatus LocateUsingStationarySpace(
                bool isSpaceHandleBaseSpace,
                ulong xrSpaceHandle,
                out Pose poseResult,
                out XrSpaceLocationFlags locationFlags)
            {
                poseResult = Pose.identity;
                locationFlags = XrSpaceLocationFlags.None;
                return xr_stationary_reference_space_TryLocateUsingStationarySpace(
                    isSpaceHandleBaseSpace, xrSpaceHandle, ref poseResult, ref locationFlags);
            }

            [DllImport("UnityOpenXR")]
            internal static extern OpenXRResultStatus xr_stationary_reference_space_TryInitializeStationaryReferenceSpace(ref Pose poseOffset);

            [DllImport("UnityOpenXR")]
            internal static extern void xr_stationary_reference_space_DestroyStationaryReferenceSpace();

            [DllImport("UnityOpenXR")]
            [return: MarshalAs(UnmanagedType.U1)]
            internal static extern bool xr_stationary_reference_space_IsStationaryReferenceSpaceValid();

            [DllImport("UnityOpenXR")]
            static extern OpenXRResultStatus xr_stationary_reference_space_TryLocateUsingStationarySpace(
                [MarshalAs(UnmanagedType.U1)] bool isSpaceHandleBaseSpace,
                ulong xrSpaceHandle,
                ref Pose poseResult,
                ref XrSpaceLocationFlags locationFlags);

            internal static OpenXRResultStatus GetStationarySpaceGenerationId(out XrUuid generationId)
            {
                generationId = XrUuid.empty;
                return xr_stationary_reference_space_TryGetGenerationId(ref generationId);
            }

            [DllImport("UnityOpenXR")]
            static extern OpenXRResultStatus xr_stationary_reference_space_TryGetGenerationId(ref XrUuid generationId);
        }
    }
}
