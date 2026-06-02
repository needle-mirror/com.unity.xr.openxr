#if XR_COMPOSITION_LAYERS
namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Specifies which eye to display a composition layer to, as defined by the OpenXR specification.
    /// </summary>
    public static class XrEyeVisibility
    {
        /// <summary>
        /// The layer is visible to both eyes.
        /// </summary>
        public const uint Both = 0;

        /// <summary>
        /// The layer is visible to the left eye only.
        /// </summary>
        public const uint Left = 1;

        /// <summary>
        /// The layer is visible to the right eye only.
        /// </summary>
        public const uint Right = 2;
    }
}
#endif
