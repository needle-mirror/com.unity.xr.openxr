using System;
using System.Runtime.InteropServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEditor.XR.OpenXR
{
    /// <summary>
    /// Custom property drawer for managing OpenXR API Layers in the Unity Inspector.
    /// This drawer provides a UI for adding layers from the OpenXR package, importing custom layers
    /// from JSON files, and managing the list of enabled API layers. It uses a ReorderableList
    /// to provide drag-and-drop functionality for layer ordering and management.
    /// </summary>
    [CustomPropertyDrawer(typeof(ApiLayers))]
    internal class OpenXRApiLayersPropertyDrawer : PropertyDrawer
    {
        // UI text constants
        const string k_HeaderText = "OpenXR API Layers";
        const string k_SelectJsonDialogTitle = "Select OpenXR API Layer JSON";
        const string k_AddLayersButtonText = "Add API Layers from OpenXR Package";

        // Serialization property name
        const string k_ApiLayersProperty = "m_Collection";

        static ApiLayersFeature s_SelectedApiLayersFeature;
        static BuildTargetGroup s_SelectedBuildTargetGroup;

        ReorderableList m_ReorderableList;
        SerializedProperty m_ApiLayersProperty;

        /// <summary>
        /// Renders the GUI for the API layers settings, including controls for adding layers and managing the layer list.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty representing the ApiLayersSettings.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (s_SelectedBuildTargetGroup != targetGroup)
            {
                s_SelectedBuildTargetGroup = targetGroup;
                var openXrSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(s_SelectedBuildTargetGroup);
                s_SelectedApiLayersFeature = openXrSettings.GetFeature<ApiLayersFeature>();
            }

            if (s_SelectedApiLayersFeature == null)
                return;

            if (m_ReorderableList == null)
                InitializeList(property);

            // Button to add bundled layers from the OpenXR package (only available on Windows atm)
            if (targetGroup == BuildTargetGroup.Standalone && Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (GUILayout.Button(k_AddLayersButtonText))
                {
                    OpenXRApiLayersBundle.AddBundle(s_SelectedBuildTargetGroup);
                    // Apply changes to the serialized object
                    m_ApiLayersProperty.serializedObject.Update();
                    RefreshList();
                }
            }

            // Draw the reorderable list of API layers
            m_ReorderableList?.DoLayoutList();
        }

        /// <summary>
        /// Returns the height of the property GUI. Since layout is handled by EditorGUILayout,
        /// a single line height is sufficient.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        /// <summary>
        /// Initializes the ReorderableList with callbacks for drawing and modification.
        /// </summary>
        /// <param name="property">The parent SerializedProperty for ApiLayersSettings.</param>
        void InitializeList(SerializedProperty property)
        {
            m_ApiLayersProperty = property.FindPropertyRelative(k_ApiLayersProperty);
            m_ReorderableList = new ReorderableList(property.serializedObject, m_ApiLayersProperty, true, true, true, true)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawElement,
                onReorderCallbackWithDetails = HandleReorderCallback,
                onAddCallback = HandleAddCallback,
                onRemoveCallback = HandleRemoveCallback
            };
        }

        void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, k_HeaderText);

        void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_ApiLayersProperty.GetArrayElementAtIndex(index);
            // Use the custom ApiLayerPropertyDrawer for rendering the element
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        void HandleAddCallback(ReorderableList list)
        {
            // Show architecture selection menu
            ShowArchitectureSelectionMenu();
        }

        /// <summary>
        /// Displays a menu for selecting the target architecture for the API layer.
        /// </summary>
        void ShowArchitectureSelectionMenu()
        {
            ApiLayers.IPlatformSupport platformSupport = null;
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            switch (targetGroup)
            {
                case BuildTargetGroup.Standalone:
                    // We currently only support Windows for Standalone.
                    platformSupport = new ApiLayers.WindowsPlatformSupport();
                    break;
                case BuildTargetGroup.Android:
                    platformSupport = new ApiLayers.AndroidPlatformSupport();
                    break;
            }

            if (platformSupport == null)
                return;

            var supportedArchitectures = platformSupport.GetSupportedArchitectures();
            var menu = new GenericMenu();

            foreach (var architecture in supportedArchitectures)
            {
                var architectureName = Enum.GetName(typeof(Architecture), architecture)?.ToLower();
                menu.AddItem(new GUIContent(architectureName), false, OnArchitectureSelected, architecture);
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// Callback invoked when an architecture is selected from the menu.
        /// </summary>
        /// <param name="selectedArchitecture">The selected architecture as an object.</param>
        void OnArchitectureSelected(object selectedArchitecture)
        {
            var architecture = (Architecture)selectedArchitecture;

            string jsonPath = EditorUtility.OpenFilePanel(k_SelectJsonDialogTitle, string.Empty, ApiLayers.k_JsonExt.TrimStart('.'));
            if (string.IsNullOrEmpty(jsonPath))
                return;

            if (s_SelectedApiLayersFeature.apiLayers.TryAdd(jsonPath, architecture, EditorUserBuildSettings.selectedBuildTargetGroup, out _))
            {
                // Apply changes to the serialized object.
                m_ApiLayersProperty.serializedObject.Update();
                RefreshList();
            }
        }

        void HandleReorderCallback(ReorderableList list, int oldIndex, int newIndex)
        {
            var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (openXRSettings == null)
                return;

            var apiLayersFeature = openXRSettings.GetFeature<ApiLayersFeature>();
            if (apiLayersFeature == null)
                return;

            apiLayersFeature.apiLayers.SetIndex(oldIndex, newIndex);
        }

        void HandleRemoveCallback(ReorderableList list)
        {
            int indexToRemove = list.index;
            if (indexToRemove < 0 || indexToRemove >= m_ApiLayersProperty.arraySize)
                return;

            // Remove the layer from settings and delete its files
            if (s_SelectedApiLayersFeature.apiLayers.TryRemove(indexToRemove))
            {
                // Apply changes to the serialized object
                m_ApiLayersProperty.serializedObject.Update();
                RefreshList();
            }
        }

        /// <summary>
        /// Refreshes the reorderable list by setting it to null, forcing re-initialization on the next OnGUI call.
        /// </summary>
        void RefreshList() => m_ReorderableList = null;
    }
}
