# Product Documentation

## Document Information
- **Product Name**: CRM API
- **Version**: 1.0
- **Last Updated**: November 16, 2025
- **Product Type**: RESTful Web API

## Executive Summary

The CRM API is a modern, cloud-ready client relationship management system that enables businesses to efficiently manage customer information through a RESTful API interface. Built on ASP.NET Core 8.0, it provides comprehensive CRUD operations for client data with integrated third-party contact synchronization via Loops.so.

### Key Value Propositions

1. **Developer-First**: Clean, intuitive RESTful API design with comprehensive Swagger documentation
2. **Enterprise-Ready**: Scalable architecture supporting horizontal scaling and high availability
3. **Integration-Friendly**: Built-in Loops.so integration with extensible service layer for additional integrations
4. **Production-Ready**: Comprehensive validation, error handling, logging, and security features
5. **Flexible Deployment**: Supports in-memory database for testing, SQL Server for production

## Target Audience

### Primary Users
- **Full-Stack Developers**: Building web/mobile applications requiring client management
- **Backend Developers**: Integrating CRM functionality into existing systems
- **API Consumers**: Third-party applications accessing client data

### Secondary Users
- **DevOps Engineers**: Deploying and maintaining the API infrastructure
- **QA Engineers**: Testing client management functionality
- **Product Managers**: Understanding capabilities and roadmap

## Core Features

### 1. Client Management

#### Create Client
- Add new clients with comprehensive contact information
- Automatic validation of input data (email format, phone format, required fields)
- Duplicate email detection and conflict prevention
- Auto-generated unique identifiers (GUID)
- Automatic timestamps (CreatedAt, UpdatedAt)

**Use Cases**:
- User registration/signup flows
- Manual client entry by sales/support teams
- Bulk client imports from external systems

#### Read Client (Single)
- Retrieve complete client details by unique ID
- Fast lookup with indexed database queries
- Returns all client attributes including address and contact info

**Use Cases**:
- Client profile views
- Pre-fill forms for client updates
- Display client details in dashboards

#### List Clients (Collection)
- Paginated client list with configurable page size (1-100 items)
- Full-text search across name, email, phone, company
- Filter by active/inactive status
- Sort by multiple fields (name, email, company, creation date)
- Ascending/descending sort directions

**Use Cases**:
- Client directory/listing pages
- Search functionality in CRM interfaces
- Admin panels for client management
- Reports and analytics dashboards

#### Update Client
- Modify existing client information
- Validation of all updated fields
- Duplicate email detection (excluding current client)
- Automatic timestamp update (UpdatedAt)

**Use Cases**:
- Client profile editing
- Data correction/maintenance
- Status changes (activate/deactivate)

#### Delete Client
- Permanently remove client records
- Hard delete (not soft delete)
- Cascade considerations for future related data

**Use Cases**:
- GDPR "right to be forgotten" compliance
- Test data cleanup
- Duplicate removal

### 2. Loops.so Contact Synchronization

#### Automatic Sync on Client Creation
- Non-blocking contact creation in Loops.so
- Fire-and-forget pattern ensures fast API responses
- Syncs email, first name, last name, and client ID
- Comprehensive error handling with graceful degradation

**Use Cases**:
- Marketing automation trigger
- Email list building
- Lead nurturing workflows
- Customer onboarding sequences

#### Configuration-Driven
- Feature flag to enable/disable sync
- Environment-specific API keys
- Configurable timeout and source identification
- Supports development and production environments

**Use Cases**:
- Development/testing without external calls
- Production sync with real marketing lists
- Multi-environment deployments

### 3. Search & Filter Capabilities

#### Full-Text Search
Search across multiple fields:
- First Name
- Last Name
- Email
- Phone Number
- Company Name

**Features**:
- Case-insensitive matching
- Partial string matching
- OR logic across fields (match any field)

**Use Cases**:
- Quick client lookup by any known attribute
- Fuzzy search for support/sales teams
- Find clients without knowing exact spelling

