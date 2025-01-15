using System.ComponentModel;
using System.Runtime.InteropServices;

namespace HighPrecisionDelay;

/// <summary>
///     A timer based on the multimedia timer API with 1ms precision.
/// </summary>
internal class MultimediaDelay : IDelay, IDisposable
{
    private const int EventTypeSingle = 0;
    private const int EventTypePeriodic = 1;

    // Hold the timer callback to prevent garbage collection.
    private readonly MultimediaTimerCallback callback;
    private readonly ManualResetEvent triggerEvent = new(false);
    private bool disposed;
    private int interval, resolution;
    private volatile uint timerId;

    /// <summary>
    ///     The period of the timer in milliseconds.
    /// </summary>
    private int Interval
    {
        get => interval;
        set
        {
            CheckDisposed();

            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            interval = value;
            if (Resolution > Interval) Resolution = value;
        }
    }

    /// <summary>
    ///     The resolution of the timer in milliseconds. The minimum resolution is 0, meaning highest possible resolution.
    /// </summary>
    private int Resolution
    {
        get => resolution;
        set
        {
            CheckDisposed();

            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            resolution = value;
        }
    }

    /// <summary>
    ///     Gets whether the timer has been started yet.
    /// </summary>
    private bool IsRunning => timerId != 0;

    public MultimediaDelay()
    {
        callback = TimerCallbackMethod;
        Resolution = 5;
        Interval = 10;
    }

    public void Delay(int delay)
    {
        SetPeriod(delay);
        Start();
        triggerEvent.WaitOne();
        Stop();
        triggerEvent.Reset();
    }

    public void Dispose()
    {
        Dispose(true);
    }

    ~MultimediaDelay()
    {
        Dispose(false);
    }

    private void Start()
    {
        CheckDisposed();

        if (IsRunning) throw new InvalidOperationException("Timer is already running");

        // Event type = 0, one off event
        // Event type = 1, periodic event
        uint userCtx = 0;
        timerId = NativeMethods.TimeSetEvent((uint)Interval, (uint)Resolution, callback, ref userCtx,
                                             EventTypePeriodic);
        if (timerId == 0)
        {
            var error = Marshal.GetLastWin32Error();

            throw new Win32Exception(error);
        }
    }

    private void Stop()
    {
        CheckDisposed();

        if (!IsRunning) throw new InvalidOperationException("Timer has not been started");

        StopInternal();
    }

    private void StopInternal()
    {
        NativeMethods.TimeKillEvent(timerId);
        timerId = 0;
        triggerEvent.Set();
    }

    private void TimerCallbackMethod(uint id, uint msg, ref uint userCtx, uint rsv1, uint rsv2)
    {
        triggerEvent.Set();
    }

    private void CheckDisposed()
    {
        if (disposed) throw new ObjectDisposedException("MultimediaTimer");
    }

    private void Dispose(bool disposing)
    {
        if (disposed) return;

        disposed = true;
        if (IsRunning) StopInternal();

        if (disposing) GC.SuppressFinalize(this);
    }

    private void SetPeriod(int periodMs)
    {
        Interval = periodMs;
    }
}