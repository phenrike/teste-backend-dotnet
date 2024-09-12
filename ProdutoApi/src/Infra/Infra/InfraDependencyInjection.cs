﻿using Infra.Services;
using Infra.Data;
using Infra.Repositories;
using Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Serilog;

namespace Infra;

public static class InfraDependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProdutoDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("ProdutoConnection")));

        services.AddScoped<IProdutoRepository, ProdutoRepository>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!));
        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddScoped<IApiFixerService>(provider =>
        {
            var httpClient = new HttpClient();
            var apiKey = configuration["ApiAccessKey"];
            var urlTaxas = configuration["EndpointTaxas"];
            var urlMoedas = configuration["EndpointMoedas"];
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ApiFixerService(httpClient, apiKey, urlTaxas, urlMoedas, cacheService);
        });

        Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

        return services;
    }
}