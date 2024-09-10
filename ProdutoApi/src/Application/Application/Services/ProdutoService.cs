using Domain.Entities;

namespace Application.Services;

public class ProdutoService
{
    public async Task<Produto> CriarProdutoAsync(Produto produto)
    {
        produto.Id = Guid.NewGuid();
        return await Task.FromResult(produto);
    }
}