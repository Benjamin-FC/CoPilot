# System Architecture Overview

## Document Information
- **Version**: 1.0
- **Last Updated**: November 16, 2025
- **Status**: Active

## Executive Summary

The CRM API is a modern, cloud-ready RESTful API built on ASP.NET Core 8.0 that provides comprehensive client relationship management capabilities with integrated third-party contact synchronization via Loops.so.

## Architecture Principles

### 1. Clean Architecture
- **Separation of Concerns**: Clear boundaries between presentation, business logic, and data layers
- **Dependency Inversion**: Dependencies flow inward toward domain entities
- **Framework Independence**: Business logic remains isolated from infrastructure concerns

### 2. Design Patterns
- **Repository Pattern**: Data access abstraction through Entity Framework Core
- **Service Layer Pattern**: Business logic encapsulated in services (e.g., LoopsService)
- **DTO Pattern**: Data Transfer Objects for API contracts
- **Dependency Injection**: Constructor-based DI for loose coupling
- **Fire-and-Forget Pattern**: Non-blocking external API calls

### 3. Architectural Drivers

#### Quality Attributes
| Attribute | Priority | Implementation |
|-----------|----------|----------------|
| Maintainability | High | Modular structure, clear separation of concerns |
| Scalability | High | Stateless API, in-memory/external database support |
| Testability | High | Dependency injection, interface-based services |
| Reliability | Medium | Comprehensive error handling, logging |
| Performance | Medium | Async operations, efficient queries with AutoMapper projections |
| Security | Medium | HTTPS, CORS policies, input validation |

## System Context

```
┌─────────────────────────────────────────────────────────┐
│                    External Systems                      │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  ┌──────────────┐              ┌─────────────────┐     │
│  │   Web Client │              │   Mobile App    │     │
│  │  (React/Vue) │              │  (iOS/Android)  │     │
│  └──────┬───────┘              └────────┬────────┘     │
│         │                                │              │
│         └────────────────┬───────────────┘              │
│                          │                              │
│                  HTTPS/REST API                         │
│                          │                              │
│         ┌────────────────▼────────────────┐             │
│         │        CRM API Service          │             │
│         │     (ASP.NET Core 8.0)          │             │
│         └────────┬──────────────┬─────────┘             │
│                  │              │                       │
│         ┌────────▼──────┐  ┌────▼────────┐             │
│         │   Database    │  │  Loops.so   │             │
│         │ (In-Memory/   │  │  API        │             │
│         │  SQL Server)  │  │  (External) │             │
│         └───────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────┘
```

## Component Architecture

### High-Level Component Diagram

