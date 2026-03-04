using BoticaPOS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace BoticaPOS.UI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/boticapos-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddBoticaInfrastructure();
        services.AddSingleton<ViewModels.LoginViewModel>();
        services.AddSingleton<ViewModels.MainViewModel>();
        Services = services.BuildServiceProvider();

        DispatcherUnhandledException += (_, args) =>
        {
            Log.Error(args.Exception, "Unhandled UI exception");
            MessageBox.Show("Ocurrió un error inesperado.");
            args.Handled = true;
        };
    }
}
