# API Design Specification

## Document Information
- **Version**: 1.0
- **Last Updated**: November 16, 2025
- **API Version**: v1

## Design Philosophy

### RESTful Principles
The CRM API adheres to REST architectural constraints:

1. **Resource-Based URLs**: `/api/clients`, `/api/clients/{id}`
2. **HTTP Methods**: Standard CRUD operations (GET, POST, PUT, DELETE)
3. **Stateless**: Each request contains all necessary information
4. **Uniform Interface**: Consistent patterns across all endpoints
5. **JSON Format**: Content negotiation with `application/json`

### Design Goals
- **Developer-Friendly**: Intuitive, predictable endpoints
- **Consistency**: Uniform response structures and error handling
- **Flexibility**: Rich query capabilities (search, filter, sort, paginate)
- **Extensibility**: Easy to add new resources and features
- **Documentation**: Self-documenting via Swagger/OpenAPI

## Base URL

### Development
```
https://localhost:5001/api
http://localhost:5000/api
```

### Production (Example)
```
https://api.crm.example.com/api
```

## API Endpoints

### 1. List Clients

**Endpoint**: `GET /api/clients`

**Description**: Retrieve a paginated list of clients with optional filtering, searching, and sorting.

**Query Parameters**:
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `query` | string | - | Search term (firstName, lastName, email, phone, company) |
| `page` | integer | 1 | Page number (min: 1) |
| `pageSize` | integer | 10 | Items per page (min: 1, max: 100) |
| `sort` | string | "lastName" | Sort field (firstName, lastName, email, company, createdAt) |
| `dir` | string | "asc" | Sort direction (asc, desc) |
| `isActive` | boolean | - | Filter by active status |

**Request Example**:
```http
GET /api/clients?query=smith&page=1&pageSize=20&sort=lastName&dir=asc&isActive=true
```

