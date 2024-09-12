using Application.Dtos;

namespace Application.Services;

public interface ICambioService
{
    Task<List<ConversaoMoedaDto>> ConverterValorProdutoAsync(string moedaOrigem, decimal valor, List<string> moedasFiltro);
    decimal ConverterDeEuroParaMoedaDestino(decimal valorOrigem, decimal taxaOrigemEmEuro, decimal taxaDestinoEmEuro);
}
