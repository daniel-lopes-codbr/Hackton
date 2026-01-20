# Plano de Fases - Requisitos Funcionais Consolidados

## Status Atual
✅ **FASE 1-5**: Concluídas
- Core do Domínio
- Ingestão de Alta Performance
- Workers & Inteligência
- Observabilidade
- Persistência de Dados

## Próximas Fases Propostas

### FASE 6: Gestão de Farms e Fields (CRUD) 🔄
**Objetivo**: Permitir gerenciamento completo de fazendas e campos através de API REST

**Endpoints a Implementar**:

#### Farms (Fazendas)
- `GET /api/farms` - Listar todas as fazendas (com paginação)
- `GET /api/farms/{id}` - Obter fazenda por ID
- `POST /api/farms` - Criar nova fazenda
- `PUT /api/farms/{id}` - Atualizar fazenda
- `DELETE /api/farms/{id}` - Deletar fazenda

#### Fields (Campos)
- `GET /api/farms/{farmId}/fields` - Listar campos de uma fazenda
- `GET /api/fields` - Listar todos os campos (com filtros)
- `GET /api/fields/{id}` - Obter campo por ID
- `POST /api/farms/{farmId}/fields` - Criar novo campo
- `PUT /api/fields/{id}` - Atualizar campo
- `DELETE /api/fields/{id}` - Deletar campo

**Componentes a Criar**:
- `FarmsController`
- `FieldsController`
- `IFarmService` e `FarmService`
- `IFieldService` e `FieldService`
- `IFarmRepository` e `FarmRepository`
- `IFieldRepository` e `FieldRepository`
- DTOs: `FarmDto`, `CreateFarmDto`, `UpdateFarmDto`, `FieldDto`, `CreateFieldDto`, `UpdateFieldDto`
- Testes unitários

---

### FASE 7: Consulta de Leituras de Sensores 📊
**Objetivo**: Permitir consulta e filtragem avançada de dados de sensores

**Endpoints a Implementar**:
- `GET /api/sensor-readings` - Listar leituras (com filtros: fieldId, sensorType, startDate, endDate)
- `GET /api/sensor-readings/{id}` - Obter leitura por ID
- `GET /api/fields/{fieldId}/readings` - Leituras de um campo específico
- `GET /api/fields/{fieldId}/readings/{sensorType}` - Leituras por tipo de sensor
- `GET /api/fields/{fieldId}/readings/latest` - Última leitura de um campo
- `GET /api/fields/{fieldId}/readings/statistics` - Estatísticas das leituras (média, min, max, count)

**Componentes a Criar**:
- `SensorReadingsController` (expandir ou criar novo)
- `ISensorReadingQueryService` e `SensorReadingQueryService`
- Expandir `ISensorReadingRepository` com métodos de query
- DTOs: `SensorReadingResponseDto`, `ReadingStatisticsDto`
- Query parameters para filtros e paginação
- Testes unitários

---

### FASE 8: Analytics e Relatórios 📈
**Objetivo**: Fornecer análises e insights dos dados coletados

**Endpoints a Implementar**:
- `GET /api/analytics/fields/{fieldId}/trends` - Análise de tendências
- `GET /api/analytics/fields/{fieldId}/anomalies` - Detecção de anomalias
- `GET /api/analytics/farms/{farmId}/summary` - Resumo da fazenda
- `GET /api/analytics/fields/{fieldId}/recommendations` - Recomendações baseadas em dados

**Componentes a Criar**:
- `AnalyticsController`
- Expandir `IAnalyticsService` (já existe nas Functions, criar versão para API)
- DTOs: `TrendAnalysisDto`, `AnomalyDto`, `FarmSummaryDto`, `RecommendationDto`
- Integração com serviços existentes das Functions
- Testes unitários

---

## Estratégia de Implementação

### Princípios
1. ✅ Manter banco em memória (InMemory) por enquanto
2. ✅ Manter estrutura de testes existente
3. ✅ Seguir padrões RESTful
4. ✅ Manter documentação Swagger atualizada
5. ✅ Implementar uma fase por vez, testando antes de avançar

### Ordem de Implementação
1. **FASE 6** - CRUD básico (Farms e Fields)
2. **FASE 7** - Consultas de leituras (expandir funcionalidade existente)
3. **FASE 8** - Analytics (integrar com Functions existentes)

### Notas Técnicas
- Usar repositórios para abstrair acesso a dados
- Manter validações no domínio
- Usar DTOs para entrada/saída da API
- Implementar paginação onde necessário
- Manter logs estruturados
- Adicionar testes unitários para cada componente
