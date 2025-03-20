#include "openxr/openxr.h"

// This data was dumped from oculus quest 2.

uint32_t s_VisibilityMaskVerticesSizes[2][3] =
    {
        // first view
        {
            // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
            52,
            // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
            49,
            // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
            48,
        },
        // second view
        {
            // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
            52,
            // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
            49,
            // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
            48,
        }};

uint32_t s_VisibilityMaskIndicesSizes[2][3] =
    {
        // first view
        {
            // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
            156,
            // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
            144,
            // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
            48,
        },
        // second view
        {
            // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
            156,
            // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
            144,
            // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
            48,
        }};

// clang-format off
XrVector2f s_VisibilityMaskVertices[2][3][99] =
{
    // first view
    {
        // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
        { { 1.01f, 1.12253f }, { 1.01f, -1.20286f }, { -1.29274f, -1.20286f }, { -1.29274f, 1.12253f }, { 0.0f, 1.11061f }, { 0.156813f, 1.10997f }, { 0.308857f, 1.07153f }, { 0.460177f, 1.02982f }, { 0.607778f, 0.971561f }, { 0.742182f, 0.886089f }, { 0.848528f, 0.767387f }, { 0.922751f, 0.626911f }, { 0.974118f, 0.481266f }, { 1.0f, 0.335506f }, { 1.0f, 0.192824f }, { 1.0f, 0.0543102f }, { 1.0f, -0.081141f }, { 1.0f, -0.216879f }, { 1.0f, -0.356927f }, { 1.0f, -0.502332f }, { 0.986307f, -0.650586f }, { 0.931474f, -0.795886f }, { 0.848528f, -0.929669f }, { 0.731909f, -1.03498f }, { 0.590278f, -1.10353f }, { 0.439847f, -1.14302f }, { 0.29074f, -1.1662f }, { 0.1458f, -1.1886f }, { -1.19209e-07f, -1.19175f }, { -0.156797f, -1.19175f }, { -0.327742f, -1.19175f }, { -0.510643f, -1.19175f }, { -0.699907f, -1.19175f }, { -0.886918f, -1.19175f }, { -1.06066f, -1.1418f }, { -1.19912f, -1.00126f }, { -1.27994f, -0.82503f }, { -1.27994f, -0.63388f }, { -1.27994f, -0.442625f }, { -1.27994f, -0.258665f }, { -1.27994f, -0.081141f }, { -1.27994f, 0.0980905f }, { -1.27994f, 0.285711f }, { -1.27994f, 0.481404f }, { -1.27994f, 0.677933f }, { -1.22896f, 0.861876f }, { -1.09602f, 1.01487f }, { -0.926783f, 1.11061f }, { -0.739629f, 1.11061f }, { -0.545324f, 1.11061f }, { -0.353049f, 1.11061f }, { -0.169805f, 1.11061f } }, // 52
        // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
        { { 0.0f, -0.081141f }, { 0.0f, 1.11061f }, { 0.156813f, 1.10997f }, { 0.308857f, 1.07153f }, { 0.460177f, 1.02982f }, { 0.607778f, 0.971561f }, { 0.742182f, 0.886089f }, { 0.848528f, 0.767387f }, { 0.922751f, 0.626911f }, { 0.974118f, 0.481266f }, { 1.0f, 0.335506f }, { 1.0f, 0.192824f }, { 1.0f, 0.0543102f }, { 1.0f, -0.081141f }, { 1.0f, -0.216879f }, { 1.0f, -0.356927f }, { 1.0f, -0.502332f }, { 0.986307f, -0.650586f }, { 0.931474f, -0.795886f }, { 0.848528f, -0.929669f }, { 0.731909f, -1.03498f }, { 0.590278f, -1.10353f }, { 0.439847f, -1.14302f }, { 0.29074f, -1.1662f }, { 0.1458f, -1.1886f }, { -1.19209e-07f, -1.19175f }, { -0.156797f, -1.19175f }, { -0.327742f, -1.19175f }, { -0.510643f, -1.19175f }, { -0.699907f, -1.19175f }, { -0.886918f, -1.19175f }, { -1.06066f, -1.1418f }, { -1.19912f, -1.00126f }, { -1.27994f, -0.82503f }, { -1.27994f, -0.63388f }, { -1.27994f, -0.442625f }, { -1.27994f, -0.258665f }, { -1.27994f, -0.081141f }, { -1.27994f, 0.0980905f }, { -1.27994f, 0.285711f }, { -1.27994f, 0.481404f }, { -1.27994f, 0.677933f }, { -1.22896f, 0.861876f }, { -1.09602f, 1.01487f }, { -0.926783f, 1.11061f }, { -0.739629f, 1.11061f }, { -0.545324f, 1.11061f }, { -0.353049f, 1.11061f }, { -0.169805f, 1.11061f } }, // 49
        // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
        { { 0.0f, 1.11061f }, { 0.156813f, 1.10997f }, { 0.308857f, 1.07153f }, { 0.460177f, 1.02982f }, { 0.607778f, 0.971561f }, { 0.742182f, 0.886089f }, { 0.848528f, 0.767387f }, { 0.922751f, 0.626911f }, { 0.974118f, 0.481266f }, { 1.0f, 0.335506f }, { 1.0f, 0.192824f }, { 1.0f, 0.0543102f }, { 1.0f, -0.081141f }, { 1.0f, -0.216879f }, { 1.0f, -0.356927f }, { 1.0f, -0.502332f }, { 0.986307f, -0.650586f }, { 0.931474f, -0.795886f }, { 0.848528f, -0.929669f }, { 0.731909f, -1.03498f }, { 0.590278f, -1.10353f }, { 0.439847f, -1.14302f }, { 0.29074f, -1.1662f }, { 0.1458f, -1.1886f }, { -1.19209e-07f, -1.19175f }, { -0.156797f, -1.19175f }, { -0.327742f, -1.19175f }, { -0.510643f, -1.19175f }, { -0.699907f, -1.19175f }, { -0.886918f, -1.19175f }, { -1.06066f, -1.1418f }, { -1.19912f, -1.00126f }, { -1.27994f, -0.82503f }, { -1.27994f, -0.63388f }, { -1.27994f, -0.442625f }, { -1.27994f, -0.258665f }, { -1.27994f, -0.081141f }, { -1.27994f, 0.0980905f }, { -1.27994f, 0.285711f }, { -1.27994f, 0.481404f }, { -1.27994f, 0.677933f }, { -1.22896f, 0.861876f }, { -1.09602f, 1.01487f }, { -0.926783f, 1.11061f }, { -0.739629f, 1.11061f }, { -0.545324f, 1.11061f }, { -0.353049f, 1.11061f }, { -0.169805f, 1.11061f } }, // 48
    },
     
    // second view
    {
        // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
        { { -1.01f, 1.12253f }, { -1.01f, -1.20286f }, { 1.29274f, -1.20286f }, { 1.29274f, 1.12253f }, { 0.0f, 1.11061f }, { -0.156813f, 1.10997f }, { -0.308857f, 1.07153f }, { -0.460177f, 1.02982f }, { -0.607778f, 0.971561f }, { -0.742182f, 0.886089f }, { -0.848528f, 0.767387f }, { -0.922751f, 0.626911f }, { -0.974118f, 0.481266f }, { -1.0f, 0.335506f }, { -1.0f, 0.192824f }, { -1.0f, 0.0543102f }, { -1.0f, -0.081141f }, { -1.0f, -0.216879f }, { -1.0f, -0.356927f }, { -1.0f, -0.502332f }, { -0.986307f, -0.650586f }, { -0.931474f, -0.795886f }, { -0.848528f, -0.929669f }, { -0.731909f, -1.03498f }, { -0.590278f, -1.10353f }, { -0.439847f, -1.14302f }, { -0.29074f, -1.1662f }, { -0.1458f, -1.1886f }, { 0.0f, -1.19175f }, { 0.156797f, -1.19175f }, { 0.327742f, -1.19175f }, { 0.510643f, -1.19175f }, { 0.699907f, -1.19175f }, { 0.886918f, -1.19175f }, { 1.06066f, -1.1418f }, { 1.19912f, -1.00126f }, { 1.27994f, -0.82503f }, { 1.27994f, -0.63388f }, { 1.27994f, -0.442625f }, { 1.27994f, -0.258665f }, { 1.27994f, -0.081141f }, { 1.27994f, 0.0980905f }, { 1.27994f, 0.285711f }, { 1.27994f, 0.481404f }, { 1.27994f, 0.677933f }, { 1.22896f, 0.861876f }, { 1.09602f, 1.01487f }, { 0.926783f, 1.11061f }, { 0.739629f, 1.11061f }, { 0.545324f, 1.11061f }, { 0.353049f, 1.11061f }, { 0.169805f, 1.11061f } }, // 52
        // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
        { { 0.0f, -0.081141f }, { 0.0f, 1.11061f }, { -0.156813f, 1.10997f }, { -0.308857f, 1.07153f }, { -0.460177f, 1.02982f }, { -0.607778f, 0.971561f }, { -0.742182f, 0.886089f }, { -0.848528f, 0.767387f }, { -0.922751f, 0.626911f }, { -0.974118f, 0.481266f }, { -1.0f, 0.335506f }, { -1.0f, 0.192824f }, { -1.0f, 0.0543102f }, { -1.0f, -0.081141f }, { -1.0f, -0.216879f }, { -1.0f, -0.356927f }, { -1.0f, -0.502332f }, { -0.986307f, -0.650586f }, { -0.931474f, -0.795886f }, { -0.848528f, -0.929669f }, { -0.731909f, -1.03498f }, { -0.590278f, -1.10353f }, { -0.439847f, -1.14302f }, { -0.29074f, -1.1662f }, { -0.1458f, -1.1886f }, { 0.0f, -1.19175f }, { 0.156797f, -1.19175f }, { 0.327742f, -1.19175f }, { 0.510643f, -1.19175f }, { 0.699907f, -1.19175f }, { 0.886918f, -1.19175f }, { 1.06066f, -1.1418f }, { 1.19912f, -1.00126f }, { 1.27994f, -0.82503f }, { 1.27994f, -0.63388f }, { 1.27994f, -0.442625f }, { 1.27994f, -0.258665f }, { 1.27994f, -0.081141f }, { 1.27994f, 0.0980905f }, { 1.27994f, 0.285711f }, { 1.27994f, 0.481404f }, { 1.27994f, 0.677933f }, { 1.22896f, 0.861876f }, { 1.09602f, 1.01487f }, { 0.926783f, 1.11061f }, { 0.739629f, 1.11061f }, { 0.545324f, 1.11061f }, { 0.353049f, 1.11061f }, { 0.169805f, 1.11061f } }, // 49
        // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
        { { 0.00000000f, 1.11061263f }, { -0.156812787f, 1.10997009f }, { -0.308857441f, 1.07153058f }, { -0.460176885f, 1.02982426f }, { -0.607777834f, 0.971561193f }, { -0.742181718f, 0.886089087f }, { -0.848528206f, 0.767387271f }, { -0.922750771f, 0.626910567f }, { -0.974118292f, 0.481266260f }, { -1.00000000f, 0.335505605f }, { -1.00000000f, 0.192823887f }, { -1.00000000f, 0.0543102026f }, { -1.00000000f, -0.0811409950f }, { -1.00000000f, -0.216879249f }, { -1.00000000f, -0.356927097f }, { -1.00000000f, -0.502331913f }, { -0.986306727f, -0.650585532f }, { -0.931473970f, -0.795886219f }, { -0.848528206f, -0.929669261f }, { -0.731908858f, -1.03498316f }, { -0.590277910f, -1.10353208f }, { -0.439846814f, -1.14302492f }, { -0.290739954f, -1.16619778f }, { -0.145799756f, -1.18859875f }, { 0.00000000f, -1.19175363f }, { 0.156797290f, -1.19175363f }, { 0.327741623f, -1.19175363f }, { 0.510643244f, -1.19175363f }, { 0.699907303f, -1.19175363f }, { 0.886917710f, -1.19175363f }, { 1.06066012f, -1.14180124f }, { 1.19912076f, -1.00125873f }, { 1.27994156f, -0.825029850f }, { 1.27994156f, -0.633879662f }, { 1.27994156f, -0.442624867f }, { 1.27994156f, -0.258665323f }, { 1.27994156f, -0.0811409950f }, { 1.27994156f, 0.0980905294f }, { 1.27994156f, 0.285711050f }, { 1.27994156f, 0.481403828f }, { 1.27994156f, 0.677932978f }, { 1.22896290f, 0.861875772f }, { 1.09601569f, 1.01487422f }, { 0.926782608f, 1.11061263f }, { 0.739629149f, 1.11061263f }, { 0.545323849f, 1.11061263f }, { 0.353048563f, 1.11061263f }, { 0.169804931f, 1.11061263f } }, // 48
    }
};

