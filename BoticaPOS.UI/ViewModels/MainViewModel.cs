namespace BoticaPOS.UI.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    private bool _isBusy;
    private string _status = "Conectado";
    public bool IsBusy { get => _isBusy; set => Set(ref _isBusy, value); }
    public string Status { get => _status; set => Set(ref _status, value); }
}
