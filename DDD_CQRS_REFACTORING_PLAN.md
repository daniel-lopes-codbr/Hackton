# Plano de Refatoração: DDD + CQRS

## 📋 Entendimento do Requisito

### Objetivo
Refatorar o código atual para seguir padrões **DDD (Domain-Driven Design)** e **CQRS (Command Query Responsibility Segregation)** antes de avançar para a próxima fase.

### Requisitos
- ✅ Manter banco em memória (InMemory)
- ✅ Manter tudo que já funciona
- ✅ Separar Commands (escrita) de Queries (leitura)
- ✅ Usar MediatR para desacoplar
- ✅ Manter lógica de domínio nas entidades
- ✅ Manter repositórios existentes

---

## 🏗️ Estrutura Proposta

### Nova Estrutura de Pastas

```
src/AgroSolutions.Api/
├── Application/
│   ├── Commands/              # Commands (escrita - mudam estado)
│   │   ├── Users/
│   │   │   ├── CreateUserCommand.cs
│   │   │   ├── UpdateUserCommand.cs
│   │   │   ├── DeleteUserCommand.cs
│   │   │   └── UpdateUserRoleCommand.cs
│   │   ├── Farms/
│   │   │   ├── CreateFarmCommand.cs
│   │   │   ├── UpdateFarmCommand.cs
│   │   │   └── DeleteFarmCommand.cs
│   │   ├── Fields/
│   │   │   ├── CreateFieldCommand.cs
│   │   │   ├── UpdateFieldCommand.cs
│   │   │   └── DeleteFieldCommand.cs
│   │   └── Ingestion/
│   │       ├── IngestSingleReadingCommand.cs
│   │       ├── IngestBatchReadingCommand.cs
│   │       └── IngestBatchParallelReadingCommand.cs
│   │
│   ├── Queries/               # Queries (leitura - não mudam estado)
│   │   ├── Users/
│   │   │   ├── GetUserByIdQuery.cs
│   │   │   ├── GetAllUsersQuery.cs
│   │   │   └── GetUserByEmailQuery.cs
│   │   ├── Farms/
│   │   │   ├── GetFarmByIdQuery.cs
│   │   │   └── GetAllFarmsQuery.cs
│   │   ├── Fields/
│   │   │   ├── GetFieldByIdQuery.cs
│   │   │   ├── GetAllFieldsQuery.cs
│   │   │   └── GetFieldsByFarmIdQuery.cs
│   │   └── SensorReadings/
│   │       └── GetSensorReadingByIdQuery.cs
│   │
│   ├── Handlers/              # Handlers (processam Commands e Queries)
│   │   ├── Commands/
│   │   │   ├── Users/
│   │   │   │   ├── CreateUserCommandHandler.cs
│   │   │   │   ├── UpdateUserCommandHandler.cs
│   │   │   │   └── DeleteUserCommandHandler.cs
│   │   │   ├── Farms/
│   │   │   │   └── ...
│   │   │   └── Fields/
│   │   │       └── ...
│   │   └── Queries/
│   │       ├── Users/
│   │       │   ├── GetUserByIdQueryHandler.cs
│   │       │   └── GetAllUsersQueryHandler.cs
│   │       └── ...
│   │
│   └── Responses/             # Response DTOs (opcional, pode usar DTOs existentes)
│
├── Controllers/               # Controllers (chamam Services)
├── Models/                    # DTOs (mantém como está)
└── Services/                  # Services (usam MediatR internamente)
    ├── IUserService.cs        # Refatorado para usar MediatR
    ├── UserService.cs
    ├── IFarmService.cs
    ├── FarmService.cs
    ├── IFieldService.cs
    ├── FieldService.cs
    ├── IIngestionService.cs   # Refatorado para usar MediatR
    ├── IngestionService.cs
    ├── IAuthService.cs        # Mantém como está
    └── AuthService.cs
```

---

## 🔄 Mudanças Propostas

### 1. Commands (CQRS - Write Side)
**Padrão**: `IRequest<TResponse>`

```csharp
// Exemplo: CreateUserCommand.cs
public class CreateUserCommand : IRequest<UserDto>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
```

### 2. Queries (CQRS - Read Side)
**Padrão**: `IRequest<TResponse>`

```csharp
// Exemplo: GetUserByIdQuery.cs
public class GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; set; }
}
```

### 3. Handlers
**Padrão**: `IRequestHandler<TRequest, TResponse>`

