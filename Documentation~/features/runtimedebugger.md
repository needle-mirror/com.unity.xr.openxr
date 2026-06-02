---
uid: openxr-feature-runtime-debugger
---
# Runtime Debugger

The Runtime Debugger is an optional OpenXR feature provided by Unity’s OpenXR plug-in. Use the **Runtime Debugger** feature to inspect how your application communicates with the active OpenXR runtime. This feature is intended for development and troubleshooting workflows, and can help you investigate runtime behavior that is not visible through standard Unity logs alone.

 The Runtime Debugger intercepts OpenXR calls and forwards them over Player Connection to an Editor window for inspection. It relies on Player Connection and an Editor-side debugging window, making it best suited for local testing, issue reproduction, and runtime investigation during development.

## When to use it

Use Runtime Debugger when you want to:

- Inspect OpenXR call flow during application startup, runtime, and shutdown.
- Diagnose runtime-specific issues on a device
- Compare behavior across different OpenXR runtimes
- Gather more detailed debugging information while testing an XR application

## Enable Runtime Debugger

To enable Runtime Debugger in your project:

1. Open **Edit** > **Project Settings**.
2. Select **XR Plug-in Management**.
3. Enable **OpenXR** for your target platform.
4. Open the **OpenXR** settings for that platform.
5. In the available **Features** list, enable **Runtime Debugger**.

If your project targets more than one platform, you must enable OpenXR separately for each platform build target. Use [Project Configuration](xref:openxr-project-config) to do this.

## How it works

- Intercepts OpenXR calls
- Sends them over Player Connection
- Forwards them to an Editor window for debugging and inspection

## Best practices

- Enable Runtime Debugger only when actively debugging or validating runtime behavior.
- Use it alongside Unity’s OpenXR validation and diagnostic information when troubleshooting configuration or compatibility issues.
- Verify that the intended OpenXR runtime is active when testing, since runtime selection can affect observed behavior.
- Disable the feature when it is not needed for investigation or testing.

## Notes

Runtime Debugger is one of the optional features exposed by Unity’s OpenXR package, alongside other feature-based extensions and tools.

## Additional resources

* [RuntimeDebuggerOpenXRFeature](xref:UnityEngine.XR.OpenXR.Features.RuntimeDebugger.RuntimeDebuggerOpenXRFeature)
* [Features](xref:openxr-features)
