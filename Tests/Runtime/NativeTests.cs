using NUnit.Framework;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "Loader")]
    [NUnit.Framework.Property("Covers", "Input")]
    class NativeTests : OpenXRLoaderSetup
    {
        public override void BeforeTest()
        {
            OpenXRLoaderBase.Internal_UnloadOpenXRLibrary();
            base.BeforeTest();
        }

        [Test]
        [Description("Loading the OpenXR library with a null path fails.")]
        public void OpenXRLoader_LoadOpenXRLibrary_NullLoaderPath()
        {
            Assert.IsFalse(OpenXRLoaderBase.Internal_LoadOpenXRLibrary(null));
        }

        [Test]
        [Description("Loading the OpenXR library from a nonexistent path fails.")]
        public void OpenXRLoader_LoadOpenXRLibrary_InvalidLoaderPath()
        {
            Assert.IsFalse(OpenXRLoaderBase.Internal_LoadOpenXRLibrary(OpenXRLoaderBase.StringToWCHAR_T("abababab")));
        }

        [Test]
        [Description("Initializing a session before the OpenXR library is loaded fails instead of crashing.")]
        public void OpenXRLoader_InitializeSession_BeforeLoadingLibrary()
        {
            Assert.IsFalse(OpenXRLoaderBase.Internal_InitializeSession());
        }

        [Test]
        [Description("Creating a session before the OpenXR library is loaded fails instead of crashing.")]
        public void OpenXRLoader_CreateSessionIfNeeded_BeforeLoadingLibrary()
        {
            Assert.IsFalse(OpenXRLoaderBase.Internal_CreateSessionIfNeeded());
        }

        [Test]
        [Description("Requesting a null extension string before the library is loaded fails.")]
        public void OpenXRLoader_RequestEnableExtensionString_BeforeLoadingLibrary()
        {
            Assert.IsFalse(OpenXRLoaderBase.Internal_RequestEnableExtensionString(null));
        }

        [Test]
        [Description("Requesting a named extension before the library is loaded fails.")]
        public void OpenXRLoader_RequestEnableExtensionString_Null()
        {
            Assert.IsFalse(OpenXRLoaderBase.Internal_RequestEnableExtensionString("some_extension"));
        }

        [Test]
        [Description("Querying an input source name before OpenXR input is initialized fails.")]
        public void OpenXRInput_TryGetInputSourceName_BeforeInitializing()
        {
            Assert.IsFalse(OpenXRInput.Internal_TryGetInputSourceName(0, 0, 0, 0, out _));
        }

        [Test]
        [Description("Suggesting bindings before OpenXR input is initialized fails.")]
        public void OpenXRInput_SuggestBindings_BeforeInitializing()
        {
            Assert.IsFalse(OpenXRInput.Internal_SuggestBindings("", null, 0));
        }

        [Test]
        [Description("Attaching action sets before OpenXR input is initialized fails.")]
        public void OpenXRInput_AttachActionSets_BeforeInitializing()
        {
            Assert.IsFalse(OpenXRInput.Internal_AttachActionSets());
        }
    }
}
