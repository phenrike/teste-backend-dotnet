using Application.Dtos;
using Application.Services;
using Application.Validations;
using Domain.Entities;
using Domain.ValueObjects;
using Infra.Repositories;
using Infra.Services;
using Moq;

namespace Tests;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<ICambioService> _cambioServiceMock;
    private readonly Mock<IApiFixerService> _apiFixerServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly ProdutoValidator _produtoValidator;
    private readonly ProdutoFiltroValidator _produtoFiltroValidator;
    private readonly ProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _cambioServiceMock = new Mock<ICambioService>();
        _apiFixerServiceMock = new Mock<IApiFixerService>();
        _cacheServiceMock = new Mock<ICacheService>();

        _apiFixerServiceMock
            .Setup(service => service.ObterMoedasAsync())
            .ReturnsAsync(new Dictionary<string, string>
            {
                { "USD", "United States Dollar" },
                { "BRL", "Brazilian Real" },
                { "EUR", "Euro" }
            });

        _produtoValidator = new ProdutoValidator(_apiFixerServiceMock.Object);
        _produtoFiltroValidator = new ProdutoFiltroValidator(_apiFixerServiceMock.Object);

        _produtoService = new ProdutoService(_produtoRepositoryMock.Object, _cambioServiceMock.Object, _produtoValidator, _produtoFiltroValidator, _cacheServiceMock.Object);
    }

    [Fact]
    public async Task CriarProduto_DeveRetornarProdutoComId()
    {
        // Arrange
        var produto = new Produto { Nome = "Produto Teste", Preco = 100, MoedaOrigem = "BRL" };
        _produtoRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Produto>())).ReturnsAsync((Produto p) => p);

        // Act
        var resultado = await _produtoService.AdicionarAsync(produto);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotEqual(Guid.Empty, resultado.Id);
    }

    [Fact]
    public async Task CriarProduto_ComNomeVazio_DeveLancarExcecao()
    {
        // Arrange
        var produto = new Produto { Nome = "", Preco = 100, MoedaOrigem = "BRL" };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _produtoService.AdicionarAsync(produto));

        // Assert
        Assert.Contains("O nome do produto é obrigatório.", exception.Message);
    }

    [Fact]
    public async Task CriarProduto_ComPrecoNegativo_DeveLancarExcecao()
    {
        // Arrange
        var produto = new Produto { Nome = "Produto Teste", Preco = -10, MoedaOrigem = "USD" };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _produtoService.AdicionarAsync(produto));

        // Assert
        Assert.Contains("O preço do produto deve ser maior que zero", exception.Message);
    }

    [Fact]
    public async Task ObterProdutoPorId_DeveRetornarProdutoSeExistir()
    {
        // Arrange
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto Teste", Preco = 100, MoedaOrigem = "BRL" };
        _produtoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);

        // Act
        var resultado = await _produtoService.ObterPorIdAsync(produto.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(produto.Id, resultado?.Id);
    }

    [Fact]
    public async Task ObterProdutoPorId_DeveRetornarNuloSeNaoExistir()
    {
        // Arrange
        var idInvalido = Guid.NewGuid();
        _produtoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idInvalido)).ReturnsAsync((Produto?)null);

        // Act
        var resultado = await _produtoService.ObterPorIdAsync(idInvalido);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarProduto_DeveAtualizarOsDadosDoProduto()
    {
        // Arrange
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto Teste", Preco = 100, MoedaOrigem = "BRL" };
        _produtoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
        _produtoRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);

        // Act
        produto.Nome = "Produto Atualizado";
        await _produtoService.AtualizarAsync(produto);

        // Assert
        _produtoRepositoryMock.Verify(repo => repo.AtualizarAsync(produto), Times.Once);
    }

    [Fact]
    public async Task AtualizarProduto_DeveLancarExcecaoSeProdutoNaoExistir()
    {
        // Arrange
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto Inexistente", Preco = 100, MoedaOrigem = "BRL" };
        _produtoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(produto.Id)).ReturnsAsync((Produto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _produtoService.AtualizarAsync(produto));
    }

    [Fact]
    public async Task RemoverProduto_DeveRemoverProduto()
    {
        // Arrange
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto Teste", Preco = 100, MoedaOrigem = "BRL" };
        _produtoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
        _produtoRepositoryMock.Setup(repo => repo.RemoverAsync(produto.Id)).Returns(Task.CompletedTask);

        // Act
        await _produtoService.RemoverAsync(produto.Id);

        // Assert
        _produtoRepositoryMock.Verify(repo => repo.RemoverAsync(produto.Id), Times.Once);
    }

    [Fact]
    public async Task RemoverProduto_DeveLancarExcecaoSeProdutoNaoExistir()
    {
        // Arrange
        var idInvalido = Guid.NewGuid();
        _produtoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idInvalido)).ReturnsAsync((Produto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _produtoService.RemoverAsync(idInvalido));
    }

    [Fact]
    public async Task ListarProdutosComConversao_DeveRetornarProdutosComConversoes()
    {
        // Arrange
        var filtro = new ProdutoFiltro { MoedaOrigem = "BRL", MoedasFiltro = new List<string> { "USD", "EUR" } };
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto Teste", Preco = 100, MoedaOrigem = "BRL" };
        var produtos = new List<Produto> { produto };

        _cacheServiceMock.Setup(cache => cache.GetOrCreateCacheListAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<(List<Produto>, int)>>>(),
                It.IsAny<TimeSpan>()
            ))
            .ReturnsAsync((produtos, 1));
        _produtoRepositoryMock.Setup(repo => repo.ObterProdutosEContagemComFiltroAsync(filtro)).ReturnsAsync((produtos, 1));
        _cambioServiceMock.Setup(service => service.ConverterValorProdutoAsync("BRL", 100, filtro.MoedasFiltro))
                          .ReturnsAsync(new List<ConversaoMoedaDto> { new ConversaoMoedaDto { Sigla = "USD", ValorConvertido = 20 } });

        // Act
        var resultado = await _produtoService.ListarProdutosComConversaoAsync(filtro);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado.Items);
        Assert.Equal("USD", resultado.Items.First().Conversoes.First().Sigla);
    }

    [Fact]
    public async Task GerarProdutoComConversoes_DeveRetornarConversoesDeMoeda()
    {
        // Arrange
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto Teste", Preco = 100, MoedaOrigem = "BRL" };
        var moedasFiltro = new List<string> { "USD", "EUR" };
        var conversoes = new List<ConversaoMoedaDto> { new ConversaoMoedaDto { Sigla = "USD", ValorConvertido = 20 } };

        _cambioServiceMock.Setup(service => service.ConverterValorProdutoAsync("BRL", 100, moedasFiltro))
                          .ReturnsAsync(conversoes);

        // Act
        var resultado = await _produtoService.GerarProdutoComConversoesAsync(produto, moedasFiltro);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("USD", resultado.Conversoes.First().Sigla);
    }

    [Fact]
    public async Task ValidarProduto_DevePassarValidacoesCorretamente()
    {
        // Arrange
        var produto = new Produto { Nome = "Produto Teste", Preco = 100, MoedaOrigem = "USD" };
        var cancellationToken = new CancellationToken();

        // Act
        var resultado = await _produtoValidator.ValidateAsync(produto, cancellationToken);

        // Assert
        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task ValidarProdutoFiltro_DevePassarValidacoesCorretamente()
    {
        // Arrange
        var filtro = new ProdutoFiltro { MoedaOrigem = "USD", MoedasFiltro = new List<string> { "BRL", "EUR" } };
        var cancellationToken = new CancellationToken();

        // Act
        var resultado = await _produtoFiltroValidator.ValidateAsync(filtro, cancellationToken);

        // Assert
        Assert.True(resultado.IsValid);
    }
}
