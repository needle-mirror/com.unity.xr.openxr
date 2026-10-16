using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    [Category("Level.Unit")]
    [NUnit.Framework.Property("Covers", "SpatialEntities")]
    class XrSpatialRaycastResultDataANDROIDTests
    {
        [Test]
        [Description("XrSpatialRaycastResultDataANDROID marshals to the 32-byte size the native struct " +
                     "expects.")]
        public void SizeValidation()
        {
            Assert.AreEqual(32, Marshal.SizeOf<XrSpatialRaycastResultDataANDROID>());
        }
    }
}
