using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample
{
    public class ActionToHaptics : MonoBehaviour
    {
        public InputActionReference action;
        public InputActionReference hapticAction;
        public float _amplitude = 1.0f;
        public float _duration = 0.1f;
        public float _frequency = 0.0f;

        private void OnEnable()
        {
            if (action == null || hapticAction == null)
                return;

            action.action.Enable();
            hapticAction.action.Enable();
            action.action.performed += SendHaptic;
        }

        private void OnDisable()
        {
            if (action != null && action.action != null)
                action.action.performed -= SendHaptic;
        }

        private void SendHaptic(InputAction.CallbackContext ctx)
        {
            var control = action.action.activeControl;
            if (null == control)
                return;

            OpenXRInput.SendHapticImpulse(hapticAction.action, _amplitude, _frequency, _duration, control.device);
        }
    }
}
