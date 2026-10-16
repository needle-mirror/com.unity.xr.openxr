#if XR_COMPOSITION_LAYERS_2_6_OR_GREATER
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.OpenXR.CompositionLayers;
using UnityEngine.XR.OpenXR.NativeTypes;
using Unity.XR.CompositionLayers.Extensions;
using GraphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat;
using TextureCreationFlags = UnityEngine.Experimental.Rendering.TextureCreationFlags;
using GraphicsDeviceType = UnityEngine.Rendering.GraphicsDeviceType;
using TextureDimension = UnityEngine.Rendering.TextureDimension;
#if LIFECYCLE_APIS_AVAILABLE
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.OpenXR.CompositionLayers.Tests
{
    [Category("Level.Unit")]
    [NUnit.Framework.Property("Covers", "CompositionLayers")]
    internal class CompositionLayerMipMapTests
    {
        const int k_Size = 64;
        const GraphicsFormat k_Format = GraphicsFormat.R8G8B8A8_UNorm;

        readonly CompositionLayerTestHelpers m_Helpers = new CompositionLayerTestHelpers();

        [SetUp]
        public void SetUp() => OpenXRLayerUtility.ClearRenderTextureCache();

        [TearDown]
        public void TearDown() => m_Helpers.Cleanup();

        [Test]
        [Description("Usage flags for a single-mip color swapchain include SAMPLED but neither transfer bit.")]
        public void SingleMipSwapchain_UsageFlags_OmitsTransferBits()
        {
            var flags = OpenXRLayerUtility.GetColorSwapchainUsageFlags(1);
            var transferBits = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_TRANSFER_SRC_BIT
                | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_TRANSFER_DST_BIT);

            Assert.AreEqual(0ul, flags & transferBits, "A single-mip swapchain must not request the transfer bits.");
            Assert.AreNotEqual(0ul, flags & (ulong)XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT,
                "The sampled bit is always required.");
        }

        [Test]
        [Description("A zero mip count, as used by external-surface swapchains, produces the same flags as a " +
                     "single mip: SAMPLED without the transfer bits.")]
        public void ZeroMipSwapchain_UsageFlags_OmitsTransferBits()
        {
            // 0 is the degenerate count used by external-surface swapchains and should behave like a single mip.
            var flags = OpenXRLayerUtility.GetColorSwapchainUsageFlags(0);
            var transferBits = (ulong)(XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_TRANSFER_SRC_BIT
                | XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_TRANSFER_DST_BIT);

            Assert.AreEqual(0ul, flags & transferBits, "A zero-mip swapchain must not request the transfer bits.");
            Assert.AreNotEqual(0ul, flags & (ulong)XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_SAMPLED_BIT,
                "The sampled bit is always required.");
        }

        [Test]
        [Description("Usage flags for a mipped swapchain include both TRANSFER_SRC and TRANSFER_DST so " +
                     "AutoGenerate and per-mip copies can work.")]
        public void MultiMipSwapchain_UsageFlags_IncludesBothTransferBits()
        {
            var flags = OpenXRLayerUtility.GetColorSwapchainUsageFlags(4);

            Assert.AreNotEqual(0ul, flags & (ulong)XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_TRANSFER_SRC_BIT,
                "A mipped swapchain must request TRANSFER_SRC for AutoGenerate.");
            Assert.AreNotEqual(0ul, flags & (ulong)XrSwapchainUsageFlags.XR_SWAPCHAIN_USAGE_TRANSFER_DST_BIT,
                "A mipped swapchain must request TRANSFER_DST for per-mip copies.");
        }

        [Description("GetSwapchainMipCount is 1 for mode None or a single-mip source, and matches the source " +
                     "mip count for CopyFromSource and AutoGenerate only where the engine supports mipmaps.")]
        [TestCase(TexturesExtension.MipMapModeEnum.None, 4, 1u)]
#if UNITY_ENGINE_MIPMAPS_SUPPORT
        [TestCase(TexturesExtension.MipMapModeEnum.CopyFromSource, 4, 4u)]
        [TestCase(TexturesExtension.MipMapModeEnum.AutoGenerate, 4, 4u)]
#else
        [TestCase(TexturesExtension.MipMapModeEnum.CopyFromSource, 4, 1u)]
        [TestCase(TexturesExtension.MipMapModeEnum.AutoGenerate, 4, 1u)]
#endif
        [TestCase(TexturesExtension.MipMapModeEnum.CopyFromSource, 1, 1u)]
        public void GetSwapchainMipCount_MatchesModeAndSource(
            TexturesExtension.MipMapModeEnum mipMapMode, int sourceMipCount, uint expected)
        {
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, k_Format, sourceMipCount);
            var extension = CreateTexturesExtension(source, mipMapMode);

            Assert.AreEqual(expected, OpenXRLayerUtility.GetSwapchainMipCount(extension));
        }

        [Test]
        [Description("A null textures extension yields one mip level and logs a warning.")]
        public void GetSwapchainMipCount_NullExtension_ReturnsOne()
        {
            LogAssert.Expect(LogType.Warning, "GetSwapchainMipCount: textures extension or its texture is null. Defaulting to a single mip level.");
            Assert.AreEqual(1u, OpenXRLayerUtility.GetSwapchainMipCount((TexturesExtension)null));
        }

        [Test]
        [Description("A textures extension with no left texture yields one mip level and logs a warning.")]
        public void GetSwapchainMipCount_NullLeftTexture_ReturnsOne()
        {
            var extension = CreateTexturesExtension(null, TexturesExtension.MipMapModeEnum.CopyFromSource);

            LogAssert.Expect(LogType.Warning, "GetSwapchainMipCount: textures extension or its texture is null. Defaulting to a single mip level.");
            Assert.AreEqual(1u, OpenXRLayerUtility.GetSwapchainMipCount(extension));
        }

        // The write tests below exercise the write path in isolation from swapchain allocation.

        [Test]
        [NUnit.Framework.Property("Requires", "Gpu")]
        [Description("CopyFromSource writes each authored mip of a Texture2D into the matching destination " +
                     "mip unchanged.")]
        public void WriteToRenderTexture_CopyFromSource_PreservesEachAuthoredMip()
        {
            m_Helpers.RequireCopySupport();
            RequirePerMipWrites();

            const int mipCount = 4;
            var source = CreatePerMipColoredTexture(k_Size, mipCount);
            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, mipCount);

            OpenXRLayerUtility.WriteToRenderTexture(source, destination, MipMapWriteMode.CopyFromSource);

            for (int mip = 0; mip < mipCount; mip++)
            {
                var actual = ReadRenderTextureMip(destination, mip);
                AssertColorsApproximatelyEqual(k_MipColors[mip], actual, $"mip {mip}");
            }
        }

        [Test]
        [NUnit.Framework.Property("Requires", "Gpu")]
        [Description("CopyFromSource writes each authored mip of a RenderTexture source into the matching " +
                     "destination mip unchanged.")]
        public void WriteToRenderTexture_CopyFromSource_RenderTextureSource_PreservesEachAuthoredMip()
        {
            m_Helpers.RequireCopySupport();
            RequirePerMipWrites();

            const int mipCount = 4;
            var source = CreatePerMipColoredRenderTexture(k_Size, mipCount);
            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, mipCount);

            OpenXRLayerUtility.WriteToRenderTexture(source, destination, MipMapWriteMode.CopyFromSource);

            for (int mip = 0; mip < mipCount; mip++)
            {
                var actual = ReadRenderTextureMip(destination, mip);
                AssertColorsApproximatelyEqual(k_MipColors[mip], actual, $"mip {mip}");
            }
        }

        // CopyFromSource must land each mip in the same orientation as the None path. Sampling all four
        // corners of a per-corner-colored pattern catches a vertical flip, a horizontal mirror, or both.
        [TestCase(false, TestName = "WriteToRenderTexture_CopyFromSource_MatchesReferenceOrientationPerMip_SwapchainTexture")]
        [TestCase(true, TestName = "WriteToRenderTexture_CopyFromSource_MatchesReferenceOrientationPerMip_OrdinaryRenderTexture")]
        [NUnit.Framework.Property("Requires", "Gpu")]
        [Description("Each mip written by CopyFromSource has the same orientation as a single-mip None write " +
                     "of that level, so no mip lands vertically flipped or horizontally mirrored. Covers both " +
                     "swapchain-style render textures (no AllowVerticalFlip) and ordinary ones.")]
        public void WriteToRenderTexture_CopyFromSource_MatchesReferenceOrientationPerMip(bool allowVerticalFlip)
        {
            m_Helpers.RequireCopySupport();
            RequirePerMipWrites();

            const int mipCount = 4;
            var source = CreatePerMipQuadrantTexture(k_Size, mipCount);

            var subject = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, mipCount, allowVerticalFlip: allowVerticalFlip);
            OpenXRLayerUtility.WriteToRenderTexture(source, subject, MipMapWriteMode.CopyFromSource);

            // (u, v) sample points for the four corners, kept off the exact edges to avoid clamp ambiguity.
            var corners = new[]
            {
                (u: 0.25f, v: 0.25f, name: "bottom-left"),
                (u: 0.75f, v: 0.25f, name: "bottom-right"),
                (u: 0.25f, v: 0.75f, name: "top-left"),
                (u: 0.75f, v: 0.75f, name: "top-right"),
            };

            for (int mip = 0; mip < mipCount; mip++)
            {
                int mipSize = Mathf.Max(1, k_Size >> mip);
                if (mipSize < 2)
                {
                    continue;
                }

                var referenceSource = CreateSingleMipQuadrant(mipSize, mip);
                var reference = m_Helpers.CreateRenderTexture(mipSize, mipSize, k_Format, 1, allowVerticalFlip: allowVerticalFlip);
                OpenXRLayerUtility.WriteToRenderTexture(referenceSource, reference, MipMapWriteMode.None);

                foreach (var corner in corners)
                {
                    var referenceColor = ReadRenderTextureMipAt(reference, 0, corner.u, corner.v);
                    var subjectColor = ReadRenderTextureMipAt(subject, mip, corner.u, corner.v);
                    AssertColorsApproximatelyEqual(referenceColor, subjectColor, $"mip {mip} {corner.name}");
                }

                // Without distinct corners a flip or mirror would be undetectable.
                var bottomLeft = ReadRenderTextureMipAt(reference, 0, corners[0].u, corners[0].v);
                var bottomRight = ReadRenderTextureMipAt(reference, 0, corners[1].u, corners[1].v);
                var topLeft = ReadRenderTextureMipAt(reference, 0, corners[2].u, corners[2].v);
                Assert.IsTrue(ColorsDiffer(bottomLeft, topLeft) && ColorsDiffer(bottomLeft, bottomRight),
                    $"mip {mip}: reference corners are not distinct enough to detect a flip or mirror.");
            }
        }

        [Test]
        [NUnit.Framework.Property("Requires", "Gpu")]
        [Description("AutoGenerate fills every destination mip from a solid single-mip source.")]
        public void WriteToRenderTexture_AutoGenerate_PopulatesAllMips()
        {
            m_Helpers.RequireCopySupport();

            const int mipCount = 4;
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, k_Format);
            var solid = new Color[k_Size * k_Size];
            for (int i = 0; i < solid.Length; i++)
                solid[i] = k_MipColors[0];
            source.SetPixels(solid);
            source.Apply(false);

            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, mipCount);

            OpenXRLayerUtility.WriteToRenderTexture(source, destination, MipMapWriteMode.AutoGenerate);

            // A solid source downsamples to the same color at every level, so every generated mip must match it.
            for (int mip = 0; mip < mipCount; mip++)
            {
                var actual = ReadRenderTextureMip(destination, mip);
                AssertColorsApproximatelyEqual(k_MipColors[0], actual, $"generated mip {mip}");
            }
        }

        [Test]
        [NUnit.Framework.Property("Requires", "Gpu")]
        [Description("When the source has fewer mips than the destination, CopyFromSource falls back to Blit " +
                     "plus GenerateMips and warns once per render texture rather than on every write.")]
        public void WriteToRenderTexture_CopyFromSource_FallbackWarnsOncePerRenderTexture()
        {
            m_Helpers.RequireCopySupport();
            RequirePerMipWrites();

            // Fewer source mips than the destination forces CopyFromSource to fall back to Blit + GenerateMips.
            var source = m_Helpers.CreateTexture2D(k_Size, k_Size, k_Format, mipCount: 1);
            var destination = m_Helpers.CreateRenderTexture(k_Size, k_Size, k_Format, mipCount: 4);

            LogAssert.Expect(LogType.Warning, new Regex("CopyFromSource could not copy the source mips directly"));

            // The warning must fire once and then be suppressed; a second warning would be an unexpected log
            // and fail the test.
            OpenXRLayerUtility.WriteToRenderTexture(source, destination, MipMapWriteMode.CopyFromSource);
            OpenXRLayerUtility.WriteToRenderTexture(source, destination, MipMapWriteMode.CopyFromSource);
        }

        [Test]
        [NUnit.Framework.Property("Requires", "Gpu")]
        [Description("CopyFromSource writes every face and mip of a cubemap source into the matching slots " +
                     "of a cube render texture unchanged.")]
        public void WriteToRenderTexture_Cube_CopyFromSource_PreservesEachFaceAndMip()
        {
            m_Helpers.RequireCopySupport();
            RequirePerMipWrites();

            const int size = 8;
            var source = CreatePerMipColoredCubemap(size, out int mipCount);
            var destination = CreateCubeRenderTexture(size, mipCount);

            OpenXRLayerUtility.WriteToRenderTexture(source, destination, MipMapWriteMode.CopyFromSource);

            for (int face = 0; face < k_CubeFaceCount; face++)
            {
                for (int mip = 0; mip < mipCount; mip++)
                {
                    var actual = ReadCubeFaceMip(destination, face, mip);
                    AssertColorsApproximatelyEqual(CubeFaceMipColor(face, mip, mipCount), actual, $"face {face} mip {mip}");
                }
            }
        }

        TexturesExtension CreateTexturesExtension(Texture leftTexture, TexturesExtension.MipMapModeEnum mipMapMode)
        {
            var gameObject = m_Helpers.CreateInactiveGameObject("MipMapTestLayer");
            var extension = gameObject.AddComponent<TexturesExtension>();
            extension.LeftTexture = leftTexture;
            extension.MipMapMode = mipMapMode;
            return extension;
        }

        // Individual swapchain mip levels can only be written on graphics APIs that expose per-mip surfaces.
        // OpenGL(ES) lands every per-mip write in mip 0, so CopyFromSource silently falls back.
        static void RequirePerMipWrites()
        {
            var api = SystemInfo.graphicsDeviceType;
            Assume.That(api != GraphicsDeviceType.OpenGLES3 && api != GraphicsDeviceType.OpenGLCore,
                "Requires a graphics API with per-mip swapchain writes (not OpenGL/GLES).");
        }

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly Color[] k_MipColors =
        {
            new Color(1f, 0f, 0f, 1f), // mip 0: red
            new Color(0f, 1f, 0f, 1f), // mip 1: green
            new Color(0f, 0f, 1f, 1f), // mip 2: blue
            new Color(1f, 1f, 0f, 1f), // mip 3: yellow
        };

