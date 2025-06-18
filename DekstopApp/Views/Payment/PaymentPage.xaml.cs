using System.Windows;
using System.Windows.Controls;
using DekstopApp.ViewModels.Payment;

namespace DekstopApp.Views.Payment;

public partial class PaymentWindow : Window
{
    public PaymentWindow(PaymentViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    // private void MonobankPay_Click(object sender, RoutedEventArgs e)
    // {
    //     // Здесь логика для оплаты через Monobank
    //     MessageBox.Show("Перенаправление на оплату через Monobank...");
    //
    //     // Пример URL для оплаты (замените на реальный)
    //     // System.Diagnostics.Process.Start("https://example.com/monobank-payment");
    //
    //     this.Close();
    // }
    //
    // private void CryptoPay_Click(object sender, RoutedEventArgs e)
    // {
    //     // Здесь логика для оплаты через Confirmo
    //     MessageBox.Show("Перенаправление на оплату через Confirmo...");
    //
    //     // Пример URL для оплаты (замените на реальный)
    //     // System.Diagnostics.Process.Start("https://confirmo.net/your-payment-link");
    //
    //     this.Close();
    // }
}