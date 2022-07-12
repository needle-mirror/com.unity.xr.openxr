#pragma once

#if XR_TYPE_SAFE_HANDLES
template <>
void SendToCSharp<XrSystemId>(const char* fieldname, XrSystemId t)
{
    SendToCSharp(fieldname, (uint64_t)t);
}

template <>
void SendToCSharp<XrSystemId*>(const char* fieldname, XrSystemId* t)
{
    SendToCSharp(fieldname, (uint64_t)*t);
}
#endif