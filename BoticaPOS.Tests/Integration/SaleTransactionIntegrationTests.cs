using Dapper;
using Microsoft.Data.SqlClient;
using Xunit;
using FluentAssertions;

namespace BoticaPOS.Tests.Integration;

public sealed class SaleTransactionIntegrationTests
{
    [Fact]
    public async Task Confirm_sale_updates_tables_consistently()
    {
        if (Environment.GetEnvironmentVariable("RUN_INTEGRATION_TESTS") != "1")
            return;

        var cs = "Server=.\\SQLEXPRESS;Database=BoticaPOS;Trusted_Connection=True;TrustServerCertificate=True;";
        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync();

        var medId = await conn.ExecuteScalarAsync<int>("SELECT TOP 1 IdMedicamento FROM Medicamento ORDER BY IdMedicamento");
        var lotId = await conn.ExecuteScalarAsync<int>("SELECT TOP 1 IdLote FROM Lote WHERE IdMedicamento=@medId", new { medId });

        var tx = conn.BeginTransaction();
        try
        {
            var idVenta = await conn.ExecuteScalarAsync<int>("INSERT INTO Venta(Fecha,IdUsuario,Total,Estado) OUTPUT INSERTED.IdVenta VALUES(SYSUTCDATETIME(),1,10,'Emitido')", transaction: tx);
            await conn.ExecuteAsync("INSERT INTO VentaDetalle(IdVenta,IdMedicamento,Cantidad,PrecioUnitario,SubTotal) VALUES(@idVenta,@medId,1,10,10)", new { idVenta, medId }, tx);
            await conn.ExecuteAsync("UPDATE Lote SET CantidadDisponible=CantidadDisponible-1 WHERE IdLote=@lotId", new { lotId }, tx);
            await conn.ExecuteAsync("INSERT INTO InventoryMovement(IdMedicamento,IdLote,Fecha,Tipo,Cantidad,ReferenciaId,ReferenciaTabla) VALUES(@medId,@lotId,SYSUTCDATETIME(),'VentaOut',1,@idVenta,'Venta')", new { medId, lotId, idVenta }, tx);
            tx.Commit();

            var ventas = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Venta WHERE IdVenta=@idVenta", new { idVenta });
            ventas.Should().Be(1);
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
