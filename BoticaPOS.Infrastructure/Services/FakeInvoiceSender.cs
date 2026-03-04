using BoticaPOS.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BoticaPOS.Infrastructure.Services;

public sealed class FakeInvoiceSender : IInvoiceSender
{
    private readonly int _failPercent;
    private readonly Random _random = new();

    public FakeInvoiceSender(IConfiguration configuration)
    {
        _failPercent = configuration.GetValue<int?>("InvoiceSender:FailurePercent") ?? 30;
    }

    public Task<bool> SendAsync(int idComprobante, CancellationToken ct)
        => Task.FromResult(_random.Next(1, 101) > _failPercent);
}
