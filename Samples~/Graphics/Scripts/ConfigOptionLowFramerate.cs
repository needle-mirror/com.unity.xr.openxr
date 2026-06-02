using System.Collections.Generic;
using System.Threading;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionLowFramerate : ConfigOption
    {
        protected override string ConfigDescription() =>
@"Used to manipulate the framerate the application will run at. Useful for simulating running an application under heavy loads";

        private bool useLowFramerate = false;
        private readonly string[] modeStrings = { "Full Framerate", "Low Framerate" };

        public override void InitializeOptions(int defaultValue = 0)
        {
            ConfigOptionNames = new List<string>(modeStrings);
            base.InitializeOptions();
        }

        public override void OnSelect(int option)
        {
            base.OnSelect(option);

            useLowFramerate = option == 1;
        }

        void Update()
        {
            if (useLowFramerate)
            {
                Thread.Sleep(60);
            }
        }
    }
}
