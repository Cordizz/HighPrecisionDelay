using System.Runtime.InteropServices;

namespace Cordis.HighPrecisionDelay;

/// <summary>
///     High performance (precision) delay based on timerfd or windows mm timer.
///     It's not perfect because some time is spent to call an external function.
/// </summary>
public class HighPrecisionDelay : IDelay, IDisposable
{
    private readonly IDelay delay;

    public HighPrecisionDelay()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            delay = new WindowsMultimediaDelay();
        }
        else
        {
            if (IntPtr.Size == 8)
                delay = new LinuxTimer64();
            else
                delay = new LinuxDelay();
        }
    }
    /// <summary>
    /// Usually doesnt need to be disposed.
    /// </summary>
    public void Dispose()
    {
        delay.Dispose();
    }

    public void WaitFor(int msDelay)
    {
        delay.WaitFor(msDelay);
    }

    public static void Wait(int delay)
    {
        using var highPrecisionDelay = new HighPrecisionDelay();
        highPrecisionDelay.WaitFor(delay);
    }

    public static async Task WaitAsync(int delay)
    {
        await Task.Run(() =>
        {
            using var highPrecisionDelay = new HighPrecisionDelay();
            highPrecisionDelay.WaitFor(delay);
        });
    }
}