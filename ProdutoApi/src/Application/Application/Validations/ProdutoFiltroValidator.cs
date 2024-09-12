using Domain.ValueObjects;
using FluentValidation;
using Infra.Services;

namespace Application.Validations;

public class ProdutoFiltroValidator : AbstractValidator<ProdutoFiltro>
{
    private readonly IApiFixerService _apiFixerService;
    private Task<Dictionary<string, string>>? _moedasCache;

    public ProdutoFiltroValidator(IApiFixerService apiFixerService)
    {
        _apiFixerService = apiFixerService;

        RuleFor(pf => pf.MoedaOrigem)
            .MustAsync(async (moedaOrigem, cancellation) =>
            {
                if (string.IsNullOrEmpty(moedaOrigem))
                    return true;

                var moedas = await ObterMoedasAsync();
                return moedas.ContainsKey(moedaOrigem);
            })
            .WithMessage(moedaOrigem => $"A moeda de origem é inválida.");

        RuleFor(pf => pf.MoedasFiltro)
            .MustAsync(async (moedasFiltro, cancellation) =>
            {
                if (moedasFiltro == null || !moedasFiltro.Any())
                    return true;

                var moedas = await ObterMoedasAsync();
                return moedasFiltro.All(moeda => moedas.ContainsKey(moeda));
            })
            .WithMessage("Uma ou mais moedas do filtro são inválidas.");
    }

    private Task<Dictionary<string, string>> ObterMoedasAsync()
    {
        return _moedasCache ??= _apiFixerService.ObterMoedasAsync();
    }
}