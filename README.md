# Devameet — API (C#)

API de salas de reunião virtuais, no estilo Gather: cada usuário monta salas com objetos em um mapa e os participantes se movem e conversam por áudio e vídeo em tempo real.

Construída com ASP.NET Core e SQL Server. Nasceu como projeto da formação Devaria e foi revisada para servir de referência: senhas com argon2, nenhum segredo no repositório, cada usuário alterando só as próprias salas e documentação OpenAPI navegável.

## Stack

| Camada       | Tecnologia                                          |
| ------------ | --------------------------------------------------- |
| Runtime      | .NET 10, ASP.NET Core                               |
| Banco        | SQL Server com Entity Framework Core 10             |
| Autenticação | JWT (HS256, validade de 7 dias), senhas em argon2id |
| Documentação | Swagger (Swashbuckle) e Scalar                      |
| Tempo real   | SignalR                                             |
| Testes       | xUnit                                               |

## Como rodar

```bash
dotnet user-secrets set "JWT:SecretKey" "uma-chave-com-pelo-menos-32-caracteres"
```

```bash
dotnet tool install --global dotnet-ef
```

```bash
dotnet ef database update
```

```bash
dotnet run
```

A string de conexão padrão, em `appsettings.json`, aponta para um SQL Server local com autenticação integrada. Para outro servidor, defina `ConnectionStrings:DefaultConnection` da mesma forma.

| Endereço                   | O que é                        |
| -------------------------- | ------------------------------ |
| `/scalar`                  | Referência interativa (Scalar) |
| `/swagger`                 | Swagger UI                     |
| `/swagger/v1/swagger.json` | Documento OpenAPI              |

### Configuração

Fora do ambiente de desenvolvimento, use variáveis de ambiente (`__` no lugar de `:`).

| Chave                                  | Variável de ambiente                   | Obrigatória | Para quê                                     |
| -------------------------------------- | -------------------------------------- | ----------- | -------------------------------------------- |
| `JWT:SecretKey`                        | `JWT__SecretKey`                       | sim         | Segredo que assina os tokens (32+ caracteres) |
| `ConnectionStrings:DefaultConnection`  | `ConnectionStrings__DefaultConnection` | sim         | Conexão com o SQL Server                     |
| `Cors:Origins`                         | `Cors__Origins__0`, `Cors__Origins__1`… | não         | Origens do front (padrão: `http://localhost:3000`) |

A API não sobe sem `JWT:SecretKey`: a falha aparece na inicialização, não na primeira requisição.

## Rotas

| Rota                        | Método                 | O que faz                                        |
| --------------------------- | ---------------------- | ------------------------------------------------ |
| `/api/Auth/login`           | POST                   | Entrar e receber o token (pública)               |
| `/api/Auth/register`        | POST                   | Criar conta (pública)                            |
| `/api/User`                 | GET, PUT               | Ver e atualizar o próprio cadastro               |
| `/api/User/getuserbyid`     | GET                    | Ver um usuário                                   |
| `/api/Meet`                 | GET, POST, PUT, DELETE | Salas de reunião do usuário logado               |
| `/api/Meet/objects`         | GET                    | Objetos (móveis, paredes) de uma sala            |
| `/api/Room`                 | GET                    | Dados da sala para entrar na reunião             |
| `/roomHub`                  | SignalR                | Posição, áudio e vídeo dos participantes em tempo real |

Fora login e registro, as rotas HTTP esperam `Authorization: Bearer <token>`.

## Estrutura

```
Controllers/  rotas da API
Hubs/         hub do SignalR com a sala em tempo real
Service/      senha e token
Repository/   acesso ao banco (interfaces e implementações)
Models/       entidades e contexto do Entity Framework
Dtos/         formatos de entrada e saída
Migrations/   migrations do Entity Framework
tests/        testes unitários
```

## Testes

```bash
dotnet test tests/DevameetCSharp.Tests
```

## Segurança

- Senhas gravadas com argon2id. Contas antigas, em MD5, continuam entrando e têm a senha regravada em argon2 no primeiro login.
- A chave do JWT vem da configuração; antes estava versionada no `appsettings.json`. Tokens assinados com a chave antiga não são mais aceitos.
- Só o dono edita ou exclui uma sala; a de outra pessoa responde como não encontrada.
- Token com validade de 7 dias e algoritmo fixo.
- Erros inesperados respondem com mensagem genérica; o detalhe fica no log do servidor.
