---
uid: openxr-composition-layers
---
# Composition Layers Support

The OpenXR Composition Layers feature provides support for rendering high quality images on layer types, such as cylinder, equirect, cube, and more, on platforms like the Quest headsets.
This functionality allows developers to manage visual layers in a flexible, performant way, tailored for XR applications.

Refer to the [Composition Layers](https://docs.unity3d.com/Packages/com.unity.xr.compositionlayers@latest) documentation for information about using composition layers in a scene.

## Installation
To use this feature, install the [Composition Layers package](https://docs.unity3d.com/Packages/com.unity.xr.compositionlayers@1.0/manual/index.html). Follow the steps in the package documentation for installation details.

## Enabling Composition Layers
Once the package is installed, you can enable OpenXR Composition Layers in your Unity project:

1. Open the **Project Settings** window (menu: **Edit > Project Settings**).
2. Select **XR Plug-in Management** from the list of settings on the left.
3. If necessary, enable **OpenXR** in the list of **Plug-in Providers**. Unity installs the OpenXR plug-in, if not already installed.
4. Select the **OpenXR** settings page under **XR Plug-in Management**.
5. Enable the **Composition Layer Support** option in the **OpenXR Feature Groups** area of the **OpenXR** settings.

![OpenXR feature options](../../images/openxr-features.png)

## Custom Layers
In addition to the built-in layer types supported by the Composition Layers feature, you can create and register your own custom composition layers. This is particularly useful when your project requires non-standard visual behavior, optimized layer rendering, or complex interactions in AR/VR environments.

### Custom Layer Example
The following example demonstrates how to create a custom layer handler for a custom layer type. This handler is responsible for managing the lifecycle of the layer, rendering, and other properties.

```c#    
#if UNITY_EDITOR
[UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "OpenXR Custom Layer Handler Example",
    BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA, BuildTargetGroup.Android },
    Company = "Unity",
    Desc = "An example to demonstrate how to enable a handler for a customized composition layer type.",
    DocumentationLink = "",
    FeatureId = "com.unity.openxr.features.customlayerexample",
    OpenxrExtensionStrings = "",
    Version = "1")]
#endif

public class CustomFeature : OpenXRFeature
{
    bool isSubscribed;

    protected override void OnEnable()
    {
        if (OpenXRLayerProvider.isStarted)
            CreateAndRegisterLayerHandler();
        else
        {
            OpenXRLayerProvider.Started += CreateAndRegisterLayerHandler;
            isSubscribed = true;
        }
    }

    protected override void OnDisable()
    {
        if (isSubscribed)
        {
            OpenXRLayerProvider.Started -= CreateAndRegisterLayerHandler;
            isSubscribed = false;
        }
    }

    protected void CreateAndRegisterLayerHandler()
    {
        if (enabled)
        {
            var layerHandler = new CustomLayerHandler();
            OpenXRLayerProvider.RegisterLayerHandler(typeof(CustomQuadLayerData), layerHandler);
        }
    }
}
#endif
```

This code can also be imported from the OpenXR Samples in the Package Manager.

### When to Use Custom Layers
You may want to create a custom layer in situations where:

- You need to implement a specific rendering technique not covered by the standard layer types (such as a specialized quad or cylinder layer).
- Performance optimizations for specific layer interactions are required.
- Create unique visual effects or layer behaviors in your XR application.

By registering custom layer handlers, you can gain full control over how the composition layers are processed and rendered, allowing for more tailored XR experiences.