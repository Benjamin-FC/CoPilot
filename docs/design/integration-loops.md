# Loops.so Integration Design

## Document Information
- **Version**: 1.0
- **Last Updated**: November 16, 2025
- **Integration Type**: HTTP REST API

## Overview

The CRM API integrates with Loops.so, a third-party email marketing and contact management platform, to automatically synchronize client contact information when new clients are created.

## Integration Architecture

### High-Level Flow
```
┌─────────────┐         ┌──────────────┐         ┌──────────────┐
│   Client    │         │   CRM API    │         │  Loops.so    │
│ Application │         │              │         │     API      │
└──────┬──────┘         └──────┬───────┘         └──────┬───────┘
       │                       │                        │
       │  POST /api/clients    │                        │
       ├──────────────────────>│                        │
       │                       │                        │
       │                       │  1. Validate Input     │
       │                       │  2. Save to Database   │
       │                       │  3. Return 201 Created │
       │                       │                        │
       │<──────────────────────┤                        │
       │    201 Created        │                        │
       │                       │                        │
       │                       │  4. Fire-and-Forget:   │
       │                       │     Task.Run(async)    │
       │                       │        ↓               │
       │                       │  5. POST /contacts/    │
       │                       │     create             │
       │                       ├───────────────────────>│
       │                       │                        │
       │                       │                        │ 6. Validate
       │                       │                        │    & Store
       │                       │                        │
       │                       │  7. Response (200 OK)  │
       │                       │<───────────────────────┤
       │                       │                        │
       │                       │  8. Log Result         │
       │                       │                        │
```

### Design Principles

#### 1. Non-Blocking (Fire-and-Forget)
- Client creation API returns immediately (201 Created)
- Loops.so sync happens asynchronously in background
- No impact on API response time
- User experience not affected by external API latency

**Implementation**:
```csharp
_ = Task.Run(async () =>
{
    try
    {
        await _loopsService.CreateContactAsync(...);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to sync to Loops.so");
    }
});
```

#### 2. Graceful Degradation
- CRM API continues to function even if Loops.so is unavailable
- Failed syncs are logged but don't fail the client creation
- Feature flag allows disabling integration entirely

#### 3. Configuration-Driven
- API key stored in configuration (not hardcoded)
- Feature flag to enable/disable integration
- Configurable timeout and base URL
- Environment-specific settings (dev vs production)

## Configuration

### Configuration Structure
```json
{
  "Loops": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "BaseUrl": "https://app.loops.so/api/v1",
    "Enabled": true,
    "DefaultSource": "CRM API",
    "TimeoutSeconds": 30
  }
}
```

### Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| ApiKey | string | "" | Loops.so API authentication key |
| BaseUrl | string | https://app.loops.so/api/v1 | Loops.so API base URL |
| Enabled | boolean | false | Master switch for integration |
| DefaultSource | string | "CRM API" | Source identifier in Loops.so |
| TimeoutSeconds | integer | 30 | HTTP request timeout |

### Environment-Specific Configuration

#### Development (appsettings.Development.json)
```json
{
  "Loops": {
    "ApiKey": "loops_dev_test_key",
    "Enabled": true
  }
}
```

#### Production (appsettings.Production.json)
```json
{
  "Loops": {
    "ApiKey": "",  // Set via environment variable
    "Enabled": true
  }
}
```

### Secrets Management

**Development**:
- User secrets: `dotnet user-secrets set "Loops:ApiKey" "your_key_here"`
- Environment variables: `LOOPS__APIKEY=your_key_here`

**Production**:
- Azure Key Vault
- AWS Secrets Manager
- Kubernetes Secrets
- Environment variables in container/app service

## API Contract

### Endpoint
```
POST https://app.loops.so/api/v1/contacts/create
```

### Authentication
```
Authorization: Bearer YOUR_API_KEY
```

### Request Headers
```
Content-Type: application/json
```

