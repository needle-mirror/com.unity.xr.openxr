---
uid: openxr-temporal-pixel-synthesis
---

# Temporal Pixel Synthesis

Understand how to enable and configure Temporal Pixel Synthesis in your Unity project.

Temporal Pixel Synthesis (TPS) is an image-quality feature for Meta Quest devices that lets the Meta runtime apply temporal anti-aliasing, upscaling, or other temporal reconstruction techniques to the current frame. Your application supplies per-view motion vector data each frame so the runtime can accurately track pixel movement between frames. Depth and stencil data are also used when available.

<a id="prerequisites"></a>

## Prerequisites

To use TPS, your project must meet the following requirements:

* Unity 6 or newer
* Unity OpenXR plugin (`com.unity.xr.openxr`) with a runtime that supports the `XR_META_temporal_pixel_synthesis` extension.
* Universal Render Pipeline (URP)
* Android build target (Meta Quest)
* Use the Vulkan API. This feature isn't available on other graphics APIs. To understand how to choose your graphics API, refer to [Configure graphics APIs](https://docs.unity3d.com/Manual/configure-graphicsAPIs.html).

> [!NOTE]
> If your application runs on any other graphics API, TPS logs an error and is disabled. Your application continues to run without it.

<a id="compatibility"></a>

## Compatibility

Avoid the following settings with TPS:

* URP upscaling filter: Set the URP Asset's **Upscaling Filter** to **Automatic**. Using any other upscaling filter alongside TPS can degrade performance.
* Multisample anti-aliasing (MSAA): Set the URP Asset's **Anti Aliasing (MSAA)** property to **Disabled**. Using MSAA alongside TPS can degrade performance.
* Per-camera anti-aliasing: In the **Rendering** section of each Camera component, set **Anti-aliasing** to **No Anti-aliasing**. This setting is separate from the URP Asset's **Anti Aliasing (MSAA)** property. The Meta runtime performs the temporal resolve, so URP's anti-aliasing is redundant and its jitter reduces image quality.

The validation rule for this setting only checks cameras in scenes that are currently open, so verify any cameras you load or instantiate at runtime.

<a id="enable"></a>

## Enable Temporal Pixel Synthesis

To enable TPS in your project:

1. Open the **Project Settings** window (menu: **Edit** > **Project Settings**).
1. Under **XR Plug-in Management**, select the **OpenXR** settings.
1. Under **OpenXR Feature Groups**, enable the **Meta: Temporal Pixel Synthesis** feature.

When your application starts, the Meta runtime begins pixel synthesis with the default parameters, so no additional scripting is required. To change those defaults, refer to the settings in the following section.

<a id="settings"></a>

## Temporal Pixel Synthesis settings

Use the following settings to configure Temporal Pixel Synthesis:

| **Setting** | **Description** |
| :---------- | :-------------- |
| **Default Fovea Quality** | Quality hint for the central (fovea) region of pixel synthesis. Defaults to **High**. |
| **Default Periphery Quality** | Quality hint for the outer (periphery) region of pixel synthesis. Defaults to **Medium**. |
| **Default Fovea Radius** | Radius hint controlling the boundary between the high-quality central region and the periphery. Defaults to **Medium**. |
| **Default Enable Upscaling** | Whether to request upscaling as part of pixel synthesis. Requires [`PixelSynthesisSupportedFeatureFlags.Upscaling`](xref:UnityEngine.XR.OpenXR.Features.Meta.PixelSynthesisSupportedFeatureFlags). Defaults to disabled. |
| **Default Upscaling Quality** | Quality hint for upscaling. Only used when **Default Enable Upscaling** is enabled. Defaults to **Medium**. |

### Quality levels

The fovea quality, periphery quality, and upscaling quality settings all use the `PixelSynthesisQualityLevel` enum. The following table lists the available values.

| **Value** | **Description** |
| :-------- | :-------------- |
| `Disabled` | No temporal accumulation for this region. |
| `Low` | Prioritizes performance over quality. |
| `Medium` | Balances performance and quality. |
| `High` | Prioritizes quality over performance. |
| `Dynamic` | The runtime dynamically adjusts quality based on headroom. Requires `PixelSynthesisSupportedFeatureFlags.DynamicQuality`. |

