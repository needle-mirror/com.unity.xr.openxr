namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Represents the types of features that can be configured for a capability. Provided by `XR_EXT_spatial_entity`.
    /// </summary>
    public enum XrSpatialCapabilityFeatureEXT
    {
        /// <summary>
        /// Represents the fixed-sized marker feature provided by `XR_EXT_spatial_marker_tracking`.
        /// Equivalent to the OpenXR value `XR_SPATIAL_CAPABILITY_FEATURE_MARKER_TRACKING_FIXED_SIZE_MARKERS_EXT`.
        /// </summary>
        MarkerTrackingFixedSizeMarkers = 1000743000,

        /// <summary>
        /// Represents the static marker feature provided by `XR_EXT_spatial_marker_tracking`.
        /// Equivalent to the OpenXR value `XR_SPATIAL_CAPABILITY_FEATURE_MARKER_TRACKING_STATIC_MARKERS_EXT`.
        /// </summary>
        MarkerTrackingStaticMarkers = 1000743001,

        /// <summary>
        /// Represents the automatic-size images feature provided by `XR_EXT_spatial_image_tracking`.
        /// Equivalent to the OpenXR value `XR_SPATIAL_CAPABILITY_FEATURE_IMAGE_TRACKING_AUTOMATIC_SIZE_IMAGES_EXT`.
        /// </summary>
        ImageTrackingAutomaticSizeImages = 1000782000,

        /// <summary>
        /// Represents the static images feature provided by `XR_EXT_spatial_image_tracking`.
        /// Equivalent to the OpenXR value `XR_SPATIAL_CAPABILITY_FEATURE_IMAGE_TRACKING_STATIC_IMAGES_EXT`.
        /// </summary>
        ImageTrackingStaticImages = 1000782001,

        /// <summary>
        /// Represents the fixed-size images feature provided by `XR_EXT_spatial_image_tracking`.
        /// Equivalent to the OpenXR value `XR_SPATIAL_CAPABILITY_FEATURE_IMAGE_TRACKING_FIXED_SIZE_IMAGES_EXT`.
        /// </summary>
        ImageTrackingFixedSizeImages = 1000782002,
    }
}