```
┌────────────────────────────────────────────────────────────┐
│                     Presentation Layer                      │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────┐       │
│  │         ClientsController                       │       │
│  │  - GET    /api/clients                          │       │
│  │  - GET    /api/clients/{id}                     │       │
│  │  - POST   /api/clients                          │       │
│  │  - PUT    /api/clients/{id}                     │       │
│  │  - DELETE /api/clients/{id}                     │       │
│  └─────────────────────────────────────────────────┘       │
│                          │                                  │
└──────────────────────────┼──────────────────────────────────┘
                           │
┌──────────────────────────┼──────────────────────────────────┐
│                 Application Services Layer                  │
├──────────────────────────┼──────────────────────────────────┤
│                          │                                  │
│  ┌──────────────┐  ┌─────▼──────┐  ┌──────────────────┐   │
│  │   AutoMapper │  │  Validators│  │   LoopsService   │   │
│  │   (Mapping)  │  │ (FluentVal)│  │ (External Sync)  │   │
│  └──────────────┘  └────────────┘  └──────────────────┘   │
│                                                             │
└──────────────────────────┼──────────────────────────────────┘
                           │
┌──────────────────────────┼──────────────────────────────────┐
│                      Domain Layer                           │
├──────────────────────────┼──────────────────────────────────┤
│                          │                                  │
│  ┌──────────────────────▼──────────────────────┐           │
│  │              Client Entity                   │           │
│  │  - Id, FirstName, LastName, Email            │           │
│  │  - Phone, Company, Address                   │           │
│  │  - IsActive, CreatedAt, UpdatedAt            │           │
│  └──────────────────────────────────────────────┘           │
│                                                             │
└──────────────────────────┼──────────────────────────────────┘
                           │
┌──────────────────────────┼──────────────────────────────────┐
│                 Infrastructure Layer                        │
├──────────────────────────┼──────────────────────────────────┤
│                          │                                  │
│  ┌──────────────────────▼──────────────────────┐           │
│  │           AppDbContext (EF Core)             │           │
│  │  - DbSet<Client>                             │           │
│  │  - Configuration, Seeding                    │           │
│  └──────────────────────┬───────────────────────┘           │
│                         │                                   │
│  ┌──────────────────────▼──────────────────────┐           │
│  │     Database (In-Memory / SQL Server)        │           │
│  └──────────────────────────────────────────────┘           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### 1. Presentation Layer (Controllers)
**Location**: `src/Api/Controllers/`

**Responsibilities**:
- HTTP request/response handling
- Route management
- Input validation coordination
- HTTP status code management
- Response formatting

**Key Components**:
- `ClientsController`: RESTful endpoints for client management

### 2. Application Services Layer
**Location**: `src/Api/Services/`, `src/Api/Validation/`, `src/Api/Mapping/`

**Responsibilities**:
- Business logic orchestration
- External service integration
- Data transformation
- Input validation
- Cross-cutting concerns

**Key Components**:
- `ILoopsService / LoopsService`: External contact synchronization
- `CreateClientValidator / UpdateClientValidator`: Input validation rules
- `ClientProfile`: AutoMapper configuration

### 3. Domain Layer
**Location**: `src/Api/Domain/`

**Responsibilities**:
- Core business entities
- Domain rules and constraints
- Entity relationships

**Key Components**:
- `Client`: Core domain entity representing a CRM client

### 4. Infrastructure Layer
**Location**: `src/Api/Data/`, `src/Api/Configuration/`

**Responsibilities**:
- Data persistence
- External system communication
- Configuration management
- Database context

**Key Components**:
- `AppDbContext`: Entity Framework Core DbContext
- `LoopsOptions`: Configuration for Loops.so integration

## Data Flow

### Create Client Flow
```
1. Client Request (POST /api/clients)
   └─> ClientsController.CreateClient()
       │
       ├─> FluentValidation: Validate input
       │   └─> Return 400 if invalid
       │
       ├─> Check duplicate email in database
       │   └─> Return 409 if exists
       │
       ├─> AutoMapper: Map DTO → Entity
       │
       ├─> Generate Id, timestamps
       │
       ├─> SaveChangesAsync() to database
       │
       ├─> Fire-and-Forget: Sync to Loops.so
       │   └─> Task.Run(() => LoopsService.CreateContactAsync())
       │       ├─> Check if enabled
       │       ├─> Validate API key
       │       ├─> Build HTTP request
       │       ├─> Send to Loops.so API
       │       └─> Log result (no blocking)
       │
       ├─> AutoMapper: Map Entity → DTO
       │
       └─> Return 201 Created with location header
```

### Query Clients Flow
```
1. Client Request (GET /api/clients?query=...&page=1)
   └─> ClientsController.GetClients()
       │
       ├─> Build IQueryable from DbSet
       │
       ├─> Apply filters (isActive, search query)
       │
       ├─> Apply sorting (lastName, firstName, etc.)
       │
       ├─> Apply pagination (Skip/Take)
       │
       ├─> AutoMapper ProjectTo: Direct projection to DTO
       │   └─> Efficient SQL generation
       │
       ├─> Execute ToListAsync()
       │
       └─> Return 200 OK with ClientListResponse
