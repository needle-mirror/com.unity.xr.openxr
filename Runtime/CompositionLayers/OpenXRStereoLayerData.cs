#if XR_COMPOSITION_LAYERS
using System.Collections.Generic;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Services;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    /// Shared constants and utilities for per-eye stereo rendering on non-projection
    /// (Quad, Cylinder, Cube, Equirect) composition layers.
    internal static class OpenXRStereoLayerData
    {
        /// Tracks the right-eye native layer data for stereo rendering.
        /// Each layer handler should store one of these per active stereo layer.
        public class RightEyeData<T> where T : struct
        {
            public T RightNativeLayer;

            public Texture LeftTexture;

            public Texture RightTexture;

            public bool IsActive;
        }

        /// Checks whether the given <see cref="TexturesExtension"/> is configured for per-eye stereo rendering.
        public static bool IsStereoRequested(TexturesExtension texturesExtension)
        {
            return texturesExtension != null
                && texturesExtension.TargetEye == TexturesExtension.TargetEyeEnum.Individual
                && texturesExtension.LeftTexture != null
                && texturesExtension.RightTexture != null;
        }

        /// Determines the <see cref="XrEyeVisibility"/> value for a layer, accounting for stereo and single-eye modes.
        public static uint GetEyeVisibility(TexturesExtension texturesExtension, bool isStereo)
        {
            if (isStereo)
                return XrEyeVisibility.Left;

            if (texturesExtension != null)
            {
#if XR_COMPOSITION_LAYERS_2_5_OR_GREATER
                if (texturesExtension.TargetEye == TexturesExtension.TargetEyeEnum.Left)
                    return XrEyeVisibility.Left;
                if (texturesExtension.TargetEye == TexturesExtension.TargetEyeEnum.Right)
                    return XrEyeVisibility.Right;
#endif
            }
            return XrEyeVisibility.Both;
        }


        /// Generates a unique synthetic key for the right-eye layer based on the original layer ID.
        /// Uses negative values to avoid collision with real layer IDs.
        public static int GetRightEyeKey(int layerId) => -layerId - 1;


        /// Gets the sub-image dimensions from a <see cref="TexturesExtension"/>, handling both
        /// local textures and Android external surfaces.
        public static void GetSubImageDimensions(TexturesExtension texturesExtension, out int width, out int height)
        {
            width = 0;
            height = 0;

            switch (texturesExtension.sourceTexture)
            {
                case TexturesExtension.SourceTextureEnum.LocalTexture:
                {
                    if (texturesExtension.LeftTexture != null)
                    {
                        width = texturesExtension.LeftTexture.width;
                        height = texturesExtension.LeftTexture.height;
                    }
                    break;
                }

                case TexturesExtension.SourceTextureEnum.AndroidSurface:
                {
                    width = (int)texturesExtension.Resolution.x;
                    height = (int)texturesExtension.Resolution.y;
                    break;
                }
            }
        }

        /// Writes stereo textures to their swapchain render targets for all active entries in the
        /// given stereo data dictionary, then marks each entry as inactive.
        public static void WriteStereoTextures<T>(Dictionary<int, RightEyeData<T>> stereoData) where T : struct
        {
            foreach (var kvp in stereoData)
            {
                var data = kvp.Value;
                if (!data.IsActive)
                    continue;

                if (OpenXRLayerUtility.CreateOrGetStereoRenderTextureIds(kvp.Key, out uint leftId, out uint rightId))
                {
                    if (leftId != 0 && data.LeftTexture != null)
                    {
                        var leftRT = OpenXRLayerUtility.FindRenderTexture(leftId);
                        if (leftRT != null)
                            OpenXRLayerUtility.WriteToRenderTexture(data.LeftTexture, leftRT);
                    }

                    if (rightId != 0 && data.RightTexture != null)
                    {
                        var rightRT = OpenXRLayerUtility.FindRenderTexture(rightId);
                        if (rightRT != null)
                            OpenXRLayerUtility.WriteToRenderTexture(data.RightTexture, rightRT);
                    }
                }

                data.IsActive = false;
            }
        }

        /// Converts a normalized source rect and texture size into an <see cref="XrRect2Di"/>
        public static XrRect2Di ToSubImageRect(Rect sourceRect, int textureWidth, int textureHeight)
        {
            return new XrRect2Di()
            {
                Offset = new XrOffset2Di()
                {
                    X = (int)(textureWidth * sourceRect.x),
                    Y = (int)(textureHeight * sourceRect.y)
                },
                Extent = new XrExtent2Di()
                {
                    Width = (int)(textureWidth * sourceRect.width),
                    Height = (int)(textureHeight * sourceRect.height)
                }
            };
        }
    }
}
#endif
