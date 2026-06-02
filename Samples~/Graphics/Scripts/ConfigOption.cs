using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    [Serializable]
    public class ConfigOption : MonoBehaviour
    {
        public string ConfigName;
        /// <summary>
        /// A description of what this config option does, as well as a summary of what it's selectable options do.
        /// An error message will be logged if this descrption is not overriden by classes which are derived from a Config Option
        /// </summary>
        protected virtual string ConfigDescription() => "";

        protected List<string> ConfigOptionNames;

        [SerializeField]
        private Text ConfigNameText;
        [SerializeField]
        protected Dropdown Dropdown;

        public void OnValidate()
        {
            ConfigNameText.text = ConfigName;
        }

        public virtual void Awake()
        {
            InitializeOptions();
        }

        public virtual void InitializeOptions(int defaultValue = 0)
        {
            ConfigNameText.text = ConfigName;
            if (ConfigDescription() == "")
            {
                Debug.LogError($"Initialize the child class {ConfigName} that inhereits ConfigOption with an appripriate description by instantiating the string variable, ConfigDescription");
            }

            Dropdown.ClearOptions();
            var dropdownOptions = new List<Dropdown.OptionData>();
            for (int i = 0; i < ConfigOptionNames.Count; i++)
            {
                dropdownOptions.Add(new Dropdown.OptionData(ConfigOptionNames[i]));
            }
            Dropdown.AddOptions(dropdownOptions);

            Dropdown.value = defaultValue;
            Dropdown.RefreshShownValue();

            Dropdown.onValueChanged.RemoveListener(OnSelect);
            Dropdown.onValueChanged.AddListener(OnSelect);
        }

        public virtual void OnSelect(int option)
        {
        }

        public void SetDescriptor()
        {
            if (ConfigOptionDescriptor.instance != null)
            {
                ConfigOptionDescriptor.instance.SetDescriptor(ConfigName, ConfigDescription());
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ConfigOption), true)]
    public class ConfigOptionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw default inspector fields
            DrawDefaultInspector();

            // Reference to the target component
            ConfigOption myTarget = (ConfigOption)target;

            // Add a button
            if (GUILayout.Button("Sync Dropdown options"))
            {
                // Record undo for editor operations (optional)
                Undo.RecordObject(myTarget, "Sync Dropdown option");

                // Call the method
                myTarget.InitializeOptions();

                // Mark the object as dirty if it changes serialized data
                EditorUtility.SetDirty(myTarget);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