#### Status Filtering
- Filter by `isActive` status (true/false)
- Exclude inactive clients from views
- Display only archived clients

**Use Cases**:
- Active client reports
- Cleanup of inactive accounts
- Audit inactive clients before deletion

#### Advanced Sorting
Sortable fields:
- First Name
- Last Name
- Email
- Company
- Creation Date

**Features**:
- Ascending/descending order
- Composite sort (lastName, then firstName)
- Consistent ordering for pagination

**Use Cases**:
- Alphabetical client listings
- Recent client reports (newest first)
- Company-based grouping

### 4. Validation & Data Integrity

#### Input Validation (FluentValidation)
- Email format validation (RFC 5322 compliant)
- Phone format validation (XXX-XXX-XXXX)
- Maximum length enforcement (prevents database overflow)
- Required field checks
- Custom validation rules

**Features**:
- Client-side friendly error messages
- Field-specific error details
- HTTP 400 Bad Request with error dictionary

**Use Cases**:
- Prevent invalid data entry
- Provide immediate feedback to users
- Maintain data quality standards

#### Business Rule Enforcement
- Unique email constraint (across all clients)
- HTTP 409 Conflict response for duplicates
- Atomic database transactions

**Use Cases**:
- Prevent duplicate client accounts
- Ensure data consistency
- Enforce business policies

### 5. API Documentation (Swagger/OpenAPI)

#### Interactive Documentation
- Auto-generated from code
- Try-it-out functionality
- Request/response examples
- Schema definitions
- Authentication details

**Access**: `https://localhost:5001/` (root URL in development)

**Features**:
- Explore all endpoints visually
- Test API calls directly from browser
- View request/response models
- Download OpenAPI spec (JSON/YAML)

**Use Cases**:
- Developer onboarding
- API exploration
- Integration testing
- Client SDK generation

## User Workflows

### Workflow 1: Create New Client (Web Application)

**Actors**: End User, Web Application, CRM API, Loops.so

**Steps**:
1. User fills out client registration form (name, email, phone, company, address)
2. Web app validates input client-side (basic checks)
3. Web app sends POST request to `/api/clients` with JSON payload
4. CRM API validates input with FluentValidation
5. CRM API checks for duplicate email in database
6. If valid: CRM API saves client to database
7. CRM API generates unique ID and timestamps
8. CRM API returns 201 Created with Location header and client details
9. Web app displays success message and redirects to client profile
10. **Background**: CRM API triggers Loops.so contact sync (non-blocking)
11. **Background**: Loops.so receives contact and adds to mailing list

**Success Outcome**: Client created in CRM, contact synced to Loops.so, user sees confirmation

**Error Scenarios**:
- Invalid email format → 400 Bad Request with error details
- Duplicate email → 409 Conflict with message
- Server error → 500 Internal Server Error (log for investigation)

---

### Workflow 2: Search for Client (Support Interface)

**Actors**: Support Agent, Support Application, CRM API

**Steps**:
1. Support agent enters search term in search box (e.g., "john" or "acme")
2. Support app sends GET request to `/api/clients?query=john&page=1&pageSize=20`
3. CRM API searches across firstName, lastName, email, phone, company fields
4. CRM API applies pagination (first 20 results)
5. CRM API returns 200 OK with matching clients and pagination metadata
6. Support app displays search results in table
7. Agent clicks on client to view full details
8. Support app sends GET request to `/api/clients/{id}`
9. CRM API returns 200 OK with complete client details
10. Support app displays client profile

**Success Outcome**: Agent finds client quickly and views complete information

**Edge Cases**:
- No results found → Empty items array, total = 0
- Large result set → Pagination handles display
- Partial match → All clients with substring match returned

---

### Workflow 3: Update Client Information (Admin Panel)

**Actors**: Admin User, Admin Application, CRM API

