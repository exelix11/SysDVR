using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    static class Exten
    {
        public static void AssertEqual(this int code, int expectedValue, Func<string> MessageFun = null)
        {
            if (code != expectedValue)
                throw new Exception($"Assertion failed {code} != {expectedValue} : {MessageFun?.Invoke() ?? "Unknown error:"}");
        }

        public static void AssertNotZero(this uint code, Func<string> MessageFun = null)
        {
            if (code == 0)
                throw new Exception($"Assertion failed: {code} {MessageFun?.Invoke() ?? "Unknown error"}");
        }

        public static void AssertZero(this int code, Func<string> MessageFun = null)
        {
            if (code != 0)
                throw new Exception($"Assertion failed: {code} {MessageFun?.Invoke() ?? "Unknown error"}");
        }

        public static void AssertNotNeg(this int code, Func<string> MessageFun = null)
        {
            if (code < 0)
                throw new Exception($"Assertion failed: {code} {MessageFun?.Invoke() ?? "Unknown error"}");
        }

        public static void AssertZero(this int code, string Message)
        {
            if (code != 0)
                throw new Exception($"Assertion failed: {code} {Message}");
        }

        public static IntPtr AssertNotNull(this IntPtr val, Func<string> MessageFun = null)
        {
            if (val == IntPtr.Zero)
                throw new Exception($"Assertion failed: pointer is null {MessageFun?.Invoke() ?? "Unknown error"}");
            return val;
        }
    }
}
