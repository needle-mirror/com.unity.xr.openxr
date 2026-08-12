using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using XrSpatialImageTrackingDatabaseEXT = System.UInt64;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Capability configuration struct for the image tracking capability.
    /// Provided by `XR_EXT_spatial_image_tracking`.
    /// </summary>
    /// <remarks>
    /// > [!WARNING]
    /// > Don't initialize this struct with the default parameterless constructor.
    /// > Use a constructor with parameters to ensure that <see cref="type"/> is correctly initialized
    /// > to <see cref="XrStructureType.SpatialCapabilityConfigurationImageTrackingEXT"/>.
    /// </remarks>
    public readonly unsafe struct XrSpatialCapabilityConfigurationImageTrackingEXT : ISpatialCapabilityConfiguration
    {
        /// <summary>
        /// The `XrStructureType` of this struct:
        /// <see cref="XrStructureType.SpatialCapabilityConfigurationImageTrackingEXT"/>.
        /// </summary>
        public XrStructureType type { get; }

        /// <summary>
        /// `null` or a pointer to the next structure in a structure chain.
        /// </summary>
        public void* next { get; }

        /// <summary>
        /// An <see cref="XrSpatialCapabilityEXT"/> and must be <see cref="XrSpatialCapabilityEXT.ImageTracking"/>.
        /// </summary>
        public XrSpatialCapabilityEXT capability { get; }

        /// <summary>
        /// The number of elements in <see cref="enabledComponents"/>. Must be greater than `0`.
        /// </summary>
        public uint enabledComponentCount { get; }

        /// <summary>
        /// Pointer to an array of `XrSpatialComponentTypeEXT` that specifies the components to enable for this capability. Must be non-null.
        /// </summary>
        public XrSpatialComponentTypeEXT* enabledComponents { get; }

        /// <summary>
        /// The number of elements in <see cref="imageTrackingDatabases"/>. Must be greater than `0`.
        /// </summary>
        public uint imageTrackingDatabaseCount { get; }

        /// <summary>
        /// Pointer to an array of `XrSpatialImageTrackingDatabaseEXT` handles that specifies the image tracking
        /// databases to enable for the spatial context. Must be non-null.
        /// </summary>
        public XrSpatialImageTrackingDatabaseEXT* imageTrackingDatabases { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="enabledComponentCount">The number of elements in <paramref name="enabledComponents"/>.
        /// Must be greater than `0`.</param>
        /// <param name="enabledComponents">Pointer to an array of `XrSpatialComponentTypeEXT` that specifies the components to enable for this capability.
        /// Must be non-null.</param>
        /// <param name="imageTrackingDatabaseCount">The number of elements in <paramref name="imageTrackingDatabases"/>.
        /// Must be greater than `0`.</param>
        /// <param name="imageTrackingDatabases">Pointer to an array of `XrSpatialImageTrackingDatabaseEXT` handles
        /// that specifies the image tracking databases to enable for the spatial context. Must be non-null.</param>
        public XrSpatialCapabilityConfigurationImageTrackingEXT(
            void* next,
            uint enabledComponentCount,
            XrSpatialComponentTypeEXT* enabledComponents,
            uint imageTrackingDatabaseCount,
            XrSpatialImageTrackingDatabaseEXT* imageTrackingDatabases)
        {
            Assert.IsTrue(enabledComponentCount > 0);
            Assert.IsTrue(enabledComponents != null);
            Assert.IsTrue(imageTrackingDatabaseCount > 0);
            Assert.IsTrue(imageTrackingDatabases != null);

            type = XrStructureType.SpatialCapabilityConfigurationImageTrackingEXT;
            this.next = next;
            capability = XrSpatialCapabilityEXT.ImageTracking;
            this.enabledComponentCount = enabledComponentCount;
            this.enabledComponents = enabledComponents;
            this.imageTrackingDatabaseCount = imageTrackingDatabaseCount;
            this.imageTrackingDatabases = imageTrackingDatabases;
        }

        /// <summary>
        /// Construct an instance with a `null` next pointer.
        /// </summary>
        /// <param name="enabledComponentCount">The number of elements in <paramref name="enabledComponents"/>.
        /// Must be greater than `0`.</param>
        /// <param name="enabledComponents">Pointer to an array of `XrSpatialComponentTypeEXT` that specifies the components to enable for this capability.
        /// Must be non-null.</param>
        /// <param name="imageTrackingDatabaseCount">The number of elements in <paramref name="imageTrackingDatabases"/>.
        /// Must be greater than `0`.</param>
        /// <param name="imageTrackingDatabases">Pointer to an array of `XrSpatialImageTrackingDatabaseEXT` handles
        /// that specifies the image tracking databases to enable for the spatial context. Must be non-null.</param>
        public XrSpatialCapabilityConfigurationImageTrackingEXT(
            uint enabledComponentCount,
            XrSpatialComponentTypeEXT* enabledComponents,
            uint imageTrackingDatabaseCount,
            XrSpatialImageTrackingDatabaseEXT* imageTrackingDatabases)
            : this(
                null, enabledComponentCount, enabledComponents,
                imageTrackingDatabaseCount, imageTrackingDatabases)
        { }

        /// <summary>
        /// Construct an instance from native arrays.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="enabledComponents">Native array of component types to enable for this capability.
        /// Must be non-empty.</param>
        /// <param name="imageTrackingDatabases">Native array of image tracking database handles.
        /// Must be non-empty.</param>
        public XrSpatialCapabilityConfigurationImageTrackingEXT(
            void* next,
            NativeArray<XrSpatialComponentTypeEXT> enabledComponents,
            NativeArray<XrSpatialImageTrackingDatabaseEXT> imageTrackingDatabases)
            : this(
                next,
                (uint)enabledComponents.Length,
                (XrSpatialComponentTypeEXT*)enabledComponents.GetUnsafePtr(),
                (uint)imageTrackingDatabases.Length,
                (XrSpatialImageTrackingDatabaseEXT*)imageTrackingDatabases.GetUnsafePtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from native arrays.
        /// </summary>
        /// <param name="enabledComponents">Native array of component types to enable for this capability.
        /// Must be non-empty.</param>
        /// <param name="imageTrackingDatabases">Native array of image tracking database handles.
        /// Must be non-empty.</param>
        public XrSpatialCapabilityConfigurationImageTrackingEXT(
            NativeArray<XrSpatialComponentTypeEXT> enabledComponents,
            NativeArray<XrSpatialImageTrackingDatabaseEXT> imageTrackingDatabases)
            : this(null, enabledComponents, imageTrackingDatabases) { }

        /// <summary>
        /// Construct an instance from read-only native arrays.
        /// </summary>
        /// <param name="next">`null` or a pointer to the next structure in a structure chain.</param>
        /// <param name="enabledComponents">Read-only native array of component types to enable for this capability.
        /// Must be non-empty.</param>
        /// <param name="imageTrackingDatabases">Read-only native array of image tracking database handles.
        /// Must be non-empty.</param>
        public XrSpatialCapabilityConfigurationImageTrackingEXT(
            void* next,
            NativeArray<XrSpatialComponentTypeEXT>.ReadOnly enabledComponents,
            NativeArray<XrSpatialImageTrackingDatabaseEXT>.ReadOnly imageTrackingDatabases)
            : this(
                next,
                (uint)enabledComponents.Length,
                (XrSpatialComponentTypeEXT*)enabledComponents.GetUnsafeReadOnlyPtr(),
                (uint)imageTrackingDatabases.Length,
                (XrSpatialImageTrackingDatabaseEXT*)imageTrackingDatabases.GetUnsafeReadOnlyPtr())
        { }

        /// <summary>
        /// Construct an instance with a `null` next pointer from read-only native arrays.
        /// </summary>
        /// <param name="enabledComponents">Read-only native array of component types to enable for this capability.
        /// Must be non-empty.</param>
        /// <param name="imageTrackingDatabases">Read-only native array of image tracking database handles.
        /// Must be non-empty.</param>
        public XrSpatialCapabilityConfigurationImageTrackingEXT(
            NativeArray<XrSpatialComponentTypeEXT>.ReadOnly enabledComponents,
            NativeArray<XrSpatialImageTrackingDatabaseEXT>.ReadOnly imageTrackingDatabases)
            : this(null, enabledComponents, imageTrackingDatabases) { }

        /// <summary>
        /// Get a string suitable for debugging purposes.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("{");
            builder.Append("  ").Append(type.ToString()).AppendLine();
            builder.Append("  ").Append(((IntPtr)next).ToString("X")).AppendLine();
            builder.Append("  ").Append(capability.ToString()).AppendLine();
            builder.AppendLine("  [");
            for (var i = 0; i < enabledComponentCount; i++)
            {
                builder.Append("    ").Append(enabledComponents[i].ToString());
                if (i < enabledComponentCount - 1)
                {
                    builder.AppendLine(",");
                }
                else
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine("  ],");
            builder.AppendLine("  [");
            for (var i = 0; i < imageTrackingDatabaseCount; i++)
            {
                builder.Append("    ").Append(imageTrackingDatabases[i].ToString());
                if (i < imageTrackingDatabaseCount - 1)
                {
                    builder.AppendLine(",");
                }
                else
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine("  ]");
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
