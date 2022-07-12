#pragma once

template <>
void SendToCSharp<int32_t>(const char* fieldname, int32_t t)
{
    SendInt32(fieldname, t);
}

template <>
void SendToCSharp<int32_t*>(const char* fieldname, int32_t* t)
{
    if (t != nullptr)
        SendInt32(fieldname, *t);
    else
        SendString(fieldname, "nullptr");
}

template <>
void SendToCSharp<int64_t>(const char* fieldname, int64_t t)
{
    SendInt64(fieldname, t);
}

template <>
void SendToCSharp<uint32_t>(const char* fieldname, uint32_t t)
{
    SendUInt32(fieldname, t);
}

template <>
void SendToCSharp<uint32_t*>(const char* fieldname, uint32_t* t)
{
    if (t != nullptr)
        SendUInt32(fieldname, *t);
    else
        SendString(fieldname, "nullptr");
}

template <>
void SendToCSharp<uint64_t>(const char* fieldname, uint64_t t)
{
    SendUInt64(fieldname, t);
}

template <>
void SendToCSharp<float>(const char* fieldname, float t)
{
    SendFloat(fieldname, t);
}

template <>
void SendToCSharp<float*>(const char* fieldname, float* t)
{
    if (t != nullptr)
        SendFloat(fieldname, *t);
    else
        SendString(fieldname, "nullptr");
}

template <>
void SendToCSharp<char*>(const char* fieldname, char* t)
{
    if (t != nullptr)
        SendString(fieldname, t);
    else
        SendString(fieldname, "nullptr");
}

template <>
void SendToCSharp<char const*>(const char* fieldname, char const* t)
{
    if (t != nullptr)
        SendString(fieldname, t);
    else
        SendString(fieldname, "nullptr");
}

template <>
void SendToCSharp<char* const>(const char* fieldname, char* const t)
{
    if (t != nullptr)
        SendString(fieldname, t);
    else
        SendString(fieldname, "nullptr");
}

#if XR_TYPE_SAFE_HANDLES
template <>
void SendToCSharp<XrPath>(const char* fieldname, XrPath t)
{
    SendXrPath(fieldname, t);
}

template <>
void SendToCSharp<XrPath*>(const char* fieldname, XrPath* t)
{
    SendXrPath(fieldname, *t);
}

template <>
void SendToCSharp<XrAction>(const char* fieldname, XrAction t)
{
    SendXrAction(fieldname, t);
}

template <>
void SendToCSharp<XrAction*>(const char* fieldname, XrAction* t)
{
    SendXrAction(fieldname, *t);
}

template <>
void SendToCSharp<XrActionSet>(const char* fieldname, XrActionSet t)
{
    SendXrActionSet(fieldname, t);
}

template <>
void SendToCSharp<XrActionSet*>(const char* fieldname, XrActionSet* t)
{
    SendXrActionSet(fieldname, *t);
}

template <>
void SendToCSharp<XrSpace>(const char* fieldname, XrSpace t)
{
    SendXrSpace(fieldname, t);
}

template <>
void SendToCSharp<XrSpace*>(const char* fieldname, XrSpace* t)
{
    SendXrSpace(fieldname, *t);
}
#endif