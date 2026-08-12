#region DynamicFoveationExample
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEngine.XR.OpenXR.CodeSamples.Editor.Tests
{
    public class DynamicFoveationStarter : MonoBehaviour
    {
        List<XRDisplaySubsystem> xrDisplays = new List<XRDisplaySubsystem>();

        void Start()
        {
            var foveatedRendering = OpenXRSettings.Instance.GetFeature<FoveatedRenderingFeature>();
            if (foveatedRendering == null || !foveatedRendering.enabled)
            {
                Debug.LogWarning("The Foveated Rendering feature is not enabled.");
                return;
            }

            // Dynamic foveation has no effect while the foveation level is 0, which is the
            // default, so set a level first. The device treats it as the maximum amount of
            // foveation it can apply.
            SubsystemManager.GetSubsystems(xrDisplays);
            if (xrDisplays.Count == 1)
            {
                xrDisplays[0].foveatedRenderingLevel = 0.7f;
            }

            // The device can now apply less foveation than the level set above whenever it
            // has GPU capacity to spare.
            foveatedRendering.DynamicFoveationEnabled = true;
        }
    }
}
#endregion
// Used in Documentation~/features/foveatedrendering.md
// This example demonstrates how to set a foveation level and enable dynamic foveation at runtime.