```csharp
// Exemplo: CreateUserCommandHandler.cs
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Lógica de criação
        // Validações
        // Hash de senha
        // Salvar no repositório
        // Retornar DTO
    }
}
```

### 4. Controllers e Services (Abordagem Profissional)
**Antes**:
```csharp
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        return Ok(user);
    }
}
```

**Depois (Controller)**:
```csharp
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });
            
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}
```

**Service (usa MediatR internamente)**:
```csharp
public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserService> _logger;

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
    {
        // Pode adicionar lógica adicional aqui se necessário
        // Ex: cache, logging adicional, transformações, etc.
        
        var command = new CreateUserCommand
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Role = dto.Role
        };
        
        return await _mediator.Send(command);
    }
    
    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var query = new GetUserByIdQuery { Id = id };
        return await _mediator.Send(query);
    }
}
```

---

## 📦 Pacotes Necessários

- `MediatR` - Para CQRS
- `FluentValidation` - Para validações de Commands/Queries
- `AutoMapper` - Para mapeamento DTO ↔ Entity
- `AutoMapper.Extensions.Microsoft.DependencyInjection` - Integração com DI

---

## ✅ Benefícios

1. **Separação de Responsabilidades**: Commands e Queries separados
2. **Desacoplamento**: Controllers não conhecem serviços/repositórios
3. **Testabilidade**: Handlers fáceis de testar isoladamente
4. **Escalabilidade**: Fácil adicionar novos Commands/Queries
5. **DDD**: Lógica de domínio nas entidades, handlers orquestram

---

## 🔄 O Que Será Mantido

- ✅ Repositórios (já seguem DDD)
- ✅ Entidades de Domínio (já seguem DDD)
- ✅ Value Objects (já seguem DDD)
- ✅ DTOs (Models)
- ✅ Controllers (simplificados)
- ✅ InMemory Database
- ✅ Authorization/Authentication
- ✅ Toda funcionalidade existente

---

## 🏛️ Arquitetura de Camadas (Atualizada)

### Abordagem Profissional: Controller → Service → MediatR → Handler

**Fluxo:**
```
Controller → Service → MediatR.Send(Command/Query) → Handler → Repository → Database
```

**Vantagens:**
- ✅ Services encapsulam lógica de negócio complexa
- ✅ Services podem orquestrar múltiplos Commands/Queries
- ✅ Services podem ter lógica adicional (cache, logging, etc.)
- ✅ Controllers ficam mais limpos
- ✅ Abstração adicional (se necessário no futuro)

**Estrutura:**
- **Controllers**: Recebem requests, chamam Services
- **Services**: Orquestram Commands/Queries via MediatR, podem ter lógica adicional
- **Handlers**: Processam Commands/Queries específicos
- **Repositories**: Acesso a dados

### Exemplo:

```csharp
// Controller
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
{
    var result = await _userService.CreateUserAsync(dto);
    if (!result.IsSuccess)
        return BadRequest(result.Errors);
    return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
}

// Service (usando AutoMapper)
public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
    {
        // Pode adicionar lógica adicional aqui (cache, logging, etc.)
        // Mapear DTO → Command usando AutoMapper
        var command = _mapper.Map<CreateUserCommand>(dto);
        
        return await _mediator.Send(command);
    }
}

// Handler
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    // Processa o Command
}
```

---

## 🗑️ O Que Será Refatorado

- ✅ **Services (UserService, FarmService, FieldService, IngestionService)**: 
  - **Manter**, mas refatorar para usar MediatR internamente
  - Services chamam `_mediator.Send(command/query)`
  - Services podem ter lógica adicional de orquestração
- ✅ **AuthService**: **Manter como está** (não é CRUD, é autenticação)
- ❌ **MapToDto manual**: Substituído por AutoMapper
- ❌ **DomainException para validações**: Substituído por Notification Pattern

---

## 🔔 Notification Pattern

**Ao invés de usar Exceptions para validações de negócio, usar Notification Pattern:**

```csharp
// Application/Common/Notifications/Notification.cs
public class Notification
{
    public string Key { get; set; }
    public string Message { get; set; }
}

// Application/Common/Notifications/NotificationContext.cs
public class NotificationContext
{
    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    public bool HasNotifications => _notifications.Any();
    
    public void AddNotification(string key, string message)
    {
        _notifications.Add(new Notification { Key = key, Message = message });
    }
    
    public void AddNotification(Notification notification)
    {
        _notifications.Add(notification);
    }
}
```

**Uso nos Handlers:**
```csharp
if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
{
    notificationContext.AddNotification("Email", $"User with email {request.Email} already exists");
    return null; // ou retornar resultado com erros
}
```