#if LIFECYCLE_APIS_AVAILABLE
        [NoAutoStaticsCleanup]
#endif
        static readonly Color[] k_MipColorsBottom =
        {
            new Color(1f, 0f, 1f, 1f), // mip 0: magenta
            new Color(0f, 1f, 1f, 1f), // mip 1: cyan
            new Color(0.5f, 0.5f, 0.5f, 1f), // mip 2: gray
            new Color(1f, 1f, 1f, 1f), // mip 3: white
        };

        Texture2D CreatePerMipColoredTexture(int size, int mipCount)
        {
            var texture = m_Helpers.CreateTexture2D(size, size, k_Format, mipCount);
            for (int mip = 0; mip < mipCount; mip++)
            {
                int mipWidth = Mathf.Max(1, size >> mip);
                int mipHeight = Mathf.Max(1, size >> mip);
                var pixels = new Color[mipWidth * mipHeight];
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = k_MipColors[mip];
                texture.SetPixels(pixels, mip);
            }
            texture.Apply(false);
            return texture;
        }

        // The four quadrant colors for a mip: a distinct color in each corner so an orientation check can tell a
        // vertical flip from a horizontal mirror. Texture rows run bottom-to-top, so y < size/2 is the bottom half.
        static Color QuadrantColor(int mip, bool bottom, bool left)
        {
            var colors = bottom ? k_MipColorsBottom : k_MipColors;
            var baseColor = colors[mip];
            // Darken the left column so left and right corners differ within the same top/bottom band.
            return left ? new Color(baseColor.r * 0.5f, baseColor.g * 0.5f, baseColor.b * 0.5f, baseColor.a) : baseColor;
        }

        static void FillQuadrants(Color[] pixels, int width, int height, int mip)
        {
            for (var y = 0; y < height; y++)
            {
                var bottom = y < height / 2;
                for (var x = 0; x < width; x++)
                {
                    var left = x < width / 2;
                    pixels[y * width + x] = QuadrantColor(mip, bottom, left);
                }
            }
        }

        // Each mip is a four-quadrant pattern (see QuadrantColor), preserved into the authored mip chain.
        Texture2D CreatePerMipQuadrantTexture(int size, int mipCount)
        {
            var texture = m_Helpers.CreateTexture2D(size, size, k_Format, mipCount);
            for (int mip = 0; mip < mipCount; mip++)
            {
                int mipWidth = Mathf.Max(1, size >> mip);
                int mipHeight = Mathf.Max(1, size >> mip);
                var pixels = new Color[mipWidth * mipHeight];
                FillQuadrants(pixels, mipWidth, mipHeight, mip);
                texture.SetPixels(pixels, mip);
            }
            texture.Apply(false);
            return texture;
        }

        // Single-mip counterpart of one CreatePerMipQuadrantTexture level, used as the orientation reference.
        Texture2D CreateSingleMipQuadrant(int size, int mip)
        {
            var texture = m_Helpers.CreateTexture2D(size, size, k_Format, 1);
            var pixels = new Color[size * size];
            FillQuadrants(pixels, size, size, mip);
            texture.SetPixels(pixels);
            texture.Apply(false);
            return texture;
        }

        RenderTexture CreatePerMipColoredRenderTexture(int size, int mipCount)
        {
            var authored = CreatePerMipColoredTexture(size, mipCount);
            var renderTexture = m_Helpers.CreateRenderTexture(size, size, k_Format, mipCount);
            for (int mip = 0; mip < mipCount; mip++)
                Graphics.CopyTexture(authored, 0, mip, renderTexture, 0, mip);
            return renderTexture;
        }

        const int k_CubeFaceCount = 6;

        static Color CubeFaceMipColor(int face, int mip, int mipCount) =>
            new Color((face + 1f) / (k_CubeFaceCount + 1f), (mip + 1f) / (mipCount + 1f), 0.25f, 1f);

        Cubemap CreatePerMipColoredCubemap(int size, out int mipCount)
        {
            var cubemap = new Cubemap(size, k_Format, TextureCreationFlags.MipChain);
            m_Helpers.Track(cubemap);
            mipCount = cubemap.mipmapCount;

            for (int face = 0; face < k_CubeFaceCount; face++)
            {
                for (int mip = 0; mip < mipCount; mip++)
                {
                    int mipWidth = Mathf.Max(1, size >> mip);
                    int mipHeight = Mathf.Max(1, size >> mip);
                    var pixels = new Color[mipWidth * mipHeight];
                    var color = CubeFaceMipColor(face, mip, mipCount);
                    for (int i = 0; i < pixels.Length; i++)
                        pixels[i] = color;
                    cubemap.SetPixels(pixels, (CubemapFace)face, mip);
                }
            }
            cubemap.Apply(false);
            return cubemap;
        }

        RenderTexture CreateCubeRenderTexture(int size, int mipCount)
        {
            var descriptor = new RenderTextureDescriptor(size, size, k_Format, 0)
            {
                dimension = TextureDimension.Cube,
                mipCount = mipCount,
                useMipMap = mipCount != 1,
                autoGenerateMips = false,
            };
            var renderTexture = new RenderTexture(descriptor);
            renderTexture.Create();
            m_Helpers.Track(renderTexture);
            return renderTexture;
        }

        static Color ReadRenderTextureMip(RenderTexture renderTexture, int mip) =>
            ReadRenderTextureMipAt(renderTexture, mip, 0f, 0f);

        static Color ReadRenderTextureMipAt(RenderTexture renderTexture, int mip, float u, float v)
        {
            int mipWidth = Mathf.Max(1, renderTexture.width >> mip);
            int mipHeight = Mathf.Max(1, renderTexture.height >> mip);

            var temp = RenderTexture.GetTemporary(new RenderTextureDescriptor(mipWidth, mipHeight, renderTexture.graphicsFormat, 0));
            Graphics.CopyTexture(renderTexture, 0, mip, temp, 0, 0);

            var color = ReadActivePixel(temp, mipWidth, mipHeight, u, v);
            RenderTexture.ReleaseTemporary(temp);
            return color;
        }

        static Color ReadCubeFaceMip(RenderTexture cubeRenderTexture, int face, int mip)
        {
            int mipWidth = Mathf.Max(1, cubeRenderTexture.width >> mip);
            int mipHeight = Mathf.Max(1, cubeRenderTexture.height >> mip);

            var temp = RenderTexture.GetTemporary(new RenderTextureDescriptor(mipWidth, mipHeight, cubeRenderTexture.graphicsFormat, 0));
            Graphics.CopyTexture(cubeRenderTexture, face, mip, temp, 0, 0);

            var color = ReadActivePixel(temp, mipWidth, mipHeight, 0f, 0f);
            RenderTexture.ReleaseTemporary(temp);
            return color;
        }

        static Color ReadActivePixel(RenderTexture source, int width, int height, float u, float v)
        {
            var previousActive = RenderTexture.active;
            RenderTexture.active = source;
            var readback = new Texture2D(width, height, TextureFormat.RGBA32, false);
            readback.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            readback.Apply();
            RenderTexture.active = previousActive;

            int x = Mathf.Clamp(Mathf.FloorToInt(u * width), 0, width - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(v * height), 0, height - 1);
            var color = readback.GetPixel(x, y);
            Object.DestroyImmediate(readback);
            return color;
        }

        static bool ColorsDiffer(Color a, Color b)
        {
            const float tolerance = 2f / 255f;
            return Mathf.Abs(a.r - b.r) > tolerance
                || Mathf.Abs(a.g - b.g) > tolerance
                || Mathf.Abs(a.b - b.b) > tolerance;
        }

        static void AssertColorsApproximatelyEqual(Color expected, Color actual, string context)
        {
            const float tolerance = 2f / 255f;
            Assert.That(actual.r, Is.EqualTo(expected.r).Within(tolerance), $"{context} (r)");
            Assert.That(actual.g, Is.EqualTo(expected.g).Within(tolerance), $"{context} (g)");
            Assert.That(actual.b, Is.EqualTo(expected.b).Within(tolerance), $"{context} (b)");
        }
    }
}
#endif
