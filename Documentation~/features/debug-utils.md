---
uid: openxr-debug-utils
---

# Debug Utils

Enable the **Debug Utils** feature to log validation errors, performance warnings, and other diagnostics from the OpenXR runtime. The **Debug Utils** feature enables the OpenXR `XR_EXT_debug_utils` extension and routes runtime diagnostic messages to the Unity **Console**.

> [!NOTE]
> The target XR platform must support the `XR_EXT_debug_utils` extension. If the extension is not available on the active runtime, the feature will not be activated. Be sure to confirm that your target platform supports the extension.

For more information about the `XR_EXT_debug_utils` extension, refer to the [OpenXR Specification](https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_debug_utils).

## Enable the Debug Utils feature

To use the Debug Utils feature, you must enable it in the **OpenXR Project Settings**:

1. Open the **Project Settings** window (menu: **Edit &gt; Project Settings**).
2. Expand the **XR Plug-in Management** section, if necessary.
3. Select the **OpenXR** area under **XR Plug-in Management**.
4. Choose the **Platform Build Target** from the tabs along the top of the **OpenXR** settings page.
5. Check the box next to **Debug Utils** (in the **All Features** group) to enable the feature.
6. Repeat for other platforms, if desired.

> [!NOTE]
> Debug Utils is supported on **Android** and **Standalone** build targets.

## Configure message filtering

Once enabled, the Debug Utils feature exposes filter settings in the feature's inspector. Click the gear icon next to the feature to open its settings.

### Message Severity

Controls which severity levels of OpenXR messages are forwarded to the Unity console. This is a bitmask — you can enable any combination of levels.

| Severity | Description |
|---|---|
| **Verbose** | Detailed diagnostic information. |
| **Info** | Informational messages about normal operations. |
| **Warning** | Potential issues or non-optimal API usage. |
| **Error** | Failures or invalid operations. |

All severity levels are enabled by default.

### Message Type

Controls which categories of OpenXR messages are forwarded to the Unity console. This is a bitmask — you can enable any combination of types.

| Type | Description |
|---|---|
| **General** | General debug messages not covered by other types. |
| **Validation** | Validation of API usage and parameters. |
| **Performance** | Performance warnings and optimization suggestions. |
| **Conformance** | OpenXR specification conformance messages. |

All message types are enabled by default.

## Runtime behavior

When the Debug Utils feature is active, OpenXR runtime messages matching the configured severity and type filters are forwarded to `Debug.Log` in the Unity console. Each message is prefixed with `[Debug Utils]` to distinguish it from other log output. Note that all severity levels use `Debug.Log`, so Unity's console log-type filtering (warning/error) won't differentiate between them — use the `[Debug Utils]` prefix to filter in the console instead.

Log message are only received while an active OpenXR session exists.
