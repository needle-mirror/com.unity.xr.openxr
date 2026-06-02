using UnityEditor;
using UnityEngine;

namespace UnityEditor.XR.OpenXR
{
    [FilePath("ProjectSettings/BuildProfileUtilityOpenXR.asset", FilePathAttribute.Location.ProjectFolder)]
    class BuildProfileUtilityOpenXR : ScriptableSingleton<BuildProfileUtilityOpenXR>
    {
        static readonly string[] metaFeatureIds =
        {
            "com.unity.openxr.feature.input.oculustouch",
            "com.unity.openxr.feature.input.metaquestpro",
            "com.unity.openxr.feature.input.metaquestplus",
            "com.unity.openxr.feature.metaquest"
        };

        [SerializeField, HideInInspector]
        bool isMetaQuestInitialized;

        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            EditorApplication.delayCall += OnEditorReloadedDelayed;
        }

        static void OnEditorReloadedDelayed()
        {
#if UNITY_META_QUEST
            if (!instance.isMetaQuestInitialized)
                instance.InitializeMetaQuestSettings();
#else
            if (instance.isMetaQuestInitialized)
            {
                instance.DisableMetaQuestSettings();
                instance.isMetaQuestInitialized = false;
                instance.Save(true);
            }
#endif
        }

        void InitializeMetaQuestSettings()
        {
            Features.FeatureHelpers.RefreshFeatures(BuildTargetGroup.Android);

            foreach (var featureId in metaFeatureIds)
            {
                var feature = Features.FeatureHelpers.GetFeatureWithIdForBuildTarget(BuildTargetGroup.Android, featureId);
                if (feature != null)
                    feature.enabled = true;
            }
            isMetaQuestInitialized = true;
            Save(true);
        }

        void DisableMetaQuestSettings()
        {
            foreach (var featureId in metaFeatureIds)
            {
                var feature = Features.FeatureHelpers.GetFeatureWithIdForBuildTarget(BuildTargetGroup.Android, featureId);
                if (feature != null)
                    feature.enabled = false;
            }
        }
    }
}
