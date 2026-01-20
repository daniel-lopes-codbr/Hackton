# Guia de Banco de Dados - AgroSolutions MVP

## FASE 5: Persistência de Dados

### Configuração

#### Desenvolvimento (InMemory)
Por padrão, em desenvolvimento, o projeto usa InMemory Database. Não é necessário configurar nada.

#### Produção (SQL Server)
Configure a connection string em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AgroSolutions;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Criar Migrations

```bash
cd src/AgroSolutions.Api
dotnet ef migrations add InitialCreate --context AgroSolutionsDbContext
```

### Aplicar Migrations

```bash
dotnet ef database update --context AgroSolutionsDbContext
```

### Estrutura do Banco de Dados

#### Tabelas

1. **Farms**
   - Id (Guid, PK)
   - Property (Owned Entity - Name, Location, Area, Description)
   - OwnerName
   - OwnerEmail
   - OwnerPhone
   - CreatedAt
   - UpdatedAt

2. **Fields**
   - Id (Guid, PK)
   - FarmId (Guid, FK)
   - Property (Owned Entity)
   - CropType
   - PlantingDate
   - HarvestDate
   - CreatedAt
   - UpdatedAt

3. **SensorReadings**
   - Id (Guid, PK)
   - FieldId (Guid, FK, Indexed)
   - SensorType (Indexed)
   - Value
   - Unit
   - ReadingTimestamp (Indexed)
   - Location
   - Metadata (JSON)
   - CreatedAt
   - UpdatedAt
   - Índice composto: (FieldId, SensorType, ReadingTimestamp)

### Repositórios

#### ISensorReadingRepository
- `GetByIdAsync`: Busca por ID
- `GetByFieldIdAsync`: Busca todas as leituras de um campo
- `GetByFieldIdAndSensorTypeAsync`: Busca leituras filtradas por tipo
- `AddAsync`: Adiciona uma leitura
- `AddRangeAsync`: Adiciona múltiplas leituras
- `CountByFieldIdAsync`: Conta leituras por campo
- `SaveChangesAsync`: Salva alterações

### Uso em Desenvolvimento

O projeto usa automaticamente InMemory Database em desenvolvimento. Os dados são perdidos ao reiniciar, mas é ideal para desenvolvimento e testes.

### Uso em Produção

1. Configure a connection string
2. Crie as migrations
3. Aplique as migrations
4. Os dados serão persistidos no SQL Server

### Notas

- Metadata é armazenado como JSON no banco
- Value Objects (Property) são mapeados como Owned Entities
- Índices otimizados para consultas frequentes
- Retry policy configurada para resiliência
