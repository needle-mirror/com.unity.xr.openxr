using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ControllerXRInputSample
{
    public class TextureSelector : MonoBehaviour
    {
        public Material birp_material;
        public Material urp_material;

        void Start()
        {
            Renderer renderer = GetComponent<Renderer>();

            if (GraphicsSettings.currentRenderPipeline != null
                && GraphicsSettings.currentRenderPipeline.GetType().Name.Contains("Universal"))
            {
                renderer.material = urp_material;
            }
            else
            {
                renderer.material = birp_material;
            }
        }
    }
}
