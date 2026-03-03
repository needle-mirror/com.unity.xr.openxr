---
uid: openxr-api-layers-overview
---

# API layers overview

A OpenXR API layer is a software component that sits between your application and the OpenXR runtime. An API layer can "intercept" and modify API calls as they move between the application, the runtime, and back. If more than one API layer is active, each is invoked in the order in which they were loaded.

An API layer is implemented as a native library (`.dll` on Windows and `.so` on Android). The OpenXR runtime initializes known API layers when your application launches. A `.json` format manifest file identifies information about the API layer library.

The Unity API layers feature lets you manage OpenXR layers in your Unity XR project.


> [!NOTE]
> For Windows, we automatically include the following API layers from Khronos: Core Validation, API Dump, and Best Practices. Refer to [Import the Core Validation, API Dump Log, and Best Practices layers](#import-included-layers) for more information.

## Enable the API layers feature {#enable-api-layer-feature}

To use the OpenXR layers feature, you must enable the feature in your project's OpenXR settings:

1. Open the **Project Settings** window (menu: **Edit > Project Settings**).
2. Select the **OpenXR** settings page under **XR Plug-in Management**.
3. Select your target platform tab (Android or Standalone).
4. Under **OpenXR Feature Groups**, enable **API Layers**.

After you enable the feature in settings, you can [Manage API layers](xref:openxr-api-layers-api) in your project using the [ApiLayers](xref:UnityEngine.XR.OpenXR.ApiLayers) class.

## Import the Core Validation, API Dump, and Best Practices layers {#import-included-layers}

The Unity OpenXR package includes the Windows versions of the following OpenXR API layers created by Khronos:

* **Core Validation**: Validates that the OpenXR runtime API is used according to the OpenXR specification. Refer to [The Core Validation Layer](https://github.com/KhronosGroup/OpenXR-SDK-Source/blob/main/src/api_layers/README_core_validation.md) for more information.
* **API Dump**: Logs OpenXR commands sent between the application and the OpenXR runtime. Refer to [The API Dump Layer](https://github.com/KhronosGroup/OpenXR-SDK-Source/blob/main/src/api_layers/README_api_dump.md) for more information.
* **Best Practices**: Includes more granular validation for several critical usage patterns that address the most common cross-runtime compatibility issues XR developers encounter. These validations help prevent subtle bugs that can degrade user experience across different hardware and runtime implementations such as Frame Timing, Synchronization, Rendering, and Composition. Refer to [The Best Practices Validation Layer](https://www.khronos.org/blog/new-openxr-validation-layer-helps-developers-build-robustly-portable-xr-applications).

To enable these layers:

1. Locate the API Layers feature and click it's settings (gear icon to the right of the feature).
2. If your build target has support for the pre-included layers then a "Add API Layers from OpenXR Package" button will appear in the settings window.
3. Click this button to import these pre-included layers.

 [!TIP]
> You should not enable these layers in a released application because they can negatively impact performance.

## API layer files

 In order to use an OpenXR API layer in your project, you need the following files:

 * [API layer manifest](#api-layer-manifest): A `.json` file that describes the layer.
 * [API layer library](#api-layer-library): The native library that implements the OpenXR API layer.

When you [Import an API layer](#import), the [ApiLayers](xref:UnityEngine.XR.OpenXR.ApiLayers) class copies these files from your source folder to the correct location within the [Unity API layers directory](#api-layer-folder). The files in your source folder are not affected by any of the [ApiLayers](xref:UnityEngine.XR.OpenXR.ApiLayers) methods.

### Unity API layers directory {#api-layer-folder}

When you import an API layer, Unity **copies** the files into your project structure:

- **Source**: Files from your original location (can be anywhere)
- **Destination**: `Assets/XR/APILayers~/[architecture]/`
  - Example Windows x64: `Assets/XR/APILayers~/x64/`
  - Example Android ARM64: `Assets/XR/APILayers~/arm64/`

Both the JSON manifest and library files are copied, so your original files remain unchanged. The copied files become part of your Unity project (but are not imported as regular assets) and are included in builds.

### API layer manifest {#api-layer-manifest}

Each API layer is defined by a JSON manifest file that describes the layer's properties, including:

*   **name**: The layer identifier (e.g., `XR_APILAYER_LUNARG_api_dump`)
*   **library_path**: Path to the native library file
*   **api_version**: The OpenXR API version supported by the layer
*   **implementation_version**: The layer's version number
*   **description**: Human-readable description of the layer's purpose

### API layer library {#api-layer-library}

Each API layer has an associated library file that is found in the **library_path** section of the manifest. These library files are architecture-specific. You must import separate versions of a layer for each target architecture:

*   **Android**: ARM64, ARMv7
*   **Windows**: x86_64, ARM64

## Layer ordering

The order of API layers in your project determines the sequence in which they intercept OpenXR calls. Layers earlier in the list intercept calls first. This ordering is important when using multiple layers that interact with each other.

