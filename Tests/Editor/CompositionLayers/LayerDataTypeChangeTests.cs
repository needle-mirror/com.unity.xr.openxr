#if XR_COMPOSITION_LAYERS_1_0_0_OR_NEWER
using System.Collections.Generic;
using NUnit.Framework;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.CompositionLayers;
using UnityEngine.XR.OpenXR.NativeTypes;
using GraphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat;
using LayerRenderInfo = UnityEngine.XR.OpenXR.CompositionLayers.OpenXRCustomLayerHandler<int>.LayerRenderInfo;

namespace UnityEditor.XR.OpenXR.CompositionLayers.Tests
{
    // Each LayerData type has its own handler, so a layer whose type changes at runtime reaches a handler that never
    // saw it created. That handler must build its native layer on the layer's live swapchain right away, or the layer
    // stops rendering.
    [Category("Level.Unit")]
    [NUnit.Framework.Property("Covers", "CompositionLayers")]
    internal class LayerDataTypeChangeTests
    {
        const int k_LayerId = 1;
        const long k_SwapchainFormat = 43;
        const uint k_SwapchainWidth = 512;
        const uint k_SwapchainHeight = 256;
        const ulong k_SwapchainHandle = 7;
        const ulong k_RebuiltSwapchainHandle = 8;

        // Stays on the managed side: every test records a live swapchain first, so no native swapchain is requested.
        sealed class RecordingLayerHandler : OpenXRCustomLayerHandler<int>
        {
            public readonly List<ulong> NativeLayerSwapchainHandles = new();
            public int ActiveNativeLayerCount;
            public bool HasSwapchainSource = true;

            protected override bool CreateSwapchain(
                CompositionLayerManager.LayerInfo layerInfo, out SwapchainCreateInfo swapchainCreateInfo)
            {
                swapchainCreateInfo = new SwapchainCreateInfo(CreateSwapchainInfo());
                return HasSwapchainSource;
            }

            protected override bool CreateNativeLayer(
                CompositionLayerManager.LayerInfo layerInfo,
                SwapchainCreatedOutput swapchainOutput,
                out int nativeLayer)
            {
                NativeLayerSwapchainHandles.Add(swapchainOutput.handle);
                nativeLayer = (int)swapchainOutput.handle;
                return true;
            }

            protected override bool ModifyNativeLayer(
                CompositionLayerManager.LayerInfo layerInfo, ref int nativeLayer) => true;

            protected override bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref int nativeLayer)
            {
                ActiveNativeLayerCount++;
                return true;
            }
        }

        readonly CompositionLayerTestHelpers m_Helpers = new();
        RecordingLayerHandler m_Handler;
        CompositionLayerManager.LayerInfo m_LayerInfo;

        [SetUp]
        public void SetUp()
        {
            Assume.That(
                OpenXRLayerProvider.isStarted, Is.False, "A running OpenXR layer provider owns the live swapchains.");

            m_Handler = new RecordingLayerHandler();
            var layer = m_Helpers.CreateInactiveGameObject("Layer").AddComponent<CompositionLayer>();
            layer.ChangeLayerDataType(typeof(CylinderLayerData));
            m_LayerInfo = new CompositionLayerManager.LayerInfo { Id = k_LayerId, Layer = layer };
        }

        [TearDown]
        public void TearDown()
        {
            m_Handler?.Dispose();
            m_Handler = null;
            OpenXRLayerSwapchainRegistry.Clear();
            m_Helpers.Cleanup();
        }

        static XrSwapchainCreateInfo CreateSwapchainInfo() => new()
        {
            Format = k_SwapchainFormat,
            SampleCount = 1,
            Width = k_SwapchainWidth,
            Height = k_SwapchainHeight,
            FaceCount = 1,
            ArraySize = 1,
            MipCount = 1,
        };

        // Models the swapchain the layer's previous type's handler created.
        static void RecordLiveSwapchain(ulong handle) =>
            OpenXRLayerSwapchainRegistry.Record(
                k_LayerId,
                CreateSwapchainInfo(),
                isExternalSurface: false,
                isStereo: false,
                handle,
                secondStereoHandle: 0);

        [Test]
        [Description("A modified layer the handler never created gets a native layer on its live swapchain at once.")]
        public void ModifiedLayerNotCreatedByHandler_BuildsNativeLayerOnLiveSwapchain()
        {
            RecordLiveSwapchain(k_SwapchainHandle);

            m_Handler.ModifyLayer(m_LayerInfo);

            Assert.That(m_Handler.NativeLayerSwapchainHandles, Is.EqualTo(new[] { k_SwapchainHandle }));
        }

