using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Features.Extensions.StationaryReferenceSpace;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Samples.TrackingOriginSample
{
    /// <summary>
    /// Creates the stationary reference space when the scene starts, keeps a GameObject at its origin up to date, and
    /// destroys the space when the scene unloads.
    /// </summary>
    /// <remarks>
    /// The stationary reference space is not a tracking origin mode. The application creates it alongside whichever
    /// tracking origin mode the <see cref="XROrigin"/> uses, and locates it relative to the application's tracking
    /// space. Content that should stay fixed in the physical world is placed under <see cref="spaceRoot"/>.
    /// </remarks>
    public class StationarySpaceController : MonoBehaviour
    {
        /// <summary>
        /// The state of the stationary reference space.
        /// </summary>
        public enum SpaceState
        {
            /// <summary>Waiting for XR to start.</summary>
            Starting,

            /// <summary>XR, the feature, or the extension is not available.</summary>
            Unavailable,

            /// <summary>The space is being created.</summary>
            Creating,

            /// <summary>The runtime could not create the space.</summary>
            CreationFailed,

            /// <summary>The space exists but the runtime cannot locate it, for example while it relocates after tracking loss.</summary>
            Relocating,

            /// <summary>The space exists and is located this frame.</summary>
            Located,
        }

        // How long to wait for XR to start, and for the runtime to create the space once XR is running, in seconds.
        const float k_TimeoutSeconds = 10f;

        // A located pose is only usable when both its position and its orientation are valid.
        const XrSpaceLocationFlags k_PoseValidFlags = XrSpaceLocationFlags.PositionValid | XrSpaceLocationFlags.OrientationValid;

        [SerializeField]
        [Tooltip("Displays a short state and the start of the generation ID of the stationary reference space.")]
        Text m_StatusText;

        readonly List<XRInputSubsystem> m_InputSubsystems = new();

        XROrigin m_XrOrigin;
        XrStationaryReferenceSpaceFeature m_Feature;
        GameObject m_SpaceRoot;
        string m_UnavailableReason = string.Empty;
        bool m_HasReadGenerationId;

        /// <summary>
        /// The current state of the stationary reference space.
        /// </summary>
        public SpaceState state { get; private set; } = SpaceState.Starting;

        /// <summary>
        /// The generation ID of the stationary reference space, or <see cref="XrUuid.empty"/> if it isn't known yet.
        /// </summary>
        /// <remarks>
        /// After the space is created again, for example after OpenXR restarts, this keeps the previous value until
        /// the runtime reports the current one.
        /// </remarks>
        public XrUuid generationId { get; private set; } = XrUuid.empty;

        /// <summary>
        /// A transform at the stationary reference space origin. It is only active while the space is located.
        /// </summary>
        /// <remarks>
        /// The origin has no guaranteed height or forward direction, and it can be far from the user. Place content
        /// at a pose relative to this transform rather than at the transform itself.
        /// </remarks>
        public Transform spaceRoot => m_SpaceRoot != null ? m_SpaceRoot.transform : null;

        /// <summary>
        /// The XR Origin found in the scene, or <see langword="null"/> before XR is running.
        /// </summary>
        public XROrigin xrOrigin => m_XrOrigin;

        /// <summary>
        /// Why the stationary reference space is <see cref="SpaceState.Unavailable"/>.
        /// </summary>
        public string unavailableReason => m_UnavailableReason;

        /// <summary>
        /// Invoked when the generation ID becomes known or changes. A change means the runtime could not relocate the
        /// previous stationary space origin, so content placed relative to it is no longer valid.
        /// </summary>
        public event Action<XrUuid> generationIdChanged;

        IEnumerator Start()
        {
            UpdateStatusText();

            // The XR Origin can come from a rig that persists across scenes, so it's found at runtime instead of
            // being referenced from the scene.
            var timeout = Time.realtimeSinceStartup + k_TimeoutSeconds;
            while (!TryFindDependencies())
            {
                if (Time.realtimeSinceStartup >= timeout)
                {
                    SetUnavailable("XR is not running.");
                    yield break;
                }

                yield return null;
            }

            if (m_Feature == null || !m_Feature.enabled ||
                !OpenXRRuntime.IsExtensionEnabled(XrStationaryReferenceSpaceFeature.extensionString))
            {
                SetUnavailable("Enable the Stationary Reference Space feature in the OpenXR settings, and use " +
                    $"a runtime that supports {XrStationaryReferenceSpaceFeature.extensionString}.");
                yield break;
            }

            // Poses from the feature are relative to the application's tracking space. Tracked devices are placed
            // relative to the camera floor offset object, so the stationary space root must be its child to line up
            // with them in every tracking origin mode and at every camera height.
            m_SpaceRoot = new GameObject("Stationary Space Root");
            m_SpaceRoot.transform.SetParent(m_XrOrigin.CameraFloorOffsetObject.transform, false);
            m_SpaceRoot.SetActive(false);

            m_Feature.stationarySpaceChanged += OnStationarySpaceChanged;

            yield return CreateStationarySpace();
        }

        bool TryFindDependencies()
        {
            if (m_XrOrigin == null)
            {
                m_XrOrigin = FindAnyObjectByType<XROrigin>(FindObjectsInactive.Exclude);
            }

            if (m_Feature == null && OpenXRSettings.Instance != null)
            {
                m_Feature = OpenXRSettings.Instance.GetFeature<XrStationaryReferenceSpaceFeature>();
            }

            return m_XrOrigin != null && IsXrRunning();
        }

        bool IsXrRunning()
        {
            // A running input subsystem means XR has started.
            SubsystemManager.GetSubsystems(m_InputSubsystems);
            return m_InputSubsystems.Exists(subsystem => subsystem.running);
        }

        IEnumerator CreateStationarySpace()
        {
            SetState(SpaceState.Creating);

            // A new session can report a different generation ID, so read it again for the new space.
            m_HasReadGenerationId = false;

            // Until the OpenXR session is running, creation reports ProviderUninitialized, so retry while it does.
            // Only time out while XR is running, so that waiting for OpenXR to restart doesn't count.
            var timeout = Time.realtimeSinceStartup + k_TimeoutSeconds;
            var status = m_Feature.InitializeStationaryReferenceSpace();
            while (status.statusCode == OpenXRResultStatus.StatusCode.ProviderUninitialized)
            {
                if (!IsXrRunning())
                {
                    timeout = Time.realtimeSinceStartup + k_TimeoutSeconds;
                }
                else if (Time.realtimeSinceStartup >= timeout)
                {
                    SetState(SpaceState.CreationFailed);
                    yield break;
                }

                yield return null;
                status = m_Feature.InitializeStationaryReferenceSpace();
            }

            // Any other failure won't go away by retrying.
            if (status.statusCode == OpenXRResultStatus.StatusCode.Unsupported)
            {
                SetUnavailable("The OpenXR runtime doesn't offer the stationary reference space.");
                yield break;
            }

            if (!status.IsSuccess())
            {
                SetState(SpaceState.CreationFailed);
                yield break;
            }

            // Update locates the space from now on and moves to Located once the runtime reports a valid pose.
            SetState(SpaceState.Relocating);
        }

        void Update()
        {
            if (state is not (SpaceState.Relocating or SpaceState.Located))
            {
                return;
            }

            // OpenXR can restart without reloading the scene, for example after the session state becomes
            // XR_SESSION_STATE_LOSS_PENDING. Ending the session destroys the stationary space, so create it again.
            if (!m_Feature.IsStationarySpaceValid())
            {
                m_SpaceRoot.SetActive(false);
                StartCoroutine(CreateStationarySpace());
                return;
            }

            // The generation ID can only be read once the session is running. After that, the
            // stationarySpaceChanged event reports changes.
            if (!m_HasReadGenerationId && m_Feature.GetStationarySpaceGenerationId(out var newGenerationId).IsSuccess())
            {
                m_HasReadGenerationId = true;
                SetGenerationId(newGenerationId);
            }

            // Locate the space every frame. The runtime can make small adjustments to the stationary space origin
            // while the user walks around, without raising an event. A successful call doesn't mean the pose is
            // usable, so check the location flags too.
            var locateStatus = m_Feature.LocateStationarySpaceInAppSpace(out var stationarySpaceInAppSpace, out var locationFlags);
            if (locateStatus.IsSuccess() && (locationFlags & k_PoseValidFlags) == k_PoseValidFlags)
            {
                m_SpaceRoot.transform.SetLocalPositionAndRotation(
                    stationarySpaceInAppSpace.position, stationarySpaceInAppSpace.rotation);
                m_SpaceRoot.SetActive(true);
                SetState(SpaceState.Located);
                return;
            }

            // While the runtime relocates the space, for example after tracking loss or after the headset is put
            // back on, it clears the valid flags. Hide content instead of showing it at a stale pose.
            m_SpaceRoot.SetActive(false);
            SetState(SpaceState.Relocating);
        }

        /// <summary>
        /// The runtime raises this event only when it starts a new generation of the stationary space, which it does
        /// when it can't relocate the previous origin. Recentering never raises it.
        /// </summary>
        void OnStationarySpaceChanged(XrStationaryReferenceSpaceFeature.StationarySpaceChangedEventArgs _)
        {
            if (m_Feature.GetStationarySpaceGenerationId(out var newGenerationId).IsSuccess())
            {
                m_HasReadGenerationId = true;
                SetGenerationId(newGenerationId);
            }
        }

        void SetGenerationId(XrUuid newGenerationId)
        {
            if (newGenerationId.Equals(generationId))
            {
                return;
            }

            generationId = newGenerationId;
            UpdateStatusText();
            generationIdChanged?.Invoke(generationId);
        }

        void SetUnavailable(string reason)
        {
            m_UnavailableReason = reason;
            SetState(SpaceState.Unavailable);
        }

        void SetState(SpaceState newState)
        {
            if (newState == state)
            {
                return;
            }

            state = newState;
            UpdateStatusText();
        }

        void UpdateStatusText()
        {
            if (m_StatusText == null)
            {
                return;
            }

            var stateText = state switch
            {
                SpaceState.Starting => "Starting...",
                SpaceState.Unavailable => "Unavailable",
                SpaceState.Creating => "Creating...",
                SpaceState.CreationFailed => "Couldn't create",
                SpaceState.Relocating => "Relocating...",
                SpaceState.Located => "Active",
                _ => string.Empty,
            };

            // The full generation ID is 33 characters. The first eight are enough to notice when it changes.
            m_StatusText.text = generationId.Equals(XrUuid.empty)
                ? stateText
                : $"{stateText} · generation {generationId.ToString().Substring(0, 8)}";
        }

        void OnDestroy()
        {
            if (m_Feature != null)
            {
                m_Feature.stationarySpaceChanged -= OnStationarySpaceChanged;

                // Don't leave the space allocated after the sample scene unloads.
                if (m_Feature.IsStationarySpaceValid())
                {
                    m_Feature.DestroyStationaryReferenceSpace();
                }
            }

            // The root lives under the XR Origin, which can persist across scenes.
            if (m_SpaceRoot != null)
            {
                Destroy(m_SpaceRoot);
            }
        }
    }
}
