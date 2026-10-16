# Prisma

Controle de finanças pessoais por mês: quanto entrou, quanto saiu, quanto foi guardado e quanto sobrou, comparando com o mês anterior.

## Language

### Período

**Mês de referência**:
O mês/ano em que um lançamento conta: sempre o mês que estava aberto na tela quando ele foi cadastrado. A data do lançamento é só informativa e pode ser de qualquer outro mês.
_Avoid_: Competência, período, mês da data

### Entradas e Reservas

**Entrada**:
Dinheiro que entra no mês (salário, freela, reembolso). Pode ser recorrente, como a Conta.
_Avoid_: Income, receita

**Reserva**:
Dinheiro guardado no mês (aporte, poupança, investimento). Controle básico de quanto foi separado, não de saldo ou rentabilidade. Sempre lançada manualmente.
_Avoid_: Investment, investimento, aporte

### Saídas

**Conta**:
Saída de um mês com vencimento e estado pago/não pago (aluguel, luz, financiamento, assinaturas como Netflix). Sempre classificada numa Subcategoria e com Forma de pagamento. Pertence a um único mês de referência.
_Avoid_: Despesa fixa, FixedExpense, bill, vencimento

**Recorrente**:
Opção de uma Conta ou Entrada que a faz reaparecer automaticamente no mês seguinte, quando ele é aberto. Não é outro tipo de lançamento: é uma Conta (ou Entrada) que se repete até ser encerrada.
_Avoid_: Fixa, modelo, template

**Gasto**:
Saída pontual, sem recorrência (mercado, compras, restaurante), inclusive o que foi no cartão. Sempre classificado numa Subcategoria.
_Avoid_: Despesa variável, VariableExpense

**Categoria**:
Agrupamento de primeiro nível de onde o dinheiro foi (Mercado, Casa, Lazer). Todo usuário começa com um conjunto padrão.
_Avoid_: Tag, grupo

**Subcategoria**:
Divisão de uma Categoria (Mercado → Feira, Açougue). Só existe um nível abaixo da Categoria.
_Avoid_: Categoria filha

**Forma de pagamento**:
Meio usado para pagar uma Conta ou um Gasto: débito, crédito (cartão), Pix ou dinheiro. Só informa como foi pago; não muda em que mês nem quanto a saída conta.
_Avoid_: Payment method, meio de pagamento

**Parcela**:
Um dos Gastos gerados por uma compra parcelada: um por mês de referência seguido, a partir do mês aberto, cada um com "x/n" na descrição (3/10 = terceira de dez). O valor informado é o da parcela, já com juros.
_Avoid_: Prestação

**À vista**:
Saídas do mês pagas por débito, Pix ou dinheiro.
_Avoid_: Uso de débito

**Crédito**:
Saídas do mês pagas no cartão de crédito.
_Avoid_: Uso de crédito, fatura

**Restante**:
Entradas menos Gastos, Contas e Reservas do mês.
_Avoid_: Saldo, sobra
