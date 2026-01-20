# Migrations - Entity Framework Core

## Criar Migration

Para criar uma nova migration:

```bash
cd src/AgroSolutions.Api
dotnet ef migrations add InitialCreate --context AgroSolutionsDbContext
```

## Aplicar Migrations

Para aplicar as migrations ao banco de dados:

```bash
dotnet ef database update --context AgroSolutionsDbContext
```

## Remover Última Migration

Se precisar remover a última migration:

```bash
dotnet ef migrations remove --context AgroSolutionsDbContext
```

## Nota

- Em desenvolvimento, o projeto usa InMemoryDatabase
- Para produção, configure a connection string em `appsettings.json`
- As migrations serão aplicadas automaticamente na primeira execução (se configurado)
