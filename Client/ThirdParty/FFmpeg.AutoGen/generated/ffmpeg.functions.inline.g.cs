using System;

namespace FFmpeg.AutoGen;

public static unsafe partial class ffmpeg
{
    /// <summary>Compute ceil(log2(x)).</summary>
    /// <param name="x">value used to compute ceil(log2(x))</param>
    /// <returns>computed ceiling of log2(x)</returns>
    public static int av_ceil_log2_c(int @x)
    {
        return av_log2((uint)(x - 1U) << 1);
    }
    // original body hash: Y9QGw919/NB5ltczSPmZu5WZt+BfR1GGQ58ULgOxiNo=
    
    /// <summary>Clip a signed integer value into the amin-amax range.</summary>
    /// <param name="a">value to clip</param>
    /// <param name="amin">minimum value of the clip range</param>
    /// <param name="amax">maximum value of the clip range</param>
    /// <returns>clipped value</returns>
    public static int av_clip_c(int @a, int @amin, int @amax)
    {
        if (a < amin)
            return amin;
        else if (a > amax)
            return amax;
        else
            return a;
    }
    // original body hash: FGSX8EvLhMgYqP9+0z1+Clej4HxjpENDPDX7uAYLx6k=
    
    /// <summary>Clip a signed integer value into the -32768,32767 range.</summary>
    /// <param name="a">value to clip</param>
    /// <returns>clipped value</returns>
    public static short av_clip_int16_c(int @a)
    {
        if (((a + 32768U) & ~65535) != 0)
            return (short)((a >> 31) ^ 32767);
        else
            return (short)a;
    }
    // original body hash: l7ot2X+8YIG7Ze9ecaMTap87pGl9Q5kffGq1e9dS9Es=
    
    /// <summary>Clip a signed integer value into the -128,127 range.</summary>
    /// <param name="a">value to clip</param>
    /// <returns>clipped value</returns>
    public static sbyte av_clip_int8_c(int @a)
    {
        if (((a + 128U) & ~255) != 0)
            return (sbyte)((a >> 31) ^ 127);
        else
            return (sbyte)a;
    }
    // original body hash: 959D6ojD8+Bo9o7pGvHcWTnCDg5Ax0o328RGYDIiUvo=
    
    /// <summary>Clip a signed integer into the -(2^p),(2^p-1) range.</summary>
    /// <param name="a">value to clip</param>
    /// <param name="p">bit position to clip at</param>
    /// <returns>clipped value</returns>
    public static int av_clip_intp2_c(int @a, int @p)
    {
        if ((((uint)a + (1 << p)) & ~((2 << p) - 1)) != 0)
            return (a >> 31) ^ ((1 << p) - 1);
        else
            return a;
    }
    // original body hash: /qM73AkEE6w4/NOhpvKw1SVRZPxbN61+Yqc3i9L/2bM=
    
    /// <summary>Clip a signed integer value into the 0-65535 range.</summary>
    /// <param name="a">value to clip</param>
    /// <returns>clipped value</returns>
    public static ushort av_clip_uint16_c(int @a)
    {
        if ((a & (~65535)) != 0)
            return (ushort)((~a) >> 31);
        else
            return (ushort)a;
    }
    // original body hash: nI5Vkw30nAjS2NmNSdCSnHeAUcY47XT0lnrnsUK/bJ4=
    
    /// <summary>Clip a signed integer value into the 0-255 range.</summary>
    /// <param name="a">value to clip</param>
    /// <returns>clipped value</returns>
    public static byte av_clip_uint8_c(int @a)
    {
        if ((a & (~255)) != 0)
            return (byte)((~a) >> 31);
        else
            return (byte)a;
    }
    // original body hash: 32OGGgXBFRL7EcU8DizK9KbIFfU356+5hgUEyAOjIUY=
    
    /// <summary>Clip a signed integer to an unsigned power of two range.</summary>
    /// <param name="a">value to clip</param>
    /// <param name="p">bit position to clip at</param>
    /// <returns>clipped value</returns>
    public static uint av_clip_uintp2_c(int @a, int @p)
    {
        if ((a & ~((1 << p) - 1)) != 0)
            return (uint)((~a) >> 31 & ((1 << p) - 1));
        else
            return (uint)a;
    }
    // original body hash: 01v+7HjG6Id/YAdTCeWBkPwvakfGiCosPM6u5MXI8pU=
    
