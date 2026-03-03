using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.TestTooling;

namespace UnityEngine.XR.OpenXR.Tests
{
    class OpenXRReferenceSpaceTests
    {
        [UnityTest]
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
