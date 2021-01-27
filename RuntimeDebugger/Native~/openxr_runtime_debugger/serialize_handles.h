#pragma once

#define SEND_TO_CSHARP_HANDLES(handlename)                             \
    template <>                                                        \
    void SendToCSharp<handlename>(const char* fieldname, handlename t) \
    {                                                                  \
        SendToCSharp(fieldname, (uint64_t)t);                          \
    }

XR_LIST_HANDLES(SEND_TO_CSHARP_HANDLES)

#define SEND_TO_CSHARP_HANDLES_PTRS(handlename)                          \
    template <>                                                          \
    void SendToCSharp<handlename*>(const char* fieldname, handlename* t) \
    {                                                                    \
        if (t != nullptr)                                                \
            SendToCSharp(fieldname, (uint64_t)*t);                       \
        else                                                             \
            SendToCSharp(fieldname, (uint64_t)0);                        \
    }

XR_LIST_HANDLES(SEND_TO_CSHARP_HANDLES_PTRS)
