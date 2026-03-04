using BoticaPOS.Domain.Services;
using FluentAssertions;

namespace BoticaPOS.Tests.Domain;

public sealed class DocumentNumberServiceTests
{
    [Fact]
    public void Document_number_increments_uniquely()
    {
        var service = new DocumentNumberService();
        var current = 30;
        var next = service.Next(current);
        next.Should().Be(31);
    }
}
