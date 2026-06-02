using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class RuntimeInfo : MonoBehaviour
    {
        public TextMesh runtimeText;

        // Start is called before the first frame update
        void Start()
        {
            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);

            string opaque = "Unknown";
            if (displays.Count > 0)
            {
                opaque = displays[0].displayOpaque ? "Yes" : "No";
            }

            runtimeText.text = $"{Application.productName}\n" +
                $"Unity Version: {Application.unityVersion}\n" +
                $"Unity OpenXR Plugin Version: {OpenXRRuntime.pluginVersion}\n" +
                $"{OpenXRRuntime.name} {OpenXRRuntime.version}\n" +
                $"Display Opaque: {opaque}";
        }
    }
}
