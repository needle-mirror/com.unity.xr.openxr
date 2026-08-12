using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine.XR.OpenXR.NativeTypes;
using Vector3 = UnityEngine.Vector3;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    class ANDROIDSpatialDiscoveryRaycastTests : MockRuntimeEditorTestBase
    {
        const string k_ExtensionName = "XR_ANDROID_spatial_discovery_raycast";
        const string k_InterceptorName = "xrCreateSpatialRaycastSnapshotANDROID";
        const float k_MaxDistance = 5f;

        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            m_Environment.Settings.RequestUseExtension(k_ExtensionName);
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_entity");
            m_Environment.Settings.RequestUseExtension("XR_EXT_future");
            m_Environment.AddSupportedExtension(k_ExtensionName, 1);
            m_Environment.AddSupportedExtension("XR_EXT_spatial_entity", 1);
            m_Environment.AddSupportedExtension("XR_EXT_future", 1);
        }

        static NativeArray<XrSpatialComponentTypeEXT> CreateRaycastComponents(Allocator allocator = Allocator.Temp)
        {
            var components = new NativeArray<XrSpatialComponentTypeEXT>(2, allocator);
            components[0] = XrSpatialComponentTypeEXT.RaycastResult;
            components[1] = XrSpatialComponentTypeEXT.Bounded2D;
            return components;
        }

        [Test]
        public unsafe void xrCreateSpatialRaycastSnapshotANDROID_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                k_InterceptorName,
                ANDROIDSpatialDiscoveryRaycastMocks.xrCreateSpatialRaycastSnapshotANDROID_Ptr);
            m_Environment.Start();

            var raycastInfo = new XrSpatialRaycastInfoANDROID(
                0,
                0,
                XrVector3f.FromSessionSpaceCoordinates(0, 0, 0),
                XrVector3f.FromSessionSpaceCoordinates(0, 0, -1),
                k_MaxDistance);
            var createInfo = new XrSpatialRaycastSnapshotCreateInfoANDROID(raycastInfo);

            var result = OpenXRNativeApi.xrCreateSpatialRaycastSnapshotANDROID(0, createInfo, out var snapshot);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(ANDROIDSpatialDiscoveryRaycastMocks.snapshotHandle, snapshot);
            Assert.AreEqual(0, ANDROIDSpatialDiscoveryRaycastMocks.lastComponentTypeCount);
            Assert.AreEqual(k_MaxDistance, ANDROIDSpatialDiscoveryRaycastMocks.lastRaycastInfo.maxDistance);
        }

        [Test]
        public unsafe void xrCreateSpatialRaycastSnapshotANDROID_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                k_InterceptorName,
                ANDROIDSpatialDiscoveryRaycastMocks.xrCreateSpatialRaycastSnapshotANDROID_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialRaycastSnapshotANDROID(
                0,
                XrVector3f.FromSessionSpaceCoordinates(0, 0, 0),
                XrVector3f.FromSessionSpaceCoordinates(0, 0, -1),
                k_MaxDistance,
                0,
                null,
                out var snapshot);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(ANDROIDSpatialDiscoveryRaycastMocks.snapshotHandle, snapshot);

            // No component type filter means the runtime includes all hit spatial entities.
            Assert.AreEqual(0, ANDROIDSpatialDiscoveryRaycastMocks.lastComponentTypeCount);

            // The ray is forwarded to the runtime untransformed.
            var direction = ANDROIDSpatialDiscoveryRaycastMocks.lastRaycastInfo.direction;
            Assert.AreEqual(0f, direction.X);
            Assert.AreEqual(0f, direction.Y);
            Assert.AreEqual(-1f, direction.Z);
            Assert.AreEqual(k_MaxDistance, ANDROIDSpatialDiscoveryRaycastMocks.lastRaycastInfo.maxDistance);
        }

        [Test]
        public void xrCreateSpatialRaycastSnapshotANDROID_UsingContext_ForwardsComponentTypes()
        {
            m_Environment.SetFunctionForInterceptor(
                k_InterceptorName,
                ANDROIDSpatialDiscoveryRaycastMocks.xrCreateSpatialRaycastSnapshotANDROID_Ptr);
            m_Environment.Start();

            var components = CreateRaycastComponents();
            var result = OpenXRNativeApi.xrCreateSpatialRaycastSnapshotANDROID(
                0,
                XrVector3f.FromSessionSpaceCoordinates(0, 0, 0),
                XrVector3f.FromSessionSpaceCoordinates(0, 0, -1),
                0,
                components,
                out var snapshot);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(ANDROIDSpatialDiscoveryRaycastMocks.snapshotHandle, snapshot);
            Assert.AreEqual(components.Length, ANDROIDSpatialDiscoveryRaycastMocks.lastComponentTypeCount);

            // A maxDistance of 0 means the runtime treats the ray as unbounded.
            Assert.AreEqual(0f, ANDROIDSpatialDiscoveryRaycastMocks.lastRaycastInfo.maxDistance);
        }

        [Test]
        public unsafe void XrSpatialRaycastInfoANDROID_ConstructorsInitializeType()
        {
            var origin = XrVector3f.FromSessionSpaceCoordinates(1, 2, 3);
            var direction = XrVector3f.FromSessionSpaceCoordinates(0, 0, -1);

            var withNext = new XrSpatialRaycastInfoANDROID(null, 1, 2, origin, direction, k_MaxDistance);
            var withoutNext = new XrSpatialRaycastInfoANDROID(1, 2, origin, direction, k_MaxDistance);
            var unbounded = new XrSpatialRaycastInfoANDROID(1, 2, origin, direction);

            Assert.AreEqual(XrStructureType.SpatialRaycastInfoANDROID, withNext.type);
            Assert.AreEqual(XrStructureType.SpatialRaycastInfoANDROID, withoutNext.type);
            Assert.AreEqual(XrStructureType.SpatialRaycastInfoANDROID, unbounded.type);

            Assert.AreEqual(k_MaxDistance, withoutNext.maxDistance);
            Assert.AreEqual(0f, unbounded.maxDistance, "Omitting maxDistance must produce an unbounded ray.");

            // Coordinates pass through untransformed; the struct stores whatever space the caller specified.
            Assert.AreEqual(origin.Z, withoutNext.origin.Z);
            Assert.AreEqual(direction.Z, withoutNext.direction.Z);
        }

        // TryCreate needs an assigned app space and a predicted display time. The app
        // space is assigned when the input provider starts; the time is cached from xrWaitFrame on the render
        // path. EditMode never renders a frame, so the success path is not reachable here and is covered on
        // device instead. These two tests pin the failure contract for both states that are reachable: the
        // factory must report failure rather than hand back a zeroed space or time, which the runtime would
        // reject as an invalid handle or time.

        [Test]
        [TestCase(false, TestName = "TryCreate_Fails_BeforeStart")]
        [TestCase(true, TestName = "TryCreate_Fails_StartedButNoFrameRendered")]
        public void TryCreate_FailsWhenStateIsUnavailable(bool start)
        {
            if (start)
                m_Environment.Start();

            var created = XrSpatialRaycastInfoANDROID.TryCreate(
                XrVector3f.FromSessionSpaceCoordinates(1, 2, 3),
                XrVector3f.FromSessionSpaceCoordinates(0, 0, -1),
                k_MaxDistance,
                out var info);

            Assert.IsFalse(created, "The factory must not report success without a usable app space and time.");
            Assert.AreEqual(default(XrStructureType), info.type, "A failed call must leave info at default.");
            Assert.AreEqual(0, info.space);
            Assert.AreEqual(0, info.time);
        }

        [Test]
        public unsafe void XrSpatialRaycastSnapshotCreateInfoANDROID_ConstructorsInitializeType()
        {
            var raycastInfo = new XrSpatialRaycastInfoANDROID(
                0, 0, new XrVector3f(), XrVector3f.FromSessionSpaceCoordinates(0, 0, -1));
            var components = CreateRaycastComponents();

            var raycastInfoOnly = new XrSpatialRaycastSnapshotCreateInfoANDROID(raycastInfo);
            var fromArray = new XrSpatialRaycastSnapshotCreateInfoANDROID(components, raycastInfo);
            var fromReadOnly = new XrSpatialRaycastSnapshotCreateInfoANDROID(components.AsReadOnly(), raycastInfo);

            Assert.AreEqual(XrStructureType.SpatialRaycastSnapshotCreateInfoANDROID, raycastInfoOnly.type);
            Assert.AreEqual(XrStructureType.SpatialRaycastSnapshotCreateInfoANDROID, fromArray.type);
            Assert.AreEqual(XrStructureType.SpatialRaycastSnapshotCreateInfoANDROID, fromReadOnly.type);

            // componentTypes is optional, so the raycastInfo-only ctor must produce an empty, null array.
            Assert.AreEqual(0, raycastInfoOnly.componentTypeCount);
            Assert.IsTrue(raycastInfoOnly.componentTypes == null);

            // The NativeArray ctors must agree with the pointer ctor.
            Assert.AreEqual((uint)components.Length, fromArray.componentTypeCount);
            Assert.AreEqual((uint)components.Length, fromReadOnly.componentTypeCount);
            Assert.IsTrue(fromArray.raycastInfo == &raycastInfo);
        }

        [Test]
        public unsafe void XrSpatialComponentRaycastResultListANDROID_ConstructorsAgree()
        {
            var results = new NativeArray<XrSpatialRaycastResultDataANDROID>(3, Allocator.Temp);

            var fromArray = new XrSpatialComponentRaycastResultListANDROID(results);
            var fromReadOnly = new XrSpatialComponentRaycastResultListANDROID(results.AsReadOnly());

            Assert.AreEqual(XrStructureType.SpatialComponentRaycastResultListANDROID, fromArray.type);
            Assert.AreEqual(XrStructureType.SpatialComponentRaycastResultListANDROID, fromReadOnly.type);
            Assert.AreEqual((uint)results.Length, fromArray.raycastResultCount);
            Assert.AreEqual((uint)results.Length, fromReadOnly.raycastResultCount);
            Assert.IsTrue(fromArray.raycastResults == fromReadOnly.raycastResults);
        }

        [Test]
        public unsafe void XrSpatialCapabilityConfigurationDepthRaycastANDROID_InitializesTypeAndCapability()
        {
            var components = CreateRaycastComponents();

            var config = new XrSpatialCapabilityConfigurationDepthRaycastANDROID(components);

            Assert.AreEqual(XrStructureType.SpatialCapabilityConfigurationDepthRaycastANDROID, config.type);
            Assert.AreEqual(XrSpatialCapabilityEXT.DepthRaycast, config.capability);
            Assert.AreEqual((uint)components.Length, config.enabledComponentCount);
            Assert.IsTrue(config.enabledComponents != null);
            Assert.IsNotEmpty(config.ToString());
        }

        [Test]
        public void XrSpatialRaycastResultDataANDROID_EqualityAndHashing()
        {
            var pose = new XrPosef(new Vector3(1, 2, 3), UnityEngine.Quaternion.identity);
            var a = new XrSpatialRaycastResultDataANDROID(pose, 4f);
            var b = new XrSpatialRaycastResultDataANDROID(pose, 4f);
            var differentDistance = new XrSpatialRaycastResultDataANDROID(pose, 5f);

            Assert.AreEqual(a, b);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a, differentDistance);
            Assert.IsFalse(a.Equals(new object()));
        }

        [DllImport("UnityOpenXR", EntryPoint = "EXT_spatial_entity_ValidateStruct")]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern unsafe bool ValidateStruct(XrBaseInStructure* structPtr);

        [Test]
        public unsafe void ValidateStruct_AcceptsDepthRaycastCapabilityConfiguration()
        {
            var components = CreateRaycastComponents();

            var config = new XrSpatialCapabilityConfigurationDepthRaycastANDROID(components);
            var configPtr = Marshal.AllocHGlobal(
                Marshal.SizeOf<XrSpatialCapabilityConfigurationDepthRaycastANDROID>());
            Marshal.StructureToPtr(config, configPtr, false);

            var configPtrs = new NativeArray<IntPtr>(1, Allocator.Temp);
            configPtrs[0] = configPtr;

            var contextCreateInfo = new XrSpatialContextCreateInfoEXT(configPtrs);

            try
            {
                Assert.IsTrue(
                    ValidateStruct((XrBaseInStructure*)&contextCreateInfo),
                    "The native side must accept the depth raycast capability configuration, which also confirms "
                    + "the C# and C struct layouts agree.");
            }
            finally
            {
                Marshal.FreeHGlobal(configPtr);
            }
        }
    }
}
