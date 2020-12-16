#pragma once

#include "openxr/openxr.h"

void AddInstance(XrInstance instance);
bool HasCurrentInstance();
XrResult GetNextEvent(XrInstance instance, XrEventDataBuffer* eventData);
void RemoveInstance(XrInstance instance);

void SetMockSession(XrInstance instance, XrSession* session);
XrInstance GetInstanceFromSession(XrSession session);
XrSession GetSessionFromInstance(XrInstance instance);
void SetSupportingDriverExtension(XrInstance instance, bool usingExtension);
bool IsSupportingDriverExtension(XrInstance instance);

bool IsStateTransitionValid(XrSession session, XrSessionState newState);
bool IsSessionAtCurrentState(XrSession session, XrSessionState state);
bool ChangeSessionStateFrom(XrSession session, XrSessionState fromState, XrSessionState toState);
void ChangeSessionState(XrSession session, XrSessionState state);
void ChangeSessionState(XrSession session, XrSessionState state);

void SetSessionRunning(XrSession session, bool running);
bool IsSessionRunning(XrSession session);

bool HasValidSession();
bool HasValidInstance();

void SetExpectedResultForFunction(const char* name, XrResult result);
XrResult GetExpectedResultForFunction(const char* name);

void SetExitRequestState(bool state);
bool HasExitBeenRequested();

void SetMockBlendMode(XrEnvironmentBlendMode blendMode);
XrEnvironmentBlendMode GetMockBlendMode();

void SetExtentsForReferenceSpace(XrSession session, XrReferenceSpaceType referenceSpace, XrExtent2Df extents);
bool GetExtentsForReferenceSpace(XrSession session, XrReferenceSpaceType referenceSpace, XrExtent2Df* extents);

XrResult CauseInstanceLoss(XrInstance instance);
bool InstanceLost(XrInstance instance);

void SetNullGfx(bool nullGfx);
bool IsNullGfx();

void SetSpacePose(XrPosef pose, XrSpaceLocationFlags locationFlags);
XrResult LocateSpace(XrSpace space, XrSpace baseSpace, XrTime time, XrSpaceLocation* location);

void SetViewPose(int viewIndex, XrPosef pose, XrFovf fov, XrViewStateFlags viewStateFlags);
XrResult LocateViews(XrSession session, const XrViewLocateInfo* viewLocateInfo, XrViewState* viewState, uint32_t viewCapacityInput, uint32_t* viewCountOutput, XrView* views);

XrResult GetEndFrameStats(int* primaryLayerCount, int* secondaryLayerCount);
XrResult EndFrame(XrSession session, const XrFrameEndInfo* frameEndInfo);
