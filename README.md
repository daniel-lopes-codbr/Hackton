# AgroSolutions API - MVP

API de alta performance para ingestão e gerenciamento de dados de sensores agrícolas, desenvolvida com .NET 8, seguindo os princípios de **Domain-Driven Design (DDD)** e **CQRS (Command Query Responsibility Segregation)**.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Tecnologias](#tecnologias)
- [Funcionalidades](#funcionalidades)
- [Fases Implementadas](#fases-implementadas)
- [Como Executar](#como-executar)
- [Testando a API](#testando-a-api)
- [Documentação](#documentação)

## 🎯 Visão Geral

O AgroSolutions é uma solução completa para gerenciamento de dados agrícolas, incluindo:
- **Ingestão de alta performance** de dados de sensores
- **Gerenciamento de Fazendas e Campos**
- **Autenticação e Autorização** com JWT
- **Processamento e Análise** de dados (Azure Functions)
- **Observabilidade** completa (Logging, Health Checks)

## 🏗️ Arquitetura

### Padrão Arquitetural: DDD + CQRS

A aplicação segue uma arquitetura híbrida **DDD + CQRS** com o seguinte fluxo:

```
Controller → Service → MediatR → Handler → Repository → Database
```

#### Componentes Principais:

1. **Controllers**: Endpoints da API REST
2. **Services**: Camada de orquestração que utiliza MediatR
3. **MediatR**: Despachador de Commands e Queries
4. **Handlers**: Lógica de negócio (Commands e Queries)
5. **Repositories**: Acesso a dados
6. **Domain Entities**: Entidades de domínio com regras de negócio

### Padrões Implementados:

- ✅ **CQRS**: Separação entre Commands (escrita) e Queries (leitura)
- ✅ **Notification Pattern**: Validações sem exceções
- ✅ **Result Pattern**: Retornos padronizados com `Result<T>`
- ✅ **AutoMapper**: Mapeamento automático DTO ↔ Entity ↔ Command
- ✅ **FluentValidation**: Validações declarativas
- ✅ **Repository Pattern**: Abstração de acesso a dados

## 📁 Estrutura do Projeto

```
AgroSolutions/
├── src/
│   ├── AgroSolutions.Api/                    # API Principal
│   │   ├── Application/                       # Camada de Aplicação (CQRS)
│   │   │   ├── Commands/                     # Commands (escrita)
│   │   │   │   ├── Users/
│   │   │   │   ├── Farms/
│   │   │   │   ├── Fields/
│   │   │   │   └── Ingestion/
│   │   │   ├── Queries/                      # Queries (leitura)
│   │   │   │   ├── Users/
│   │   │   │   ├── Farms/
│   │   │   │   ├── Fields/
│   │   │   │   └── Ingestion/
│   │   │   ├── Handlers/                     # Handlers (lógica de negócio)
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── Users/
│   │   │   │   │   ├── Farms/
│   │   │   │   │   ├── Fields/
│   │   │   │   │   └── Ingestion/
│   │   │   │   └── Queries/
│   │   │   │       ├── Users/
│   │   │   │       ├── Farms/
│   │   │   │       ├── Fields/
│   │   │   │       └── Ingestion/
│   │   │   ├── Validators/                   # FluentValidation
│   │   │   │   └── Commands/
│   │   │   ├── Mappings/                     # AutoMapper Profiles
│   │   │   │   ├── UserMappingProfile.cs
│   │   │   │   ├── FarmMappingProfile.cs
│   │   │   │   ├── FieldMappingProfile.cs
│   │   │   │   └── IngestionMappingProfile.cs
│   │   │   └── Common/
│   │   │       ├── Notifications/            # Notification Pattern
│   │   │       │   ├── Notification.cs
│   │   │       │   └── NotificationContext.cs
│   │   │       └── Results/                  # Result Pattern
│   │   │           └── Result.cs
│   │   ├── Controllers/                      # Controllers REST
│   │   │   ├── AuthController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── FarmsController.cs
│   │   │   ├── FieldsController.cs
│   │   │   └── IngestionController.cs
│   │   ├── Services/                         # Services (orquestração)
│   │   │   ├── IAuthService.cs / AuthService.cs
│   │   │   ├── IUserService.cs / UserService.cs
│   │   │   ├── IFarmService.cs / FarmService.cs
│   │   │   ├── IFieldService.cs / FieldService.cs
│   │   │   └── IIngestionService.cs / IngestionService.cs
│   │   ├── Models/                           # DTOs
│   │   │   ├── UserDto.cs
│   │   │   ├── FarmDto.cs
│   │   │   ├── FieldDto.cs
│   │   │   └── SensorReadingDto.cs
│   │   ├── HealthChecks/                     # Health Checks
│   │   └── Program.cs                        # Configuração da aplicação
│   │
│   ├── AgroSolutions.Domain/                 # Camada de Domínio
│   │   ├── Entities/                         # Entidades de Domínio
│   │   │   ├── Entity.cs                     # Classe base
│   │   │   ├── User.cs
│   │   │   ├── Farm.cs
│   │   │   ├── Field.cs
│   │   │   └── SensorReading.cs
│   │   ├── ValueObjects/                     # Value Objects
│   │   │   └── Property.cs
│   │   ├── Repositories/                     # Interfaces e Implementações
│   │   │   ├── IUserRepository.cs / UserRepository.cs
│   │   │   ├── IFarmRepository.cs / FarmRepository.cs
│   │   │   ├── IFieldRepository.cs / FieldRepository.cs
│   │   │   └── ISensorReadingRepository.cs / SensorReadingRepository.cs
│   │   ├── Enums/
│   │   │   └── UserRole.cs
│   │   ├── Exceptions/
│   │   │   └── DomainException.cs
│   │   └── Data/
│   │       └── AgroSolutionsDbContext.cs     # EF Core DbContext
│   │
│   └── AgroSolutions.Functions/              # Azure Functions
│       ├── Functions/
│       │   └── ProcessSensorDataFunction.cs
│       └── Services/
│           ├── IDataProcessingService.cs / DataProcessingService.cs
│           └── IAnalyticsService.cs / AnalyticsService.cs
│
└── tests/
    ├── AgroSolutions.Api.Tests/
    ├── AgroSolutions.Domain.Tests/
    └── AgroSolutions.Functions.Tests/
```

## 🛠️ Tecnologias

### Core
- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core 8.0** - ORM
- **SQL Server** / **InMemory Database** - Persistência

### Arquitetura DDD + CQRS
- **MediatR 12.2.0** - Implementação de CQRS
- **FluentValidation 11.9.0** - Validações declarativas
- **AutoMapper 12.0.1** - Mapeamento objeto-objeto

### Autenticação e Segurança
- **JWT (JSON Web Tokens)** - Autenticação
- **BCrypt.Net-Next 4.0.3** - Hash de senhas
- **Role-Based Authorization** - Admin e User

### Observabilidade
- **Serilog** - Logging estruturado
- **Health Checks** - Monitoramento de saúde
- **Health Checks UI** - Interface visual

### Processamento
- **Azure Functions** (Isolated Worker Model) - Processamento assíncrono

### Documentação
- **Swagger/OpenAPI** - Documentação interativa da API

## ✨ Funcionalidades

### 1. Autenticação e Autorização
- ✅ Login com JWT
- ✅ Roles: Admin (acesso total) e User (somente leitura)
- ✅ Proteção de endpoints com `[Authorize]`

### 2. Gerenciamento de Usuários (Admin Only)
- ✅ CRUD completo de usuários
- ✅ Hash de senhas com BCrypt
- ✅ Validação de email único
- ✅ Gerenciamento de roles

### 3. Gerenciamento de Fazendas
- ✅ CRUD completo de fazendas
- ✅ Value Object `Property` (Nome, Localização, Área, Descrição)
- ✅ Informações do proprietário

### 4. Gerenciamento de Campos
- ✅ CRUD completo de campos
- ✅ Associação com fazendas
- ✅ Informações de cultivo (tipo, datas)

### 5. Ingestão de Dados de Sensores
- ✅ Ingestão única (single)
- ✅ Ingestão em lote (batch sequencial)
- ✅ Ingestão paralela (batch paralelo)
- ✅ Alta performance com processamento otimizado

### 6. Processamento e Análise (Azure Functions)
- ✅ Detecção de anomalias
- ✅ Análise de tendências
- ✅ Recomendações
- ✅ Estatísticas

## 📊 Fases Implementadas

### ✅ FASE 1: API Base e Ingestão
- API REST básica
- Endpoint de ingestão única
- InMemory storage

### ✅ FASE 2: Ingestão em Lote
- Endpoint de batch sequencial
- Endpoint de batch paralelo
- Otimizações de performance

### ✅ FASE 3: Azure Functions
- Processamento assíncrono
- Análise de dados
- Detecção de anomalias

### ✅ FASE 4: Observabilidade
- Serilog para logging estruturado
- Health Checks
- Health Checks UI

### ✅ FASE 5: Persistência
- Entity Framework Core
- SQL Server / InMemory Database
- Migrations

### ✅ FASE 6: Gerenciamento de Fazendas e Campos
- CRUD completo de Farms
- CRUD completo de Fields
- Relacionamentos

### ✅ FASE 6.5: Autenticação e Usuários
- JWT Authentication
- Role-Based Authorization
- CRUD de Usuários

### ✅ FASE 7: Refatoração DDD + CQRS
- **MediatR** para Commands e Queries
- **Notification Pattern** para validações
- **Result Pattern** para retornos padronizados
- **AutoMapper** para mapeamentos
- **FluentValidation** para validações
- Arquitetura: Controller → Service → MediatR → Handler

## 🚀 Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- SQL Server (opcional, usa InMemory por padrão em Development)
- Visual Studio 2022 ou VS Code

### 1. Clone o repositório
```bash
git clone <repository-url>
cd AgroSolutions
```

### 2. Restaure as dependências
```bash
dotnet restore
```

### 3. Configure o banco de dados (opcional)
Para usar SQL Server, edite `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AgroSolutions;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 4. Execute a API
```bash
cd src/AgroSolutions.Api
dotnet run
```

A API estará disponível em:
- **HTTP**: `http://localhost:5000` (ou porta dinâmica)
- **Swagger**: `http://localhost:5000` (raiz)

## 🧪 Testando a API

### 1. Autenticação

#### Criar Usuário Admin (primeira vez)
```bash
POST /api/users
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "name": "Admin User",
  "email": "admin@agrosolutions.com",
  "password": "Admin123!",
  "role": "Admin"
}
```

#### Login
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@agrosolutions.com",
  "password": "Admin123!"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 2. Usar o Token

Adicione o token no header de todas as requisições:
```
Authorization: Bearer <seu-token>
```

### 3. Exemplos de Requisições

#### Criar Fazenda (Admin)
```bash
POST /api/farms
Authorization: Bearer <token>
Content-Type: application/json

{
  "property": {
    "name": "Fazenda São João",
    "location": "São Paulo, SP",
    "area": 1000.50,
    "description": "Fazenda de soja"
  },
  "ownerName": "João Silva",
  "ownerEmail": "joao@example.com"
}
```

#### Criar Campo (Admin)
```bash
POST /api/fields/farm/{farmId}
Authorization: Bearer <token>
Content-Type: application/json

{
  "property": {
    "name": "Campo Norte",
    "location": "Norte da Fazenda",
    "area": 250.00
  },
  "cropType": "Soja"
}
```

#### Ingestão de Sensor (Admin)
```bash
POST /api/ingestion/single
Authorization: Bearer <token>
Content-Type: application/json

{
  "fieldId": "guid-do-campo",
  "sensorType": "Temperature",
  "value": 25.5,
  "unit": "Celsius",
  "readingTimestamp": "2024-01-20T10:00:00Z",
  "location": "Campo Norte",
  "metadata": {
    "sensorId": "TEMP-001",
    "batteryLevel": "85%"
  }
}
```

#### Ingestão em Lote (Admin)
```bash
POST /api/ingestion/batch
Authorization: Bearer <token>
Content-Type: application/json

{
  "readings": [
    {
      "fieldId": "guid-1",
      "sensorType": "Temperature",
      "value": 25.5,
      "unit": "Celsius",
      "readingTimestamp": "2024-01-20T10:00:00Z"
    },
    {
      "fieldId": "guid-2",
      "sensorType": "Humidity",
      "value": 65.0,
      "unit": "Percent",
      "readingTimestamp": "2024-01-20T10:00:00Z"
    }
  ]
}
```

### 4. Endpoints Disponíveis

#### Autenticação
- `POST /api/auth/login` - Login (público)

#### Usuários (Admin Only)
- `GET /api/users` - Listar usuários
- `GET /api/users/{id}` - Obter usuário
- `POST /api/users` - Criar usuário
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Deletar usuário

#### Fazendas (Admin: CRUD, User: Read)
- `GET /api/farms` - Listar fazendas
- `GET /api/farms/{id}` - Obter fazenda
- `POST /api/farms` - Criar fazenda (Admin)
- `PUT /api/farms/{id}` - Atualizar fazenda (Admin)
- `DELETE /api/farms/{id}` - Deletar fazenda (Admin)

#### Campos (Admin: CRUD, User: Read)
- `GET /api/fields` - Listar campos
- `GET /api/fields/{id}` - Obter campo
- `GET /api/fields/farm/{farmId}` - Campos por fazenda
- `POST /api/fields/farm/{farmId}` - Criar campo (Admin)
- `PUT /api/fields/{id}` - Atualizar campo (Admin)
- `DELETE /api/fields/{id}` - Deletar campo (Admin)

#### Ingestão (Admin Only)
- `POST /api/ingestion/single` - Ingestão única
- `POST /api/ingestion/batch` - Ingestão em lote (sequencial)
- `POST /api/ingestion/batch/parallel` - Ingestão em lote (paralelo)
- `GET /api/ingestion/health` - Health check (público)

#### Health Checks
- `GET /health` - Health check geral
- `GET /health-ui` - Interface visual dos health checks

## 📚 Documentação

### Documentação Adicional
- [TEST_API.md](./TEST_API.md) - Guia detalhado de testes
- [VERIFICATION.md](./VERIFICATION.md) - Checklist de verificação
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Solução de problemas
- [DEPLOYMENT.md](./DEPLOYMENT.md) - Guia de deploy
- [DATABASE.md](./DATABASE.md) - Configuração do banco de dados
- [DDD_CQRS_ARCHITECTURE.md](./DDD_CQRS_ARCHITECTURE.md) - Arquitetura DDD + CQRS
- [REFACTORING_STATUS.md](./REFACTORING_STATUS.md) - Status da refatoração

### Swagger
Acesse `http://localhost:5000` (raiz) para ver a documentação interativa do Swagger.

## 🔐 Segurança

### Autenticação
- JWT tokens com expiração configurável
- Senhas hasheadas com BCrypt
- Validação de email único

### Autorização
- **Admin**: Acesso total (CRUD em todos os recursos)
- **User**: Acesso somente leitura (GET em Farms e Fields)

### Configuração JWT
Edite `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "SuaChaveSecretaSuperSeguraAqui",
    "Issuer": "AgroSolutionsIssuer",
    "Audience": "AgroSolutionsAudience",
    "ExpiryMinutes": 60
  }
}
```

## 🏃 Performance

### Otimizações Implementadas
- ✅ Processamento paralelo para batch ingestion
- ✅ Kestrel configurado para alta concorrência
- ✅ Batch saves no Entity Framework
- ✅ Thread-safe operations

### Configurações de Performance
- Max Concurrent Connections: 1000
- Max Request Body Size: 10MB
- Parallel Processing: Environment.ProcessorCount

## 🧪 Testes

### Executar Testes
```bash
dotnet test
```

### Estrutura de Testes
- `AgroSolutions.Api.Tests` - Testes da API
- `AgroSolutions.Domain.Tests` - Testes de domínio
- `AgroSolutions.Functions.Tests` - Testes das Functions

## 📝 Logging

### Logs Estruturados
Os logs são salvos em:
- **Console**: Output padrão
- **Arquivo**: `logs/agrosolutions-YYYYMMDD.log`

### Níveis de Log
- Development: Information
- Production: Warning (configurável)

## 🐳 Docker

### Build da Imagem
```bash
docker build -t agrosolutions-api .
```

### Executar com Docker Compose
```bash
docker-compose up
```

## 📦 Próximos Passos

- [ ] Testes unitários completos para Handlers
- [ ] Testes de integração
- [ ] Separação em microserviços
- [ ] Cache com Redis
- [ ] Message Queue (RabbitMQ/Azure Service Bus)
- [ ] API Gateway

## 👥 Contribuindo

Este é um projeto acadêmico desenvolvido como MVP. Para contribuições, abra uma issue ou pull request.

## 📄 Licença

Este projeto é parte de um trabalho acadêmico.

---

**Desenvolvido com ❤️ usando .NET 8, DDD e CQRS**
