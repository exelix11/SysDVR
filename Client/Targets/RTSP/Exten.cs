using Rtsp.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SysDVR.Client.Targets.RTSP
{
    static internal class Exten
    {
        public static RtspResponse CreateResponse(this RtspRequestOptions t, string options)
        {
            RtspResponse response = t.CreateResponse();
            response.Headers[RtspHeaderNames.Public] = options;
            return response;
        }
    }
}
