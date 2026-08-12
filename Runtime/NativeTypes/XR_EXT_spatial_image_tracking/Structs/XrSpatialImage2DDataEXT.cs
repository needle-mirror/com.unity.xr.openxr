using UnityEngine.Assertions;
using XrSpatialImageTrackingDatabaseEXT = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Per-entity data returned for the Image 2D component when querying spatial component data.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// Use <see cref="imageTrackingDatabase"/> to identify which previously created database contains the
    /// reference image of this structure. For that database, use <see cref="referenceImageIndex"/> to identify the
    /// reference image by matching it to the input order of the `spatialReferenceImages` array.
    /// </remarks>
    public readonly struct XrSpatialImage2DDataEXT
    {
        /// <summary>
        /// The database that detected or tracked the image.
        /// </summary>
        public XrSpatialImageTrackingDatabaseEXT imageTrackingDatabase { get; }

        /// <summary>
        /// The index that maps into the `spatialReferenceImages` array of input reference images that
        /// <see cref="imageTrackingDatabase"/> was created with.
        /// </summary>
        public uint referenceImageIndex { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="imageTrackingDatabase">The database that detected or tracked the image. Must be a
        /// valid, non-null database handle.</param>
        /// <param name="referenceImageIndex">The index that maps into the `spatialReferenceImages` array of input
        /// reference images that <paramref name="imageTrackingDatabase"/> was created with.</param>
        public XrSpatialImage2DDataEXT(
            XrSpatialImageTrackingDatabaseEXT imageTrackingDatabase, uint referenceImageIndex)
        {
            Assert.IsTrue(imageTrackingDatabase != 0);

            this.imageTrackingDatabase = imageTrackingDatabase;
            this.referenceImageIndex = referenceImageIndex;
        }
    }
}
