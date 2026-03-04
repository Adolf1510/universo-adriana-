using BoticaPOS.Application.DTOs;
using BoticaPOS.Application.Interfaces;
using BoticaPOS.Domain.Services;
using Serilog;

namespace BoticaPOS.Application.Services;

public sealed class SalesService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FefoAllocator _allocator;
    private readonly IAuditService _audit;

    public SalesService(IVentaRepository ventaRepository, IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork, FefoAllocator allocator, IAuditService audit)
    {
        _ventaRepository = ventaRepository;
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _allocator = allocator;
        _audit = audit;
    }

    public async Task<ConfirmSaleResult> ConfirmSaleAsync(ConfirmSaleRequest request, CancellationToken ct)
    {
        await _unitOfWork.BeginAsync(ct);
        try
        {
            var saleId = await _ventaRepository.InsertVentaAsync(request, ct);
            foreach (var line in request.Lineas)
            {
                var lots = await _inventoryRepository.GetAvailableLotsAsync(line.IdMedicamento, ct);
                var allocations = _allocator.Allocate(lots, line.Cantidad);
                await _ventaRepository.InsertVentaDetalleAsync(saleId, line, ct);
                await _ventaRepository.InsertLoteAllocationsAsync(saleId, line.IdMedicamento, allocations, ct);
            }

            var total = request.Lineas.Sum(x => x.Cantidad * x.PrecioUnitario);
            await _ventaRepository.InsertPagoAsync(saleId, request.MetodoPago, total, ct);
            await _audit.LogAsync("VentaConfirmada", $"Venta {saleId} registrada", request.IdUsuario, ct);
            await _unitOfWork.CommitAsync(ct);
            return new ConfirmSaleResult(saleId, request.TipoComprobante == Domain.Enums.TipoComprobante.Ticket ? "T001" : "B001", saleId, Domain.Enums.EstadoComprobante.PendienteEnvio);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error confirmando venta");
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task VoidSaleAsync(int idVenta, string motivo, int idUsuario, CancellationToken ct)
    {
        await _unitOfWork.BeginAsync(ct);
        try
        {
            await _ventaRepository.ReverseVentaAsync(idVenta, motivo, idUsuario, ct);
            await _audit.LogAsync("VentaAnulada", $"Venta {idVenta} anulada: {motivo}", idUsuario, ct);
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
