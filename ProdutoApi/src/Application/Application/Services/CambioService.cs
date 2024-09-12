using Application.Dtos;
using Infra.Services;

namespace Application.Services;

public class CambioService : ICambioService
{
    private readonly IApiFixerService _apiFixerService;

    public CambioService(IApiFixerService apiFixerClient)
    {
        _apiFixerService = apiFixerClient;
    }

    public async Task<List<ConversaoMoedaDto>> ConverterValorProdutoAsync(string moedaOrigem, decimal valor, List<string> moedasFiltro)
    {
        var cotacoes = await _apiFixerService.ObterCotacoesAsync();
        var moedas = await _apiFixerService.ObterMoedasAsync();

        var listaConversaoMoedas = new List<ConversaoMoedaDto>();

        foreach (var moeda in moedasFiltro)
        {
            if (cotacoes.ContainsKey(moeda) && moedas.ContainsKey(moeda))
            {
                var valorConvertido = ConverterDeEuroParaMoedaDestino(valor, cotacoes[moedaOrigem], cotacoes[moeda]);
                listaConversaoMoedas.Add(new ConversaoMoedaDto
                {
                    Sigla = moeda,
                    Descricao = moedas[moeda],
                    ValorConvertido = valorConvertido
                });
            }
        }

        return listaConversaoMoedas;
    }

    public decimal ConverterDeEuroParaMoedaDestino(decimal valorOrigem, decimal taxaOrigemEmEuro, decimal taxaDestinoEmEuro)
    {
        decimal valorOrigemEmEuro = valorOrigem / taxaOrigemEmEuro;

        decimal valorDestino = valorOrigemEmEuro * taxaDestinoEmEuro;

        return valorDestino;
    }
}