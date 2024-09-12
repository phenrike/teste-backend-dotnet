using Domain.Entities;
using Domain.ValueObjects;
using Infra.Data;
using Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ProdutoDbContext _dbContext;

    public ProdutoRepository(ProdutoDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Produto> AdicionarAsync(Produto produto)
    {
        _dbContext.Produtos.Add(produto);
        await _dbContext.SaveChangesAsync();
        return produto;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Produtos.FindAsync(id);
    }

    public async Task<List<Produto>> ObterTodosAsync()
    {
        return await _dbContext.Produtos.ToListAsync();
    }

    public async Task<(List<Produto>, int)> ObterProdutosEContagemComFiltroAsync(ProdutoFiltro filtro)
    {
        var query = _dbContext.Produtos.AsQueryable();

        if (filtro.Ids != null && filtro.Ids.Any())
            query = query.Where(p => filtro.Ids.Contains(p.Id));

        if (!string.IsNullOrEmpty(filtro.Nome))
            query = query.Where(p => p.Nome.ToLower().Contains(filtro.Nome.ToLower()));

        if (filtro.Preco.HasValue)
            query = query.Where(p => p.Preco == filtro.Preco.Value);

        if (!string.IsNullOrEmpty(filtro.MoedaOrigem))
            query = query.Where(p => p.MoedaOrigem == filtro.MoedaOrigem);

        var totalItems = await query.CountAsync();

        var produtos = await query.Skip((filtro.Page - 1) * filtro.PageSize)
                                  .Take(filtro.PageSize)
                                  .ToListAsync();

        return (produtos, totalItems);
    }


    public async Task AtualizarAsync(Produto produto)
    {
        _dbContext.Produtos.Update(produto);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var produto = await _dbContext.Produtos.FindAsync(id);
        if (produto != null)
        {
            _dbContext.Remove(produto);
            await _dbContext.SaveChangesAsync();
        }
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}