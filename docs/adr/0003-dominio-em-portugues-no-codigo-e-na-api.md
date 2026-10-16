# Domínio em português no código e na API

Os termos do domínio (Entrada, Conta, Gasto, Reserva, Categoria, Fatura...) são escritos em português em toda parte: classes C#, tabelas, rotas (`/contas`, `/gastos`), JSON e tipos do front. Termos técnicos continuam em inglês (`Service`, `Repository`, `Controller`). O domínio é brasileiro e não traduz bem ("Conta" com vencimento não é `FixedExpense`). Antes, a API em inglês obrigava o front a ter uma camada de mappers só para traduzir nomes. O glossário fica em `CONTEXT.md`.
