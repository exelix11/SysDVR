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
 * @brief Retrieves stream data from the continuous recorder in use (from the video recording of the currently running application).
 * @note This will block until data is available. This will hang if there is no application running which has video capture enabled.
 * @param[in] stream \ref GrcStream
 * @param[out] buffer Output buffer.
 * @param[in] size Max size of the output buffer.
 * @param[out] num_frames num_frames
 * @param[out] data_size Actual output data size.
 * @param[out] start_timestamp Start timestamp.
 */
Result grcdServiceTransfer(Service* srv, GrcStream stream, void* buffer, size_t size, u32* num_frames, u32* data_size, u64* start_timestamp);

