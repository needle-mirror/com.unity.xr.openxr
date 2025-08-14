#if XR_COMPOSITION_LAYERS
namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Field of view.
    /// </summary>
    public struct XrFovf
    {
        /// <summary>
        /// The angle of the left side of the field of view. For a symmetric field of view this value is negative.
        /// </summary>
        public float AngleLeft;

        /// <summary>
        /// The angle of the right side of the field of view.
        /// </summary>
        public float AngleRight;

        /// <summary>
        /// The angle of the top part of the field of view.
        /// </summary>
        public float AngleUp;

        /// <summary>
        /// The angle of the bottom part of the field of view. For a symmetric field of view this value is negative.
        /// </summary>
        public float AngleDown;
    }
}
#endif
