using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Linq;

namespace UnityEngine.XR.OpenXR.Samples
{
    public class SceneNavigationUI : MonoBehaviour
    {
        public static SceneNavigationUI instance;
        public Text sceneLabel;
        public Dropdown sceneSelectionDropdown;

        private string SampleDirectoryName = "Samples";
        private string PackageName = "OpenXR Plugin";

        void Start()
        {
            // Implementation of the singleton pattern

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            // Settng up the Scene Navigation Menu's Dropdown
            sceneSelectionDropdown.options.Clear();

            // This iterates through all scenes that have been added to the build settings
            // identifing scenes which are included in samples imported through the OpenXR package,
            // and then adds them to the dropdown menu
            string sampleDirectoryPath = Path.Combine(SampleDirectoryName, PackageName);
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (Path.GetFullPath(scenePath).Contains(sampleDirectoryPath))
                {
                    string sceneName = Path.GetFileName(scenePath);
                    sceneSelectionDropdown.options.Add(new Dropdown.OptionData(sceneName));
                }
            }

            // initialize the dropdown's text to the selected defaul options text (i.e. index 0)
            if (sceneSelectionDropdown.options.Count > 0)
                sceneSelectionDropdown.captionText.text = sceneSelectionDropdown.options[sceneSelectionDropdown.value].text;
        }

        // De-Initialize the instance when it is destroyed to clear the static reference
        void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        void Update()
        {
            // Ensures that the name of the current scene is kept up to date.
            sceneLabel.text = SceneManager.GetActiveScene().name;
        }

        void SwitchScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
