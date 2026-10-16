using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    [Category("Level.Integration")]
    [Property("Covers", "SpatialEntities")]
    class EXTSpatialPersistenceTests : MockRuntimeEditorTestBase
    {
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_persistence");
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_entity");
            m_Environment.Settings.RequestUseExtension("XR_EXT_future");
            m_Environment.AddSupportedExtension("XR_EXT_spatial_persistence", 1);
            m_Environment.AddSupportedExtension("XR_EXT_spatial_entity", 1);
            m_Environment.AddSupportedExtension("XR_EXT_future", 1);
        }

        [Test]
        [Description("The raw async persistence-context create binding returns the runtime's result and " +
                     "future handle.")]
        public void xrCreateSpatialPersistenceContextAsyncEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialPersistenceContextAsyncEXT",
                EXTSpatialPersistenceMocks.xrCreateSpatialPersistenceContextAsyncEXT_Ptr);
            m_Environment.Start();

            var createInfo = new XrSpatialPersistenceContextCreateInfoEXT(XrSpatialPersistenceScopeEXT.SystemManaged);
            var result = OpenXRNativeApi.xrCreateSpatialPersistenceContextAsyncEXT(0, createInfo, out var future);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(123456, future);
        }

        [Test]
        [Description("The context overload of the async persistence-context create returns the runtime's " +
                     "result and future handle.")]
        public void xrCreateSpatialPersistenceContextAsyncEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialPersistenceContextAsyncEXT",
                EXTSpatialPersistenceMocks.xrCreateSpatialPersistenceContextAsyncEXT_Ptr);
            m_Environment.Start();

            var createInfo = new XrSpatialPersistenceContextCreateInfoEXT(XrSpatialPersistenceScopeEXT.SystemManaged);
            var result = OpenXRNativeApi.xrCreateSpatialPersistenceContextAsyncEXT(createInfo, out var future);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(123456, future);
        }

        [Test]
        [Description("The convenience overload taking only a persistence scope returns the runtime's result " +
                     "and future handle.")]
        public void xrCreateSpatialPersistenceContextAsyncEXT_ConvenienceOverload_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialPersistenceContextAsyncEXT",
                EXTSpatialPersistenceMocks.xrCreateSpatialPersistenceContextAsyncEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialPersistenceContextAsyncEXT(
                XrSpatialPersistenceScopeEXT.SystemManaged, out var future);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(123456, future);
        }

        [Test]
        [Description("The two-call enumeration of persistence scopes reports the count first and then fills " +
                     "the array with the runtime's scopes.")]
        public unsafe void xrEnumerateSpatialPersistenceScopesEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialPersistenceScopesEXT",
                EXTSpatialPersistenceMocks.xrEnumerateSpatialPersistenceScopesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialPersistenceScopesEXT(0, 0, 0, out var scopeCount, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, scopeCount);

            var array = new NativeArray<XrSpatialPersistenceScopeEXT>(2, Allocator.Temp);
            result = OpenXRNativeApi.xrEnumerateSpatialPersistenceScopesEXT(
                0, 0, scopeCount, out scopeCount, (XrSpatialPersistenceScopeEXT*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, scopeCount);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.SystemManaged, array[0]);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.LocalAnchors, array[1]);
        }

        [Test]
        [Description("The NativeArray overload of scope enumeration allocates and fills an array with the " +
                     "runtime's scopes.")]
        public void xrEnumerateSpatialPersistenceScopesEXT_NativeArray_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialPersistenceScopesEXT",
                EXTSpatialPersistenceMocks.xrEnumerateSpatialPersistenceScopesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialPersistenceScopesEXT(0, 0, Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.SystemManaged, array[0]);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.LocalAnchors, array[1]);
        }

        [Test]
        [Description("The context overload of the two-call scope enumeration reports the count and then " +
                     "fills the array with the runtime's scopes.")]
        public unsafe void xrEnumerateSpatialPersistenceScopesEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialPersistenceScopesEXT",
                EXTSpatialPersistenceMocks.xrEnumerateSpatialPersistenceScopesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialPersistenceScopesEXT(0, out var scopeCount, null);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, scopeCount);

            var array = new NativeArray<XrSpatialPersistenceScopeEXT>(2, Allocator.Temp);
            result = OpenXRNativeApi.xrEnumerateSpatialPersistenceScopesEXT(
                scopeCount, out scopeCount, (XrSpatialPersistenceScopeEXT*)array.GetUnsafePtr());

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, scopeCount);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.SystemManaged, array[0]);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.LocalAnchors, array[1]);
        }

        [Test]
        [Description("The context NativeArray overload of scope enumeration allocates and fills an array " +
                     "with the runtime's scopes.")]
        public void xrEnumerateSpatialPersistenceScopesEXT_NativeArray_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrEnumerateSpatialPersistenceScopesEXT",
                EXTSpatialPersistenceMocks.xrEnumerateSpatialPersistenceScopesEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrEnumerateSpatialPersistenceScopesEXT(Allocator.Temp, out var array);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.SystemManaged, array[0]);
            Assert.AreEqual(XrSpatialPersistenceScopeEXT.LocalAnchors, array[1]);
        }

        [Test]
        [Description("Completing a persistence-context future yields success results and the runtime's " +
                     "context handle.")]
        public void xrCreateSpatialPersistenceContextCompleteEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialPersistenceContextCompleteEXT",
                EXTSpatialPersistenceMocks.xrCreateSpatialPersistenceContextCompleteEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialPersistenceContextCompleteEXT(0, 0, out var completion);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(XrSpatialPersistenceContextResultEXT.Success, completion.createResult);
            Assert.AreEqual(123456, completion.persistenceContext);
        }

        [Test]
        [Description("The context overload of the persistence-context completion yields success results " +
                     "and the runtime's context handle.")]
        public void xrCreateSpatialPersistenceContextCompleteEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialPersistenceContextCompleteEXT",
                EXTSpatialPersistenceMocks.xrCreateSpatialPersistenceContextCompleteEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialPersistenceContextCompleteEXT(0, out var completion);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(XrResult.Success, completion.futureResult);
            Assert.AreEqual(XrSpatialPersistenceContextResultEXT.Success, completion.createResult);
            Assert.AreEqual(123456, completion.persistenceContext);
        }

        [Test]
        [Description("Destroying a persistence context through the binding returns the runtime's success " +
                     "result.")]
        public void xrDestroySpatialPersistenceContextEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrDestroySpatialPersistenceContextEXT",
                EXTSpatialPersistenceMocks.xrDestroySpatialPersistenceContextEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrDestroySpatialPersistenceContextEXT(0);

            Assert.AreEqual(XrResult.Success, result);
        }
    }
}
