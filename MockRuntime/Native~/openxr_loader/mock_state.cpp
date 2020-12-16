#include "mock_state.h"
#include "mock.h"

#include "XR/IUnityXRTrace.h"

#include <chrono>
#include <map>
#include <queue>
#include <string>
#include <unordered_map>

#include "openxr/openxr_reflection.h"

#include "enums_to_string.h"

IUnityXRTrace* s_Trace = nullptr;

// TODO: Make thread safe
static std::unordered_map<std::string, XrResult> s_FunctionResultMap{};

// TODO: Make thread safe
struct MockState
{
    typedef std::map<XrReferenceSpaceType, XrExtent2Df> ExtentMap;

    std::queue<XrEventDataBuffer> eventQueue;
    XrInstance instance;
    XrSession session;
    XrSessionState currentState;
    XrEnvironmentBlendMode blendMode{XR_ENVIRONMENT_BLEND_MODE_OPAQUE};
    bool supportingDriverExtension;
    bool isRunning;

    ExtentMap extents;

    bool instanceIsLost;
    bool nullGfx;

    int primaryLayersRendered;
    int secondaryLayersRendered;

    XrPosef spacePose;
    XrSpaceLocationFlags spaceLocationFlags;

    XrViewStateFlags viewStateFlags;
    XrPosef viewPose[2];
    XrFovf viewFov[2];

} s_MockState{};

static void ResetMockState(XrInstance instance)
{
    s_MockState = {};

    // Default pose is identity pose
    s_MockState.spacePose = {{0.0f, 0.0f, 0.0f, 1.0f}, {0.0f, 0.0f, 0.0f}};
    s_MockState.spaceLocationFlags =
        XR_SPACE_LOCATION_ORIENTATION_VALID_BIT |
        XR_SPACE_LOCATION_POSITION_VALID_BIT |
        XR_SPACE_LOCATION_ORIENTATION_TRACKED_BIT |
        XR_SPACE_LOCATION_POSITION_TRACKED_BIT;

    // Matches unity's mock hmd which is modeled after vive.
    s_MockState.viewPose[0] = {{0.0f, 0.0f, 0.0f, 1.0f}, {-0.011f, 0.0f, 0.0f}};
    s_MockState.viewPose[1] = {{0.0f, 0.0f, 0.0f, 1.0f}, {0.011f, 0.0f, 0.0f}};
    s_MockState.viewFov[0] = {-0.995535672f, 0.811128199f, 0.954059243f, -0.954661012f};
    s_MockState.viewFov[1] = {-0.812360585f, 0.995566666f, 0.955580175f, -0.953877985f};
    s_MockState.viewStateFlags =
        XR_VIEW_STATE_ORIENTATION_TRACKED_BIT |
        XR_VIEW_STATE_ORIENTATION_VALID_BIT |
        XR_VIEW_STATE_POSITION_TRACKED_BIT |
        XR_VIEW_STATE_POSITION_VALID_BIT;
}

bool HasCurrentInstance()
{
    return s_MockState.instance != XR_NULL_HANDLE;
}

void AddInstance(XrInstance instance)
{
    ResetMockState(instance);
    s_MockState.instance = instance;
}

void RemoveInstance(XrInstance instance)
{
    ResetMockState(instance);
}

void SetMockSession(XrInstance instance, XrSession* session)
{
    if (s_MockState.instance == instance)
        s_MockState.session = session ? *session : nullptr;
}

XrInstance GetInstanceFromSession(XrSession session)
{
    if (s_MockState.session == session)
        return s_MockState.instance;
    return (XrInstance)XR_NULL_HANDLE;
}

XrSession GetSessionFromInstance(XrInstance instance)
{
    if (s_MockState.instance == instance)
        return s_MockState.session;
    return (XrSession)XR_NULL_HANDLE;
}

XrResult GetNextEvent(XrInstance instance, XrEventDataBuffer* eventData)
{
    if (!eventData)
        return XR_ERROR_HANDLE_INVALID;

    if (s_MockState.eventQueue.size() > 0)
    {
        *eventData = s_MockState.eventQueue.front();
        if (s_Trace)
            s_Trace->Trace(kXRLogTypeDebug, "  - Returning event type: %s\n", to_string(eventData->type));
        s_MockState.eventQueue.pop();
        return XR_SUCCESS;
    }

    return XR_EVENT_UNAVAILABLE;
}

