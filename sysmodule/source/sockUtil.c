#include "sockUtil.h"

#ifdef RELEASE
#define ReturnFail(code) do { if (sock > 0) close(sock); svcSleepThread(5E+8); return -1; } while(0)
#else

#define ReturnFail(code) do { fatalThrow(ERR_SOCK_FAIL(DebugFlag, code)); return -1; } while(0)
#endif

static inline int AttemptOpenTCPListener(int port, bool LocalOnly, int DebugFlag)
{
	int err = 0, sock = -1;
	struct sockaddr_in temp;

	sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock < 0)
		ReturnFail(1);

	temp.sin_family = AF_INET;
	temp.sin_addr.s_addr = LocalOnly ? htonl(INADDR_LOOPBACK) : INADDR_ANY;
	temp.sin_port = htons(port);

	fcntl(sock, F_SETFL, O_NONBLOCK);

	const int optVal = 1;
	err = setsockopt(sock, SOL_SOCKET, SO_REUSEADDR, (void*)&optVal, sizeof(optVal));
	if (err)
		ReturnFail(2);

	err = bind(sock, (struct sockaddr*) & temp, sizeof(temp));
	if (err)
		ReturnFail(3);

	err = listen(sock, 3);
	if (err)
		ReturnFail(4);

	return sock;
}

int CreateTCPListener(int port, bool LocalOnly, int DebugFlag)
{
	int res = -1;
	do
		res = AttemptOpenTCPListener(port, LocalOnly, DebugFlag);
	while (res == -1);
	return res;
}
