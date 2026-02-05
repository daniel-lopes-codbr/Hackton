# Migrations - Entity Framework Core

## Criar a migration inicial (do zero)

Na **raiz da solução** (pasta `Hackton`):

```bash
dotnet ef migrations add InitialCreate --context AgroSolutionsDbContext --project src/AgroSolutions.Api --startup-project src/AgroSolutions.Api
```

No **PowerShell (Windows)**:

```powershell
dotnet ef migrations add InitialCreate --context AgroSolutionsDbContext --project src\AgroSolutions.Api --startup-project src\AgroSolutions.Api
```

## Aplicar as migrations no banco

```powershell
dotnet ef database update --context AgroSolutionsDbContext --project src\AgroSolutions.Api --startup-project src\AgroSolutions.Api
```

## Ou: rodar de dentro da pasta da API

```powershell
cd src\AgroSolutions.Api
dotnet ef migrations add InitialCreate --context AgroSolutionsDbContext
dotnet ef database update --context AgroSolutionsDbContext
```

## Remover a última migration

```powershell
dotnet ef migrations remove --context AgroSolutionsDbContext --project src\AgroSolutions.Api --startup-project src\AgroSolutions.Api
```
