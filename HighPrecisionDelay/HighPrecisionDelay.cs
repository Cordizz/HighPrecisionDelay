using System.Runtime.InteropServices;

namespace HighPrecisionDelay;

/// <summary>
///     High performance (precision) delay based on timerfd or windows mm timer.
///     It's not perfect because some time is spent to call external function.
/// </summary>
public class HighPrecisionDelay : IDelay, IDisposable
{
    private readonly IDelay delay;

    public HighPrecisionDelay()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            delay = new MultimediaDelay();
        }
        else
        {
            if (IntPtr.Size == 8)
                delay = new LinuxTimer64();
            else
                delay = new LinuxDelay();
        }
    }

    public void Dispose()
    {
        delay.Dispose();
    }

    public void Delay(int delay)
    {
        this.delay.Delay(delay);
    }
}