uint32_t s_VisibilityMaskIndices[2][3][200] =
{
    // first view
    {
        // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
        { 4, 0, 3, 16, 1, 0, 28, 2, 1, 40, 3, 2, 0, 4, 5, 0, 5, 6, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 12, 13, 0, 13, 14, 0, 14, 15, 0, 15, 16, 1, 16, 17, 1, 17, 18, 1, 18, 19, 1, 19, 20, 1, 20, 21, 1, 21, 22, 1, 22, 23, 1, 23, 24, 1, 24, 25, 1, 25, 26, 1, 26, 27, 1, 27, 28, 2, 28, 29, 2, 29, 30, 2, 30, 31, 2, 31, 32, 2, 32, 33, 2, 33, 34, 2, 34, 35, 2, 35, 36, 2, 36, 37, 2, 37, 38, 2, 38, 39, 2, 39, 40, 3, 40, 41, 3, 41, 42, 3, 42, 43, 3, 43, 44, 3, 44, 45, 3, 45, 46, 3, 46, 47, 3, 47, 48, 3, 48, 49, 3, 49, 50, 3, 50, 51, 3, 51, 4 }, // 156
        // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
        { 1, 0, 2, 2, 0, 3, 3, 0, 4, 4, 0, 5, 5, 0, 6, 6, 0, 7, 7, 0, 8, 8, 0, 9, 9, 0, 10, 10, 0, 11, 11, 0, 12, 12, 0, 13, 13, 0, 14, 14, 0, 15, 15, 0, 16, 16, 0, 17, 17, 0, 18, 18, 0, 19, 19, 0, 20, 20, 0, 21, 21, 0, 22, 22, 0, 23, 23, 0, 24, 24, 0, 25, 25, 0, 26, 26, 0, 27, 27, 0, 28, 28, 0, 29, 29, 0, 30, 30, 0, 31, 31, 0, 32, 32, 0, 33, 33, 0, 34, 34, 0, 35, 35, 0, 36, 36, 0, 37, 37, 0, 38, 38, 0, 39, 39, 0, 40, 40, 0, 41, 41, 0, 42, 42, 0, 43, 43, 0, 44, 44, 0, 45, 45, 0, 46, 46, 0, 47, 47, 0, 48, 48, 0, 1 }, // 144
        // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
        { 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, // 48
    },
    // second view
    {
        // XR_VISIBILITY_MASK_TYPE_HIDDEN_TRIANGLE_MESH_KHR
        { 0, 4, 3, 1, 16, 0, 2, 28, 1, 3, 40, 2, 4, 0, 5, 5, 0, 6, 6, 0, 7, 7, 0, 8, 8, 0, 9, 9, 0, 10, 10, 0, 11, 11, 0, 12, 12, 0, 13, 13, 0, 14, 14, 0, 15, 15, 0, 16, 16, 1, 17, 17, 1, 18, 18, 1, 19, 19, 1, 20, 20, 1, 21, 21, 1, 22, 22, 1, 23, 23, 1, 24, 24, 1, 25, 25, 1, 26, 26, 1, 27, 27, 1, 28, 28, 2, 29, 29, 2, 30, 30, 2, 31, 31, 2, 32, 32, 2, 33, 33, 2, 34, 34, 2, 35, 35, 2, 36, 36, 2, 37, 37, 2, 38, 38, 2, 39, 39, 2, 40, 40, 3, 41, 41, 3, 42, 42, 3, 43, 43, 3, 44, 44, 3, 45, 45, 3, 46, 46, 3, 47, 47, 3, 48, 48, 3, 49, 49, 3, 50, 50, 3, 51, 51, 3, 4 }, // 156
        // XR_VISIBILITY_MASK_TYPE_VISIBLE_TRIANGLE_MESH_KHR
        { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 12, 13, 0, 13, 14, 0, 14, 15, 0, 15, 16, 0, 16, 17, 0, 17, 18, 0, 18, 19, 0, 19, 20, 0, 20, 21, 0, 21, 22, 0, 22, 23, 0, 23, 24, 0, 24, 25, 0, 25, 26, 0, 26, 27, 0, 27, 28, 0, 28, 29, 0, 29, 30, 0, 30, 31, 0, 31, 32, 0, 32, 33, 0, 33, 34, 0, 34, 35, 0, 35, 36, 0, 36, 37, 0, 37, 38, 0, 38, 39, 0, 39, 40, 0, 40, 41, 0, 41, 42, 0, 42, 43, 0, 43, 44, 0, 44, 45, 0, 45, 46, 0, 46, 47, 0, 47, 48, 0, 48, 1 }, // 144
        // XR_VISIBILITY_MASK_TYPE_LINE_LOOP_KHR
        { 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, // 48
    }
};