### Fovea radius

The [`PixelSynthesisFoveaRadius`](xref:UnityEngine.XR.OpenXR.Features.Meta.PixelSynthesisFoveaRadius) enum controls the size of the high-quality central region:

| **Value** | **Description** |
| :-------- | :-------------- |
| `Small` | Small fovea region — prioritizes performance. |
| `Medium` | Medium fovea region — balances performance and quality. |
| `Large` | Large fovea region — prioritizes quality. |
| `Dynamic` | The runtime dynamically varies the radius. Requires `PixelSynthesisSupportedFeatureFlags.DynamicQuality`. |

<a id="runtime"></a>

## Use Temporal Pixel Synthesis at runtime

Call [`MetaTemporalPixelSynthesisFeature.SetPixelSynthesis`](xref:UnityEngine.XR.OpenXR.Features.Meta.MetaTemporalPixelSynthesisFeature) to supply custom parameters:

| **Method** | **Description** |
| :--------- | :-------------- |
| `SetPixelSynthesis` | Sets pixel synthesis parameters. Cached and reused every frame until called again. |
| `ClearPixelSynthesis` | Resets parameters to their defaults without disabling pixel synthesis. |
| `SetPixelSynthesisEnabled` | Enables or disables pixel synthesis at runtime. Cached parameters are preserved and resume when re-enabled. |

### Frame parameters

Start from [`PixelSynthesisFrameParams.Default`](xref:UnityEngine.XR.OpenXR.Features.Meta.PixelSynthesisFrameParams) to get the default values in the following table. A default-constructed `PixelSynthesisFrameParams` leaves `nearZ` and `farZ` at `0`, which `SetPixelSynthesis` treats as an explicit near and far plane instead of reading [`Camera.main`](xref:UnityEngine.Camera.main).

| **Field** | **Type** | **Description** |
| :-------- | :------- | :-------------- |
| `motionVectorScale` | `Vector2` | Per-axis scale applied to motion vector texel values to convert to normalized device coordinates (NDC). The Y axis is negative because the compositor expects the opposite vertical convention to the render pipeline's output. Default: `new Vector2(1f, -1f)`. |
| `motionVectorOffset` | `Vector2` | Per-axis offset applied to motion vector texel values to convert to NDC. Default: `Vector2.zero`. |
| `appSpaceDeltaPosition` | `Vector3` | Position delta of the app space between frames. Set to `Vector3.zero` when no artificial locomotion occurs. |
| `appSpaceDeltaOrientation` | `Quaternion` | Orientation delta of the app space between frames. Set to `Quaternion.identity` when no artificial locomotion occurred. |
| `nearZ` | `float` | Near clip distance in meters. A negative value makes `SetPixelSynthesis` read the value from `Camera.main.nearClipPlane`, or use `0.1` if the scene has no main camera. Default: `-1`. |
| `farZ` | `float` | Far clip distance in meters. A negative value makes `SetPixelSynthesis` read the value from `Camera.main.farClipPlane`, or use `1000` if the scene has no main camera. Default: `-1`. |
| `stencilBitMask` | `uint` | Bitmask of stencil bits to compare. Set to `0` to omit stencil masking. |
| `stencilValue` | `uint` | Reference stencil value. Pixels where `(stencilBuffer & stencilBitMask) == stencilValue` are excluded from temporal accumulation. |
| `resetAggregations` | `bool` | When `true`, the runtime discards all accumulated temporal state and starts fresh. Set this field to `true` on scene transitions, camera cuts, or teleportation. |
| `enableUpscaling` | `bool` | Whether to request upscaling for this frame. Overrides the **Default Enable Upscaling** setting. Requires runtime support for `PixelSynthesisSupportedFeatureFlags.Upscaling`. Default: `false`. |

## Additional resources

* [Foveated rendering in OpenXR](xref:openxr-foveated-rendering)
* [Application SpaceWarp in OpenXR](xref:openxr-spacewarp)
* [Automatic viewport dynamic resolution](xref:openxr-automatic-dynamic-resolution)
* [Using the Universal Render Pipeline](xref:um-universal-render-pipeline)
