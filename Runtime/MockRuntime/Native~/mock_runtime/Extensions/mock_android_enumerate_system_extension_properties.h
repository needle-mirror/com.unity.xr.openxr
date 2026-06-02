#pragma once

#include "../mock.h"
#include <memory>
#include <string>
#include <unordered_map>

struct XrSystemExtensionPropertiesANDROID;

class MockAndroidEnumerateSystemExtensionProperties
{
public:
    static MockAndroidEnumerateSystemExtensionProperties* Instance();
    static void Init(MockRuntime& runtime);
    static void Deinit();

    MockAndroidEnumerateSystemExtensionProperties(MockRuntime& runtime);

    XrResult EnumerateSystemExtensionProperties(
        XrInstance instance,
        XrSystemId systemId,
        uint32_t propertyCapacityInput,
        uint32_t* propertyCountOutput,
        XrSystemExtensionPropertiesANDROID* properties);

    static void SetSystemExtensionEnabled(const char* extName, bool enabled);

private:
    static std::unique_ptr<MockAndroidEnumerateSystemExtensionProperties> s_Instance;

    MockRuntime& m_Runtime;

    static std::unordered_map<std::string, bool> s_SystemEnabledExtensions;
};

XrResult MockAndroidEnumerateSystemExtensionProperties_GetInstanceProcAddr(const char* name, PFN_xrVoidFunction* function);
