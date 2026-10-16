using System;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;
using XrSpatialEntityIdEXT = System.UInt64;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    [Category("Level.Integration")]
    [Property("Covers", "SpatialEntities")]
    class EXTSpatialEntityTests : MockRuntimeEditorTestBase
    {
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_entity");
            m_Environment.Settings.RequestUseExtension("XR_EXT_future");
            m_Environment.AddSupportedExtension("XR_EXT_spatial_entity", 1);
            m_Environment.AddSupportedExtension("XR_EXT_future", 1);
        }

        [Test]
        [Description("The two-call raw enumeration of spatial capabilities reports the count and then fills " +
                     "the array with PlaneTracking, MarkerTrackingQRCode and Anchor.")]
        public unsafe void xrEnumerateSpatialCapabilities_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilitiesEXT", EXTSpatialEntityMocks.xrEnumerateSpatialCapabilitiesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilitiesEXT(0, 0, 0, out var countOutput, null);
            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(3, countOutput);

            var array = new NativeArray<XrSpatialCapabilityEXT>((int)countOutput, Allocator.Temp);
            result = OpenXRNativeApi.xrEnumerateSpatialCapabilitiesEXT(
                0, 0, countOutput, out countOutput, (XrSpatialCapabilityEXT*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(3, countOutput);
            Assert.AreEqual(XrSpatialCapabilityEXT.PlaneTracking, array[0]);
            Assert.AreEqual(XrSpatialCapabilityEXT.MarkerTrackingQRCode, array[1]);
            Assert.AreEqual(XrSpatialCapabilityEXT.Anchor, array[2]);
        }

        [Test]
        [Description("The NativeArray overload of capability enumeration allocates a three-element array " +
                     "holding the runtime's capabilities.")]
        public void xrEnumerateSpatialCapabilities_Array_AllocatesCorrectly()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilitiesEXT", EXTSpatialEntityMocks.xrEnumerateSpatialCapabilitiesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilitiesEXT(0, 0, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(XrSpatialCapabilityEXT.PlaneTracking, array[0]);
            Assert.AreEqual(XrSpatialCapabilityEXT.MarkerTrackingQRCode, array[1]);
            Assert.AreEqual(XrSpatialCapabilityEXT.Anchor, array[2]);
        }

        [Test]
        [Description("The context overload of the two-call capability enumeration reports the count and " +
                     "fills the array with the runtime's capabilities.")]
        public unsafe void xrEnumerateSpatialCapabilities_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilitiesEXT", EXTSpatialEntityMocks.xrEnumerateSpatialCapabilitiesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilitiesEXT(0, out var countOutput, null);
            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(3, countOutput);

            var array = new NativeArray<XrSpatialCapabilityEXT>((int)countOutput, Allocator.Temp);
            result = OpenXRNativeApi.xrEnumerateSpatialCapabilitiesEXT(
                countOutput, out countOutput, (XrSpatialCapabilityEXT*)array.GetUnsafePtr());

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(3, countOutput);
            Assert.AreEqual(XrSpatialCapabilityEXT.PlaneTracking, array[0]);
            Assert.AreEqual(XrSpatialCapabilityEXT.MarkerTrackingQRCode, array[1]);
            Assert.AreEqual(XrSpatialCapabilityEXT.Anchor, array[2]);
        }

        [Test]
        [Description("The context NativeArray overload of capability enumeration allocates and fills an " +
                     "array with the runtime's capabilities.")]
        public void xrEnumerateSpatialCapabilities_UsingContext_Array_AllocatesCorrectly()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilitiesEXT", EXTSpatialEntityMocks.xrEnumerateSpatialCapabilitiesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilitiesEXT(Allocator.Temp, out var array);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(XrSpatialCapabilityEXT.PlaneTracking, array[0]);
            Assert.AreEqual(XrSpatialCapabilityEXT.MarkerTrackingQRCode, array[1]);
            Assert.AreEqual(XrSpatialCapabilityEXT.Anchor, array[2]);
        }

        [Test]
        [Description("The two-call raw enumeration of a capability's component types reports the count and " +
                     "then fills Bounded2D and PlaneAlignment for plane tracking.")]
        public unsafe void xrEnumerateSpatialCapabilityComponentTypesEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityComponentTypesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityComponentTypesEXT_Ptr);
            m_Environment.Start();

            var types = XrSpatialCapabilityComponentTypesEXT.defaultValue;
            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityComponentTypesEXT(
                0, 0, XrSpatialCapabilityEXT.PlaneTracking, ref types);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, types.componentTypeCountOutput);

            types.componentTypeCapacityInput = types.componentTypeCountOutput;
            var array = new NativeArray<XrSpatialComponentTypeEXT>((int)types.componentTypeCountOutput, Allocator.Temp);
            types.componentTypes = (XrSpatialComponentTypeEXT*)array.GetUnsafePtr();
            result = OpenXRNativeApi.xrEnumerateSpatialCapabilityComponentTypesEXT(
                0, 0, XrSpatialCapabilityEXT.PlaneTracking, ref types);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, types.componentTypeCountOutput);
            Assert.AreEqual(XrSpatialComponentTypeEXT.Bounded2D, types.componentTypes[0]);
            Assert.AreEqual(XrSpatialComponentTypeEXT.PlaneAlignment, types.componentTypes[1]);
        }

        [Test]
        [Description("The NativeArray overload of component-type enumeration allocates and fills an array " +
                     "with the runtime's component types.")]
        public void xrEnumerateSpatialCapabilityComponentTypesEXT_Array_AllocatesCorrectly()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityComponentTypesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityComponentTypesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityComponentTypesEXT(
                0, 0, XrSpatialCapabilityEXT.PlaneTracking, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(XrSpatialComponentTypeEXT.Bounded2D, array[0]);
            Assert.AreEqual(XrSpatialComponentTypeEXT.PlaneAlignment, array[1]);
        }

        [Test]
        [Description("The context overload of the two-call component-type enumeration reports the count and " +
                     "fills the runtime's component types.")]
        public unsafe void xrEnumerateSpatialCapabilityComponentTypesEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityComponentTypesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityComponentTypesEXT_Ptr);
            m_Environment.Start();

            var types = XrSpatialCapabilityComponentTypesEXT.defaultValue;
            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityComponentTypesEXT(
                XrSpatialCapabilityEXT.PlaneTracking, ref types);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(2, types.componentTypeCountOutput);

            types.componentTypeCapacityInput = types.componentTypeCountOutput;
            var array = new NativeArray<XrSpatialComponentTypeEXT>((int)types.componentTypeCountOutput, Allocator.Temp);
            types.componentTypes = (XrSpatialComponentTypeEXT*)array.GetUnsafePtr();
            result = OpenXRNativeApi.xrEnumerateSpatialCapabilityComponentTypesEXT(
                XrSpatialCapabilityEXT.PlaneTracking, ref types);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(2, types.componentTypeCountOutput);
            Assert.AreEqual(XrSpatialComponentTypeEXT.Bounded2D, types.componentTypes[0]);
            Assert.AreEqual(XrSpatialComponentTypeEXT.PlaneAlignment, types.componentTypes[1]);
        }

        [Test]
        [Description("The context NativeArray overload of component-type enumeration allocates and fills an " +
                     "array with the runtime's component types.")]
        public void xrEnumerateSpatialCapabilityComponentTypesEXT_UsingContext_Array_AllocatesCorrectly()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityComponentTypesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityComponentTypesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityComponentTypesEXT(
                XrSpatialCapabilityEXT.PlaneTracking, Allocator.Temp, out var array);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(XrSpatialComponentTypeEXT.Bounded2D, array[0]);
            Assert.AreEqual(XrSpatialComponentTypeEXT.PlaneAlignment, array[1]);
        }

        [Test]
        [Description("The two-call raw enumeration of a capability's features reports the count and then " +
                     "fills the QR-code marker tracking features.")]
        public unsafe void xrEnumerateSpatialCapabilityFeaturesEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityFeaturesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityFeaturesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityFeaturesEXT(
                0, 0, XrSpatialCapabilityEXT.MarkerTrackingQRCode, 0, out var featureCount, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, featureCount);

            var array = new NativeArray<XrSpatialCapabilityFeatureEXT>((int)featureCount, Allocator.Temp);
            result = OpenXRNativeApi.xrEnumerateSpatialCapabilityFeaturesEXT(
                0,
                0,
                XrSpatialCapabilityEXT.MarkerTrackingQRCode,
                featureCount,
                out featureCount,
                (XrSpatialCapabilityFeatureEXT*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, featureCount);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingFixedSizeMarkers, array[0]);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingStaticMarkers, array[1]);
        }

        [Test]
        [Description("The NativeArray overload of capability-feature enumeration allocates and fills an " +
                     "array with the runtime's features.")]
        public void xrEnumerateSpatialCapabilityFeaturesEXT_Array_AllocatesCorrectly()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityFeaturesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityFeaturesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityFeaturesEXT(
                0, 0, XrSpatialCapabilityEXT.MarkerTrackingQRCode, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingFixedSizeMarkers, array[0]);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingStaticMarkers, array[1]);
        }

        [Test]
        [Description("The context overload of the two-call capability-feature enumeration reports the count " +
                     "and fills the runtime's features.")]
        public unsafe void xrEnumerateSpatialCapabilityFeaturesEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityFeaturesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityFeaturesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityFeaturesEXT(
                XrSpatialCapabilityEXT.MarkerTrackingQRCode, 0, out var featureCount, null);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(2, featureCount);

            var array = new NativeArray<XrSpatialCapabilityFeatureEXT>((int)featureCount, Allocator.Temp);
            result = OpenXRNativeApi.xrEnumerateSpatialCapabilityFeaturesEXT(
                XrSpatialCapabilityEXT.MarkerTrackingQRCode,
                featureCount,
                out featureCount,
                (XrSpatialCapabilityFeatureEXT*)array.GetUnsafePtr());

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(2, featureCount);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingFixedSizeMarkers, array[0]);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingStaticMarkers, array[1]);
        }

        [Test]
        [Description("The context NativeArray overload of capability-feature enumeration allocates and fills " +
                     "an array with the runtime's features.")]
        public void xrEnumerateSpatialCapabilityFeaturesEXT_UsingContext_Array_AllocatesCorrectly()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialCapabilityFeaturesEXT",
                EXTSpatialEntityMocks.xrEnumerateSpatialCapabilityFeaturesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialCapabilityFeaturesEXT(
                XrSpatialCapabilityEXT.MarkerTrackingQRCode, Allocator.Temp, out var array);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingFixedSizeMarkers, array[0]);
            Assert.AreEqual(XrSpatialCapabilityFeatureEXT.MarkerTrackingStaticMarkers, array[1]);
        }

        [Test]
        [Description("The raw async spatial-context create binding returns the runtime's result and future " +
                     "handle.")]
        public void xrCreateSpatialContextAsyncEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialContextAsyncEXT", EXTSpatialEntityMocks.xrCreateSpatialContextAsyncEXT_Ptr);
            m_Environment.Start();

            // Can't create a valid SpatialContextCreateInfo because there are no capabilities in EXT_spatial_entity
            var createInfo = new XrSpatialContextCreateInfoEXT(
                new NativeArray<IntPtr>(1, Allocator.Temp));

            var result = OpenXRNativeApi.xrCreateSpatialContextAsyncEXT(0, in createInfo, out var future);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(123456, future);
        }

        [Test]
        [Description("The context overload of the async spatial-context create returns success and the " +
                     "runtime's future handle.")]
        public void xrCreateSpatialContextAsyncEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialContextAsyncEXT", EXTSpatialEntityMocks.xrCreateSpatialContextAsyncEXT_Ptr);
            m_Environment.Start();

            // Can't create a valid SpatialContextCreateInfo because there are no capabilities in EXT_spatial_entity
            var createInfo = new XrSpatialContextCreateInfoEXT(
                new NativeArray<IntPtr>(1, Allocator.Temp));
            var result = OpenXRNativeApi.xrCreateSpatialContextAsyncEXT(in createInfo, out var future);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(123456, future);
        }

        [Test]
        [Description("Completing a spatial-context future yields a success futureResult and the runtime's " +
                     "context handle.")]
        public void xrCreateSpatialContextCompleteEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialContextCompleteEXT", EXTSpatialEntityMocks.xrCreateSpatialContextCompleteEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialContextCompleteEXT(0, 123, out var completion);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(123456, completion.spatialContext);
        }

        [Test]
        [Description("The context overload of the spatial-context completion yields a success futureResult " +
                     "and the runtime's context handle.")]
        public void xrCreateSpatialContextCompleteEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialContextCompleteEXT", EXTSpatialEntityMocks.xrCreateSpatialContextCompleteEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialContextCompleteEXT(0, out var completion);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(123456, completion.spatialContext);
        }

        [Test]
        [Description("Destroying a spatial context through the binding returns the runtime's success result.")]
        public void xrDestroySpatialContextEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrDestroySpatialContextEXT", EXTSpatialEntityMocks.xrDestroySpatialContextEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrDestroySpatialContextEXT(0);

            Assert.AreEqual(XrResult.Success, result);
        }

        [Test]
        [Description("Creating a spatial entity from an id through the raw binding returns the runtime's " +
                     "entity handle.")]
        public void xrCreateSpatialEntityFromIdEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialEntityFromIdEXT", EXTSpatialEntityMocks.xrCreateSpatialEntityFromIdEXT_Ptr);
            m_Environment.Start();

            var createInfo = new XrSpatialEntityFromIdCreateInfoEXT(123);
            var result = OpenXRNativeApi.xrCreateSpatialEntityFromIdEXT(0, in createInfo, out var entity);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(456, entity);
        }

        [Test]
        [Description("The convenience overload taking a bare entity id returns the runtime's entity handle.")]
        public void xrCreateSpatialEntityFromIdEXT_ConvenienceOverload_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialEntityFromIdEXT", EXTSpatialEntityMocks.xrCreateSpatialEntityFromIdEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialEntityFromIdEXT(123, 123, out var entity);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(456, entity);
        }

        [Test]
        [Description("Destroying a spatial entity through the binding returns the runtime's success result.")]
        public void xrDestroySpatialEntityEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrDestroySpatialEntityEXT", EXTSpatialEntityMocks.xrDestroySpatialEntityEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrDestroySpatialEntityEXT(123);

            Assert.AreEqual(XrResult.Success, result);
        }

        [Test]
        [Description("The raw async discovery-snapshot create binding accepts a component-type list and " +
                     "returns the runtime's future handle.")]
        public unsafe void xrCreateSpatialDiscoverySnapshotAsyncEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialDiscoverySnapshotAsyncEXT",
                EXTSpatialEntityMocks.xrCreateSpatialDiscoverySnapshotAsyncEXT_Ptr);
            m_Environment.Start();

            var componentTypes = new NativeArray<XrSpatialComponentTypeEXT>(2, Allocator.Temp);
            componentTypes[0] = XrSpatialComponentTypeEXT.Bounded2D;
            componentTypes[1] = XrSpatialComponentTypeEXT.PlaneAlignment;

            var createInfo = new XrSpatialDiscoverySnapshotCreateInfoEXT(
                2, (XrSpatialComponentTypeEXT*)componentTypes.GetUnsafePtr());

            var result = OpenXRNativeApi.xrCreateSpatialDiscoverySnapshotAsyncEXT(0, in createInfo, out var future);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(123456, future);
        }

        [Test]
        [Description("Completing a discovery-snapshot future through the raw binding yields a success " +
                     "futureResult and the runtime's snapshot handle.")]
        public void xrCreateSpatialDiscoverySnapshotCompleteEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialDiscoverySnapshotCompleteEXT",
                EXTSpatialEntityMocks.xrCreateSpatialDiscoverySnapshotCompleteEXT_Ptr);
            m_Environment.Start();

            var completionInfo = new XrCreateSpatialDiscoverySnapshotCompletionInfoEXT(0, 0, 0);
            var result = OpenXRNativeApi.xrCreateSpatialDiscoverySnapshotCompleteEXT(
                123, in completionInfo, out var completion);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(123456, completion.snapshot);
        }

        [Test]
        [Description("The context overload of the discovery-snapshot completion yields a success " +
                     "futureResult and the runtime's snapshot handle.")]
        public void xrCreateSpatialDiscoverySnapshotCompleteEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialDiscoverySnapshotCompleteEXT",
                EXTSpatialEntityMocks.xrCreateSpatialDiscoverySnapshotCompleteEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialDiscoverySnapshotCompleteEXT(
                123, 0, out var completion);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(123456, completion.snapshot);
        }

        [Test]
        [Description("The two-call component-data query reports entity id and state counts, then fills the " +
                     "entity id and Tracking state the runtime returns.")]
        public unsafe void xrQuerySpatialComponentDataEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrQuerySpatialComponentDataEXT",
                EXTSpatialEntityMocks.xrQuerySpatialComponentDataEXT_Ptr);
            m_Environment.Start();

            var componentTypes = new NativeArray<XrSpatialComponentTypeEXT>(2, Allocator.Temp);
            componentTypes[0] = XrSpatialComponentTypeEXT.Bounded2D;
            componentTypes[1] = XrSpatialComponentTypeEXT.PlaneAlignment;

            var queryCondition = new XrSpatialComponentDataQueryConditionEXT(componentTypes);
            var queryResult = XrSpatialComponentDataQueryResultEXT.defaultValue;
            var result = OpenXRNativeApi.xrQuerySpatialComponentDataEXT(123, queryCondition, ref queryResult);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(1, queryResult.entityIdCountOutput);
            Assert.AreEqual(1, queryResult.entityStateCountOutput);

            var entityIds = new NativeArray<XrSpatialEntityIdEXT>(
                checked((int)queryResult.entityIdCountOutput), Allocator.Temp);

            var entityStates = new NativeArray<XrSpatialEntityTrackingStateEXT>(
                checked((int)queryResult.entityStateCountOutput), Allocator.Temp);

            queryResult.entityIdCapacityInput = queryResult.entityIdCountOutput;
            queryResult.entityStateCapacityInput = queryResult.entityStateCountOutput;
            queryResult.entityIds = (XrSpatialEntityIdEXT*)entityIds.GetUnsafePtr();
            queryResult.entityStates = (XrSpatialEntityTrackingStateEXT*)entityStates.GetUnsafePtr();
            result = OpenXRNativeApi.xrQuerySpatialComponentDataEXT(123, queryCondition, ref queryResult);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(1, queryResult.entityIdCountOutput);
            Assert.AreEqual(1, queryResult.entityStateCountOutput);
            Assert.AreEqual(5555, queryResult.entityIds[0]);
            Assert.AreEqual(XrSpatialEntityTrackingStateEXT.Tracking, queryResult.entityStates[0]);
        }

        [Test]
        [Description("The two-call raw string-buffer getter reports the byte count and then fills the " +
                     "runtime's UTF-8 string.")]
        public unsafe void xrGetSpatialBufferStringEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferStringEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferStringEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferStringEXT(0, getInfo, 0, out var bufferCountOutput, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, bufferCountOutput);

            var array = new NativeArray<byte>(13, Allocator.Temp);
            result = OpenXRNativeApi.xrGetSpatialBufferStringEXT(
                0, getInfo, bufferCountOutput, out bufferCountOutput, (byte*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, bufferCountOutput);
            Assert.AreEqual(
                "Hello, world.", Encoding.UTF8.GetString((byte*)array.GetUnsafePtr(), (int)bufferCountOutput));
        }

        [Test]
        [Description("The NativeArray overload of the string-buffer getter allocates and fills the runtime's " +
                     "UTF-8 bytes.")]
        public void xrGetSpatialBufferStringEXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferStringEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferStringEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferStringEXT(0, getInfo, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, array.Length);
            Assert.AreEqual("Hello, world.", Encoding.UTF8.GetString(array.AsReadOnlySpan()));
        }

        [Test]
        [Description("The convenience overload of the string-buffer getter taking a bare buffer id allocates " +
                     "and fills the runtime's UTF-8 bytes.")]
        public void xrGetSpatialBufferStringEXT_NativeArray_Convenience_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferStringEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferStringEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrGetSpatialBufferStringEXT(0, 123, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, array.Length);
            Assert.AreEqual("Hello, world.", Encoding.UTF8.GetString(array.AsReadOnlySpan()));
        }

        [Test]
        [Description("The two-call raw uint8 buffer getter reports the count and then fills the runtime's " +
                     "bytes.")]
        public unsafe void xrGetSpatialBufferUint8EXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint8EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint8EXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferUint8EXT(0, getInfo, 0, out var bufferCountOutput, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, bufferCountOutput);

            var array = new NativeArray<byte>(13, Allocator.Temp);
            result = OpenXRNativeApi.xrGetSpatialBufferUint8EXT(
                0, getInfo, bufferCountOutput, out bufferCountOutput, (byte*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, bufferCountOutput);
            Assert.AreEqual(
                "Hello, world.", Encoding.UTF8.GetString((byte*)array.GetUnsafePtr(), (int)bufferCountOutput));
        }

        [Test]
        [Description("The NativeArray overload of the uint8 buffer getter allocates and fills the runtime's " +
                     "bytes.")]
        public void xrGetSpatialBufferUint8EXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint8EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint8EXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferUint8EXT(0, getInfo, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, array.Length);
            Assert.AreEqual("Hello, world.", Encoding.UTF8.GetString(array.AsReadOnlySpan()));
        }

        [Test]
        [Description("The convenience overload of the uint8 buffer getter taking a bare buffer id allocates " +
                     "and fills the runtime's bytes.")]
        public void xrGetSpatialBufferUint8EXT_NativeArray_Convenience_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint8EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint8EXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrGetSpatialBufferUint8EXT(0, 123, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(13, array.Length);
            Assert.AreEqual("Hello, world.", Encoding.UTF8.GetString(array.AsReadOnlySpan()));
        }

        [Test]
        [Description("The two-call raw uint16 buffer getter reports the count and then fills the runtime's " +
                     "five values.")]
        public unsafe void xrGetSpatialBufferUint16EXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint16EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint16EXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferUint16EXT(0, getInfo, 0, out var bufferCountOutput, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(5, bufferCountOutput);

            var array = new NativeArray<ushort>(5, Allocator.Temp);
            result = OpenXRNativeApi.xrGetSpatialBufferUint16EXT(
                0, getInfo, bufferCountOutput, out bufferCountOutput, (ushort*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(5, bufferCountOutput);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
        }

        [Test]
        [Description("The NativeArray overload of the uint16 buffer getter allocates and fills the runtime's " +
                     "values.")]
        public void xrGetSpatialBufferUint16EXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint16EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint16EXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferUint16EXT(0, getInfo, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(5, array.Length);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
        }

        [Test]
        [Description("The convenience overload of the uint16 buffer getter taking a bare buffer id allocates " +
                     "and fills the runtime's values.")]
        public void xrGetSpatialBufferUint16EXT_NativeArray_Convenience_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint16EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint16EXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrGetSpatialBufferUint16EXT(0, 123, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(5, array.Length);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
        }

        [Test]
        [Description("The two-call raw uint32 buffer getter reports the count and then fills the runtime's " +
                     "two values.")]
        public unsafe void xrGetSpatialBufferUint32EXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint32EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint32EXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferUint32EXT(0, getInfo, 0, out var bufferCountOutput, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);

            var array = new NativeArray<uint>(2, Allocator.Temp);
            result = OpenXRNativeApi.xrGetSpatialBufferUint32EXT(
                0, getInfo, bufferCountOutput, out bufferCountOutput, (uint*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);
            Assert.AreEqual(12345, array[0]);
            Assert.AreEqual(67890, array[1]);
        }

        [Test]
        [Description("The NativeArray overload of the uint32 buffer getter allocates and fills the runtime's " +
                     "values.")]
        public void xrGetSpatialBufferUint32EXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint32EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint32EXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferUint32EXT(0, getInfo, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(12345, array[0]);
            Assert.AreEqual(67890, array[1]);
        }

        [Test]
        [Description("The convenience overload of the uint32 buffer getter taking a bare buffer id allocates " +
                     "and fills the runtime's values.")]
        public void xrGetSpatialBufferUint32EXT_NativeArray_Convenience_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferUint32EXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferUint32EXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrGetSpatialBufferUint32EXT(0, 123, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(12345, array[0]);
            Assert.AreEqual(67890, array[1]);
        }

        [Test]
        [Description("The two-call raw float buffer getter reports the count and then fills the runtime's " +
                     "two values.")]
        public unsafe void xrGetSpatialBufferFloatEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferFloatEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferFloatEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferFloatEXT(0, getInfo, 0, out var bufferCountOutput, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);

            var array = new NativeArray<float>(2, Allocator.Temp);
            result = OpenXRNativeApi.xrGetSpatialBufferFloatEXT(
                0, getInfo, bufferCountOutput, out bufferCountOutput, (float*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);
            Assert.AreEqual(.333f, array[0]);
            Assert.AreEqual(.12345f, array[1]);
        }

        [Test]
        [Description("The NativeArray overload of the float buffer getter allocates and fills the runtime's " +
                     "values.")]
        public void xrGetSpatialBufferFloatEXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferFloatEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferFloatEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferFloatEXT(0, getInfo, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(.333f, array[0]);
            Assert.AreEqual(.12345f, array[1]);
        }

        [Test]
        [Description("The convenience overload of the float buffer getter taking a bare buffer id allocates " +
                     "and fills the runtime's values.")]
        public void xrGetSpatialBufferFloatEXT_NativeArray_Convenience_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferFloatEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferFloatEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrGetSpatialBufferFloatEXT(0, 123, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(.333f, array[0]);
            Assert.AreEqual(.12345f, array[1]);
        }

        [Test]
        [Description("The two-call raw Vector2f buffer getter reports the count and then fills the runtime's " +
                     "two vectors.")]
        public unsafe void xrGetSpatialBufferVector2fEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferVector2fEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferVector2fEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferVector2fEXT(0, getInfo, 0, out var bufferCountOutput, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);

            var array = new NativeArray<XrVector2f>(2, Allocator.Temp);
            result = OpenXRNativeApi.xrGetSpatialBufferVector2fEXT(
                0, getInfo, bufferCountOutput, out bufferCountOutput, (XrVector2f*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);
            Assert.IsTrue(new XrVector2f(1.5f, 2.5f).Equals(array[0]));
            Assert.IsTrue(new XrVector2f(3.5f, 4.5f).Equals(array[1]));
        }

        [Test]
        [Description("The NativeArray overload of the Vector2f buffer getter allocates and fills the " +
                     "runtime's vectors.")]
        public void xrGetSpatialBufferVector2fEXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferVector2fEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferVector2fEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferVector2fEXT(0, getInfo, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.IsTrue(new XrVector2f(1.5f, 2.5f).Equals(array[0]));
            Assert.IsTrue(new XrVector2f(3.5f, 4.5f).Equals(array[1]));
        }

        [Test]
        [Description("The convenience overload of the Vector2f buffer getter taking a bare buffer id " +
                     "allocates and fills the runtime's vectors.")]
        public void xrGetSpatialBufferVector2fEXT_NativeArray_Convenience_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferVector2fEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferVector2fEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrGetSpatialBufferVector2fEXT(0, 123, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.IsTrue(new XrVector2f(1.5f, 2.5f).Equals(array[0]));
            Assert.IsTrue(new XrVector2f(3.5f, 4.5f).Equals(array[1]));
        }

        [Test]
        [Description("The two-call raw Vector3f buffer getter reports the count and then fills the runtime's " +
                     "two vectors.")]
        public unsafe void xrGetSpatialBufferVector3fEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferVector3fEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferVector3fEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferVector3fEXT(0, getInfo, 0, out var bufferCountOutput, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);

            var array = new NativeArray<XrVector3f>(2, Allocator.Temp);
            result = OpenXRNativeApi.xrGetSpatialBufferVector3fEXT(
                0, getInfo, bufferCountOutput, out bufferCountOutput, (XrVector3f*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, bufferCountOutput);
            Assert.IsTrue(new XrVector3f(1.5f, 2.5f, 3.5f).Equals(array[0]));
            Assert.IsTrue(new XrVector3f(4.5f, 5.5f, 6.5f).Equals(array[1]));
        }

        [Test]
        [Description("The NativeArray overload of the Vector3f buffer getter allocates and fills the " +
                     "runtime's vectors.")]
        public void xrGetSpatialBufferVector3fEXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferVector3fEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferVector3fEXT_Ptr);
            m_Environment.Start();

            var getInfo = new XrSpatialBufferGetInfoEXT(123);
            var result = OpenXRNativeApi.xrGetSpatialBufferVector3fEXT(0, getInfo, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.IsTrue(new XrVector3f(1.5f, 2.5f, 3.5f).Equals(array[0]));
            Assert.IsTrue(new XrVector3f(4.5f, 5.5f, 6.5f).Equals(array[1]));
        }

        [Test]
        [Description("The convenience overload of the Vector3f buffer getter taking a bare buffer id " +
                     "allocates and fills the runtime's vectors.")]
        public void xrGetSpatialBufferVector3fEXT_NativeArray_Convenience_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrGetSpatialBufferVector3fEXT",
                EXTSpatialEntityMocks.xrGetSpatialBufferVector3fEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrGetSpatialBufferVector3fEXT(0, 123, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.IsTrue(new XrVector3f(1.5f, 2.5f, 3.5f).Equals(array[0]));
            Assert.IsTrue(new XrVector3f(4.5f, 5.5f, 6.5f).Equals(array[1]));
        }

        [Test]
        [Description("The raw update-snapshot create binding returns the runtime's snapshot handle.")]
        public unsafe void xrCreateSpatialUpdateSnapshotEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialUpdateSnapshotEXT",
                EXTSpatialEntityMocks.xrCreateSpatialUpdateSnapshotEXT_Ptr);
            m_Environment.Start();

            var createInfo = new XrSpatialUpdateSnapshotCreateInfoEXT(0, null, 0, null, 0, 0);
            var result = OpenXRNativeApi.xrCreateSpatialUpdateSnapshotEXT(0, createInfo, out var snapshot);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(123456, snapshot);
        }

        [Test]
        [Description("The context overload of update-snapshot create taking raw pointers returns success " +
                     "and the runtime's snapshot handle.")]
        public unsafe void xrCreateSpatialUpdateSnapshotEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialUpdateSnapshotEXT",
                EXTSpatialEntityMocks.xrCreateSpatialUpdateSnapshotEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialUpdateSnapshotEXT(0, 0, null, 0, null, out var snapshot);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(123456, snapshot);
        }

        [Test]
        [Description("The context NativeArray overload of update-snapshot create returns success and the " +
                     "runtime's snapshot handle.")]
        public void xrCreateSpatialUpdateSnapshotEXT_NativeArray_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialUpdateSnapshotEXT",
                EXTSpatialEntityMocks.xrCreateSpatialUpdateSnapshotEXT_Ptr);
            m_Environment.Start();

            var entities = new NativeArray<XrSpatialEntityIdEXT>(1, Allocator.Temp);
            var componentTypes = new NativeArray<XrSpatialComponentTypeEXT>(1, Allocator.Temp);
            var result = OpenXRNativeApi.xrCreateSpatialUpdateSnapshotEXT(0, entities, componentTypes, out var snapshot);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(123456, snapshot);
        }

        [Test]
        [Description("Destroying a spatial snapshot through the binding returns the runtime's success result.")]
        public void xrDestroySpatialSnapshotEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrDestroySpatialSnapshotEXT",
                EXTSpatialEntityMocks.xrDestroySpatialSnapshotEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrDestroySpatialSnapshotEXT(0);

            Assert.AreEqual(XrResult.Success, result);
        }

        [DllImport("UnityOpenXR", EntryPoint = "EXT_spatial_entity_ValidateStruct")]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern unsafe bool ValidateStruct(XrBaseInStructure* structPtr);

        [Test]
        [Description("The native EXT_spatial_entity validator accepts a context create-info chaining plane, " +
                     "anchor, ArUco, AprilTag, QR and micro-QR capability configurations, confirming the C# " +
                     "and C layouts agree.")]
        public unsafe void ValidateStruct_XrSpatialContextCreateInfoEXT()
        {
            var planeComponents = new NativeArray<XrSpatialComponentTypeEXT>(4, Allocator.Temp);
            planeComponents[0] = XrSpatialComponentTypeEXT.Bounded2D;
            planeComponents[1] = XrSpatialComponentTypeEXT.PlaneAlignment;
            planeComponents[2] = XrSpatialComponentTypeEXT.Polygon2D;
            planeComponents[3] = XrSpatialComponentTypeEXT.PlaneSemanticLabel;

            var planeConfig = new XrSpatialCapabilityConfigurationPlaneTrackingEXT(planeComponents);
            var planeConfigPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XrSpatialCapabilityConfigurationPlaneTrackingEXT>());
            Marshal.StructureToPtr(planeConfig, planeConfigPtr, false);

            var anchorComponents = new NativeArray<XrSpatialComponentTypeEXT>(2, Allocator.Temp);
            anchorComponents[0] = XrSpatialComponentTypeEXT.Anchor;
            anchorComponents[1] = XrSpatialComponentTypeEXT.Persistence;

            var anchorConfig = new XrSpatialCapabilityConfigurationAnchorEXT(anchorComponents);
            var anchorConfigPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XrSpatialCapabilityConfigurationAnchorEXT>());
            Marshal.StructureToPtr(anchorConfig, anchorConfigPtr, false);

            var markerComponents = new NativeArray<XrSpatialComponentTypeEXT>(2, Allocator.Temp);
            markerComponents[0] = XrSpatialComponentTypeEXT.Marker;
            markerComponents[1] = XrSpatialComponentTypeEXT.Bounded2D;

            var arucoConfig = new XrSpatialCapabilityConfigurationArucoMarkerEXT(
                markerComponents, XrSpatialMarkerArucoDictEXT.Dict_4x4_50);
            var arucoConfigPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XrSpatialCapabilityConfigurationArucoMarkerEXT>());
            Marshal.StructureToPtr(arucoConfig, arucoConfigPtr, false);

            var aprilConfig = new XrSpatialCapabilityConfigurationAprilTagEXT(
                markerComponents, XrSpatialMarkerAprilTagDictEXT.Dict_16h5);
            var aprilConfigPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XrSpatialCapabilityConfigurationAprilTagEXT>());
            Marshal.StructureToPtr(aprilConfig, aprilConfigPtr, false);

            var qrConfig = new XrSpatialCapabilityConfigurationQrCodeEXT(markerComponents);
            var qrConfigPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XrSpatialCapabilityConfigurationQrCodeEXT>());
            Marshal.StructureToPtr(qrConfig, qrConfigPtr, false);

            var microQrConfig = new XrSpatialCapabilityConfigurationMicroQrCodeEXT(markerComponents);
            var microQrConfigPtr = Marshal.AllocHGlobal(
                Marshal.SizeOf<XrSpatialCapabilityConfigurationMicroQrCodeEXT>());
            Marshal.StructureToPtr(microQrConfig, microQrConfigPtr, false);

            var configPtrs = new NativeArray<IntPtr>(6, Allocator.Temp);
            configPtrs[0] = planeConfigPtr;
            configPtrs[1] = anchorConfigPtr;
            configPtrs[2] = arucoConfigPtr;
            configPtrs[3] = aprilConfigPtr;
            configPtrs[4] = qrConfigPtr;
            configPtrs[5] = microQrConfigPtr;

            var contextCreateInfo = new XrSpatialContextCreateInfoEXT(configPtrs);

            try
            {
                Assert.IsTrue(ValidateStruct((XrBaseInStructure*)&contextCreateInfo));
            }
            finally
            {
                Marshal.FreeHGlobal(planeConfigPtr);
                Marshal.FreeHGlobal(anchorConfigPtr);
                Marshal.FreeHGlobal(arucoConfigPtr);
                Marshal.FreeHGlobal(aprilConfigPtr);
                Marshal.FreeHGlobal(qrConfigPtr);
                Marshal.FreeHGlobal(microQrConfigPtr);
            }
        }
    }
}
