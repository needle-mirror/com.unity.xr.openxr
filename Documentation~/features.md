---
uid: openxr-features
---
# OpenXR features

OpenXR is an extensible API that can be extended with new features. To facilitate this within the Unity ecosystem, the Unity OpenXR provider supports a feature extension mechanism.

For information about managing the OpenXR features in your project, refer to [Feature management](features/feature-management.md).

For information about the available OpenXR features, refer to:

| OpenXR feature                               | Description |
| :------------------------------------------- | :------------------------------------------- |
| [Meta Quest Support](features/metaquest.md) | Enables Meta Quest support for the OpenXR plug-in. |
| [Composition Layers support](features/compositionlayers.md) | Provides support for rendering high quality images on layer types, such as cylinder, equirect, cube, and more. |
| [XR Performance Settings](features/performance-settings.md) | Lets you provide performance hints to an OpenXR runtime and allows you to get notification when an important aspect of device performance changes. |
| [Foveated Rendering](features/foveatedrendering.md) | An optimization technique that renders peripheral areas at a lower resolution. Similar to [Quad Views](features/quadviews.md). |
| [Subsampled layout](features/subsampledlayout.md) | An optimization technique that can improve [Foveated rendering](xref:openxr-foveated-rendering) performance by optimizing eye texture sampling. |
| [Quad Views](features/quadviews.md) | An optimization technique that renders peripheral areas at a lower resolution. A sub-option of [Foveated Rendering](features/foveatedrendering.md). |
| [Multiview Render Regions](features/multiviewrenderregions.md) | An optimization technique that prevents processing on areas of the screen that are not visible to the user. |
| [Application SpaceWarp](features/spacewarp.md) | An optimization technique that synthesizes every other frame. |
| [Automatic dynamic resolution](features/automaticdynamicresolution.md) | Dynamically adjusts the resolution of your XR project to maintain a stable frame rate and improve graphical performance. |

For information about creating features as part of an OpenXR provider, refer to [Integrate OpenXR features](features/feature-integration.md).
