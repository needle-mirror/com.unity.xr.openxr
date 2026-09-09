#if XR_COMPOSITION_LAYERS_1_0_0_OR_NEWER
using NUnit.Framework;
using UnityEngine.XR.OpenXR.CompositionLayers;
using TextureDimension = UnityEngine.Rendering.TextureDimension;
using GraphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat;

namespace UnityEditor.XR.OpenXR.CompositionLayers.Tests
{
    // Tests for <see cref="OpenXRLayerUtility.CanTextureUseGraphicsCopy"/>, which decides whether a composition
    // layer texture transfer can use a direct Graphics.CopyTexture or must fall back to Graphics.Blit.
    internal class CopyTextureOverBlitTests
    {
        const int k_Size = 64;
        const GraphicsFormat k_Format = GraphicsFormat.R8G8B8A8_UNorm;

        readonly CompositionLayerTestHelpers m_Helpers = new CompositionLayerTestHelpers();

        [TearDown]
        public void TearDown() => m_Helpers.Cleanup();

        [Test]
        public void MatchingTexture2D_CanCopy()
        {
            m_Helpers.RequireCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format);
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, destination.graphicsFormat);

            Assert.IsTrue(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void SizeMismatch_FallsBackToBlit()
        {
            m_Helpers.RequireCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format);
            var source = m_Helpers.CreateTexture2D(k_Size / 2, k_Size / 2, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void FormatMismatch_FallsBackToBlit()
        {
            m_Helpers.RequireCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, GraphicsFormat.R8G8B8A8_SRGB);
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, GraphicsFormat.R8G8B8A8_UNorm);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void MipCountMismatch_FallsBackToBlit()
        {
            m_Helpers.RequireCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, mipCount: 3);
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, destination.graphicsFormat, mipCount: 1);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void DimensionMismatch_FallsBackToBlit()
        {
            m_Helpers.RequireCopySupport();

            // A single Tex2D written into a Tex2DArray must blit for correct slice routing.
            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, dimension: TextureDimension.Tex2DArray, slices: 2);
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void ArraySliceCountMismatch_FallsBackToBlit()
        {
            m_Helpers.RequireCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, dimension: TextureDimension.Tex2DArray, slices: 4);
            var source = m_Helpers.CreateTexture2DArray(k_Size, k_Size, 2, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void MsaaMismatch_FallsBackToBlit()
        {
            m_Helpers.RequireCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, msaaSamples: 4);
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void MatchingTexture2DArray_CanCopy()
        {
            m_Helpers.RequireCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, dimension: TextureDimension.Tex2DArray, slices: 2);
            var source = m_Helpers.CreateTexture2DArray(k_Size, k_Size, 2, destination.graphicsFormat);

            Assert.IsTrue(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void MatchingRenderTexture_CanCopy()
        {
            m_Helpers.RequireBasicCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format);
            var source = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format);

            Assert.IsTrue(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }

        [Test]
        public void RenderTextureMsaaMismatch_FallsBackToBlit()
        {
            m_Helpers.RequireBasicCopySupport();

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, msaaSamples: 4);
            var source = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, msaaSamples: 1);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
        }
    }
}
#endif
