# Plano de lançamento

Resultado da revisão de código, arquitetura e domínio (out/2026). Vocabulário em [`CONTEXT.md`](../CONTEXT.md); decisões difíceis de reverter em [`docs/adr/`](./adr/). Este arquivo é a fonte para os tickets: cada item abaixo vira um ou mais, e a prioridade final se define na hora de criá-los. A ordem das seções já é a sugerida.

## Produto

- **Fase atual: amigos e família.** Cadastro aberto para quem tiver a URL, sem monetização. Vira produto depois do feedback deles. Não construir nada "de produto" agora, mas também não fechar portas: dados isolados por usuário e exportáveis.
- **Login: só Google na interface.** Login e cadastro com senha continuam na API, testados, mas ficam escondidos do front. Assim não é preciso recuperação de senha nem provedor de e-mail. O e-mail volta quando virar produto.
- **Feedback: informal** (WhatsApp). Sem botão de feedback nem analytics por enquanto.
- **Celular é prioridade no fluxo de lançar Gasto** (mobile-first + PWA instalável). O resto pode continuar desktop-first.
- **Hospedagem:** Fly.io + Supabase ([ADR 0004](./adr/0004-hospedagem-fly-e-supabase.md)), front servido pela API ([ADR 0005](./adr/0005-spa-servida-pela-api-na-mesma-origem.md)).

## Regras de negócio decididas

1. **Mês de referência** = mês aberto na tela no cadastro; a data é informativa ([ADR 0001](./adr/0001-mes-de-referencia-vem-da-tela.md)).
2. **Fluxo do mês:** Restante = Entradas − (Gastos + Contas + Reservas).
3. **Cartão de crédito é só uma Forma de pagamento** ([ADR 0002](./adr/0002-cartao-de-credito-e-so-forma-de-pagamento.md)). Compra no cartão é Gasto. Assinatura (Netflix) é Conta recorrente com forma de pagamento cartão. Não existe Fatura.
4. **Conta e Gasto têm Subcategoria (obrigatória) e Forma de pagamento.** O dashboard mostra **À vista** (débito + Pix + dinheiro) vs **Crédito**, somando Gastos e Contas.
5. **Recorrente** é uma opção de Conta e de Entrada, não um tipo. Ao abrir um mês sem lançamentos recorrentes, eles são copiados do mês anterior. Reserva e Gasto nunca são recorrentes.
   - Ao editar ou apagar, o sistema sempre pergunta: **"só esta"** ou **"todas"** (ver regra 7).
   - Para encerrar uma recorrência sem perder o histórico, desmarque "recorrente" na Conta do mês atual ("só esta"). Os meses seguintes deixam de copiá-la, e os anteriores ficam como estão.
6. **Parcelamento (só Gasto):** ao lançar, marcar "parcelado" e informar o número de parcelas e o **valor da parcela** (já com juros). O sistema cria a 1/n no mês aberto e as seguintes nos meses seguintes, com "x/n" na descrição (3/10 = terceira de dez).
7. **Editar ou apagar um lançamento de uma série** (Parcela ou Conta/Entrada recorrente): o sistema sempre pergunta **"só esta"** ou **"todas"**. É tudo ou nada: "todas" altera ou apaga a série inteira, inclusive os meses passados ([ADR 0006](./adr/0006-series-materializadas.md)). Algumas coisas nunca são propagadas por "todas": o estado pago/não pago, o mês de referência e o "x/n" de cada Parcela.
8. **Categorias:** dois níveis (Categoria → Subcategoria). Gasto e Conta exigem Subcategoria. O gráfico agrupa pela Categoria e detalha ao clicar. Uma Categoria com Subcategorias ou lançamentos não pode ser apagada. Todo usuário novo ganha um conjunto padrão.
9. **Mês nunca fecha:** qualquer mês pode ser editado a qualquer momento.

## Trabalho

### 1. Fundação: refatoração do domínio (fazer antes de tudo)

Como nada está no ar, as mudanças de schema saem juntas, numa migration nova.

- [ ] Apagar as 11 migrations e gerar uma `InitialCreate` limpa.
- [ ] Renomear o domínio para pt-BR em C#, tabelas, rotas e JSON ([ADR 0003](./adr/0003-dominio-em-portugues-no-codigo-e-na-api.md)): `Entrada`, `Conta`, `Gasto`, `Reserva`, `Categoria`, `FormaPagamento`. Rotas `/entradas`, `/contas`, `/gastos`, `/reservas`, `/categorias`, `/formas-pagamento`, `/dashboard`.
- [ ] Remover o endpoint genérico `/transaction` (`TransactionController` e `TransactionService`), que está obsoleto.
- [ ] **Bug:** FK duplicada `PaymentMethodId1` em `Transactions`. `TransactionConfiguration` declara `HasOne<PaymentMethod>()` sem a navegação existente. Usar `HasOne(x => x.PaymentMethod)`.
- [ ] Índice composto `(UserId, Ano, Mes, Tipo)` nas transações. Remover os índices só em `IsDeleted`.
- [ ] Índice único `(UserId, Nome)` em Categoria (dentro da mesma Categoria pai) e em Forma de pagamento.
- [ ] Conta e Gasto passam a exigir Subcategoria e Forma de pagamento (`ParentCategoryId` passa a ser usado; um nível só).
- [ ] Identificador de série nos lançamentos (Parcelas de uma compra, meses de uma recorrente), para "só esta / todas" (ADR 0006).
- [ ] Extrair o mapeamento entidade → DTO (`ToDto()`), hoje copiado 4× em cada service.
- [ ] Corrigir os warnings CS8603 em `VariableExpenseService.cs:30` e `:79`.
- [ ] Remover os repositórios e as interfaces de uma implementação só. Os services usam o `DbContext` direto, e as consultas por usuário e mês viram extensões de `IQueryable` ([ADR 0007](./adr/0007-ddd-tatico-sem-repositorios.md)). Juntar os três arquivos de DI num só.
- [ ] Tratador global de erros com ProblemDetails: `ArgumentException` do domínio vira 400, o resto vira 500 com log. Remover os `catch (Exception)` que engolem erro no `AuthController`.
- [ ] `AuthController`: mover a criação do usuário Google para o `AuthService`, e o bloco de cookies (copiado 4×) para um helper.
- [ ] Validar mês (1–12) e ano nas rotas `{mes}/{ano}`.
- [ ] Remover a credencial de teste comentada no fim do `Program.cs`.
- [ ] `CategoryController.Create` deve devolver o que foi salvo, e não a entrada. Corrigir a mensagem "Nome não pode ser vazio" em `ChangeColor`.
- [ ] Índices únicos filtrados por `IsDeleted = false`.

