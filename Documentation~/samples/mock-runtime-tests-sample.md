---
uid: mock-runtime-tests-sample
---
# Mock Runtime Test Suite Sample

Build a low-level XR functional test suite using the Mock OpenXR Environment test tools.

The Mock Runtime Test Suite Sample helps you build functional tests that make use of OpenXR native functions, and demonstrates how to write C# code that can interact with the native code of the Mock Runtime.

This sample and the low-level test tools are intended for advanced developers who create OpenXR features that perform native calls to the OpenXR runtime, or make use of OpenXR extensions.

With this sample, you will learn how to:
- [Set up a test suite using Mock OpenXR Environment API](#set-up-a-test-suite-using-mock-openxr-environment-api).
- [Intercept OpenXR functions with a managed, unsafe C# method](#intercept-openxr-functions-with-a-managed-unsafe-c-method).
- [Mock an OpenXR extension](#mock-an-openxr-extension).
- [Mock system properties that can be queried by native OpenXR functions](#mock-system-properties-that-can-be-queried-by-native-openxr-functions).
- [Enqueue mock events that OpenXR features under test can respond to](#enqueue-mock-events-that-openxr-features-under-test-can-respond-to).

> [!NOTE]
> You don't need an XR-capable device for using this sample, as Mock Runtime stands in as the device. Consider also that this feature only allows code to be tested, as Mock Runtime doesn't support interactive testing.

If you need a more detailed description on the low-level testing tools available in the Unity OpenXR plugin, please refer to [Low-level testing with Mock OpenXR Environment](../mock-environment.md).

## Prerequisites

To use this sample, your project must include the folllowing packages:

1. [Test Framework](https://docs.unity3d.com/6000.2/Documentation/Manual/test-framework/test-framework-introduction.html).
2. [OpenXR Plugin](../index.md), version 1.16.0 or later.

## Initial setup

To install the test suite sample, please follow these instructions:

1. Open the **Package Manager** window.
    - In Unity 6, you can find it in the menu: **Window > Package Management > Package Manager**.
2. In the **Package Manager** window, select the **OpenXR Plugin** package in the list of packages in your project.
3. Select the **Samples** tab to show the list of OpenXR samples.
4. Click the **Import** button for the **Mock Runtime Test Suite Sample** to copy the sample into your project.

To verify the sample installation, run the test suite sample with the Editor's Test Runner:

1. Open the **Test Runner** window.
    - In Unity 6, you can find it in the menu: **Window > General > Test Runner**.
2. In the **Test Runner** window, select the **PlayMode** tab.
3. Select the `LowLevelOpenXRTestSample.dll` tests in the **Test Runner** window.
5. Verify that all tests have passed once the Test Runner completes its execution.

## Set up a test suite using Mock OpenXR Environment API

To use the Unity Test Framework, your Mock OpenXR Environment tests must be created inside a test assembly. The sample includes the `MockRuntimeTestApiSample.asmdef` asset in the directory `SampleTests` to define the required test assembly. All test files inside `SampleTests` are included in the test assembly.

The sample includes the following files in `SampleTests`:

| File | Usage |
| -- | -- |
| `MockRuntimeTestApiSample.asmdef` | Identifies the container directory as a test assembly. References assemblies required for running the tests. |
| `MockRuntimeTestToolingSampleTests.cs` | C# code file that contains the sample's functional tests based on Mock OpenXR Environment. |

Note that the `MockRuntimeTestApiSample.asmdef` includes the following assembly definition references. These are required to write tests that make use of Mock OpenXR Environment API.
- Unity.XR.OpenXR
- Unity.XR.OpenXR.TestTooling
- UnityEngine.TestRunner
- UnityEditor.TestRunner
- Unity.XR.OpenXR.Features.MockRuntime
- Unity.XR.Management

To see a detailed list of steps needed for preparing the test assembly, refer to [Initial setup and execution of a test case](../mock-environment.md#initial-setup-and-execution-of-a-test-case) in the Mock OpenXR Environment section of the OpenXR Plugin manual.

## Intercept OpenXR functions with a managed, unsafe C# method.

The `MockRuntimeTestToolingSamplleTests.cs` file defines the test method, `LowLevelOpenXRTestSample.TestSetFunctionForInterceptor`, to demonstrate how to intercept OpenXR functions from the Mock Runtime so that you can inject test data that the code under test can react to. The OpenXR function to intercepted by the sample test is `xrGetSystem`. This sample test is decorated with a `UnityTest` attribute, which makes the test run as a coroutine.

The function interceptor is `System_MockCallback`, and it increases the variable `s_NumTimesMockGetSystemCalled` each time it is called before returning `XrResult.Success` as required by the OpenXR function. The function interceptor is declared with a `MonoPInvokeCallback` attribute and `static unsafe` keywords to allow it to be called by native code.

The sample test starts by creating a new instance of the `MockOpenXREnvironment` class with the `MockOpenXREnvironment.CreateEnvironment()` method. This object is needed for managing the lifecycle of the Mock Runtime.

The test proceeds to perform three actions:

1. The test starts the `MockOpenXREnvironment` instance and verifies that the function interceptor is not being called by checking the static variable `s_NumTimesMockGetSystemCalled`.
2. Then, the test injects the function interceptor with the method `mockRuntimeEnvironment.SetFunctionForInterceptor("xrGetSystem", GetSystem_MockCallback())`, and makes sure the interceptor is called with the static variable `s_NumTimesMockGetSystemCalled`.
    - The first parameter `xrGetSystem` indicates which function will be intercepted
    - The second parameter `GetSystem_MockCallback()` is a function call that provides the function pointer of the C# method that the Mock Runtime will use to intercept the function.
3. Last, the test removes the injected function interceptor with another call to `mockRuntimeEnvironment.SetFunctionForInterceptor`, but this times passes a `IntPtr.Zero` as the function pointer. This clears the intercept function pointer from the Mock Runtime, so no more calls will be made to the injected test function.

Note that after each call to `mockRuntimeEnvironment.Start` and `mockRuntimeEnvironment.Stop`, the test performs a `yield return new WaitForSeconds(0.1F)`. A yield operation is required to let the Mock Runtime execute, because part of its code is multithreaded and may require some frames to resolve.

For more detailed instructions on mocking OpenXR functions, refer to [Adding mock support for new extensions](../mock-environment.md#adding-mock-support-for-new-extensions).

## Mock an OpenXR extension

Often, a new functionality requires that certain OpenXR extensions be available and enabled in the runtime. The `LowLevelOpenXRTestSample.TestAddSupportedExtension` test demonstrates how you can test such functionality.

The test case prepares this setup by performing the following:
- Using the `MockOpenXREnvironmentSettings.RequestUseExtension` method, the test adds the extension `XR_FB_spatial_entity` to the mocked OpenXR settings, so the Unity OpenXR plugin includes this extension when it starts the XR session.
- The test also adds the extension `XR_FB_spatial_entity` to the Mock Runtime with `MockOpenXREnvironment.AddSupportedExtension`, so the runtime can simulate that the extension is supported and can be enabled.

After the setup, the test starts the Mock Runtime, and validates that the `XR_FB_spatial_entity` extension is enabled, while verifying that another extension `XR_META_spatial_entity_discovery` is not.

You can use this feature, along with function interceptor injection, to completely mock OpenXR extensions alongside their functions. Refer to [Adding mock support for new extensions](../mock-environment.md#adding-mock-support-for-new-extensions) for an in-depth guide on mocking OpenXR extensions.

## Mock system properties that can be queried by native OpenXR functions

The test method `LowLevelOpenXRTestSample.TestSetSysPropsFunctionForXrStructureType` demonstrates how to provide a customized system properties struct to test functionality that may require the XR device to have an specific form factor or hardware components.

The test makes use of the function `SysProperties_UserPresence_MockCallback`, which is injected into the Mock Runtime. Then, when the code under test performs the OpenXR call `xrGetSystemProperties`, the struct returned will be the one provided by the `SysProperties_UserPresence_MockCallback` function.

Refer to [Modifying Mock Runtime's system properties](../mock-environment.md#modifying-mock-runtimes-system-properties) for details on how to further customize system properties.

## Enqueue mock events that OpenXR features under test can respond to

The `LowLevelOpenXRTestSample.TestEventQueueAPIs` test case enqueues an `XrEventDataUserPresenceChangedEXT` event into the Mock Runtime, which in turn sends it over to event callbacks added, which can be verified later through an input device representing the HMD.

This sample test makes use of structs as defined by the OpenXR specification, so these can be marshalled into native representations. Once queued, the test code can query the HMD device to validate if the event was received by the code under test.

An in-depth explanation on using the Mock Runtime's queueing [Queueing OpenXR events](../mock-environment.md#queueing-openxr-events)

## Additional resources

- [Low-level testing with Mock OpenXR Environment](../mock-environment.md)
- [Unity Manual on "Testing your code"](https://docs.unity3d.com/6000.2/Documentation/Manual/test-framework/test-framework-introduction.html)
- [The OpenXR 1.1 Specification](https://registry.khronos.org/OpenXR/specs/1.1/html/xrspec.html)
