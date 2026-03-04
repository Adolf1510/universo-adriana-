using BoticaPOS.Application.Interfaces;
using BoticaPOS.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace BoticaPOS.Infrastructure.Caching;

public sealed class MedicamentoCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IMedicamentoRepository _repository;
    private const string Key = "med-catalog";

    public MedicamentoCacheService(IMemoryCache cache, IMedicamentoRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public Task<IReadOnlyCollection<Medicamento>> GetCatalogAsync(CancellationToken ct)
        => _cache.GetOrCreateAsync(Key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _repository.SearchAsync(string.Empty, ct);
        })!;

    public void Invalidate() => _cache.Remove(Key);
}
