# AgendAI — Backend

API REST do AgendAI desenvolvida em .NET 10 com Minimal APIs.

## 🛠️ Stack

- **.NET 10** — Minimal API
- **Entity Framework Core** — ORM
- **PostgreSQL** — Banco de dados
- **Keycloak** — Autenticação e autorização (OAuth2 + JWT)
- **RabbitMQ + MassTransit** — Fila de mensagens (em breve)
- **Semantic Kernel + MCP** — Agente LLM (em breve)

## 📁 Estrutura
```
src/
├── AgendAI.API/            # Endpoints REST + configuração
├── AgendAI.Worker/         # Processamento assíncrono + LLM
├── AgendAI.Core/           # Domínios e regras de negócio
└── AgendAI.Infrastructure/ # EF Core, MCP Servers, integrações
```

## 🚀 Rodando localmente

### Pré-requisitos

- .NET 10 SDK
- Docker Engine
- WSL2 (Windows)

### 1. Sobe o banco e o Keycloak
```bash
cd docker
docker compose -f docker-compose.dev.yml --env-file .env up -d
```

### 2. Configura as variáveis
```bash
cp docker/.env.example docker/.env
# edite o .env com suas credenciais
```

### 3. Roda a API
```bash
cd src/AgendAI.API
dotnet run
```

A API sobe em `http://localhost:5080`

## 📖 Documentação da API

Acesse o Scalar (Swagger) em:
```
http://localhost:5080/scalar/v1
```

## 🔐 Autenticação

A API usa Keycloak com OAuth2 + JWT. Para obter um token de teste:
```bash
curl -X POST http://localhost:8080/realms/agendai/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=agendai-web" \
  -d "username=owner_test" \
  -d "password=123456"
```

### Usuários de teste

| Usuário | Senha | Role |
|---|---|---|
| owner_test | 123456 | owner |
| hairdresser_test | 123456 | hairdresser |
| client_test | 123456 | client |

## 🗄️ Migrations
```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration \
  --project src/AgendAI.Infrastructure \
  --startup-project src/AgendAI.API

# Aplicar migrations
dotnet ef database update \
  --project src/AgendAI.Infrastructure \
  --startup-project src/AgendAI.API
```

## 📋 Endpoints disponíveis

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| GET | /health | Health check | ❌ |
| GET | /api/v1/scheduling | Lista agendamentos | ✅ |
| GET | /api/v1/scheduling/{id} | Busca agendamento | ✅ |
| POST | /api/v1/scheduling | Cria agendamento | ✅ |
| PATCH | /api/v1/scheduling/{id}/reschedule | Reagenda | ✅ |
| PATCH | /api/v1/scheduling/{id}/cancel | Cancela | ✅ |

## 🗺️ Roadmap

- [x] Setup inicial .NET 10
- [x] EF Core + PostgreSQL
- [x] Autenticação Keycloak + JWT
- [x] Endpoints de agendamento
- [ ] Autorização por roles
- [ ] Domínio de profissionais e comissões
- [ ] Integração RabbitMQ + Worker
- [ ] Integração WhatsApp (Meta Cloud API)
- [ ] Agente LLM (Semantic Kernel + MCP)