---

## 📝 Exemplo Completo: User

### Command
```csharp
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
```

### FluentValidation
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
            
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(r => r == "Admin" || r == "User").WithMessage("Role must be either 'Admin' or 'User'");
    }
}
```

### Handler com Notification Pattern e AutoMapper
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validações de negócio com Notification Pattern
        if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            _notificationContext.AddNotification("Email", $"User with email {request.Email} already exists");
            return Result<UserDto>.Failure(_notificationContext.Notifications);
        }

        // Criar entidade (geralmente criamos manualmente para ter controle total)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Name, request.Email, passwordHash, request.Role);

        // Salvar
        await _repository.AddAsync(user, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // Mapear Entity → DTO usando AutoMapper
        var userDto = _mapper.Map<UserDto>(user);
        
        _logger.LogInformation("Created user {UserId} with email {Email}", user.Id, user.Email);
        
        return Result<UserDto>.Success(userDto);
    }
}
```

**Nota**: No Handler, geralmente criamos a entidade manualmente (não mapeamos Command → Entity) porque:
- Precisamos de controle total na criação (hash de senha, validações, etc.)
- AutoMapper é usado principalmente para Entity → DTO e DTO → Command

### Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public IReadOnlyCollection<Notification> Errors { get; private set; }

    private Result(bool isSuccess, T? value, IReadOnlyCollection<Notification> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(true, value, Array.Empty<Notification>());
    public static Result<T> Failure(IReadOnlyCollection<Notification> errors) => new(false, default, errors);
}
```

### Query
```csharp
public class GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; set; }
}
```

### Query Handler com AutoMapper
```csharp
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}
```

### Controller e Service (Abordagem Profissional)
```csharp
// Controller
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto);
        
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });
            
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
}

// Service (usando AutoMapper)
public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
    {
        // Mapear DTO → Command usando AutoMapper
        var command = _mapper.Map<CreateUserCommand>(dto);
        
        return await _mediator.Send(command);
    }
    
    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var query = new GetUserByIdQuery { Id = id };
        return await _mediator.Send(query);
    }
}
```

---

## ✅ Decisões Confirmadas

1. ✅ **MediatR**: Usar MediatR para CQRS
2. ✅ **Services**: 
   - Remover: UserService, FarmService, FieldService, IngestionService
   - Manter: AuthService (não é CRUD)
3. ✅ **Validação**: Usar FluentValidation para validações de Commands/Queries
4. ✅ **Mapeamento**: Usar AutoMapper ao invés de MapToDto manual
5. ✅ **Erros**: Usar Notification Pattern ao invés de DomainException para validações de negócio
6. ✅ **Escopo**: Refatorar todos (Users, Farms, Fields, Ingestion)

---

## 📋 Estrutura Detalhada de Commands/Queries

### Users
**Commands:**
- `CreateUserCommand` → `CreateUserCommandHandler`
- `UpdateUserCommand` → `UpdateUserCommandHandler`
- `DeleteUserCommand` → `DeleteUserCommandHandler`
- `UpdateUserRoleCommand` → `UpdateUserRoleCommandHandler`

**Queries:**
- `GetUserByIdQuery` → `GetUserByIdQueryHandler`
- `GetAllUsersQuery` → `GetAllUsersQueryHandler`
- `GetUserByEmailQuery` → `GetUserByEmailQueryHandler`

### Farms
**Commands:**
- `CreateFarmCommand` → `CreateFarmCommandHandler`
- `UpdateFarmCommand` → `UpdateFarmCommandHandler`
- `DeleteFarmCommand` → `DeleteFarmCommandHandler`

**Queries:**
- `GetFarmByIdQuery` → `GetFarmByIdQueryHandler`
- `GetAllFarmsQuery` → `GetAllFarmsQueryHandler`

### Fields
**Commands:**
- `CreateFieldCommand` → `CreateFieldCommandHandler`
- `UpdateFieldCommand` → `UpdateFieldCommandHandler`
- `DeleteFieldCommand` → `DeleteFieldCommandHandler`

**Queries:**
- `GetFieldByIdQuery` → `GetFieldByIdQueryHandler`
- `GetAllFieldsQuery` → `GetAllFieldsQueryHandler`
- `GetFieldsByFarmIdQuery` → `GetFieldsByFarmIdQueryHandler`

### Ingestion (Sensor Readings)
**Commands:**
- `IngestSingleReadingCommand` → `IngestSingleReadingCommandHandler`
- `IngestBatchReadingCommand` → `IngestBatchReadingCommandHandler`
- `IngestBatchParallelReadingCommand` → `IngestBatchParallelReadingCommandHandler`

**Queries:**
- `GetSensorReadingByIdQuery` → `GetSensorReadingByIdQueryHandler`
- `GetSensorReadingsByFieldIdQuery` → `GetSensorReadingsByFieldIdQueryHandler`

---

## 🔧 AutoMapper Profiles

**AutoMapper será usado para TODOS os mapeamentos:**
- DTO → Command (no Service)
- Command → Entity (no Handler, se necessário)
- Entity → DTO (no Handler)

```csharp
// Application/Mappings/UserMappingProfile.cs
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // DTO → Command (usado no Service)
        CreateMap<CreateUserDto, CreateUserCommand>();
        CreateMap<UpdateUserDto, UpdateUserCommand>();
        
        // Entity → DTO (usado no Handler)
        CreateMap<User, UserDto>();
        
        // Command → Entity (usado no Handler, se necessário)
        // Nota: Geralmente criamos a entidade manualmente no Handler
        // mas podemos mapear se os campos forem compatíveis
    }
}

