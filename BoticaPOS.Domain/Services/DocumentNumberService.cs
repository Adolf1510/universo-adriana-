namespace BoticaPOS.Domain.Services;

public sealed class DocumentNumberService
{
    private readonly object _sync = new();
    public int Next(int current)
    {
        lock (_sync)
        {
            checked { return current + 1; }
        }
    }
}
