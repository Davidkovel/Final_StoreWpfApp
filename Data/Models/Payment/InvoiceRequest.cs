using Newtonsoft.Json;

namespace Data.Models.Payment;

public class InvoiceRequest
{
    [JsonProperty("amount")]
    public long Amount { get; set; }
    
    [JsonProperty("ccy")]
    public int Currency { get; set; } = 980; // UAH
    
    [JsonProperty("redirectUrl")]
    public string RedirectUrl { get; set; }
    
    [JsonProperty("webHookUrl")]
    public string WebHookUrl { get; set; }
    
    [JsonProperty("validity")]
    public long Validity { get; set; } = 86400; // 24 hours in seconds
    
    [JsonProperty("paymentType")]
    public string PaymentType { get; set; } = "debit";
}