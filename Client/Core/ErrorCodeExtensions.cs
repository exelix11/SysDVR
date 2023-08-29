using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    static class Exten
    {
        private static void FailImpl(string message)
        {
            Console.WriteLine(message);
            throw new Exception(message);
        }

        public static void AssertEqual(this int code, int expectedValue, Func<string> MessageFun = null)
        {
            if (code != expectedValue)
                FailImpl($"Assertion failed {code} != {expectedValue} : {MessageFun?.Invoke() ?? "Unknown error:"}");
        }

        public static void AssertNotZero(this uint code, Func<string> MessageFun = null)
        {
            if (code == 0)
                FailImpl($"Assertion failed: {code} {MessageFun?.Invoke() ?? "Unknown error"}");
        }

        public static void AssertZero(this int code, Func<string> MessageFun = null)
        {
            if (code != 0)
                FailImpl($"Assertion failed: {code} {MessageFun?.Invoke() ?? "Unknown error"}");
        }

        public static void AssertNotNeg(this int code, Func<string> MessageFun = null)
        {
            if (code < 0)
                FailImpl($"Assertion failed: {code} {MessageFun?.Invoke() ?? "Unknown error"}");
        }

        public static void AssertZero(this int code, string Message)
        {
            if (code != 0)
                FailImpl($"Assertion failed: {code} {Message}");
        }

        public static IntPtr AssertNotNull(this IntPtr val, Func<string> MessageFun = null)
        {
            if (val == IntPtr.Zero)
                FailImpl($"Assertion failed: pointer is null {MessageFun?.Invoke() ?? "Unknown error"}");
            return val;
        }
    }
}