```

## Technology Stack

### Core Framework
- **ASP.NET Core 8.0**: Web API framework
- **C# 12**: Programming language
- **.NET 8 Runtime**: Execution environment

### Data Access
- **Entity Framework Core 8.0**: ORM
- **In-Memory Database**: Development/Testing
- **SQL Server**: Production-ready option

### Libraries & Packages
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation
- **Swagger/OpenAPI**: API documentation
- **Microsoft.Extensions.Http**: HTTP client factory

### External Services
- **Loops.so API**: Email marketing and contact management

## Deployment Architecture

### Development Environment
```
┌──────────────────────────────────────┐
│      Developer Workstation           │
├──────────────────────────────────────┤
│  - Visual Studio 2022 / VS Code      │
│  - .NET 8 SDK                        │
│  - In-Memory Database                │
│  - Swagger UI (/)                    │
│  - HTTPS (Development Certificate)   │
└──────────────────────────────────────┘
```

### Production Environment (Example)
```
┌─────────────────────────────────────────────┐
│           Cloud Provider (Azure/AWS)        │
├─────────────────────────────────────────────┤
│                                             │
│  ┌────────────────────────────────┐         │
│  │     Load Balancer / CDN        │         │
│  └─────────────┬──────────────────┘         │
│                │                            │
│  ┌─────────────▼──────────────────┐         │
│  │   App Service / Container      │         │
│  │   (CRM API - Multiple Instances)│        │
│  └─────────────┬──────────────────┘         │
│                │                            │
│  ┌─────────────▼──────────────────┐         │
│  │   Azure SQL Database /          │         │
│  │   RDS (SQL Server)              │         │
│  └─────────────────────────────────┘         │
│                                             │
│  External: Loops.so API (HTTPS)             │
│                                             │
└─────────────────────────────────────────────┘
```

## Security Architecture

### Authentication & Authorization
- **Current**: None (open API)
- **Recommended**: JWT Bearer tokens, API keys, OAuth 2.0

### Data Security
- **HTTPS**: TLS 1.2+ encryption in transit
- **Input Validation**: FluentValidation for all inputs
- **SQL Injection Protection**: Entity Framework parameterized queries
- **CORS**: Configurable origins (currently development only)

### Secrets Management
- **Configuration**: appsettings.json (non-production)
- **Recommended**: Azure Key Vault, AWS Secrets Manager, or environment variables

## Scalability Considerations

### Horizontal Scaling
- **Stateless API**: No session state, supports multiple instances
- **Database**: Centralized data store shared across instances
- **Load Balancing**: Round-robin or least-connection distribution

### Performance Optimization
- **Async Operations**: All I/O operations are async
- **Database Indexes**: Email (unique), Name composite, IsActive, CreatedAt
- **AutoMapper Projections**: Direct DTO projection reduces memory overhead
- **Fire-and-Forget**: External API calls don't block responses
- **Connection Pooling**: HTTP client factory manages connection lifecycle

## Monitoring & Observability

### Logging
- **Framework**: Microsoft.Extensions.Logging
- **Levels**: Information, Warning, Error
- **Scopes**: Structured logging with context

### Metrics (Recommended)
- Request count, latency, error rates
- Database query performance
- External API call success/failure rates

### Health Checks (Recommended)
- Database connectivity
- External API availability
- Application responsiveness

## Future Architectural Considerations

### Microservices Evolution
- Split into ClientService, NotificationService, ReportingService
- Event-driven architecture with message bus (RabbitMQ, Azure Service Bus)

### CQRS Pattern
- Separate read/write models for complex queries
- Event sourcing for audit trails

### Caching Layer
- Redis for frequently accessed data
- Response caching for read-heavy endpoints

### API Gateway
- Centralized authentication, rate limiting, request routing
- Azure API Management, AWS API Gateway, or Kong

## References

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://learn.microsoft.com/ef/core)
- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Loops.so API Documentation](https://app.loops.so/api/v1)
