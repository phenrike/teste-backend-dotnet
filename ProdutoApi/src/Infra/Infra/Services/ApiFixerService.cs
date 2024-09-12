using System.Text.Json;
using Serilog;

namespace Infra.Services;

public class ApiFixerService : IApiFixerService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _urlTaxas;
    private readonly string _urlMoedas;
    private readonly ICacheService _cacheService;

    public ApiFixerService(HttpClient httpClient, string apiKey, string urlTaxas, string urlMoedas, ICacheService cacheService)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _urlTaxas = urlTaxas;
        _urlMoedas = urlMoedas;
        _cacheService = cacheService;
    }

    public async Task<Dictionary<string, decimal>> ObterCotacoesAsync()
    {
        try
        {
            Log.Information("Solicitando cotações da API Fixer...");

            return await _cacheService.GetOrCreateCacheAsync(
                "fixer_cotacoes",
                async () =>
                {
                    var response = await _httpClient.GetAsync($"{_urlTaxas}{_apiKey}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);

                    if (!jsonDoc.RootElement.GetProperty("success").GetBoolean())
                    {
                        Log.Warning("A API Fixer não retornou sucesso ao solicitar cotações.");
                        throw new Exception("Falha ao obter as cotações da API Fixer");
                    }

                    Log.Information("Cotações obtidas com sucesso.");
                    return jsonDoc.RootElement.GetProperty("rates")
                                      .EnumerateObject()
                                      .ToDictionary(x => x.Name, x => x.Value.GetDecimal());
                },
                TimeSpan.FromHours(1)
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao obter as cotações da API Fixer");
            throw;
        }
    }

    public async Task<Dictionary<string, string>> ObterMoedasAsync()
    {
        try
        {
            Log.Information("Solicitando moedas da API Fixer...");

            return await _cacheService.GetOrCreateCacheAsync(
                "fixer_moedas",
                async () =>
                {
                    var response = await _httpClient.GetAsync($"{_urlMoedas}{_apiKey}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);

                    if (!jsonDoc.RootElement.GetProperty("success").GetBoolean())
                    {
                        Log.Warning("A API Fixer não retornou sucesso ao solicitar moedas.");
                        throw new Exception("Falha ao obter as moedas da API Fixer");
                    }

                    Log.Information("Moedas obtidas com sucesso.");
                    return jsonDoc.RootElement.GetProperty("symbols").EnumerateObject()
                              .ToDictionary(x => x.Name, x => x.Value.GetString()!);
                },
                TimeSpan.FromDays(365)
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao obter as moedas da API Fixer");
            throw;
        }
    }
}