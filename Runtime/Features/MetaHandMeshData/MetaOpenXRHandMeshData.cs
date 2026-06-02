#if XR_HANDS_1_9_OR_NEWER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.SubsystemsImplementation.Extensions;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Meshing;
using UnityEngine.XR.Hands.Meshing.ProviderImplementation;
using UnityEngine.XR.Hands.OpenXR;
using UnityEngine.XR.Hands.OpenXR.Meshing;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.Meta
{
    /// <summary>
    /// Enables access to Meta hand mesh data.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Meta Quest: Hand Mesh Data",
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        Company = "Unity",
        Desc = "Access to Meta hand mesh data via XR_FB_hand_tracking_mesh.",
        DocumentationLink = Constants.k_DocumentationManualURL + "features/hand-mesh-data.html",
        OpenxrExtensionStrings = k_OpenXRRequestedExtensions,
        Category = FeatureCategory.Feature,
        FeatureId = featureId,
        Version = "0.1.0")]
#endif
    public class MetaOpenXRHandMeshData : OpenXRFeature, IOpenXRHandMeshDataSupplier
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.metahandmeshdata";

        /// <summary>
        /// Attempts to retrieve hand mesh data for both hands from the OpenXR runtime.
        /// </summary>
        /// <param name="result">
        /// The query result to populate with mesh data for each hand.
        /// </param>
        /// <param name="queryParams">
        /// Parameters controlling the query, including the allocator to use for native arrays.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if mesh data was successfully retrieved for at least
        /// one hand, <see langword="false"/> otherwise.
        /// </returns>
        public unsafe bool TryGetMeshData(ref XRHandMeshDataQueryResult result, ref XRHandMeshDataQueryParams queryParams)
        {
            bool leftSuccess = TryGetMeshData(ref result, queryParams.allocator, Handedness.Left);
            bool rightSuccess = TryGetMeshData(ref result, queryParams.allocator, Handedness.Right);

            return leftSuccess || rightSuccess;
        }

        unsafe bool TryGetMeshData(ref XRHandMeshDataQueryResult result, Allocator allocator, Handedness handedness)
        {
            ulong xrHandTracker = handedness.GetOpenXRHandTrackerHandle();
            if (NativeApi.TryGetHandMeshDataCounts(
                out int jointCountCondensed,
                out int indexCount,
                out int vertexCount,
                xrHandTracker) == 0 ||
                jointCountCondensed == 0 ||
                indexCount == 0 ||
                vertexCount == 0)
                return false;

            NativeArray<XRHandJointID> jointIDsCondensed = default;
            NativeArray<Pose> jointBindPosesCondensed = default;
            NativeArray<float> jointRadiiCondensed = default;

            NativeArray<int> vertexIndices = default;
            NativeArray<Vector3> vertexPositions = default;
            NativeArray<Vector3> vertexNormals = default;
            NativeArray<Vector2> vertexUVs = default;

            NativeArray<int> vertexBlendIndices = default;
            NativeArray<float> vertexBlendWeights = default;

            if (jointCountCondensed > 0)
            {
                jointIDsCondensed = new NativeArray<XRHandJointID>(jointCountCondensed, Allocator.Temp);
                jointBindPosesCondensed = new NativeArray<Pose>(jointCountCondensed, Allocator.Temp);
                jointRadiiCondensed = new NativeArray<float>(jointCountCondensed, Allocator.Temp);
            }

            if (indexCount > 0)
                vertexIndices = new NativeArray<int>(indexCount, allocator);

            if (vertexCount > 0)
            {
                vertexPositions = new NativeArray<Vector3>(vertexCount, allocator);
                vertexNormals = new NativeArray<Vector3>(vertexCount, allocator);
                vertexUVs = new NativeArray<Vector2>(vertexCount, allocator);

                vertexBlendIndices = new NativeArray<int>(vertexCount * k_MaxBonesPerVertex, Allocator.Temp);
                vertexBlendWeights = new NativeArray<float>(vertexCount * k_MaxBonesPerVertex, Allocator.Temp);
            }

            if (NativeApi.TryGetHandMeshData(
                xrHandTracker,
                jointCountCondensed,
                jointIDsCondensed.GetUnsafePtr(),
                jointBindPosesCondensed.GetUnsafePtr(),
                jointRadiiCondensed.GetUnsafePtr(),
                indexCount,
                vertexIndices.GetUnsafePtr(),
                vertexCount,
                vertexPositions.GetUnsafePtr(),
                vertexNormals.GetUnsafePtr(),
                vertexUVs.GetUnsafePtr(),
                vertexBlendIndices.GetUnsafePtr(),
                vertexBlendWeights.GetUnsafePtr()) == 0)
            {
                if (jointIDsCondensed.IsCreated)
                    jointIDsCondensed.Dispose();

                if (jointBindPosesCondensed.IsCreated)
                    jointBindPosesCondensed.Dispose();

                if (jointRadiiCondensed.IsCreated)
                    jointRadiiCondensed.Dispose();

                if (vertexIndices.IsCreated)
                    vertexIndices.Dispose();

                if (vertexPositions.IsCreated)
                    vertexPositions.Dispose();

                if (vertexNormals.IsCreated)
                    vertexNormals.Dispose();

                if (vertexUVs.IsCreated)
                    vertexUVs.Dispose();

                if (vertexBlendIndices.IsCreated)
                    vertexBlendIndices.Dispose();

                if (vertexBlendWeights.IsCreated)
                    vertexBlendWeights.Dispose();

                return false;
            }

            var hand = handedness == Handedness.Left ? result.leftHand : result.rightHand;

            hand.SetIndices(vertexIndices);
            hand.SetPositions(vertexPositions);
            hand.SetNormals(vertexNormals);
            hand.SetUVs(vertexUVs);

            if (jointCountCondensed > 0)
            {
                // The condensed joints from XR_FB_hand_tracking_mesh are in XrHandJointEXT
                // enum order. The C++ layer passes jointParents (parent joint IDs) as jointIDs,
                // which is not the joint's own identity. We derive the correct Unity joint index
                // from the condensed index directly using the known XrHandJointEXT-to-Unity mapping.
                int finalJointArrayLengths = XRHandJointID.EndMarker.ToIndex();
                var matchingBindPoseValidity = new NativeArray<bool>(finalJointArrayLengths, allocator);
                var jointBindPoses = new NativeArray<Matrix4x4>(finalJointArrayLengths, allocator);
                var matchingRadiusValidity = new NativeArray<bool>(finalJointArrayLengths, allocator);
                var jointRadii = new NativeArray<float>(finalJointArrayLengths, allocator);

                for (int jointIndexCondensed = 0; jointIndexCondensed < jointCountCondensed; ++jointIndexCondensed)
                {
                    var jointIndex = OpenXRCondensedIndexToUnityJointIndex(jointIndexCondensed);
                    if (jointIndex < 0 || jointIndex >= finalJointArrayLengths)
                        continue;

                    var mtx = new Matrix4x4();
                    mtx.SetTRS(jointBindPosesCondensed[jointIndexCondensed].position, jointBindPosesCondensed[jointIndexCondensed].rotation, Vector3.one);
                    jointBindPoses[jointIndex] = mtx;
                    jointRadii[jointIndex] = jointRadiiCondensed[jointIndexCondensed];
                    matchingBindPoseValidity[jointIndex] = true;
                    matchingRadiusValidity[jointIndex] = true;
                }

                hand.SetMatchingJointBindPoseValidity(matchingBindPoseValidity);
                hand.SetJointBindPoses(jointBindPoses);
                hand.SetMatchingJointRadiusValidity(matchingRadiusValidity);
                hand.SetJointRadii(jointRadii);

                jointIDsCondensed.Dispose();
                jointBindPosesCondensed.Dispose();
                jointRadiiCondensed.Dispose();
            }

            if (vertexCount > 0)
            {
                var bonesPerVertex = new NativeArray<byte>(vertexCount, allocator);
                for (int bonesPerVertexIndex = 0; bonesPerVertexIndex < bonesPerVertex.Length; ++bonesPerVertexIndex)
                    bonesPerVertex[bonesPerVertexIndex] = k_MaxBonesPerVertex;

                var boneWeights = new NativeArray<BoneWeight1>(vertexCount * k_MaxBonesPerVertex, allocator);
                for (int boneWeightIndex = 0; boneWeightIndex < boneWeights.Length; ++boneWeightIndex)
                {
                    var boneWeight = new BoneWeight1();
                    // Remap from condensed (OpenXR) bone index to full Unity joint-index space
                    boneWeight.boneIndex = OpenXRCondensedIndexToUnityJointIndex(vertexBlendIndices[boneWeightIndex]);
                    boneWeight.weight = vertexBlendWeights[boneWeightIndex];
                    boneWeights[boneWeightIndex] = boneWeight;
                }

                vertexBlendIndices.Dispose();
                vertexBlendWeights.Dispose();

                hand.SetBonesPerVertex(bonesPerVertex);
                hand.SetBoneWeights(boneWeights);
            }

            result.FlushChanges(hand);
            return true;
        }

        /// <summary>
        /// The set of OpenXR spec extension strings to enable, separated by spaces.
        /// </summary>
        const string k_OpenXRRequestedExtensions = "XR_FB_hand_tracking_mesh";

        const int k_MaxBonesPerVertex = 4;

        const string k_HandTrackingPermission = "com.oculus.permission.HAND_TRACKING";

        /// <summary>
        /// Initializes the native resources required to provide hand mesh data.
        /// </summary>
        protected override void OnSubsystemStart()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(k_HandTrackingPermission))
            {
                Debug.LogWarning($"Hand mesh data requires system permission {k_HandTrackingPermission}, but permission was not granted. Requesting permission.");
                Permission.RequestUserPermission(k_HandTrackingPermission);
            }
