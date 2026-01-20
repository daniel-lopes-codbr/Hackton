# Análise do PRD - Requisitos Funcionais Consolidados

## Endpoints Atuais Implementados (FASE 1-5)

### Ingestão de Dados
- ✅ `POST /api/ingestion/single` - Ingestão única
- ✅ `POST /api/ingestion/batch` - Ingestão em lote
- ✅ `POST /api/ingestion/batch/parallel` - Ingestão paralela
- ✅ `GET /api/ingestion/health` - Health check

## Endpoints Prováveis do PRD (A Implementar)

Baseado em padrões de APIs RESTful e sistemas de sensores agrícolas, os seguintes endpoints provavelmente estão no PRD:

### 1. Gestão de Farms (Fazendas)
- `GET /api/farms` - Listar todas as fazendas
- `GET /api/farms/{id}` - Obter fazenda por ID
- `POST /api/farms` - Criar nova fazenda
- `PUT /api/farms/{id}` - Atualizar fazenda
- `DELETE /api/farms/{id}` - Deletar fazenda

### 2. Gestão de Fields (Campos)
- `GET /api/farms/{farmId}/fields` - Listar campos de uma fazenda
- `GET /api/fields/{id}` - Obter campo por ID
- `POST /api/farms/{farmId}/fields` - Criar novo campo
- `PUT /api/fields/{id}` - Atualizar campo
- `DELETE /api/fields/{id}` - Deletar campo

### 3. Consulta de Leituras de Sensores
- `GET /api/sensor-readings` - Listar leituras (com filtros)
- `GET /api/sensor-readings/{id}` - Obter leitura por ID
- `GET /api/fields/{fieldId}/readings` - Leituras de um campo
- `GET /api/fields/{fieldId}/readings/{sensorType}` - Leituras por tipo de sensor
- `GET /api/fields/{fieldId}/readings/latest` - Última leitura de um campo
- `GET /api/fields/{fieldId}/readings/statistics` - Estatísticas das leituras

### 4. Analytics e Relatórios
- `GET /api/analytics/fields/{fieldId}/trends` - Análise de tendências
- `GET /api/analytics/fields/{fieldId}/anomalies` - Detecção de anomalias
- `GET /api/analytics/farms/{farmId}/summary` - Resumo da fazenda

## Proposta de Fases

### FASE 6: Gestão de Farms e Fields (CRUD)
**Objetivo**: Permitir gerenciamento completo de fazendas e campos

**Endpoints**:
- CRUD completo de Farms
- CRUD completo de Fields
- Relacionamento Farm -> Fields

**Implementação**:
- Controllers: `FarmsController`, `FieldsController`
- Services: `IFarmService`, `IFieldService`
- Repositories: `IFarmRepository`, `IFieldRepository`
- DTOs para request/response

### FASE 7: Consulta de Leituras de Sensores
**Objetivo**: Permitir consulta e filtragem de dados de sensores

**Endpoints**:
- Listar leituras com filtros (pagination, sorting)
- Consultar leituras por campo
- Consultar leituras por tipo de sensor
- Obter última leitura
- Estatísticas básicas

**Implementação**:
- Controller: `SensorReadingsController`
- Service: `ISensorReadingQueryService`
- Query parameters para filtros
- Pagination e sorting

### FASE 8: Analytics e Relatórios
**Objetivo**: Fornecer análises e insights dos dados coletados

**Endpoints**:
- Análise de tendências
- Detecção de anomalias
- Resumos e dashboards
- Relatórios por período

**Implementação**:
- Controller: `AnalyticsController`
- Service: `IAnalyticsService` (já existe, expandir)
- Integração com Azure Functions existentes

## Notas
- Manter banco em memória (InMemory) por enquanto
- Manter estrutura de testes existente
- Seguir padrões RESTful
- Manter documentação Swagger atualizada