bool IsStateTransitionValid(XrSession session, XrSessionState newState)
{
    if (newState == XR_SESSION_STATE_LOSS_PENDING)
        return true;

    switch (s_MockState.currentState)
    {
    case XR_SESSION_STATE_IDLE:
        return newState == XR_SESSION_STATE_READY || newState == XR_SESSION_STATE_EXITING;
    case XR_SESSION_STATE_READY:
        return newState == XR_SESSION_STATE_SYNCHRONIZED;
    case XR_SESSION_STATE_SYNCHRONIZED:
        return newState == XR_SESSION_STATE_STOPPING || newState == XR_SESSION_STATE_VISIBLE;
    case XR_SESSION_STATE_VISIBLE:
        return newState == XR_SESSION_STATE_SYNCHRONIZED || newState == XR_SESSION_STATE_FOCUSED;
    case XR_SESSION_STATE_FOCUSED:
        return newState == XR_SESSION_STATE_VISIBLE;
    case XR_SESSION_STATE_STOPPING:
        return newState == XR_SESSION_STATE_IDLE;
    case XR_SESSION_STATE_LOSS_PENDING:
        return newState == XR_SESSION_STATE_LOSS_PENDING;
    case XR_SESSION_STATE_EXITING:
        return newState == XR_SESSION_STATE_IDLE;
    default:
        return false;
    }
}

bool IsSessionAtCurrentState(XrSession session, XrSessionState state)
{
    return s_MockState.currentState == state;
}

bool ChangeSessionStateFrom(XrSession session, XrSessionState fromState, XrSessionState toState)
{
    if (s_Trace)
        s_Trace->Trace(kXRLogTypeDebug, "  - Transitioning from state %s => %s\n", to_string(fromState), to_string(toState));

    if (!IsSessionAtCurrentState(session, fromState))
        return false;

    ChangeSessionState(session, toState);
    return true;
}

void ChangeSessionState(XrSession session, XrSessionState state)
{
    if (s_MockState.currentState == state)
        return;

    if (s_Trace)
        s_Trace->Trace(kXRLogTypeDebug, "  - Settings state to %s\n", to_string(state));

    s_MockState.currentState = state;

    s_MockState.eventQueue.emplace();
    auto& evt = *(XrEventDataSessionStateChanged*)&s_MockState.eventQueue.back();
    evt.type = XR_TYPE_EVENT_DATA_SESSION_STATE_CHANGED;
    evt.next = nullptr;
    evt.session = s_MockState.session;
    evt.state = state;
}

void SetSupportingDriverExtension(XrInstance instance, bool usingExtension)
{
    s_MockState.supportingDriverExtension = usingExtension;
}

bool IsSupportingDriverExtension(XrInstance instance)
{
    return s_MockState.supportingDriverExtension;
}

void SetSessionRunning(XrSession session, bool running)
{
    s_MockState.isRunning = running;
}

bool IsSessionRunning(XrSession session)
{
    return s_MockState.isRunning;
}

bool HasValidSession()
{
    return s_MockState.session != XR_NULL_HANDLE;
}

bool HasValidInstance()
{
    return s_MockState.instance != XR_NULL_HANDLE;
}

void SetExpectedResultForFunction(const char* functionName, XrResult result)
{
    s_FunctionResultMap[functionName] = result;
}

XrResult GetExpectedResultForFunction(const char* functionName)
{
    // Assume success if nothing else specified.
    XrResult ret = XR_SUCCESS;

    auto it = s_FunctionResultMap.find(functionName);
    if (it != s_FunctionResultMap.end())
    {
        XrResult ret = it->second;
        s_FunctionResultMap.erase(it);
        return ret;
    }

    return ret;
}

static bool s_ExitHasBeenRequested = false;

void SetExitRequestState(bool state)
{
    s_ExitHasBeenRequested = state;
}

bool HasExitBeenRequested()
{
    return s_ExitHasBeenRequested;
}

void SetMockBlendMode(XrEnvironmentBlendMode blendMode)
{
    s_MockState.blendMode = blendMode;
}

XrEnvironmentBlendMode GetMockBlendMode()
{
    return s_MockState.blendMode;
}

void SetExtentsForReferenceSpace(XrSession session, XrReferenceSpaceType referenceSpace, XrExtent2Df extents)
{
    s_MockState.extents[referenceSpace] = extents;

    // queue a reference space changed pending event
    s_MockState.eventQueue.emplace();

    auto& evt = *(XrEventDataReferenceSpaceChangePending*)&s_MockState.eventQueue.back();

    evt.type = XR_TYPE_EVENT_DATA_REFERENCE_SPACE_CHANGE_PENDING;
    evt.next = nullptr;
    evt.session = s_MockState.session;
    evt.referenceSpaceType = referenceSpace;
    evt.changeTime = 0;
    evt.poseValid = false;
    evt.poseInPreviousSpace = {{0.0f, 0.0f, 0.0f, 1.0f}, {0.0f, 0.0f, 0.0f}};
}

