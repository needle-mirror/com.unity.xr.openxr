using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionBlitMode : ConfigOption
    {
        protected override string ConfigDescription() =>
@"Determines which view from the device should be mirrored to a secondary display.
Mirror Blit Modes are only an option on platforms which have device mirroring, such as in the unity editor and standalone applications
Calls into the function XRDisplaySubsystem.SetPreferredMirrorBlitMode";

        private void Start()
        {
            // Mirror Blit Modes are only an option on platforms which have device mirroring, such as in the unity editor and standalone applications
            // If we aren't on one of those platforms, disable this option
#if !(PLATFORM_STANDALONE || UNITY_EDITOR)
            gameObject.SetActive(false);
#endif
        }

        public override void InitializeOptions(int defaultValue = 0)
        {
            ConfigOptionNames = new List<string>() { "None", "Left Eye", "Right Eye", "Both Eyes", "Occlusion Mesh" };
            base.InitializeOptions(TranslateXRMirrorViewBlitModeToMode(XRMirrorViewBlitMode.SideBySide));
        }

        public override void OnSelect(int option)
        {
            base.OnSelect(option);

            var displaySubsystem = GetFirstDisplaySubsystem();
            if (displaySubsystem == null)
                return;

            displaySubsystem.SetPreferredMirrorBlitMode(TranslateModeToXRMirrorViewBlitMode(option));
        }

        static XRDisplaySubsystem GetFirstDisplaySubsystem()
        {
            List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displaySubsystems);
            if (displaySubsystems.Count == 0)
            {
                Debug.Log("No display subsystem found.");
                return null;
            }
            return displaySubsystems[0];
        }

        private int TranslateModeToXRMirrorViewBlitMode(int mode)
        {
            switch (mode)
            {
                case 1:
                    return (int)XRMirrorViewBlitMode.LeftEye; // Left Eye
                case 2:
                    return (int)XRMirrorViewBlitMode.RightEye; // Right Eye
                case 3:
                    return (int)XRMirrorViewBlitMode.SideBySide; // Both Eyes
                case 4:
                    return (int)XRMirrorViewBlitMode.SideBySideOcclusionMesh; // Occlusion Mesh
                default:
                    return (int)XRMirrorViewBlitMode.Default;
            }
        }

        private int TranslateXRMirrorViewBlitModeToMode(int xrMode)
        {
            switch (xrMode)
            {
                case (int)XRMirrorViewBlitMode.LeftEye:
                    return 1; // Left Eye
                case (int)XRMirrorViewBlitMode.RightEye:
                    return 2; // Right Eye
                case (int)XRMirrorViewBlitMode.SideBySide:
                    return 3; // Both Eyes
                case (int)XRMirrorViewBlitMode.SideBySideOcclusionMesh:
                    return 4; // Occlusion Mesh
                default:
                    return 0;
            }
        }

        [DllImport("UnityOpenXR", EntryPoint = "unity_ext_GetVisibleTexRect")]
        private static extern void Internal_GetVisibleTexRect(uint viewIndex, out Rect rect);

        public Texture2D oldBoxBorder;
        public Texture2D newBoxBorder;

        public bool shouldDrawRectangles;

#if UNITY_EDITOR
        void OnGUI()
        {
            var borderSize = 3; // Border size in pixels

            // Style for old mirror rectangle
            var oldStyle = new GUIStyle();
            oldStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
            oldStyle.normal.background = oldBoxBorder;

            // Style for the new mirror rectangle
            var newStyle = new GUIStyle();
            newStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
            newStyle.normal.background = newBoxBorder;

            if (shouldDrawRectangles && TranslateModeToXRMirrorViewBlitMode(Dropdown.value) == XRMirrorViewBlitMode.SideBySideOcclusionMesh)
            {
                // Left/Right Rects are in Device coordinates: (0,0) in lower-left and (1,1) in upper-right.
                Internal_GetVisibleTexRect(0, out Rect leftEyeRect);
                Internal_GetVisibleTexRect(1, out Rect rightEyeRect);

                // GUI Coordinates: (0,0) in the upper-left.
                // GUI Y coordinate: (1.0f - (oldRect.y + oldRect.height))
                Rect guiLeftEyeRect = new Rect(
                    leftEyeRect.x * Screen.width / 2,
                    (1.0f - (leftEyeRect.y + leftEyeRect.height)) * Screen.height,
                    leftEyeRect.width * Screen.width / 2,
                    leftEyeRect.height * Screen.height);

                Rect guiRightEyeRect = new Rect(
                    (Screen.width / 2) + rightEyeRect.x * Screen.width / 2,
                    (1.0f - (rightEyeRect.y + rightEyeRect.height)) * Screen.height,
                    rightEyeRect.width * Screen.width / 2,
                    rightEyeRect.height * Screen.height);

                Rect oldLeftEyeRect = new Rect(
                    0.2f * Screen.width / 2,
                    0.2f * Screen.height,
                    0.6f * Screen.width / 2,
                    0.6f * Screen.height);

                Rect oldRightEyeRect = new Rect(
                    (Screen.width / 2) + 0.2f * Screen.width / 2,
                    0.2f * Screen.height,
                    0.6f * Screen.width / 2,
                    0.6f * Screen.height);

                GUI.Box(oldLeftEyeRect, GUIContent.none, oldStyle);
                GUI.Box(guiLeftEyeRect, GUIContent.none, newStyle);
                GUI.Box(oldRightEyeRect, GUIContent.none, oldStyle);
                GUI.Box(guiRightEyeRect, GUIContent.none, newStyle);
            }
        }
#endif
    }
}
