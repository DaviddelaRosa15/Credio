# Credio 🏦

**Plataforma Integral de Gestión de Créditos y Cobranza**

![Estado](https://img.shields.io/badge/Estado-En%20Desarrollo-orange)
![Licencia](https://img.shields.io/badge/Licencia-MIT-blue)
![Database](https://img.shields.io/badge/BD-PostgreSQL-336791)

## 📋 Descripción General

**Credio** es una plataforma SaaS de gestión de préstamos diseñada para instituciones de microfinanzas. Implementa un **core bancario robusto** capaz de ejecutar operaciones financieras complejas: cálculos de amortización, distribución de pagos en cascada, detección de mora, reamortización automática y servicio omnicanal (Web, Móvil, Bots).

El sistema va más allá de un simple CRUD: centraliza el ciclo completo del crédito desde la originación, aprobación y desembolso, hasta la cobranza en campo con geolocalización y generación de comprobantes digitales.

---

## 🏗️ Arquitectura

### Principios de Diseño

Credio implementa **Clean Architecture** con **CQRS (Command Query Responsibility Segregation)** para garantizar:

- ✅ **Separación de capas**: Cada capa tiene responsabilidades definidas
- ✅ **Desacoplamiento**: Controladores ↔ Lógica de negocio ↔ Persistencia
- ✅ **Testabilidad**: Inyección de dependencias y patrones CQRS
- ✅ **Escalabilidad**: Eventos de dominio para operaciones asincrónicas

### Capas del Proyecto

```
┌─────────────────────────────────────────────────────────────┐
│           Interface Layer (Presentation)                     │
│   - Credio.Authentication.Api  (Gestión de Usuarios)        │
│   - Credio.Lending.Api         (Gestión de Créditos)        │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│           Core.Application Layer                             │
│   - Features (Commands & Queries)                           │
│   - DTOs & Mappings                                         │
│   - Business Logic & Validations                            │
│   - Pipeline Behaviors (MediatR)                            │
│   - Services (Amortization, Email, etc.)                    │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│           Core.Domain Layer                                 │
│   - Entities & Aggregates                                   │
│   - Value Objects                                           │
│   - Domain Events                                           │
│   - Business Rules (Invariants)                             │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│           Infrastructure Layers                             │
│   - Persistence (EF Core, Repositories, DB Migrations)      │
│   - Identity (JWT, Role-Based Authorization)                │
│   - Shared (Email, Logging, ExternalServices)               │
│   - Event Handling (Domain Event Publishing)                │
└─────────────────────────────────────────────────────────────┘
```

### Patrón CQRS + MediatR

```csharp
// Endpoint: HTTP Request → Controller (thin)
[Authorize(Roles = "Administrator, Officer")]
[HttpPost("create")]
public async Task<IResult> CreateLoan([FromBody] CreateLoanCommand command, CancellationToken ct)
{
    Result<LoanDTO> result = await _sender.Send(command, ct);
    return result.Match(onSuccess: CustomResult.Success, onFailure: CustomResult.Problem);
}

// Command → Pipeline Behaviors → Handler (business logic)
public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, Result<LoanDTO>>
{
    // Repositories, Validators, Services injected here
    public async Task<Result<LoanDTO>> Handle(CreateLoanCommand request, CancellationToken ct)
    {
        // Business logic, event publishing
    }
}
```

---

## 🚀 Módulos Principales

### 📊 **Motor de Amortización** (`IAmortizationCalculatorService`)

- Cálculo automático de tablas de pago (Método Francés)
- Soporte para simulaciones "al vuelo"
- Precisión decimal (5 decimales) para integridad contable

### 💳 **Gestión de Préstamos** (`Loan Module`)

- Crear y validar solicitudes de préstamo
- Desembolso con ajuste automático de fechas
- Creación de registros de balance inicial
- Consulta de cronogramas de amortización

### 💰 **Motor de Pagos** (Cascada inteligente)

```
Pago Recibido
    ↓
1. Pagar Mora (si existe)
    ↓ (resto)
2. Pagar Intereses
    ↓ (resto)
3. Pagar Capital
    ↓
Abono Extraordinario → Reamortización Automática
```

### 👥 **Gestión de Clientes**

- Registro y validación de datos
- Gestión de perfiles y contacto
- Historial de transacciones

### 👨‍💼 **Gestión de Empleados**

- Registros de oficiales de cobro
- Asignación de rutas
- Tracking de desempeño

### 📞 **Bot de Auto-servicio** (`BotController`)

- Endpoints optimizados para consultas rápidas
- Integración con WhatsApp/Telegram
- Response times < 100ms

### 📈 **Dashboard y Reportes** (`DashboardController`, `ReportController`)

- KPIs de cobranza
- Estado de cartera
- Reportes de liquidez

### 📋 **Catálogo** (`CatalogController`)

- Tasas de interés configurables
- Tipos de productos
- Parámetros del negocio

---

## 🛠️ Stack Tecnológico

| Componente            | Tecnología                    | Versión |
| --------------------- | ----------------------------- | ------- |
| **Backend**           | .NET / ASP.NET Core           | 8.0     |
| **Lenguaje**          | C#                            | 12      |
| **Base de Datos**     | PostgreSQL                    | 12+     |
| **ORM**               | Entity Framework Core         | 8.0     |
| **Messaging**         | MediatR                       | 12.x    |
| **Validación**        | FluentValidation              | 11.x    |
| **Mapeo de Objetos**  | AutoMapper                    | 13.x    |
| **JWT & Seguridad**   | Microsoft.AspNetCore.Identity | 8.0     |
| **Documentación API** | Swagger/Swashbuckle           | 6.x     |
| **Logging**           | Serilog (opcional)            | -       |
| **Contenedores**      | Docker                        | 24+     |

### Pipeline Behaviors (MediatR)

- ✅ **Validación**: FluentValidation automática
- ✅ **Logging**: Registro de requests/responses
- ✅ **Performance**: Medición de tiempos de ejecución
- ✅ **Transacciones**: Manejo automático de Unit of Work
- ✅ **Excepciones**: Global error handling
- ✅ **Caché**: Query caching para optimización

---

## 📁 Estructura de Directorios

```
Credio/
├── Credio.Core.Domain/                    # Entidades, contratos, eventos
│   ├── Entities/                          # Agregados (Loan, Client, Payment, etc.)
│   ├── Events/                            # Domain Events
│   ├── Contracts/                         # Interfaces del dominio
│   └── Settings/                          # Configuraciones globales
│
├── Credio.Core.Application/               # Lógica de negocio
│   ├── Features/
│   │   ├── Loan/                          # Queries y Commands de préstamos
│   │   ├── LoanApplication/               # Solicitudes de préstamo
│   │   ├── Client/                        # Gestión de clientes
│   │   ├── Employee/                      # Gestión de empleados
│   │   ├── AmortizationSchedule/          # Cronogramas
│   │   ├── Catalog/                       # Catálogos
│   │   └── Account/                       # Cuentas y balances
│   ├── Services/
│   │   ├── AmortizationCalculatorService/ # Motor de amortización
│   │   └── ...
│   ├── Dtos/                              # Modelos de transferencia
│   ├── Validators/                        # Validadores FluentValidation
│   ├── Mappings/                          # AutoMapper profiles
│   └── Common/
│       └── Pipelines/                     # MediatR behaviors
│
├── Credio.Infrastructure.Persistence/    # Acceso a datos
│   ├── Contexts/                          # DbContext
│   ├── Repositories/                      # Implementación del patrón
│   ├── Migrations/                        # EF Core migrations
│   ├── Interceptors/                      # AuditableEntity, SoftDelete
│   └── Seeds/                             # Datos iniciales
│
├── Credio.Infrastructure.Identity/       # Autenticación y autorización
│   ├── Entities/                          # Usuarios, roles
│   ├── Services/                          # JWT, autenticación
│   └── Migrations/
│
├── Credio.Infrastructure.Shared/         # Servicios transversales
│   ├── Services/                          # Email, notificaciones
│   └── EmailTemplates/                    # Templates de correos
│
├── Credio.Authentication.Api/            # Endpoint de autenticación
│   ├── Controllers/
│   │   ├── AccountController.cs           # Login, registro
│   │   └── SampleController.cs
│   └── Program.cs
│
└── Credio.Lending.Api/                   # Endpoint de préstamos
    ├── Controllers/
    │   ├── LoanController.cs              # CRUD de préstamos
    │   ├── ClientController.cs            # Gestión de clientes
    │   ├── BotController.cs               # Endpoints del bot
    │   ├── DashboardController.cs         # KPIs y reportes
    │   ├── ReportController.cs            # Reportes detallados
    │   └── CatalogController.cs           # Parámetros
    └── Program.cs
```

---

## 🔐 Seguridad y Autorización

El sistema implementa **role-based access control (RBAC)** con JWT:

```csharp
// Roles disponibles
- Administrator      // Acceso total
- Officer            // Alta (Aprobación de préstamos)
- Collector          // Media (Cobranza en campo)
- Client             // Baja (Consulta de saldos)
```

Cada endpoint declara su requerimiento explícitamente:

```csharp
[Authorize(Roles = "Administrator, Officer")]
[HttpPost("create")]
public async Task<IResult> CreateLoan(...)
```

---

## ⚙️ Patrones y Principios

| Patrón                       | Implementación                                      |
| ---------------------------- | --------------------------------------------------- |
| **CQRS**                     | Commands (escritura) + Queries (lectura) separados  |
| **Repository**               | IRepository<T> con implementaciones genéricas       |
| **Unit of Work**             | Manejo automático en transacciones                  |
| **Dependency Injection**     | Contenedor nativo de ASP.NET Core                   |
| **Domain Events**            | INotification + INotificationHandler                |
| **Soft Delete**              | Interceptor que marca eliminación lógica            |
| **Auditable Entities**       | Tracking de CreatedAt, UpdatedAt, CreatedBy         |
| **Global Exception Handler** | Middleware centralizado                             |
| **Pipeline Behaviors**       | Cross-cutting concerns (validación, logging, caché) |

---

## 🚀 Guía Rápida

### Requisitos Previos

- .NET 8 SDK
- PostgreSQL 12+
- Docker (opcional)

### Configuración Inicial

1. **Clonar repositorio**

```bash
git clone <repo-url>
cd Credio
```

2. **Restaurar dependencias**

```bash
dotnet restore
```

3. **Configurar conexión a BD**
   Editar `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DBCredioCore": "Host=localhost;Port=5432;Database=credio_dev;Username=postgres;Password=..."
  }
}
```

4. **Aplicar migraciones**

```bash
dotnet ef database update --project Credio.Infrastructure.Persistence
```

5. **Ejecutar APIs**

```bash
# Terminal 1: Authentication API
cd Credio.Authentication.Api
dotnet run

# Terminal 2: Lending API
cd Credio.Lending.Api
dotnet run
```

6. **Acceder a Swagger**

- Authentication: http://localhost:5000/swagger
- Lending: http://localhost:5001/swagger

---

## 📝 Estándares de Código

### Crear un nuevo endpoint (CheckList)

- [ ] Definir **Entity** en `Core.Domain/Entities`
- [ ] Crear **DTOs** en `Core.Application/Dtos`
- [ ] Implementar **Command/Query** en `Core.Application/Features/{Module}`
- [ ] Crear **Validator** usando FluentValidation
- [ ] Implementar **Handler** (lógica de negocio)
- [ ] Crear **AutoMapper profile** en `Mappings`
- [ ] Registrar servicio en `ServiceRegistration.cs`
- [ ] Implementar **Repository** si es necesario
- [ ] Crear **Controller endpoint** con `[Authorize(Roles = "...")]`
- [ ] Documentar con **Swagger attributes**
- [ ] Escribir **tests unitarios**

### Estructura de un Command Handler

```csharp
public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, Result<LoanDTO>>
{
    private readonly IRepository<Loan> _loanRepository;
    private readonly IAmortizationCalculatorService _amortizationService;
    private readonly IMapper _mapper;

    public CreateLoanCommandHandler(
        IRepository<Loan> loanRepository,
        IAmortizationCalculatorService amortizationService,
        IMapper mapper)
    {
        _loanRepository = loanRepository;
        _amortizationService = amortizationService;
        _mapper = mapper;
    }

    public async Task<Result<LoanDTO>> Handle(CreateLoanCommand request, CancellationToken ct)
    {
        // Validación
        var validation = new CreateLoanValidator().Validate(request);
        if (!validation.IsValid)
            return Result.Failure<LoanDTO>(validation.Errors);

        // Lógica de negocio
        var loan = Loan.Create(request.ClientId, request.Amount, request.Term);
        var schedule = _amortizationService.CalculateAmortization(request);

        // Persistencia
        await _loanRepository.AddAsync(loan, ct);
        await _loanRepository.SaveChangesAsync(ct);

        // Mapeo
        return Result.Success(_mapper.Map<LoanDTO>(loan));
    }
}
```

---

## 🤝 Contribución

1. Crear rama: `git checkout -b feature/nombre-feature`
2. Hacer commit: `git commit -m "feat: descripción clara"`
3. Push: `git push origin feature/nombre-feature`
4. Crear Pull Request

---

## 📚 Documentación Adicional

- [Diagrama de Casos de Uso](#) (Por completar)
- [Guía de Eventos de Dominio](#) (Por completar)
- [API Reference](#) (Swagger en `/swagger`)
- [Troubleshooting](#) (Por completar)

---

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver archivo `LICENSE` para más detalles.

---

**Credio © 2026** | _Construyendo el futuro de las microfinanzas_ 🚀
