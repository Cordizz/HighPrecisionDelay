using System.Runtime.InteropServices;

namespace Cordis.HighPrecisionDelay;

internal static class NativeMethods
{
    [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeSetEvent")]
    internal static extern uint TimeSetEvent(uint msDelay,
                                             uint msResolution,
                                             MultimediaTimerCallback callback,
                                             ref uint userCtx,
                                             uint eventType);

    [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeKillEvent")]
    internal static extern void TimeKillEvent(uint uTimerId);
}