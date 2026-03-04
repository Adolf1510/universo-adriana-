using BoticaPOS.UI.Commands;
using System.Windows;

namespace BoticaPOS.UI.ViewModels;

public sealed class LoginViewModel : BaseViewModel
{
    private string _username = "admin";
    private string _status = string.Empty;
    public string Username { get => _username; set => Set(ref _username, value); }
    public string Status { get => _status; set => Set(ref _status, value); }
    public AsyncRelayCommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(50);
            Status = "Login OK (demo).";
            var shell = new Views.MainWindow { DataContext = App.Services.GetService(typeof(MainViewModel)) };
            shell.Show();
            foreach (Window w in Application.Current.Windows) { if (w is Views.LoginView) { w.Close(); break; } }
        });
    }
}
