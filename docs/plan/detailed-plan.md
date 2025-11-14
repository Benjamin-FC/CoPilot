# PEO Clients — Detailed Implementation Plan

## Project Overview

**PEO Clients** is a modern web application for managing client information with server-side rendering, Material UI components, and a C# ASP.NET Core backend. The system provides search, CRUD operations, and responsive design with SEO optimization.

## Architecture Overview

### Frontend Stack
- **Next.js 14+** with App Router (React + TypeScript)
- **Server-Side Rendering (SSR)** and Static Site Generation (SSG)
- **Material UI (MUI) v5** for component library
- **Native fetch API** with React Server Components
- **React Hook Form + Zod** for forms and validation
- **Next.js App Router** for routing

### Backend Stack
- **ASP.NET Core Web API (.NET 8)**
- **Entity Framework Core InMemory** provider
- **AutoMapper** for entity-DTO mapping
- **FluentValidation** for server validation
- **Swagger/OpenAPI** for API documentation
- **xUnit + WebApplicationFactory** for testing

### Cross-cutting Concerns
- **CORS** enabled for localhost development
- **ProblemDetails** for standardized error responses
- **Pagination/search patterns** for data handling
- **Server components** for data fetching, **client components** for interactivity

## Data Model

### Core Entity: Client

```csharp
public class Client
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
```

### Data Transfer Objects

- **ClientListItemDto**: Subset for table displays
- **ClientDetailDto**: Full client details
- **CreateClientDto**: New client creation
- **UpdateClientDto**: Client updates

### Seed Data
- 50–200 realistic client records for testing search/sort/pagination

## API Contracts

### Endpoints

#### GET /api/clients
**Query Parameters:**
- `query?: string` - Search term
- `page?: number` - Page number (1-based)
- `pageSize?: number` - Items per page
- `sort?: string` - Sort field (firstName, lastName, email, company, createdAt)
- `dir?: 'asc'|'desc'` - Sort direction
- `isActive?: boolean` - Filter by active status

**Response:**
```json
{
  "items": "ClientListItemDto[]",
  "total": "number",
  "page": "number",
  "pageSize": "number",
  "sort": "string",
  "dir": "asc|desc"
}
```

#### GET /api/clients/{id}
Returns full client details as `ClientDetailDto`

#### POST /api/clients
Creates new client from `CreateClientDto`

#### PUT /api/clients/{id}
Updates client from `UpdateClientDto`

#### DELETE /api/clients/{id}
Soft delete (sets `IsActive = false`) or hard delete

### DTO Definitions

```typescript
// ClientListItemDto
{
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  company?: string;
  isActive: boolean;
  createdAt: string;
}

// ClientDetailDto
{
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  company?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Frontend Implementation

### Project Structure

```
app/
├── clients/
│   ├── page.tsx              # Server Component - list page
│   ├── new/page.tsx          # Client Component - create form
│   └── [id]/
│       ├── page.tsx          # Server Component - detail view
│       └── edit/page.tsx     # Client Component - edit form
├── layout.tsx                # Root layout with MUI provider
└── globals.css               # Global styles

components/
├── ui/                       # MUI components and theme
└── clients/
    ├── ClientTable.tsx       # Client Component with DataGrid
    ├── ClientForm.tsx        # Client Component with validation
    ├── SearchBar.tsx         # Client Component with debounced input
    └── ClientDetail.tsx      # Server Component

lib/
├── api/                      # Fetch utilities and typed functions
├── validations/              # Zod schemas
└── utils/                    # Helper functions

types/                        # TypeScript type definitions
hooks/                        # Custom React hooks
tests/                        # Component and integration tests
```

### Component Architecture

#### Server Components
- **ClientListPage**: Fetches initial data, handles search params
- **ClientDetailPage**: Fetches individual client data
- **Static components**: Headers, footers, non-interactive content

#### Client Components
- **ClientTable**: MUI DataGrid with sorting, filtering, pagination
- **ClientForm**: Form with validation, submission handling
- **SearchBar**: Debounced search input with clear functionality
- **Pagination**: Page navigation with size options

### Data Fetching Strategy

#### Server-Side Data Fetching
```typescript
// Server Component example
async function ClientListPage({ searchParams }: PageProps) {
  const clients = await fetchClients(searchParams);
  return <ClientTable clients={clients} />;
}
```

#### Client-Side Interactions
```typescript
// Client Component with fetch
'use client';

function ClientTable({ initialClients }: Props) {
  const [clients, setClients] = useState(initialClients);
  
  const handleSearch = async (query: string) => {
    const results = await fetch(`/api/clients?query=${query}`);
    setClients(await results.json());
  };
}
```

### Form Handling

#### React Hook Form + Zod Integration
```typescript
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { createClientSchema } from '@/lib/validations';

