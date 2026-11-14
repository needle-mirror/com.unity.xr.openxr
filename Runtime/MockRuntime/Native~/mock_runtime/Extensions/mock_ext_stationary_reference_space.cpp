#include "mock_ext_stationary_reference_space.h"

std::unique_ptr<MockStationaryReferenceSpace> MockStationaryReferenceSpace::s_ext;

void MockStationaryReferenceSpace::Init(MockRuntime& runtime)
{
    s_ext.reset(new MockStationaryReferenceSpace(runtime));
}

MockStationaryReferenceSpace* MockStationaryReferenceSpace::Instance()
{
    return s_ext.get();
}

MockStationaryReferenceSpace::MockStationaryReferenceSpace(MockRuntime& runtime)
    : m_Runtime(runtime)
{
}

bool MockStationaryReferenceSpace::SetStationarySpaceGenerationId(const XrUuid& generationId)
{
    m_GenerationId = generationId;
    return true;
}

XrResult MockStationaryReferenceSpace::GetStationaryReferenceSpaceGenerationId(
    XrSession session,
    const XrStationaryReferenceSpaceGenerationIdGetInfoEXT* getInfo,
    XrStationaryReferenceSpaceGenerationIdResultEXT* result)
{
    if (session == XR_NULL_HANDLE || getInfo == nullptr || result == nullptr)
        return XR_ERROR_RUNTIME_FAILURE;

    *result = {
        XR_TYPE_STATIONARY_REFERENCE_SPACE_GENERATION_ID_RESULT_EXT,
        nullptr,
        m_GenerationId};

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR
xrGetStationaryReferenceSpaceGenerationIdEXT(
    XrSession session,
    const XrStationaryReferenceSpaceGenerationIdGetInfoEXT* getInfo,
    XrStationaryReferenceSpaceGenerationIdResultEXT* idResult)
{
    LOG_FUNC();
    CHECK_SESSION(session);
    MOCK_HOOK_BEFORE();

    const XrResult result = MockStationaryReferenceSpace::Instance()->GetStationaryReferenceSpaceGenerationId(session, getInfo, idResult);

    MOCK_HOOK_AFTER(result);

    return result;
}

XrResult MockStationaryReferenceSpace_GetInstanceProcAddr(const char* name, PFN_xrVoidFunction* function)
{
    GET_PROC_ADDRESS(xrGetStationaryReferenceSpaceGenerationIdEXT);
    return XR_ERROR_FUNCTION_UNSUPPORTED;
}