bool GetExtentsForReferenceSpace(XrSession session, XrReferenceSpaceType referenceSpace, XrExtent2Df* extents)
{
    auto bounds = s_MockState.extents.find(referenceSpace);
    if (bounds != s_MockState.extents.end())
    {
        *extents = bounds->second;
        return true;
    }
    return false;
}

XrResult CauseInstanceLoss(XrInstance instance)
{
    if (instance == s_MockState.instance)
    {
        s_MockState.instanceIsLost = true;
        s_MockState.eventQueue.emplace();

        auto now = std::chrono::system_clock::now();
        auto killTime = now + std::chrono::seconds(5);

        auto& evt = *(XrEventDataInstanceLossPending*)&s_MockState.eventQueue.back();

        evt.type = XR_TYPE_EVENT_DATA_INSTANCE_LOSS_PENDING;
        evt.next = nullptr;
        evt.lossTime = killTime.time_since_epoch().count();
        return XR_SUCCESS;
    }

    return XR_ERROR_HANDLE_INVALID;
}

bool InstanceLost(XrInstance instance)
{
    if (instance == s_MockState.instance)
        return s_MockState.instanceIsLost;

    return false;
}

void SetNullGfx(bool nullGfx)
{
    s_MockState.nullGfx = nullGfx;
}

bool IsNullGfx()
{
    return s_MockState.nullGfx;
}

void SetSpacePose(XrPosef pose, XrSpaceLocationFlags locationFlags)
{
    s_MockState.spacePose = pose;
    s_MockState.spaceLocationFlags = locationFlags;
}

XrResult LocateSpace(XrSpace space, XrSpace baseSpace, XrTime time, XrSpaceLocation* location)
{
    location->pose = s_MockState.spacePose;
    location->locationFlags = s_MockState.spaceLocationFlags;
    return XR_SUCCESS;
}

void SetViewPose(int viewIndex, XrPosef pose, XrFovf fov, XrViewStateFlags viewStateFlags)
{
    if (viewIndex < 0 || viewIndex > 1)
        return;

    s_MockState.viewStateFlags = viewStateFlags;
    s_MockState.viewFov[viewIndex] = fov;
    s_MockState.viewPose[viewIndex] = pose;
}

XrResult LocateViews(XrSession session, const XrViewLocateInfo* viewLocateInfo, XrViewState* viewState, uint32_t viewCapacityInput, uint32_t* viewCountOutput, XrView* views)
{
    *viewCountOutput = 2;
    if (viewCapacityInput == 0)
        return XR_SUCCESS;
    if (viewCapacityInput < *viewCountOutput)
        return XR_ERROR_VALIDATION_FAILURE;

    viewState->viewStateFlags = s_MockState.viewStateFlags;
    views[0].pose = s_MockState.viewPose[0];
    views[0].fov = s_MockState.viewFov[0];
    views[1].pose = s_MockState.viewPose[1];
    views[1].fov = s_MockState.viewFov[1];

    return XR_SUCCESS;
}

XrResult GetEndFrameStats(int* primaryLayersRendered, int* secondaryLayersRendered)
{
    *primaryLayersRendered = s_MockState.primaryLayersRendered;
    *secondaryLayersRendered = s_MockState.secondaryLayersRendered;
    return XR_SUCCESS;
}

XrResult EndFrame(XrSession session, const XrFrameEndInfo* frameEndInfo)
{
    s_MockState.primaryLayersRendered = frameEndInfo->layerCount;
    s_MockState.secondaryLayersRendered = frameEndInfo->next ? ((const XrFrameEndInfo*)frameEndInfo->next)->layerCount : 0;
    return XR_SUCCESS;
}

void VisibilityMaskChangedKHR(XrSession session, XrViewConfigurationType viewConfigurationType, uint32_t viewIndex)
{
    s_MockState.eventQueue.emplace();
    auto& evt = *(XrEventDataVisibilityMaskChangedKHR*)&s_MockState.eventQueue.back();
    evt.type = XR_TYPE_EVENT_DATA_VISIBILITY_MASK_CHANGED_KHR;
    evt.next = nullptr;
    evt.session = session;
    evt.viewConfigurationType = viewConfigurationType;
    evt.viewIndex = viewIndex;
}
