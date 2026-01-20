# Troubleshooting Guide - AgroSolutions MVP

## Problemas Comuns e Soluções

### 1. Erro ao Compilar/Restaurar

**Erro**: `Access to the path '/usr/local/share/dotnet/sdk/.../trustedroots/codesignctl.pem' is denied`

**Solução**: Este é um problema de permissão do sistema. Tente:
```bash
# Limpar cache do NuGet
dotnet nuget locals all --clear

# Tentar restaurar novamente
dotnet restore AgroSolutions.sln
```

### 2. Erro ao Executar a API

**Erro**: `DirectoryNotFoundException: Could not find a part of the path 'logs/agrosolutions-.log'`

**Solução**: A pasta `logs/` precisa existir. Execute:
```bash
mkdir -p logs
cd src/AgroSolutions.Api
dotnet run
```

Ou modifique o `Program.cs` para criar a pasta automaticamente.

### 3. Erro de Namespace/Using

**Erro**: `The type or namespace name 'HealthChecks' could not be found`

**Solução**: Verifique se os pacotes NuGet estão instalados:
```bash
cd src/AgroSolutions.Api
dotnet restore
dotnet build
```

### 4. Health Checks UI não funciona

**Erro**: `MapHealthChecksUI` não encontrado

**Solução**: Adicione o using:
```csharp
using HealthChecks.UI;
```

### 5. Porta já em uso

**Erro**: `Address already in use` ou `Port 5000 is already in use`

**Solução**: 
- Altere a porta em `launchSettings.json`
- Ou mate o processo usando a porta:
```bash
# macOS/Linux
lsof -ti:5000 | xargs kill -9

# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### 6. Problemas com Docker

**Erro**: `Cannot connect to the Docker daemon`

**Solução**: 
- Certifique-se de que o Docker está rodando
- No macOS, abra o Docker Desktop
- Verifique com: `docker ps`

### 7. Erro de Referência de Projeto

**Erro**: `The type or namespace name 'AgroSolutions.Domain' could not be found`

**Solução**: Verifique se as referências estão corretas:
```bash
# Verificar referências
cd src/AgroSolutions.Api
dotnet list reference

# Deve mostrar:
# ProjectReference: ../AgroSolutions.Domain/AgroSolutions.Domain.csproj
```

### 8. Problemas com Serilog

**Erro**: `Serilog` não encontrado ou logs não aparecem

**Solução**: 
- Verifique se a pasta `logs/` existe e tem permissão de escrita
- Verifique se os pacotes Serilog estão instalados no `.csproj`

### 9. Erro ao Executar Testes

**Erro**: Testes não encontram os projetos

**Solução**: Verifique as referências nos projetos de teste:
```bash
cd tests/AgroSolutions.Api.Tests
dotnet list reference

# Deve mostrar:
# ProjectReference: ../../src/AgroSolutions.Api/AgroSolutions.Api.csproj
# ProjectReference: ../../src/AgroSolutions.Domain/AgroSolutions.Domain.csproj
```

### 10. Build Falha Silenciosamente

**Solução**: Execute com mais verbosidade:
```bash
dotnet build AgroSolutions.sln --verbosity detailed
```

## Verificação Rápida

Execute o script de verificação:
```bash
./check-project.sh
```

## Limpar e Reconstruir

Se nada funcionar, tente limpar tudo e reconstruir:

```bash
# Limpar todos os binários e objetos
find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null
find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null

# Limpar cache do NuGet
dotnet nuget locals all --clear

# Restaurar e compilar
dotnet restore AgroSolutions.sln
dotnet build AgroSolutions.sln
```

## Obter Ajuda

Se o problema persistir:
1. Execute `dotnet build AgroSolutions.sln --verbosity detailed` e copie o erro completo
2. Verifique os logs em `logs/agrosolutions-*.log`
3. Consulte a documentação em `DEPLOYMENT.md`