### 2. Segurança e isolamento

- [ ] **Validar posse** de Categoria e Forma de pagamento ao criar ou editar Conta e Gasto. Hoje um usuário pode gravar o id de outro, e como a FK é `Restrict`, isso impede o dono de apagar a própria categoria.
- [ ] Testes de isolamento entre usuários para todos os recursos (hoje só Categoria tem).
- [ ] Cookie do refresh token `SameSite=Strict` e remoção do CORS depois da mesma origem (ADR 0005).
- [ ] Rate limiting em `/auth/*` (o middleware nativo do ASP.NET Core 8 resolve). Prioridade menor enquanto o login for só Google.

### 3. Regras de negócio novas

- [ ] Dashboard: Restante subtrai Reservas (já faz); cards "Uso de débito"/"Uso de crédito" viram À vista (débito + Pix + dinheiro) vs Crédito, somando Gastos **e** Contas (hoje só Gastos, e Pix e dinheiro ficam de fora).
- [ ] Recorrente (Conta e Entrada) com cópia ao abrir o mês. **Isso não existe hoje:** abrir um mês novo mostra a lista vazia. Índice único `(série, mês, ano)`, para duas abas abrindo o mês não duplicarem.
- [ ] Diálogo "só esta / todas" ao editar ou apagar Parcela ou recorrente. Pago/não pago, mês e "x/n" nunca são propagados.
- [ ] Conjunto padrão de Categorias e Subcategorias no primeiro login.
- [ ] Interface de Subcategoria: cadastro, seleção no Gasto e drill-down no gráfico.
- [ ] Parcelamento no Gasto: campos "parcelado" e "número de parcelas", com o valor digitado sendo o da parcela, gerando os registros x/n a partir do mês aberto.
- [ ] Dashboard: consultar o mês em menos idas ao banco (hoje são 8) e trocar os `FirstOrDefault` de forma de pagamento por um dicionário.

### 4. Frontend

- [ ] **Bug:** URL base divergente. `App.tsx` usa `https://localhost:5001/api` e `apiClient.ts` usa `http://localhost:5000/api`. Com a mesma origem, as duas viram o caminho relativo `/api`.
- [ ] Apagar `financeStore`, resto do mock: é um cache paralelo ao React Query, com `delay` sem uso.
- [ ] Apagar os mappers PT↔EN dos services, que ficam desnecessários depois do ADR 0003.
- [ ] `useFinancialData.ts` (570 linhas): uma factory de hooks CRUD otimistas no lugar de 12 cópias.
- [ ] `<ProtectedRoute>` no lugar de `token ? <X/> : <Navigate/>` repetido em cada rota.
- [ ] Login: esconder o formulário de e-mail e senha, deixando só o Google.
- [ ] Formulário de lançamento de Gasto mobile-first + manifest PWA.
- [ ] Code-split por rota (o bundle tem 872 kB num chunk só).
- [ ] Corrigir os 8 warnings de lint (fast-refresh, `exhaustive-deps`).
- [ ] Remover os 37 componentes shadcn sem uso e as dependências sem uso (`react-hook-form`, `zod`, `cmdk`, `embla-carousel-react`, `vaul`, `input-otp`, `react-resizable-panels`...).
- [ ] Remover as guardas de login duplicadas nas páginas (Login, Dashboard, FormasPagamento), que o `<ProtectedRoute>` cobre. No store, `isAuthenticated` passa a ser derivado de `token`.

### 5. Deploy

- [ ] Build do front copiado para o `wwwroot` da API, com fallback de rota para `index.html`.
- [ ] `Dockerfile` multi-stage (node build → dotnet publish) e `fly.toml`.
- [ ] Projeto no Supabase, com a connection string em secret do Fly (`ConnectionStrings__DefaultConnection`), mais `JwtSettings__SecretKey` e `GoogleAuth__ClientId`.
- [ ] `dotnet ef database update` no deploy (release command do Fly).
- [ ] CI: deploy no Fly a cada push na `main`, depois dos testes.

### 6. Limpeza e documentação

- [ ] Apagar `frontend/api-contracts.md` (diz que os dados são mockados, o que já não é verdade).
- [ ] Apagar ou mover `insomnia-scratchpad.json` da raiz (`backend/backend.http` já cobre).
- [ ] Atualizar o README: rotas em pt-BR, deploy, login só Google e limitações.
- [ ] Nota para quem desenvolve: os testes exigem o runtime .NET 8. Com só o .NET 10 instalado e roll-forward, 39 testes falham.

## Depois do feedback (fora deste plano)

Recuperação de senha por e-mail, verificação de e-mail, LGPD formal, analytics, testes no front, Fatura e conciliação do cartão, fechar mês.
