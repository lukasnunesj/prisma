# SPA servida pela própria API, na mesma origem

O build do React vai para o `wwwroot` da API e é servido por ela, com um app e uma URL só. Com uma origem só, não há CORS, o cookie do refresh token pode ser `SameSite=Strict`, e o bootstrap de CSRF não chama outro host (origem do bug de porta 5000 vs 5001). Front em CDN separado foi rejeitado: na escala de poucos usuários não traz ganho e dobra a configuração de deploy e de cookies.