**Steps**:
1. Admin navigates to client list and selects client to edit
2. Admin app fetches client details: GET `/api/clients/{id}`
3. CRM API returns 200 OK with current client data
4. Admin app pre-fills edit form with current values
5. Admin modifies fields (e.g., updates company name, changes email, marks inactive)
6. Admin submits update form
7. Admin app sends PUT request to `/api/clients/{id}` with updated JSON
8. CRM API validates updated data
9. CRM API checks for email conflicts (if email changed)
10. If valid: CRM API updates database record and UpdatedAt timestamp
11. CRM API returns 200 OK with updated client details
12. Admin app displays success message and refreshed data

**Success Outcome**: Client information updated, audit trail captured via timestamps

**Error Scenarios**:
- Email changed to existing email → 409 Conflict
- Invalid data format → 400 Bad Request
- Client not found → 404 Not Found

---

### Workflow 4: Bulk Import Clients (ETL Process)

**Actors**: ETL Script, CRM API, Source Database

**Steps**:
1. ETL script extracts client data from legacy system (CSV, database query)
2. ETL transforms data to match CRM API schema (field mapping, format conversion)
3. For each client record:
   a. ETL sends POST request to `/api/clients` with client data
   b. CRM API validates and creates client
   c. CRM API returns 201 Created
   d. ETL logs success and client ID
4. If duplicate email encountered: ETL logs 409 Conflict and skips record
5. ETL script generates summary report (created count, skipped count, errors)

**Success Outcome**: Historical client data migrated to new CRM system

**Considerations**:
- Rate limiting (future: batch endpoint)
- Duplicate handling strategy
- Loops.so integration (may want to disable during bulk import)

## Technical Specifications

### System Requirements

#### Server
- **Operating System**: Windows Server 2019+, Linux (Ubuntu 20.04+), macOS 10.15+
- **Runtime**: .NET 8 Runtime or SDK
- **Memory**: 512 MB minimum, 2 GB recommended
- **CPU**: 1 core minimum, 2+ cores recommended
- **Storage**: 10 GB minimum (database dependent)

#### Database
- **Development**: In-memory (no installation required)
- **Production**: SQL Server 2019+, Azure SQL Database, PostgreSQL 12+, MySQL 8+

#### Network
- **Inbound**: HTTPS port 443 (or custom)
- **Outbound**: HTTPS to Loops.so API (app.loops.so)

### Performance Characteristics

#### Response Times (Expected)
| Endpoint | Avg Response Time | 95th Percentile |
|----------|-------------------|-----------------|
| GET /api/clients/{id} | 10-50 ms | 100 ms |
| GET /api/clients (list) | 50-150 ms | 300 ms |
| POST /api/clients | 50-150 ms | 300 ms |
| PUT /api/clients/{id} | 50-150 ms | 300 ms |
| DELETE /api/clients/{id} | 30-100 ms | 200 ms |

**Note**: Times exclude Loops.so sync (non-blocking)

#### Throughput
- **Expected**: 100-500 requests per second (per instance)
- **Scalability**: Horizontal (add instances behind load balancer)

#### Database
- **Sample Data**: 150 clients pre-seeded
- **Index Performance**: O(log n) for ID and email lookups
- **Query Performance**: Optimized with AutoMapper projections

### Security Features

#### Current Implementation
- **HTTPS**: TLS 1.2+ encryption in transit
- **Input Validation**: FluentValidation prevents injection attacks
- **SQL Injection Protection**: Entity Framework parameterized queries
- **CORS**: Configurable allowed origins
- **Secrets Management**: Configuration-based (environment variables, user secrets)

#### Recommended Enhancements
- **Authentication**: JWT Bearer tokens, OAuth 2.0
- **Authorization**: Role-based access control (RBAC)
- **Rate Limiting**: IP-based throttling
- **API Keys**: Per-client authentication
- **Audit Logging**: Track all data modifications

### Compliance & Privacy

#### GDPR Compliance
- **Right to Access**: GET endpoints provide data export
- **Right to Rectification**: PUT endpoint allows updates
- **Right to Erasure**: DELETE endpoint enables hard deletion
- **Data Portability**: JSON responses support export

