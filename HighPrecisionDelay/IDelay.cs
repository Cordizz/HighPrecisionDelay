namespace HighPrecisionDelay;

public interface IDelay : IDisposable
{
    /// <summary>
    ///     Ожидает заданное количество времени.
    /// </summary>
    /// <param name="delay">Время ожидания в миллисекундах</param>
    void Delay(int delay);
}