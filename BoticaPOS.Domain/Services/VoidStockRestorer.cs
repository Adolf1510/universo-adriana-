using BoticaPOS.Domain.Entities;
using BoticaPOS.Domain.ValueObjects;

namespace BoticaPOS.Domain.Services;

public sealed class VoidStockRestorer
{
    public IReadOnlyCollection<Lote> Restore(IEnumerable<Lote> lotes, IEnumerable<LoteAsignacion> asignaciones)
    {
        var map = lotes.ToDictionary(l => l.IdLote);
        foreach (var asignacion in asignaciones)
        {
            if (!map.TryGetValue(asignacion.IdLote, out var lote)) continue;
            lote.CantidadDisponible += asignacion.Cantidad;
        }

        return map.Values.ToList();
    }
}
