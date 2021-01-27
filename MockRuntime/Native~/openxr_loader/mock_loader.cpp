#include "IUnityInterface.h"
#include "XR/IUnityXRTrace.h"
#include <cstring>
#include <queue>
#include <string>
#include <vector>

#define DEBUG_LOG_EVERY_FUNC_CALL 1

#include "mock.h"
#include "mock_driver_extension.h"
#include "mock_state.h"
#include "openxr_mock_driver.h"

XR_DEFINE_HANDLE(MockHandle)
static uint64_t s_HandleCounter = 0;
static uint64_t GetNextHandleId()
{
    return ++s_HandleCounter;
}

static std::vector<std::string> s_PathStrings;

#define XR_UNITY_mock_test_SPEC_VERSION 123
#define XR_UNITY_MOCK_TEST_EXTENSION_NAME "XR_UNITY_mock_test"

#define XR_UNITY_null_gfx_SPEC_VERSION 1
#define XR_UNITY_NULL_GFX_EXTENSION_NAME "XR_UNITY_null_gfx"

#define CHECK_EXPECTED_RESULT(...)                                                                          \
    std::vector<XrResult> expectedResults{__VA_ARGS__};                                                     \
    XrResult expectedResult = GetExpectedResultForFunction(__FUNCTION__);                                   \
    if (std::find(expectedResults.begin(), expectedResults.end(), expectedResult) == expectedResults.end()) \
        return expectedResult;

