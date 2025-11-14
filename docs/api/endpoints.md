# API Endpoints Documentation

## Overview

The CRM API provides RESTful endpoints for managing contact data with full CRUD operations, advanced search capabilities, and comprehensive validation.

## Base URL

```
Development: http://localhost:5190/api
Production:  https://api.crmapi.com/api
```

## Authentication

Currently no authentication is required (development mode). Future versions will include JWT-based authentication.

## Common Headers

```http
Content-Type: application/json
Accept: application/json
```

## Common Response Format

### Success Responses
```json
{
  "data": "response data",
  "success": true,
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### Error Responses
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "FirstName": ["First name is required."],
    "Email": ["Invalid email format."]
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

---

## Endpoints

### 1. Get Contacts List

Retrieve a paginated list of contacts with optional search, sorting, and filtering.

#### Endpoint
```http
GET /api/contacts
```

#### Query Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| query | string | No | - | Search term to find contacts |
| page | integer | No | 1 | Page number (1-based) |
| pageSize | integer | No | 10 | Number of items per page |
| sort | string | No | lastName | Field to sort by |
| dir | string | No | asc | Sort direction (asc/desc) |
| isActive | boolean | No | - | Filter by active status |

#### Sort Field Options
- `firstName` - Sort by first name
- `lastName` - Sort by last name
- `email` - Sort by email address
- `company` - Sort by company name
- `createdAt` - Sort by creation date

#### Example Requests

```http
GET /api/contacts
GET /api/contacts?query=john&page=1&pageSize=25
GET /api/contacts?sort=company&dir=desc&isActive=true
GET /api/contacts?query=tech&sort=email&page=2&pageSize=50
```

#### Response Example

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "firstName": "John",
      "lastName": "Smith",
      "email": "john.smith@example.com",
      "company": "Tech Corp",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "total": 150,
  "page": 1,
  "pageSize": 10,
  "sort": "lastName",
  "dir": "asc"
}
```

#### Response Schema

```typescript
interface ContactListResponse {
  items: ContactListItemDto[];
  total: number;
  page: number;
  pageSize: number;
  sort?: string;
  dir?: 'asc' | 'desc';
}

interface ContactListItemDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  company?: string;
  isActive: boolean;
  createdAt: string;
}
```

---

### 2. Get Contact by ID

Retrieve detailed information for a specific contact.

#### Endpoint
```http
GET /api/contacts/{id}
```

#### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | string | Yes | Contact unique identifier |

#### Example Request

```http
GET /api/contacts/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

#### Response Example

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "555-123-4567",
  "company": "Tech Corp",
  "addressLine1": "123 Main St",
  "addressLine2": "Apt 4B",
  "city": "New York",
  "state": "NY",
  "postalCode": "10001",
  "country": "USA",
  "isActive": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-20T14:45:00Z"
}
```

#### Response Schema

```typescript
interface ContactDetailDto {
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

---

### 3. Create Contact

Create a new contact record. When a contact is successfully created, it will also be automatically synchronized to Loops.so for email marketing purposes (if enabled).

#### Endpoint
```http
POST /api/contacts
```

#### Request Body

```typescript
interface CreateContactDto {
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
  isActive?: boolean;
}
```

#### Validation Rules

| Field | Rules |
|-------|-------|
| firstName | Required, max 100 characters |
| lastName | Required, max 100 characters |
| email | Required, valid email format, max 255 characters |
| phone | Optional, format 555-555-5555, max 20 characters |
| company | Optional, max 200 characters |
| addressLine1 | Optional, max 255 characters |
| addressLine2 | Optional, max 255 characters |
| city | Optional, max 100 characters |
| state | Optional, max 100 characters |
| postalCode | Optional, max 20 characters |
| country | Optional, max 100 characters |
| isActive | Optional, boolean, defaults to true |

#### Example Request

```http
POST /api/contacts
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "phone": "555-987-6543",
  "company": "Digital Solutions",
  "addressLine1": "456 Oak Ave",
  "city": "Los Angeles",
  "state": "CA",
  "postalCode": "90001",
  "country": "USA",
  "isActive": true
}
```

#### Response Example

```json
{
  "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "phone": "555-987-6543",
  "company": "Digital Solutions",
  "addressLine1": "456 Oak Ave",
  "city": "Los Angeles",
  "state": "CA",
  "postalCode": "90001",
  "country": "USA",
  "isActive": true,
  "createdAt": "2024-01-20T15:30:00Z",
  "updatedAt": "2024-01-20T15:30:00Z"
}
```

#### Status Codes
- `201 Created` - Contact created successfully
- `400 Bad Request` - Validation errors
- `409 Conflict` - Email already exists

---

### 4. Update Contact

Update an existing contact record.

#### Endpoint
```http
PUT /api/contacts/{id}
```

#### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | string | Yes | Contact unique identifier |

#### Request Body

```typescript
interface UpdateContactDto {
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
}
```

#### Example Request

```http
PUT /api/contacts/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@newcompany.com",
  "phone": "555-123-9999",
  "company": "New Tech Corp",
  "isActive": true
}
```

#### Response Example

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@newcompany.com",
  "phone": "555-123-9999",
  "company": "New Tech Corp",
  "addressLine1": "123 Main St",
  "addressLine2": "Apt 4B",
  "city": "New York",
  "state": "NY",
  "postalCode": "10001",
  "country": "USA",
  "isActive": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-20T16:45:00Z"
}
```

