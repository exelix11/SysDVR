
// We need a custom sprintf impl for rtsp since the one in libnx breaks
// when the thread stack is manually allocated (which we do to statically allocate all resources).

#define NANOPRINTF_USE_FIELD_WIDTH_FORMAT_SPECIFIERS 0
#define NANOPRINTF_USE_PRECISION_FORMAT_SPECIFIERS 0
#define NANOPRINTF_USE_FLOAT_FORMAT_SPECIFIERS 0
#define NANOPRINTF_USE_LARGE_FORMAT_SPECIFIERS 0
#define NANOPRINTF_USE_BINARY_FORMAT_SPECIFIERS 0
#define NANOPRINTF_USE_WRITEBACK_FORMAT_SPECIFIERS 0 

#define NANOPRINTF_SNPRINTF_SAFE_TRIM_STRING_ON_OVERFLOW 1

#define NANOPRINTF_IMPLEMENTATION
#include "nanoprintf.h"