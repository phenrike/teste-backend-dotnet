using Domain.Entities;

namespace Application.Dtos;

public class ProdutoResponseDto : Produto
{
    public List<ConversaoMoedaDto> Conversoes { get; set; }
}