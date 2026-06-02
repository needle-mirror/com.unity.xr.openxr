using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample
{
    public class ActionToVector2SliderISX : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference m_ActionReference;
        public InputActionReference actionReference { get => m_ActionReference; set => m_ActionReference = value; }

        [SerializeField]
        public Slider xAxisSlider = null;

        [SerializeField]
        public Slider yAxisSlider = null;


        Graphic graphic;
        Graphic[] childGraphics;

        private void OnEnable()
        {
            if (xAxisSlider == null)
                Debug.LogWarning("ActionToSlider MonoBehaviour started without any associated X-axis slider.  This input won't be reported.", this);

            if (yAxisSlider == null)
                Debug.LogWarning("ActionToSlider MonoBehaviour started without any associated Y-axis slider.  This input won't be reported.", this);


            graphic = gameObject.GetComponent<Graphic>();
            childGraphics = gameObject.GetComponentsInChildren<Graphic>();
        }

        void Update()
        {
            if (actionReference != null && actionReference.action != null && xAxisSlider != null && yAxisSlider != null)
            {
                if (actionReference.action.enabled)
                {
                    SetVisible(true);
                }

                Vector2 value = actionReference.action.ReadValue<Vector2>();
                xAxisSlider.value = value.x;
                yAxisSlider.value = value.y;
            }
            else
            {
                SetVisible(false);
            }
        }

        void SetVisible(bool visible)
        {
            if (graphic != null)
                graphic.enabled = visible;

            for (int i = 0; i < childGraphics.Length; i++)
            {
                childGraphics[i].enabled = visible;
            }
        }
    }
}
