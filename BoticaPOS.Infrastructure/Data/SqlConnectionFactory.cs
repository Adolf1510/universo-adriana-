using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BoticaPOS.Infrastructure.Data;

public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("BoticaPOS")
            ?? "Server=.\\SQLEXPRESS;Database=BoticaPOS;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    public IDbConnection Create() => new SqlConnection(_connectionString);
}
