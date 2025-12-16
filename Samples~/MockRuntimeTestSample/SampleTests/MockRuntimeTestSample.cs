using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.TestTools;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.TestTooling;

using XrInstance = System.UInt64;
using XrSystemId = System.UInt64;
using XrFormFactor = System.UInt32;

namespace UnityEngine.XR.OpenXR.Samples.MockRuntimeTestApi
{
    unsafe struct XrSystemPropertiesHeader
    {
        internal XrStructureType type;
        internal void* next;
    }

    unsafe struct XrSystemGetInfo
    {
        internal XrStructureType type;
        internal void* next;
        internal XrFormFactor formFactor;
    }

    unsafe delegate XrResult GetSystemProperties_Delegate(XrSystemPropertiesHeader* systemPropertiesStruct);
    unsafe delegate XrResult GetSystem_Delegate(XrInstance instance, XrSystemGetInfo* getInfo, XrSystemId* systemId);

    internal class LowLevelOpenXRTestSample
    {
        static readonly string[] s_LifecycleCalls =
        {
            "OnInstanceCreate",
            "OnSessionCreate",
            "OnSessionBegin",
            "OnSessionEnd",
            "OnSessionDestroy",
            "OnSessionExiting",
            "OnSubsystemCreate",
            "OnSubsystemStart",
            "OnSubsystemStop",
            "OnSubsystemDestroy",
            "OnSessionStateChange",
        };

        const uint XR_FB_spatial_entity_SPEC_VERSION = 3;
        const uint XR_TYPE_SYSTEM_GET_INFO = 4;
        const uint XR_TYPE_SYSTEM_USER_PRESENCE_PROPERTIES_EXT = 1000470001;
        const uint XR_TYPE_EVENT_DATA_USER_PRESENCE_CHANGED_EXT = 1000470000;
        internal const int EventDataBufferSize = 128;

        static uint s_NumTimesMockSysPropertiesCalled;

        static unsafe IntPtr GetSysProperties_UserPresence_MockCallback() =>
            Marshal.GetFunctionPointerForDelegate((GetSystemProperties_Delegate)SysProperties_UserPresence_MockCallback);

        [MonoPInvokeCallback(typeof(GetSystemProperties_Delegate))]
        static unsafe XrResult SysProperties_UserPresence_MockCallback(XrSystemPropertiesHeader* systemPropertiesStruct)
        {
            s_NumTimesMockSysPropertiesCalled++;
            return XrResult.Success;
        }

        static uint s_NumTimesMockGetSystemCalled;

        static unsafe IntPtr GetSystem_MockCallback() =>
            Marshal.GetFunctionPointerForDelegate((GetSystem_Delegate)System_MockCallback);

        [MonoPInvokeCallback(typeof(GetSystem_Delegate))]
        static unsafe XrResult System_MockCallback(XrInstance instance, XrSystemGetInfo* getInfo, XrSystemId* systemId)
        {
            s_NumTimesMockGetSystemCalled++;
            *systemId = 2;
            return XrResult.Success;
        }

        unsafe struct XrEventDataUserPresenceChangedEXT
        {
            /* XrStructureType */
            public uint type;
            /* const void* */
            public void* next;
            /* XrSession */
            public ulong session;
            /* XrBool32 */
            public uint isUserPresent;
        }

        /// <summary>
        /// Tests below are for Mock OpenXR Environment support for extensions test methods
        /// </summary>
        /// <returns></returns>
#if !OPENXR_USE_KHRONOS_LOADER
        // // the Android Generic Loader is *not* supported by this API
        [UnityTest]
        public IEnumerator TestAddSupportedExtension()
        {
            using (var mockRuntimeEnvironment = MockOpenXREnvironment.CreateEnvironment())
            {
                // Request extension that Mockruntime doesn't support by default.
                mockRuntimeEnvironment.Settings.RequestUseExtension("XR_FB_spatial_entity");
                // Enable extension
                mockRuntimeEnvironment.AddSupportedExtension("XR_FB_spatial_entity", XR_FB_spatial_entity_SPEC_VERSION);

                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(1);

                Assert.IsTrue(OpenXRRuntime.IsExtensionEnabled("XR_FB_spatial_entity"), "XR_FB_spatial_entity should be enabled.");
                Assert.IsFalse(OpenXRRuntime.IsExtensionEnabled("XR_META_spatial_entity_discovery"), "XR_META_spatial_entity_discovery shouldn't be enabled.");

                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(1);
            }
        }
#endif

        /// <summary>
        /// Tests that mocked system properties get methods are called when desired.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestSetSysPropsFunctionForXrStructureType()
        {
            using (var mockRuntimeEnvironment = MockOpenXREnvironment.CreateEnvironment())
            {
                // our "XR_TYPE_SYSTEM_USER_PRESENCE_PROPERTIES_EXT" mock should not be called at the start
                s_NumTimesMockSysPropertiesCalled = 0;
                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(0.1F);
                Assert.IsTrue(s_NumTimesMockSysPropertiesCalled == 0);
                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(0.1F);

                // our "XR_TYPE_SYSTEM_USER_PRESENCE_PROPERTIES_EXT" mock SHOULD be called when SET
                mockRuntimeEnvironment.SetSysPropertiesFunctionForXrStructureType(
                    XR_TYPE_SYSTEM_USER_PRESENCE_PROPERTIES_EXT,
                    GetSysProperties_UserPresence_MockCallback());
                s_NumTimesMockSysPropertiesCalled = 0;
                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(0.1F);
                Assert.IsTrue(s_NumTimesMockSysPropertiesCalled > 0);
                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(0.1F);

                // our "XR_TYPE_SYSTEM_USER_PRESENCE_PROPERTIES_EXT" mock SHOULD NOT be called when CLEARED
                mockRuntimeEnvironment.SetSysPropertiesFunctionForXrStructureType(
                    XR_TYPE_SYSTEM_USER_PRESENCE_PROPERTIES_EXT,
                    IntPtr.Zero);
                s_NumTimesMockSysPropertiesCalled = 0;
                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(0.1F);
                Assert.IsTrue(s_NumTimesMockSysPropertiesCalled == 0);
                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(0.1F);
            }
        }