    /// <summary>Clip a signed 64bit integer value into the amin-amax range.</summary>
    /// <param name="a">value to clip</param>
    /// <param name="amin">minimum value of the clip range</param>
    /// <param name="amax">maximum value of the clip range</param>
    /// <returns>clipped value</returns>
    public static long av_clip64_c(long @a, long @amin, long @amax)
    {
        if (a < amin)
            return amin;
        else if (a > amax)
            return amax;
        else
            return a;
    }
    // original body hash: FGSX8EvLhMgYqP9+0z1+Clej4HxjpENDPDX7uAYLx6k=
    
    /// <summary>Clip a double value into the amin-amax range. If a is nan or -inf amin will be returned. If a is +inf amax will be returned.</summary>
    /// <param name="a">value to clip</param>
    /// <param name="amin">minimum value of the clip range</param>
    /// <param name="amax">maximum value of the clip range</param>
    /// <returns>clipped value</returns>
    public static double av_clipd_c(double @a, double @amin, double @amax)
    {
        return ((((a) > (amin) ? (a) : (amin))) > (amax) ? (amax) : (((a) > (amin) ? (a) : (amin))));
    }
    // original body hash: 3g76qefPWCYqXraY2vYdxoH58/EKn5EeR9v7cGEBM6Y=
    
    /// <summary>Clip a float value into the amin-amax range. If a is nan or -inf amin will be returned. If a is +inf amax will be returned.</summary>
    /// <param name="a">value to clip</param>
    /// <param name="amin">minimum value of the clip range</param>
    /// <param name="amax">maximum value of the clip range</param>
    /// <returns>clipped value</returns>
    public static float av_clipf_c(float @a, float @amin, float @amax)
    {
        return ((((a) > (amin) ? (a) : (amin))) > (amax) ? (amax) : (((a) > (amin) ? (a) : (amin))));
    }
    // original body hash: 3g76qefPWCYqXraY2vYdxoH58/EKn5EeR9v7cGEBM6Y=
    
    /// <summary>Clip a signed 64-bit integer value into the -2147483648,2147483647 range.</summary>
    /// <param name="a">value to clip</param>
    /// <returns>clipped value</returns>
    public static int av_clipl_int32_c(long @a)
    {
        if ((((ulong)a + 2147483648UL) & ~(4294967295UL)) != 0)
            return (int)((a >> 63) ^ 2147483647);
        else
            return (int)a;
    }
    // original body hash: 00dWv9FNYsEeRh1lPjYlSw3TQiOlthet3Kyi6z91Hbo=
    
    /// <summary>Compare two rationals.</summary>
    /// <param name="a">First rational</param>
    /// <param name="b">Second rational</param>
    /// <returns>One of the following values: - 0 if `a == b` - 1 if `a &gt; b` - -1 if `a &lt; b` - `INT_MIN` if one of the values is of the form `0 / 0`</returns>
    public static int av_cmp_q(AVRational @a, AVRational @b)
    {
        long tmp = a.num * (long)b.den - b.num * (long)a.den;
        if (tmp != 0)
            return (int)((tmp ^ a.den ^ b.den) >> 63) | 1;
        else if (b.den != 0 && a.den != 0)
            return 0;
        else if (a.num != 0 && b.num != 0)
            return (a.num >> 31) - (b.num >> 31);
        else
            return (-2147483647 - 1);
    }
    // original body hash: M+RGb5gXGdDjfY/gK5ZeCYeYrZAxjTXZA9+XVu0I66Q=
    
    /// <summary>Reinterpret a double as a 64-bit integer.</summary>
    public static ulong av_double2int(double @f)
    {
        return (ulong)@f;
    }
    // original body hash: 2HuHK8WLchm3u+cK6H4QWhflx2JqfewtaSpj2Cwfi8M=
    
