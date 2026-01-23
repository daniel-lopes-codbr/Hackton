# Arquitetura DDD + CQRS - Abordagem Profissional

## 🏛️ Fluxo de Dados

```
┌─────────────┐
│  Controller │
└──────┬──────┘
       │
       │ chama
       ▼
┌─────────────┐
│   Service   │  ← Abstração e orquestração
└──────┬──────┘
       │
       │ _mediator.Send(command/query)
       ▼
┌─────────────┐
│   MediatR   │  ← Despacha Commands/Queries
└──────┬──────┘
       │
       │ roteia para
       ▼
┌─────────────┐
│   Handler   │  ← Processa Command/Query
└──────┬──────┘
       │
       │ usa
       ▼
┌─────────────┐
│  Repository │  ← Acesso a dados
└──────┬──────┘
       │
       │ acessa
       ▼
┌─────────────┐
│  Database   │  ← InMemory ou SQL Server
└─────────────┘
```

## 📋 Responsabilidades de Cada Camada

### 1. Controller
- **Responsabilidade**: Receber HTTP requests, validar formato, retornar HTTP responses
- **Não faz**: Lógica de negócio, acesso a dados
- **Faz**: Chama Services, trata erros HTTP, formata respostas

### 2. Service
- **Responsabilidade**: 
  - Orquestrar Commands/Queries via MediatR
  - Lógica adicional (cache, logging, transformações)
  - Coordenar múltiplos Commands/Queries se necessário
- **Não faz**: Acesso direto a Repository (exceto em casos especiais)
- **Faz**: Chama `_mediator.Send()`, pode ter lógica de negócio adicional

### 3. MediatR
- **Responsabilidade**: Despachar Commands/Queries para Handlers corretos
- **Não faz**: Lógica de negócio
- **Faz**: Roteamento, pipeline behaviors (validação, logging)

### 4. Handler
- **Responsabilidade**: Processar um Command ou Query específico
- **Não faz**: Orquestração de múltiplos Commands
- **Faz**: Validações de negócio, chamar Repository, retornar Result

### 5. Repository
- **Responsabilidade**: Abstrair acesso a dados
- **Não faz**: Lógica de negócio
- **Faz**: CRUD operations, queries específicas

## ✅ Vantagens desta Abordagem

1. **Separação de Responsabilidades**: Cada camada tem responsabilidade clara
2. **Testabilidade**: Fácil mockar Services, Handlers, Repositories
3. **Flexibilidade**: Services podem ter lógica adicional sem afetar Handlers
4. **Manutenibilidade**: Mudanças em uma camada não afetam outras
5. **Escalabilidade**: Fácil adicionar cache, logging, etc. nos Services
6. **CQRS**: Commands e Queries separados via MediatR
7. **DDD**: Lógica de domínio nas entidades, orquestração nos Services/Handlers

## 🔄 Exemplo Completo: Create User

### 1. Controller
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
{
    var result = await _userService.CreateUserAsync(dto);
    
    if (!result.IsSuccess)
        return BadRequest(new { errors = result.Errors });
        
    return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
}
```

### 2. Service (usando AutoMapper)
```csharp
public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    
    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
    {
        // Pode adicionar lógica adicional aqui:
        // - Cache check
        // - Logging adicional
        // - Transformações
        // - Orquestração de múltiplos Commands
        
        // Mapear DTO → Command usando AutoMapper (não mapeamento manual!)
        var command = _mapper.Map<CreateUserCommand>(dto);
        
        return await _mediator.Send(command);
    }
}
```

### 3. Command
```csharp
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
```

### 4. Handler
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly NotificationContext _notificationContext;

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validação de negócio
        if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            _notificationContext.AddNotification("Email", "Email already exists");
            return Result<UserDto>.Failure(_notificationContext.Notifications);
        }

        // Criar entidade
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Name, request.Email, passwordHash, request.Role);

        // Salvar
        await _repository.AddAsync(user, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // Mapear e retornar
        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(userDto);
    }
}
```

## 🎯 Conclusão

Esta abordagem mantém Services como camada de orquestração, enquanto usa CQRS via MediatR para separar Commands e Queries. É uma abordagem profissional que combina:

- ✅ **CQRS**: Commands e Queries separados
- ✅ **DDD**: Lógica de domínio nas entidades
- ✅ **Separação de Responsabilidades**: Cada camada tem seu papel
- ✅ **Flexibilidade**: Services podem evoluir sem afetar Handlers
- ✅ **Testabilidade**: Fácil testar cada camada isoladamente
