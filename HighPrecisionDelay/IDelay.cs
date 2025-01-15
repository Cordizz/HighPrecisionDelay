namespace Cordis.HighPrecisionDelay;

public interface IDelay : IDisposable
{
    /// <summary>
    ///     Ожидает заданное количество времени.
    /// </summary>
    /// <param name="msDelay">Время ожидания в миллисекундах</param>
    void WaitFor(int msDelay);
}