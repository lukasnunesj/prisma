# Padrão atual (frontend -> backend) - Flourish Finance Hub
 
## Observação
 
- No momento, as chamadas estão centralizadas em serviços, mas ainda usam dados mockados (simulando backend).
- Tipos base extraídos de `src/types/finance.ts`.
 
---
 
## AUTH
 
### 1) `authService.login`
 
- **Local**: `src/services/auth/authService.ts`
- **Request (`LoginRequest`)**:
 
```ts
{
  email: string;
  password: string;
}
```
 
- **Response (`LoginResponse`)**:
 
```ts
{
  user: {
    id: string;
    name: string;
    email: string;
    avatar?: string;
  };
  token: string;
}
```
 
---
 
## FINANCE
 
### Entradas
 
#### 2) `financeService.entradas.list`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: (sem body)
- **Response**: `Entrada[]`
- **Tipo `Entrada`**:
 
```ts
type Entrada = {
  id: string;
  nome: string;
  valor: number;
  porcentagem?: number;
  mes: number;
  ano: number;
}
```
 
#### 3) `financeService.entradas.add`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: `Omit<Entrada, 'id' | 'mes' | 'ano'>`
 
```ts
{
  nome: string;
  valor: number;
  porcentagem?: number;
}
```
 
- **Response**: `Entrada`
 
#### 4) `financeService.entradas.update`
 
- **Request**: `Entrada`
- **Response**: `Entrada`
 
#### 5) `financeService.entradas.remove`
 
- **Params**:
 
```ts
{
  id: string;
}
```
 
- **Response**: `string` (id removido)
 
### Gastos
 
#### 6) `financeService.gastos.list`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: (sem body)
- **Response**: `Gasto[]`
- **Tipo `Gasto`**:
 
```ts
type Gasto = {
  id: string;
  categoriaId: string;
  formaPagamentoId: string;
  valor: number;
  data: string;
  descricao?: string;
  mes: number;
  ano: number;
}
```
 
#### 7) `financeService.gastos.add`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: `Omit<Gasto, 'id' | 'mes' | 'ano'>`
 
```ts
{
  categoriaId: string;
  formaPagamentoId: string;
  valor: number;
  data: string;
  descricao?: string;
}
```
 
- **Response**: `Gasto`
 
#### 8) `financeService.gastos.update`
 
- **Request**: `Gasto`
- **Response**: `Gasto`
 
#### 9) `financeService.gastos.remove`
 
- **Params**:
 
```ts
{
  id: string;
}
```
 
- **Response**: `string` (id removido)
 
### Contas
 
#### 10) `financeService.contas.list`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: (sem body)
- **Response**: `Conta[]`
- **Tipo `Conta`**:
 
```ts
type Conta = {
  id: string;
  nome: string;
  planejado: number;
  valor: number;
  dataPago?: string;
  pago: boolean;
  mes: number;
  ano: number;
}
```
 
#### 11) `financeService.contas.add`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: `Omit<Conta, 'id' | 'mes' | 'ano'>`
 
```ts
{
  nome: string;
  planejado: number;
  valor: number;
  dataPago?: string;
  pago: boolean;
}
```
 
- **Response**: `Conta`
 
#### 12) `financeService.contas.update`
 
- **Request**: `Conta`
- **Response**: `Conta`
 
#### 13) `financeService.contas.remove`
 
- **Params**:
 
```ts
{
  id: string;
}
```
 
- **Response**: `string` (id removido)
 
### Reservas/Investimentos
 
#### 14) `financeService.reservas.list`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: (sem body)
- **Response**: `ReservaInvestimento[]`
- **Tipo `ReservaInvestimento`**:
 
```ts
type ReservaInvestimento = {
  id: string;
  nome: string;
  tipo: 'reserva' | 'investimento';
  valor: number;
  data: string;
  mes: number;
  ano: number;
}
```
 
#### 15) `financeService.reservas.add`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: `Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>`
 
```ts
{
  nome: string;
  tipo: 'reserva' | 'investimento';
  valor: number;
  data: string;
}
```
 
- **Response**: `ReservaInvestimento`
 
#### 16) `financeService.reservas.update`
 
- **Request**: `ReservaInvestimento`
- **Response**: `ReservaInvestimento`
 
#### 17) `financeService.reservas.remove`
 
- **Params**:
 
```ts
{
  id: string;
}
```
 
- **Response**: `string` (id removido)
 
### Categorias
 
#### 18) `financeService.categorias.list`
 
- **Request**: (sem body)
- **Response**: `Categoria[]`
- **Tipo `Categoria`**:
 
```ts
type Categoria = {
  id: string;
  nome: string;
  cor: string;
  icone?: string;
}
```
 
#### 19) `financeService.categorias.add`
 
- **Request**: `Omit<Categoria, 'id'>`
 
```ts
{
  nome: string;
  cor: string;
  icone?: string;
}
```
 
- **Response**: `Categoria`
 
#### 20) `financeService.categorias.update`
 
- **Request**: `Categoria`
- **Response**: `Categoria`
 
#### 21) `financeService.categorias.remove`
 
- **Params**:
 
```ts
{
  id: string;
}
```
 
- **Response**: `string` (id removido)
 
### Formas de Pagamento
 
#### 22) `financeService.formasPagamento.list`
 
- **Request**: (sem body)
- **Response**: `FormaPagamento[]`
- **Tipo `FormaPagamento`**:
 
```ts
type FormaPagamento = {
  id: string;
  nome: string;
  tipo: 'debito' | 'credito' | 'dinheiro' | 'pix';
}
```
 
#### 23) `financeService.formasPagamento.add`
 
- **Request**: `Omit<FormaPagamento, 'id'>`
 
```ts
{
  nome: string;
  tipo: 'debito' | 'credito' | 'dinheiro' | 'pix';
}
```
 
- **Response**: `FormaPagamento`
 
#### 24) `financeService.formasPagamento.update`
 
- **Request**: `FormaPagamento`
- **Response**: `FormaPagamento`
 
#### 25) `financeService.formasPagamento.remove`
 
- **Params**:
 
```ts
{
  id: string;
}
```
 
- **Response**: `string` (id removido)
 
### Dashboard
 
#### 26) `financeService.dashboard.metrics`
 
- **Params**:
 
```ts
{
  mes: number;
  ano: number;
}
```
 
- **Request**: (sem body)
- **Response**: `DashboardMetrics`
- **Tipo `DashboardMetrics`**:
 
```ts
type DashboardMetrics = {
  totais: {
    entradas: number;
    saidas: number;
    reservas: number;
    usoDebito: number;
    usoCredito: number;
    restante: number;
  };
  comparacaoMesAnterior: {
    entradas: number;
    saidas: number;
    reservas: number;
    usoDebito: number;
    usoCredito: number;
  };
  graficos: {
    fluxoContas: Array<{ nome: string; valor: number; planejado: number; pago: boolean }>;
    alocacaoCategorias: Array<{ categoria: string; valor: number; cor: string }>;
  };
}
