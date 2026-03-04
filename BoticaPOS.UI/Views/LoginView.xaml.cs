using BoticaPOS.UI.ViewModels;
using System.Windows;

namespace BoticaPOS.UI.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = App.Services.GetService(typeof(LoginViewModel));
    }
}
