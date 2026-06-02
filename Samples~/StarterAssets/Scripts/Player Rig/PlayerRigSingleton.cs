using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.OpenXR.Samples
{
    public class PlayerRigSingleton : MonoBehaviour
    {
        public static PlayerRigSingleton instance;
        public Camera playerCamera;

        [Tooltip("Input action assets to affect when inputs are enabled or disabled.")]
        public List<InputActionAsset> playerActionAssets;

        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);

                InitializeUICameras();
                EnableInput();
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
                Destroy(this.gameObject);
        }

        // De-Initialize the instance when it is destroyed to clear the static reference
        void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }


        // Initializes UI in the scene when the scene loads
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeUICameras();
        }

        void InitializeUICameras()
        {
#if UNITY_6000_4_OR_NEWER
            foreach (Canvas uiCanvas in FindObjectsByType<Canvas>())
#else
            foreach (Canvas uiCanvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
#endif
            {
                if (uiCanvas.renderMode == RenderMode.WorldSpace)
                {
                    uiCanvas.worldCamera = playerCamera;
                }
            }
        }

        void EnableInput()
        {
            if (playerActionAssets == null)
                return;

            foreach (var actionAsset in playerActionAssets)
            {
                if (actionAsset != null)
                {
                    actionAsset.Enable();
                }
            }
        }
    }
}