#endif

            SubsystemManager.GetSubsystems(s_SubsystemsReuse);
            foreach (var subsystem in s_SubsystemsReuse)
            {
                var provider = subsystem.GetProvider() as OpenXRHandProvider;
                if (provider == null)
                    continue;

                provider.handMeshDataSupplier = this;
                s_SubsystemsReuse.Clear();
                return;
            }

            s_SubsystemsReuse.Clear();
            Debug.LogWarning("Hand Tracking Subsystem feature is not enabled - subsystem APIs for hand mesh data will fail.");
        }

        static List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

#if UNITY_EDITOR
        /// <summary>
        /// Gets validation rules for the MetaOpenXRHandMeshData feature. Called by the
        /// OpenXR project validation system when this feature is enabled in the Editor.
        /// </summary>
        /// <param name="rules">List to add validation rules to.</param>
        /// <param name="targetGroup">The build target group being validated.</param>
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            var moreRules = new ValidationRule[]
            {
                new ValidationRule(this)
                {
                    message = "Meta OpenXR Hand Mesh Data requires that the Hand Tracking Subsystem feature be enabled.",
                    checkPredicate = () =>
                    {
                        var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                        return settings != null && (settings.GetFeature<HandTracking>()?.enabled ?? false);
                    },
                    fixItAutomatic = true,
                    fixItMessage = "Enable the Hand Tracking Subsystem feature.",
                    fixIt = () =>
                    {
                        var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                        var feature = settings?.GetFeature<HandTracking>();
                        if (feature != null)
                            feature.enabled = true;
                    },
                    error = false,
                }
            };

            rules.AddRange(moreRules);
        }
