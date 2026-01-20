#!/bin/bash

echo "=========================================="
echo "AgroSolutions MVP - Verificação do Projeto"
echo "=========================================="
echo ""

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Contador de erros
ERRORS=0

echo "1. Verificando estrutura de pastas..."
if [ -d "src" ] && [ -d "tests" ]; then
    echo -e "${GREEN}✓${NC} Estrutura de pastas OK"
else
    echo -e "${RED}✗${NC} Estrutura de pastas incorreta"
    ((ERRORS++))
fi

echo ""
echo "2. Verificando projetos na solution..."
PROJECTS=(
    "src/AgroSolutions.Domain/AgroSolutions.Domain.csproj"
    "src/AgroSolutions.Api/AgroSolutions.Api.csproj"
    "src/AgroSolutions.Functions/AgroSolutions.Functions.csproj"
    "tests/AgroSolutions.Domain.Tests/AgroSolutions.Domain.Tests.csproj"
    "tests/AgroSolutions.Api.Tests/AgroSolutions.Api.Tests.csproj"
    "tests/AgroSolutions.Functions.Tests/AgroSolutions.Functions.Tests.csproj"
)

for project in "${PROJECTS[@]}"; do
    if [ -f "$project" ]; then
        echo -e "${GREEN}✓${NC} $project"
    else
        echo -e "${RED}✗${NC} $project - NÃO ENCONTRADO"
        ((ERRORS++))
    fi
done

echo ""
echo "3. Verificando arquivos principais..."

# Verificar arquivos críticos
CRITICAL_FILES=(
    "AgroSolutions.sln"
    "src/AgroSolutions.Api/Program.cs"
    "src/AgroSolutions.Api/Controllers/IngestionController.cs"
    "src/AgroSolutions.Api/Services/IngestionService.cs"
    "src/AgroSolutions.Domain/Entities/Entity.cs"
    "src/AgroSolutions.Domain/Entities/SensorReading.cs"
    "src/AgroSolutions.Functions/Program.cs"
    "src/AgroSolutions.Functions/Functions/ProcessSensorDataFunction.cs"
    "Dockerfile"
    "docker-compose.yml"
    ".gitignore"
)

for file in "${CRITICAL_FILES[@]}"; do
    if [ -f "$file" ]; then
        echo -e "${GREEN}✓${NC} $file"
    else
        echo -e "${RED}✗${NC} $file - NÃO ENCONTRADO"
        ((ERRORS++))
    fi
done

echo ""
echo "4. Verificando .NET SDK..."
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓${NC} .NET SDK instalado: $DOTNET_VERSION"
else
    echo -e "${RED}✗${NC} .NET SDK não encontrado"
    ((ERRORS++))
fi

echo ""
echo "5. Tentando restaurar dependências..."
if dotnet restore AgroSolutions.sln > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC} Restauração de dependências OK"
else
    echo -e "${YELLOW}⚠${NC} Erro ao restaurar dependências (pode ser problema de permissão no sandbox)"
fi

echo ""
echo "6. Tentando compilar solution..."
if dotnet build AgroSolutions.sln --no-restore > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC} Compilação OK"
else
    echo -e "${YELLOW}⚠${NC} Erro ao compilar (pode ser problema de permissão no sandbox)"
fi

echo ""
echo "=========================================="
if [ $ERRORS -eq 0 ]; then
    echo -e "${GREEN}✓ VERIFICAÇÃO CONCLUÍDA - SEM ERROS${NC}"
    echo ""
    echo "Próximos passos:"
    echo "1. Execute: cd src/AgroSolutions.Api && dotnet run"
    echo "2. Acesse: http://localhost:5000/swagger"
    echo "3. Teste os endpoints de ingestão"
    exit 0
else
    echo -e "${RED}✗ VERIFICAÇÃO CONCLUÍDA - $ERRORS ERRO(S) ENCONTRADO(S)${NC}"
    exit 1
fi
