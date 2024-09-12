using Application.Services;
using Application.Validations;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using Infra.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddAplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!));
        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddTransient<IValidator<Produto>, ProdutoValidator>();
        services.AddTransient<IValidator<ProdutoFiltro>, ProdutoFiltroValidator>();
        services.AddScoped<ICambioService, CambioService>();
        services.AddScoped<IProdutoService, ProdutoService>();

        return services;
    }
}