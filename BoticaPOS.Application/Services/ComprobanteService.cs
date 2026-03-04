using BoticaPOS.Application.Interfaces;
using Polly;

namespace BoticaPOS.Application.Services;

public sealed class ComprobanteService : IComprobanteService
{
    private readonly IInvoiceSender _sender;
    private readonly IAuditService _audit;

    public ComprobanteService(IInvoiceSender sender, IAuditService audit)
    {
        _sender = sender;
        _audit = audit;
    }

    public async Task RetryPendingAsync(CancellationToken ct)
    {
        var policy = Policy.HandleResult<bool>(r => !r)
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(i));

        var result = await policy.ExecuteAsync(token => _sender.SendAsync(0, token), ct);
        await _audit.LogAsync("ReintentoEnvio", result ? "Envío OK" : "Envío falló", null, ct);
    }
}
