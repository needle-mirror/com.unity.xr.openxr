using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using static Unity.XR.CoreUtils.XROrigin;


namespace UnityEngine.XR.OpenXR.Samples.TrackingOriginSample
{
    public class TrackingOriginUiController : MonoBehaviour
    {
        [Header("Mode toggles")]
        [SerializeField]
        Toggle m_deviceTrackModeToggle;

        [SerializeField]
        Toggle m_floorTrackModeToggle;

        [SerializeField]
        Toggle m_unboundedTrackModeToggle;

        [Header("Mode descriptions")]
        [SerializeField]
        GameObject m_deviceDescription;

        [SerializeField]
        GameObject m_floorDescription;

        [SerializeField]
        GameObject m_unboundedDescription;

        [Header("Height and recenter controls")]
        [SerializeField]
        GameObject m_heightSection;

        [SerializeField]
        Slider m_heightSlider;

        [SerializeField]
        float m_minHeightValue;

        [SerializeField]
        float m_maxHeightValue;

        [SerializeField]
        Text m_minHeightText;

        [SerializeField]
        Text m_maxHeightText;

        readonly List<TrackingOriginMode> supportedTrackingOriginModes = new();
        XROrigin m_xrOrigin;

        IEnumerator Start()
        {
            // The UI layout require a single frame wait before
            // modifying its elements, or the UI gets scrambled
            yield return null;

            m_xrOrigin = FindAnyObjectByType<XROrigin>(FindObjectsInactive.Exclude);

            // Setup initial values and interactivity for tracking origin modes
            GetSupportedTrackingOriginModes(supportedTrackingOriginModes);

            m_deviceTrackModeToggle.gameObject.SetActive(supportedTrackingOriginModes.Contains(TrackingOriginMode.Device));
            m_floorTrackModeToggle.gameObject.SetActive(supportedTrackingOriginModes.Contains(TrackingOriginMode.Floor));
            m_unboundedTrackModeToggle.gameObject.SetActive(supportedTrackingOriginModes.Contains(TrackingOriginMode.Unbounded));

            var currentTrackingMode = ConvertFromTrackModeFlags(m_xrOrigin.CurrentTrackingOriginMode);
            switch (currentTrackingMode)
            {
                case TrackingOriginMode.NotSpecified:
                    Debug.Log($"Tracking mode \"{currentTrackingMode}\" is the current tracking mode. Make sure the XR device is on and the XR app is active.");
                    break;
                case TrackingOriginMode.Device:
                    m_deviceTrackModeToggle.isOn = true;
                    break;
                case TrackingOriginMode.Floor:
                    m_floorTrackModeToggle.isOn = true;
                    break;
                case TrackingOriginMode.Unbounded:
                    m_unboundedTrackModeToggle.isOn = true;
                    break;
                default:
                    Debug.LogWarning($"Tracking mode \"{currentTrackingMode}\" is not supported.");
                    break;
            }

            // Setup initial values for recenter properties
            m_heightSlider.value = m_xrOrigin.CameraYOffset;

            // Setup min and max values for slider
            m_heightSlider.minValue = m_minHeightValue;
            m_heightSlider.maxValue = m_maxHeightValue;
            m_minHeightText.text = m_minHeightValue.ToString();
            m_maxHeightText.text = m_maxHeightValue.ToString();

            // Subscribe to UI events
            m_heightSlider.onValueChanged.AddListener(OnHeightChanged);

            m_deviceTrackModeToggle.onValueChanged.AddListener(isOn => ChangeTrackingMode(isOn, TrackingOriginMode.Device));
            m_floorTrackModeToggle.onValueChanged.AddListener(isOn => ChangeTrackingMode(isOn, TrackingOriginMode.Floor));
            m_unboundedTrackModeToggle.onValueChanged.AddListener(isOn => ChangeTrackingMode(isOn, TrackingOriginMode.Unbounded));

            UpdateUiVisibility(currentTrackingMode);
        }

        void GetSupportedTrackingOriginModes(List<TrackingOriginMode> list)
        {
            list.Clear();

            List<XRInputSubsystem> xrInputSubsystems = new();
            SubsystemManager.GetSubsystems(xrInputSubsystems);

            if (xrInputSubsystems.Count > 0)
            {
                var subsystem = xrInputSubsystems.First();
                var trackingOriginFlags = subsystem.GetSupportedTrackingOriginModes();

                if ((trackingOriginFlags & TrackingOriginModeFlags.Unknown) == 0)
                {
                    list.Add(TrackingOriginMode.NotSpecified);
                }
                if ((trackingOriginFlags & TrackingOriginModeFlags.Device) > 0)
                {
                    list.Add(TrackingOriginMode.Device);
                }
                if ((trackingOriginFlags & TrackingOriginModeFlags.Floor) > 0)
                {
                    list.Add(TrackingOriginMode.Floor);
                }
                if ((trackingOriginFlags & TrackingOriginModeFlags.TrackingReference) > 0)
                {
                    Debug.LogWarning($"TrackingOriginModeFlags \"{TrackingOriginModeFlags.TrackingReference}\" is not supported.");
                }
                if ((trackingOriginFlags & TrackingOriginModeFlags.Unbounded) > 0)
                {
                    list.Add(TrackingOriginMode.Unbounded);
                }
            }
        }

        TrackingOriginMode ConvertFromTrackModeFlags(TrackingOriginModeFlags flags)
        {
            if ((flags & TrackingOriginModeFlags.Device) > 0)
            {
                return TrackingOriginMode.Device;
            }
            if ((flags & TrackingOriginModeFlags.Floor) > 0)
            {
                return TrackingOriginMode.Floor;
            }
            if ((flags & TrackingOriginModeFlags.Unbounded) > 0)
            {
                return TrackingOriginMode.Unbounded;
            }

            return TrackingOriginMode.NotSpecified;
        }

        void ChangeTrackingMode(bool isOn, TrackingOriginMode newTrackingMode)
        {
            if (!isOn)
                return;

            m_xrOrigin.RequestedTrackingOriginMode = newTrackingMode;
            UpdateUiVisibility(newTrackingMode);
        }

        void UpdateUiVisibility(TrackingOriginMode mode)
        {
            m_deviceDescription.SetActive(mode == TrackingOriginMode.Device);
            m_floorDescription.SetActive(mode == TrackingOriginMode.Floor);
            m_unboundedDescription.SetActive(mode == TrackingOriginMode.Unbounded);
            // In most cases, the height is only used for the device tracking origin mode
            m_heightSection.SetActive(mode == TrackingOriginMode.Device);
        }

        void OnHeightChanged(float height)
        {
            m_xrOrigin.CameraYOffset = height;
        }
    }
}
