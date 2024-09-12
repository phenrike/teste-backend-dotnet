using Application.Commons;
using Application.Dtos;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using Infra.Repositories;
using Infra.Services;

namespace Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICambioService _cambioService;
    private readonly IValidator<Produto> _produtoValidator;
    private readonly IValidator<ProdutoFiltro> _produtoFiltroValidator;
    private readonly ICacheService _cacheService;

    public ProdutoService(IProdutoRepository produtoRepository, ICambioService cambioService, IValidator<Produto> produtoValidator, IValidator<ProdutoFiltro> produtoFiltroValidator, ICacheService cacheService)
    {
        _produtoRepository = produtoRepository;
        _cambioService = cambioService;
        _produtoValidator = produtoValidator;
        _produtoFiltroValidator = produtoFiltroValidator;
        _cacheService = cacheService;
    }

    public async Task<Produto> AdicionarAsync(Produto produto)
    {
        var validationResult = await _produtoValidator.ValidateAsync(produto);
        if (!validationResult.IsValid)
            throw new ArgumentException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        produto.Id = Guid.NewGuid();
        return await _produtoRepository.AdicionarAsync(produto);
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _produtoRepository.ObterPorIdAsync(id);
    }

    public async Task<List<Produto>> ObterTodosAsync()
    {
        return await _produtoRepository.ObterTodosAsync();
    }

    public async Task AtualizarAsync(Produto produto)
    {
        var validationResult = await _produtoValidator.ValidateAsync(produto);
        if (!validationResult.IsValid)
            throw new ArgumentException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var existente = await _produtoRepository.ObterPorIdAsync(produto.Id);
        if (existente == null)
            throw new KeyNotFoundException("Produto não encontrado.");

        existente.Nome = produto.Nome;
        existente.Preco = produto.Preco;
        existente.MoedaOrigem = produto.MoedaOrigem;

        await _produtoRepository.AtualizarAsync(existente);
    }

    public async Task RemoverAsync(Guid id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto == null)
            throw new KeyNotFoundException("Produto não encontrado.");

        await _produtoRepository.RemoverAsync(id);
    }

    public async Task<PaginatedResult<ProdutoResponseDto>> ListarProdutosComConversaoAsync(ProdutoFiltro filtro)
    {
        var validationResult = await _produtoFiltroValidator.ValidateAsync(filtro);
        if (!validationResult.IsValid)
            throw new ArgumentException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var cacheKey = $"produtos_filtro:{GerarCacheKey(filtro)}";

        var (produtos, totalItems) = await _cacheService.GetOrCreateCacheListAsync(
            cacheKey,
            async () => await _produtoRepository.ObterProdutosEContagemComFiltroAsync(filtro),
            TimeSpan.FromMinutes(2)
        );

        var produtosComValorConvertido = new List<ProdutoResponseDto>();

        foreach (var produto in produtos)
        {
            ProdutoResponseDto produtoResponseDto = await GerarProdutoComConversoesAsync(produto, filtro.MoedasFiltro);
            produtosComValorConvertido.Add(produtoResponseDto);
        }

        return new PaginatedResult<ProdutoResponseDto>
        {
            Items = produtosComValorConvertido,
            TotalItems = totalItems,
            Page = filtro.Page,
            PageSize = filtro.PageSize
        };
    }

    public async Task<ProdutoResponseDto> GerarProdutoComConversoesAsync(Produto produto, List<string>? moedasFiltro)
    {
        var conversoes = new List<ConversaoMoedaDto>();

        if (moedasFiltro is not null && moedasFiltro.Count() > 0)
        {
            conversoes = await _cambioService.ConverterValorProdutoAsync(
                produto.MoedaOrigem, produto.Preco, moedasFiltro);
        }

        return new ProdutoResponseDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            MoedaOrigem = produto.MoedaOrigem,
            Conversoes = conversoes
        };
    }

    private string GerarCacheKey(ProdutoFiltro filtro)
    {
        var filtroString = $"{filtro.Ids?.Count ?? 0}-{filtro.Nome ?? ""}-{filtro.Preco ?? 0}-{filtro.MoedaOrigem ?? ""}-{filtro.Page}-{filtro.PageSize}";
        return $"produtos_filtro:{filtroString}";
    }

    public void Dispose()
    {
        _produtoRepository.Dispose();
    }
}