function ClientForm() {
  const form = useForm({
    resolver: zodResolver(createClientSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      isActive: true,
    },
  });
  
  const onSubmit = async (data: CreateClientFormData) => {
    await fetch('/api/clients', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  };
}
```

### Material UI Integration

#### Theme Configuration
```typescript
// theme/provider.tsx
import { ThemeProvider, createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
  },
});

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  return <MuiThemeProvider theme={theme}>{children}</MuiThemeProvider>;
}
```

#### Component Examples
```typescript
// ClientTable with DataGrid
import { DataGrid, GridColDef } from '@mui/x-data-grid';

const columns: GridColDef[] = [
  { field: 'firstName', headerName: 'First Name', flex: 1 },
  { field: 'lastName', headerName: 'Last Name', flex: 1 },
  { field: 'email', headerName: 'Email', flex: 2 },
  { field: 'company', headerName: 'Company', flex: 1 },
  { field: 'isActive', headerName: 'Active', width: 100 },
];

function ClientTable({ clients }: Props) {
  return (
    <DataGrid
      rows={clients}
      columns={columns}
      pageSizeOptions={[10, 25, 50]}
      checkboxSelection
    />
  );
}
```

## Backend Implementation

### Project Structure

```
src/Api/
├── Program.cs                 # Application entry point
├── Controllers/
│   └── ClientsController.cs   # API endpoints
├── Data/
│   └── AppDbContext.cs        # Database context
├── Domain/
│   └── Client.cs              # Entity definition
├── Dtos/
│   ├── ClientListItemDto.cs
│   ├── ClientDetailDto.cs
│   ├── CreateClientDto.cs
│   └── UpdateClientDto.cs
├── Mapping/
│   └── ClientProfile.cs       # AutoMapper configuration
├── Validation/
│   ├── CreateClientValidator.cs
│   └── UpdateClientValidator.cs
└── Seed/
    └── SeedData.cs            # Sample data generation

