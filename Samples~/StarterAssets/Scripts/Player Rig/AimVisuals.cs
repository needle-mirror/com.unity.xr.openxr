using UnityEngine;
using UnityEngine.InputSystem;

public class AimVisuals : MonoBehaviour
{
    public Renderer selfRenderer;
    public Color inactiveColor;
    public Color activeColor;
    public InputActionReference interactionAction;

    // Update is called once per frame
    void Update()
    {
        selfRenderer.material.color = Color.Lerp(inactiveColor, activeColor, interactionAction.action.ReadValue<float>());
    }
}
