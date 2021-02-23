#include <string.h>
#include <switch.h>

static Result _grcCmdNoIO(Service* srv, u32 cmd_id) {
	return serviceDispatch(srv, cmd_id);
}

Result grcdServiceOpen(Service* out) {
    if (serviceIsActive(out))
        return 0;

    Result rc = smGetService(out, "grc:d");

    if (R_FAILED(rc)) 
		grcdServiceClose(out);

    return rc;
}

void grcdServiceClose(Service* svc){
	serviceClose(svc);
}

Result grcdServiceBegin(Service* svc){
    return _grcCmdNoIO(svc, 1);
}

Result grcdServiceTransfer(Service* srv, GrcStream stream, void* buffer, size_t size, u32* num_frames, u32* data_size, u64* start_timestamp) {
	struct {
		u32 num_frames;
		u32 data_size;
		u64 start_timestamp;
	} out;

	u32 tmp = stream;
	Result rc = serviceDispatchInOut(srv, 2, tmp, out,
		.buffer_attrs = { SfBufferAttr_HipcMapAlias | SfBufferAttr_Out },
		.buffers = { { buffer, size } },
		);
	if (R_SUCCEEDED(rc)) {
		if (num_frames) *num_frames = out.num_frames;
		if (data_size) *data_size = out.data_size;
		if (start_timestamp) *start_timestamp = out.start_timestamp;
	}
	return rc;
}