        [Test]
        [Description("A layer whose type changed after this frame's update is still submitted in this frame.")]
        public void ActiveLayerNotCreatedByHandler_IsSubmittedInSameFrame()
        {
            RecordLiveSwapchain(k_SwapchainHandle);

            m_Handler.SetActiveLayer(m_LayerInfo);

            Assert.That(m_Handler.NativeLayerSwapchainHandles, Is.EqualTo(new[] { k_SwapchainHandle }));
            Assert.That(m_Handler.ActiveNativeLayerCount, Is.EqualTo(1));
        }

        [Test]
        [Description("A layer whose swapchain another handler rebuilt gets its native layer rebuilt on the new one.")]
        public void LayerSwapchainRebuiltElsewhere_RebuildsNativeLayerOnNewSwapchain()
        {
            RecordLiveSwapchain(k_SwapchainHandle);
            m_Handler.ModifyLayer(m_LayerInfo);
            RecordLiveSwapchain(k_RebuiltSwapchainHandle);

            m_Handler.SetActiveLayer(m_LayerInfo);

            Assert.That(
                m_Handler.NativeLayerSwapchainHandles,
                Is.EqualTo(new[] { k_SwapchainHandle, k_RebuiltSwapchainHandle }));
            Assert.That(m_Handler.ActiveNativeLayerCount, Is.EqualTo(1));
        }

        [Test]
        [Description("A layer whose swapchain was destroyed and is being rebuilt elsewhere is not submitted.")]
        public void LayerSwapchainBeingRebuiltElsewhere_IsNotSubmitted()
        {
            RecordLiveSwapchain(k_SwapchainHandle);
            m_Handler.ModifyLayer(m_LayerInfo);
            OpenXRLayerSwapchainRegistry.Remove(k_LayerId);

            m_Handler.SetActiveLayer(m_LayerInfo);

            Assert.That(m_Handler.ActiveNativeLayerCount, Is.Zero);
        }

        [Test]
        [Description("A layer whose swapchain was replaced elsewhere and that cannot be rebuilt is not submitted.")]
        public void LayerSwapchainReplacedButNotRebuilt_IsNotSubmitted()
        {
            RecordLiveSwapchain(k_SwapchainHandle);
            m_Handler.ModifyLayer(m_LayerInfo);
            RecordLiveSwapchain(k_RebuiltSwapchainHandle);
            m_Handler.HasSwapchainSource = false;

            m_Handler.SetActiveLayer(m_LayerInfo);

            Assert.That(m_Handler.ActiveNativeLayerCount, Is.Zero);
        }

        [Test]
        [Description("A live swapchain another layer type needs is left in place, and the layer is not submitted.")]
        public void LiveSwapchainOfOtherDescription_IsNotAdoptedOrReplaced()
        {
            OpenXRLayerSwapchainRegistry.Record(
                k_LayerId,
                CreateSwapchainInfo(),
                isExternalSurface: false,
                isStereo: true,
                k_SwapchainHandle,
                k_RebuiltSwapchainHandle);

            m_Handler.ModifyLayer(m_LayerInfo);
            m_Handler.SetActiveLayer(m_LayerInfo);

            Assert.That(m_Handler.NativeLayerSwapchainHandles, Is.Empty);
            Assert.That(m_Handler.ActiveNativeLayerCount, Is.Zero);
            Assert.IsTrue(OpenXRLayerSwapchainRegistry.TryGet(k_LayerId, out var swapchain));
            Assert.That(swapchain.Handle, Is.EqualTo(k_SwapchainHandle));
        }

        [Test]
        [Description("A handler is told another handler used the swapchain only when one did since it last did.")]
        public void SwapchainUser_IsReportedChangedOnlyAfterAnotherHandlerUsedIt()
        {
            var handler = new object();
            var otherTypeHandler = new object();

            Assert.IsFalse(OpenXRLayerSwapchainRegistry.SetUser(k_LayerId, handler), "First use");
            Assert.IsFalse(OpenXRLayerSwapchainRegistry.SetUser(k_LayerId, handler), "Same handler again");
            Assert.IsTrue(OpenXRLayerSwapchainRegistry.SetUser(k_LayerId, otherTypeHandler), "Other type's handler");
            Assert.IsTrue(OpenXRLayerSwapchainRegistry.SetUser(k_LayerId, handler), "Returning handler");

            OpenXRLayerSwapchainRegistry.Remove(k_LayerId);
            Assert.IsFalse(OpenXRLayerSwapchainRegistry.SetUser(k_LayerId, otherTypeHandler), "After removal");
        }

