# Parcelas e recorrências são lançamentos reais ligados por uma série

Uma compra parcelada gera de uma vez um Gasto por mês, com "x/n" na descrição. Uma Conta ou Entrada recorrente é copiada como um lançamento novo a cada mês aberto. Esses lançamentos são registros comuns: o dashboard e as listagens não calculam nada "virtual". O que os liga é um identificador de série, usado só para a pergunta feita ao editar ou apagar: "só esta" ou "todas".

"Todas" é tudo ou nada, por decisão de produto: altera ou apaga a série inteira, **inclusive meses passados**, que então mudam no dashboard. Não existe "esta e as seguintes". Pago/não pago, mês de referência e o "x/n" de cada Parcela nunca são propagados. Para encerrar uma recorrência mantendo o histórico, desmarque "recorrente" no mês atual ("só esta").

## Considered Options

- **Calcular parcelas e recorrências na hora, a partir de uma regra**: rejeitada. Cada exceção (juros diferentes numa parcela, conta de luz com valor diferente) vira um caso especial, e toda consulta do mês precisaria expandir as regras.
- **"Esta e as seguintes", preservando o passado**: rejeitada para manter uma única opção, simples de entender.
