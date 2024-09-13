# Etapa base, usada para rodar a aplicação com o runtime do ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa de build, usada para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências
COPY ["Api/Api/Api.csproj", "Api/Api/"]
COPY ["Application/Application/Application.csproj", "Application/Application/"]
COPY ["Domain/Domain/Domain.csproj", "Domain/Domain/"]
COPY ["Infra/Infra/Infra.csproj", "Infra/Infra/"]
RUN dotnet restore "Api/Api/Api.csproj"

# Copia todo o restante do código para o diretório de build
COPY . .

# Realiza o build da aplicação
WORKDIR "Api/Api"
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicação, usada para gerar o pacote final da aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final, usada para rodar a aplicação no container de produção
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configura o ambiente e as URLs expostas pela aplicação
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV ASPNETCORE_URLS="http://+:80"

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "Api.dll"]