        [Test]
        [Description("A returning handler forgets images written elsewhere and keeps the texture to detect a resize.")]
        public void ImagesWrittenElsewhere_AreForgottenAndTextureIsKept()
        {
            var texture = m_Helpers.CreateTexture2D(
                (int)k_SwapchainWidth, (int)k_SwapchainHeight, GraphicsFormat.R8G8B8A8_UNorm);
            var layerRenderInfo = new LayerRenderInfo
            {
                Texture = texture,
                SwapchainMipCount = 2,
                IsSettled = true,
                HasAcquiredImage = true,
            };
            layerRenderInfo.SwapchainImageInfos.Add(1, new LayerRenderInfo.SwapchainImageInfo { IsWritten = true });

            layerRenderInfo.InvalidateImagesWrittenElsewhere();

            Assert.That(layerRenderInfo.Texture, Is.SameAs(texture));
            Assert.That(layerRenderInfo.SwapchainMipCount, Is.EqualTo(2));
            Assert.That(layerRenderInfo.SwapchainImageInfos, Is.Empty);
            Assert.IsFalse(layerRenderInfo.IsSettled);
            Assert.IsFalse(layerRenderInfo.HasAcquiredImage);
        }

        static IEnumerable<TestCaseData> MismatchedSwapchainDescriptions()
        {
            var differentWidth = CreateSwapchainInfo();
            differentWidth.Width = k_SwapchainWidth * 2;
            yield return new TestCaseData(differentWidth, false, false).SetName("{m}(DifferentWidth)");

            var differentHeight = CreateSwapchainInfo();
            differentHeight.Height = k_SwapchainHeight * 2;
            yield return new TestCaseData(differentHeight, false, false).SetName("{m}(DifferentHeight)");

            var differentFormat = CreateSwapchainInfo();
            differentFormat.Format = k_SwapchainFormat + 1;
            yield return new TestCaseData(differentFormat, false, false).SetName("{m}(DifferentFormat)");

            var differentSampleCount = CreateSwapchainInfo();
            differentSampleCount.SampleCount = 4;
            yield return new TestCaseData(differentSampleCount, false, false).SetName("{m}(DifferentSampleCount)");

            var differentArraySize = CreateSwapchainInfo();
            differentArraySize.ArraySize = 2;
            yield return new TestCaseData(differentArraySize, false, false).SetName("{m}(DifferentArraySize)");

            var differentUsage = CreateSwapchainInfo();
            differentUsage.UsageFlags = 1;
            yield return new TestCaseData(differentUsage, false, false).SetName("{m}(DifferentUsage)");

            var differentCreateFlags = CreateSwapchainInfo();
            differentCreateFlags.CreateFlags = 1;
            yield return new TestCaseData(differentCreateFlags, false, false).SetName("{m}(DifferentCreateFlags)");

            var differentMips = CreateSwapchainInfo();
            differentMips.MipCount = 2;
            yield return new TestCaseData(differentMips, false, false).SetName("{m}(DifferentMips)");

            var differentFaces = CreateSwapchainInfo();
            differentFaces.FaceCount = 6;
            yield return new TestCaseData(differentFaces, false, false).SetName("{m}(DifferentFaces)");

            yield return new TestCaseData(CreateSwapchainInfo(), false, true).SetName("{m}(Stereo)");
            yield return new TestCaseData(CreateSwapchainInfo(), true, false).SetName("{m}(ExternalSurface)");
        }

        [TestCaseSource(nameof(MismatchedSwapchainDescriptions))]
        [Description("A swapchain is not reused for a description whose images would differ.")]
        public void Swapchain_IsNotReusableForMismatchedDescription(
            XrSwapchainCreateInfo createInfo, bool isExternalSurface, bool isStereo)
        {
            RecordLiveSwapchain(k_SwapchainHandle);
            Assert.IsTrue(OpenXRLayerSwapchainRegistry.TryGet(k_LayerId, out var swapchain));

            Assert.IsTrue(swapchain.CanBeReusedFor(CreateSwapchainInfo(), isExternalSurface: false, isStereo: false));
            Assert.IsFalse(swapchain.CanBeReusedFor(createInfo, isExternalSurface, isStereo));
        }
    }
}
#endif
