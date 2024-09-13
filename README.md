# Desafio Técnico: Desenvolvimento de API RESTful com ASP.NET 8

## Descrição
O ProdutoApi é uma API RESTful desenvolvida com ASP.NET Core que segue o conceito Clean Architecture. O objetivo é fornecer uma interface para gerenciar produtos, incluindo operações de CRUD e conversão de preços entre moedas.

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
Acesse o diretório pelo terminal e execute o comando abaixo:
```bash
docker-compose -f docker-compose-development.yml up --build
```

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

## Status do Desafio Técnico 
- [x] CRUD de Produtos: Implementar as operações de criação, leitura, atualização e deleção de produtos no banco de dados PostgreSQL.
- [x] Integração com API Externa: Consumir a API Fixer.io para obter taxas de câmbio e realizar a conversão de preços de produtos para diferentes moedas.
- [x] Listagem de Produtos com Conversão de Moeda: Deve ser possível listar os produtos com seus preços convertidos para uma ou mais moedas com base nas taxas da API Fixer.
- [x] Paginação e Filtros: Implementar paginação e filtros nos endpoints de listagem de produtos.
- [x] Arquitetura Limpa (Clean Architecture): O projeto deve seguir os princípios da Clean Architecture, separando camadas de domínio, aplicação, infraestrutura e interfaces de forma clara.
- [x] Design Orientado a Domínio (DDD): O domínio deve ser modelado seguindo os conceitos de DDD, com agregados, entidades e repositórios devidamente definidos.
- [x] Boas Práticas de SOLID: O código deve respeitar os princípios SOLID, com separação de responsabilidades, uso adequado de injeção de dependências e padrões de design apropriados.
- [x] Testes Unitários (TDD): A aplicação deve ser construída com a abordagem de TDD, utilizando xUnit para cobrir as funcionalidades principais.
- [ ] Testes de Integração (TDD): A aplicação deve ser construída com a abordagem de TDD, utilizando xUnit para cobrir a integração com a API externa.
- [ ] Segurança: A API deve seguir boas práticas de segurança, como autenticação/autorização (OAuth2 ou JWT).
- [x] Boas Práticas REST: A API deve seguir boas práticas como versionamento de API e tratamento adequado de erros.
- [ ] Monitoramento e Observabilidade: Implementar logging, monitoramento e rastreamento (Tracing) usando ferramentas como Serilog e OpenTelemetry.
- [x] Containerização: Deve ser criado um Dockerfile e docker-compose para facilitar a configuração e execução da aplicação, com suporte a múltiplos ambientes (dev, staging, produção).
- [x] CI/CD: Implementar um pipeline de CI/CD usando GitHub Actions ou outra ferramenta, com build, testes e deploy automatizado no provedor de nuvem de sua escolha (AWS, Azure, Google Cloud, etc.).
- [ ] Publicação em Cloud: A API deve ser publicada em um ambiente de produção no provedor de nuvem de sua escolha.