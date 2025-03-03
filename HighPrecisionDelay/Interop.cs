﻿using System.Runtime.InteropServices;

namespace Cordis.HighPrecisionDelay;
// Disable these StyleCop rules for this file, as we are using native names here.
#pragma warning disable IDE1006 // Element should begin with upper-case letter

internal class Interop
{
    public enum ClockIds
    {
        CLOCK_REALTIME = 0,
        CLOCK_MONOTONIC = 1,
        CLOCK_PROCESS_CPUTIME_ID = 2,
        CLOCK_THREAD_CPUTIME_ID = 3,
        CLOCK_MONOTONIC_RAW = 4,
        CLOCK_REALTIME_COARSE = 5,
        CLOCK_MONOTONIC_COARSE = 6,
        CLOCK_BOOTTIME = 7,
        CLOCK_REALTIME_ALARM = 8,
        CLOCK_BOOTTIME_ALARM = 9
    }

    private const string LibcLibrary = "libc";

    [DllImport(LibcLibrary, SetLastError = true)]
    internal static extern int timerfd_create(ClockIds clockId, int flags);

    [DllImport(LibcLibrary, SetLastError = true)]
    internal static extern int timerfd_settime(int fd, int flags, itimerspec new_value, itimerspec old_value);

    [DllImport(LibcLibrary, SetLastError = true)]
    internal static extern int read(int fd, IntPtr buf, int count);

    [DllImport(LibcLibrary)]
    internal static extern int close(int fd);

    [StructLayout(LayoutKind.Explicit)]
    public class timespec
    {
        [FieldOffset(4)]
        public uint tv_nsec; /* nanoseconds */

        [FieldOffset(0)]
        public uint tv_sec; /* seconds */
    }

    [StructLayout(LayoutKind.Explicit)]
    public class itimerspec
    {
        [FieldOffset(0)]
        public timespec it_interval; /* timer period */

        [FieldOffset(8)]
        public timespec it_value; /* timer expiration */
    }
}