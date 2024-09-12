using Application.Dtos;
using Application.Services;
using Asp.Versioning;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Api.V2.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<IActionResult> ListarProdutosComConversao(
    [FromQuery] List<string>? moedasFiltro = null,
    [FromQuery] List<Guid>? ids = null,
    [FromQuery] string? nome = null,
    [FromQuery] decimal? preco = null,
    [FromQuery] string? moedaOrigem = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            var filtro = new ProdutoFiltro
            {
                Ids = ids,
                Nome = nome,
                Preco = preco,
                MoedaOrigem = moedaOrigem,
                MoedasFiltro = moedasFiltro,
                Page = page,
                PageSize = pageSize
            };

            var produtosComValorConvertido = await _produtoService.ListarProdutosComConversaoAsync(filtro);
            return Ok(produtosComValorConvertido);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}