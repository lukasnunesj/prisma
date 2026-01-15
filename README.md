# Prisma

Dashboard simples de finanças pessoais com backend em .NET e frontend em React.

## Estrutura

- `backend/`: API em .NET 8 (Entity Framework Core + PostgreSQL)
- `frontend/`: UI em React (Vite + TypeScript + Tailwind/shadcn-ui)

## Requisitos

- Node.js 18+
- .NET SDK 8
- PostgreSQL

## Configuração

### Backend

1. Ajuste a string de conexão em `backend/appsettings.json`.
2. Aplique as migrations:

```sh
# dentro de backend/
dotnet ef database update
```

3. Inicie a API:

```sh
# dentro de backend/
dotnet run
```

A API sobe em `http://localhost:5000` (e HTTPS em `https://localhost:5001`).

### Frontend

```sh
# dentro de frontend/
npm install
npm run dev
```

O Vite inicia em `http://localhost:5173`.

## Testes

```sh
# dentro de backend/
dotnet test Tests/backend.Tests.csproj
```

## Tecnologias

- Backend: .NET 8, ASP.NET Core, EF Core, PostgreSQL, JWT
- Frontend: React, TypeScript, Vite, Tailwind CSS, shadcn-ui
