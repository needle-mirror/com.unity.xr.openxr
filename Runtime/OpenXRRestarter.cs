using System;
using System.Collections;

using UnityEngine;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR
{
    internal class OpenXRRestarter : MonoBehaviour
    {
#if UNITY_INCLUDE_TESTS
        public Action onAfterRestart;
        public Action onAfterShutdown;
        public Action onQuit;
        public Action onAfterCoroutine;

        public void ResetCallbacks ()
        {
            onAfterRestart = null;
            onAfterShutdown = null;
            onAfterCoroutine = null;
            onQuit = null;
        }
#endif

        /// <summary>
        /// True if the restarter is currently running
        /// </summary>
        public bool isRunning => m_Coroutine != null;

        private static OpenXRRestarter s_Instance = null;

        private Coroutine m_Coroutine;

        public static OpenXRRestarter Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var go = GameObject.Find("~oxrestarter");
                    if (go == null)
                    {
                        go = new GameObject("~oxrestarter");
                        go.hideFlags = HideFlags.HideAndDontSave;
                        go.AddComponent<OpenXRRestarter>();
                    }
                    s_Instance = go.GetComponent<OpenXRRestarter>();
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// Shutdown the the OpenXR loader and optionally quit the application
        /// </summary>
        public void Shutdown ()
        {
            if (OpenXRLoader.Instance == null)
                return;

            if (m_Coroutine != null)
            {
                Debug.LogError("Only one shutdown or restart can be executed at a time");
                return;
            }

            m_Coroutine = StartCoroutine(RestartCoroutine(false));
        }

        /// <summary>
        /// Restart the OpenXR loader
        /// </summary>
        public void ShutdownAndRestart ()
        {
            if (OpenXRLoader.Instance == null)
                return;

            if (m_Coroutine != null)
            {
                Debug.LogError("Only one shutdown or restart can be executed at a time");
                return;
            }

            m_Coroutine = StartCoroutine(RestartCoroutine(true));
        }

        private IEnumerator RestartCoroutine (bool shouldRestart)
        {
            try
            {
                // Always shutdown the loader
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
                yield return null;

#if UNITY_INCLUDE_TESTS
                onAfterShutdown?.Invoke();
#endif

                // Restart?
                if (shouldRestart && OpenXRRuntime.ShouldRestart())
                {
                    yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

                    XRGeneralSettings.Instance.Manager.StartSubsystems();

                    if (XRGeneralSettings.Instance.Manager.activeLoader == null)
                        Debug.LogError("Failure to restart OpenXRLoader after shutdown.");

#if UNITY_INCLUDE_TESTS
                    onAfterRestart?.Invoke();
#endif
                }
                // Quit?
                else if (OpenXRRuntime.ShouldQuit())
                {
#if UNITY_INCLUDE_TESTS
                    onQuit?.Invoke();
#elif UNITY_EDITOR
                    if (EditorApplication.isPlaying || EditorApplication.isPaused)
                    {
                        EditorApplication.ExitPlaymode();
                    }
#else
                    Application.Quit();
#endif
                }
            }
            finally
            {
#if UNITY_INCLUDE_TESTS
                m_Coroutine = null;
                onAfterCoroutine?.Invoke();
#endif
            }
        }
    }
}
