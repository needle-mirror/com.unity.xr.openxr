#include "IUnityInterface.h"
#include "XR/IUnityXRTrace.h"

#define DEBUG_LOG_EVERY_FUNC_CALL 1
#define DEBUG_TRACE 1

#include "mock.h"
#include "mock_state.h"

#include "mock_driver_extension.h"

#include <cstring>

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrRequestExitSession(XrSession session);

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrTransitionMockToStateUNITY(XrSession session, XrSessionState requestedState, bool forceTransition)
{
    LOG_FUNC();
    TRACE("[Mock] [Driver] Transition request to state %d with force %s\n", requestedState, forceTransition ? "TRUE" : "FALSE");
    if (!forceTransition && !IsStateTransitionValid(session, requestedState))
    {
        TRACE("[Mock] [Driver] Failed to request state. Was transition valid: %s with force %s\n",
            IsStateTransitionValid(session, requestedState) ? "TRUE" : "FALSE",
            forceTransition ? "TRUE" : "FALSE");
        return XR_ERROR_VALIDATION_FAILURE;
    }

    TRACE("[Mock] [Driver] Transitioning to requested state %d\n", requestedState);
    ChangeSessionState(session, requestedState);
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrSetReturnCodeForFunctionUNITY(const char* functionName, XrResult result)
{
    LOG_FUNC();
    SetExpectedResultForFunction(functionName, result);
    return XR_SUCCESS;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrRequestExitSessionUNITY(XrSession session)
{
    LOG_FUNC();
    if (IsSessionAtCurrentState(session, XR_SESSION_STATE_READY) ||
        IsSessionAtCurrentState(session, XR_SESSION_STATE_SYNCHRONIZED) ||
        IsSessionAtCurrentState(session, XR_SESSION_STATE_VISIBLE) ||
        IsSessionAtCurrentState(session, XR_SESSION_STATE_FOCUSED))
        return xrRequestExitSession(session);

    return XR_ERROR_VALIDATION_FAILURE;
}

extern "C" XrResult UNITY_INTERFACE_EXPORT XRAPI_PTR xrSetBlendModeUNITY(XrEnvironmentBlendMode blendMode)
{
    SetMockBlendMode(blendMode);
    return XR_SUCCESS;
}

XrResult xrSetReferenceSpaceBoundsRectUNITY(XrSession session, XrReferenceSpaceType referenceSpace, const XrExtent2Df bounds)
{
    LOG_FUNC();
    SetExtentsForReferenceSpace(session, referenceSpace, bounds);
    return XR_SUCCESS;
}

XrResult xrCauseInstanceLossUNITY(XrInstance instance)
{
    return CauseInstanceLoss(instance);
}

XrResult xrSetSpacePoseUNITY(XrPosef pose, XrSpaceLocationFlags locationFlags)
{
    SetSpacePose(pose, locationFlags);
    return XR_SUCCESS;
}

XrResult xrSetViewPoseUNITY(int viewIndex, XrPosef pose, XrFovf fov, XrViewStateFlags viewStateFlags)
{
    SetViewPose(viewIndex, pose, fov, viewStateFlags);
    return XR_SUCCESS;
}

XrResult xrGetEndFrameStatsUNITY(int* primaryLayerCount, int* secondaryLayerCount)
{
    return GetEndFrameStats(primaryLayerCount, secondaryLayerCount);
}

XrResult mock_driver_xrGetInstanceProcAddr(XrInstance instance, const char* name, PFN_xrVoidFunction* function)
{
#define LOOKUP(funcName)                           \
    if (strcmp(#funcName, name) == 0)              \
    {                                              \
        *function = (PFN_xrVoidFunction)&funcName; \
        return XR_SUCCESS;                         \
    }

    LOOKUP(xrTransitionMockToStateUNITY)
    LOOKUP(xrSetReturnCodeForFunctionUNITY)
    LOOKUP(xrRequestExitSessionUNITY)
    LOOKUP(xrSetBlendModeUNITY)
    LOOKUP(xrSetReferenceSpaceBoundsRectUNITY)
    LOOKUP(xrCauseInstanceLossUNITY);
    LOOKUP(xrSetSpacePoseUNITY);
    LOOKUP(xrSetViewPoseUNITY);
    LOOKUP(xrGetEndFrameStatsUNITY);

#undef LOOKUP

    return XR_ERROR_FUNCTION_UNSUPPORTED;
}

#undef DEBUG_LOG_EVERY_FUNC_CALL
#undef DEBUG_TRACE
