# Guia de Deployment - AgroSolutions MVP

## FASE 4: Observabilidade & Entrega Final

Este documento descreve como fazer o deployment da aplicação AgroSolutions.

## Pré-requisitos

- .NET 8.0 SDK
- Docker e Docker Compose (opcional, para containerização)
- Visual Studio 2022 ou VS Code (opcional)

## Estrutura do Projeto

```
AgroSolutions/
├── src/                    # Código-fonte
│   ├── AgroSolutions.Domain/
│   ├── AgroSolutions.Api/
│   └── AgroSolutions.Functions/
├── tests/                  # Testes
├── Dockerfile              # Container da API
├── docker-compose.yml      # Orquestração de containers
└── AgroSolutions.sln      # Solution file
```

## Opção 1: Execução Local

### 1. Restaurar Dependências

```bash
dotnet restore AgroSolutions.sln
```

### 2. Compilar o Projeto

```bash
dotnet build AgroSolutions.sln -c Release
```

### 3. Executar Testes

```bash
dotnet test AgroSolutions.sln
```

### 4. Executar a API

```bash
cd src/AgroSolutions.Api
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`
- Health Check: `http://localhost:5000/health`
- Health Checks UI: `http://localhost:5000/health-ui`

## Opção 2: Docker

### 1. Build da Imagem

```bash
docker build -t agrosolutions-api:latest .
```

### 2. Executar Container

```bash
docker run -d \
  --name agrosolutions-api \
  -p 8080:8080 \
  -v $(pwd)/logs:/app/logs \
  agrosolutions-api:latest
```

### 3. Usar Docker Compose

```bash
docker-compose up -d
```

Para ver os logs:
```bash
docker-compose logs -f
```

Para parar:
```bash
docker-compose down
```

## Opção 3: Azure App Service

### 1. Publicar para Azure

```bash
cd src/AgroSolutions.Api
dotnet publish -c Release -o ./publish
```

### 2. Criar App Service no Azure Portal

1. Acesse o Azure Portal
2. Crie um novo App Service
3. Configure:
   - Runtime: .NET 8
   - Operating System: Linux
   - Region: Escolha a região mais próxima

### 3. Deploy via Azure CLI

```bash
az webapp up \
  --name agrosolutions-api \
  --resource-group <resource-group-name> \
  --runtime "DOTNET|8.0" \
  --os-type Linux
```

## Observabilidade

### Logs

Os logs são gerados em:
- Console (stdout)
- Arquivo: `logs/agrosolutions-YYYYMMDD.log`

### Health Checks

Endpoints disponíveis:
- `/health` - Health check completo
- `/health/ready` - Readiness check
- `/health/live` - Liveness check
- `/health-ui` - Interface visual dos health checks

### Métricas

As seguintes métricas são coletadas:
- Total de leituras processadas
- Taxa de sucesso/falha
- Tempo de processamento
- Uso de recursos

### Swagger/OpenAPI

Documentação da API disponível em:
- `/swagger` (Development)
- `/swagger/v1/swagger.json` (JSON)

## Variáveis de Ambiente

Configure as seguintes variáveis de ambiente:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
UseHttps=false
```

## Monitoramento

### Application Insights (Opcional)

Para adicionar Application Insights:

1. Adicione o pacote:
```xml
<PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.21.0" />
```

2. Configure em `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

3. Configure a connection string em `appsettings.json`:
```json
{
  "ApplicationInsights": {
    "ConnectionString": "your-connection-string"
  }
}
```

## Troubleshooting

### Problemas Comuns

1. **Porta já em uso**
   - Altere a porta em `launchSettings.json` ou via variável de ambiente `ASPNETCORE_URLS`

2. **Erro de permissão no Docker**
   - Execute com `sudo` ou adicione seu usuário ao grupo docker

3. **Logs não aparecem**
   - Verifique permissões na pasta `logs/`
   - Certifique-se de que o diretório existe

4. **Health check falhando**
   - Verifique se o serviço está rodando
   - Consulte os logs para mais detalhes

## Próximos Passos

- [ ] Configurar banco de dados persistente
- [ ] Adicionar autenticação/autorização
- [ ] Configurar CI/CD pipeline
- [ ] Adicionar monitoramento com Application Insights
- [ ] Configurar alertas e notificações

## Suporte

Para suporte, consulte:
- README.md - Documentação geral
- TEST_API.md - Guia de testes
- PRD/ - Product Requirements Document
