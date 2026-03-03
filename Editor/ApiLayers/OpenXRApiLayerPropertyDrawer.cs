using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace UnityEditor.XR.OpenXR
{
    /// <summary>
    /// Custom property drawer for displaying OpenXR API Layer information in the Unity Inspector.
    /// This drawer provides a toggle for enabling or disabling a layer and shows detailed layer
    /// information in a tooltip. It is responsible for the visual representation of individual
    /// API layer entries.
    /// </summary>
    [CustomPropertyDrawer(typeof(ApiLayers.ApiLayer))]
    internal class OpenXRApiLayerPropertyDrawer : PropertyDrawer
    {
        // Property field names
        internal const string k_JsonProperty = "m_Json";
        internal const string k_JsonNameProperty = "name";

        internal const string k_LibraryArchitectureProperty = "m_LibraryArchitecture";
        const string k_IsEnabledProperty = "m_IsEnabled";
        const string k_JsonLibraryPathProperty = "library_path";
        const string k_JsonApiVersionProperty = "api_version";
        const string k_JsonImplementationVersionProperty = "implementation_version";
        const string k_JsonDescriptionProperty = "description";

        // Tooltip formatting constants
        const string k_TooltipHeader = "API Layer Details:\n";
        const string k_TooltipNamePrefix = "  Name: ";
        const string k_TooltipLibraryPrefix = "  Library: ";
        const string k_TooltipApiVersionPrefix = "  API Version: ";
        const string k_TooltipImplVersionPrefix = "  Implementation Version: ";
        const string k_TooltipDescriptionPrefix = "  Description: ";
        const string k_InvalidLayerText = "Invalid API Layer";

        // Dimensions for the enabled toggle
        const float k_ToggleWidth = 20f;
        const float k_ToggleMargin = 25f;

        /// <summary>
        /// Renders the GUI for an API layer property. This includes an enable/disable toggle
        /// and the layer's name with a detailed tooltip.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to create the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isEnabled = property.FindPropertyRelative(k_IsEnabledProperty);
            var nameProperty = property.FindPropertyRelative(k_JsonProperty)?.FindPropertyRelative(k_JsonNameProperty);

            // If the name property can't be found, display an error label.
            if (nameProperty == null)
            {
                EditorGUI.LabelField(position, k_InvalidLayerText);
                return;
            }

            // Define rectangles for the toggle and the label
            var toggleRect = new Rect(position.x, position.y, k_ToggleWidth, position.height);
            var labelRect = new Rect(position.x + k_ToggleMargin, position.y, position.width - k_ToggleMargin, position.height);

            var tooltip = CreateTooltip(property);
            var architectureName = Enum.GetName(typeof(Architecture), property.FindPropertyRelative(k_LibraryArchitectureProperty)?.uintValue)?.ToLower();
            var labelContent = new GUIContent($"[{architectureName}] {nameProperty?.stringValue}", tooltip);

            EditorGUI.BeginProperty(position, label, property);
            // Draw the toggle and apply changes
            isEnabled.boolValue = EditorGUI.Toggle(toggleRect, isEnabled.boolValue);
            property.ApplyModifiedProperties();

            // Draw the label with the tooltip
            EditorGUI.LabelField(labelRect, labelContent);
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Returns the height of the property GUI. Uses a single line height for a compact display.
        /// </summary>
        /// <param name="property">The SerializedProperty to get the height for.</param>
        /// <param name="label">The label of the property.</param>
        /// <returns>The height in pixels that the property will occupy.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        /// <summary>
        /// Creates a formatted tooltip string containing details about the API layer.
        /// </summary>
        /// <param name="layer">The SerializedProperty representing the API layer.</param>
        /// <returns>A formatted string with layer details for use in a tooltip.</returns>
        static string CreateTooltip(SerializedProperty layer)
        {
            var sb = new StringBuilder(k_TooltipHeader);
            sb.AppendLine($"{k_TooltipNamePrefix}{layer.FindPropertyRelative(k_JsonProperty)?.FindPropertyRelative(k_JsonNameProperty)?.stringValue}");
            sb.AppendLine($"{k_TooltipLibraryPrefix}{layer.FindPropertyRelative(k_JsonProperty)?.FindPropertyRelative(k_JsonLibraryPathProperty)?.stringValue}");
            sb.AppendLine($"{k_TooltipApiVersionPrefix}{layer.FindPropertyRelative(k_JsonProperty)?.FindPropertyRelative(k_JsonApiVersionProperty)?.stringValue}");
            sb.AppendLine($"{k_TooltipImplVersionPrefix}{layer.FindPropertyRelative(k_JsonProperty)?.FindPropertyRelative(k_JsonImplementationVersionProperty)?.stringValue}");
            sb.AppendLine($"{k_TooltipDescriptionPrefix}{layer.FindPropertyRelative(k_JsonProperty)?.FindPropertyRelative(k_JsonDescriptionProperty)?.stringValue}");
            return sb.ToString();
        }
    }
}
