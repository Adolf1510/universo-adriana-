namespace BoticaPOS.Domain.Entities;

public sealed class Lote
{
    public int IdLote { get; set; }
    public int IdMedicamento { get; set; }
    public string NumeroLote { get; set; } = string.Empty;
    public DateTime FechaVencimiento { get; set; }
    public int CantidadDisponible { get; set; }
    public DateTime FechaIngreso { get; set; }
}