    /// <summary>Reinterpret a float as a 32-bit integer.</summary>
    public static uint av_float2int(float @f)
    {
        return (uint)@f;
    }
    // original body hash: uBvsHd8EeFnxDvSdDE1+k5Um29kCuf0aEJhAvDy0wZk=
    
    /// <summary>Reinterpret a 64-bit integer as a double.</summary>
    public static double av_int2double(ulong @i)
    {
        return (double)@i;
    }
    // original body hash: iFt3hVHTpF9jjqIGAAf/c7FrGfenOXGxdsyMjmrbwvw=
    
    /// <summary>Reinterpret a 32-bit integer as a float.</summary>
    public static float av_int2float(uint @i)
    {
        return (float)@i;
    }
    // original body hash: wLGFPpW+aIvxW79y6BVY1LKz/j7yc3BdiaJ7mD4oQmw=
    
    /// <summary>Invert a rational.</summary>
    /// <param name="q">value</param>
    /// <returns>1 / q</returns>
    public static AVRational av_inv_q(AVRational @q)
    {
        var r = new AVRational { @num = q.den, @den = q.num };
        return r;
    }
    // original body hash: sXbO4D7vmayAx56EFqz9C0kakcSPSryJHdk0hr0MOFY=
    
    /// <summary>Fill the provided buffer with a string containing an error string corresponding to the AVERROR code errnum.</summary>
    /// <param name="errbuf">a buffer</param>
    /// <param name="errbuf_size">size in bytes of errbuf</param>
    /// <param name="errnum">error code to describe</param>
    /// <returns>the buffer in input, filled with the error description</returns>
    public static byte* av_make_error_string(byte* @errbuf, ulong @errbuf_size, int @errnum)
    {
        av_strerror(errnum, errbuf, errbuf_size);
        return errbuf;
    }
    // original body hash: DRHQHyLQNo9pTxA+wRw4zVDrC7Md1u3JWawQX0BVkqE=
    
    /// <summary>Create an AVRational.</summary>
    public static AVRational av_make_q(int @num, int @den)
    {
        var r = new AVRational { @num = num, @den = den };
        return r;
    }
    // original body hash: IAPYNNcg3GX0PGxINeLQhb41dH921lPVKcnqxCk7ERA=
    
    /// <summary>Clear high bits from an unsigned integer starting with specific bit position</summary>
    /// <param name="a">value to clip</param>
    /// <param name="p">bit position to clip at</param>
    /// <returns>clipped value</returns>
    public static uint av_mod_uintp2_c(uint @a, uint @p)
    {
        return a & (uint)((1 << (int)p) - 1);
    }
    // original body hash: ncn4Okxr9Nas1g/qCfpRHKtywuNmJuf3UED+o3wjadc=
    
    public static int av_parity_c(uint @v)
    {
        return av_popcount_c(v) & 1;
    }
    // original body hash: Hsrq5CWkNvuNTnqES92ZJYVYpKXFwosrZNja/oaUd0s=
    
    /// <summary>Count number of bits set to one in x</summary>
    /// <param name="x">value to count bits of</param>
    /// <returns>the number of bits set to one in x</returns>
    public static int av_popcount_c(uint @x)
    {
        x -= (x >> 1) & 1431655765;
        x = (x & 858993459) + ((x >> 2) & 858993459);
        x = (x + (x >> 4)) & 252645135;
        x += x >> 8;
        return (int)((x + (x >> 16)) & 63);
    }
    // original body hash: 6EqV8Ll7t/MGINV9Nh3TSEbNyUYeskm7HucpU0SAkgg=
    
    /// <summary>Count number of bits set to one in x</summary>
    /// <param name="x">value to count bits of</param>
    /// <returns>the number of bits set to one in x</returns>
    public static int av_popcount64_c(ulong @x)
    {
        return av_popcount_c((uint)x) + av_popcount_c((uint)(x >> 32));
    }
    // original body hash: 4wjPAKU9R0yS6OI8Y9h3L6de+uXt/lBm+zX7t5Ch18k=
    
