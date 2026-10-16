using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.TestTooling;

namespace UnityEngine.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "ReferenceSpace")]
    class OpenXRReferenceSpaceTests
    {
        [UnityTest]
        [Description("Starting an OpenXR environment requests the XR_EXT_local_floor " +
                     "extension from the runtime, so local-floor reference spaces are available.")]
        public IEnumerator CheckLocalFloorExtensionRequested()
        {
            var localFloorExtension = "XR_EXT_local_floor";
            bool extensionRequested = false;

            using (var environment = MockOpenXREnvironment.CreateEnvironment())
            {
                environment.Start();

                yield return new WaitForXrFrame();

                extensionRequested = OpenXRRuntime.IsExtensionRequested(localFloorExtension);

                environment.Stop();
            }

            Assert.IsTrue(extensionRequested, "Extension was not requested");
        }
    }
}
