using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample
{
    public class TrackingToVisibility : MonoBehaviour
    {
        [SerializeField] private InputActionReference _trackingActionReference = null;

        [SerializeField] private GameObject _target = null;

        // Update is called once per frame
        void Update()
        {
            // This queries the input action reference, ensuring that it is non-null and that there is a valid input devices which uses the input action.
            // It then sets the targets active state based on the state of the tracking input action, which is commonly represented as a "pressed" button.
            if (_trackingActionReference.action != null &&
                _trackingActionReference.action.controls.Count > 0 &&
                _trackingActionReference.action.controls[0].device != null &&
                OpenXRInput.TryGetInputSourceName(_trackingActionReference.action, 0, out var actionName, OpenXRInput.InputSourceNameFlags.Component, _trackingActionReference.action.controls[0].device))
            {
                if (_target != null)
                    _target.SetActive(_trackingActionReference.action.IsPressed());
            }
        }
    }
}
