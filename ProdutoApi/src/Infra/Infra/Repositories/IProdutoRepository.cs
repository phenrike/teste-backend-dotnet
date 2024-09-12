namespace Infra.Repositories;

using Domain.Entities;
using Domain.ValueObjects;

public interface IProdutoRepository : IDisposable
{
    Task<Produto> AdicionarAsync(Produto produto);
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task<List<Produto>> ObterTodosAsync();
    Task<(List<Produto>, int)> ObterProdutosEContagemComFiltroAsync(ProdutoFiltro filtro);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(Guid id);
}
