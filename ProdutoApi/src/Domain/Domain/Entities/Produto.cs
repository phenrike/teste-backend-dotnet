namespace Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public string MoedaOrigem { get; set; }
}