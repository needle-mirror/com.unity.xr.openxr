using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.Features.Mock;

namespace UnityEngine.XR.OpenXR.Tests
{
    class OpenXRLatencyOptimizationTests : OpenXRLoaderSetup
    {
        public override void BeforeTest()
        {
            NativeApi.EnableLatencyStats = true;
            base.BeforeTest();
        }

        public override void AfterTest()
        {
            NativeApi.EnableLatencyStats = false;
            base.AfterTest();
        }

        static readonly OpenXRSettings.LatencyOptimization[] s_latencyOptimizations = {
            OpenXRSettings.LatencyOptimization.PrioritizeRendering,
            OpenXRSettings.LatencyOptimization.PrioritizeInputPolling,
        };

        [UnityTest]
        public IEnumerator CheckFrameSyncFunctionsCalledOncePerFrame(
            [ValueSource(nameof(s_latencyOptimizations))] OpenXRSettings.LatencyOptimization priorityStrategy)
        {
            var waitFrameCallCount = 0;
            var beginFrameCallCount = 0;
            var endFrameCallCount = 0;

            MockRuntime.SetFunctionCallback(
                "xrWaitFrame",
                null,
                (_, _) =>
                {
                    waitFrameCallCount++;
                });
            MockRuntime.SetFunctionCallback(
                "xrBeginFrame",
                null,
                (_, _) =>
                {
                    beginFrameCallCount++;
                });
            MockRuntime.SetFunctionCallback(
                "xrEndFrame",
                null,
                (_, _) =>
                {
                    endFrameCallCount++;
                });

            Initialize();
            NativeApi.SetLatencyOptimization(priorityStrategy);
            Start();

            var testFrames = 20;
            var waitForEndFrame = new WaitForEndOfFrame();
            for (var i = 0; i <= testFrames; i++)
            {
                yield return waitForEndFrame;
            }

            Assert.AreEqual(testFrames, waitFrameCallCount, "xrWaitFrame calls doesn't match the frame count");
            Assert.AreEqual(testFrames, beginFrameCallCount, "xrBeginFrame calls doesn't match the frame count");
            // Coroutine wait is called right before the xrEndFrame call, so reduce one from testFrames
            Assert.AreEqual(testFrames - 1, endFrameCallCount, "xrEndFrame calls doesn't match the expected frame count");

            StopAndShutdown();
        }

        [UnityTest]
        public IEnumerator CheckFrameSynchronizationFunctionsAreCalledInOrder(
            [ValueSource(nameof(s_latencyOptimizations))] OpenXRSettings.LatencyOptimization priorityStrategy)
        {
            bool xrWaitFrameCalled = false;
            bool xrBeginFrameCalled = false;

            bool xrWaitFrameCalledMultipleTimes = false;
            bool xrBeginFrameInWrongOrder = false;
            bool xrEndFrameInWrongOrder = false;

            MockRuntime.SetFunctionCallback(
                "xrWaitFrame",
                (name) =>
                {
                    if (xrWaitFrameCalled)
                    {
                        xrWaitFrameCalledMultipleTimes = true;
                        return NativeTypes.XrResult.RuntimeFailure;
                    }

                    xrWaitFrameCalled = true;
                    return NativeTypes.XrResult.Success;
                });
            MockRuntime.SetFunctionCallback(
                "xrBeginFrame",
                (name) =>
                {
                    if (!xrWaitFrameCalled || xrBeginFrameCalled)
                    {
                        xrBeginFrameInWrongOrder = true;
                        return NativeTypes.XrResult.CallOrderInvalid;
                    }

                    xrBeginFrameCalled = true;
                    xrWaitFrameCalled = false;
                    return NativeTypes.XrResult.Success;
                });
            MockRuntime.SetFunctionCallback(
                "xrEndFrame",
                (name) =>
                {
                    if (!xrBeginFrameCalled)
                    {
                        xrEndFrameInWrongOrder = true;
                        return NativeTypes.XrResult.CallOrderInvalid;
                    }

                    xrBeginFrameCalled = false;
                    return NativeTypes.XrResult.Success;
                });

            Initialize();
            NativeApi.SetLatencyOptimization(priorityStrategy);
            Start();

            var waitFrameCoroutine = new WaitForXrFrame();
            for (int i = 0; i < 20; i++)
            {
                if (xrWaitFrameCalledMultipleTimes || xrBeginFrameInWrongOrder || xrEndFrameInWrongOrder)
                {
                    break;
                }
                yield return waitFrameCoroutine;
            }

            Assert.IsFalse(xrWaitFrameCalledMultipleTimes, "xrWaitFrame was called multiple times in a single frame");
            Assert.IsFalse(xrBeginFrameInWrongOrder, "xrBeginFrame was called out of order");
            Assert.IsFalse(xrEndFrameInWrongOrder, "xrEndFrame was called out of order");

            StopAndShutdown();
        }

        static class NativeApi
        {
            const string LibraryName = "UnityOpenXR";

            public static bool EnableLatencyStats
            {
                get => Internal_GetEnableLatencyStats();
                set => Internal_SetEnableLatencyStats(value);
            }

            public static void SetLatencyOptimization(OpenXRSettings.LatencyOptimization latencyOptimization) =>
                Internal_SetLatencyOptimization(latencyOptimization);

            [DllImport(LibraryName, EntryPoint = "NativeConfig_GetEnableLatencyStats")]
            [return: MarshalAs(UnmanagedType.U1)]
            static extern bool Internal_GetEnableLatencyStats();

            [DllImport(LibraryName, EntryPoint = "NativeConfig_SetEnableLatencyStats")]
            static extern void Internal_SetEnableLatencyStats([MarshalAs(UnmanagedType.U1)] bool unityVersion);

            [DllImport(LibraryName, EntryPoint = "NativeConfig_SetLatencyOptimization")]
            static extern void Internal_SetLatencyOptimization(OpenXRSettings.LatencyOptimization latencyOptimzation);
        }
    }
}
