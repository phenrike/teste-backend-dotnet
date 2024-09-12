using Application.Dtos;
using Application.Services;
using Asp.Versioning;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarProduto([FromBody] Produto produto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var novoProduto = await _produtoService.AdicionarAsync(produto);
            return CreatedAtAction(nameof(ObterProdutoPorId), new { id = novoProduto.Id }, novoProduto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterProdutoPorId(Guid id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        return Ok(produto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarProduto(Guid id, [FromBody] Produto produto)
    {
        if (id != produto.Id)
            return BadRequest("ID do produto não coincide.");

        try
        {
            await _produtoService.AtualizarAsync(produto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoverProduto(Guid id)
    {
        try
        {
            await _produtoService.RemoverAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [ApiVersion("1.0")]
    [Obsolete("Esta versão está obsoleta. Use a versão 2.0.")]
    [HttpGet]
    public async Task<IActionResult> ListarProdutosComConversao(
    [FromQuery] List<string>? moedasFiltro = null,
    [FromQuery] List<Guid>? ids = null,
    [FromQuery] string? nome = null,
    [FromQuery] decimal? preco = null,
    [FromQuery] string? moedaOrigem = null)
    {
        try
        {
            var filtro = new ProdutoFiltro
            {
                Ids = ids,
                Nome = nome,
                Preco = preco,
                MoedaOrigem = moedaOrigem,
                MoedasFiltro = moedasFiltro
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