using System.Runtime.InteropServices;

namespace Cordis.HighPrecisionDelay;

internal class LinuxDelay : IDelay, IDisposable
{
    private readonly int fileDescriptor;

    public LinuxDelay()
    {
        fileDescriptor = Interop.timerfd_create(Interop.ClockIds.CLOCK_MONOTONIC, 0);

        if (fileDescriptor == -1) throw new Exception($"Unable to create timer, error = {Marshal.GetLastWin32Error()}");
    }

    public void WaitFor(int msDelay)
    {
        SetPeriod(msDelay);
        Wait();
    }

    public void Dispose()
    {
        Interop.close(fileDescriptor);
    }

    private void SetPeriod(int periodMs)
    {
        SetFrequency((uint)periodMs * 1000);
    }

    private void SetFrequency(uint period)
    {
        var sec = period / 1000000;
        var ns = (period - sec * 1000000) * 1000;
        var itval = new Interop.itimerspec
        {
            it_interval = new Interop.timespec { tv_sec = 0, tv_nsec = 0 }, // 0, чтобы таймер начал отсчёт заново один раз
            it_value = new Interop.timespec { tv_sec = sec, tv_nsec = ns }
        };

        var ret = Interop.timerfd_settime(fileDescriptor, 0, itval, null);

        if (ret != 0) throw new Exception($"Error from timerfd_settime = {Marshal.GetLastWin32Error()}");
    }

    private long Wait()
    {
        // Wait for the next timer event. If we have missed any the number is written to "missed"
        var buf = new byte[16];
        var handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            var ret = Interop.read(fileDescriptor, pointer, buf.Length);

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