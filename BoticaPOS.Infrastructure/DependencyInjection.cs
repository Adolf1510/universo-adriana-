using BoticaPOS.Application.Interfaces;
using BoticaPOS.Application.Services;
using BoticaPOS.Domain.Services;
using BoticaPOS.Infrastructure.Caching;
using BoticaPOS.Infrastructure.Data;
using BoticaPOS.Infrastructure.Repositories;
using BoticaPOS.Infrastructure.Services;
using BoticaPOS.Infrastructure.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace BoticaPOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBoticaInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<SqlConnectionFactory>();
        services.AddScoped<SqlUnitOfWork>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqlUnitOfWork>());
        services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IInvoiceSender, FakeInvoiceSender>();
        services.AddScoped<MedicamentoCacheService>();
        services.AddScoped<SalesService>();
        services.AddScoped<IComprobanteService, ComprobanteService>();
        services.AddSingleton<FefoAllocator>();
        return services;
    }
}
