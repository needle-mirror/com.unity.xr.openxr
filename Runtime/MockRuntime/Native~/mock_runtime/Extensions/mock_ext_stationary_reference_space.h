#pragma once

#include "../mock.h"
#include <memory>

class MockRuntime;

class MockStationaryReferenceSpace
{
public:
    static void Init(MockRuntime& runtime);
    static MockStationaryReferenceSpace* Instance();

    XrResult GetStationaryReferenceSpaceGenerationId(XrSession session,
        const XrStationaryReferenceSpaceGenerationIdGetInfoEXT* getInfo,
        XrStationaryReferenceSpaceGenerationIdResultEXT* result);
    bool SetStationarySpaceGenerationId(const XrUuid& generationId);

private:
    MockStationaryReferenceSpace(MockRuntime& runtime);
    XrUuid m_GenerationId{};
    static std::unique_ptr<MockStationaryReferenceSpace> s_ext;
    MockRuntime& m_Runtime;
};

XrResult MockStationaryReferenceSpace_GetInstanceProcAddr(const char* name, PFN_xrVoidFunction* function);