### Request Body
```json
{
  "email": "john.smith@example.com",
  "firstName": "John",
  "lastName": "Smith",
  "source": "CRM API",
  "subscribed": true,
  "userGroup": null,
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Request Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| email | string | Yes | Contact email address (unique) |
| firstName | string | No | Contact first name |
| lastName | string | No | Contact last name |
| source | string | No | Source of contact (for tracking) |
| subscribed | boolean | No | Email subscription status (default: true) |
| userGroup | string | No | Contact group/segment |
| userId | string | No | External user ID (CRM client ID) |

### Response (Success)
```json
{
  "success": true,
  "id": "clxyz123abc456",
  "message": "Contact created successfully"
}
```

### Response (Error)
```json
{
  "success": false,
  "message": "Contact with this email already exists"
}
```

### Status Codes
- `200 OK`: Contact created or already exists
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Invalid API key
- `429 Too Many Requests`: Rate limit exceeded
- `500 Internal Server Error`: Loops.so server error

## Implementation Details

### Service Interface
```csharp
public interface ILoopsService
{
    Task<bool> CreateContactAsync(
        string email,
        string? firstName = null,
        string? lastName = null,
        string? userId = null,
        CancellationToken cancellationToken = default);
}
```

### Service Implementation

**File**: `src/Api/Services/LoopsService.cs`

**Key Features**:
1. **Feature Flag Check**: Return early if `Enabled = false`
2. **API Key Validation**: Log warning if API key missing
3. **HTTP Client Factory**: Efficient connection management
4. **Bearer Authentication**: API key in Authorization header
5. **JSON Serialization**: Request/response JSON handling
6. **Comprehensive Error Handling**: HttpRequestException, TaskCanceledException, general Exception
7. **Structured Logging**: Context-rich log messages

### Error Handling Strategy

#### 1. HTTP Errors (4xx, 5xx)
```csharp
if (!response.IsSuccessStatusCode)
{
    _logger.LogWarning(
        "Failed to create contact. Status: {StatusCode}, Response: {Response}",
        response.StatusCode,
        errorContent);
    return false;
}
```

#### 2. Network Errors
```csharp
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "HTTP error while creating contact");
    return false;
}
```

#### 3. Timeout Errors
```csharp
catch (TaskCanceledException ex)
{
    _logger.LogError(ex, "Request timeout while creating contact");
    return false;
}
```

#### 4. Unexpected Errors
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error while creating contact");
    return false;
}
```

### Dependency Injection Setup

**File**: `src/Api/Program.cs`

```csharp
// Configure options
builder.Services.Configure<LoopsOptions>(
    builder.Configuration.GetSection(LoopsOptions.SectionName));

// Register HTTP client with factory
builder.Services.AddHttpClient<ILoopsService, LoopsService>(
    (serviceProvider, client) =>
{
    var options = serviceProvider
        .GetRequiredService<IOptions<LoopsOptions>>()
        .Value;
    
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});
```

**Benefits**:
- HTTP client reuse (connection pooling)
- Automatic disposal
- Testability (mockable HttpClient)
- Centralized configuration

## Integration Lifecycle

### When Contact is Created
**Trigger**: New client created via `POST /api/clients`

**Process**:
1. Validate client input
2. Save client to database
3. Return 201 Created to caller
4. Background task: Call LoopsService.CreateContactAsync()
5. Log success or failure

### When Contact is Updated
**Current**: No integration

**Future Enhancement**: Sync updates to Loops.so
- Update contact via `PUT /contacts/update`
- Only sync when email, firstName, or lastName changes

### When Contact is Deleted
**Current**: No integration

**Future Enhancement**: Remove from Loops.so
- Delete contact via `POST /contacts/delete`
- Or mark as unsubscribed

## Monitoring & Observability

### Logging

#### Success
```
[2025-11-16 15:45:30 INF] Creating contact in Loops.so for john.smith@example.com
[2025-11-16 15:45:31 INF] Successfully created contact in Loops.so for john.smith@example.com. Response: Contact created successfully
```

#### Disabled
```
[2025-11-16 15:45:30 DBG] Loops.so integration is disabled. Skipping contact creation for john.smith@example.com
```

#### Missing API Key
```
[2025-11-16 15:45:30 WRN] Loops.so API key is not configured. Cannot create contact for john.smith@example.com
```

#### HTTP Error
```
[2025-11-16 15:45:31 WRN] Failed to create contact in Loops.so for john.smith@example.com. Status: 400, Response: Invalid email format
```

#### Network Error
```
[2025-11-16 15:45:31 ERR] HTTP error while creating contact in Loops.so for john.smith@example.com
System.Net.Http.HttpRequestException: Connection refused
```

### Metrics (Recommended)

Track the following metrics:
- Total contacts synced (count)
- Sync success rate (percentage)
- Sync failure count by error type
- Average sync duration (milliseconds)
- Loops.so API availability (uptime percentage)

### Alerting (Recommended)

Alert on:
- Sync failure rate > 10% over 1 hour
- Consecutive failures > 5
- Loops.so API unavailable for > 5 minutes

## Rate Limiting

### Loops.so Limits
- Check Loops.so documentation for current rate limits
- Typical: 100 requests per minute

### Current Implementation
- No rate limiting in CRM API
- Each client creation = 1 Loops.so request

### Future Enhancement
- Queue-based processing (Azure Queue, RabbitMQ)
- Batch create contacts (if Loops.so supports)
- Retry logic with exponential backoff

## Testing

