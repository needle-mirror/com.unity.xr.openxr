using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;

using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

namespace UnityEditor.XR.OpenXR.Features.MetaQuestSupport
{
    [CustomEditor(typeof(MetaQuestFeature))]
    internal class MetaQuestFeatureEditor : Editor
    {
        private bool m_ShowAndroidExperimental = false;
        private bool m_LateLatchingModeEnabled;
        private bool m_LateLatchingDebug;

        private static GUIContent s_LateLatchingSupportedLabel = EditorGUIUtility.TrTextContent("Late Latching (Vulkan)");
        private static GUIContent s_LateLatchingDebugLabel = EditorGUIUtility.TrTextContent("Late Latching Debug Mode");
        private static GUIContent s_ShowAndroidExperimentalLabel = EditorGUIUtility.TrTextContent("Experimental", "Experimental settings that are under active development and should be used with caution.");
#if UNITY_6000_1_OR_NEWER
        private static GUIContent s_MultiviewRenderRegionsOptimizationsLabel = EditorGUIUtility.TrTextContent("Multiview Render Regions Optimizations (Vulkan)", "Activates Multiview Render Regions optimizations at application start. Requires usage of Unity 6.1 or later, Vulkan as the Graphics API, Render Mode set to Multi-view and Symmetric rendering enabled.");
#endif

        struct TargetDeviceProperty
        {
            public SerializedProperty property;
            public GUIContent label;
        }

        private List<TargetDeviceProperty> targetDeviceProperties;
        private Dictionary<string, bool> activeTargetDevices;
        private SerializedProperty forceRemoveInternetPermission;
        private SerializedProperty systemSplashScreen;

        private SerializedProperty symmetricProjection;

        private SerializedProperty m_LateLatchingModeProperty;
        private SerializedProperty m_LateLatchingDebugProperty;

        private SerializedProperty optimizeBufferDiscards;
#if UNITY_6000_1_OR_NEWER
        private SerializedProperty multiviewRenderRegionsOptimizationMode;
#endif

        private SerializedProperty spacewarpMotionVectorTextureFormat;

        void InitActiveTargetDevices()
        {
            activeTargetDevices = new Dictionary<string, bool>();

            OpenXRSettings androidOpenXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            var questFeature = androidOpenXRSettings.GetFeature<MetaQuestFeature>();

            if (questFeature == null)
                return;

            foreach (var dev in questFeature.targetDevices)
            {
                activeTargetDevices.Add(dev.manifestName, dev.active);
            }
        }

        void OnEnable()
        {
            forceRemoveInternetPermission =
                serializedObject.FindProperty("forceRemoveInternetPermission");
            systemSplashScreen =
                serializedObject.FindProperty("systemSplashScreen");

            symmetricProjection =
                serializedObject.FindProperty("symmetricProjection");

            optimizeBufferDiscards =
                serializedObject.FindProperty("optimizeBufferDiscards");
#if UNITY_6000_1_OR_NEWER
            multiviewRenderRegionsOptimizationMode =
                serializedObject.FindProperty("multiviewRenderRegionsOptimizationMode");
#endif

            targetDeviceProperties = new List<TargetDeviceProperty>();
            InitActiveTargetDevices();
            if (activeTargetDevices.Count == 0)
                return;
            var targetDevicesProperty = serializedObject.FindProperty("targetDevices");

            // mapping these to Properties so tha we can get Undo/redo functionality
            m_LateLatchingDebugProperty = serializedObject.FindProperty("lateLatchingDebug");
            m_LateLatchingModeProperty = serializedObject.FindProperty("lateLatchingMode");

            spacewarpMotionVectorTextureFormat = serializedObject.FindProperty("spacewarpMotionVectorTextureFormat");

            for (int i = 0; i < targetDevicesProperty.arraySize; ++i)
            {
                var targetDeviceProp = targetDevicesProperty.GetArrayElementAtIndex(i);
                var propManifestName = targetDeviceProp.FindPropertyRelative("manifestName");

                // don't present inactive target devices to the user
                if (propManifestName == null || activeTargetDevices[propManifestName.stringValue] == false)
                    continue;
                var propEnabled = targetDeviceProp.FindPropertyRelative("enabled");
                var propName = targetDeviceProp.FindPropertyRelative("visibleName");
                TargetDeviceProperty curTarget = new TargetDeviceProperty { property = propEnabled, label = EditorGUIUtility.TrTextContent(propName.stringValue) };
                targetDeviceProperties.Add(curTarget);
            }
        }

        public override void OnInspectorGUI()
        {
            // Update anything from the serializable object
            EditorGUIUtility.labelWidth = 300.0f;

            serializedObject.Update();
            EditorGUILayout.LabelField("Rendering Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(symmetricProjection, new GUIContent("Symmetric Projection (Vulkan)"));
            EditorGUILayout.PropertyField(optimizeBufferDiscards, new GUIContent("Optimize Buffer Discards (Vulkan)"));
            // OptimizeMultiviewRenderRegions (aka MVPVV) only supported on Unity 6.1 onwards
#if UNITY_6000_1_OR_NEWER
            EditorGUILayout.PropertyField(multiviewRenderRegionsOptimizationMode, s_MultiviewRenderRegionsOptimizationsLabel);
#endif
            EditorGUILayout.PropertyField(spacewarpMotionVectorTextureFormat, new GUIContent("Space Warp motion vector texture format"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Manifest Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(forceRemoveInternetPermission);
            EditorGUILayout.PropertyField(systemSplashScreen);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Target Devices", EditorStyles.boldLabel);

            // Layout the Target Device properties
            EditorGUI.indentLevel++;
            foreach (var device in targetDeviceProperties)
            {
                EditorGUILayout.PropertyField(device.property, device.label);
            }
            EditorGUI.indentLevel--;

            // Foldout for the Experimental properties
            if (m_ShowAndroidExperimental = EditorGUILayout.Foldout(m_ShowAndroidExperimental, s_ShowAndroidExperimentalLabel, EditorStyles.miniBoldFont))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_LateLatchingModeProperty, s_LateLatchingSupportedLabel);
                EditorGUILayout.PropertyField(m_LateLatchingDebugProperty, s_LateLatchingDebugLabel);
                EditorGUI.indentLevel--;
            }

            EditorGUIUtility.labelWidth = 0.0f;

            // update any serializable properties
            serializedObject.ApplyModifiedProperties();

            OpenXRSettings androidOpenXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            var serializedOpenXrSettings = new SerializedObject(androidOpenXRSettings);

            androidOpenXRSettings.symmetricProjection = symmetricProjection.boolValue;
#if UNITY_6000_1_OR_NEWER
            androidOpenXRSettings.multiviewRenderRegionsOptimizationMode = (OpenXRSettings.MultiviewRenderRegionsOptimizationMode)multiviewRenderRegionsOptimizationMode.intValue;
#endif
            androidOpenXRSettings.optimizeBufferDiscards = optimizeBufferDiscards.boolValue;
            androidOpenXRSettings.spacewarpMotionVectorTextureFormat = (OpenXRSettings.SpaceWarpMotionVectorTextureFormat)spacewarpMotionVectorTextureFormat.enumValueIndex;
            serializedOpenXrSettings.ApplyModifiedProperties();

            EditorGUIUtility.labelWidth = 0.0f;
        }
    }
}
