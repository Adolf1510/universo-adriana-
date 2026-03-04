using BoticaPOS.Application.Interfaces;
using BoticaPOS.Infrastructure.Data;
using System.Data;

namespace BoticaPOS.Infrastructure.Transactions;

public sealed class SqlUnitOfWork : IUnitOfWork
{
    private readonly SqlConnectionFactory _factory;
    public IDbConnection? Connection { get; private set; }
    public IDbTransaction? Transaction { get; private set; }

    public SqlUnitOfWork(SqlConnectionFactory factory) => _factory = factory;

    public async Task BeginAsync(CancellationToken ct)
    {
        Connection = _factory.Create();
        if (Connection is Microsoft.Data.SqlClient.SqlConnection sql)
        {
            await sql.OpenAsync(ct);
            Transaction = await sql.BeginTransactionAsync(ct);
        }
    }

    public Task CommitAsync(CancellationToken ct)
    {
        Transaction?.Commit();
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken ct)
    {
        Transaction?.Rollback();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Transaction?.Dispose();
        Connection?.Dispose();
        return ValueTask.CompletedTask;
    }
}
