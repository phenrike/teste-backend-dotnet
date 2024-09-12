using Domain.Entities;
using FluentValidation;
using Infra.Services;

namespace Application.Validations
{
    public class ProdutoValidator : AbstractValidator<Produto>
    {
        private readonly IApiFixerService _apiFixerService;
        private Task<Dictionary<string, string>>? _moedasCache;

        public ProdutoValidator(IApiFixerService apiFixerService)
        {
            _apiFixerService = apiFixerService;

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.");

            RuleFor(p => p.Preco)
                .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");

            RuleFor(p => p.MoedaOrigem)
                .NotEmpty().WithMessage("A moeda de origem é obrigatória.")
                .Length(3).WithMessage("A moeda de origem deve ser uma sigla de 3 letras.")
                .MustAsync(async (moedaOrigem, cancellation) =>
                {
                    var moedas = await ObterMoedasAsync();
                    return moedas.ContainsKey(moedaOrigem);
                })
                .WithMessage(moedaOrigem => $"A moeda de origem é inválida.");
        }

        private Task<Dictionary<string, string>> ObterMoedasAsync()
        {
            return _moedasCache ??= _apiFixerService.ObterMoedasAsync();
        }
    }
}