tests/Api.Tests/
└── ClientsControllerTests.cs  # Unit and integration tests
```

### Controller Implementation

```csharp
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    [HttpGet]
    public async Task<ActionResult<ClientListResponse>> GetClients(
        [FromQuery] string? query = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sort = "lastName",
        [FromQuery] string dir = "asc",
        [FromQuery] bool? isActive = null)
    {
        var clients = await _context.Clients
            .Where(c => isActive == null || c.IsActive == isActive)
            .Where(c => string.IsNullOrEmpty(query) || 
                c.FirstName.Contains(query) ||
                c.LastName.Contains(query) ||
                c.Email.Contains(query) ||
                (c.Phone != null && c.Phone.Contains(query)) ||
                (c.Company != null && c.Company.Contains(query)))
            .ApplySorting(sort, dir)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ClientListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var total = await _context.Clients
            .Where(c => isActive == null || c.IsActive == isActive)
            .Where(c => string.IsNullOrEmpty(query) || 
                c.FirstName.Contains(query) ||
                c.LastName.Contains(query) ||
                c.Email.Contains(query) ||
                (c.Phone != null && c.Phone.Contains(query)) ||
                (c.Company != null && c.Company.Contains(query)))
            .CountAsync();

        return Ok(new ClientListResponse
        {
            Items = clients,
            Total = total,
            Page = page,
            PageSize = pageSize,
            Sort = sort,
            Dir = dir
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDetailDto>> GetClient(Guid id)
    {
        var client = await _context.Clients
            .ProjectTo<ClientDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null) return NotFound();
        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDetailDto>> CreateClient(
        [FromBody] CreateClientDto createDto,
        [FromServices] IValidator<CreateClientDto> validator)
    {
        var validationResult = await validator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var client = _mapper.Map<Client>(createDto);
        client.CreatedAt = DateTimeOffset.UtcNow;
        client.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var resultDto = _mapper.Map<ClientDetailDto>(client);
        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDetailDto>> UpdateClient(
        Guid id,
        [FromBody] UpdateClientDto updateDto,
        [FromServices] IValidator<UpdateClientDto> validator)
    {
        var validationResult = await validator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound();

        _mapper.Map(updateDto, client);
        client.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        var resultDto = _mapper.Map<ClientDetailDto>(client);
        return Ok(resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteClient(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound();

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

### Database Configuration

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new ClientConfiguration());
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Generate 50-200 sample clients
        var clients = GenerateSampleClients();
        modelBuilder.Entity<Client>().HasData(clients);
    }
}
```

### Validation Implementation

```csharp
public class CreateClientValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("First name is required and must be less than 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Last name is required and must be less than 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(255)
            .EmailAddress()
            .WithMessage("Valid email address is required.");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches(@"^\d{3}-\d{3}-\d{4}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone number must be in format 555-555-5555.");
    }
}
```

## Testing Strategy

### Frontend Testing

#### Component Tests
```typescript
// ClientForm.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { ClientForm } from './ClientForm';

describe('ClientForm', () => {
  it('renders form fields correctly', () => {
    render(<ClientForm />);
    
    expect(screen.getByLabelText('First Name')).toBeInTheDocument();
    expect(screen.getByLabelText('Last Name')).toBeInTheDocument();
    expect(screen.getByLabelText('Email')).toBeInTheDocument();
  });

  it('validates required fields', async () => {
    render(<ClientForm />);
    
    const submitButton = screen.getByRole('button', { name: /submit/i });
    fireEvent.click(submitButton);
    
    expect(await screen.findByText('First name is required')).toBeInTheDocument();
    expect(await screen.findByText('Last name is required')).toBeInTheDocument();
  });
});
```

#### API Integration Tests
```typescript
// clients.test.ts
import { renderHook, waitFor } from '@testing-library/react';
import { useClients } from '@/hooks/useClients';

describe('useClients', () => {
  it('fetches clients successfully', async () => {
    const { result } = renderHook(() => useClients());
    
    await waitFor(() => {
      expect(result.current.clients).toHaveLength(50); // Based on seed data
    });
  });
});
```

### Backend Testing

#### Controller Tests
```csharp
public class ClientsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ClientsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetClients_ReturnsPagedResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/clients?page=1&pageSize=10");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ClientListResponse>(content);
        
        Assert.NotNull(result);
        Assert.Equal(10, result.Items.Count);
        Assert.True(result.Total > 0);
    }

    [Fact]
    public async Task CreateClient_WithValidData_ReturnsCreatedClient()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createDto = new CreateClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        
        var json = JsonSerializer.Serialize(createDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await client.PostAsync("/api/clients", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## Performance Optimizations

### Frontend Optimizations
- **Server-Side Rendering**: Fast initial page loads
- **Code Splitting**: Dynamic imports for heavy components
- **Image Optimization**: Next.js Image component
- **Bundle Analysis**: Regular bundle size monitoring
- **Caching Strategy**: Browser and CDN caching

### Backend Optimizations
- **Database Indexing**: Optimized query performance
- **Pagination**: Limit result set sizes
- **Response Compression**: Reduce payload sizes
- **Entity Framework Optimization**: Efficient queries
- **Memory Caching**: Cache frequently accessed data

### Network Optimizations
- **HTTP/2**: Multiplexed requests
- **CDN Distribution**: Static asset delivery
- **Minification**: Asset size reduction
- **Lazy Loading**: On-demand resource loading

## Security Considerations

### Frontend Security
- **XSS Protection**: Input sanitization
- **CSRF Protection**: Token validation
- **Content Security Policy**: Restrict resource loading
- **Secure Cookies**: HttpOnly and Secure flags

### Backend Security
- **Input Validation**: Comprehensive validation rules
- **SQL Injection Prevention**: Parameterized queries
- **Authentication**: Identity integration (future)
- **Authorization**: Role-based access control (future)

### Data Protection
- **PII Handling**: Sensitive data protection
- **Audit Logging**: Change tracking
- **Data Encryption**: At rest and in transit
- **Privacy Compliance**: GDPR/CCPA considerations

## Deployment Strategy

### Development Environment
- **Local Development**: Docker Compose setup
- **Hot Reload**: Instant development feedback
- **Development Database**: InMemory provider
- **Debugging**: Full debugging capabilities

### Staging Environment
- **Production-like Setup**: Mirror production configuration
- **Automated Testing**: Full test suite execution
- **Performance Testing**: Load and stress testing
- **Security Testing**: Vulnerability scanning

### Production Environment
- **Container Deployment**: Docker containers
- **Load Balancing**: Multiple instance support
- **Database**: SQL Server or PostgreSQL
- **Monitoring**: Application performance monitoring
- **Backup Strategy**: Regular data backups

## Future Enhancements

### Phase 2 Features
- **Authentication & Authorization**: User management
- **Advanced Search**: Filters and saved searches
- **Data Export**: CSV and PDF generation
- **Real-time Updates**: WebSocket integration
- **Mobile App**: React Native application

### Technical Improvements
- **Microservices Architecture**: Service decomposition
- **Event Sourcing**: Audit trail and temporal queries
- **CQRS Pattern**: Read/write model separation
- **Graphql API**: Flexible data querying
- **Progressive Web App**: Offline capabilities

### Infrastructure Enhancements
- **Kubernetes Deployment**: Container orchestration
- **Auto-scaling**: Dynamic resource allocation
- **Multi-region Deployment**: Geographic distribution
- **Disaster Recovery**: Business continuity planning
- **Cost Optimization**: Resource usage optimization

## Success Metrics

### Performance Metrics
- **Page Load Time**: < 2 seconds for initial load
- **API Response Time**: < 500ms for 95th percentile
- **Database Query Time**: < 100ms average
- **Bundle Size**: < 1MB initial JavaScript

### User Experience Metrics
- **Search Response Time**: < 300ms
- **Form Submission Time**: < 1 second
- **Mobile Responsiveness**: 100% mobile-friendly
- **Accessibility Score**: WCAG 2.1 AA compliance

### Development Metrics
- **Code Coverage**: > 80% test coverage
- **Build Time**: < 5 minutes for full build
- **Deployment Time**: < 10 minutes for production
- **Bug Density**: < 1 critical bug per release

This comprehensive plan provides a roadmap for building a modern, scalable, and maintainable client management system using the latest web technologies and best practices.
