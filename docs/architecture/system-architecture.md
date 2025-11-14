# System Architecture

## Overview

The CRM API system follows a modern web architecture with clear separation of concerns between frontend and backend, leveraging server-side rendering for optimal performance and SEO.

## High-Level Architecture

```mermaid
graph TB
    subgraph "Client Layer"
        Browser[Browser/Client]
    end
    
    subgraph "Frontend (Next.js)"
        SSR[Server Components]
        CSR[Client Components]
        MUI[Material UI]
        Router[App Router]
    end
    
    subgraph "Backend (ASP.NET Core)"
        API[Web API]
        Controllers[Controllers]
        Services[Services]
        Validation[Validation]
    end
    
    subgraph "Data Layer"
        EF[Entity Framework]
        DB[(InMemory DB)]
        Seed[Seed Data]
    end
    
    Browser --> SSR
    Browser --> CSR
    SSR --> API
    CSR --> API
    API --> Controllers
    Controllers --> Services
    Services --> Validation
    Services --> EF
    EF --> DB
    DB --> Seed
    
    classDef frontend fill:#e1f5fe
    classDef backend fill:#f3e5f5
    classDef data fill:#e8f5e8
    
    class SSR,CSR,MUI,Router frontend
    class API,Controllers,Services,Validation backend
    class EF,DB,Seed data
```

## Component Architecture

### Frontend Components

```mermaid
graph TB
    subgraph "Next.js App Router"
        Layout[Root Layout]
        Pages[Page Components]
        Loading[Loading States]
        Error[Error Boundaries]
    end
    
    subgraph "Server Components"
        ListPage[ContactListPage]
        DetailPage[ContactDetailPage]
        DataFetch[Data Fetching]
    end
    
    subgraph "Client Components"
        Table[ContactTable]
        Form[ContactForm]
        Search[SearchBar]
        Pagination[Pagination]
        Navigation[Navigation]
    end
    
    subgraph "Shared Components"
        UI[UI Components]
        Theme[ThemeProvider]
        Providers[Query Providers]
    end
    
    Layout --> Pages
    Pages --> ListPage
    Pages --> DetailPage
    ListPage --> Table
    ListPage --> Search
    ListPage --> Pagination
    DetailPage --> Form
    Form --> Navigation
    Pages --> Loading
    Pages --> Error
    Layout --> Theme
    Layout --> Providers
    Table --> UI
    Form --> UI
    Search --> UI
    Navigation --> UI
```

### Backend Architecture

```mermaid
graph TB
    subgraph "API Layer"
        Controller[ContactsController]
        Endpoints[REST Endpoints]
        Filters[Action Filters]
    end
    
    subgraph "Business Layer"
        Services[Contact Services]
        Validation[FluentValidation]
        Mapping[AutoMapper]
    end
    
    subgraph "Data Layer"
        Context[AppDbContext]
        Entities[Contact Entity]
        Repositories[Repository Pattern]
        Seed[Seed Data]
    end
    
    subgraph "Cross-Cutting"
        Logging[Serilog]
        ErrorHandling[Exception Middleware]
        CORS[CORS Configuration]
        Swagger[OpenAPI/Swagger]
    end
    
    Controller --> Services
    Controller --> Validation
    Controller --> Mapping
    Services --> Context
    Context --> Entities
    Context --> Seed
    Services --> Repositories
    Controller --> Filters
    Filters --> ErrorHandling
    ErrorHandling --> Logging
    Controller --> CORS
    Controller --> Swagger
```

## Data Flow Architecture

### Request Flow (Server-Side Rendering)

```mermaid
sequenceDiagram
    participant Browser
    participant NextJS as Next.js Server
    participant API as ASP.NET API
    participant DB as InMemory DB
    
    Browser->>NextJS: Request /contacts
    NextJS->>NextJS: Render Server Component
    NextJS->>API: GET /api/contacts
    API->>DB: Query contacts
    DB-->>API: Contact data
    API-->>NextJS: JSON response
    NextJS->>NextJS: Generate HTML
    NextJS-->>Browser: HTML + Client JS
    Browser->>Browser: Hydrate Client Components
```

### Interactive Flow (Client Components)

```mermaid
sequenceDiagram
    participant User
    participant Client as Client Component
    participant API as ASP.NET API
    participant DB as InMemory DB
    
    User->>Client: Search action
    Client->>Client: Debounce input
    Client->>API: GET /api/contacts?query=...
    API->>DB: Search contacts
    DB-->>API: Filtered results
    API-->>Client: JSON response
    Client->>Client: Update UI state
    Client-->>User: Display results
```

## Technology Integration

### Frontend Stack Integration

