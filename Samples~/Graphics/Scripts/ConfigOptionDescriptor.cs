using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class ConfigOptionDescriptor : MonoBehaviour
    {
        public Text ConfigOptionLabel;

        public Text ConfigOptionDescription;

        public static ConfigOptionDescriptor instance;

        // Initialize the instance so it can be accessed by any game object
        void Start()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }

        // De-Initialize the instance when it is destroyed to clear the static reference
        void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        public void SetDescriptor(string configName, string description)
        {
            if (ConfigOptionLabel != null)
                ConfigOptionLabel.text = configName;
            if (ConfigOptionDescription != null)
                ConfigOptionDescription.text = description;
        }
    }
}
