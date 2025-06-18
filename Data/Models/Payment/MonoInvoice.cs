namespace Data.Models.Payment;

public class MonoInvoiceRequest
{
    public decimal Amount { get; set; }
    public int Ccy { get; set; } = 980; // UAH
    public MerchantPaymInfo MerchantPaymInfo { get; set; }
    public string RedirectUrl { get; set; }
    public string WebHookUrl { get; set; }
    public int Validity { get; set; } = 3600;
    public string QrId { get; set; }
    public string Code { get; set; }
    public SaveCardData SaveCardData { get; set; }
    public double AgentFeePercent { get; set; }
}

public class MerchantPaymInfo
{
    public string Reference { get; set; }
    public string Destination { get; set; }
    public string Comment { get; set; }
    public List<string> CustomerEmails { get; set; }
    public List<Discount> Discounts { get; set; }
    public List<BasketOrder> BasketOrder { get; set; }
}

public class BasketOrder
{
    public string Name { get; set; }
    public int Qty { get; set; }
    public decimal Sum { get; set; }
    public decimal Total { get; set; }
    public string Icon { get; set; }
    public string Unit { get; set; }
    public string Code { get; set; }
    public string Barcode { get; set; }
    public string Header { get; set; }
    public string Footer { get; set; }
    public List<string> Tax { get; set; }
    public string Uktzed { get; set; }
    public string SplitReceiverId { get; set; }
    public List<Discount> Discounts { get; set; }
}

public class Discount
{
    public string Type { get; set; }
    public string Mode { get; set; }
    public decimal? Value { get; set; }
}

public class SaveCardData
{
    public bool? SaveCard { get; set; }
    public string WalletId { get; set; }
}