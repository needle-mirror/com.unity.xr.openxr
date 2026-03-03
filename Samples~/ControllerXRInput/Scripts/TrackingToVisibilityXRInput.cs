using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSampleXRInput
{
    public class TrackingToVisibilityXRInput : MonoBehaviour
    {
        [SerializeField]
        XRNode m_XRNode;

        [SerializeField] private GameObject m_target = null;

        // Update is called once per frame
        void Update()
        {
            // This queries the device at the specified XR node (i.e. left hand, right hand) for the
            // tracking state, as specified by the CommonUsages constants:
            // https://docs.unity3d.com/ScriptReference/XR.CommonUsages-isTracked.html
            // It then sets the targets active state based on whether it's tracked or not.
            var device = InputDevices.GetDeviceAtXRNode(m_XRNode);
            if (device.isValid && device.TryGetFeatureValue(CommonUsages.isTracked, out var isTracking))
            {
                if (m_target != null)
                    m_target.SetActive(isTracking);
            }
        }
    }
}
