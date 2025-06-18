using System.Text;
using Data.Models.Payment;
using Newtonsoft.Json;

namespace Services.Features.Payment;

public class MonobankService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiToken;

    public MonobankService()
    {
        _apiToken = "API_TOKEN_MONOBNAK";
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.monobank.ua/")
        };
    }

    public async Task<InvoiceResponse?> CreateInvoiceAsync(MonoInvoiceRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Token", _apiToken);

        var response = await _httpClient.PostAsync("/api/merchant/invoice/create", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();

        var invoiceResponse = JsonConvert.DeserializeObject<InvoiceResponse>(responseJson);
        return invoiceResponse;
    }
}