// Application/Mappings/FarmMappingProfile.cs
public class FarmMappingProfile : Profile
{
    public FarmMappingProfile()
    {
        // DTO → Command
        CreateMap<CreateFarmDto, CreateFarmCommand>();
        CreateMap<UpdateFarmDto, UpdateFarmCommand>();
        
        // Entity → DTO
        CreateMap<Farm, FarmDto>();
        CreateMap<Property, PropertyDto>(); // Value Object
    }
}

// Application/Mappings/FieldMappingProfile.cs
public class FieldMappingProfile : Profile
{
    public FieldMappingProfile()
    {
        CreateMap<CreateFieldDto, CreateFieldCommand>();
        CreateMap<UpdateFieldDto, UpdateFieldCommand>();
        CreateMap<Field, FieldDto>();
    }
}

// Application/Mappings/IngestionMappingProfile.cs
public class IngestionMappingProfile : Profile
{
    public IngestionMappingProfile()
    {
        CreateMap<SensorReadingDto, IngestSingleReadingCommand>();
        CreateMap<SensorReading, SensorReadingDto>();
    }
}
```

---

## ✅ Próximos Passos de Implementação

1. ✅ Instalar pacotes: MediatR, FluentValidation, AutoMapper
2. ✅ Criar estrutura de pastas (Application/Commands, Queries, Handlers)
3. ✅ Criar Notification Pattern (Notification, NotificationContext, Result<T>)
4. ✅ Criar AutoMapper Profiles
5. ✅ Refatorar Users (Commands, Queries, Handlers, Validators)
6. ✅ Refatorar Farms (Commands, Queries, Handlers, Validators)
7. ✅ Refatorar Fields (Commands, Queries, Handlers, Validators)
8. ✅ Refatorar Ingestion (Commands, Queries, Handlers, Validators)
9. ✅ **Refatorar Services** para usar MediatR internamente
10. ✅ Atualizar Controllers para usar Services (que usam MediatR)
11. ✅ Configurar MediatR, FluentValidation e AutoMapper no Program.cs
12. ✅ Testar tudo

**Nota**: Services serão mantidos e refatorados para usar MediatR internamente, mantendo a interface atual dos Services.

---

## 🎯 Resumo Final

### Tecnologias
- ✅ **MediatR**: CQRS pattern
- ✅ **FluentValidation**: Validação de Commands/Queries
- ✅ **AutoMapper**: Mapeamento DTO ↔ Entity
- ✅ **Notification Pattern**: Tratamento de erros de negócio (ao invés de Exceptions)

### Estrutura
- ✅ **Commands**: Operações de escrita (Create, Update, Delete)
- ✅ **Queries**: Operações de leitura (Get, GetAll)
- ✅ **Handlers**: Processam Commands/Queries
- ✅ **Validators**: FluentValidation para cada Command/Query

### Refatoração
- ✅ **Refatorar Services**: UserService, FarmService, FieldService, IngestionService
  - Services mantidos, mas agora usam MediatR internamente
  - Services chamam `_mediator.Send(command/query)`
- ✅ **Manter**: AuthService (não é CRUD, mantém como está)

### Padrões
- ✅ **Notification Pattern**: Para erros de validação de negócio
- ✅ **Result Pattern**: Para retornos de Commands
- ✅ **AutoMapper**: Para mapeamento automático

---

**Plano completo e pronto para implementação!** 🚀
