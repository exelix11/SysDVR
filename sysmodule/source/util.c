#include <switch.h>
#include "util.h"
#include "modes/modes.h"

bool UtilSetConsoleScreenMode(bool on)
{
	Result rc = lblInitialize();
	LOG("lblInitialize %x", rc);

	if (R_SUCCEEDED(rc))
	{
		LblBacklightSwitchStatus lblstatus = LblBacklightSwitchStatus_Disabled;
		if (R_SUCCEEDED(lblGetBacklightSwitchStatus(&lblstatus)))
		{
			LOG("lblGetBacklightSwitchStatus %x", rc);
			if (on && lblstatus == LblBacklightSwitchStatus_Disabled)
				rc = lblSwitchBacklightOn(0);
			else if (!on && lblstatus == LblBacklightSwitchStatus_Enabled)
				rc = lblSwitchBacklightOff(0);
		}
	}

	LOG("UtilSetConsoleScreenMode(%d) %x", on, rc);
	return R_SUCCEEDED(rc);
}

