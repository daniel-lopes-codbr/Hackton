# Plano de Implementação - User & Authentication

## 📋 Entendimento do Requisito

### 1. User Entity e CRUD
- **Fonte**: PRD - "Modelo de Dados (Entidades de Domínio)"
- **Requisito**: Criar entidade User com campos específicos do PRD
- **Endpoints necessários**:
  - `GET /api/users` - Listar todos os usuários
  - `GET /api/users/{id}` - Obter usuário por ID
  - `POST /api/users` - Criar novo usuário
  - `PUT /api/users/{id}` - Atualizar usuário
  - `DELETE /api/users/{id}` - Deletar usuário

### 2. Authorization & Authentication
- **Geração de Token**: JWT (JSON Web Token)
- **Permissões**:
  - **Admin**: Pode executar qualquer endpoint (GET, POST, PUT, DELETE)
  - **User**: Pode executar apenas endpoints de leitura (GET)
- **Funcionalidades**:
  - [ ] Gerar token (login/autenticação)
  - [ ] Adicionar permissão de usuário (Admin ou User)
  - [ ] Atualizar permissão
  - [ ] Testes unitários

### 3. Arquitetura de Microserviços (Futuro)
**Estrutura planejada**:
- **Microserviço 1**: Tudo relacionado a `IngestionController` (Fases 1-5: Core Domain + Ingestão de Dados)
- **Microserviço 2**: Tudo relacionado a `FarmsController` e `FieldsController` (Fase 6: CRUD de Farms/Fields)
- **Microserviço 3**: Tudo relacionado a User e Authentication (Nova Fase: User Management & Auth)

**Estratégia atual**: 
- ✅ Manter tudo na mesma solution por enquanto
- ✅ Separar em microserviços depois de testar tudo
- ✅ Estruturar código pensando na separação futura
- ✅ Cada microserviço terá seu próprio contexto de domínio quando separado

### 4. Documentação
**Opções**:
1. Adicionar ao PRD (PDF - difícil de editar)
2. Criar novo arquivo `.md` com tudo que foi feito até agora + novos requisitos

**Recomendação**: Criar `IMPLEMENTATION_LOG.md` ou `PROJECT_STATUS.md` que:
- Documenta tudo implementado (Fases 1-6)
- Documenta requisitos novos (User + Auth)
- Serve como referência para separação em microserviços

---

## 🎯 Proposta de Implementação

### FASE 6.5: User Management & Authentication

#### Componentes a Criar:

1. **Domain Layer**:
   - `User` entity com campos:
     - `Guid Id` (herdado de `Entity`)
     - `string Name`
     - `string Email`
     - `string PasswordHash`
     - `string Role` (Admin ou User)
   - Enum `UserRole` (Admin, User)
   - `IUserRepository` e `UserRepository`
   - Atualizar `AgroSolutionsDbContext` com `DbSet<User>`

2. **API Layer**:
   - `UserDto`, `CreateUserDto`, `UpdateUserDto`
   - `LoginDto`, `TokenResponseDto`
   - `IUserService` e `UserService` (com hash de senha)
   - `IAuthService` e `AuthService` (JWT + hash verification)
   - `UsersController` (CRUD completo)
   - `AuthController` (Login, Token generation)

3. **Authorization**:
   - JWT Authentication middleware configurado
   - Authorization policies:
     - `Admin`: Pode executar qualquer endpoint (GET, POST, PUT, DELETE)
     - `User`: Pode executar apenas endpoints de leitura (GET)
   - Attributes: `[Authorize]`, `[Authorize(Roles = "Admin")]`
   - Aplicar authorization nos controllers:
     - `IngestionController`: Admin para POST, User para GET (se houver)
     - `FarmsController`: Admin para POST/PUT/DELETE, User para GET
     - `FieldsController`: Admin para POST/PUT/DELETE, User para GET

4. **Password Security**:
   - Usar BCrypt ou similar para hash de senhas
   - Hash na criação/atualização de usuário
   - Verificação de hash no login

5. **Testes**:
   - Testes unitários para User CRUD
   - Testes unitários para Authentication (login, token)
   - Testes de autorização (Admin vs User)
   - Testes de hash de senha

---

## 📝 Estrutura de Documentação Proposta

### Arquivo: `PROJECT_STATUS.md` (Novo)

**Conteúdo**:
1. **Status Geral do Projeto**
   - Fases concluídas (1-6)
   - Fase atual (6.5 - User & Auth)
   - Próximas fases (7, 8)

2. **Microserviços Planejados**
   - Mapeamento: Fases → Microserviços
   - Dependências entre serviços
   - Estratégia de separação

3. **Requisitos do PRD**
   - User Entity (campos específicos)
   - Authorization requirements
   - Endpoints necessários

4. **Decisões Técnicas**
   - JWT para autenticação
   - Role-based authorization
   - Estrutura atual vs. futura

---

## ✅ Informações Confirmadas

1. **User Entity - Campos**:
   - `Guid Id` (herdado de `Entity`)
   - `string Name`
   - `string Email`
   - `string PasswordHash` (senha com hash)

2. **Arquitetura de Microserviços**:
   - **Microserviço 1**: `IngestionController` (Fases 1-5)
   - **Microserviço 2**: `FarmsController` + `FieldsController` (Fase 6)
   - **Microserviço 3**: User & Authentication (Nova Fase)

3. **Password Management**:
   - ✅ Hash de senha (BCrypt ou similar)
   - ✅ Armazenar `PasswordHash` no banco

4. **Documentação**:
   - ✅ Criar `PROJECT_STATUS.md` para documentar tudo

---

## 📊 Mapeamento de Microserviços

### Microserviço 1: Data Ingestion Service
**Controllers**:
- `IngestionController` (Fases 1-5)

**Domain**:
- `SensorReading` entity
- `ISensorReadingRepository`

**Services**:
- `IIngestionService`

**Quando separar**: Tudo relacionado à ingestão de dados de sensores

---

### Microserviço 2: Farm Management Service
**Controllers**:
- `FarmsController` (Fase 6)
- `FieldsController` (Fase 6)

**Domain**:
- `Farm` entity
- `Field` entity
- `IFarmRepository`, `IFieldRepository`

**Services**:
- `IFarmService`
- `IFieldService`

**Quando separar**: Tudo relacionado ao gerenciamento de fazendas e campos

---

### Microserviço 3: User & Authentication Service
**Controllers**:
- `UsersController` (Nova Fase)
- `AuthController` (Nova Fase)

**Domain**:
- `User` entity (Id, Name, Email, PasswordHash, Role)
- `IUserRepository`

**Services**:
- `IUserService`
- `IAuthService` (JWT)

**Quando separar**: Tudo relacionado a usuários e autenticação/autorização

---

## ✅ Próximos Passos (Aguardando Confirmação Final)

1. ✅ Atualizar `IMPLEMENTATION_PLAN.md` com informações corretas
2. Criar `PROJECT_STATUS.md` com toda a documentação
3. Implementar User Entity (Id, Name, Email, PasswordHash, Role)
4. Implementar CRUD de User (com hash de senha)
5. Implementar JWT Authentication
6. Implementar Authorization (Admin/User)
7. Aplicar authorization nos endpoints existentes
8. Criar testes unitários

---

**Aguardando sua confirmação final antes de começar a implementação!** 🚀

**Por favor, confirme se está tudo correto:**
- ✅ Microserviços mapeados corretamente
- ✅ Campos do User corretos
- ✅ Estratégia de hash de senha
- ✅ Estrutura de documentação
