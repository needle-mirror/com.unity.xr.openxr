#if XR_COMPOSITION_LAYERS_1_0_0_OR_NEWER
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CopyTextureSupport = UnityEngine.Rendering.CopyTextureSupport;
using TextureDimension = UnityEngine.Rendering.TextureDimension;
using GraphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat;
using TextureCreationFlags = UnityEngine.Experimental.Rendering.TextureCreationFlags;

namespace UnityEditor.XR.OpenXR.CompositionLayers.Tests
{
    // Shared fixtures for composition-layer editor tests.
    // Compose it into a test class with a field and delegate that class's [TearDown] to Cleanup().
    internal sealed class CompositionLayerTestHelpers
    {
        readonly List<Object> m_Created = new List<Object>();

        // Destroys every object created (or tracked) through this helper. Call from the owning fixture's [TearDown].
        public void Cleanup()
        {
            foreach (var obj in m_Created)
            {
                if (obj != null)
                    Object.DestroyImmediate(obj);
            }
            m_Created.Clear();
        }

        public GameObject CreateInactiveGameObject(string name)
        {
            var gameObject = new GameObject(name);
            gameObject.SetActive(false);
            m_Created.Add(gameObject);
            return gameObject;
        }

        // Registers an externally created object for teardown and returns it.
        public T Track<T>(T obj) where T : Object
        {
            m_Created.Add(obj);
            return obj;
        }

        public RenderTexture CreateRenderTexture(int width, int height, GraphicsFormat format, int mipCount = 1,
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

        public Texture2D CreateTexture2D(int width, int height, GraphicsFormat format, int mipCount = 1)
        {
            var texture = new Texture2D(width, height, format, mipCount, TextureCreationFlags.None);
            m_Created.Add(texture);
            return texture;
        }

        public Texture2DArray CreateTexture2DArray(int width, int height, int slices, GraphicsFormat format)
        {
            var texture = new Texture2DArray(width, height, slices, format, TextureCreationFlags.None, 1);
            m_Created.Add(texture);
            return texture;
        }

        // A Texture2D / Texture2DArray source copied into a RenderTexture needs both Basic and Texture-to-RT support.
        const CopyTextureSupport k_RequiredCopySupport = CopyTextureSupport.Basic | CopyTextureSupport.TextureToRT;

        bool CopyIsSupported => (SystemInfo.copyTextureSupport & k_RequiredCopySupport) == k_RequiredCopySupport;

        // Marks the calling test inconclusive when a Texture-to-RenderTexture Graphics.CopyTexture isn't supported.
        public void RequireCopySupport() =>
            Assume.That(CopyIsSupported,
                "Requires a graphics device that supports Texture-to-RenderTexture Graphics.CopyTexture "
                + "(CopyTextureSupport.Basic | CopyTextureSupport.TextureToRT).");

        // A RenderTexture-to-RenderTexture copy only needs Basic support.
        public void RequireBasicCopySupport() =>
            Assume.That((SystemInfo.copyTextureSupport & CopyTextureSupport.Basic) != 0,
                "Requires a graphics device that supports Graphics.CopyTexture (CopyTextureSupport.Basic).");
    }
}
#endif
