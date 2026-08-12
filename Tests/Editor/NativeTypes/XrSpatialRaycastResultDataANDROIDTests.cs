using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    class XrSpatialRaycastResultDataANDROIDTests
    {
        [Test]
        public void SizeValidation()
        {
            Assert.AreEqual(32, Marshal.SizeOf<XrSpatialRaycastResultDataANDROID>());
        }
    }
}
