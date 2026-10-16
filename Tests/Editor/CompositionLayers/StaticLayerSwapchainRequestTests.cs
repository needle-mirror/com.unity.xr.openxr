#if XR_COMPOSITION_LAYERS_1_0_0_OR_NEWER
using NUnit.Framework;
using LayerRenderInfo = UnityEngine.XR.OpenXR.CompositionLayers.OpenXRCustomLayerHandler<UnityEngine.XR.OpenXR.NativeTypes.XrCompositionLayerQuad>.LayerRenderInfo;

namespace UnityEditor.XR.OpenXR.CompositionLayers.Tests
{
    // Tests for LayerRenderInfo's settle logic, which stops a static layer from acquiring a new swapchain image every
    // frame so the compositor holds its last released image instead of cycling through images written at different
    // times. A static layer settles once the runtime hands it an image it has already written.
    [Category("Level.Unit")]
    [NUnit.Framework.Property("Covers", "CompositionLayers")]
    internal class StaticLayerSwapchainRequestTests
    {
        const int k_SwapchainImageCount = 3;

        // The requiresContinuousWrite argument: a static layer's texture never changes, so it does not need to be
        // written into every image the runtime hands out.
        const bool k_StaticLayer = false;
        const bool k_ContinuousLayer = true;

        // Models the runtime side of the request flow with production's one-frame delay. In frame F the active-layer
        // pass checks NeedsSwapchainImage and requests an image, the graphics thread acquires it during F, and
        // WriteAcquiredImage records it in F+1. So the request check always runs before the previous frame's image is
        // recorded, and a layer that settles has already requested one more image.
        sealed class SimulatedSwapchain
        {
            uint m_NextImage = 1;
            uint m_PendingImage;

            // When false the runtime acquires nothing, as when the request yields texture id 0.
            public bool HandsOutImages = true;

            public bool RunFrame(LayerRenderInfo layerRenderInfo, bool requiresContinuousWrite)
            {
                var requested = layerRenderInfo.NeedsSwapchainImage(requiresContinuousWrite);

                if (m_PendingImage != 0)
                {
                    layerRenderInfo.RenderTextureId = m_PendingImage;
                    layerRenderInfo.RecordAcquiredImage(null, requiresContinuousWrite);
                }

                m_PendingImage = 0;
                if (requested && HandsOutImages)
                {
                    m_PendingImage = m_NextImage;
                    m_NextImage = m_NextImage % k_SwapchainImageCount + 1;
                }

                return requested;
            }

            public int CountRequests(LayerRenderInfo layerRenderInfo, bool requiresContinuousWrite, int frames)
            {
                var requests = 0;
                for (var i = 0; i < frames; i++)
                {
                    if (RunFrame(layerRenderInfo, requiresContinuousWrite))
                    {
                        requests++;
                    }
                }

                return requests;
            }
        }

        // A cold static layer writes every image once (k_SwapchainImageCount requests), requests one more that comes
        // back already written and settles it, and has requested a further image before that record happened.
        const int k_RequestsToSettle = k_SwapchainImageCount + 2;

        // Enough frames to settle and then sit idle for a while.
        const int k_Frames = k_SwapchainImageCount * 3;

        // The guarantee the design depends on: the image the compositor is left holding has the layer's content.
        static void AssertSettledOnWrittenImage(LayerRenderInfo layerRenderInfo)
        {
            Assert.IsTrue(layerRenderInfo.IsSettled);
            Assert.IsTrue(layerRenderInfo.SwapchainImageInfos.TryGetValue(layerRenderInfo.RenderTextureId, out var lastImage),
                "The last acquired image must have been recorded.");
            Assert.IsTrue(lastImage.IsWritten, "The last acquired image must hold the layer's content.");
        }

