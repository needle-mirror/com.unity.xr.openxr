using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionDepthMode : ConfigOption
    {
        protected override string ConfigDescription() =>
    @"A setting which determines the OpenXR Runtime's Depth submission mode.
Use this setting to test project compatibility with different depth submission modes. It is typically used for sophisticated visual effects.
Directly sets the property OpenXRSettings.DepthSubmissionMode";

        public override void InitializeOptions(int defaultValue = 0)
        {
            ConfigOptionNames = new List<string>() { "None", "16-bit", "24-bit" };
            base.InitializeOptions((int)OpenXRSettings.DepthSubmissionMode.None);
        }

        public override void OnSelect(int option)
        {
            base.OnSelect(option);

            OpenXRSettings.Instance.depthSubmissionMode = (OpenXRSettings.DepthSubmissionMode)option;
        }
    }
}
