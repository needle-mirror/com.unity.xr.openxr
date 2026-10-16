using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Mock;
using UnityEngine.XR.OpenXR.Tests;
using Assert = UnityEngine.Assertions.Assert;

[assembly: UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]

namespace UnityEditor.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "Loader")]
    class XRLoaderLifecycleTests : OpenXRLoaderSetup
    {
        [Test]
        [Description("Feature callbacks fire in order HookGetInstanceProcAddr, OnSubsystemCreate, " +
                     "OnSubsystemStart on start, and OnSubsystemStop before OnSubsystemDestroy on shutdown.")]
        public void FullLifecycleOrder()
        {
            bool subsystemCreate = false;
            bool subsystemStart = false;
            bool hookInstanceProc = false;
            MockRuntime.Instance.TestCallback = (methodName, _) =>
            {
                switch (methodName)
                {
                    case nameof(OpenXRFeature.HookGetInstanceProcAddr):
                        Assert.IsFalse(subsystemCreate);
                        Assert.IsFalse(subsystemStart);
                        hookInstanceProc = true;
                        break;

                    case nameof(OpenXRFeature.OnSubsystemCreate):
                        Assert.IsTrue(hookInstanceProc);
                        Assert.IsFalse(subsystemStart);
                        subsystemCreate = true;
                        break;

                    case nameof(OpenXRFeature.OnSubsystemStart):
                        Assert.IsTrue(hookInstanceProc);
                        Assert.IsTrue(subsystemCreate);
                        subsystemStart = true;
                        break;
                }

                return true;
            };

            InitializeAndStart();

            ProcessOpenXRMessageLoop();

            Assert.IsTrue(hookInstanceProc);
            Assert.IsTrue(subsystemCreate);
            Assert.IsTrue(subsystemStart);

            bool subsystemStop = false;
            bool subsystemDestroy = false;
            MockRuntime.Instance.TestCallback = (methodName, _) =>
            {
                switch (methodName)
                {
                    case nameof(OpenXRFeature.OnSubsystemStop):
                        Assert.IsFalse(subsystemDestroy);
                        subsystemStop = true;
                        break;

                    case nameof(OpenXRFeature.OnSubsystemDestroy):
                        Assert.IsTrue(subsystemStop);
                        subsystemDestroy = true;
                        break;
                }
                return true;
            };

            StopAndShutdown();

            Assert.IsTrue(subsystemStop);
            Assert.IsTrue(subsystemDestroy);
        }

        [Test]
        [Description("A non-required feature whose OnInstanceCreate succeeds stays enabled, keeps receiving " +
                     "callbacks, and the loader comes up.")]
        public void InstanceCreate() => TestInstanceCreate(true, false);

        [Test]
        [Description("A non-required feature whose OnInstanceCreate fails is disabled and receives no " +
                     "further callbacks, while the loader still comes up.")]
        public void InstanceCreateFail() => TestInstanceCreate(false, false);

        [Test]
        [Description("A required feature whose OnInstanceCreate succeeds lets the loader come up.")]
        public void InstanceCreateRequired() => TestInstanceCreate(true, true);

        [Test]
        [Description("A required feature whose OnInstanceCreate fails aborts loader start, leaving no " +
                     "loader instance.")]
        public void InstanceCreateFailRequired() => TestInstanceCreate(false, true);

        public void TestInstanceCreate(bool result, bool required)
        {
            Loader.DisableValidationChecksOnEnteringPlaymode = true;
            bool instanceCreated = false;
            bool hookInstanceProcAddr = false;
            bool other = false;
            MockRuntime.Instance.TestCallback = (methodName, param) =>
            {
                switch (methodName)
                {
                    case nameof(OpenXRFeature.OnInstanceCreate):
                        instanceCreated = true;
                        return result;

                    case nameof(OpenXRFeature.HookGetInstanceProcAddr):
                        hookInstanceProcAddr = true;
                        break;

                    default:
                        other = true;
                        break;
                }

                return true;
            };

            MockRuntime.Instance.required = required;
            InitializeAndStart();

            Assert.IsTrue(instanceCreated);
            Assert.IsTrue(hookInstanceProcAddr);

            if (required && !result)
                Assert.IsNull(OpenXRLoaderBase.Instance);
            else
                Assert.IsNotNull(OpenXRLoaderBase.Instance);

            // A feature that fails that is not required should be disabled
            if (!result && !required)
                Assert.IsFalse(MockRuntime.Instance.enabled);

            StopAndShutdown();

            if (result)
                Assert.IsTrue(other);
            else
                // A feature that fails initialize should have no further callbacks
                Assert.IsFalse(other);
        }

        [Test]
        [Description("Shutting down a loader that was never initialized invokes no feature callbacks.")]
        public void DeinitWithoutInit()
        {
            bool callback = false;
            MockRuntime.Instance.TestCallback = (_, _) =>
            {
                callback = true;
                return true;
            };

            StopAndShutdown();

            Assert.IsFalse(callback);
        }
    }
}
