#include "mock_android_enumerate_system_extension_properties.h"
#include "openxr/androidxr/xr_android_enumerate_system_extension_properties.h"
#include <cassert>
#include <cstddef>
#include <vector>

#define CHECK_EXT_INIT()                                                      \
    if (nullptr == MockAndroidEnumerateSystemExtensionProperties::Instance()) \
        return XR_ERROR_FUNCTION_UNSUPPORTED;

std::unique_ptr<MockAndroidEnumerateSystemExtensionProperties> MockAndroidEnumerateSystemExtensionProperties::s_Instance;
std::unordered_map<std::string, bool> MockAndroidEnumerateSystemExtensionProperties::s_SystemEnabledExtensions{};

MockAndroidEnumerateSystemExtensionProperties* MockAndroidEnumerateSystemExtensionProperties::Instance()
{
    return s_Instance.get();
}

void MockAndroidEnumerateSystemExtensionProperties::Init(MockRuntime& runtime)
{
    s_Instance.reset(new MockAndroidEnumerateSystemExtensionProperties(runtime));
}

void MockAndroidEnumerateSystemExtensionProperties::Deinit()
{
    s_Instance.reset();
}

MockAndroidEnumerateSystemExtensionProperties::MockAndroidEnumerateSystemExtensionProperties(MockRuntime& runtime)
    : m_Runtime{runtime}
{
}

void MockAndroidEnumerateSystemExtensionProperties::SetSystemExtensionEnabled(const char* extName, bool enabled)
{
    s_SystemEnabledExtensions[extName] = enabled;
}

XrResult MockAndroidEnumerateSystemExtensionProperties::EnumerateSystemExtensionProperties(
    XrInstance instance,
    XrSystemId systemId,
    uint32_t propertyCapacityInput,
    uint32_t* propertyCountOutput,
    XrSystemExtensionPropertiesANDROID* properties)
{
    *propertyCountOutput = static_cast<uint32_t>(s_SystemEnabledExtensions.size());

    if (propertyCapacityInput == 0)
    {
        return XR_SUCCESS;
    }

    if (propertyCapacityInput < s_SystemEnabledExtensions.size())
    {
        return XR_ERROR_SIZE_INSUFFICIENT;
    }

    if (!properties)
    {
        return XR_ERROR_VALIDATION_FAILURE;
    }
    for (size_t i = 0; i < propertyCapacityInput; i++)
    {
        if (properties[i].type != XR_TYPE_SYSTEM_EXTENSION_PROPERTIES_ANDROID)
            return XR_ERROR_VALIDATION_FAILURE;
        if (properties[i].properties.type != XR_TYPE_EXTENSION_PROPERTIES)
            return XR_ERROR_VALIDATION_FAILURE;
    }

    auto iter = s_SystemEnabledExtensions.cbegin();
    for (uint32_t i = 0; i < *propertyCountOutput; i++)
    {
        XrSystemExtensionPropertiesANDROID prop{};
        prop.type = XR_TYPE_SYSTEM_EXTENSION_PROPERTIES_ANDROID;
        prop.properties.type = XR_TYPE_EXTENSION_PROPERTIES;
        strncpy(prop.properties.extensionName, iter->first.c_str(), XR_MAX_EXTENSION_NAME_SIZE - 1);
        prop.properties.extensionName[XR_MAX_EXTENSION_NAME_SIZE - 1] = '\0';
        prop.properties.extensionVersion = 0; // we don't care about version for now
        prop.isSupported = iter->second ? XR_TRUE : XR_FALSE;

        properties[i] = prop;
        iter++;
    }

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR
xrEnumerateSystemExtensionPropertiesANDROID(
    XrInstance instance,
    XrSystemId systemId,
    uint32_t propertyCapacityInput,
    uint32_t* propertyCountOutput,
    XrSystemExtensionPropertiesANDROID* properties)
{
    LOG_FUNC();
    CHECK_INSTANCE(instance);
    CHECK_EXT_INIT();
    MOCK_HOOK_BEFORE();

    XrResult result = MockAndroidEnumerateSystemExtensionProperties::Instance()
                          ->EnumerateSystemExtensionProperties(
                              instance,
                              systemId,
                              propertyCapacityInput,
                              propertyCountOutput,
                              properties);

    MOCK_HOOK_AFTER(result);

    return result;
}

XrResult MockAndroidEnumerateSystemExtensionProperties_GetInstanceProcAddr(const char* name, PFN_xrVoidFunction* function)
{
    GET_PROC_ADDRESS(xrEnumerateSystemExtensionPropertiesANDROID)
    return XR_ERROR_FEATURE_UNSUPPORTED;
}

extern "C" void UNITY_INTERFACE_EXPORT
MockAndroidEnumerateSystemExtensionProperties_SetSystemExtensionEnabled(const char* extName, bool enabled)
{
    MockAndroidEnumerateSystemExtensionProperties::SetSystemExtensionEnabled(extName, enabled);
}

#undef CHECK_EXT_INIT
