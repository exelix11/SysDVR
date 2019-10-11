#pragma once
#include <switch.h>

//Mostly taken from libnx but edited to allow multiple sessions to grc:d

/// Initialize grc:d.
Result grcdServiceOpen(Service* out);

/// Exit grc:d.
void grcdServiceClose(Service* svc);

/// Begins streaming. This must not be called more than once, even from a different service session: otherwise the sysmodule will assert.
Result grcdServiceBegin(Service* svc);

/**
 * @brief Reads a stream, from the video recording being done of the currently running game title.
 * @note This will block until data is available. This will hang if there is no game title running which has video capture enabled.
 * @param[in] stream \ref GrcStream
 * @param[out] buffer Output buffer.
 * @param[in] size Max size of the output buffer.
 * @param[out] unk Unknown.
 * @param[out] data_size Actual output data size.
 * @param[out] timestamp Timestamp?
 */
Result grcdServiceRead(Service* svc, GrcStream stream, void* buffer, size_t size, u32 *unk, u32 *data_size, u64 *timestamp);

