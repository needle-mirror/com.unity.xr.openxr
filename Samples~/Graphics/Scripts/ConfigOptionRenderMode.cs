using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionRenderMode : ConfigOption
    {
        protected override string ConfigDescription() =>
    @"A setting which determines the OpenXR Runtime's Stereo rendering mode.
MultiPass submits a separate draw calls for each eye. SinglePassInstanced submits one draw call for both eyes.
Directly sets the property OpenXRSettings.RenderMode";
        public override void InitializeOptions(int defaultValue = 0)
        {
            ConfigOptionNames = new List<string>() { "Multi-Pass", "Single-Pass Instanced" };
            base.InitializeOptions((int)OpenXRSettings.RenderMode.MultiPass);
        }

        public override void OnSelect(int option)
        {
            base.OnSelect(option);

            OpenXRSettings.Instance.renderMode = (OpenXRSettings.RenderMode)option;
        }
    }
}
