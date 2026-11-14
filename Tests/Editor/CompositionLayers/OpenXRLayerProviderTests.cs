#if XR_COMPOSITION_LAYERS_1_0_0_OR_NEWER
using System.Collections.Generic;
using NUnit.Framework;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.CompositionLayers;

namespace UnityEditor.XR.OpenXR.CompositionLayers.Tests
{
    // Tests that <see cref="OpenXRLayerProvider"/> skips layers with no LayerData. Composition Layers versions
    // that still report these layers to providers would otherwise throw every frame.
    [Category("Level.Unit")]
    [NUnit.Framework.Property("Covers", "CompositionLayers")]
    internal class OpenXRLayerProviderTests
    {
        // Records the layer ids passed to each handler callback.
        sealed class RecordingLayerHandler : OpenXRLayerProvider.ILayerHandler
        {
            public readonly List<int> Created = new List<int>();
            public readonly List<int> Modified = new List<int>();
            public readonly List<int> Active = new List<int>();

            public void OnUpdate() { }
            public void CreateLayer(CompositionLayerManager.LayerInfo layerInfo) => Created.Add(layerInfo.Id);
            public void RemoveLayer(int id) { }
            public void ModifyLayer(CompositionLayerManager.LayerInfo layerInfo) => Modified.Add(layerInfo.Id);
            public void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo) => Active.Add(layerInfo.Id);
        }

        const int k_NoLayerDataId = 1;
        const int k_QuadLayerId = 2;

        readonly CompositionLayerTestHelpers m_Helpers = new CompositionLayerTestHelpers();

        OpenXRLayerProvider m_Provider;
        RecordingLayerHandler m_QuadHandler;
        List<CompositionLayerManager.LayerInfo> m_Layers;

        [SetUp]
        public void SetUp()
        {
            m_Provider = new OpenXRLayerProvider();

            // Replace the built-in quad handler so no layer reaches native code.
            m_QuadHandler = new RecordingLayerHandler();
            OpenXRLayerProvider.RegisterLayerHandler(typeof(QuadLayerData), m_QuadHandler);

            m_Layers = new List<CompositionLayerManager.LayerInfo>
            {
                CreateLayerInfo(k_NoLayerDataId, null),
                CreateLayerInfo(k_QuadLayerId, new QuadLayerData()),
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Clearing the active layers unsubscribes the provider from Application.onBeforeRender.
            m_Provider?.UpdateLayers(null, null, null, null);
            m_Provider?.Dispose();
            m_Helpers.Cleanup();
        }

        CompositionLayerManager.LayerInfo CreateLayerInfo(int id, LayerData layerData)
        {
            // The GameObject stays inactive so the layer never registers with CompositionLayerManager.
            var gameObject = m_Helpers.CreateInactiveGameObject($"Layer {id}");
            var layer = gameObject.AddComponent<CompositionLayer>();
            layer.ChangeLayerDataType(layerData);
            return new CompositionLayerManager.LayerInfo { Id = id, Layer = layer };
        }

        [Test]
        [Description("A created layer with no layer type is skipped without throwing, and other created layers " +
                     "still reach their handler.")]
        public void CreatedLayers_WithNoLayerData_AreSkipped()
        {
            Assert.DoesNotThrow(() => m_Provider.UpdateLayers(m_Layers, null, null, null));

            CollectionAssert.AreEqual(new[] { k_QuadLayerId }, m_QuadHandler.Created);
        }

        [Test]
        [Description("A modified layer with no layer type is skipped without throwing, and other modified layers " +
                     "still reach their handler.")]
        public void ModifiedLayers_WithNoLayerData_AreSkipped()
        {
            Assert.DoesNotThrow(() => m_Provider.UpdateLayers(null, null, m_Layers, null));

            CollectionAssert.AreEqual(new[] { k_QuadLayerId }, m_QuadHandler.Modified);
        }

        [Test]
        [Description("An active layer with no layer type is skipped without throwing before render, and other " +
                     "active layers still reach their handler.")]
        public void ActiveLayers_WithNoLayerData_AreSkipped()
        {
            m_Provider.UpdateLayers(null, null, null, m_Layers);

            Assert.DoesNotThrow(() => m_Provider.HandleBeforeRender());

            CollectionAssert.AreEqual(new[] { k_QuadLayerId }, m_QuadHandler.Active);
        }
    }
}
#endif
