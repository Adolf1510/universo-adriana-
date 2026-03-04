using BoticaPOS.Application.Interfaces;
using BoticaPOS.Infrastructure.Data;
using Dapper;

namespace BoticaPOS.Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private readonly SqlConnectionFactory _factory;
    public AuditService(SqlConnectionFactory factory) => _factory = factory;

    public async Task LogAsync(string accion, string detalle, int? idUsuario, CancellationToken ct)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(new CommandDefinition("INSERT INTO AuditLog(Fecha,Accion,Detalle,IdUsuario) VALUES(SYSUTCDATETIME(),@accion,@detalle,@idUsuario)", new { accion, detalle, idUsuario }, cancellationToken: ct));
    }
}
