namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Identifies a reference image format supported by the runtime for image tracking.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// A runtime that supports `XR_EXT_spatial_image_tracking` is required to enumerate at least
    /// <see cref="RGBA_8888"/> and <see cref="RGB_888"/>.
    /// </remarks>
    public enum XrSpatialReferenceImageFormatEXT
    {
        /// <summary>
        /// Four-channel 8-bit-per-channel red/green/blue/alpha. Always supported.
        /// Equivalent to the OpenXR value `XR_SPATIAL_REFERENCE_IMAGE_FORMAT_RGBA_8888_EXT`.
        /// </summary>
        RGBA_8888 = 1,

        /// <summary>
        /// Three-channel 8-bit-per-channel red/green/blue. Always supported.
        /// Equivalent to the OpenXR value `XR_SPATIAL_REFERENCE_IMAGE_FORMAT_RGB_888_EXT`.
        /// </summary>
        RGB_888 = 2,

        /// <summary>
        /// Planar YUV 4:2:0 with 8 bits per channel. May not be supported.
        /// Equivalent to the OpenXR value `XR_SPATIAL_REFERENCE_IMAGE_FORMAT_YUV_420_888_EXT`.
        /// </summary>
        YUV_420_888 = 3,
    }
}
