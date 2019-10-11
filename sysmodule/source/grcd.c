#include <string.h>
#include <switch.h>

static Result _grcCmdNoIO(Service* srv, u64 cmd_id) {
	IpcCommand c;
	ipcInitialize(&c);

	struct {
		u64 magic;
		u64 cmd_id;
	} *raw;

	raw = serviceIpcPrepareHeader(srv, &c, sizeof(*raw));

	raw->magic = SFCI_MAGIC;
	raw->cmd_id = cmd_id;

	Result rc = serviceIpcDispatch(srv);

	if (R_SUCCEEDED(rc)) {
		IpcParsedCommand r;
		struct {
			u64 magic;
			u64 result;
		} *resp;

		serviceIpcParse(srv, &r, sizeof(*resp));
		resp = r.Raw;

		rc = resp->result;
	}

	return rc;
}


Result grcdServiceOpen(Service* out) {
    if (serviceIsActive(out))
        return 0;

    Result rc = smGetService(out, "grc:d");

    if (R_FAILED(rc)) grcdExit();

    return rc;
}

void grcdServiceClose(Service* svc){
	serviceClose(svc);
}

Result grcdServiceBegin(Service* svc){
    return _grcCmdNoIO(svc, 1);
}

Result grcdServiceRead(Service* svc, GrcStream stream, void* buffer, size_t size, u32 *unk, u32 *data_size, u64 *timestamp) {
    IpcCommand c;
    ipcInitialize(&c);

    ipcAddRecvBuffer(&c, buffer, size, BufferType_Normal);

    struct {
        u64 magic;
        u64 cmd_id;
        u32 stream;
    } *raw;

    raw = serviceIpcPrepareHeader(svc, &c, sizeof(*raw));

    raw->magic = SFCI_MAGIC;
    raw->cmd_id = 2;
    raw->stream = stream;

    Result rc = serviceIpcDispatch(svc);

    if (R_SUCCEEDED(rc)) {
        IpcParsedCommand r;
        struct {
            u64 magic;
            u64 result;
            u32 unk;
            u32 data_size;
            u64 timestamp;
        } *resp;

        serviceIpcParse(svc, &r, sizeof(*resp));
        resp = r.Raw;

        rc = resp->result;

        if (R_SUCCEEDED(rc)) {
            if (unk) *unk = resp->unk;
            if (data_size) *data_size = resp->data_size;
            if (timestamp) *timestamp = resp->timestamp;
        }
    }

    return rc;
}

