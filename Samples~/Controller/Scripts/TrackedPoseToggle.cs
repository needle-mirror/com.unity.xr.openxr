using UnityEngine.InputSystem.XR;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample
{
    public class TrackedPoseToggle : MonoBehaviour
    {
        public TrackedPoseDriver LeftControlsUITrackedPoseDriver;
        public TrackedPoseDriver RightControlsUITrackedPoseDriver;

        private void OnDestroy()
        {
            if (LeftControlsUITrackedPoseDriver != null)
                Destroy(LeftControlsUITrackedPoseDriver.gameObject);
            if (RightControlsUITrackedPoseDriver != null)
                Destroy(RightControlsUITrackedPoseDriver.gameObject);
        }

        public void ToggleLeftTrackingPose(bool isEnabled)
        {
            ToggleTrackingPose(LeftControlsUITrackedPoseDriver, isEnabled);
        }
        public void ToggleRightTrackingPose(bool isEnabled)
        {
            ToggleTrackingPose(RightControlsUITrackedPoseDriver, isEnabled);
        }

        public void ToggleTrackingPose(TrackedPoseDriver ControlsUITrackedPoseDriver, bool isEnabled)
        {
            ControlsUITrackedPoseDriver.enabled = isEnabled;
            if (!isEnabled)
            {
                // Decouple the pose driver's transform from the associated node on the player rig
                ControlsUITrackedPoseDriver.transform.parent = null;

                ControlsUITrackedPoseDriver.transform.position = transform.position + Vector3.back * 0.05f;
                ControlsUITrackedPoseDriver.transform.localRotation = Quaternion.Euler(-90 * Vector3.right);
            }
            else
            {
                // Parent the pose driver to the same origin transform that the player camera is parented to, preserving any camera offset values
                if (PlayerRigSingleton.instance != null && PlayerRigSingleton.instance.playerCamera)
                {
                    ControlsUITrackedPoseDriver.transform.parent = PlayerRigSingleton.instance.playerCamera.transform.parent;
                }
            }
        }
    }
}