        /// <summary>
        /// Tests that mocked OpenXR function pointer methods are called when desired.
        /// </summary>
        [UnityTest]
        public IEnumerator TestSetFunctionForInterceptor()
        {
            using (var mockRuntimeEnvironment = MockOpenXREnvironment.CreateEnvironment())
            {
                // our "xrGetSystem" mock should not be called at the start
                Console.WriteLine("TestSetFunctionForInterceptor() first test");
                s_NumTimesMockGetSystemCalled = 0;
                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(0.1F);
                Assert.IsTrue(s_NumTimesMockGetSystemCalled == 0);
                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(0.1F);

                // our "xrGetSystem" mock SHOULD be called when SET
                Console.WriteLine("TestSetFunctionForInterceptor() second test");
                mockRuntimeEnvironment.SetFunctionForInterceptor(
                    "xrGetSystem", GetSystem_MockCallback());
                s_NumTimesMockGetSystemCalled = 0;
                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(0.1F);
                Assert.IsTrue(s_NumTimesMockGetSystemCalled > 0);
                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(0.1F);

                // our "xrGetSystem" mock SHOULD NOT be called when CLEARED
                Console.WriteLine("TestSetFunctionForInterceptor() third test");
                mockRuntimeEnvironment.SetFunctionForInterceptor(
                    "xrGetSystem", IntPtr.Zero);
                s_NumTimesMockGetSystemCalled = 0;
                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(0.1F);
                Assert.IsTrue(s_NumTimesMockGetSystemCalled == 0);
                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(0.1F);
            }
        }

        /// <summary>
        /// Tests below are for Mock OpenXR Environment support for EnqueueMockEventData and ProcessEventQueue
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestEventQueueAPIs()
        {
            using (var mockRuntimeEnvironment = MockOpenXREnvironment.CreateEnvironment())
            {
                // Utilize unit test for XR_EXT_user_presence extension - UserPresence in OpenXRRuntimeTests.cs
                mockRuntimeEnvironment.Settings.RequestUseExtension("XR_EXT_user_presence");

                mockRuntimeEnvironment.Start();
                yield return new WaitForSeconds(1);

                List<InputDevice> hmdDevices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, hmdDevices);
                Assert.That(hmdDevices.Count > 0, Is.True);

                //Queue Event to set userPresent to False.
                var userPresenceEventData = new XrEventDataUserPresenceChangedEXT
                {
                    type = XR_TYPE_EVENT_DATA_USER_PRESENCE_CHANGED_EXT,
                    next = null,
                    isUserPresent = 0 // Set userPresent to False
                };
                IntPtr unmanagedEventData = Marshal.AllocHGlobal(EventDataBufferSize);
                Marshal.StructureToPtr(userPresenceEventData, unmanagedEventData, false);
                mockRuntimeEnvironment.EnqueueMockEventData(unmanagedEventData);
                Marshal.FreeHGlobal(unmanagedEventData);
                mockRuntimeEnvironment.ProcessEventQueue();
                yield return new WaitForSeconds(1);

                bool hasValue = hmdDevices[0].TryGetFeatureValue(CommonUsages.userPresence, out bool isUserPresent);
                Assert.That(hasValue, Is.True);
                Assert.That(isUserPresent, Is.False);
                // Queue Event to set userPresent to True.
                userPresenceEventData = new XrEventDataUserPresenceChangedEXT
                {
                    type = XR_TYPE_EVENT_DATA_USER_PRESENCE_CHANGED_EXT,
                    next = null,
                    isUserPresent = 1 // Set userPresent to True
                };
                unmanagedEventData = Marshal.AllocHGlobal(EventDataBufferSize);
                Marshal.StructureToPtr(userPresenceEventData, unmanagedEventData, false);
                mockRuntimeEnvironment.EnqueueMockEventData(unmanagedEventData);
                Marshal.FreeHGlobal(unmanagedEventData);
                mockRuntimeEnvironment.ProcessEventQueue();
                yield return new WaitForSeconds(1);

                hasValue = hmdDevices[0].TryGetFeatureValue(CommonUsages.userPresence, out isUserPresent);
                Assert.That(hasValue, Is.True);
                Assert.That(isUserPresent, Is.True);

                mockRuntimeEnvironment.Stop();
                yield return new WaitForSeconds(1);
            }
        }

        List<string> GetNamesOfActiveLoaders()
        {
            var activeLoaders = XRGeneralSettings.Instance.Manager.activeLoaders;

            if (activeLoaders != null && activeLoaders.Count > 0)
            {
                return XRGeneralSettings.Instance.Manager.activeLoaders
                    .Where(loader => loader != null)
                    .Select(loader => loader.name)
                    .OrderBy(name => name)
                    .ToList();
            }

            return new List<string>();
        }
    }
}
