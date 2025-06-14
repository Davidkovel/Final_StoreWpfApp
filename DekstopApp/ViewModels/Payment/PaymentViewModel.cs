using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using Data.Models.Payment;
using DekstopApp.Services;
using Services;
using Services.Features.Payment;

namespace DekstopApp.ViewModels.Payment;

public partial class PaymentViewModel : ObservableObject
{
    private readonly CartService _cartService;
    private readonly AuthService _authService;
    private readonly MonobankService _monobankService;
    private readonly DialogService _dialogService;

    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private string _paymentUrl;
    [ObservableProperty] private decimal _totalAmount;
    [ObservableProperty] private ObservableCollection<Cart> _cartItems = new();

    private readonly string _successRedirectUrl = "https://example.com/success";

    public PaymentViewModel(CartService cartService, AuthService authService, MonobankService monobankService,
        DialogService dialogService)
    {
        _cartService = cartService;
        _authService = authService;
        _monobankService = monobankService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    public async Task MonobankPayment()
    {
        try
        {
            IsProcessing = true;
            var response = await MonobankPaymentAsync();

            if (response != null)
            {
                PaymentUrl = response.PageUrl;
                Process.Start(new ProcessStartInfo(PaymentUrl) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowErrorMessage($"Ошибка оплаты: {ex.Message}");
            Debug.WriteLine(ex);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task<InvoiceResponse?> MonobankPaymentAsync()
    {
        var currentUser = await _authService.GetCurrentUser();
        var userId = currentUser.Id;

        var cartItems = await _cartService.LoadCartItemsByUserId(userId);

        if (cartItems == null || !cartItems.Any())
        {
            _dialogService.ShowMessage("Cart is empty");
            return null;
        }

        var totalAmount = cartItems.Sum(item => item.ProductPrice * item.Quantity);

        var basketOrders = cartItems.Select(item => new BasketOrder
        {
            Name = item.ProductName,
            Qty = item.Quantity,
            Sum = item.ProductPrice,
            Total = item.ProductPrice * item.Quantity,
            Unit = "шт.",
            Code = item.ProductId.ToString(),
            Discounts = new List<Discount> { new Discount() },
            SplitReceiverId = "a1b2c3d4e5f6",
            Tax = new List<string>()
        }).ToList();

        var jsonRequest = new MonoInvoiceRequest
        {
            Amount = totalAmount,
            MerchantPaymInfo = new MerchantPaymInfo
            {
                Reference = Guid.NewGuid().ToString("N"),
                Destination = "Покупка товаров",
                Comment = "Спасибо за покупку!",
                CustomerEmails = new List<string>(),
                Discounts = new List<Discount> { new Discount() },
                BasketOrder = basketOrders
            },
            RedirectUrl = "https://example.com/result",
            WebHookUrl = "https://example.com/webhook",
            QrId = null,
            Code = null,
            SaveCardData = new SaveCardData
            {
                SaveCard = true,
                WalletId = null
            },
            AgentFeePercent = 1.42
        };

        InvoiceResponse response = await _monobankService.CreateInvoiceAsync(jsonRequest);
        return response;
    }
}