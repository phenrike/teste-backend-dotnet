namespace Infra.Services;

public interface IApiFixerService
{
    Task<Dictionary<string, decimal>> ObterCotacoesAsync();
    Task<Dictionary<string, string>> ObterMoedasAsync();
}