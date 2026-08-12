#if XR_COMPOSITION_LAYERS_1_0_0_OR_NEWER
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.OpenXR.CompositionLayers;
using CopyTextureSupport = UnityEngine.Rendering.CopyTextureSupport;
using TextureDimension = UnityEngine.Rendering.TextureDimension;
using GraphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat;
using TextureCreationFlags = UnityEngine.Experimental.Rendering.TextureCreationFlags;

namespace UnityEditor.XR.OpenXR.CompositionLayers.Tests
{
    // Tests for <see cref="OpenXRLayerUtility.CanTextureUseGraphicsCopy"/>, to decides whether a composition
    // layer texture transfer can use a direct Graphics.CopyTexture or must fall back to Graphics.Blit
    internal class CopyTextureOverBlitTests
    {
        const int k_Size = 64;
        const GraphicsFormat k_Format = GraphicsFormat.R8G8B8A8_UNorm;

        readonly List<Object> m_Created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var obj in m_Created)
            {
                if (obj != null)
                    Object.DestroyImmediate(obj);
            }
            m_Created.Clear();
        }

        // Every source in this suite is a Texture2D / Texture2DArray (not a RenderTexture), so the copy
        // path requires both Basic and Texture-to-RenderTexture support.
        const CopyTextureSupport k_RequiredCopySupport = CopyTextureSupport.Basic | CopyTextureSupport.TextureToRT;

        static bool CopyIsSupported => (SystemInfo.copyTextureSupport & k_RequiredCopySupport) == k_RequiredCopySupport;

        static void RequireCopySupport() =>
            Assume.That(CopyIsSupported,
                "Requires a graphics device that supports Texture-to-RenderTexture Graphics.CopyTexture "
                + "(CopyTextureSupport.Basic | CopyTextureSupport.TextureToRT).");

        // A RenderTexture source into the RenderTexture swapchain is an RT-to-RT copy, which only needs Basic
        static void RequireBasicCopySupport() =>
            Assume.That((SystemInfo.copyTextureSupport & CopyTextureSupport.Basic) != 0,
                "Requires a graphics device that supports Graphics.CopyTexture (CopyTextureSupport.Basic).");

        RenderTexture CreateRenderTexture(int width, int height, GraphicsFormat format, int mipCount = 1,
            TextureDimension dimension = TextureDimension.Tex2D, int slices = 1, int msaaSamples = 1)
        {
            var descriptor = new RenderTextureDescriptor(width, height, format, 0)
            {
                dimension = dimension,
                volumeDepth = slices,
                msaaSamples = msaaSamples,
                mipCount = mipCount,
                useMipMap = mipCount != 1,
                autoGenerateMips = false,
            };

            var renderTexture = new RenderTexture(descriptor);
            renderTexture.Create();
            m_Created.Add(renderTexture);
            return renderTexture;
        }

        Texture2D CreateTexture2D(int width, int height, GraphicsFormat format, int mipCount = 1)
        {
            var texture = new Texture2D(width, height, format, mipCount, TextureCreationFlags.None);
            m_Created.Add(texture);
            return texture;
        }

        Texture2DArray CreateTexture2DArray(int width, int height, int slices, GraphicsFormat format)
        {
            var texture = new Texture2DArray(width, height, slices, format, TextureCreationFlags.None, 1);
            m_Created.Add(texture);
            return texture;
        }

        [Test]
        public async Task MatchingTexture2D_CanCopy()
        {
            RequireCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format);
            var source = CreateTexture2D(k_Size, k_Size, destination.graphicsFormat);

            Assert.IsTrue(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task SizeMismatch_FallsBackToBlit()
        {
            RequireCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format);
            var source = CreateTexture2D(k_Size / 2, k_Size / 2, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task FormatMismatch_FallsBackToBlit()
        {
            RequireCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, GraphicsFormat.R8G8B8A8_SRGB);
            var source = CreateTexture2D(k_Size, k_Size, GraphicsFormat.R8G8B8A8_UNorm);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task MipCountMismatch_FallsBackToBlit()
        {
            RequireCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format, mipCount: 3);
            var source = CreateTexture2D(k_Size, k_Size, destination.graphicsFormat, mipCount: 1);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task DimensionMismatch_FallsBackToBlit()
        {
            RequireCopySupport();

            // A single Tex2D written into a Tex2DArray must blit for correct slice routing.
            var destination = CreateRenderTexture(k_Size, k_Size, k_Format, dimension: TextureDimension.Tex2DArray, slices: 2);
            var source = CreateTexture2D(k_Size, k_Size, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task ArraySliceCountMismatch_FallsBackToBlit()
        {
            RequireCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format, dimension: TextureDimension.Tex2DArray, slices: 4);
            var source = CreateTexture2DArray(k_Size, k_Size, 2, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task MsaaMismatch_FallsBackToBlit()
        {
            RequireCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format, msaaSamples: 4);
            var source = CreateTexture2D(k_Size, k_Size, destination.graphicsFormat);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task MatchingTexture2DArray_CanCopy()
        {
            RequireCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format, dimension: TextureDimension.Tex2DArray, slices: 2);
            var source = CreateTexture2DArray(k_Size, k_Size, 2, destination.graphicsFormat);

            Assert.IsTrue(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task MatchingRenderTexture_CanCopy()
        {
            RequireBasicCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format);
            var source = CreateRenderTexture(k_Size, k_Size, k_Format);

            Assert.IsTrue(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }

        [Test]
        public async Task RenderTextureMsaaMismatch_FallsBackToBlit()
        {
            RequireBasicCopySupport();

            var destination = CreateRenderTexture(k_Size, k_Size, k_Format, msaaSamples: 4);
            var source = CreateRenderTexture(k_Size, k_Size, k_Format, msaaSamples: 1);

            Assert.IsFalse(OpenXRLayerUtility.CanTextureUseGraphicsCopy(source, destination));
            await Awaitable.NextFrameAsync();
        }
    }
}
#endif
