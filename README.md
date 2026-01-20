# AgroSolutions MVP

Projeto desenvolvido em fases conforme PRD.

## Estrutura do Projeto

```
AgroSolutions/
├── src/                           # Código-fonte dos projetos
│   ├── AgroSolutions.Domain/      # Core do Domínio (FASE 1)
│   │   ├── Entities/             # Entidades de domínio
│   │   ├── ValueObjects/          # Objetos de valor
│   │   ├── Exceptions/            # Exceções de domínio
│   │   ├── Data/                  # DbContext (FASE 5)
│   │   └── Repositories/          # Repositórios (FASE 5)
│   ├── AgroSolutions.Api/         # API de Ingestão (FASE 2)
│   │   ├── Controllers/           # Controllers da API
│   │   ├── Services/              # Serviços de ingestão
│   │   ├── Models/                # DTOs
│   │   ├── HealthChecks/          # Health checks (FASE 4)
│   │   └── Migrations/             # EF Core Migrations (FASE 5)
│   └── AgroSolutions.Functions/   # Workers & Inteligência (FASE 3)
│       ├── Functions/             # Azure Functions
│       └── Services/              # Serviços de processamento e analytics
├── tests/                         # Projetos de testes
│   ├── AgroSolutions.Domain.Tests/    # Testes do domínio
│   ├── AgroSolutions.Api.Tests/       # Testes da API
│   └── AgroSolutions.Functions.Tests/ # Testes das Functions
├── PRD/                           # Documentação do projeto
└── AgroSolutions.sln              # Solution file
```

## FASE 1: Core do Domínio (Identity & Properties) ✅

### Implementado:

1. **Entity (Identity)**
   - Classe base `Entity` com `Id` (Guid)
   - Timestamps: `CreatedAt` e `UpdatedAt`
   - Métodos de igualdade e hash code

2. **Property (Value Object)**
   - Representa propriedades rurais (fazendas/campos)
   - Validações: nome, localização, área
   - Imutável e comparável

3. **Farm (Entidade)**
   - Representa uma fazenda
   - Possui uma `Property` e informações do proprietário
   - Métodos para atualização

4. **Field (Entidade)**
   - Representa um campo dentro de uma fazenda
   - Possui uma `Property` e informações de cultivo
   - Relacionado a uma `Farm` via `FarmId`

### Testes

Todos os componentes possuem testes unitários cobrindo:
- Criação de entidades
- Validações
- Atualizações
- Comparações e igualdade

## FASE 2: Ingestão de Alta Performance ✅

### Implementado:

1. **SensorReading (Entidade)**
   - Representa leituras de sensores agrícolas
   - Suporta múltiplos tipos de sensores (temperatura, umidade, umidade do solo, etc.)
   - Inclui metadados e localização

2. **API de Ingestão**
   - **POST `/api/ingestion/single`**: Ingestão de leitura única
   - **POST `/api/ingestion/batch`**: Ingestão em lote (sequencial)
   - **POST `/api/ingestion/batch/parallel`**: Ingestão em lote paralela (alta performance)
   - **GET `/api/ingestion/health`**: Health check

3. **Otimizações de Performance**
   - Processamento paralelo com `Task.Run` e `SemaphoreSlim`
   - Limites de Kestrel configurados para alta concorrência
   - Processamento em lote otimizado
   - Thread-safe operations

4. **Serviços**
   - `IIngestionService`: Interface para ingestão
   - `IngestionService`: Implementação com suporte a batch e paralelismo

### Características de Performance:

- **Ingestão Sequencial**: Processa leituras uma por uma
- **Ingestão Paralela**: Processa múltiplas leituras simultaneamente usando todos os cores disponíveis
- **Thread-Safe**: Operações seguras para processamento concorrente
- **Error Handling**: Tratamento robusto de erros sem interromper o processamento do lote

### Testes

Testes unitários cobrindo:
- Ingestão única
- Ingestão em lote
- Ingestão paralela
- Tratamento de erros
- Validações

## Como Testar

```bash
# Compilar o projeto
dotnet build AgroSolutions.sln

# Executar testes
dotnet test AgroSolutions.sln

# Executar a API
cd AgroSolutions.Api
dotnet run
```

### Endpoints da API

#### Ingestão Única
```bash
POST /api/ingestion/single
Content-Type: application/json

{
  "fieldId": "guid",
  "sensorType": "Temperature",
  "value": 25.5,
  "unit": "Celsius",
  "readingTimestamp": "2024-01-19T10:00:00Z"
}
```

#### Ingestão em Lote
```bash
POST /api/ingestion/batch
Content-Type: application/json

{
  "readings": [
    {
      "fieldId": "guid",
      "sensorType": "Temperature",
      "value": 25.5,
      "unit": "Celsius",
      "readingTimestamp": "2024-01-19T10:00:00Z"
    },
    ...
  ]
}
```

#### Ingestão Paralela (Alta Performance)
```bash
POST /api/ingestion/batch/parallel
Content-Type: application/json

{
  "readings": [...]
}
```

## FASE 3: Workers & Inteligência (Azure Functions) ✅

### Implementado:

1. **Azure Functions Project**
   - Projeto `AgroSolutions.Functions` configurado para Azure Functions
   - Suporte para HTTP triggers
   - Configuração de host.json e local.settings.json

2. **Data Processing Service**
   - `IDataProcessingService`: Interface para processamento de dados
   - `DataProcessingService`: Implementação com:
     - **Detecção de Anomalias**: Identifica valores fora dos thresholds normais
     - **Normalização de Valores**: Converte unidades (ex: Fahrenheit para Celsius)
     - **Geração de Insights**: Cria recomendações baseadas nos dados
     - **Processamento em Lote**: Processa múltiplas leituras eficientemente