    /// <summary>Convert an AVRational to a `double`.</summary>
    /// <param name="a">AVRational to convert</param>
    /// <returns>`a` in floating-point form</returns>
    public static double av_q2d(AVRational @a)
    {
        return a.num / (double)a.den;
    }
    // original body hash: j4R2BS8nF6czcUDVk5kKi9nLEdlTI/NRDYtnc1KFeyE=
    
    /// <summary>Add two signed 32-bit values with saturation.</summary>
    /// <param name="a">one value</param>
    /// <param name="b">another value</param>
    /// <returns>sum with signed saturation</returns>
    public static int av_sat_add32_c(int @a, int @b)
    {
        return av_clipl_int32_c((long)a + b);
    }
    // original body hash: GAAy4GsS2n+9kJ/8hzuONPUOGIsiOj7PvXnLHUVrimY=
    
    /// <summary>Add two signed 64-bit values with saturation.</summary>
    /// <param name="a">one value</param>
    /// <param name="b">another value</param>
    /// <returns>sum with signed saturation</returns>
    public static long av_sat_add64_c(long @a, long @b)
    {
        try
        {
            return @a + @b;
        }
        catch (OverflowException)
        {
            return ((double)@a + (double)@b) > 0d ? long.MaxValue : long.MinValue;
        }
    }
    // original body hash: qeup76rp1rjakhMYQJWWEYIkpgscUcDfzDIrjyqk5iM=
    
    /// <summary>Add a doubled value to another value with saturation at both stages.</summary>
    /// <param name="a">first value</param>
    /// <param name="b">value doubled and added to a</param>
    /// <returns>sum sat(a + sat(2*b)) with signed saturation</returns>
    public static int av_sat_dadd32_c(int @a, int @b)
    {
        return av_sat_add32_c(a, av_sat_add32_c(b, b));
    }
    // original body hash: Kbha6XFULk7dxB6zc5WRwoPczQVN7HBcNs9Hjlj/Caw=
    
    /// <summary>Subtract a doubled value from another value with saturation at both stages.</summary>
    /// <param name="a">first value</param>
    /// <param name="b">value doubled and subtracted from a</param>
    /// <returns>difference sat(a - sat(2*b)) with signed saturation</returns>
    public static int av_sat_dsub32_c(int @a, int @b)
    {
        return av_sat_sub32_c(a, av_sat_add32_c(b, b));
    }
    // original body hash: ypu4i+30n3CeMxdL8pq7XDYAFBi1N5d2mkIT6zQ1bO0=
    
    /// <summary>Subtract two signed 32-bit values with saturation.</summary>
    /// <param name="a">one value</param>
    /// <param name="b">another value</param>
    /// <returns>difference with signed saturation</returns>
    public static int av_sat_sub32_c(int @a, int @b)
    {
        return av_clipl_int32_c((long)a - b);
    }
    // original body hash: /tgXI2zbIgliqOwZbpnq7jSiVj0N70RjBFsbkIkWhsM=
    
    /// <summary>Subtract two signed 64-bit values with saturation.</summary>
    /// <param name="a">one value</param>
    /// <param name="b">another value</param>
    /// <returns>difference with signed saturation</returns>
    public static long av_sat_sub64_c(long @a, long @b)
    {
        try
        {
            return @a - @b;
        }
        catch (OverflowException)
        {
            return ((double)@a - (double)@b) > 0d ? long.MaxValue : long.MinValue;
        }
    }
    // original body hash: 6YrSxDrYVG1ac1wlCiXKMhTwj7Kx6eym/YtspKusrGk=
    
    /// <summary>Return x default pointer in case p is NULL.</summary>
    public static void* av_x_if_null(void* @p, void* @x)
    {
        return (void*)(p != null ? p : x);
    }
    // original body hash: zOY924eIk3VeTSNb9XcE2Yw8aZ4/jlzQSfP06k5n0nU=
    
    /// <summary>ftell() equivalent for AVIOContext.</summary>
    /// <returns>position or AVERROR.</returns>
    public static long avio_tell(AVIOContext* @s)
    {
        return avio_seek(s, 0, 1);
    }
    // original body hash: o18c3ypeh9EsmYaplTel2ssgM2PZKTTDfMjsqEopycw=
    
}
