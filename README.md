# Prisma

[![CI](https://github.com/lukasnunesj/prisma/actions/workflows/ci.yml/badge.svg)](https://github.com/lukasnunesj/prisma/actions/workflows/ci.yml)

Dashboard de finanças pessoais com API em **.NET 8 / ASP.NET Core** e interface em **React + TypeScript**.
Registra receitas, despesas fixas e variáveis, investimentos e formas de pagamento por mês, e resume o mês
(totais, uso de débito/crédito, saldo restante e comparação com o mês anterior).

> Projeto pessoal, em evolução. O foco foi o backend: autenticação, isolamento de dados por usuário e testes de integração.
> A base da interface foi gerada com Lovable + shadcn/ui.

## Stack

| Camada | Tecnologias |
| :--- | :--- |
| API | .NET 8, ASP.NET Core, EF Core 8, ASP.NET Identity, JWT, Swagger |
| Banco | PostgreSQL (Npgsql), migrations do EF Core |
| Frontend | React 18, TypeScript, Vite, Tailwind, shadcn/ui, React Query, Zustand, Recharts |
| Testes | xUnit, FluentAssertions, `WebApplicationFactory`, EF Core InMemory |
| CI | GitHub Actions (testes do backend, lint e build do frontend) |

## Arquitetura

Camadas organizadas por pasta, em um único projeto:

```text
backend/
├── Api/             controllers finos, ICurrentUser (usuário logado), extensões
├── Application/     serviços, DTOs com validação, validação do ID token do Google
├── Domain/          entidades e enums (User, Category, Transaction, Investment, PaymentMethod, RefreshToken)
├── Infrastructure/  DbContext, configurações do EF Core, repositórios
├── Migrations/
└── Tests/           testes de integração
frontend/            SPA em React
```

- Controllers só recebem a requisição, chamam um serviço e devolvem o status HTTP.
- `ICurrentUser` centraliza a leitura do id do usuário autenticado; os dados são sempre consultados por usuário.
- Datas de lançamento usam `DateOnly`; listagens são filtradas por mês e ano na rota.

## Autenticação

- Cadastro e login com ASP.NET Identity (senha: mínimo de 6 caracteres, com maiúscula, minúscula, dígito e símbolo).
- **Access token** JWT de 30 minutos, enviado em `Authorization: Bearer`.
- **Refresh token** aleatório (64 bytes), guardado no banco apenas como hash e entregue em cookie `HttpOnly`
  (`Secure` + `SameSite=None` em HTTPS, `SameSite=Lax` em HTTP). Validade de 7 dias.
- **Rotação**: cada `/refresh` revoga o token anterior e emite um novo. Se um token já revogado for reapresentado
  (possível replay), todos os refresh tokens do usuário são revogados.
- **CSRF**: cookie `XSRF-TOKEN` + header `X-XSRF-TOKEN` (double submit) exigidos em `/refresh` e `/logout`.
- **Login com Google**: o backend valida o ID token (`Google.Apis.Auth`) e cria ou reaproveita o usuário.
- A API recusa subir fora de `Development` com a chave JWT de exemplo do repositório.

## Endpoints

Com a API rodando em desenvolvimento, a documentação interativa fica em `/swagger`.

| Recurso | Rotas |
| :--- | :--- |
| Auth | `POST /api/auth/register`, `/login`, `/google`, `/refresh`, `/logout`, `/change-password` · `GET /api/auth/csrf` |
| Categorias | `GET`, `POST /api/category` · `GET`, `PUT`, `DELETE /api/category/{id}` |
| Formas de pagamento | `GET`, `POST /api/paymentmethod` · `GET`, `PUT`, `DELETE /api/paymentmethod/{id}` |
| Receitas | `GET`, `POST /api/income/{mes}/{ano}` · `GET`, `PUT`, `DELETE /api/income/{id}` |
| Despesas fixas | `GET`, `POST /api/fixedexpense/{mes}/{ano}` · `GET`, `PUT`, `DELETE /api/fixedexpense/{id}` |
| Despesas variáveis | `GET`, `POST /api/variableexpense/{mes}/{ano}` · `GET`, `PUT`, `DELETE /api/variableexpense/{id}` |
| Investimentos | `GET`, `POST /api/investment/{mes}/{ano}` · `GET`, `PUT`, `DELETE /api/investment/{id}` |
| Transações | `GET /api/transaction/{mes}/{ano}` · `POST /api/transaction` · `GET`, `PUT`, `DELETE /api/transaction/{id}` |
| Dashboard | `GET /api/dashboard/metrics/{mes}/{ano}` |

Todos os recursos, exceto `/api/auth/*`, exigem autenticação.

## Como rodar

Pré-requisitos: .NET 8 SDK, Node 20+ e PostgreSQL (ou Docker).

**1. Banco**

```sh
docker run --name prisma-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=gerentecontas -p 5432:5432 -d postgres:16
```

**2. API**

```sh
dotnet tool install --global dotnet-ef --version 8.*
cd backend
dotnet ef database update
dotnet run
```

A API sobe em `http://localhost:5000` (e `https://localhost:5001`).
O `appsettings.json` traz apenas valores de desenvolvimento (banco local e uma chave JWT de exemplo).
Para usar a sua própria chave:

```sh
dotnet user-secrets set "JwtSettings:SecretKey" "<chave com 32+ caracteres>"
```

Em produção, defina por variável de ambiente: `JwtSettings__SecretKey`, `ConnectionStrings__DefaultConnection`,
`Cors__AllowedOrigins__0` e, para o login com Google, `GoogleAuth__ClientId`.

**3. Frontend**

```sh
cd frontend
npm install
npm run dev
```

Abre em `http://localhost:8080`. Variáveis opcionais: `VITE_API_URL` (padrão `http://localhost:5000/api`)
e `VITE_GOOGLE_CLIENT_ID` (login com Google).

## Testes

```sh
dotnet test backend/Tests/backend.Tests.csproj
```

Mais de 50 testes de integração sobem a API em memória (EF Core InMemory), então não precisam de banco.
Cobrem registro, login, refresh e logout, o CRUD de cada recurso, token ausente/inválido/expirado,
cookies e CSRF, e o isolamento entre usuários nas categorias.

## Limitações conhecidas

- As camadas são separadas por pasta, não por projeto, e alguns serviços (como o de autenticação) usam o `DbContext` direto em vez de repositórios.
- Não há rate limiting nos endpoints de autenticação.
- Sem `docker-compose`: o banco sobe com o comando acima.