#### PII Handling
**Personal Data Stored**:
- Name (FirstName, LastName)
- Email
- Phone
- Address (Line1, Line2, City, State, PostalCode, Country)

**Recommendations**:
- Encrypt database at rest (TDE)
- Encrypt backups
- Implement data retention policy
- Document data processing in privacy policy
- Obtain consent for Loops.so data sharing

## Deployment Guide

### Development Deployment

#### Prerequisites
1. .NET 8 SDK installed
2. Visual Studio 2022 or VS Code
3. Git for version control

#### Steps
```bash
# Clone repository
git clone https://github.com/Benjamin-FC/CoPilot.git
cd CoPilot

# Restore dependencies
dotnet restore

# Configure Loops.so (optional)
dotnet user-secrets set "Loops:ApiKey" "your_api_key_here" --project src/Api

# Run application
cd src/Api
dotnet run

# Access Swagger UI
# Navigate to: https://localhost:5001/
```

### Production Deployment (Azure App Service)

#### Prerequisites
1. Azure subscription
2. Azure CLI installed
3. Docker installed (optional, for containers)

#### Option A: Direct Deployment
```bash
# Login to Azure
az login

# Create resource group
az group create --name crm-api-rg --location eastus

# Create App Service plan
az appservice plan create \
  --name crm-api-plan \
  --resource-group crm-api-rg \
  --sku B1 \
  --is-linux

# Create web app
az webapp create \
  --name crm-api-prod \
  --resource-group crm-api-rg \
  --plan crm-api-plan \
  --runtime "DOTNETCORE:8.0"

# Configure app settings
az webapp config appsettings set \
  --name crm-api-prod \
  --resource-group crm-api-rg \
  --settings \
    Loops__ApiKey="your_loops_api_key" \
    Loops__Enabled="true"

# Deploy application
dotnet publish -c Release
az webapp deployment source config-zip \
  --name crm-api-prod \
  --resource-group crm-api-rg \
  --src publish.zip
```

#### Option B: Container Deployment
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

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

```bash
# Build Docker image
docker build -t crm-api:1.0 .

# Run container locally
docker run -d -p 8080:80 \
  -e Loops__ApiKey="your_key" \
  -e Loops__Enabled="true" \
  crm-api:1.0

# Push to Azure Container Registry
az acr create --name crmapiacr --resource-group crm-api-rg --sku Basic
docker tag crm-api:1.0 crmapiacr.azurecr.io/crm-api:1.0
docker push crmapiacr.azurecr.io/crm-api:1.0

# Deploy to Azure Web App
az webapp create \
  --name crm-api-prod \
  --resource-group crm-api-rg \
  --plan crm-api-plan \
  --deployment-container-image-name crmapiacr.azurecr.io/crm-api:1.0
```

### Database Configuration

#### In-Memory (Development)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "InMemory"
  }
}
```

#### SQL Server (Production)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:crm-db-server.database.windows.net,1433;Database=CrmDb;User ID=admin;Password=SecurePass123!;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

Update `Program.cs`:
```csharp
// Replace UseInMemoryDatabase with UseSqlServer
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Run migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Configuration Reference

### appsettings.json (Full)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "InMemory"
  },
  "Loops": {
    "ApiKey": "",
    "BaseUrl": "https://app.loops.so/api/v1",
    "Enabled": false,
    "DefaultSource": "CRM API",
    "TimeoutSeconds": 30
  },
  "Cors": {
    "AllowedOrigins": ["https://app.example.com"]
  }
}
```

### Environment Variables
```bash
# Loops Configuration
LOOPS__APIKEY=loops_sk_abc123xyz
LOOPS__ENABLED=true

# Database Connection
CONNECTIONSTRINGS__DEFAULTCONNECTION="Server=..."

