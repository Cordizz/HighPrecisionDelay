using System.Runtime.InteropServices;

namespace Cordis.HighPrecisionDelay;

internal class LinuxTimer64 : IDelay, IDisposable
{
    private readonly int fileDescriptor;

    public LinuxTimer64()
    {
        fileDescriptor = Interop64.timerfd_create(Interop64.ClockIds.CLOCK_MONOTONIC, 0);

        if (fileDescriptor == -1) throw new Exception($"Unable to create timer, error = {Marshal.GetLastWin32Error()}");
    }

    public void WaitFor(int msDelay)
    {
        SetPeriod(msDelay);
        Wait();
    }

    public void Dispose()
    {
        Interop64.close(fileDescriptor);
    }

    private void SetPeriod(int periodMs)
    {
        SetFrequency((uint)periodMs * 1000); // 1000микросекунд = 1миллисекунда
    }

    private void SetFrequency(uint period)
    {
        var sec = period / 1000000;
        var ns = (period - sec * 1000000) * 1000;
        var itval = new Interop64.itimerspec64
        {
            it_interval = new Interop64.timespec64 { tv_sec = 0, tv_nsec = 0 }, // 0, чтобы установить разовый таймер
            it_value = new Interop64.timespec64 { tv_sec = sec, tv_nsec = ns }
        };

        var ret = Interop64.timerfd_settime(fileDescriptor, 0, itval, null);

        if (ret != 0) throw new Exception($"Error from timerfd_settime = {Marshal.GetLastWin32Error()}");
    }

    private long Wait()
    {
        // Wait for the next timer event. If we have missed any the number is written to "missed"
        var buf = new byte[8];
        var handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            var ret = Interop64.read(fileDescriptor, pointer, 8);

            // ret = bytes read
            if (ret < 0) throw new Exception($"Error in read = {Marshal.GetLastWin32Error()}");

            var missed = Marshal.ReadInt64(pointer);

            return missed;
        }
        finally
        {
            handle.Free();
        }
    }
}