### Unit Tests

**File**: `tests/Api.Tests/LoopsServiceTests.cs`

**Test Cases**:
1. ✅ `CreateContactAsync_WithValidData_ReturnsTrue`
2. ✅ `CreateContactAsync_WhenDisabled_ReturnsFalse`
3. ✅ `CreateContactAsync_WithoutApiKey_ReturnsFalse`
4. ✅ `CreateContactAsync_WithHttpError_ReturnsFalse`
5. ✅ `CreateContactAsync_WithHttpException_ReturnsFalse`
6. ✅ `CreateContactAsync_SendsCorrectRequestData`

**Mocking**:
- `HttpMessageHandler` mocked using `Moq`
- Response status codes and content controlled
- Request content captured and validated

### Integration Tests

**File**: `tests/Api.Tests/UnitTest1.cs`

**Test Case**: `CreateClient_CallsLoopsService` ✅

**Process**:
1. Mock ILoopsService
2. Create client via POST
3. Verify LoopsService.CreateContactAsync called with correct parameters

### Manual Testing

#### Prerequisites
1. Valid Loops.so API key
2. Set `Loops:Enabled = true` in appsettings.Development.json
3. Set `Loops:ApiKey` in user secrets or environment

#### Test Steps
1. Create client via POST /api/clients
2. Verify 201 Created response
3. Check application logs for Loops.so sync messages
4. Login to Loops.so dashboard
5. Verify contact exists with correct details

## Security Considerations

### API Key Protection
- ❌ Never commit API keys to source control
- ✅ Use environment variables or secrets management
- ✅ Add `appsettings.*.json` to .gitignore (except base)

### HTTPS Only
- ✅ Always use HTTPS for Loops.so API calls
- ✅ TLS 1.2+ enforced

### Data Privacy
- Client email sent to third-party (Loops.so)
- Ensure Loops.so GDPR compliance
- Document data sharing in privacy policy

### Audit Trail
- Log all contact sync attempts
- Include client ID, email, timestamp, success/failure

## Troubleshooting

### Issue: Contacts Not Syncing

**Symptoms**: No logs, no contacts in Loops.so

**Diagnosis**:
1. Check `Loops:Enabled` setting
2. Check `Loops:ApiKey` is set
3. Check logs for errors

**Solution**:
- Set `Enabled = true` in appsettings
- Provide valid API key via secrets or environment

---

### Issue: 401 Unauthorized

**Symptoms**: HTTP 401 errors in logs

**Diagnosis**: Invalid or expired API key

**Solution**:
1. Verify API key in Loops.so dashboard
2. Generate new API key if needed
3. Update configuration/secrets

---

### Issue: Timeout Errors

**Symptoms**: TaskCanceledException in logs

**Diagnosis**: Loops.so API slow or network issues

**Solution**:
1. Increase `TimeoutSeconds` setting (default: 30)
2. Check network connectivity to Loops.so
3. Check Loops.so status page

---

### Issue: Duplicate Contacts

**Symptoms**: Loops.so returns "Contact already exists"

**Diagnosis**: Email already in Loops.so

**Solution**:
- This is expected behavior (idempotent)
- Contact update endpoint (future enhancement)

## Future Enhancements

### 1. Sync Updates
- Update contact when client email/name changes
- Call Loops.so update endpoint

### 2. Sync Deletions
- Remove contact when client deleted
- Or unsubscribe in Loops.so

### 3. Batch Processing
- Queue contact syncs (Azure Queue, RabbitMQ)
- Process in batches to respect rate limits
- Retry failed syncs

### 4. Bidirectional Sync
- Webhook from Loops.so to CRM API
- Update client when contact unsubscribes

### 5. Custom Fields
- Sync additional client data (company, phone)
- Map to Loops.so custom fields

### 6. Webhooks
- Notify external systems of sync events
- Publish events to message bus

### 7. Sync Status Tracking
- Add `LoopsSyncStatus` field to Client entity
- Track last sync timestamp
- Enable retry of failed syncs

## Performance Impact

### API Response Time
- **Without Loops.so**: ~50-100ms
- **With Loops.so (fire-and-forget)**: ~50-100ms (no impact)
- **If blocking**: +200-500ms (not recommended)

### Background Task Overhead
- Minimal CPU/memory impact
- One Task.Run per client creation
- Task cleanup handled by runtime

## References

- [Loops.so API Documentation](https://app.loops.so/api/v1)
- [ASP.NET Core HttpClient Factory](https://learn.microsoft.com/aspnet/core/fundamentals/http-requests)
- [Fire-and-Forget Pattern](https://stackoverflow.com/questions/12803012/fire-and-forget-with-async-vs-old-async-delegate)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration)
