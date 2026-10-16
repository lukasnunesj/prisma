# DDD tático sem repositórios nem interfaces de uma implementação

O que vale do DDD aqui é o modelo: entidades ricas, com setters privados e as invariantes dentro delas (valor maior que zero, mês válido); domínio sem dependência de infraestrutura (o mapeamento do EF fica nas `Configurations`); e uma linguagem ubíqua ([`CONTEXT.md`](../../CONTEXT.md)). As pastas `Api`, `Application`, `Domain` e `Infrastructure` organizam o código num projeto só.

Não usamos uma camada de repositórios. O `DbContext` do EF Core já é repositório e unit of work, e os services o usam direto. As consultas repetidas (do usuário, no mês de referência, por tipo) ficam em métodos de extensão sobre `IQueryable`, num lugar só. Também não criamos interfaces para classes com uma implementação só: os testes são de integração, sobem a API inteira e não usam mocks. A exceção é o que fala com o mundo externo (validação do token do Google), que tem interface para os testes poderem trocar.

## Considered Options

- **Clean Architecture completa** (repositório + interface por recurso, projetos separados por camada): rejeitada. Os repositórios só repassariam chamadas ao EF, sem isolar nada, porque os services continuariam dependendo de `IQueryable` e do comportamento do EF (soft delete, tracking). As interfaces não serviriam a nenhum teste. Trocar o ORM, o motivo clássico, não é um cenário deste projeto.
