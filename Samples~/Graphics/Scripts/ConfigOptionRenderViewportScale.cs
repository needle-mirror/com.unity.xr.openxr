using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionRenderViewportScale : ConfigOption
    {
        protected override string ConfigDescription() =>
    @"Controls how much of the allocated display texture should be used for rendering. A lower setting will result in a image with lower resolution.
Directly sets the property XRDisplaySubsystem.scaleOfAllViewports";


        private enum ViewportScaleOptions
        {
            Full,
            Half,
            Variable
        }

        private ViewportScaleOptions mode = ViewportScaleOptions.Full;
        private const float full_viewport_scale = 1.0f;
        private const float half_viewport_scale = 0.5f;

        private XRDisplaySubsystem display = null;
        private float currentViewportScale = full_viewport_scale;
        private float variableScaleSpeed = 2.0f;

        public override void InitializeOptions(int defaultValue = 0)
        {
            ConfigOptionNames = new List<string>() { "Full", "Half", "Variable" };
            base.InitializeOptions(defaultValue);
        }

        public override void OnSelect(int option)
        {
            base.OnSelect(option);
            mode = (ViewportScaleOptions)option;

            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);
            if (displays.Count == 1)
            {
                display = displays[0];
                display.scaleOfAllViewports = 1.0f;
            }
        }

        void Update()
        {
            if (display == null)
                return;

            switch (mode)
            {
                case ViewportScaleOptions.Full:
                    if (display.scaleOfAllViewports != full_viewport_scale)
                        display.scaleOfAllViewports = full_viewport_scale;
                    break;
                case ViewportScaleOptions.Half:
                    if (display.scaleOfAllViewports != half_viewport_scale)
                        display.scaleOfAllViewports = half_viewport_scale;
                    break;
                case ViewportScaleOptions.Variable:
                    currentViewportScale -= Time.deltaTime * variableScaleSpeed;
                    while (currentViewportScale < 0.0f)
                        currentViewportScale += 1.0f;
                    float variableScale = full_viewport_scale - Mathf.Abs(currentViewportScale - half_viewport_scale);
                    display.scaleOfAllViewports = variableScale;
                    break;
            }
        }
    }
}
