using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.XR.OpenXR.Input
{
    [InputControlLayout(displayName = "OpenXR HMD")]
    internal class OpenXRHmd : XRHMD
    {
        [InputControl] ButtonControl userPresence { get; set; }

        /// <inheritdoc/>
        protected override void FinishSetup()
        {
            base.FinishSetup();
            userPresence = GetChildControl<ButtonControl>("UserPresence");
        }
    }
}
