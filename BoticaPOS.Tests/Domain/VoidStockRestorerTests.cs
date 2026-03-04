using BoticaPOS.Domain.Entities;
using BoticaPOS.Domain.Services;
using BoticaPOS.Domain.ValueObjects;
using FluentAssertions;

namespace BoticaPOS.Tests.Domain;

public sealed class VoidStockRestorerTests
{
    [Fact]
    public void Void_restores_stock()
    {
        var restorer = new VoidStockRestorer();
        var lotes = new[] { new Lote { IdLote = 10, CantidadDisponible = 1 } };
        var restored = restorer.Restore(lotes, new[] { new LoteAsignacion(10, 4, DateTime.UtcNow) });
        restored.Single().CantidadDisponible.Should().Be(5);
    }
}
