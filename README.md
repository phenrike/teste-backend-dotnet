# teste-backend-dotnet Desafio Técnico: Desenvolvimento de API RESTful com ASP.NET 8

## ProdutoApi
### Descrição
O ProdutoApi é uma API RESTful desenvolvida com ASP.NET Core que segue a arquitetura Clean Architecture. O objetivo é fornecer uma interface para gerenciar produtos, incluindo operações de CRUD e conversão de preços entre moedas.

## Arquitetura

- Camada Api(Presentation)
    - Responsabilidade: Configuração da API e implementação dos controladores.
    - Pacotes:
        - Asp.Versioning.Mvc: Para versionamento da API.
        - Swashbuckle.AspNetCore: Para documentação da API com Swagger.

- Camada Application
    - Responsabilidade: Lógica de aplicação, serviços e validação de dados.
    - Pacotes:
        - FluentValidation: Para validação de modelos e dados.

- Camada Domain
    - Responsabilidade: Entidades.

- Camada Infra(Infrastructure)
    - Responsabilidade: Implementação de acesso a dados, cache, logging e integração com a API [Fixer.io](https://fixer.io/).
    - Pacotes:
        - Microsoft.EntityFrameworkCore.Design: Ferramentas de design do Entity Framework Core.
        - Microsoft.EntityFrameworkCore.Tools: Ferramentas do Entity Framework Core.
        - Microsoft.Extensions.Caching.StackExchangeRedis: Para cache com Redis.
        - Npgsql.EntityFrameworkCore.PostgreSQL: Provedor de banco de dados PostgreSQL.
        - Serilog.AspNetCore: Para logging com Serilog.
        - StackExchange.Redis: Cliente Redis.

- Tests
    - Responsabilidade: Testes unitários e de integração.
    - Pacotes:
        - Moq: Para criação de mocks.
        - xunit: Framework de teste.
        - xunit.runner.visualstudio: Suporte para execução de testes no Visual Studio.

## Decisões Técnicas
- Cache: Redis para caching das consultas de Moedas e Cotações/Taxas da API Fixer.io para economizar requisições, utilizando Microsoft.Extensions.Caching.StackExchangeRedis.
- Cache: Redis para caching da consulta de Produto com filtros e paginação para economizar consultas no banco de dados, utilizando Microsoft.Extensions.Caching.StackExchangeRedis.
- Validação: FluentValidation para validação de dados na camada Application.

## Instruções de Execução


## Endpoints

- Listar produtos com Conversão de Moeda, Filtros(moedasFiltro, ids, nome, preco, moedaOrigem, page(Paginação), pageSize(Paginação)):
GET /api/v2/produtos
Descrição: Obtém uma lista de todos os produtos.
Resposta: 200 OK com a lista de produtos.

- Obter produto por ID:
GET /api/v1/produtos/{id}
Descrição: Obtém um produto específico pelo seu ID.
Parâmetros: id (GUID do produto)
Resposta: 200 OK com o produto encontrado ou 404 Not Found se não encontrado.

- Criar novo produto:
POST /api/v1/produtos
Descrição: Cria um novo produto.
Corpo: Dados do produto no formato JSON.
Resposta: 201 Created com o produto criado.

- Atualizar produto:
PUT /api/v1/produtos/{id}
Descrição: Atualiza um produto existente.
Parâmetros: id (GUID do produto)
Corpo: Dados atualizados do produto no formato JSON.
Resposta: 204 No Content se bem-sucedido.

- Excluir produto:
DELETE /api/v1/produtos/{id}
Descrição: Exclui um produto específico pelo seu ID.
Parâmetros: id (GUID do produto)
Resposta: 204 No Content se bem-sucedido.