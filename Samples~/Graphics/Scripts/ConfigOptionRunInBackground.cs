using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionRunInBackground : ConfigOption
    {
        protected override string ConfigDescription() =>
            @"A setting which determines if the application pauses when it is in the background. Directly sets the property Application.runInBackground";
        public override void InitializeOptions(int defaultValue = 0)
        {
            ConfigOptionNames = new List<string>() { "Disabled", "Enabled" };
            if (Application.runInBackground)
                base.InitializeOptions(1);
            else
                base.InitializeOptions(0);
        }

        public override void OnSelect(int option)
        {
            base.OnSelect(option);

            Application.runInBackground = option == 1;
        }
    }
}
