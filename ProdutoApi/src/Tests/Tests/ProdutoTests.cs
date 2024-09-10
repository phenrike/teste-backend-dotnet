using Application.Services;
using Domain.Entities;

namespace Tests;

public class ProdutoTests
{
    [Fact]
    public async Task CriarProduto_DeveRetornarProdutoComId()
    {
        // Arrange
        var produtoService = new ProdutoService();
        var produto = new Produto
        {
            Nome = "Produto Teste",
            Preco = 100,
            MoedaOrigem = "BRL"
        };

        // Act
        var resultado = await produtoService.CriarProdutoAsync(produto);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotEqual(Guid.Empty, resultado.Id);
    }
}