        [Test]
        [Description("A continuous layer requests a swapchain image every frame and never settles.")]
        public void ContinuousLayer_RequestsEveryFrame()
        {
            var layerRenderInfo = new LayerRenderInfo();
            var swapchain = new SimulatedSwapchain();

            Assert.AreEqual(k_Frames, swapchain.CountRequests(layerRenderInfo, k_ContinuousLayer, k_Frames));
            Assert.IsFalse(layerRenderInfo.IsSettled);
        }

        [Test]
        [Description("A static layer writes each swapchain image once, settles when an already-written image comes back, then stops requesting.")]
        public void StaticLayer_SettlesAfterOneCycleThenStops()
        {
            var layerRenderInfo = new LayerRenderInfo();
            var swapchain = new SimulatedSwapchain();

            Assert.AreEqual(k_RequestsToSettle, swapchain.CountRequests(layerRenderInfo, k_StaticLayer, k_Frames));
            AssertSettledOnWrittenImage(layerRenderInfo);
            Assert.AreEqual(k_SwapchainImageCount, layerRenderInfo.SwapchainImageInfos.Count);
            foreach (var swapchainImageInfo in layerRenderInfo.SwapchainImageInfos.Values)
            {
                Assert.IsTrue(swapchainImageInfo.IsWritten);
            }

            Assert.AreEqual(0, swapchain.CountRequests(layerRenderInfo, k_StaticLayer, k_Frames), "A settled layer must not request again.");
        }

        [Test]
        [Description("A static layer keeps requesting while the runtime hands out no image, records nothing, then settles once images arrive.")]
        public void StaticLayer_KeepsRequestingUntilImagesAreWritten()
        {
            var layerRenderInfo = new LayerRenderInfo();
            var swapchain = new SimulatedSwapchain { HandsOutImages = false };

            Assert.AreEqual(k_Frames, swapchain.CountRequests(layerRenderInfo, k_StaticLayer, k_Frames));
            Assert.IsFalse(layerRenderInfo.IsSettled);
            Assert.AreEqual(0, layerRenderInfo.SwapchainImageInfos.Count, "Frames that yield no image must record nothing.");

            swapchain.HandsOutImages = true;
            Assert.AreEqual(k_RequestsToSettle, swapchain.CountRequests(layerRenderInfo, k_StaticLayer, k_Frames));
            AssertSettledOnWrittenImage(layerRenderInfo);
        }

        [Test]
        [Description("A continuous layer that turns static settles on its next acquisition, since every image is already written.")]
        public void ContinuousLayerTurnedStatic_SettlesOnNextAcquisition()
        {
            var layerRenderInfo = new LayerRenderInfo();
            var swapchain = new SimulatedSwapchain();

            // One extra frame so the last image of the cycle is recorded, not still pending.
            swapchain.CountRequests(layerRenderInfo, k_ContinuousLayer, k_SwapchainImageCount + 1);
            Assert.AreEqual(k_SwapchainImageCount, layerRenderInfo.SwapchainImageInfos.Count);

            Assert.AreEqual(1, swapchain.CountRequests(layerRenderInfo, k_StaticLayer, k_Frames));
            AssertSettledOnWrittenImage(layerRenderInfo);
        }

        [Test]
        [Description("Invalidating a settled layer forgets its written images and makes it request for a full cycle again.")]
        public void InvalidatedSettledLayer_RequestsForFullCycleAgain()
        {
            var layerRenderInfo = new LayerRenderInfo();
            var swapchain = new SimulatedSwapchain();
            swapchain.CountRequests(layerRenderInfo, k_StaticLayer, k_Frames);
            Assert.IsTrue(layerRenderInfo.IsSettled, "Layer should be settled before invalidation.");

            layerRenderInfo.InvalidateSwapchainImages();

            Assert.IsFalse(layerRenderInfo.IsSettled);
            Assert.AreEqual(0, layerRenderInfo.SwapchainImageInfos.Count, "Invalidation must forget which images were written.");
            Assert.AreEqual(k_RequestsToSettle, swapchain.CountRequests(layerRenderInfo, k_StaticLayer, k_Frames));
            AssertSettledOnWrittenImage(layerRenderInfo);
        }
    }
}
#endif
