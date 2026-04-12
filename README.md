# GPS POI API

API desenvolvida em **.NET 10** para gerenciamento de *Points of Interest (POIs)* com base em coordenadas cartesianas, incluindo busca por proximidade.
Um desafio técnico da empresa LuizaLabs que consiste numa API para um serviço de GPS simplificado.

---

# Tecnologias utilizadas

* .NET (ASP.NET Core Web API)
* Entity Framework Core
* SQLite (produção simples) / SQLite In-Memory (testes)
* xUnit
* FluentAssertions
* Moq (testes unitários)

---

# Como rodar o projeto

## Pré-requisitos

* .NET SDK instalado (versão compatível com o projeto)
* CLI do .NET (`dotnet`)

---

## Clonar e rodar

```
git clone https://github.com/IvanSSantana/desafio-jr-luizalabs.git
cd GpsPoiApp
dotnet restore
dotnet run
```

## Testar

```
cd GpsPoiApp.Tests
dotnet test
```

---

## Banco de dados

O projeto utiliza:

```text
SQLite + Entity Framework Core
```

Ao iniciar a aplicação:

```text
- Migrations são aplicadas automaticamente
- Seed inicial é executado
```

Isso acontece no `Program.cs`.

---

# Estrutura do projeto

```text
GpsPoiApp
│
├── Controllers        → Camada de entrada (HTTP)
├── Services           → Regras de negócio
├── Repository         → Acesso a dados
├── Infrastructure     → DbContext, Migrations, Seed
├── Models             → Entidades de domínio
├── Communication      → Request/Response
│
└── Tests
    ├── Unit
    ├── Integration
    └── E2E
```

---

# Decisões de arquitetura

## Separação em camadas

A aplicação segue uma separação clara:

* **Controller** → orquestra a requisição
* **Service** → aplica regras de negócio
* **Repository** → abstrai acesso ao banco
* **Communication** → isolamento entre domínio e API

---

## Uso de Request/Response 

Mesmo quando semelhantes ao Model:

```
São utilizados para:
- evitar acoplamento com o banco
- controlar contrato da API
- permitir evolução independente
```

---

## Validação

Validações são feitas com:

```text
DataAnnotations + [ApiController]
```

Isso garante:

```text
- validação automática
- retorno 400 (BadRequest) padronizado
```

---

## Error Handling

A API utiliza:

```
Middleware global de exceções
```

Responsável por:

* capturar erros inesperados
* retornar resposta padronizada (RFC 7807-like)
* incluir `traceId` para rastreabilidade

---

## Banco de dados

* Produção: SQLite simples
* Testes: SQLite In-Memory

Motivo:

```text
facilidade de setup pensando no recrutador e isolamento de testes
```

---

# Endpoints principais

## GET /api/point/v1

Retorna todos os pontos

* `200 OK` → com dados
* `204 NoContent` → sem dados
* `500 InternalServerError` → problemas desconhecidos

---

## GET /api/point/v1/proximity

Busca pontos por proximidade

### Query params:

```text
x: int
y: int
maxDistance: double
```

### Respostas:

* `200 OK` → pontos encontrados
* `204 NoContent` → nenhum ponto encontrado
* `400 BadRequest` → parâmetros inválidos
* `500 InternalServerError` → problemas desconhecidos

---

## POST /api/point/v1

Cria um novo ponto

### Body:

```json
{
  "name": "string",
  "x": 10,
  "y": 20
}
```

### Respostas:

* `201 Created`
* `400 BadRequest`
* `500 InternalServerError` → problemas desconhecidos

---

# Testes

## Tipos de teste

### Unitários

* Testam regras de negócio isoladas como cálculos e conversões
* Uso de Moq

---

### Integração

* Testam Repository
* Banco SQLite em memória

---

### E2E

* Testam a API completa (HTTP real)
* Uso de `WebApplicationFactory`

---

## Banco nos testes

* Banco compartilhado para cenários reais
* Banco isolado para testes específicos

Isso garante:

```text
- previsibilidade
- independência entre testes
```

---

# Padrões adotados

* DRY (evitar repetição)
* KISS (simplicidade)
* TDD (testabilidade como prioridade)
* Layered Architecture

---
