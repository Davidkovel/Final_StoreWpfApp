using Newtonsoft.Json;

namespace Data.Models.Payment;

public class InvoiceResponse
{
    [JsonProperty("invoiceId")]
    public string InvoiceId { get; set; }
    
    [JsonProperty("pageUrl")]
    public string PageUrl { get; set; }
}