# Logging
LOGGING__LOGLEVEL__DEFAULT=Information
```

## Monitoring & Operations

### Health Checks (Recommended)
```csharp
// Add to Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddUrlGroup(new Uri("https://app.loops.so"), name: "loops-api");

app.MapHealthChecks("/health");
```

### Logging
- **Framework**: Microsoft.Extensions.Logging
- **Providers**: Console (development), File/Cloud (production)
- **Levels**: Debug, Information, Warning, Error, Critical

**Key Log Events**:
- Client creation/update/deletion
- Loops.so sync attempts and results
- Validation errors
- Unhandled exceptions

### Metrics (Recommended)
- Request count by endpoint
- Response time percentiles (p50, p95, p99)
- Error rate by status code
- Database query duration
- Loops.so sync success rate

### Alerting (Recommended)
- API error rate > 5% for 5 minutes
- Response time p95 > 1 second for 10 minutes
- Database connection failures
- Loops.so sync failure rate > 10%

## Troubleshooting

### Issue: 500 Internal Server Error

**Symptoms**: Unexpected errors, no specific error message

**Diagnosis**:
1. Check application logs for exception stack traces
2. Verify database connectivity
3. Check configuration settings

**Solution**:
- Review exception details in logs
- Verify connection strings
- Check database server status

---

### Issue: Slow API Responses

**Symptoms**: Requests taking > 1 second

**Diagnosis**:
1. Check database query execution plans
2. Review index usage
3. Monitor server resource utilization (CPU, memory)

**Solution**:
- Rebuild fragmented indexes
- Update database statistics
- Scale up/out server resources
- Enable query caching

---

### Issue: Loops.so Not Syncing

**Symptoms**: Clients created but not in Loops.so

**Diagnosis**:
1. Check `Loops:Enabled` configuration
2. Verify `Loops:ApiKey` is set
3. Review application logs for sync errors

**Solution**:
- Set `Enabled = true`
- Provide valid API key
- Check network connectivity to Loops.so

## Roadmap & Future Enhancements

### Version 1.1 (Q1 2026)
- [ ] Authentication (JWT Bearer tokens)
- [ ] API key management
- [ ] Rate limiting
- [ ] Batch client operations
- [ ] PATCH endpoint for partial updates

### Version 1.2 (Q2 2026)
- [ ] Client tags (many-to-many)
- [ ] Client notes
- [ ] Client activity timeline
- [ ] Advanced search filters
- [ ] Export to CSV/Excel

### Version 2.0 (Q3 2026)
- [ ] Bidirectional Loops.so sync
- [ ] Webhooks for client events
- [ ] Custom fields
- [ ] Audit trail
- [ ] Multi-tenancy support

## Support & Resources

### Documentation
- **Architecture**: `docs/architecture/overview.md`
- **API Design**: `docs/design/api-design.md`
- **Database Schema**: `docs/design/database-schema.md`
- **Loops Integration**: `docs/design/integration-loops.md`
- **API Reference**: Swagger UI at root URL

### Code Repository
- **GitHub**: https://github.com/Benjamin-FC/CoPilot
- **Branch**: master

### Contact
- **Email**: dev@example.com
- **Issue Tracker**: GitHub Issues

## Appendix

### Glossary

- **CRM**: Customer Relationship Management
- **DTO**: Data Transfer Object
- **GUID**: Globally Unique Identifier (UUID)
- **REST**: Representational State Transfer
- **CRUD**: Create, Read, Update, Delete
- **EF Core**: Entity Framework Core (ORM)
- **PII**: Personally Identifiable Information
- **GDPR**: General Data Protection Regulation

### API Quick Reference

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/clients` | GET | List clients (paginated) |
| `/api/clients/{id}` | GET | Get single client |
| `/api/clients` | POST | Create client |
| `/api/clients/{id}` | PUT | Update client |
| `/api/clients/{id}` | DELETE | Delete client |

### Sample Requests

See `docs/design/api-design.md` for comprehensive examples with all query parameters and request/response payloads.
