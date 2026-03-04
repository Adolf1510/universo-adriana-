using BoticaPOS.Application.DTOs;
using BoticaPOS.Application.Interfaces;
using BoticaPOS.Domain.Enums;
using BoticaPOS.Domain.ValueObjects;
using BoticaPOS.Infrastructure.Transactions;
using Dapper;

namespace BoticaPOS.Infrastructure.Repositories;

public sealed class VentaRepository : IVentaRepository
{
    private readonly SqlUnitOfWork _uow;
    public VentaRepository(SqlUnitOfWork uow) => _uow = uow;

    public async Task<int> InsertVentaAsync(ConfirmSaleRequest request, CancellationToken ct)
    {
        var sql = @"INSERT INTO Venta(Fecha,IdUsuario,Total,Estado) OUTPUT INSERTED.IdVenta VALUES (SYSUTCDATETIME(),@IdUsuario,@Total,'Emitido')";
        return await _uow.Connection!.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { request.IdUsuario, Total = request.Lineas.Sum(x => x.Cantidad * x.PrecioUnitario) }, _uow.Transaction, cancellationToken: ct));
    }

    public Task InsertVentaDetalleAsync(int idVenta, SaleLineRequest line, CancellationToken ct)
    {
        var sql = @"INSERT INTO VentaDetalle(IdVenta,IdMedicamento,Cantidad,PrecioUnitario,SubTotal) VALUES (@idVenta,@IdMedicamento,@Cantidad,@PrecioUnitario,@SubTotal)";
        return _uow.Connection!.ExecuteAsync(new CommandDefinition(sql, new { idVenta, line.IdMedicamento, line.Cantidad, line.PrecioUnitario, SubTotal = line.Cantidad * line.PrecioUnitario }, _uow.Transaction, cancellationToken: ct));
    }

    public async Task InsertLoteAllocationsAsync(int idVenta, int idMedicamento, IReadOnlyCollection<LoteAsignacion> allocations, CancellationToken ct)
    {
        foreach (var a in allocations)
        {
            await _uow.Connection!.ExecuteAsync(new CommandDefinition("INSERT INTO VentaLoteAllocation(IdVenta,IdMedicamento,IdLote,Cantidad) VALUES(@idVenta,@idMedicamento,@IdLote,@Cantidad)", new { idVenta, idMedicamento, a.IdLote, a.Cantidad }, _uow.Transaction, cancellationToken: ct));
            await _uow.Connection.ExecuteAsync(new CommandDefinition("UPDATE Lote SET CantidadDisponible=CantidadDisponible-@Cantidad WHERE IdLote=@IdLote", new { a.Cantidad, a.IdLote }, _uow.Transaction, cancellationToken: ct));
            await _uow.Connection.ExecuteAsync(new CommandDefinition("INSERT INTO InventoryMovement(IdMedicamento,IdLote,Fecha,Tipo,Cantidad,ReferenciaId,ReferenciaTabla) VALUES(@idMedicamento,@IdLote,SYSUTCDATETIME(),@Tipo,@Cantidad,@idVenta,'Venta')", new { idMedicamento, a.IdLote, Tipo = TipoMovimientoInventario.VentaOut.ToString(), a.Cantidad, idVenta }, _uow.Transaction, cancellationToken: ct));
        }
    }

    public Task InsertPagoAsync(int idVenta, MetodoPago metodoPago, decimal monto, CancellationToken ct)
        => _uow.Connection!.ExecuteAsync(new CommandDefinition("INSERT INTO Pago(IdVenta,MetodoPago,Monto,Fecha) VALUES(@idVenta,@metodo,@monto,SYSUTCDATETIME())", new { idVenta, metodo = metodoPago.ToString(), monto }, _uow.Transaction, cancellationToken: ct));

    public async Task ReverseVentaAsync(int idVenta, string motivo, int idUsuario, CancellationToken ct)
    {
        var allocations = await _uow.Connection!.QueryAsync<(int IdLote, int IdMedicamento, int Cantidad)>(new CommandDefinition("SELECT IdLote, IdMedicamento, Cantidad FROM VentaLoteAllocation WHERE IdVenta=@idVenta", new { idVenta }, _uow.Transaction, cancellationToken: ct));
        foreach (var a in allocations)
        {
            await _uow.Connection.ExecuteAsync(new CommandDefinition("UPDATE Lote SET CantidadDisponible=CantidadDisponible+@Cantidad WHERE IdLote=@IdLote", a, _uow.Transaction, cancellationToken: ct));
            await _uow.Connection.ExecuteAsync(new CommandDefinition("INSERT INTO InventoryMovement(IdMedicamento,IdLote,Fecha,Tipo,Cantidad,ReferenciaId,ReferenciaTabla) VALUES(@IdMedicamento,@IdLote,SYSUTCDATETIME(),'AnulacionIn',@Cantidad,@idVenta,'Venta')", new { a.IdMedicamento, a.IdLote, a.Cantidad, idVenta }, _uow.Transaction, cancellationToken: ct));
        }
        await _uow.Connection.ExecuteAsync(new CommandDefinition("UPDATE Venta SET Estado='Anulado', MotivoAnulacion=@motivo WHERE IdVenta=@idVenta", new { idVenta, motivo }, _uow.Transaction, cancellationToken: ct));
        await _uow.Connection.ExecuteAsync(new CommandDefinition("INSERT INTO AuditLog(Fecha,Accion,Detalle,IdUsuario) VALUES(SYSUTCDATETIME(),'AnularVenta',@detalle,@idUsuario)", new { detalle = $"Venta {idVenta}: {motivo}", idUsuario }, _uow.Transaction, cancellationToken: ct));
    }
}