3. **Analytics Service**
   - `IAnalyticsService`: Interface para análise de dados
   - `AnalyticsService`: Implementação com:
     - **Análise de Tendências**: Detecta se valores estão aumentando, diminuindo ou estáveis
     - **Estatísticas**: Calcula média, mínimo, máximo e contagem
     - **Armazenamento em Memória**: Mantém histórico de leituras para análise

4. **Azure Functions**
   - **ProcessSensorReading**: HTTP trigger para processar leitura única
     - `POST /api/process/reading`
   - **ProcessSensorBatch**: HTTP trigger para processar lote de leituras
     - `POST /api/process/batch`

5. **Inteligência Implementada**
   - **Detecção de Anomalias**: Thresholds configuráveis por tipo de sensor
   - **Recomendações Automáticas**: Sugestões baseadas em valores detectados
   - **Análise de Tendências**: Identifica padrões nos dados
   - **Normalização**: Converte diferentes unidades para comparação

### Características de Inteligência:

- **Anomaly Detection**: Detecta valores fora do normal (temperatura, umidade, etc.)
- **Trend Analysis**: Identifica se valores estão aumentando, diminuindo ou estáveis
- **Smart Recommendations**: Gera recomendações baseadas em condições detectadas
- **Statistics**: Calcula estatísticas descritivas dos dados

### Testes

Testes unitários cobrindo:
- Processamento de leituras normais e anômalas
- Detecção de anomalias
- Normalização de valores
- Análise de tendências
- Cálculo de estatísticas

## FASE 4: Observabilidade & Entrega Final ✅

### Implementado:

1. **Logging Estruturado (Serilog)**
   - Logging estruturado com Serilog
   - Logs em console e arquivo (rolling daily)
   - Enriquecimento com contexto (Environment, Machine, Thread)
   - Request logging middleware
   - Logs formatados em JSON

2. **Health Checks Avançados**
   - Health check básico (`/health`)
   - Readiness check (`/health/ready`)
   - Liveness check (`/health/live`)
   - Health check customizado para serviço de ingestão
   - Health Checks UI (`/health-ui`) com interface visual
   - Métricas de saúde do sistema

3. **Telemetria e Monitoramento**
   - Application Insights configurado nas Functions
   - Métricas de performance
   - Rastreamento de requisições
   - Coleta de dados de telemetria

4. **Containerização**
   - Dockerfile multi-stage para otimização
   - docker-compose.yml para orquestração
   - .dockerignore configurado
   - Suporte para deployment em containers

5. **Documentação de Deployment**
   - Guia completo de deployment (DEPLOYMENT.md)
   - Instruções para execução local
   - Instruções para Docker
   - Instruções para Azure App Service
   - Troubleshooting guide

### Características de Observabilidade:

- **Structured Logging**: Logs estruturados com Serilog
- **Health Monitoring**: Health checks completos com UI
- **Request Tracking**: Middleware de logging de requisições
- **Metrics Collection**: Coleta de métricas de performance
- **Container Ready**: Pronto para deployment em containers

### Endpoints de Observabilidade:

- `/health` - Health check completo
- `/health/ready` - Readiness check
- `/health/live` - Liveness check
- `/health-ui` - Interface visual dos health checks
- `/swagger` - Documentação da API

### Deployment:

Consulte `DEPLOYMENT.md` para instruções detalhadas de deployment.

## FASE 5: Persistência de Dados (Entity Framework Core) ✅

### Implementado:

1. **Entity Framework Core**
   - EF Core 8.0 configurado no projeto Domain
   - Suporte para SQL Server e InMemory Database
   - Configuração de DbContext com mapeamento completo
   - Conversão de tipos complexos (Dictionary para JSON)

2. **AgroSolutionsDbContext**
   - DbContext configurado para todas as entidades
   - Configuração de relacionamentos (Farm -> Field)
   - Índices otimizados para consultas
   - Value Objects mapeados como Owned Entities

3. **Repositórios**
   - `ISensorReadingRepository`: Interface do repositório
   - `SensorReadingRepository`: Implementação com EF Core
   - Métodos para consultas otimizadas
   - Suporte a operações em lote

4. **Atualização de Serviços**
   - `IngestionService` atualizado para usar repositório
   - Persistência real de dados (não mais em memória)
   - Operações batch otimizadas
   - Transações gerenciadas pelo EF Core

5. **Configuração de Banco de Dados**
   - InMemory Database para desenvolvimento
   - SQL Server para produção
   - Connection string configurável
   - Retry policy configurada

6. **Testes Atualizados**
   - Testes atualizados para usar InMemory Database
   - Isolamento de testes com DbContext separado
   - Testes de integração com banco

### Características de Persistência:

- **EF Core**: ORM completo com migrations
- **Repository Pattern**: Abstração de acesso a dados
- **InMemory para Dev**: Desenvolvimento sem necessidade de SQL Server
- **SQL Server para Prod**: Pronto para produção
- **Migrations**: Suporte a versionamento de schema

### Configuração:

- **Development**: Usa InMemory Database automaticamente
- **Production**: Configurar connection string em `appsettings.json`
- **Migrations**: Criar com `dotnet ef migrations add InitialCreate`

### Status do Projeto

✅ **FASE 1**: Core do Domínio - Concluída
✅ **FASE 2**: Ingestão de Alta Performance - Concluída
✅ **FASE 3**: Workers & Inteligência - Concluída
✅ **FASE 4**: Observabilidade & Entrega Final - Concluída
✅ **FASE 5**: Persistência de Dados - Concluída

**MVP Completo com Persistência de Dados!** 🎉
