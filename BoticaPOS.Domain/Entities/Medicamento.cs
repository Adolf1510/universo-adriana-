namespace BoticaPOS.Domain.Entities;

public sealed class Medicamento
{
    public int IdMedicamento { get; set; }
    public string NombreComercial { get; set; } = string.Empty;
    public string? PrincipioActivo { get; set; }
    public string? Laboratorio { get; set; }
    public string? Presentacion { get; set; }
    public string? Concentracion { get; set; }
    public string? CodigoBarras { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal? CostoCompra { get; set; }
    public bool RequiereReceta { get; set; }
    public int StockMinimo { get; set; }
    public bool Estado { get; set; } = true;
}
