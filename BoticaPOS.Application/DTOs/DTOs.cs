using BoticaPOS.Domain.Enums;
using BoticaPOS.Domain.ValueObjects;

namespace BoticaPOS.Application.DTOs;

public sealed record MedicamentoDto(int IdMedicamento, string NombreComercial, string? CodigoBarras, decimal PrecioVenta, int StockMinimo, bool Estado);
public sealed record SaleLineRequest(int IdMedicamento, int Cantidad, decimal PrecioUnitario);
public sealed record ConfirmSaleRequest(int IdUsuario, TipoComprobante TipoComprobante, MetodoPago MetodoPago, IReadOnlyCollection<SaleLineRequest> Lineas);
public sealed record ConfirmSaleResult(int IdVenta, string Serie, int Correlativo, EstadoComprobante Estado);
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Total, int Page, int PageSize);
public sealed record RetryResult(int IdComprobante, bool Exito, string Mensaje);
public sealed record SaleAllocation(int IdMedicamento, IReadOnlyCollection<LoteAsignacion> Lotes);
