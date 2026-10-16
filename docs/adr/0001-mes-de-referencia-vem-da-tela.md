# O mês de referência vem da tela, não da data

Todo lançamento (Entrada, Conta, Gasto, Reserva) conta no mês/ano que estava aberto na tela quando foi cadastrado. A data do lançamento é só informativa e pode cair em outro mês: o aluguel de março pago em 2 de abril pertence a março. Por isso `mes`/`ano` são gravados separados de `data`, e as listagens filtram por eles. Os dois não são redundantes, e não se deve derivar um do outro.
