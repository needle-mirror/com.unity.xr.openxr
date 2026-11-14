using UnityEngine.UI;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Samples.TrackingOriginSample
{
    /// <summary>
    /// Places a pair of markers at the same spot: one in the stationary reference space and one relative to the
    /// XR Origin. When the tracking origin moves, for example after the user recenters, the XR Origin marker moves
    /// with it while the stationary marker stays fixed in the physical world.
    /// </summary>
    public class StationarySpaceMarkers : MonoBehaviour
    {
        const string k_StaleLabel = "Placed in an earlier\nstationary space";

        // Distance between the markers, in meters, beyond which the tracking origin has visibly moved.
        const float k_SeparationThreshold = 0.05f;

        [SerializeField]
        StationarySpaceController m_Controller;

        [Header("UI")]
        [SerializeField]
        Button m_PlaceButton;

        [SerializeField]
        Button m_ClearButton;

        [SerializeField]
        [Tooltip("Displays the next step of the demonstration.")]
        Text m_HintText;

        [Header("Markers")]
        [SerializeField]
        [Tooltip("Marker placed in the stationary reference space.")]
        GameObject m_StationaryMarkerPrefab;

        [SerializeField]
        [Tooltip("Marker placed relative to the XR Origin, at the same spot as the stationary marker.")]
        GameObject m_XrOriginMarkerPrefab;

        [SerializeField]
        [Tooltip("Applied to the stationary marker when it belongs to an earlier generation of the stationary space.")]
        Material m_StaleMaterial;

        [SerializeField]
        [Tooltip("Distance in front of the user to place the markers, in meters.")]
        float m_PlacementDistance = 1f;

        GameObject m_StationaryMarker;
        GameObject m_XrOriginMarker;
        XrUuid m_PlacementGenerationId = XrUuid.empty;
        bool m_MarkersHaveSeparated;
        bool m_SpaceHasRelocated;

        void Start()
        {
            m_PlaceButton.onClick.AddListener(PlaceMarkers);
            m_ClearButton.onClick.AddListener(ClearMarkers);
            m_Controller.generationIdChanged += OnGenerationIdChanged;
            UpdateUi();
        }

        void Update()
        {
            TrackDemonstrationProgress();
            UpdateUi();
        }

        /// <summary>
        /// Places both markers in front of the user, facing them.
        /// </summary>
        public void PlaceMarkers()
        {
            if (m_Controller.state != StationarySpaceController.SpaceState.Located)
            {
                return;
            }

            ClearMarkers();

            var xrOrigin = m_Controller.xrOrigin;
            var head = xrOrigin.Camera.transform;

            // Keep the markers upright, at head height, in the direction the user is facing.
            var forward = Vector3.ProjectOnPlane(head.forward, Vector3.up);
            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.ProjectOnPlane(head.up, Vector3.up);
            }

            forward.Normalize();
            var position = head.position + forward * m_PlacementDistance;
            var rotation = Quaternion.LookRotation(-forward, Vector3.up);

            // The stationary marker keeps its pose relative to the stationary space origin. The origin itself can be
            // anywhere, so the marker is placed relative to the user and then parented to the origin.
            m_StationaryMarker = Instantiate(m_StationaryMarkerPrefab, position, rotation, m_Controller.spaceRoot);
            m_StationaryMarker.name = "Stationary Marker";

            // The XR Origin marker behaves like the rest of the scene content, such as the floor and the wall.
            m_XrOriginMarker = Instantiate(m_XrOriginMarkerPrefab, position, rotation, xrOrigin.Origin.transform);
            m_XrOriginMarker.name = "XR Origin Marker";

            m_PlacementGenerationId = m_Controller.generationId;
        }

        /// <summary>
        /// Removes both markers.
        /// </summary>
        public void ClearMarkers()
        {
            if (m_StationaryMarker != null)
            {
                Destroy(m_StationaryMarker);
            }

            if (m_XrOriginMarker != null)
            {
                Destroy(m_XrOriginMarker);
            }

            m_StationaryMarker = null;
            m_XrOriginMarker = null;
            m_PlacementGenerationId = XrUuid.empty;
            m_MarkersHaveSeparated = false;
            m_SpaceHasRelocated = false;
        }

        /// <summary>
        /// A new generation means the runtime could not relocate the previous stationary space origin. The new origin
        /// is unrelated to the old one, so a marker placed earlier no longer marks the spot where it was placed.
        /// </summary>
        void OnGenerationIdChanged(XrUuid generationId)
        {
            if (m_StationaryMarker == null)
            {
                return;
            }

            if (m_PlacementGenerationId.Equals(XrUuid.empty))
            {
                // Placed before the generation ID was known.
                m_PlacementGenerationId = generationId;
                return;
            }

            if (!m_PlacementGenerationId.Equals(generationId))
            {
                MarkStationaryMarkerStale();
            }
        }

        /// <summary>
        /// Generation IDs never return to an earlier value, so a stale marker stays stale until it's placed again.
        /// </summary>
        void MarkStationaryMarkerStale()
        {
            foreach (var markerRenderer in m_StationaryMarker.GetComponentsInChildren<Renderer>(true))
            {
                markerRenderer.sharedMaterial = m_StaleMaterial;
            }

            var label = m_StationaryMarker.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.text = k_StaleLabel;
            }
        }

        void UpdateUi()
        {
            var state = m_Controller.state;
            var isLocated = state == StationarySpaceController.SpaceState.Located;
            var hasMarkers = m_XrOriginMarker != null;

            m_PlaceButton.interactable = isLocated;
            m_ClearButton.interactable = hasMarkers;

            var hint = GetHint(state, isLocated, hasMarkers);

            // Only assign when the hint changes, to avoid rebuilding the canvas every frame.
            if (m_HintText.text != hint)
            {
                m_HintText.text = hint;
            }
        }

        /// <summary>
        /// Suggests one next step at a time, so the panel guides the user through the demonstration without listing
        /// every step up front.
        /// </summary>
        string GetHint(StationarySpaceController.SpaceState state, bool isLocated, bool hasMarkers)
        {
            switch (state)
            {
                case StationarySpaceController.SpaceState.Starting:
                    return "Waiting for XR to start.";
                case StationarySpaceController.SpaceState.Unavailable:
                    return m_Controller.unavailableReason;
                case StationarySpaceController.SpaceState.CreationFailed:
                    return "The runtime couldn't create the stationary space.";
            }

            if (!hasMarkers)
            {
                return isLocated
                    ? "Select Place markers to put two markers in front of you."
                    : "Waiting for the stationary space.";
            }

            if (!isLocated)
            {
                return "The solid cube is hidden while the runtime relocates the stationary space.";
            }

            if (!m_PlacementGenerationId.Equals(m_Controller.generationId))
            {
                return "The stationary space started a new generation, so the solid cube no longer marks where you " +
                    "placed it. Place the markers again.";
            }

            if (m_SpaceHasRelocated)
            {
                return "The runtime relocated the stationary space, and the solid cube is back where you placed it.";
            }

            return m_MarkersHaveSeparated
                ? "The solid cube stayed fixed in your physical room. The wireframe box moved with the XR Origin, " +
                    "along with the rest of the scene. Next, take the headset off and put it back on."
                : "Now recenter, or move the Height slider in Device mode. Watch which marker stays fixed in your " +
                    "physical room.";
        }

        /// <summary>
        /// Tracks the steps the user has tried, so the hint can move on to the next one.
        /// </summary>
        void TrackDemonstrationProgress()
        {
            if (m_StationaryMarker == null || m_XrOriginMarker == null)
            {
                return;
            }

            // The relocation hint is only shown once the space is located again.
            if (m_Controller.state == StationarySpaceController.SpaceState.Relocating)
            {
                m_SpaceHasRelocated = true;
                return;
            }

            if (m_Controller.state != StationarySpaceController.SpaceState.Located)
            {
                return;
            }

            // The runtime can adjust the stationary space origin slightly, so ignore small differences.
            var separation = Vector3.Distance(m_StationaryMarker.transform.position, m_XrOriginMarker.transform.position);
            if (separation > k_SeparationThreshold)
            {
                m_MarkersHaveSeparated = true;
            }
        }

        void OnDestroy()
        {
            if (m_Controller != null)
            {
                m_Controller.generationIdChanged -= OnGenerationIdChanged;
            }

            // The markers live under the XR Origin, which can persist across scenes.
            ClearMarkers();
        }
    }
}
