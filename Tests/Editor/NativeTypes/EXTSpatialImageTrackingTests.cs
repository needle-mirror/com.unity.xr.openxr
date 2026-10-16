using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "SpatialEntities")]
    class EXTSpatialImageTrackingTests : MockRuntimeEditorTestBase
    {
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_image_tracking");
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_entity");
            m_Environment.Settings.RequestUseExtension("XR_EXT_future");
            m_Environment.AddSupportedExtension("XR_EXT_spatial_image_tracking", 1);
            m_Environment.AddSupportedExtension("XR_EXT_spatial_entity", 1);
            m_Environment.AddSupportedExtension("XR_EXT_future", 1);
        }

        [Test]
        [Description("The two-call enumeration of reference image formats reports the count on the sizing " +
                     "call and fills the array with the runtime's formats on the second.")]
        public unsafe void xrEnumerateSpatialReferenceImageFormatsEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialReferenceImageFormatsEXT",
                EXTSpatialImageTrackingMocks.xrEnumerateSpatialReferenceImageFormatsEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialReferenceImageFormatsEXT(
                0, 0, XrSpatialCapabilityEXT.ImageTracking, 0, out var countOutput, null);
            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, countOutput);

            var array = new NativeArray<XrSpatialReferenceImageFormatEXT>((int)countOutput, Allocator.Temp);
            result = OpenXRNativeApi.xrEnumerateSpatialReferenceImageFormatsEXT(
                0, 0, XrSpatialCapabilityEXT.ImageTracking, countOutput, out countOutput,
                (XrSpatialReferenceImageFormatEXT*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, countOutput);
            Assert.AreEqual(XrSpatialReferenceImageFormatEXT.RGBA_8888, array[0]);
            Assert.AreEqual(XrSpatialReferenceImageFormatEXT.RGB_888, array[1]);
        }

        [Test]
        [Description("Enumerating reference image formats into a too-small array returns SizeInsufficient " +
                     "together with the required count.")]
        public unsafe void xrEnumerateSpatialReferenceImageFormatsEXT_InsufficientCapacity_ReturnsSizeInsufficient()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialReferenceImageFormatsEXT",
                EXTSpatialImageTrackingMocks.xrEnumerateSpatialReferenceImageFormatsEXT_Ptr);
            m_Environment.Start();

            var array = new NativeArray<XrSpatialReferenceImageFormatEXT>(1, Allocator.Temp);
            var result = OpenXRNativeApi.xrEnumerateSpatialReferenceImageFormatsEXT(
                0, 0, XrSpatialCapabilityEXT.ImageTracking, (uint)array.Length, out var countOutput,
                (XrSpatialReferenceImageFormatEXT*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.SizeInsufficient, result);
            Assert.AreEqual(2, countOutput);
        }

        [Test]
        [Description("The context overload allocates a NativeArray of the right length and fills it with the " +
                     "runtime's reference image formats.")]
        public void xrEnumerateSpatialReferenceImageFormatsEXT_UsingContext_Array_AllocatesCorrectly()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialReferenceImageFormatsEXT",
                EXTSpatialImageTrackingMocks.xrEnumerateSpatialReferenceImageFormatsEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialReferenceImageFormatsEXT(
                XrSpatialCapabilityEXT.ImageTracking, Allocator.Temp, out var array);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(XrSpatialReferenceImageFormatEXT.RGBA_8888, array[0]);
            Assert.AreEqual(XrSpatialReferenceImageFormatEXT.RGB_888, array[1]);
        }

        [Test]
        [Description("The raw async database-create binding accepts a reference image with planes and " +
                     "returns the runtime's future handle.")]
        public void xrCreateSpatialImageTrackingDatabaseAsyncEXT_ReturnsFuture()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialImageTrackingDatabaseAsyncEXT",
                EXTSpatialImageTrackingMocks.xrCreateSpatialImageTrackingDatabaseAsyncEXT_Ptr);
            m_Environment.Start();

            using var buffer = new NativeArray<byte>(16, Allocator.Temp);
            using var planes = new NativeArray<XrSpatialReferenceImagePlaneEXT>(4, Allocator.Temp)
            {
                [0] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4),
                [1] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4),
                [2] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4),
                [3] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4)
            };
            using var images = new NativeArray<XrSpatialReferenceImageEXT>(1, Allocator.Temp)
            {
                [0] = new XrSpatialReferenceImageEXT(4, 4, XrSpatialReferenceImageFormatEXT.RGBA_8888, planes)
            };

            var createInfo = new XrSpatialImageTrackingDatabaseCreateInfoEXT(images);
            var result = OpenXRNativeApi.xrCreateSpatialImageTrackingDatabaseAsyncEXT(0, in createInfo, out var future);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(EXTSpatialImageTrackingMocks.k_Future, future);
        }

        [Test]
        [Description("The context overload of the async database-create call returns success and the " +
                     "runtime's future handle.")]
        public void xrCreateSpatialImageTrackingDatabaseAsyncEXT_UsingContext_ReturnsFuture()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialImageTrackingDatabaseAsyncEXT",
                EXTSpatialImageTrackingMocks.xrCreateSpatialImageTrackingDatabaseAsyncEXT_Ptr);
            m_Environment.Start();

            using var buffer = new NativeArray<byte>(16, Allocator.Temp);
            using var planes = new NativeArray<XrSpatialReferenceImagePlaneEXT>(4, Allocator.Temp)
            {
                [0] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4),
                [1] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4),
                [2] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4),
                [3] = new XrSpatialReferenceImagePlaneEXT(buffer, 16, 4)
            };
            using var images = new NativeArray<XrSpatialReferenceImageEXT>(1, Allocator.Temp)
            {
                [0] = new XrSpatialReferenceImageEXT(4, 4, XrSpatialReferenceImageFormatEXT.RGBA_8888, planes)
            };

            var createInfo = new XrSpatialImageTrackingDatabaseCreateInfoEXT(images);
            var result = OpenXRNativeApi.xrCreateSpatialImageTrackingDatabaseAsyncEXT(in createInfo, out var future);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(EXTSpatialImageTrackingMocks.k_Future, future);
        }

        [Test]
        [Description("Completing the database-create future returns a completion with the right structure " +
                     "type, a success futureResult and the database handle.")]
        public void xrCreateSpatialImageTrackingDatabaseCompleteEXT_ReturnsDatabase()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialImageTrackingDatabaseCompleteEXT",
                EXTSpatialImageTrackingMocks.xrCreateSpatialImageTrackingDatabaseCompleteEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialImageTrackingDatabaseCompleteEXT(
                0, EXTSpatialImageTrackingMocks.k_Future, out var completion);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(
                XrStructureType.CreateSpatialImageTrackingDatabaseCompletionEXT, completion.type);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(EXTSpatialImageTrackingMocks.k_DatabaseHandle, completion.database);
        }

        [Test]
        [Description("The context overload of the database-create completion returns the typed completion " +
                     "with a success result and the database handle.")]
        public void xrCreateSpatialImageTrackingDatabaseCompleteEXT_UsingContext_ReturnsDatabase()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialImageTrackingDatabaseCompleteEXT",
                EXTSpatialImageTrackingMocks.xrCreateSpatialImageTrackingDatabaseCompleteEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialImageTrackingDatabaseCompleteEXT(
                EXTSpatialImageTrackingMocks.k_Future, out var completion);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(
                XrStructureType.CreateSpatialImageTrackingDatabaseCompletionEXT, completion.type);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(EXTSpatialImageTrackingMocks.k_DatabaseHandle, completion.database);
        }

        [Test]
        [Description("Destroying an image tracking database through the binding returns the runtime's " +
                     "success result.")]
        public void xrDestroySpatialImageTrackingDatabaseEXT_ReturnsSuccess()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrDestroySpatialImageTrackingDatabaseEXT",
                EXTSpatialImageTrackingMocks.xrDestroySpatialImageTrackingDatabaseEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrDestroySpatialImageTrackingDatabaseEXT(
                EXTSpatialImageTrackingMocks.k_DatabaseHandle);

            Assert.AreEqual(XrResult.Success, result);
        }

        [Test]
        [Description("The defaultValue of XrCreateSpatialImageTrackingDatabaseCompletionEXT carries the " +
                     "correct structure type.")]
        public void XrCreateSpatialImageTrackingDatabaseCompletionEXT_DefaultValue_HasCorrectType()
        {
            var completion = XrCreateSpatialImageTrackingDatabaseCompletionEXT.defaultValue;
            Assert.AreEqual(
                XrStructureType.CreateSpatialImageTrackingDatabaseCompletionEXT, completion.type);
        }

        [Test]
        [Description("Building a reference image plane and image from NativeArrays records buffer size, " +
                     "strides, dimensions, format and plane count.")]
        public void XrSpatialReferenceImageEXT_ConstructsFromNativeArray()
        {
            using var buffer = new NativeArray<byte>(16, Allocator.Temp);
            var plane = new XrSpatialReferenceImagePlaneEXT(buffer, 12, 3);
            Assert.AreEqual(16u, plane.bufferSize);
            Assert.AreEqual(12u, plane.rowStride);
            Assert.AreEqual(3u, plane.pixelStride);

            using var planes = new NativeArray<XrSpatialReferenceImagePlaneEXT>(3, Allocator.Temp)
            {
                [0] = plane, [1] = plane, [2] = plane
            };
            var image = new XrSpatialReferenceImageEXT(64, 32, XrSpatialReferenceImageFormatEXT.RGB_888, planes);

            Assert.AreEqual(XrStructureType.SpatialReferenceImageEXT, image.type);
            Assert.AreEqual(64u, image.width);
            Assert.AreEqual(32u, image.height);
            Assert.AreEqual(XrSpatialReferenceImageFormatEXT.RGB_888, image.format);
            Assert.AreEqual(3u, image.planeCount);
        }

        [Test]
        [Description("The read-only NativeArray constructors for reference image planes and images record " +
                     "the same fields as the mutable ones.")]
        public void XrSpatialReferenceImageEXT_ConstructsFromReadOnlyNativeArray()
        {
            using var buffer = new NativeArray<byte>(16, Allocator.Temp);
            var plane = new XrSpatialReferenceImagePlaneEXT(buffer.AsReadOnly(), 12, 3);
            Assert.AreEqual(16u, plane.bufferSize);
            Assert.AreEqual(12u, plane.rowStride);
            Assert.AreEqual(3u, plane.pixelStride);

            using var planes = new NativeArray<XrSpatialReferenceImagePlaneEXT>(3, Allocator.Temp)
            {
                [0] = plane, [1] = plane, [2] = plane
            };
            var image = new XrSpatialReferenceImageEXT(
                64, 32, XrSpatialReferenceImageFormatEXT.RGB_888, planes.AsReadOnly());

            Assert.AreEqual(XrStructureType.SpatialReferenceImageEXT, image.type);
            Assert.AreEqual(64u, image.width);
            Assert.AreEqual(32u, image.height);
            Assert.AreEqual(XrSpatialReferenceImageFormatEXT.RGB_888, image.format);
            Assert.AreEqual(3u, image.planeCount);
        }
    }
}
