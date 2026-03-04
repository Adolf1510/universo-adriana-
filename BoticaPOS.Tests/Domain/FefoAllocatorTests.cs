using BoticaPOS.Domain.Entities;
using BoticaPOS.Domain.Services;
using FluentAssertions;

namespace BoticaPOS.Tests.Domain;

public sealed class FefoAllocatorTests
{
    [Fact]
    public void Consumes_earliest_expiration_first()
    {
        var allocator = new FefoAllocator();
        var lots = new[]
        {
            new Lote{ IdLote=1, CantidadDisponible=5, FechaVencimiento=new DateTime(2026,1,1), FechaIngreso=DateTime.UtcNow},
            new Lote{ IdLote=2, CantidadDisponible=5, FechaVencimiento=new DateTime(2025,1,1), FechaIngreso=DateTime.UtcNow},
        };
        var result = allocator.Allocate(lots, 4);
        result.First().IdLote.Should().Be(2);
    }

    [Fact]
    public void Splits_across_multiple_lots()
    {
        var allocator = new FefoAllocator();
        var lots = new[]
        {
            new Lote{ IdLote=1, CantidadDisponible=2, FechaVencimiento=new DateTime(2025,1,1), FechaIngreso=DateTime.UtcNow},
            new Lote{ IdLote=2, CantidadDisponible=4, FechaVencimiento=new DateTime(2025,2,1), FechaIngreso=DateTime.UtcNow},
        };
        var result = allocator.Allocate(lots, 5);
        result.Should().HaveCount(2);
        result.First(x => x.IdLote == 1).Cantidad.Should().Be(2);
        result.First(x => x.IdLote == 2).Cantidad.Should().Be(3);
    }

    [Fact]
    public void Throws_when_stock_is_insufficient()
    {
        var allocator = new FefoAllocator();
        var lots = new[] { new Lote { IdLote = 1, CantidadDisponible = 1, FechaVencimiento = DateTime.UtcNow, FechaIngreso = DateTime.UtcNow } };
        var action = () => allocator.Allocate(lots, 2);
        action.Should().Throw<InvalidOperationException>();
    }
}
