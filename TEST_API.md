# Como Testar a API - FASE 2

## Executar a API

```bash
cd AgroSolutions.Api
dotnet run
```

A API estará disponível em: `http://localhost:5000`

## Testar os Endpoints

### 1. Health Check
```bash
curl http://localhost:5000/api/ingestion/health
```

### 2. Ingestão Única
```bash
curl -X POST http://localhost:5000/api/ingestion/single \
  -H "Content-Type: application/json" \
  -d '{
    "fieldId": "123e4567-e89b-12d3-a456-426614174000",
    "sensorType": "Temperature",
    "value": 25.5,
    "unit": "Celsius",
    "readingTimestamp": "2024-01-19T10:00:00Z"
  }'
```

### 3. Ingestão em Lote
```bash
curl -X POST http://localhost:5000/api/ingestion/batch \
  -H "Content-Type: application/json" \
  -d '{
    "readings": [
      {
        "fieldId": "123e4567-e89b-12d3-a456-426614174000",
        "sensorType": "Temperature",
        "value": 25.5,
        "unit": "Celsius",
        "readingTimestamp": "2024-01-19T10:00:00Z"
      },
      {
        "fieldId": "123e4567-e89b-12d3-a456-426614174001",
        "sensorType": "Humidity",
        "value": 60.0,
        "unit": "Percent",
        "readingTimestamp": "2024-01-19T10:00:00Z"
      }
    ]
  }'
```

### 4. Ingestão Paralela (Alta Performance)
```bash
curl -X POST http://localhost:5000/api/ingestion/batch/parallel \
  -H "Content-Type: application/json" \
  -d '{
    "readings": [
      {
        "fieldId": "123e4567-e89b-12d3-a456-426614174000",
        "sensorType": "Temperature",
        "value": 25.5,
        "unit": "Celsius",
        "readingTimestamp": "2024-01-19T10:00:00Z"
      },
      {
        "fieldId": "123e4567-e89b-12d3-a456-426614174001",
        "sensorType": "Humidity",
        "value": 60.0,
        "unit": "Percent",
        "readingTimestamp": "2024-01-19T10:00:00Z"
      }
    ]
  }'
```

## Swagger UI

Após iniciar a API, acesse o Swagger UI em:
- http://localhost:5000/swagger

## Possíveis Problemas

### Erro de Build
Se houver erro de build relacionado a permissões do NuGet, tente:
```bash
dotnet restore --no-cache
dotnet build
```

### Porta já em uso
Se a porta 5000 estiver em uso, você pode alterar em `Properties/launchSettings.json` ou executar:
```bash
dotnet run --urls "http://localhost:5001"
```

### Erro de HTTPS
Se houver erro relacionado a HTTPS, certifique-se de que `UseHttps: false` está em `appsettings.json`
