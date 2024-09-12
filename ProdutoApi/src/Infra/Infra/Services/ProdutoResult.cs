using Domain.Entities;

namespace Infra.Services;

public class ProdutoResult
{
    public List<Produto> Items { get; set; }
    public int TotalItems { get; set; }
}