```mermaid
graph LR
    subgraph "Core"
        Next[Next.js 14+]
        TS[TypeScript]
        React[React 18+]
    end
    
    subgraph "UI/Styling"
        MUI[Material UI v5]
        Emotion[Emotion CSS-in-JS]
        Theme[Custom Theme]
    end
    
    subgraph "Data/State"
        Fetch[Native Fetch]
        ServerActions[Server Actions]
        Hooks[React Hooks]
    end
    
    subgraph "Forms"
        RHF[React Hook Form]
        Zod[Zod Validation]
    end
    
    Next --> TS
    Next --> React
    React --> MUI
    MUI --> Emotion
    Emotion --> Theme
    Next --> Fetch
    Fetch --> ServerActions
    React --> Hooks
    RHF --> Zod
    MUI --> RHF
```

### Backend Stack Integration

```mermaid
graph LR
    subgraph "Core"
        ASPNET[ASP.NET Core 8]
        CSharp[C#]
        EF[EF Core]
    end
    
    subgraph "API"
        Controllers[API Controllers]
        Routing[Attribute Routing]
        DTOs[Data Transfer Objects]
    end
    
    subgraph "Business"
        Services[Service Layer]
        Validation[FluentValidation]
        Mapping[AutoMapper]
    end
    
    subgraph "Infrastructure"
        InMemory[InMemory Provider]
        Swagger[Swagger/OpenAPI]
        Logging[Serilog]
    end
    
    ASPNET --> CSharp
    ASPNET --> EF
    ASPNET --> Controllers
    Controllers --> Routing
    Controllers --> DTOs
    Controllers --> Services
    Services --> Validation
    Services --> Mapping
    EF --> InMemory
    ASPNET --> Swagger
    ASPNET --> Logging
```

## Security Architecture

```mermaid
graph TB
    subgraph "Frontend Security"
        XSS[XSS Protection]
        CSRF[CSRF Tokens]
        Validation[Client Validation]
    end
    
    subgraph "API Security"
        Auth[Authentication]
        Authz[Authorization]
        CORS[CORS Policy]
        RateLimit[Rate Limiting]
    end
    
    subgraph "Data Security"
        InputSanit[Input Sanitization]
        ValidationRules[Validation Rules]
        ErrorHandling[Error Handling]
    end
    
    XSS --> Auth
    CSRF --> CORS
    Validation --> InputSanit
    Auth --> Authz
    CORS --> RateLimit
    Authz --> ValidationRules
    RateLimit --> ErrorHandling
```

## Performance Architecture

```mermaid
graph TB
    subgraph "Frontend Performance"
        SSR[Server-Side Rendering]
        CodeSplit[Code Splitting]
        LazyLoad[Lazy Loading]
        Cache[Browser Cache]
    end
    
    subgraph "API Performance"
        Pagination[Pagination]
        Indexing[Database Indexing]
        CacheAPI[Response Caching]
        Compression[Response Compression]
    end
    
    subgraph "Network Performance"
        CDN[CDN Distribution]
        Minify[Asset Minification]
        HTTP2[HTTP/2]
        OptimizedImages[Image Optimization]
    end
    
    SSR --> Pagination
    CodeSplit --> CacheAPI
    LazyLoad --> Compression
    Cache --> CDN
    Pagination --> Minify
    Indexing --> HTTP2
    CacheAPI --> OptimizedImages
```

## Deployment Architecture

```mermaid
graph TB
    subgraph "Development"
        DevLocal[Local Development]
        Docker[Docker Compose]
        HotReload[Hot Reload]
    end
    
    subgraph "Staging"
        StagingEnv[Staging Environment]
        CI[Continuous Integration]
        Tests[Automated Tests]
    end
    
    subgraph "Production"
        ProdEnv[Production Environment]
        CD[Continuous Deployment]
        Monitoring[Monitoring & Logging]
    end
    
    DevLocal --> Docker
    Docker --> HotReload
    DevLocal --> CI
    CI --> StagingEnv
    StagingEnv --> Tests
    Tests --> CD
    CD --> ProdEnv
    ProdEnv --> Monitoring
```

## Key Architectural Decisions

### 1. Server-Side Rendering (SSR)
- **Rationale**: Fast initial loads, SEO optimization, better user experience
- **Implementation**: Next.js App Router with Server Components
- **Trade-offs**: Increased server load, more complex state management

### 2. Material UI Component Library
- **Rationale**: Consistent design system, accessibility features, rapid development
- **Implementation**: MUI v5 with custom theming
- **Trade-offs**: Bundle size, less design flexibility

### 3. Entity Framework Core InMemory
- **Rationale**: Rapid development, easy setup, no external dependencies
- **Implementation**: EF Core with InMemory provider for development
- **Trade-offs**: Not production-ready, limited SQL features

### 4. RESTful API Design
- **Rationale**: Standardized interface, easy client integration, tooling support
- **Implementation**: ASP.NET Core Web API with OpenAPI documentation
- **Trade-offs**: Multiple round trips, over-fetching/under-fetching

### 5. TypeScript Full Stack
- **Rationale**: Type safety, better developer experience, reduced runtime errors
- **Implementation**: TypeScript in frontend and C# strong typing in backend
- **Trade-offs**: Initial setup complexity, compilation overhead
