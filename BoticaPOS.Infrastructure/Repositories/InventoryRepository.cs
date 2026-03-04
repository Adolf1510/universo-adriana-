using BoticaPOS.Application.Interfaces;
using BoticaPOS.Domain.Entities;
using BoticaPOS.Infrastructure.Data;
using Dapper;

namespace BoticaPOS.Infrastructure.Repositories;

public sealed class InventoryRepository : IInventoryRepository
{
    private readonly SqlConnectionFactory _factory;
    public InventoryRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyCollection<Lote>> GetAvailableLotsAsync(int idMedicamento, CancellationToken ct)
    {
        using var conn = _factory.Create();
        var sql = "SELECT * FROM Lote WHERE IdMedicamento=@id AND CantidadDisponible>0 ORDER BY FechaVencimiento, FechaIngreso";
        var rows = await conn.QueryAsync<Lote>(new CommandDefinition(sql, new { id = idMedicamento }, cancellationToken: ct));
        return rows.ToList();
    }
}
