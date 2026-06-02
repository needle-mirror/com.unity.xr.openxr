using UnityEngine.UI;

namespace UnityEngine.XR.OpenXR.Samples.GraphicsSample
{
    public class TimerDisplay : MonoBehaviour
    {
        public Text timerText;
        private float timeElapsed = 0.0f;

        // Start is called before the first frame update
        void Update()
        {
            timeElapsed += Time.deltaTime;
            timerText.text = timeElapsed.ToString("0.00");
        }
    }
}
