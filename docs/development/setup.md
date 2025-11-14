# Development Setup Guide

## Overview

This guide provides step-by-step instructions for setting up the CRM API development environment on your local machine.

## Prerequisites

### Required Software

| Software | Minimum Version | Recommended | Installation |
|----------|-----------------|-------------|--------------|
| Node.js | 18.0.0 | 20.0.0+ | [nodejs.org](https://nodejs.org/) |
| npm | 8.0.0 | 10.0.0+ | Included with Node.js |
| .NET SDK | 8.0.0 | 8.0.100+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/) |
| Git | 2.30.0 | 2.40.0+ | [git-scm.com](https://git-scm.com/) |
| VS Code | Latest | Latest | [code.visualstudio.com](https://code.visualstudio.com/) |
| Docker | 20.0.0 | Latest | [docker.com](https://docker.com/) (optional) |

### VS Code Extensions (Recommended)

```json
{
  "recommendations": [
    "ms-vscode.vscode-typescript-next",
    "bradlc.vscode-tailwindcss",
    "ms-dotnettools.csharp",
    "ms-dotnettools.csdevkit",
    "esbenp.prettier-vscode",
    "dbaeumer.vscode-eslint",
    "ms-vscode.vscode-json",
    "redhat.vscode-yaml",
    "ms-vscode-remote.remote-containers"
  ]
}
```

---

## Project Setup

### 1. Clone the Repository

```bash
git clone <repository-url> crm-api
cd crm-api
```

### 2. Backend Setup

#### Navigate to Backend Directory
```bash
cd src/Api
```

#### Restore NuGet Packages
```bash
dotnet restore
```

#### Verify .NET SDK
```bash
dotnet --version
# Should show 8.0.100 or later
```

#### Run the Backend API
```bash
dotnet run
```

The API should start at `https://localhost:5191` and `http://localhost:5190`

#### Access Swagger Documentation
Open your browser and navigate to:
- Swagger UI: `http://localhost:5190/swagger`
- OpenAPI JSON: `http://localhost:5190/swagger/v1/swagger.json`

### 3. Frontend (Planned)

The CRM frontend is planned as a separate Next.js application but is **not yet implemented in this repository**.

When the frontend is added, it is expected to:

- Live in a dedicated frontend folder (for example `frontend/` or `client/`).
- Consume the existing CRM API at `http://localhost:5190/api` / `https://localhost:5191/api`.
- Use TypeScript and a modern React-based stack.

Until that code exists, there are **no frontend setup steps** to run from this repository.

---

## Development Workflow

### 1. Start Development Environment

#### Terminal 1 - Backend API
```bash
cd src/Api
dotnet run
```

#### Terminal 2 - Database (Optional - for SQL database)
```bash
# If using SQL Server instead of InMemory
dotnet ef database update
```

### 2. Verify Setup

#### Backend Health Check
```bash
curl http://localhost:5190/api/contacts
# Should return JSON with contact data
```

#### API Documentation
Visit `http://localhost:5190/swagger` for interactive API testing

### 3. Development Features

#### Hot Reload
- **Backend**: Automatic compilation on file changes
- **Frontend**: Fast Refresh enabled by Next.js

#### Debugging
- **Backend**: Use VS Code debugger with launch configuration
- **Frontend**: Use browser dev tools or VS Code debugger

#### Database Seeding
The backend automatically seeds 150+ sample contacts on startup when using InMemory provider.

---

## Project Structure

```
crm-api/
├── docs/                           # Documentation
│   ├── README.md
│   ├── architecture/
│   ├── tech-stack/
│   ├── plan/
│   ├── tasks/
│   ├── api/
│   ├── development/
│   └── deployment/
├── src/
│   └── Api/                        # ASP.NET Core Web API
│       ├── Controllers/
│       ├── Data/
│       ├── Domain/
│       ├── Dtos/
│       ├── Mapping/
│       ├── Validation/
│       ├── Seed/
│       └── Program.cs
├── frontend/                       # Next.js Frontend
│   ├── app/                        # App Router pages
│   │   ├── contacts/
│   │   │   ├── page.tsx
│   │   │   ├── new/page.tsx
│   │   │   └── [id]/
│   │   │       ├── page.tsx
│   │   │       └── edit/page.tsx
│   │   ├── layout.tsx
│   │   └── globals.css
│   ├── components/
│   │   ├── ui/
│   │   └── contacts/
│   ├── lib/
│   │   ├── api/
│   │   ├── validations/
│   │   └── utils/
│   ├── types/
│   ├── hooks/
│   └── tests/
├── tests/                          # Test Projects
│   ├── Api.Tests/
│   └── Frontend.Tests/
├── docker-compose.yml              # Docker Development Setup
├── .gitignore
├── README.md
└── CrmApi.sln
```

---

## Common Development Tasks

### 1. Adding a New API Endpoint

#### Backend Steps
1. Create DTO in `src/Api/Dtos/`
2. Add validation rules in `src/Api/Validation/`
3. Add mapping in `src/Api/Mapping/`
4. Implement endpoint in `src/Api/Controllers/`
5. Add unit tests in `tests/Api.Tests/`

#### Frontend Steps
1. Add TypeScript types in `frontend/types/`
2. Create API functions in `frontend/lib/api/`
3. Add Zod validation schemas in `frontend/lib/validations/`
4. Implement components in `frontend/components/`
5. Add pages in `frontend/app/`

### 2. Database Schema Changes

#### Using EF Core Migrations
```bash
# Add new migration
dotnet ef migrations Add MigrationName

# Update database
dotnet ef database update

# Remove last migration (if needed)
dotnet ef migrations remove
```

#### Updating Seed Data
Modify the seed data in `src/Api/Data/AppDbContext.cs` and restart the application.

### 3. Adding New UI Components

#### Material UI Components
```typescript
// frontend/components/ui/MyComponent.tsx
import { Box, Typography, Button } from '@mui/material';

interface MyComponentProps {
  title: string;
  onAction: () => void;
}

export function MyComponent({ title, onAction }: MyComponentProps) {
  return (
    <Box sx={{ p: 2, border: 1, borderColor: 'grey.300' }}>
      <Typography variant="h6">{title}</Typography>
      <Button onClick={onAction}>Action</Button>
    </Box>
  );
}
```

#### Server vs Client Components
```typescript
// Server Component (no 'use client')
export async function ServerComponent() {
  const data = await fetch('https://api.example.com/data');
  return <div>{data}</div>;
}

// Client Component (with 'use client')
'use client';

export function ClientComponent() {
  const [count, setCount] = useState(0);
  return (
    <button onClick={() => setCount(count + 1)}>
      Count: {count}
    </button>
  );
}
```

---

## Testing Setup

### Backend Testing

#### Run Unit Tests
```bash
cd tests/Api.Tests
dotnet test
```

#### Run Specific Test
```bash
dotnet test --filter "TestMethodName"
```

#### Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend Testing

#### Run Unit Tests
```bash
cd frontend
npm test
```

#### Run Tests in Watch Mode
```bash
npm run test:watch
```

#### Run E2E Tests
```bash
npm run test:e2e
```

#### Test Coverage
```bash
npm run test:coverage
```

---

## Troubleshooting

### Common Issues

#### Backend Issues

**Issue**: "HTTPS development certificate is not trusted"
```bash
# Solution: Trust the dev certificate
dotnet dev-certs https --trust
```

**Issue**: "Port 7000/7001 is already in use"
```bash
# Solution: Kill processes using the ports
netstat -ano | findstr :7000
taskkill /PID <PID> /F
```

**Issue**: "Entity Framework migration failed"
```bash
# Solution: Recreate database
dotnet ef database drop
dotnet ef database update
```

#### Frontend Issues

**Issue**: "Module not found: Can't resolve 'module'"
```bash
# Solution: Clear node modules and reinstall
rm -rf node_modules package-lock.json
npm install
```

**Issue**: "Port 3000 is already in use"
```bash
# Solution: Kill process or use different port
lsof -ti:3000 | xargs kill -9
# or
npm run dev -- -p 3001
```

**Issue**: "TypeScript compilation errors"
```bash
# Solution: Check TypeScript configuration
npx tsc --noEmit
```

### Performance Issues

#### Slow API Response Times
1. Check database queries in SQL Profiler
2. Verify indexes are properly configured
3. Enable response compression
4. Check for N+1 query problems

#### Slow Frontend Build Times
1. Enable Next.js build optimization
2. Check for large dependencies
3. Use dynamic imports for heavy components
4. Enable bundle analyzer

### Debugging Tips

#### Backend Debugging
```json
// .vscode/launch.json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/Api/bin/Debug/net8.0/Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/Api",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      }
    }
  ]
}
```

#### Frontend Debugging
```typescript
// Add debug breakpoints
console.log('Debug data:', data);
debugger; // Browser will pause here

// Use React DevTools
// Install browser extension for React debugging
```

---

## Development Best Practices

### Code Organization

#### Backend
- Use dependency injection for services
- Implement repository pattern for data access
- Use DTOs for API contracts
- Separate validation logic
- Follow C# naming conventions

#### Frontend
- Use TypeScript for all new code
- Separate server and client components
- Use custom hooks for reusable logic
- Implement proper error boundaries
- Follow React and Next.js best practices

### Git Workflow

#### Branch Naming
```bash
feature/client-management
bugfix/search-pagination
hotfix/security-patch
```

#### Commit Messages
```bash
feat: add client search functionality
fix: resolve pagination bug
docs: update API documentation
test: add unit tests for client service
```

#### Pull Request Template
```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
```

### Environment Management

#### Development Variables
```bash
# .env.development
NEXT_PUBLIC_API_BASE_URL=http://localhost:5190/api
NEXT_PUBLIC_ENABLE_DEBUG=true
```

#### Production Variables
```bash
# .env.production
NEXT_PUBLIC_API_BASE_URL=https://api.crmapi.com/api
NEXT_PUBLIC_ENABLE_DEBUG=false
```

---

## Performance Optimization

### Backend Optimization

#### Database Optimization
```csharp
// Use indexes for frequently queried fields
builder.HasIndex(c => c.Email).IsUnique();
builder.HasIndex(c => new { c.FirstName, c.LastName });

// Use projection for API responses
var contacts = await _context.Contacts
    .Select(c => new ContactListItemDto
    {
        Id = c.Id,
        FirstName = c.FirstName,
        // ... other properties
    })
    .ToListAsync();
```

#### Caching Strategy
```csharp
// Add response caching
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public async Task<IActionResult> GetContacts()
{
    // Implementation
}
```

### Frontend Optimization

#### Code Splitting
```typescript
// Dynamic imports for heavy components
const ContactForm = dynamic(() => import('./ContactForm'), {
  loading: () => <div>Loading...</div>,
  ssr: false
});
```

#### Image Optimization
```typescript
import Image from 'next/image';

<Image
  src="/contact-logo.png"
  alt="Contact Logo"
  width={100}
  height={100}
  priority
/>
```

---

## Security Best Practices

### Backend Security
```csharp
// Input validation
public class CreateContactValidator : AbstractValidator<CreateContactDto>
{
    public CreateContactValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Phone).Matches(@"^\d{3}-\d{3}-\d{4}$");
    }
}

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Frontend Security
```typescript
// Content Security Policy
const securityHeaders = [
  {
    key: 'Content-Security-Policy',
    value: "default-src 'self'; script-src 'self' 'unsafe-eval';"
  }
];

// Input sanitization
import DOMPurify from 'dompurify';

const cleanHtml = DOMPurify.sanitize(userInput);
```

---

## Deployment Preparation

### Build Commands

#### Backend
```bash
dotnet publish -c Release -o ./publish
```

#### Frontend
```bash
npm run build
npm run export  # For static export (optional)
```

### Docker Setup

#### Dockerfile (Backend)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Api/Api.csproj", "src/Api/"]
RUN dotnet restore "src/Api/Api.csproj"
COPY . .
WORKDIR "/src/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
```

#### Dockerfile (Frontend)
```dockerfile
FROM node:18-alpine AS base
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production

FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM base AS production
COPY --from=build /app/.next ./.next
COPY --from=build /app/public ./public
EXPOSE 3000
CMD ["npm", "start"]
```

---

## Resources

### Documentation
- [Next.js Documentation](https://nextjs.org/docs)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Material UI Documentation](https://mui.com/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

### Tools
- [Postman](https://www.postman.com/) - API testing
- [Insomnia](https://insomnia.rest/) - REST client
- [DBeaver](https://dbeaver.io/) - Database management
- [GitKraken](https://www.gitkraken.com/) - Git GUI

### Learning Resources
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [React Documentation](https://react.dev/)
- [C# Guide](https://docs.microsoft.com/dotnet/csharp/)
- [LINQ Examples](https://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811e)

---

This setup guide should help you get the PEO Clients development environment running quickly and efficiently. If you encounter any issues not covered here, please check the troubleshooting section or reach out to the development team.
