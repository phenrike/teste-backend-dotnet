namespace ProdutoApi.Application.Services;

public class ProdutoService
{
    public async Task<Produto> CriarProdutoAsync(Produto produto)
    {
        produto.Id = Guid.NewGuid(); // Simples geração de ID
        return await Task.FromResult(produto);  // Simulação de operação assíncrona
    }
}