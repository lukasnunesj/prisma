# Hospedagem: app no Fly.io, Postgres no Supabase

A aplicação roda no Fly.io e o banco é um Postgres gerenciado no Supabase. Não usamos o Postgres do Fly porque ele roda como um app nosso, e backup, upgrade e failover seriam responsabilidade nossa. O Supabase tem free tier, backup automático e é Postgres puro: usamos só a connection string, sem SDK nem recursos próprios. Trocar de provedor é trocar `ConnectionStrings__DefaultConnection`.
