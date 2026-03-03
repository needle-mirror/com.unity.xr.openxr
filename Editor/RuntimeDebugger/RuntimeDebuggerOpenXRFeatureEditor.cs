using UnityEngine;

using UnityEngine.XR.OpenXR.Features.RuntimeDebugger;

namespace UnityEditor.XR.OpenXR.Features.RuntimeDebugger
{
    [CustomEditor(typeof(RuntimeDebuggerOpenXRFeature))]
    internal class RuntimeDebuggerOpenXRFeatureEditor : Editor
    {
        private SerializedProperty cacheSize;
        private SerializedProperty perThreadCacheSize;

        void OnEnable()
        {
            cacheSize = serializedObject.FindProperty("cacheSize");
            perThreadCacheSize = serializedObject.FindProperty("perThreadCacheSize");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(cacheSize, new GUIContent("Cache Size", "Defines the maximum size of the cache (in bytes) used to store OpenXR runtime debugging information. The cache stores function call data and frame statistics for analysis in the Runtime Debugger Window. Increase this value if you need to capture more debugging data, especially for longer recording sessions."));
            EditorGUILayout.PropertyField(perThreadCacheSize, new GUIContent("Per Thread Cache Size", "Size of per-thread cache on device for runtime debugger in bytes."));

            if (GUILayout.Button("Open Debugger Window"))
            {
                RuntimeDebuggerWindow.Init();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
