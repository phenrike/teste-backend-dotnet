using Application.Commons;
using Application.Dtos;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services;

public interface IProdutoService : IDisposable
{
    Task<Produto> AdicionarAsync(Produto produto);
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task<List<Produto>> ObterTodosAsync();
    Task<PaginatedResult<ProdutoResponseDto>> ListarProdutosComConversaoAsync(ProdutoFiltro filtro);
    Task<ProdutoResponseDto> GerarProdutoComConversoesAsync(Produto produto, List<string> moedasFiltro);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(Guid id);
}