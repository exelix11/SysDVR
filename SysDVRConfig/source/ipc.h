#pragma once
#include <switch.h>

Result SysDvrConnect();
void SysDvrClose();

Result SysDvrGetVersion(u32* out_ver);
Result SysDvrGetMode(u32* out_mode);

Result SysDvrSetUSB();
Result SysDvrSetRTSP();
Result SysDvrSetTCP();
Result SysDvrSetOFF();