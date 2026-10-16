# Cartão de crédito é só uma forma de pagamento, sem fatura

Uma compra no cartão é um Gasto como outro qualquer (ou uma Conta, no caso de assinaturas recorrentes como a Netflix), e conta como saída no mês em que é lançada. Não existe Fatura, nem fechamento e vencimento do cartão, nem conciliação. As saídas do mês são sempre Gastos + Contas, sem exceção por forma de pagamento.

## Considered Options

- **Fatura como Conta, e as compras no crédito só como detalhamento**: rejeitada. Exige uma regra de exclusão nas saídas e confunde ("por que esse Gasto não conta?").
- **Fatura derivada da soma das compras, com dia de fechamento e vencimento no cartão**: rejeitada. É complexa demais para a fase atual.

## Consequences

- Não lance a fatura do cartão como Conta: ela somaria de novo os Gastos já lançados.
- O mês de uma compra no cartão é o mês em que ela foi lançada, e não o mês em que a fatura vence. Quem quiser o mês da fatura abre esse mês na tela e lança lá (ADR 0001).
