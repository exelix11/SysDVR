using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        static string ErrorMessage(Func<string>? MessageFun)
        {
            if (MessageFun == null)
                return "Unknown error function";
            
            var msg = MessageFun();

            if (string.IsNullOrWhiteSpace(msg))
                return "Unknown error";

            return msg;
        }

        public static void AssertTrue(this bool value, string message, [CallerMemberName] string? caller = null)
        {
            if (!value)
                FailImpl($"Call in {caller} failed: {value} is false {message}");
        }

        public static void AssertEqual(this int code, int expectedValue, Func<string> MessageFun = null, [CallerMemberName] string? caller = null)
        {
            if (code != expectedValue)
                FailImpl($"Call in {caller} failed: {code} != {expectedValue} {ErrorMessage(MessageFun)}");
        }

        public static void AssertNotZero(this uint code, Func<string> MessageFun = null, [CallerMemberName] string? caller = null)
        {
            if (code == 0)
                FailImpl($"Call in {caller} failed: {code} {ErrorMessage(MessageFun)}");
        }

        public static void AssertZero(this int code, Func<string> MessageFun = null, [CallerMemberName] string? caller = null)
        {
            if (code != 0)
                FailImpl($"Call in {caller} failed: {code} {ErrorMessage(MessageFun)}");
        }

        public static void AssertNotNeg(this int code, Func<string> MessageFun = null, [CallerMemberName] string? caller = null)
        {
            if (code < 0)
                FailImpl($"Call in {caller} failed: {code} {ErrorMessage(MessageFun)}");
        }

        public static void AssertZero(this int code, string Message, [CallerMemberName] string? caller = null)
        {
            if (code != 0)
                FailImpl($"Call in {caller} failed: {code} {Message}");
        }

        public static IntPtr AssertNotNull(this IntPtr val, Func<string> MessageFun = null, [CallerMemberName] string? caller = null)
        {
            if (val == IntPtr.Zero)
                FailImpl($"Call in {caller} failed: pointer is null {ErrorMessage(MessageFun)}");
            return val;
        }
    }
}