#endif

        /// <summary>
        /// Maps from OpenXR XrHandJointEXT condensed index to Unity XRHandJointID.ToIndex().
        /// OpenXR has Palm=0, Wrist=1. Unity has Wrist=0, Palm=1. All other joints match.
        /// </summary>
        static int OpenXRCondensedIndexToUnityJointIndex(int condensedIndex)
        {
            if (condensedIndex == 0) return 1; // OpenXR Palm -> Unity Palm index
            if (condensedIndex == 1) return 0; // OpenXR Wrist -> Unity Wrist index
            return condensedIndex;
        }

        const string k_NativeLibraryName = "UnityOpenXR";

        static class NativeApi
        {
            [DllImport(k_NativeLibraryName, EntryPoint = "UnityOpenXRHands_TryGetHandMeshDataCounts")]
            public static unsafe extern int TryGetHandMeshDataCounts(
                out int jointCount,
                out int indexCount,
                out int vertexCount,
                ulong xrHandTracker);

            [DllImport(k_NativeLibraryName, EntryPoint = "UnityOpenXRHands_TryGetHandMeshData")]
            public static unsafe extern int TryGetHandMeshData(
                ulong xrHandTracker,
                int jointCount,
                void* jointIDs,
                void* jointBindPoses,
                void* jointRadii,
                int indexCount,
                void* indices,
                int vertexCount,
                void* positions,
                void* normals,
                void* uvs,
                void* blendIndices,
                void* blendWeights);

            [DllImport(k_NativeLibraryName, EntryPoint = "UnityOpenXRHands_GetHandMeshXrResult")]
            public static extern long GetXrResult();
        }
    }
}
#endif // XR_HANDS_1_9_OR_NEWER
