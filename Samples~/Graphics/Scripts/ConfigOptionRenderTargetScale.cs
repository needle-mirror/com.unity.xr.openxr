using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionRenderTargetScale : ConfigOption
    {
        private readonly float[] renderScaleValues = { 1.0f, 0.5f, 1.2f };
        private XRDisplaySubsystem display = null;

        protected override string ConfigDescription() =>
@"Controls the size of the textures submitted to the display as a multiplier of the display's default resolution.
Values less than 1.0 use lower resolution textures, which might improve performance at the expense of a less sharp image.
Values greater than 1.0 use higher resolution textures, resulting in a potentially sharper image at a cost to performance and increased memory usage.
Directly sets the property XRDisplaySubsystem.scaleOfAllRenderTargets";

        public override void InitializeOptions(int defaultValue = 0)
        {
            ConfigOptionNames = renderScaleValues.Select(val => val.ToString()).ToList();
            base.InitializeOptions();
        }

        public override void OnSelect(int option)
        {
            base.OnSelect(option);

            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);
            if (displays.Count == 1)
            {
                display = displays[0];
                display.scaleOfAllRenderTargets = renderScaleValues[option];
            }
        }
    }
}