**Response**: `200 OK`
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "firstName": "John",
      "lastName": "Smith",
      "email": "john.smith@example.com",
      "phone": "555-123-4567",
      "company": "Acme Corp",
      "isActive": true,
      "createdAt": "2025-01-15T10:30:00Z"
    }
  ],
  "total": 42,
  "page": 1,
  "pageSize": 20,
  "sort": "lastName",
  "dir": "asc"
}
```

**Response Fields**:
- `items`: Array of client summary objects
- `total`: Total number of matching clients
- `page`: Current page number
- `pageSize`: Number of items per page
- `sort`: Applied sort field
- `dir`: Applied sort direction

**Status Codes**:
- `200 OK`: Success

---

### 2. Get Client by ID

**Endpoint**: `GET /api/clients/{id}`

**Description**: Retrieve detailed information about a specific client.

**Path Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | UUID | Yes | Client unique identifier |

**Request Example**:
```http
GET /api/clients/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "555-123-4567",
  "company": "Acme Corp",
  "addressLine1": "123 Main Street",
  "addressLine2": "Suite 100",
  "city": "New York",
  "state": "NY",
  "postalCode": "10001",
  "country": "USA",
  "isActive": true,
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-11-16T14:20:00Z"
}
```

**Status Codes**:
- `200 OK`: Client found and returned
- `404 Not Found`: Client with specified ID does not exist

**Error Response**: `404 Not Found`
```json
{
  "message": "Client with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found."
}
```

---

### 3. Create Client

**Endpoint**: `POST /api/clients`

**Description**: Create a new client record and optionally sync to Loops.so.

**Request Headers**:
```
Content-Type: application/json
```

**Request Body**:
```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "phone": "555-987-6543",
  "company": "Tech Innovations",
  "addressLine1": "456 Oak Avenue",
  "addressLine2": "Floor 3",
  "city": "San Francisco",
  "state": "CA",
  "postalCode": "94102",
  "country": "USA"
}
```

**Required Fields**:
- `firstName` (string, max 100 chars)
- `lastName` (string, max 100 chars)
- `email` (string, max 255 chars, valid email format)

**Optional Fields**:
- `phone` (string, max 20 chars, format: XXX-XXX-XXXX)
- `company` (string, max 255 chars)
- `addressLine1` (string, max 255 chars)
- `addressLine2` (string, max 255 chars)
- `city` (string, max 100 chars)
- `state` (string, max 100 chars)
- `postalCode` (string, max 20 chars)
- `country` (string, max 100 chars)

**Response**: `201 Created`
```json
{
  "id": "7b9e4f82-8d3a-4c91-b2f5-1e6a9c7d4b3a",
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "phone": "555-987-6543",
  "company": "Tech Innovations",
  "addressLine1": "456 Oak Avenue",
  "addressLine2": "Floor 3",
  "city": "San Francisco",
  "state": "CA",
  "postalCode": "94102",
  "country": "USA",
  "isActive": true,
  "createdAt": "2025-11-16T15:45:00Z",
  "updatedAt": "2025-11-16T15:45:00Z"
}
```

**Response Headers**:
```
Location: /api/clients/7b9e4f82-8d3a-4c91-b2f5-1e6a9c7d4b3a
```

**Status Codes**:
- `201 Created`: Client successfully created
- `400 Bad Request`: Validation errors
- `409 Conflict`: Email already exists

**Error Response**: `400 Bad Request`
```json
{
  "errors": {
    "Email": ["Email must be a valid email address."],
    "FirstName": ["First name is required."]
  }
}
```

**Error Response**: `409 Conflict`
```json
{
  "message": "A client with this email already exists."
}
```

**Side Effects**:
- Generates unique `id` (UUID)
- Sets `createdAt` and `updatedAt` timestamps
- Sets `isActive` to `true` by default
- Asynchronously syncs contact to Loops.so (non-blocking)

---

### 4. Update Client

**Endpoint**: `PUT /api/clients/{id}`

**Description**: Update an existing client's information.

**Path Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | UUID | Yes | Client unique identifier |

**Request Headers**:
```
Content-Type: application/json
```

**Request Body**:
```json
{
  "firstName": "Jane",
  "lastName": "Doe-Smith",
  "email": "jane.doe-smith@example.com",
  "phone": "555-987-6543",
  "company": "Tech Innovations Inc",
  "addressLine1": "456 Oak Avenue",
  "addressLine2": "Floor 5",
  "city": "San Francisco",
  "state": "CA",
  "postalCode": "94102",
  "country": "USA",
  "isActive": true
}
```

**Required Fields**: All fields from CreateClientDto plus:
- `isActive` (boolean)

**Response**: `200 OK`
```json
{
  "id": "7b9e4f82-8d3a-4c91-b2f5-1e6a9c7d4b3a",
  "firstName": "Jane",
  "lastName": "Doe-Smith",
  "email": "jane.doe-smith@example.com",
  "phone": "555-987-6543",
  "company": "Tech Innovations Inc",
  "addressLine1": "456 Oak Avenue",
  "addressLine2": "Floor 5",
  "city": "San Francisco",
  "state": "CA",
  "postalCode": "94102",
  "country": "USA",
  "isActive": true,
  "createdAt": "2025-11-16T15:45:00Z",
  "updatedAt": "2025-11-16T16:30:00Z"
}
```

**Status Codes**:
- `200 OK`: Client successfully updated
- `400 Bad Request`: Validation errors
- `404 Not Found`: Client does not exist
- `409 Conflict`: Email conflicts with another client

**Side Effects**:
- Updates `updatedAt` timestamp

---

### 5. Delete Client

**Endpoint**: `DELETE /api/clients/{id}`

**Description**: Permanently delete a client record.

**Path Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | UUID | Yes | Client unique identifier |

**Request Example**:
```http
DELETE /api/clients/7b9e4f82-8d3a-4c91-b2f5-1e6a9c7d4b3a
```

**Response**: `204 No Content`

**Status Codes**:
- `204 No Content`: Client successfully deleted
- `404 Not Found`: Client does not exist

**Error Response**: `404 Not Found`
```json
{
  "message": "Client with ID 7b9e4f82-8d3a-4c91-b2f5-1e6a9c7d4b3a not found."
}
```

---

## Data Models

### ClientListItemDto (Summary)
```typescript
{
  id: string;              // UUID
  firstName: string;       // Max 100 chars
  lastName: string;        // Max 100 chars
  email: string;           // Max 255 chars
  phone?: string;          // Max 20 chars
  company?: string;        // Max 255 chars
  isActive: boolean;       // Default: true
  createdAt: string;       // ISO 8601 datetime
}
```

### ClientDetailDto (Full)
```typescript
{
  id: string;              // UUID
  firstName: string;       // Max 100 chars
  lastName: string;        // Max 100 chars
  email: string;           // Max 255 chars
  phone?: string;          // Max 20 chars
  company?: string;        // Max 255 chars
  addressLine1?: string;   // Max 255 chars
  addressLine2?: string;   // Max 255 chars
  city?: string;           // Max 100 chars
  state?: string;          // Max 100 chars
  postalCode?: string;     // Max 20 chars
  country?: string;        // Max 100 chars
  isActive: boolean;       // Default: true
  createdAt: string;       // ISO 8601 datetime
  updatedAt: string;       // ISO 8601 datetime
}
```

### ClientListResponse
```typescript
{
  items: ClientListItemDto[];
  total: number;           // Total matching records
  page: number;            // Current page
  pageSize: number;        // Items per page
  sort: string;            // Sort field
  dir: string;             // Sort direction (asc/desc)
}
```

## Error Handling

### Standard Error Response
```json
{
  "message": "Error description"
}
```

### Validation Error Response
```json
{
  "errors": {
    "FieldName": ["Error message 1", "Error message 2"]
  }
}
```

### HTTP Status Codes
| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful GET, PUT |
| 201 | Created | Successful POST |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Validation errors, malformed request |
| 404 | Not Found | Resource does not exist |
| 409 | Conflict | Duplicate resource (email) |
| 500 | Internal Server Error | Unexpected server error |

## Validation Rules

### Email Validation
- Required for create/update
- Maximum length: 255 characters
- Must match email format pattern
- Must be unique across all clients

### Phone Validation
- Optional
- Maximum length: 20 characters
- Format: XXX-XXX-XXXX (e.g., 555-123-4567)

### Name Validation
- FirstName and LastName required
- Maximum length: 100 characters each

### Text Field Validation
- Company, Address lines, City, State, Country, PostalCode all optional
- Maximum lengths vary (see Data Models section)

## Pagination Design

### Default Behavior
- Default page: 1
- Default pageSize: 10
- Maximum pageSize: 100
- Minimum page: 1
- Invalid values reset to defaults

### Calculation
```
Skip = (page - 1) × pageSize
Take = pageSize
```

### Response Metadata
Always includes:
- `total`: Total matching records
- `page`: Current page number
- `pageSize`: Items in current page

## Sorting Design

### Supported Sort Fields
- `firstName`
- `lastName`
- `email`
- `company`
- `createdAt`

### Sort Direction
- `asc`: Ascending (A-Z, oldest-newest)
- `desc`: Descending (Z-A, newest-oldest)

### Default Sort
- Field: `lastName`
- Direction: `asc`
- Secondary: `firstName` (asc)

## Search/Filter Design

### Search Query (`query` parameter)
Searches across multiple fields (case-insensitive):
- `firstName`
- `lastName`
- `email`
- `phone`
- `company`

### Filter (`isActive` parameter)
- `true`: Only active clients
- `false`: Only inactive clients
- Omit: All clients

## CORS Configuration

### Development
```
Allowed Origins:
  - http://localhost:3000
  - http://127.0.0.1:3000

