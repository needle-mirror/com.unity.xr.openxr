using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Features.MockRuntime")]
[assembly: InternalsVisibleTo("Unity.XR.OpenXR.Features.ConformanceAutomation")]

namespace UnityEngine.XR.OpenXR.NativeTypes
{
    /// <summary>
    /// Convenience type for iterating (read only).
    /// </summary>
    internal unsafe struct XrBaseInStructure
    {
        /// <summary>
        /// The XrStructureType of this structure. This base structure itself has no associated XrStructureType value.
        /// </summary>
        public uint Type;

        /// <summary>
        /// Pointer to the next structure in a structure chain.
        /// </summary>
        public void* Next;
    }
}