// clang-format off
static XrExtensionProperties s_Extensions[] = {
    {
        XR_TYPE_EXTENSION_PROPERTIES,
        nullptr,
        XR_UNITY_MOCK_TEST_EXTENSION_NAME,
        XR_UNITY_mock_test_SPEC_VERSION
    },
    {
        XR_TYPE_EXTENSION_PROPERTIES,
        nullptr,
        XR_UNITY_MOCK_DRIVER_EXTENSION_NAME,
        XR_UNITY_mock_driver_SPEC_VERSION
    },
    {
        XR_TYPE_EXTENSION_PROPERTIES,
        nullptr,
        XR_UNITY_NULL_GFX_EXTENSION_NAME,
        XR_UNITY_null_gfx_SPEC_VERSION
    },
    {
        XR_TYPE_EXTENSION_PROPERTIES,
        nullptr,
        XR_KHR_VISIBILITY_MASK_EXTENSION_NAME,
        XR_KHR_visibility_mask_SPEC_VERSION
    }
};
// clang-format on

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateApiLayerProperties(uint32_t propertyCapacityInput, uint32_t* propertyCountOutput, XrApiLayerProperties* properties)
{
    LOG_FUNC();
    *propertyCountOutput = 0;
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateInstanceExtensionProperties(const char* layerName, uint32_t propertyCapacityInput, uint32_t* propertyCountOutput, XrExtensionProperties* properties)
{
    LOG_FUNC();
    if (properties == nullptr)
    {
        *propertyCountOutput = sizeof(s_Extensions) / sizeof(XrExtensionProperties);
    }
    else
    {
        uint32_t count = 0;
        while (count < *propertyCountOutput)
        {
            properties[count] = s_Extensions[count];
            ++count;
        }
    }
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrCreateInstance(const XrInstanceCreateInfo* createInfo, XrInstance* instance)
{
    LOG_FUNC();

    *instance = (XrInstance)1;
    bool supportsDriverExtension = false;
    bool wantsNullGfx = false;
    for (uint32_t i = 0; i < createInfo->enabledExtensionCount; ++i)
    {
        if (strncmp(XR_UNITY_MOCK_TEST_EXTENSION_NAME, createInfo->enabledExtensionNames[i], sizeof(XR_UNITY_MOCK_TEST_EXTENSION_NAME)) == 0)
        {
            *instance = (XrInstance)10;
        }

        if (strncmp(XR_UNITY_MOCK_DRIVER_EXTENSION_NAME, createInfo->enabledExtensionNames[i], sizeof(XR_UNITY_MOCK_TEST_EXTENSION_NAME)) == 0)
        {
            supportsDriverExtension = true;
        }

        if (strncmp(XR_UNITY_NULL_GFX_EXTENSION_NAME, createInfo->enabledExtensionNames[i], sizeof(XR_UNITY_MOCK_TEST_EXTENSION_NAME)) == 0)
        {
            wantsNullGfx = true;
        }
    }

    AddInstance(*instance);
    SetSupportingDriverExtension(*instance, supportsDriverExtension);
    SetNullGfx(wantsNullGfx);

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrDestroyInstance(XrInstance instance)
{
    LOG_FUNC();
    RemoveInstance(instance);
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetInstanceProperties(XrInstance instance, XrInstanceProperties* instanceProperties)
{
    LOG_FUNC();

    instanceProperties->runtimeVersion = XR_MAKE_VERSION(0, 0, 1);
    strncpy(instanceProperties->runtimeName, "Unity Mock Runtime", XR_MAX_RUNTIME_NAME_SIZE);

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrPollEvent(XrInstance instance, XrEventDataBuffer* eventData)
{
    LOG_FUNC();

    return GetNextEvent(instance, eventData);
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrResultToString(XrInstance instance, XrResult value, char buffer[XR_MAX_RESULT_STRING_SIZE])
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrStructureTypeToString(XrInstance instance, XrStructureType value, char buffer[XR_MAX_STRUCTURE_NAME_SIZE])
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetSystem(XrInstance instance, const XrSystemGetInfo* getInfo, XrSystemId* systemId)
{
    LOG_FUNC();
    *systemId = (XrSystemId)2;
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetSystemProperties(XrInstance instance, XrSystemId systemId, XrSystemProperties* properties)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateEnvironmentBlendModes(XrInstance instance, XrSystemId systemId, XrViewConfigurationType viewConfigurationType, uint32_t environmentBlendModeCapacityInput, uint32_t* environmentBlendModeCountOutput, XrEnvironmentBlendMode* environmentBlendModes)
{
    LOG_FUNC();

    if (viewConfigurationType != XR_VIEW_CONFIGURATION_TYPE_PRIMARY_STEREO)
    {
        return XR_ERROR_VIEW_CONFIGURATION_TYPE_UNSUPPORTED;
    }

    *environmentBlendModeCountOutput = 1;

    if (environmentBlendModeCapacityInput == 0)
        return XR_SUCCESS;
    if (environmentBlendModeCapacityInput < *environmentBlendModeCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    environmentBlendModes[0] = GetMockBlendMode();

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrCreateSession(XrInstance instance, const XrSessionCreateInfo* createInfo, XrSession* session)
{
    LOG_FUNC();
    CHECK_EXPECTED_RESULT(XR_SUCCESS);

    *session = (XrSession)3;
    SetMockSession(instance, session);

    ChangeSessionState(*session, XR_SESSION_STATE_IDLE);

    // TODO: STATEMANAGEMENT: If users presence is enabled then we need to do this transition at the point
    // where the user is actually known to exist (i.e. presence sensor is triggered.)
    ChangeSessionStateFrom(*session, XR_SESSION_STATE_IDLE, XR_SESSION_STATE_READY);

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrDestroySession(XrSession session)
{
    LOG_FUNC();
    CHECK_EXPECTED_RESULT(XR_SUCCESS);

    SetSessionRunning(session, false);
    SetMockSession(GetInstanceFromSession(session), XR_NULL_HANDLE);

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateReferenceSpaces(XrSession session, uint32_t spaceCapacityInput, uint32_t* spaceCountOutput, XrReferenceSpaceType* spaces)
{
    LOG_FUNC();

    if (!spaceCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    *spaceCountOutput = 4;

    if (spaceCapacityInput == 0)
        return XR_SUCCESS;

    if (spaceCapacityInput < *spaceCountOutput || !spaces)
        return XR_ERROR_VALIDATION_FAILURE;

    spaces[0] = XR_REFERENCE_SPACE_TYPE_VIEW;
    spaces[1] = XR_REFERENCE_SPACE_TYPE_LOCAL;
    spaces[2] = XR_REFERENCE_SPACE_TYPE_STAGE;
    spaces[3] = XR_REFERENCE_SPACE_TYPE_UNBOUNDED_MSFT;

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrCreateReferenceSpace(XrSession session, const XrReferenceSpaceCreateInfo* createInfo, XrSpace* space)
{
    LOG_FUNC();

    switch (createInfo->referenceSpaceType)
    {
    case XR_REFERENCE_SPACE_TYPE_LOCAL:
        *space = (XrSpace)4;
        break;
    case XR_REFERENCE_SPACE_TYPE_STAGE:
        *space = (XrSpace)5;
        break;
    case XR_REFERENCE_SPACE_TYPE_VIEW:
        *space = (XrSpace)6;
        break;
    default:
        break;
    }

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetReferenceSpaceBoundsRect(XrSession session, XrReferenceSpaceType referenceSpaceType, XrExtent2Df* bounds)
{
    LOG_FUNC();

    return (GetExtentsForReferenceSpace(session, referenceSpaceType, bounds)) ? XR_SUCCESS : XR_SPACE_BOUNDS_UNAVAILABLE;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrCreateActionSpace(XrSession session, const XrActionSpaceCreateInfo* createInfo, XrSpace* space)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrLocateSpace(XrSpace space, XrSpace baseSpace, XrTime time, XrSpaceLocation* location)
{
    LOG_FUNC();
    CHECK_EXPECTED_RESULT(XR_SUCCESS);
    return LocateSpace(space, baseSpace, time, location);
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrDestroySpace(XrSpace space)
{
    LOG_FUNC();
    CHECK_EXPECTED_RESULT(XR_SUCCESS);
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateViewConfigurations(XrInstance instance, XrSystemId systemId, uint32_t viewConfigurationTypeCapacityInput, uint32_t* viewConfigurationTypeCountOutput, XrViewConfigurationType* viewConfigurationTypes)
{
    LOG_FUNC();

    *viewConfigurationTypeCountOutput = 1;

    if (viewConfigurationTypeCapacityInput == 0)
        return XR_SUCCESS;
    if (viewConfigurationTypeCapacityInput < *viewConfigurationTypeCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    viewConfigurationTypes[0] = XR_VIEW_CONFIGURATION_TYPE_PRIMARY_STEREO;

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetViewConfigurationProperties(XrInstance instance, XrSystemId systemId, XrViewConfigurationType viewConfigurationType, XrViewConfigurationProperties* configurationProperties)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateViewConfigurationViews(XrInstance instance, XrSystemId systemId, XrViewConfigurationType viewConfigurationType, uint32_t viewCapacityInput, uint32_t* viewCountOutput, XrViewConfigurationView* views)
{
    LOG_FUNC();

    *viewCountOutput = 2;

    if (viewCapacityInput == 0)
        return XR_SUCCESS;
    if (viewCapacityInput < *viewCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    // Matches unity's mock hmd which is modeled after vive.
    views[0].recommendedImageRectWidth = 1512;
    views[0].maxImageRectWidth = 1512 * 2;
    views[0].recommendedImageRectHeight = 1680;
    views[0].maxImageRectHeight = 1680 * 2;
    views[0].recommendedSwapchainSampleCount = 1;
    views[0].maxSwapchainSampleCount = 1;

    views[1] = views[0];

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateSwapchainFormats(XrSession session, uint32_t formatCapacityInput, uint32_t* formatCountOutput, int64_t* formats)
{
    LOG_FUNC();

    *formatCountOutput = 1;

    if (formatCapacityInput == 0)
        return XR_SUCCESS;
    if (formatCapacityInput < *formatCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    formats[0] = 0;

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrCreateSwapchain(XrSession session, const XrSwapchainCreateInfo* createInfo, XrSwapchain* swapchain)
{
    LOG_FUNC();

    static uint64_t uniqueSwapchainHandle = 0;

    *swapchain = (XrSwapchain)++uniqueSwapchainHandle;

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrDestroySwapchain(XrSwapchain swapchain)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateSwapchainImages(XrSwapchain swapchain, uint32_t imageCapacityInput, uint32_t* imageCountOutput, XrSwapchainImageBaseHeader* images)
{
    LOG_FUNC();

    *imageCountOutput = 1;

    if (imageCapacityInput == 0)
        return XR_SUCCESS;
    if (imageCapacityInput < *imageCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrAcquireSwapchainImage(XrSwapchain swapchain, const XrSwapchainImageAcquireInfo* acquireInfo, uint32_t* index)
{
    LOG_FUNC();

    *index = 0;

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrWaitSwapchainImage(XrSwapchain swapchain, const XrSwapchainImageWaitInfo* waitInfo)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrReleaseSwapchainImage(XrSwapchain swapchain, const XrSwapchainImageReleaseInfo* releaseInfo)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrBeginSession(XrSession session, const XrSessionBeginInfo* beginInfo)
{
    LOG_FUNC();
    CHECK_EXPECTED_RESULT(XR_SUCCESS);

    SetSessionRunning(session, true);

    ChangeSessionStateFrom(session, XR_SESSION_STATE_READY, XR_SESSION_STATE_SYNCHRONIZED);
    ChangeSessionStateFrom(session, XR_SESSION_STATE_SYNCHRONIZED, XR_SESSION_STATE_VISIBLE);
    ChangeSessionStateFrom(session, XR_SESSION_STATE_VISIBLE, XR_SESSION_STATE_FOCUSED);

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEndSession(XrSession session)
{
    LOG_FUNC();
    CHECK_EXPECTED_RESULT(XR_SUCCESS, XR_ERROR_SESSION_NOT_STOPPING);

    SetSessionRunning(session, false);
    ChangeSessionStateFrom(session, XR_SESSION_STATE_STOPPING, XR_SESSION_STATE_IDLE);

    if (HasExitBeenRequested())
    {
        SetExitRequestState(false);
        ChangeSessionStateFrom(session, XR_SESSION_STATE_IDLE, XR_SESSION_STATE_EXITING);
    }

    return expectedResult;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrRequestExitSession(XrSession session)
{
    LOG_FUNC();
    CHECK_EXPECTED_RESULT(XR_SUCCESS);

    ChangeSessionStateFrom(session, XR_SESSION_STATE_FOCUSED, XR_SESSION_STATE_VISIBLE);
    ChangeSessionStateFrom(session, XR_SESSION_STATE_VISIBLE, XR_SESSION_STATE_SYNCHRONIZED);
    ChangeSessionStateFrom(session, XR_SESSION_STATE_SYNCHRONIZED, XR_SESSION_STATE_STOPPING);

    SetExitRequestState(true);
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrWaitFrame(XrSession session, const XrFrameWaitInfo* frameWaitInfo, XrFrameState* frameState)
{
    LOG_FUNC();
    frameState->predictedDisplayPeriod = 16666000;
    frameState->shouldRender = IsNullGfx();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrBeginFrame(XrSession session, const XrFrameBeginInfo* frameBeginInfo)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEndFrame(XrSession session, const XrFrameEndInfo* frameEndInfo)
{
    LOG_FUNC();
    return EndFrame(session, frameEndInfo);
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrLocateViews(XrSession session, const XrViewLocateInfo* viewLocateInfo, XrViewState* viewState, uint32_t viewCapacityInput, uint32_t* viewCountOutput, XrView* views)
{
    LOG_FUNC();
    return LocateViews(session, viewLocateInfo, viewState, viewCapacityInput, viewCountOutput, views);
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrStringToPath(XrInstance instance, const char* pathString, XrPath* path)
{
    LOG_FUNC();

    if (strlen(pathString) >= XR_MAX_PATH_LENGTH)
        return XR_ERROR_PATH_COUNT_EXCEEDED;

    for (int i = 0llu; i < s_PathStrings.size(); i++)
    {
        if (strncmp(pathString, s_PathStrings[i].c_str(), XR_MAX_PATH_LENGTH) == 0)
        {
            // Path is incremented by 1 because the invalid path Id is 0.
            *path = (XrPath)(i + 1);
            return XR_SUCCESS;
        }
    }

    s_PathStrings.push_back(std::string(pathString));
    *path = (XrPath)s_PathStrings.size();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrPathToString(XrInstance instance, XrPath path, uint32_t bufferCapacityInput, uint32_t* bufferCountOutput, char* buffer)
{
    LOG_FUNC();

    // Path is 1 greater than the index because 0 is reserved as an invalid path.
    size_t pathIdx = ((size_t)path) - 1;
    if (pathIdx < 0 && pathIdx >= s_PathStrings.size())
    {
        *bufferCountOutput = 0;
        return XR_ERROR_PATH_INVALID;
    }

    std::string& pathString = s_PathStrings[pathIdx];

    if (pathString.size() >= bufferCapacityInput)
    {
        *bufferCountOutput = 0;
        return XR_ERROR_SIZE_INSUFFICIENT;
    }

    uint32_t strSize = (uint32_t)pathString.size();
    memcpy(buffer, pathString.c_str(), strSize);

    buffer[strSize] = '\0';
    *bufferCountOutput = strSize + 1;

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrCreateActionSet(XrInstance instance, const XrActionSetCreateInfo* createInfo, XrActionSet* actionSet)
{
    LOG_FUNC();
    *actionSet = reinterpret_cast<XrActionSet>(GetNextHandleId());
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrDestroyActionSet(XrActionSet actionSet)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrCreateAction(XrActionSet actionSet, const XrActionCreateInfo* createInfo, XrAction* action)
{
    LOG_FUNC();
    *action = reinterpret_cast<XrAction>(GetNextHandleId());
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrDestroyAction(XrAction action)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrSuggestInteractionProfileBindings(XrInstance instance, const XrInteractionProfileSuggestedBinding* suggestedBindings)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrAttachSessionActionSets(XrSession session, const XrSessionActionSetsAttachInfo* attachInfo)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetCurrentInteractionProfile(XrSession session, XrPath topLevelUserPath, XrInteractionProfileState* interactionProfile)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetActionStateBoolean(XrSession session, const XrActionStateGetInfo* getInfo, XrActionStateBoolean* state)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetActionStateFloat(XrSession session, const XrActionStateGetInfo* getInfo, XrActionStateFloat* state)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetActionStateVector2f(XrSession session, const XrActionStateGetInfo* getInfo, XrActionStateVector2f* state)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetActionStatePose(XrSession session, const XrActionStateGetInfo* getInfo, XrActionStatePose* state)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrSyncActions(XrSession session, const XrActionsSyncInfo* syncInfo)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrEnumerateBoundSourcesForAction(XrSession session, const XrBoundSourcesForActionEnumerateInfo* enumerateInfo, uint32_t sourceCapacityInput, uint32_t* sourceCountOutput, XrPath* sources)
{
    LOG_FUNC();
    *sourceCountOutput = 0;
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetInputSourceLocalizedName(XrSession session, const XrInputSourceLocalizedNameGetInfo* getInfo, uint32_t bufferCapacityInput, uint32_t* bufferCountOutput, char* buffer)
{
    LOG_FUNC();
    *bufferCountOutput = 0;
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrApplyHapticFeedback(XrSession session, const XrHapticActionInfo* hapticActionInfo, const XrHapticBaseHeader* hapticFeedback)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrStopHapticFeedback(XrSession session, const XrHapticActionInfo* hapticActionInfo)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

#ifdef _WIN32
extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrConvertWin32PerformanceCounterToTimeKHR(XrInstance instance, const LARGE_INTEGER* performanceCounter, XrTime* time)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrConvertTimeToWin32PerformanceCounterKHR(XrInstance instance, XrTime time, LARGE_INTEGER* performanceCounter)
{
    LOG_FUNC();
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetD3D11GraphicsRequirementsKHR(XrInstance instance, XrSystemId systemId, XrGraphicsRequirementsD3D11KHR* graphicsRequirements)
{
    LOG_FUNC();
    return XR_SUCCESS;
}
#endif

// extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetOpenGLESGraphicsRequirementsKHR(XrInstance instance, XrSystemId systemId, XrGraphicsRequirementsOpenGLESKHR* graphicsRequirements)
// {
//     return XR_SUCCESS;
// }

#ifdef XR_USE_GRAPHICS_API_VULKAN
extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetVulkanInstanceExtensionsKHR(XrInstance instance, XrSystemId systemId, uint32_t bufferCapacityInput, uint32_t* bufferCountOutput, char* buffer)
{
    *bufferCountOutput = 0;
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetVulkanDeviceExtensionsKHR(XrInstance instance, XrSystemId systemId, uint32_t bufferCapacityInput, uint32_t* bufferCountOutput, char* buffer)
{
    *bufferCountOutput = 0;
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetVulkanGraphicsDeviceKHR(XrInstance instance, XrSystemId systemId, VkInstance vkInstance, VkPhysicalDevice* vkPhysicalDevice)
{
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetVulkanGraphicsRequirementsKHR(XrInstance instance, XrSystemId systemId, XrGraphicsRequirementsVulkanKHR* graphicsRequirements)
{
    return XR_SUCCESS;
}
#endif

extern uint32_t s_VisibilityMaskVerticesSizes[2][3];
extern uint32_t s_VisibilityMaskIndicesSizes[2][3];
extern XrVector2f s_VisibilityMaskVertices[2][3][99];
extern uint32_t s_VisibilityMaskIndices[2][3][99];

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetVisibilityMaskKHR(XrSession session, XrViewConfigurationType viewConfigurationType, uint32_t viewIndex, XrVisibilityMaskTypeKHR visibilityMaskType, XrVisibilityMaskKHR* visibilityMask)
{
    const uint32_t visiblityMaskTypeLookup = visibilityMaskType - 1;
    visibilityMask->vertexCountOutput = s_VisibilityMaskVerticesSizes[viewIndex][visiblityMaskTypeLookup];
    visibilityMask->indexCountOutput = s_VisibilityMaskIndicesSizes[viewIndex][visiblityMaskTypeLookup];

    if (visibilityMask->vertexCapacityInput == 0 || visibilityMask->indexCapacityInput == 0)
        return XR_SUCCESS;
    if (visibilityMask->vertexCapacityInput < visibilityMask->vertexCountOutput ||
        visibilityMask->indexCapacityInput < visibilityMask->indexCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    memcpy(visibilityMask->vertices, &s_VisibilityMaskVertices[viewIndex][visiblityMaskTypeLookup], sizeof(XrVector2f) * visibilityMask->vertexCountOutput);
    memcpy(visibilityMask->indices, &s_VisibilityMaskIndices[viewIndex][visiblityMaskTypeLookup], sizeof(uint32_t) * visibilityMask->indexCountOutput);

    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrGetInstanceProcAddr(XrInstance instance, const char* name, PFN_xrVoidFunction* function)
{
    LOG_FUNC();
#define LOOKUP(funcName)                           \
    if (strcmp(#funcName, name) == 0)              \
    {                                              \
        *function = (PFN_xrVoidFunction)&funcName; \
        return XR_SUCCESS;                         \
    }

    LOOKUP(xrEnumerateApiLayerProperties)
    LOOKUP(xrEnumerateInstanceExtensionProperties)
    LOOKUP(xrCreateInstance)
    LOOKUP(xrDestroyInstance)
    LOOKUP(xrGetInstanceProperties)
    LOOKUP(xrPollEvent)
    LOOKUP(xrResultToString)
    LOOKUP(xrStructureTypeToString)
    LOOKUP(xrGetSystem)
    LOOKUP(xrGetSystemProperties)
    LOOKUP(xrEnumerateEnvironmentBlendModes)
    LOOKUP(xrCreateSession)
    LOOKUP(xrDestroySession)
    LOOKUP(xrEnumerateReferenceSpaces)
    LOOKUP(xrCreateReferenceSpace)
    LOOKUP(xrGetReferenceSpaceBoundsRect)
    LOOKUP(xrCreateActionSpace)
    LOOKUP(xrLocateSpace)
    LOOKUP(xrDestroySpace)
    LOOKUP(xrEnumerateViewConfigurations)
    LOOKUP(xrGetViewConfigurationProperties)
    LOOKUP(xrEnumerateViewConfigurationViews)
    LOOKUP(xrEnumerateSwapchainFormats)
    LOOKUP(xrCreateSwapchain)
    LOOKUP(xrDestroySwapchain)
    LOOKUP(xrEnumerateSwapchainImages)
    LOOKUP(xrAcquireSwapchainImage)
    LOOKUP(xrWaitSwapchainImage)
    LOOKUP(xrReleaseSwapchainImage)
    LOOKUP(xrBeginSession)
    LOOKUP(xrEndSession)
    LOOKUP(xrRequestExitSession)
    LOOKUP(xrWaitFrame)
    LOOKUP(xrBeginFrame)
    LOOKUP(xrEndFrame)
    LOOKUP(xrLocateViews)
    LOOKUP(xrStringToPath)
    LOOKUP(xrPathToString)
    LOOKUP(xrCreateActionSet)
    LOOKUP(xrDestroyActionSet)
    LOOKUP(xrCreateAction)
    LOOKUP(xrDestroyAction)
    LOOKUP(xrSuggestInteractionProfileBindings)
    LOOKUP(xrAttachSessionActionSets)
    LOOKUP(xrGetCurrentInteractionProfile)
    LOOKUP(xrGetActionStateBoolean)
    LOOKUP(xrGetActionStateFloat)
    LOOKUP(xrGetActionStateVector2f)
    LOOKUP(xrGetActionStatePose)
    LOOKUP(xrSyncActions)
    LOOKUP(xrEnumerateBoundSourcesForAction)
    LOOKUP(xrGetInputSourceLocalizedName)
    LOOKUP(xrApplyHapticFeedback)
    LOOKUP(xrStopHapticFeedback)
    LOOKUP(xrGetVisibilityMaskKHR)

#ifdef _WIN32
    LOOKUP(xrConvertWin32PerformanceCounterToTimeKHR)
    LOOKUP(xrConvertTimeToWin32PerformanceCounterKHR)

    LOOKUP(xrGetD3D11GraphicsRequirementsKHR)
#endif

    if (IsSupportingDriverExtension(instance))
    {
        auto ret = mock_driver_xrGetInstanceProcAddr(instance, name, function);
        if (ret != XR_ERROR_FUNCTION_UNSUPPORTED)
            return ret;
    }

    // LOOKUP(xrGetOpenGLESGraphicsRequirementsKHR)

#ifdef XR_USE_GRAPHICS_API_VULKAN
    LOOKUP(xrGetVulkanInstanceExtensionsKHR)
    LOOKUP(xrGetVulkanDeviceExtensionsKHR)
    LOOKUP(xrGetVulkanGraphicsDeviceKHR)
    LOOKUP(xrGetVulkanGraphicsRequirementsKHR)
#endif

#undef LOOKUP

    return XR_ERROR_FUNCTION_UNSUPPORTED;
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetXRTrace(IUnityXRTrace* trace)
{
    s_Trace = trace;
}