Allowed Methods: All
Allowed Headers: All
```

### Production
Recommended: Restrict to specific origins

## Rate Limiting

**Current**: None

**Recommended**: 
- 100 requests per minute per IP
- 1000 requests per hour per API key

## Versioning Strategy

### Current
- Version: v1
- No version in URL (implicit)

### Future
- URL versioning: `/api/v2/clients`
- Header versioning: `Accept: application/vnd.crm.v2+json`

## Content Negotiation

### Request Headers
```
Content-Type: application/json
```

### Response Headers
```
Content-Type: application/json; charset=utf-8
```

## Swagger/OpenAPI

### Access
- **URL**: `https://localhost:5001/` (root)
- **Enabled**: Development environment only

### Features
- Interactive API documentation
- Request/response examples
- Schema definitions
- Try-it-out functionality

## Design Patterns

### DTO Pattern
- Separate DTOs for create, update, list, detail
- Prevents over-posting attacks
- Clear API contracts

### Repository Pattern
- Entity Framework Core as repository abstraction
- DbContext manages data access

### Service Layer
- External integrations isolated in services (LoopsService)
- Testable, mockable interfaces

## Performance Considerations

### Database Queries
- Uses AutoMapper `ProjectTo` for efficient SQL generation
- Avoids N+1 query problems
- Indexed fields for common queries

### Async Operations
- All controller actions are async
- Non-blocking I/O operations

### Fire-and-Forget
- External API calls (Loops.so) don't block responses
- Background task with try-catch error handling

## Security Considerations

### Input Validation
- FluentValidation for all inputs
- Prevents SQL injection (EF Core parameterized queries)
- Prevents XSS (JSON serialization)

### HTTPS
- Required for production
- TLS 1.2+ encryption

### Authentication
- **Current**: None (open API)
- **Recommended**: JWT Bearer tokens, API keys

## Future API Enhancements

1. **Batch Operations**: Create/update multiple clients
2. **Partial Updates**: PATCH for selective field updates
3. **Bulk Delete**: Delete multiple clients by IDs
4. **Export**: CSV/Excel export of client list
5. **Advanced Filters**: Date ranges, multiple criteria
6. **Field Selection**: Sparse fieldsets (`?fields=id,email`)
7. **Relationships**: Tags, notes, activities linked to clients
8. **Webhooks**: Event notifications for client changes

## References

- [REST API Design Best Practices](https://stackoverflow.blog/2020/03/02/best-practices-for-rest-api-design/)
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines)
- [OpenAPI Specification](https://swagger.io/specification/)