#### Status Codes
- `200 OK` - Contact updated successfully
- `400 Bad Request` - Validation errors
- `404 Not Found` - Contact not found
- `409 Conflict` - Email already exists (for another contact)

---

### 5. Delete Contact

Delete a contact record (soft delete by setting isActive to false).

#### Endpoint
```http
DELETE /api/contacts/{id}
```

#### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | string | Yes | Contact unique identifier |

#### Example Request

```http
DELETE /api/contacts/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

#### Response

No content returned

#### Status Codes
- `204 No Content` - Contact deleted successfully
- `404 Not Found` - Contact not found

---

## Error Handling

### Common Error Responses

#### 400 Bad Request - Validation Errors
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "FirstName": ["First name is required."],
    "Email": ["Invalid email format.", "Email already exists."]
  },
  "timestamp": "2024-01-20T16:45:00Z"
}
```

#### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Contact with ID 'invalid-id' not found.",
  "timestamp": "2024-01-20T16:45:00Z"
}
```

#### 409 Conflict
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "A contact with email 'john@example.com' already exists.",
  "timestamp": "2024-01-20T16:45:00Z"
}
```

#### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "detail": "Database connection failed.",
  "timestamp": "2024-01-20T16:45:00Z"
}
```

---

## Rate Limiting

Currently no rate limiting is implemented in development. Production will include:
- 100 requests per minute per IP
- 1000 requests per hour per authenticated user

---

## CORS Configuration

### Development
```http
Access-Control-Allow-Origin: http://localhost:3000
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization
```

### Production
```http
Access-Control-Allow-Origin: https://crmapi.com
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization
```

---

## Pagination

### Request Parameters
- `page`: Page number (1-based, default: 1)
- `pageSize`: Items per page (default: 10, max: 100)

### Response Headers
```http
X-Pagination-Total-Count: 150
X-Pagination-Page-Count: 15
X-Pagination-Current-Page: 1
X-Pagination-Page-Size: 10
```

### Pagination Example
```http
GET /api/contacts?page=2&pageSize=25

# Response
X-Pagination-Total-Count: 150
X-Pagination-Page-Count: 6
X-Pagination-Current-Page: 2
X-Pagination-Page-Size: 25
```

---

## Search Functionality

### Search Fields
The search query searches across the following fields:
- `firstName` - Case-insensitive contains
- `lastName` - Case-insensitive contains
- `email` - Case-insensitive contains
- `phone` - Exact match
- `company` - Case-insensitive contains

### Search Examples
```http
# Search by name
GET /api/contacts?query=john

# Search by email domain
GET /api/contacts?query=@example.com

# Search by company
GET /api/contacts?query=tech

# Search by phone number
GET /api/contacts?query=555-123
```

---

## Sorting

### Sort Options
- `firstName` - Sort by first name (A-Z, case-insensitive)
- `lastName` - Sort by last name (A-Z, case-insensitive)
- `email` - Sort by email (A-Z, case-insensitive)
- `company` - Sort by company (A-Z, case-insensitive, nulls last)
- `createdAt` - Sort by creation date (newest/oldest first)

### Sort Examples
```http
# Sort by last name ascending
GET /api/contacts?sort=lastName&dir=asc

# Sort by creation date descending
GET /api/contacts?sort=createdAt&dir=desc

# Sort by company descending
GET /api/contacts?sort=company&dir=desc
```

---

## SDK Examples

### JavaScript/TypeScript

```typescript
// API client setup
const API_BASE_URL = 'http://localhost:5190/api';

class ContactsAPI {
  async getContacts(params?: ContactQueryParams): Promise<ContactListResponse> {
    const searchParams = new URLSearchParams(params as any).toString();
    const response = await fetch(`${API_BASE_URL}/contacts?${searchParams}`);
    return response.json();
  }

  async getContact(id: string): Promise<ContactDetailDto> {
    const response = await fetch(`${API_BASE_URL}/contacts/${id}`);
    return response.json();
  }

