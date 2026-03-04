using BoticaPOS.Application.Interfaces;
using BoticaPOS.Domain.Entities;
using BoticaPOS.Infrastructure.Data;
using Dapper;

namespace BoticaPOS.Infrastructure.Repositories;

public sealed class MedicamentoRepository : IMedicamentoRepository
{
    private readonly SqlConnectionFactory _factory;
    public MedicamentoRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyCollection<Medicamento>> SearchAsync(string query, CancellationToken ct)
    {
        using var conn = _factory.Create();
        var sql = @"SELECT TOP 200 * FROM Medicamento WHERE NombreComercial LIKE @q OR CodigoBarras LIKE @q ORDER BY NombreComercial";
        var data = await conn.QueryAsync<Medicamento>(new CommandDefinition(sql, new { q = $"%{query}%" }, cancellationToken: ct));
        return data.ToList();
    }

    public async Task UpsertAsync(Medicamento entity, CancellationToken ct)
    {
        using var conn = _factory.Create();
        var sql = entity.IdMedicamento == 0
            ? @"INSERT INTO Medicamento(NombreComercial,PrincipioActivo,Laboratorio,Presentacion,Concentracion,CodigoBarras,PrecioVenta,CostoCompra,RequiereReceta,StockMinimo,Estado) VALUES (@NombreComercial,@PrincipioActivo,@Laboratorio,@Presentacion,@Concentracion,@CodigoBarras,@PrecioVenta,@CostoCompra,@RequiereReceta,@StockMinimo,@Estado)"
            : @"UPDATE Medicamento SET NombreComercial=@NombreComercial,PrecioVenta=@PrecioVenta,StockMinimo=@StockMinimo,Estado=@Estado WHERE IdMedicamento=@IdMedicamento";
        await conn.ExecuteAsync(new CommandDefinition(sql, entity, cancellationToken: ct));
    }
}
