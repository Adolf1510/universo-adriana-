using BoticaPOS.Application.DTOs;
using BoticaPOS.Domain.Entities;

namespace BoticaPOS.Application.Interfaces;

public interface IMedicamentoRepository
{
    Task<IReadOnlyCollection<Medicamento>> SearchAsync(string query, CancellationToken ct);
    Task UpsertAsync(Medicamento entity, CancellationToken ct);
}

public interface IInventoryRepository
{
    Task<IReadOnlyCollection<Lote>> GetAvailableLotsAsync(int idMedicamento, CancellationToken ct);
}

public interface IVentaRepository
{
    Task<int> InsertVentaAsync(ConfirmSaleRequest request, CancellationToken ct);
    Task InsertVentaDetalleAsync(int idVenta, SaleLineRequest line, CancellationToken ct);
    Task InsertLoteAllocationsAsync(int idVenta, int idMedicamento, IReadOnlyCollection<BoticaPOS.Domain.ValueObjects.LoteAsignacion> allocations, CancellationToken ct);
    Task InsertPagoAsync(int idVenta, BoticaPOS.Domain.Enums.MetodoPago metodoPago, decimal monto, CancellationToken ct);
    Task ReverseVentaAsync(int idVenta, string motivo, int idUsuario, CancellationToken ct);
}

public interface IComprobanteService
{
    Task RetryPendingAsync(CancellationToken ct);
}

public interface IInvoiceSender
{
    Task<bool> SendAsync(int idComprobante, CancellationToken ct);
}

public interface IUnitOfWork : IAsyncDisposable
{
    Task BeginAsync(CancellationToken ct);
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}

public interface IAuditService
{
    Task LogAsync(string accion, string detalle, int? idUsuario, CancellationToken ct);
}