  async createContact(data: CreateContactDto): Promise<ContactDetailDto> {
    const response = await fetch(`${API_BASE_URL}/contacts`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    return response.json();
  }

  async updateContact(id: string, data: UpdateContactDto): Promise<ContactDetailDto> {
    const response = await fetch(`${API_BASE_URL}/contacts/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    return response.json();
  }

  async deleteContact(id: string): Promise<void> {
    await fetch(`${API_BASE_URL}/contacts/${id}`, {
      method: 'DELETE',
    });
  }
}

// Usage example
const api = new ContactsAPI();

// Get contacts with search and pagination
const contacts = await api.getContacts({
  query: 'john',
  page: 1,
  pageSize: 25,
  sort: 'lastName',
  dir: 'asc'
});

// Create new contact
const newContact = await api.createContact({
  firstName: 'John',
  lastName: 'Doe',
  email: 'john.doe@example.com',
  company: 'Tech Corp'
});
```

### C# HttpClient

```csharp
public class ContactsApiService
{
    private readonly HttpClient _httpClient;
    
    public ContactsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5190/api/");
    }
    
    public async Task<ContactListResponse> GetContactsAsync(
        string? query = null,
        int page = 1,
        int pageSize = 10,
        string sort = "lastName",
        string dir = "asc",
        bool? isActive = null)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["query"] = query,
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["sort"] = sort,
            ["dir"] = dir,
            ["isActive"] = isActive?.ToString()
        };
        
        var queryString = string.Join("&", 
            parameters.Where(p => !string.IsNullOrEmpty(p.Value))
                      .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!)}"));
        
        var response = await _httpClient.GetAsync($"contacts?{queryString}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<ContactListResponse>();
    }
    
    public async Task<ContactDetailDto?> GetContactAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"contacts/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
            
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ContactDetailDto>();
    }
    
    public async Task<ContactDetailDto> CreateContactAsync(CreateContactDto contact)
    {
        var response = await _httpClient.PostAsJsonAsync("contacts", contact);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ContactDetailDto>();
    }
    
    public async Task<ContactDetailDto> UpdateContactAsync(Guid id, UpdateContactDto contact)
    {
        var response = await _httpClient.PutAsJsonAsync($"contacts/{id}", contact);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ContactDetailDto>();
    }
    
    public async Task DeleteContactAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"contacts/{id}");
        response.EnsureSuccessStatusCode();
    }
}
```

---

## Testing

### Postman Collection

A Postman collection is available with all endpoints pre-configured:
- Environment variables for base URL
- Request examples for all endpoints
- Test scripts for response validation
- Authentication setup (when implemented)

### curl Examples

```bash
# Get all contacts
curl -X GET "http://localhost:5190/api/contacts" \
  -H "Accept: application/json"

# Search contacts
curl -X GET "http://localhost:5190/api/contacts?query=john&page=1&pageSize=25" \
  -H "Accept: application/json"

# Create contact
curl -X POST "http://localhost:5190/api/contacts" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "company": "Tech Corp"
  }'

# Update contact
curl -X PUT "http://localhost:5190/api/contacts/3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@newcompany.com",
    "company": "New Tech Corp",
    "isActive": true
  }'

# Delete contact
curl -X DELETE "http://localhost:5190/api/contacts/3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

## Versioning

API versioning is implemented using URL path versioning:
- Current version: `/api/v1/contacts`
- Backward compatibility: Previous versions maintained for 6 months
- Deprecation notices: Included in response headers

---

## Loops.so Integration

The API includes automatic synchronization with Loops.so for email marketing purposes.

### Configuration

Configure Loops.so integration in `appsettings.json`:

```json
{
  "Loops": {
    "ApiKey": "your-loops-api-key",
    "BaseUrl": "https://app.loops.so/api/v1",
    "Enabled": true,
    "DefaultSource": "CRM API",
    "TimeoutSeconds": 30
  }
}
```

### Behavior

- **Contact Creation**: When a new contact is created via `POST /api/contacts`, it is automatically synchronized to Loops.so in the background
- **Non-blocking**: Loops.so synchronization runs asynchronously and does not affect the API response time
- **Error Handling**: If Loops.so synchronization fails, it is logged but does not affect the contact creation in the main database
- **Duplicate Handling**: If a contact already exists in Loops.so, the duplicate is handled gracefully

### Configuration Options

| Option | Description | Default |
|--------|-------------|---------|
| `ApiKey` | Your Loops.so API key | Empty (required) |
| `BaseUrl` | Loops.so API base URL | `https://app.loops.so/api/v1` |
| `Enabled` | Enable/disable Loops.so integration | `false` |
| `DefaultSource` | Default source tag for contacts | `"CRM API"` |
| `TimeoutSeconds` | Request timeout in seconds | `30` |

---

## Future Enhancements

### Planned Features
- **Authentication**: JWT-based authentication
- **Authorization**: Role-based access control
- **Bulk Operations**: Batch create/update/delete
- **Advanced Search**: Full-text search with filters
- **Export**: CSV and PDF export functionality
- **Webhooks**: Real-time notifications for changes
- **GraphQL**: Alternative query interface
- **Rate Limiting**: Advanced rate limiting with tiers

### Performance Improvements
- **Caching**: Redis-based response caching
- **Pagination**: Cursor-based pagination for large datasets
- **Compression**: Response compression for large payloads
- **CDN**: Static content delivery optimization

---

This API documentation provides comprehensive information for integrating with the CRM API backend service. All endpoints are designed to be RESTful, consistent, and well-documented for easy consumption by frontend applications.
