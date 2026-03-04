using BoticaPOS.Domain.Entities;
using BoticaPOS.Domain.ValueObjects;

namespace BoticaPOS.Domain.Services;

public sealed class FefoAllocator
{
    public IReadOnlyCollection<LoteAsignacion> Allocate(IEnumerable<Lote> lotes, int cantidadSolicitada)
    {
        if (cantidadSolicitada <= 0) throw new ArgumentException("Cantidad inválida");
        var sorted = lotes.Where(x => x.CantidadDisponible > 0)
            .OrderBy(x => x.FechaVencimiento)
            .ThenBy(x => x.FechaIngreso)
            .ToList();

        var total = sorted.Sum(s => s.CantidadDisponible);
        if (total < cantidadSolicitada) throw new InvalidOperationException("Stock insuficiente");

        var rest = cantidadSolicitada;
        var allocations = new List<LoteAsignacion>();
        foreach (var lote in sorted)
        {
            if (rest == 0) break;
            var use = Math.Min(rest, lote.CantidadDisponible);
            allocations.Add(new LoteAsignacion(lote.IdLote, use, lote.FechaVencimiento));
            rest -= use;
        }

        return allocations;
    }
}
