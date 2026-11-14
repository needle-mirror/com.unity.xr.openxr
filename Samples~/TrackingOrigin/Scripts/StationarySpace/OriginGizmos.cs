using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.TrackingOriginSample
{
    /// <summary>
    /// Shows axis gizmos at the XR Origin and at the stationary reference space origin.
    /// </summary>
    /// <remarks>
    /// The runtime chooses where the stationary reference space origin is. It has no guaranteed height or forward
    /// direction, and it can be far from the user, so its gizmo can appear anywhere. Content should be placed relative
    /// to the origin rather than at it, as the markers demonstrate.
    /// </remarks>
    public class OriginGizmos : MonoBehaviour
    {
        // Name of the sphere in the gizmo prefab that marks the origin.
        const string k_OriginSphereName = "Origin Sphere";

        [SerializeField]
        StationarySpaceController m_Controller;

        [SerializeField]
        [Tooltip("Gizmo instantiated at each origin. Its label and sphere material are set at runtime.")]
        GameObject m_GizmoPrefab;

        [SerializeField]
        [Tooltip("Shows or hides both gizmos.")]
        Toggle m_ShowOriginsToggle;

        [SerializeField]
        [Tooltip("Material for the sphere of the XR Origin gizmo. Matches the XR Origin marker.")]
        Material m_XrOriginMaterial;

        [SerializeField]
        [Tooltip("Material for the sphere of the stationary space origin gizmo. Matches the stationary marker.")]
        Material m_StationaryOriginMaterial;

        GameObject m_XrOriginGizmo;
        GameObject m_StationaryOriginGizmo;
        Transform m_XrOriginLabel;
        Transform m_StationaryOriginLabel;

        void Start()
        {
            m_ShowOriginsToggle.onValueChanged.AddListener(_ => UpdateVisibility());
        }

        void Update()
        {
            // The XR Origin and the stationary space root are created at runtime, so add the gizmos once they exist.
            if (m_XrOriginGizmo == null && m_Controller.xrOrigin != null)
            {
                m_XrOriginGizmo = CreateGizmo(m_Controller.xrOrigin.Origin.transform, "XR Origin", m_XrOriginMaterial, out m_XrOriginLabel);
                UpdateVisibility();
            }

            if (m_StationaryOriginGizmo == null && m_Controller.spaceRoot != null)
            {
                m_StationaryOriginGizmo = CreateGizmo(m_Controller.spaceRoot, "Stationary space origin", m_StationaryOriginMaterial, out m_StationaryOriginLabel);
                UpdateVisibility();
            }
        }

        void UpdateVisibility()
        {
            var isVisible = m_ShowOriginsToggle.isOn;

            // The stationary gizmo is also hidden while the space isn't located, because its parent is inactive then.
            if (m_XrOriginGizmo != null)
            {
                m_XrOriginGizmo.SetActive(isVisible);
            }

            if (m_StationaryOriginGizmo != null)
            {
                m_StationaryOriginGizmo.SetActive(isVisible);
            }
        }

        void LateUpdate()
        {
            // The origins can face any direction, so turn the labels toward the viewer to keep them readable.
            var xrOrigin = m_Controller.xrOrigin;
            if (xrOrigin == null || xrOrigin.Camera == null)
            {
                return;
            }

            var cameraPosition = xrOrigin.Camera.transform.position;
            FaceCamera(m_XrOriginLabel, cameraPosition);
            FaceCamera(m_StationaryOriginLabel, cameraPosition);
        }

        GameObject CreateGizmo(Transform parent, string labelText, Material sphereMaterial, out Transform label)
        {
            var gizmo = Instantiate(m_GizmoPrefab, parent, false);
            gizmo.name = labelText + " Gizmo";

            gizmo.GetComponentInChildren<Text>(true).text = labelText;

            // Turn the whole label canvas, so its background turns with the text.
            label = gizmo.GetComponentInChildren<Canvas>(true).transform;

            // Color the sphere like the matching marker, so each origin is easy to pair with its marker.
            gizmo.transform.Find(k_OriginSphereName).GetComponent<Renderer>().sharedMaterial = sphereMaterial;

            return gizmo;
        }

        static void FaceCamera(Transform label, Vector3 cameraPosition)
        {
            if (label == null || !label.gameObject.activeInHierarchy)
            {
                return;
            }

            // A world space canvas is readable from the side its forward axis points away from.
            var direction = Vector3.ProjectOnPlane(label.position - cameraPosition, Vector3.up);
            if (direction.sqrMagnitude > 0.0001f)
            {
                label.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        void OnDestroy()
        {
            // The gizmos live under the XR Origin, which can persist across scenes.
            if (m_XrOriginGizmo != null)
            {
                Destroy(m_XrOriginGizmo);
            }

            if (m_StationaryOriginGizmo != null)
            {
                Destroy(m_StationaryOriginGizmo);
            }
        }
    }
}
