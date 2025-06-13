using System.Windows;

namespace Services;

public interface IDialogService
{
    Task ShowMessage(string message);
    Task ShowErrorMessage(string message);
}

public class DialogService : IDialogService
{
    public Task ShowMessage(string message)
    {
        MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        return Task.CompletedTask;
    }

    public Task ShowErrorMessage(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return Task.CompletedTask;
    }
}