using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEditor.XR.OpenXR.Tests.NativeTypes
{
    [Category("Level.Unit")]
    [Property("Covers", "SpatialEntities")]
    class XrSpatialPersistenceDataEXTTests
    {
        [Test]
        [Description("XrSpatialPersistenceDataEXT marshals to the 20-byte size the native struct expects.")]
        public void SizeValidation()
        {
            Assert.AreEqual(20, Marshal.SizeOf<XrSpatialPersistenceDataEXT>());
        }
    }
}
