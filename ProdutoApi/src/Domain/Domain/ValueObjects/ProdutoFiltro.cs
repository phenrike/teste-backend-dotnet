namespace Domain.ValueObjects;

public class ProdutoFiltro
{
    public List<Guid>? Ids { get; set; }
    public string? Nome { get; set; }
    public decimal? Preco { get; set; }
    public string? MoedaOrigem { get; set; }
    public List<string>? MoedasFiltro { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
