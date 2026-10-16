using NUnit.Framework;
using UnityEngine.XR.OpenXR.NativeTypes;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    [Category("Level.Integration")]
    [Property("Covers", "SpatialEntities")]
    class EXTSpatialAnchorTests : MockRuntimeEditorTestBase
    {
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_anchor");
            m_Environment.Settings.RequestUseExtension("XR_EXT_spatial_entity");
            m_Environment.Settings.RequestUseExtension("XR_EXT_future");
            m_Environment.AddSupportedExtension("XR_EXT_spatial_anchor", 1);
            m_Environment.AddSupportedExtension("XR_EXT_spatial_entity", 1);
            m_Environment.AddSupportedExtension("XR_EXT_future", 1);
        }

        [Test]
        [Description("The raw xrCreateSpatialAnchorEXT binding returns the runtime's result plus the anchor " +
                     "id and entity handle it reports.")]
        public void xrCreateSpatialAnchorEXT_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialAnchorEXT", EXTSpatialAnchorMocks.xrCreateSpatialAnchorEXT_Ptr);
            m_Environment.Start();

            var createInfo = new XrSpatialAnchorCreateInfoEXT(0, 0, new XrPosef());
            var result = OpenXRNativeApi.xrCreateSpatialAnchorEXT(0, createInfo, out var id, out var entity);

            Assert.AreEqual(XrResult.Success, result);
            Assert.AreEqual(123, id);
            Assert.AreEqual(456, entity);
        }

        [Test]
        [Description("The context overload of xrCreateSpatialAnchorEXT taking an XrPosef returns success " +
                     "plus the runtime's anchor id and entity handle.")]
        public void xrCreateSpatialAnchorEXT_UsingContext_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialAnchorEXT", EXTSpatialAnchorMocks.xrCreateSpatialAnchorEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialAnchorEXT(0, new XrPosef(), out var id, out var entity);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(123, id);
            Assert.AreEqual(456, entity);
        }

        [Test]
        [Description("The xrCreateSpatialAnchorEXT overload taking a Unity Vector3 and Quaternion returns " +
                     "success plus the runtime's anchor id and entity handle.")]
        public void xrCreateSpatialAnchorEXT_UsingContext_WithUnityInputs_ReturnsRuntimeValues()
        {
            m_Environment.SetFunctionForInterceptor(
                "xrCreateSpatialAnchorEXT", EXTSpatialAnchorMocks.xrCreateSpatialAnchorEXT_Ptr);
            m_Environment.Start();

            var result = OpenXRNativeApi.xrCreateSpatialAnchorEXT(
                0, Vector3.zero, Quaternion.identity, out var id, out var entity);

            Assert.AreEqual(OpenXRResultStatus.unqualifiedSuccess, result);
            Assert.AreEqual(123, id);
            Assert.AreEqual(456, entity);
        }
    }
}
