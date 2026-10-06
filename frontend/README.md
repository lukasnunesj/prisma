# Prisma — frontend

SPA em React 18 + TypeScript (Vite, Tailwind, shadcn/ui, React Query, Zustand, Recharts).
Visão geral do projeto, autenticação e como rodar a API: veja o [README da raiz](../README.md).

```sh
npm install
npm run dev      # http://localhost:8080
npm run lint
npm run build
```

Variáveis opcionais: `VITE_API_URL` (padrão `http://localhost:5000/api`) e `VITE_GOOGLE_CLIENT_ID` (login com Google).
Os contratos esperados da API estão em [`api-contracts.md`](./api-contracts.md).
