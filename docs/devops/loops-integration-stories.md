# Loops.so Integration - User Stories, Tasks, and Acceptance Criteria

## Document Information
- **Version**: 1.0
- **Created**: November 16, 2025
- **Status**: Ready for Implementation
- **Epic**: External Contact Synchronization
- **Priority**: High
- **Repository**: https://github.com/Benjamin-FC/CoPilot

---

## Table of Contents
1. [Epic Overview](#epic-overview)
2. [User Stories](#user-stories)
3. [Implementation Tasks](#implementation-tasks)
4. [Testing Tasks](#testing-tasks)
5. [Documentation Tasks](#documentation-tasks)
6. [DevOps Tasks](#devops-tasks)
7. [Definition of Done](#definition-of-done)

---

## Epic Overview

**Epic**: Loops.so Integration for Automated Contact Synchronization

**Business Value**: 
- Automatically synchronize CRM contacts with Loops.so email marketing platform
- Reduce manual data entry and human error
- Enable immediate email marketing campaigns for new clients
- Maintain centralized contact database across systems

**Success Criteria**:
- Contacts automatically sync to Loops.so when created in CRM
- CRM API performance not impacted by external integration
- Failed syncs are logged and don't break client creation
- Integration can be enabled/disabled via configuration
- 95%+ sync success rate in production

**Estimated Effort**: 34 Story Points (5-7 working days)

---

## User Stories

### Story 1: Configure Loops.so Integration Settings

**Story ID**: LOOP-001  
**Priority**: P0 (Blocker)  
**Estimate**: 3 Story Points  
**Sprint**: Sprint 1  
**Assignee**: Backend Developer + DevOps Engineer

**As a** DevOps Engineer  
**I want to** configure Loops.so API credentials and settings  
**So that** the CRM API can authenticate and communicate with Loops.so

#### Acceptance Criteria

**AC1.1**: Configuration structure exists in appsettings.json
```gherkin
Given I open the appsettings.json file
When I navigate to the root configuration object
Then I should see a "Loops" section with the following properties:
  - ApiKey (string, empty by default)
  - BaseUrl (string, default: "https://app.loops.so/api/v1")
  - Enabled (boolean, default: false)
  - DefaultSource (string, default: "CRM API")
  - TimeoutSeconds (integer, default: 30)
```

**AC1.2**: Configuration options class is created
```gherkin
Given I navigate to the configuration folder
When I review the LoopsOptions.cs file
Then it should have:
  - Public properties matching appsettings schema
  - SectionName constant set to "Loops"
  - All properties with appropriate data types
  - XML documentation comments
  - Data annotations for validation
```

**AC1.3**: Configuration is registered in dependency injection
```gherkin
Given I open the Program.cs file
When I review the service registration code
Then I should see:
  - builder.Services.Configure<LoopsOptions>() called
  - Configuration section "Loops" bound to LoopsOptions
  - Configuration loaded before LoopsService registration
```

**AC1.4**: Environment-specific configuration is supported
```gherkin
Given I have appsettings.Development.json file
When I set Loops:Enabled to true
And I run the application in Development environment
Then the configuration should override base settings
And I can set different API keys per environment
```

**AC1.5**: Secrets management is documented
```gherkin
Given I read the configuration documentation
When I review secrets management section
Then I should find instructions for:
  - Using dotnet user-secrets in development
  - Using environment variables (LOOPS__APIKEY format)
  - Using Azure Key Vault in production
  - Never committing API keys to source control
```

#### Technical Notes
- **Files to Create**:
  - `src/Api/Configuration/LoopsOptions.cs`
  - `src/Api/appsettings.Development.json` (if not exists)
- **Files to Modify**:
  - `src/Api/appsettings.json`
  - `src/Api/Program.cs`

#### Dependencies
- None

#### Testing
- Manual verification of configuration loading
- Unit test for LoopsOptions binding

---

### Story 2: Implement Loops.so Service Interface and Implementation

**Story ID**: LOOP-002  
**Priority**: P0 (Blocker)  
**Estimate**: 8 Story Points  
**Sprint**: Sprint 1  
**Assignee**: Senior Backend Developer

**As a** Backend Developer  
**I want to** create a service that communicates with Loops.so API  
**So that** I can create contacts programmatically

#### Acceptance Criteria

**AC2.1**: Service interface is defined
```gherkin
Given I navigate to the Services folder
When I open ILoopsService.cs
Then I should see:
  - CreateContactAsync method signature
  - Parameters: email (required), firstName, lastName, userId (optional)
  - Returns: Task<bool> indicating success/failure
  - CancellationToken parameter
  - XML documentation comments
```

**AC2.2**: Service implementation exists
```gherkin
Given I open LoopsService.cs
When I review the implementation
Then it should have:
  - Constructor injection of IHttpClientFactory, IOptions<LoopsOptions>, ILogger
  - Private readonly fields for dependencies
  - CreateContactAsync method implementation
```

**AC2.3**: Feature flag check is implemented
```gherkin
Given Loops:Enabled is set to false
When CreateContactAsync is called
Then it should:
  - Log at Debug level: "Loops.so integration is disabled"
  - Return false immediately
  - Not make any HTTP requests
```

**AC2.4**: API key validation is implemented
```gherkin
Given Loops:ApiKey is empty or null
When CreateContactAsync is called
Then it should:
  - Log at Warning level: "Loops.so API key is not configured"
  - Return false immediately
  - Not make any HTTP requests
```

**AC2.5**: HTTP request is correctly formatted
```gherkin
Given valid configuration and parameters
When CreateContactAsync is called
Then the HTTP request should:
  - Use POST method
  - Target endpoint: {BaseUrl}/contacts/create
  - Include header: Authorization: Bearer {ApiKey}
  - Include header: Content-Type: application/json
  - Include JSON body with email, firstName, lastName, source, subscribed, userId
  - Set subscribed to true by default
  - Set source to DefaultSource from config
```

**AC2.6**: Successful response is handled
```gherkin
Given Loops.so API returns 200 OK
When CreateContactAsync completes
Then it should:
  - Log at Information level: "Successfully created contact"
  - Include email and response message in log
  - Return true
```

**AC2.7**: HTTP error responses are handled
```gherkin
Given Loops.so API returns 4xx or 5xx status
When CreateContactAsync completes
Then it should:
  - Log at Warning level: "Failed to create contact"
  - Include status code and response body in log
  - Return false
  - Not throw exception
```

**AC2.8**: Network errors are handled
```gherkin
Given network connection fails
When CreateContactAsync is called
Then it should:
  - Catch HttpRequestException
  - Log at Error level: "HTTP error while creating contact"
  - Include exception details in log
  - Return false
  - Not throw exception
```

**AC2.9**: Timeout errors are handled
```gherkin
Given request exceeds TimeoutSeconds
When CreateContactAsync is called
Then it should:
  - Catch TaskCanceledException
  - Log at Error level: "Request timeout while creating contact"
  - Include exception details in log
  - Return false
  - Not throw exception
```

**AC2.10**: Unexpected errors are handled
```gherkin
Given an unexpected exception occurs
When CreateContactAsync is called
Then it should:
  - Catch Exception
  - Log at Error level: "Unexpected error while creating contact"
  - Include exception details in log
  - Return false
  - Not throw exception
```

#### Technical Notes
- **Files to Create**:
  - `src/Api/Services/ILoopsService.cs`
  - `src/Api/Services/LoopsService.cs`
- **Dependencies**:
  - System.Net.Http.Json
  - Microsoft.Extensions.Http
  - Microsoft.Extensions.Options
  - Microsoft.Extensions.Logging

#### Testing
- Unit tests for all error scenarios (AC2.3 - AC2.10)
- Integration test with mocked HttpMessageHandler
- Manual test with real Loops.so API (development only)

---

### Story 3: Register Loops.so Service with HTTP Client Factory

**Story ID**: LOOP-003  
**Priority**: P0 (Blocker)  
**Estimate**: 2 Story Points  
**Sprint**: Sprint 1  
**Assignee**: Backend Developer

**As a** Backend Developer  
**I want to** register LoopsService with HTTP Client Factory  
**So that** HTTP connections are efficiently managed and reused

#### Acceptance Criteria

**AC3.1**: HTTP Client Factory is registered
```gherkin
Given I open Program.cs
When I review service registration
Then I should see:
  - AddHttpClient<ILoopsService, LoopsService>() called
  - HTTP client configured with BaseAddress from LoopsOptions
  - HTTP client timeout set to TimeoutSeconds from LoopsOptions
```

**AC3.2**: Service can be injected via constructor
```gherkin
Given LoopsService is registered
When I inject ILoopsService in a controller
Then it should:
  - Resolve successfully from DI container
  - Have HttpClient configured correctly
  - Not throw exceptions
```

**AC3.3**: HTTP client reuses connections
```gherkin
Given multiple requests are made to Loops.so
When I monitor HTTP connections
Then connections should be:
  - Reused across requests (connection pooling)
  - Not creating new sockets for each request
  - Respecting timeout configuration
```

#### Technical Notes
- **Files to Modify**:
  - `src/Api/Program.cs`
- **NuGet Packages**:
  - Microsoft.Extensions.Http (already included)

#### Testing
- Unit test for DI registration
- Integration test for connection pooling

---

### Story 4: Integrate Loops.so Sync in Client Creation Endpoint

**Story ID**: LOOP-004  
**Priority**: P1 (High)  
**Estimate**: 5 Story Points  
**Sprint**: Sprint 1  
**Assignee**: Backend Developer

**As a** API User  
**I want** newly created clients to automatically sync to Loops.so  
**So that** I can immediately start email campaigns without manual data entry

#### Acceptance Criteria

**AC4.1**: LoopsService is injected in ClientsController
```gherkin
Given I open ClientsController.cs
When I review the constructor
Then it should:
  - Accept ILoopsService parameter
  - Store in private readonly field
  - Be properly dependency injected
```

**AC4.2**: Sync happens after successful client creation
```gherkin
Given a new client is created successfully
When the client is saved to database
Then the system should:
  - Return 201 Created to the caller immediately
  - Trigger fire-and-forget sync to Loops.so
  - Not wait for Loops.so response
```

**AC4.3**: Fire-and-forget pattern is implemented correctly
```gherkin
Given client creation succeeds
When Loops.so sync is triggered
Then it should:
  - Use Task.Run(() => ...) for background execution
  - Discard task result with _ = Task.Run(...)
  - Include try-catch block inside Task.Run
  - Log any exceptions without rethrowing
  - Not block the HTTP response
```

**AC4.4**: Correct data is passed to LoopsService
```gherkin
Given client data: John Doe (john.doe@example.com)
When CreateContactAsync is called
Then parameters should be:
  - email: "john.doe@example.com"
  - firstName: "John"
  - lastName: "Doe"
  - userId: <generated client GUID>
```

**AC4.5**: Failed sync doesn't affect client creation
```gherkin
Given Loops.so API is unavailable
When a client is created
Then:
  - Client should be saved to database successfully
  - HTTP response should be 201 Created
  - Error should be logged
  - Client creation should not fail
```

**AC4.6**: Sync is skipped when integration is disabled
```gherkin
Given Loops:Enabled is false
When a client is created
Then:
  - Client should be saved successfully
  - No HTTP request to Loops.so should be made
  - Debug log message should indicate integration is disabled
```

**AC4.7**: Response time is not affected by sync
```gherkin
Given Loops.so has 500ms response time
When I create a client
Then:
  - API response time should be < 150ms
  - Background task handles Loops.so latency
  - No performance degradation for user
```

#### Technical Notes
- **Files to Modify**:
  - `src/Api/Controllers/ClientsController.cs`
- **Implementation Pattern**:
```csharp
// After saving client
_ = Task.Run(async () =>
{
    try
    {
        await _loopsService.CreateContactAsync(
            client.Email,
            client.FirstName,
            client.LastName,
            client.Id.ToString());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to sync client {ClientId} to Loops.so", client.Id);
    }
});
```

#### Dependencies
- LOOP-002 (LoopsService implementation)
- LOOP-003 (Service registration)

#### Testing
- Integration test with mocked ILoopsService
- Performance test to verify non-blocking behavior
- Manual test with real Loops.so API

---

### Story 5: Implement Comprehensive Logging

**Story ID**: LOOP-005  
**Priority**: P1 (High)  
**Estimate**: 3 Story Points  
**Sprint**: Sprint 1  
**Assignee**: Backend Developer

**As a** DevOps Engineer  
**I want** comprehensive logging for all Loops.so operations  
**So that** I can monitor, troubleshoot, and debug integration issues

#### Acceptance Criteria

**AC5.1**: Log when integration is disabled
```gherkin
Given Loops:Enabled is false
When CreateContactAsync is called
Then log at Debug level:
  - Message: "Loops.so integration is disabled. Skipping contact creation for {Email}"
  - Include email address
```

**AC5.2**: Log when API key is missing
```gherkin
Given Loops:ApiKey is empty
When CreateContactAsync is called
Then log at Warning level:
  - Message: "Loops.so API key is not configured. Cannot create contact for {Email}"
  - Include email address
```

**AC5.3**: Log before making API request
```gherkin
Given all prerequisites are met
When CreateContactAsync starts execution
Then log at Information level:
  - Message: "Creating contact in Loops.so for {Email}"
  - Include email address
```

**AC5.4**: Log successful creation
```gherkin
Given Loops.so returns 200 OK
When CreateContactAsync completes
Then log at Information level:
  - Message: "Successfully created contact in Loops.so for {Email}. Response: {ResponseMessage}"
  - Include email and response message
```

**AC5.5**: Log HTTP errors with details
```gherkin
Given Loops.so returns 400 Bad Request
When CreateContactAsync completes
Then log at Warning level:
  - Message: "Failed to create contact in Loops.so for {Email}. Status: {StatusCode}, Response: {ResponseBody}"
  - Include email, status code, and response body
```

**AC5.6**: Log network errors
```gherkin
Given network connection fails
When HttpRequestException is caught
Then log at Error level:
  - Message: "HTTP error while creating contact in Loops.so for {Email}"
  - Include email and exception details
```

**AC5.7**: Log timeout errors
```gherkin
Given request times out
When TaskCanceledException is caught
Then log at Error level:
  - Message: "Request timeout while creating contact in Loops.so for {Email}"
  - Include email and exception details
```

**AC5.8**: Log unexpected errors
```gherkin
Given an unexpected exception occurs
When Exception is caught
Then log at Error level:
  - Message: "Unexpected error while creating contact in Loops.so for {Email}"
  - Include email and full exception details
```

**AC5.9**: Log from ClientsController on sync failure
```gherkin
Given sync task throws exception
When exception is caught in fire-and-forget handler
Then log at Error level:
  - Message: "Failed to sync client {ClientId} to Loops.so"
  - Include client ID and exception details
```

**AC5.10**: All logs use structured logging
```gherkin
Given any log message is written
Then it should:
  - Use structured logging parameters (not string interpolation)
  - Include relevant context (email, clientId, statusCode, etc.)
  - Follow consistent message format
  - Be parseable by log aggregation tools
```

#### Technical Notes
- Use `ILogger<T>` for all logging
- Use structured logging: `_logger.LogInformation("Message {Param}", param)`
- Avoid string interpolation in log messages
- Include exception objects in error logs

#### Testing
- Unit tests verify log messages are called with correct parameters
- Manual verification of log output in console/file

---

### Story 6: Create Unit Tests for LoopsService

**Story ID**: LOOP-006  
**Priority**: P1 (High)  
**Estimate**: 5 Story Points  
**Sprint**: Sprint 2  
**Assignee**: Backend Developer / QA Engineer

**As a** Developer  
**I want** comprehensive unit tests for LoopsService  
**So that** I can ensure reliability and catch regressions early

#### Acceptance Criteria

**AC6.1**: Test fixture is set up correctly
```gherkin
Given I create LoopsServiceTests class
Then it should:
  - Use xUnit framework
  - Mock HttpMessageHandler using Moq
  - Mock ILogger<LoopsService>
  - Create test LoopsOptions configurations
  - Have setup and teardown methods if needed
```

**AC6.2**: Test: CreateContactAsync with valid data returns true
```gherkin
Given valid email "test@example.com" and names
And Loops.so returns 200 OK with success response
When CreateContactAsync is called
Then:
  - Result should be true
  - HTTP POST should be made to /contacts/create
  - Request should include correct Authorization header
  - Request body should match expected JSON structure
```

**AC6.3**: Test: CreateContactAsync when disabled returns false
```gherkin
Given Loops:Enabled is false
When CreateContactAsync is called
Then:
  - Result should be false
  - No HTTP request should be made
  - Debug log should be written
```

**AC6.4**: Test: CreateContactAsync without API key returns false
```gherkin
Given Loops:ApiKey is null or empty
When CreateContactAsync is called
Then:
  - Result should be false
  - No HTTP request should be made
  - Warning log should be written
```

**AC6.5**: Test: CreateContactAsync with HTTP 400 returns false
```gherkin
Given Loops.so returns 400 Bad Request
When CreateContactAsync is called
Then:
  - Result should be false
  - Warning log should be written with status code
  - No exception should be thrown
```

**AC6.6**: Test: CreateContactAsync with HTTP 401 returns false
```gherkin
Given Loops.so returns 401 Unauthorized
When CreateContactAsync is called
Then:
  - Result should be false
  - Warning log should be written
  - No exception should be thrown
```

**AC6.7**: Test: CreateContactAsync with HTTP 500 returns false
```gherkin
Given Loops.so returns 500 Internal Server Error
When CreateContactAsync is called
Then:
  - Result should be false
  - Warning log should be written
  - No exception should be thrown
```

**AC6.8**: Test: CreateContactAsync with network exception returns false
```gherkin
Given HttpClient throws HttpRequestException
When CreateContactAsync is called
Then:
  - Result should be false
  - Error log should be written
  - Exception should be caught and not rethrown
```

**AC6.9**: Test: CreateContactAsync with timeout returns false
```gherkin
Given request times out (TaskCanceledException)
When CreateContactAsync is called
Then:
  - Result should be false
  - Error log should be written
  - Exception should be caught and not rethrown
```

**AC6.10**: Test: CreateContactAsync sends correct request data
```gherkin
Given email "john@example.com", firstName "John", lastName "Doe", userId "test-guid"
When CreateContactAsync is called
Then HTTP request body should contain:
  - "email": "john@example.com"
  - "firstName": "John"
  - "lastName": "Doe"
  - "userId": "test-guid"
  - "source": "CRM API" (from config)
  - "subscribed": true
```

**AC6.11**: Test: All tests pass with 100% code coverage
```gherkin
Given all unit tests are implemented
When I run code coverage analysis
Then:
  - LoopsService should have >95% code coverage
  - All branches should be covered
  - All error paths should be tested
```

#### Technical Notes
- **Files to Create**:
  - `tests/Api.Tests/Services/LoopsServiceTests.cs`
- **Mocking Strategy**:
  - Mock `HttpMessageHandler` to control HTTP responses
  - Mock `ILogger<LoopsService>` to verify logging
  - Use `IOptions<LoopsOptions>` with test configurations

#### Dependencies
- LOOP-002 (LoopsService implementation)

---

### Story 7: Create Integration Tests for Client Creation with Loops Sync

**Story ID**: LOOP-007  
**Priority**: P2 (Medium)  
**Estimate**: 4 Story Points  
**Sprint**: Sprint 2  
**Assignee**: QA Engineer / Backend Developer

**As a** QA Engineer  
**I want** integration tests that verify Loops.so sync during client creation  
**So that** I can ensure end-to-end functionality works correctly

#### Acceptance Criteria

**AC7.1**: Test: Create client calls LoopsService with correct parameters
```gherkin
Given I mock ILoopsService
And I POST a new client with email "test@example.com"
When the request completes
Then:
  - HTTP response should be 201 Created
  - ILoopsService.CreateContactAsync should be called once
  - Parameters should match client data (email, firstName, lastName, id)
```

**AC7.2**: Test: Create client succeeds even when LoopsService fails
```gherkin
Given ILoopsService.CreateContactAsync returns false
When I POST a new client
Then:
  - HTTP response should be 201 Created
  - Client should exist in database
  - No exception should be thrown
```

**AC7.3**: Test: Create client succeeds when LoopsService throws exception
```gherkin
Given ILoopsService.CreateContactAsync throws exception
When I POST a new client
Then:
  - HTTP response should be 201 Created
  - Client should exist in database
  - Exception should be logged but not propagated
```

**AC7.4**: Test: Loops sync is skipped when integration is disabled
```gherkin
Given Loops:Enabled is false in test configuration
When I POST a new client
Then:
  - HTTP response should be 201 Created
  - ILoopsService.CreateContactAsync should not be called
```

**AC7.5**: Test: Multiple clients can be created concurrently
```gherkin
Given I create 10 clients concurrently
When all requests complete
Then:
  - All should return 201 Created
  - All clients should be in database
  - ILoopsService should be called 10 times
  - No race conditions or deadlocks
```

#### Technical Notes
- **Files to Create/Modify**:
  - `tests/Api.Tests/Integration/ClientsControllerIntegrationTests.cs`
- Use WebApplicationFactory for integration tests
- Mock ILoopsService in test DI container
- Use in-memory database for each test

#### Dependencies
- LOOP-004 (Integration in controller)

---

## Implementation Tasks

### Task 1: Create Configuration Infrastructure

**Task ID**: TASK-001  
**Story**: LOOP-001  
**Estimate**: 2 hours  
**Assignee**: Backend Developer

**Steps**:
1. Create `src/Api/Configuration/` folder (if not exists)
2. Create `LoopsOptions.cs` with properties:
   - ApiKey (string)
   - BaseUrl (string, default: "https://app.loops.so/api/v1")
   - Enabled (bool, default: false)
   - DefaultSource (string, default: "CRM API")
   - TimeoutSeconds (int, default: 30)
   - SectionName constant ("Loops")
3. Add XML documentation to all properties
4. Add data annotations for validation (Required, Url, Range)

**Verification**:
- Class compiles without errors
- Properties have correct types and defaults

---

### Task 2: Update Configuration Files

**Task ID**: TASK-002  
**Story**: LOOP-001  
**Estimate**: 1 hour  
**Assignee**: Backend Developer

**Steps**:
1. Open `src/Api/appsettings.json`
2. Add "Loops" section with all properties (empty ApiKey)
3. Create/update `src/Api/appsettings.Development.json`
4. Set Loops:Enabled to true in Development
5. Add placeholder for ApiKey in Development
6. Add `.user-secrets` to .gitignore (if not present)

**Verification**:
- Configuration files are valid JSON
- No API keys committed to repository
- Development settings override base settings

---

### Task 3: Register Configuration in DI

**Task ID**: TASK-003  
**Story**: LOOP-001  
**Estimate**: 30 minutes  
**Assignee**: Backend Developer

**Steps**:
1. Open `src/Api/Program.cs`
2. Add after builder creation:
```csharp
builder.Services.Configure<LoopsOptions>(
    builder.Configuration.GetSection(LoopsOptions.SectionName));
```
3. Place before service registrations

**Verification**:
- Application starts without errors
- Configuration can be injected via IOptions<LoopsOptions>

---

### Task 4: Create ILoopsService Interface

**Task ID**: TASK-004  
**Story**: LOOP-002  
**Estimate**: 30 minutes  
**Assignee**: Backend Developer

**Steps**:
1. Create `src/Api/Services/` folder (if not exists)
2. Create `ILoopsService.cs`
3. Define interface with:
```csharp
public interface ILoopsService
{
    /// <summary>
    /// Creates a contact in Loops.so
    /// </summary>
    /// <param name="email">Contact email address (required)</param>
    /// <param name="firstName">Contact first name</param>
    /// <param name="lastName">Contact last name</param>
    /// <param name="userId">External user ID (CRM client ID)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> CreateContactAsync(
        string email,
        string? firstName = null,
        string? lastName = null,
        string? userId = null,
        CancellationToken cancellationToken = default);
}
```

**Verification**:
- Interface compiles
- XML documentation is complete

---

### Task 5: Implement LoopsService Core Structure

**Task ID**: TASK-005  
**Story**: LOOP-002  
**Estimate**: 2 hours  
**Assignee**: Senior Backend Developer

**Steps**:
1. Create `src/Api/Services/LoopsService.cs`
2. Implement class structure:
```csharp
public class LoopsService : ILoopsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<LoopsOptions> _options;
    private readonly ILogger<LoopsService> _logger;

    public LoopsService(
        IHttpClientFactory httpClientFactory,
        IOptions<LoopsOptions> options,
        ILogger<LoopsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public async Task<bool> CreateContactAsync(
        string email,
        string? firstName = null,
        string? lastName = null,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        // Implementation in next tasks
        throw new NotImplementedException();
    }
}
```

**Verification**:
- Class compiles
- Dependencies are injected correctly

---

### Task 6: Implement Feature Flag and API Key Checks

**Task ID**: TASK-006  
**Story**: LOOP-002  
**Estimate**: 1 hour  
**Assignee**: Backend Developer

**Steps**:
1. In CreateContactAsync, add at the beginning:
```csharp
var options = _options.Value;

// Check if integration is enabled
if (!options.Enabled)
{
    _logger.LogDebug(
        "Loops.so integration is disabled. Skipping contact creation for {Email}",
        email);
    return false;
}

// Check if API key is configured
if (string.IsNullOrWhiteSpace(options.ApiKey))
{
    _logger.LogWarning(
        "Loops.so API key is not configured. Cannot create contact for {Email}",
        email);
    return false;
}
```

**Verification**:
- Returns false when disabled
- Returns false when API key missing
- Logs appropriate messages

---

### Task 7: Implement HTTP Request Building

**Task ID**: TASK-007  
**Story**: LOOP-002  
**Estimate**: 2 hours  
**Assignee**: Backend Developer

**Steps**:
1. Create request body object:
```csharp
var requestBody = new
{
    email = email,
    firstName = firstName,
    lastName = lastName,
    source = options.DefaultSource,
    subscribed = true,
    userId = userId
};
```

2. Create HttpClient and request:
```csharp
var httpClient = _httpClientFactory.CreateClient();
httpClient.BaseAddress = new Uri(options.BaseUrl);
httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", options.ApiKey);

_logger.LogInformation(
    "Creating contact in Loops.so for {Email}",
    email);

var response = await httpClient.PostAsJsonAsync(
    "/contacts/create",
    requestBody,
    cancellationToken);
```

**Verification**:
- Request format matches Loops.so API spec
- Headers are set correctly
- Timeout is configured

---

### Task 8: Implement Response Handling

**Task ID**: TASK-008  
**Story**: LOOP-002  
**Estimate**: 2 hours  
**Assignee**: Backend Developer

**Steps**:
1. Handle successful response:
```csharp
if (response.IsSuccessStatusCode)
{
    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
    _logger.LogInformation(
        "Successfully created contact in Loops.so for {Email}. Response: {Response}",
        email,
        responseContent);
    return true;
}
```

2. Handle error response:
```csharp
else
{
    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
    _logger.LogWarning(
        "Failed to create contact in Loops.so for {Email}. Status: {StatusCode}, Response: {Response}",
        email,
        response.StatusCode,
        errorContent);
    return false;
}
```

**Verification**:
- Success returns true with log
- Error returns false with detailed log

---

### Task 9: Implement Exception Handling

**Task ID**: TASK-009  
**Story**: LOOP-002  
**Estimate**: 1.5 hours  
**Assignee**: Backend Developer

**Steps**:
1. Wrap HTTP call in try-catch:
```csharp
try
{
    // HTTP request code here
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex,
        "HTTP error while creating contact in Loops.so for {Email}",
        email);
    return false;
}
catch (TaskCanceledException ex)
{
    _logger.LogError(ex,
        "Request timeout while creating contact in Loops.so for {Email}",
        email);
    return false;
}
catch (Exception ex)
{
    _logger.LogError(ex,
        "Unexpected error while creating contact in Loops.so for {Email}",
        email);
    return false;
}
```

**Verification**:
- All exception types caught
- No exceptions propagate
- All return false on error

---

### Task 10: Register LoopsService with HTTP Client Factory

**Task ID**: TASK-010  
**Story**: LOOP-003  
**Estimate**: 1 hour  
**Assignee**: Backend Developer

**Steps**:
1. Open `src/Api/Program.cs`
2. Add after configuration registration:
```csharp
builder.Services.AddHttpClient<ILoopsService, LoopsService>();
```

**Verification**:
- Application starts successfully
- ILoopsService can be injected
- HttpClient is configured correctly

---

### Task 11: Inject LoopsService in ClientsController

**Task ID**: TASK-011  
**Story**: LOOP-004  
**Estimate**: 30 minutes  
**Assignee**: Backend Developer

**Steps**:
1. Open `src/Api/Controllers/ClientsController.cs`
2. Add to constructor:
```csharp
private readonly ILoopsService _loopsService;

public ClientsController(
    AppDbContext context,
    IMapper mapper,
    ILoopsService loopsService)
{
    _context = context;
    _mapper = mapper;
    _loopsService = loopsService;
}
```

**Verification**:
- Controller initializes without errors
- Service is injected correctly

---

### Task 12: Implement Fire-and-Forget Sync in CreateClient

**Task ID**: TASK-012  
**Story**: LOOP-004  
**Estimate**: 2 hours  
**Assignee**: Backend Developer

**Steps**:
1. In CreateClient method, after SaveChangesAsync:
```csharp
// Fire-and-forget: Sync to Loops.so without blocking
_ = Task.Run(async () =>
{
    try
    {
        await _loopsService.CreateContactAsync(
            client.Email,
            client.FirstName,
            client.LastName,
            client.Id.ToString());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,
            "Failed to sync client {ClientId} to Loops.so",
            client.Id);
    }
});
```

2. Place before returning CreatedAtAction

**Verification**:
- Client creation returns immediately
- Sync happens in background
- No performance impact on response time

---

## Testing Tasks

### Task 13: Write LoopsService Unit Tests - Feature Flag

**Task ID**: TASK-013  
**Story**: LOOP-006  
**Estimate**: 1 hour  
**Assignee**: Backend Developer

**Steps**:
1. Create `tests/Api.Tests/Services/LoopsServiceTests.cs`
2. Set up test fixture with mocked dependencies
3. Write test: `CreateContactAsync_WhenDisabled_ReturnsFalse_AndLogsDebug`
4. Assert: returns false, no HTTP call, debug log written

**Verification**:
- Test passes
- Code coverage for feature flag path

---

### Task 14: Write LoopsService Unit Tests - API Key Validation

**Task ID**: TASK-014  
**Story**: LOOP-006  
**Estimate**: 1 hour  
**Assignee**: Backend Developer

**Steps**:
1. Write test: `CreateContactAsync_WithoutApiKey_ReturnsFalse_AndLogsWarning`
2. Configure options with empty ApiKey
3. Assert: returns false, no HTTP call, warning log written

**Verification**:
- Test passes
- Code coverage for API key validation

---

### Task 15: Write LoopsService Unit Tests - Successful Response

**Task ID**: TASK-015  
**Story**: LOOP-006  
**Estimate**: 2 hours  
**Assignee**: Backend Developer

**Steps**:
1. Write test: `CreateContactAsync_WithValidData_ReturnsTrue`
2. Mock HttpMessageHandler to return 200 OK
3. Assert: returns true, correct HTTP request, info log written
4. Verify request body structure

**Verification**:
- Test passes
- HTTP request validated
- Response handling verified

---

### Task 16: Write LoopsService Unit Tests - Error Responses

**Task ID**: TASK-016  
**Story**: LOOP-006  
**Estimate**: 2 hours  
**Assignee**: Backend Developer

**Steps**:
1. Write tests for HTTP errors:
   - `CreateContactAsync_With400_ReturnsFalse`
   - `CreateContactAsync_With401_ReturnsFalse`
   - `CreateContactAsync_With500_ReturnsFalse`
2. Mock different status codes
3. Assert: returns false, warning logs written

**Verification**:
- All tests pass
- Error handling verified

---

### Task 17: Write LoopsService Unit Tests - Exception Handling

**Task ID**: TASK-017  
**Story**: LOOP-006  
**Estimate**: 2 hours  
**Assignee**: Backend Developer

**Steps**:
1. Write tests for exceptions:
   - `CreateContactAsync_WithHttpException_ReturnsFalse`
   - `CreateContactAsync_WithTimeout_ReturnsFalse`
   - `CreateContactAsync_WithUnexpectedException_ReturnsFalse`
2. Mock handler to throw exceptions
3. Assert: returns false, error logs written, no exception propagated

**Verification**:
- All tests pass
- Exception handling verified
- >95% code coverage

---

### Task 18: Write Integration Test - Successful Sync

**Task ID**: TASK-018  
**Story**: LOOP-007  
**Estimate**: 2 hours  
**Assignee**: QA Engineer

**Steps**:
1. Create integration test class
2. Set up WebApplicationFactory with mocked ILoopsService
3. Write test: `CreateClient_CallsLoopsService_WithCorrectParameters`
4. POST new client, verify ILoopsService called

**Verification**:
- Test passes
- End-to-end flow verified

---

### Task 19: Write Integration Test - Resilience

**Task ID**: TASK-019  
**Story**: LOOP-007  
**Estimate**: 2 hours  
**Assignee**: QA Engineer

**Steps**:
1. Write test: `CreateClient_SucceedsWhenLoopsServiceFails`
2. Mock ILoopsService to return false
3. Assert: 201 response, client in database
4. Write test: `CreateClient_SucceedsWhenLoopsServiceThrows`
5. Mock to throw exception
6. Assert: 201 response, client in database

**Verification**:
- Both tests pass
- Resilience verified

---

### Task 20: Manual Testing with Real Loops.so API

**Task ID**: TASK-020  
**Story**: LOOP-004  
**Estimate**: 2 hours  
**Assignee**: QA Engineer

**Steps**:
1. Obtain Loops.so test API key
2. Set in user secrets: `dotnet user-secrets set "Loops:ApiKey" "your_key"`
3. Set Loops:Enabled to true
4. Create test clients via Swagger UI
5. Verify contacts appear in Loops.so dashboard
6. Monitor application logs
7. Test error scenarios (invalid email, etc.)

**Verification**:
- Contacts sync successfully
- Logs show expected messages
- No errors in happy path

---

## Documentation Tasks

### Task 21: Create User Documentation

**Task ID**: TASK-021  
**Estimate**: 2 hours  
**Assignee**: Technical Writer / Developer

**Steps**:
1. Create `docs/features/loops-integration-guide.md`
2. Document:
   - What the integration does
   - How to enable/disable
   - How to configure API key
   - Troubleshooting common issues
   - FAQ section

**Verification**:
- Documentation is clear and accurate
- Covers all user-facing aspects

---

### Task 22: Update API Documentation

**Task ID**: TASK-022  
**Estimate**: 1 hour  
**Assignee**: Developer

**Steps**:
1. Update Swagger/OpenAPI annotations
2. Add note to POST /api/clients endpoint about Loops.so sync
3. Document that sync is non-blocking
4. Add examples to XML documentation

**Verification**:
- Swagger UI shows updated info
- Examples are accurate

---

### Task 23: Create Developer Onboarding Guide

**Task ID**: TASK-023  
**Estimate**: 1.5 hours  
**Assignee**: Senior Developer

**Steps**:
1. Update `docs/development/setup.md`
2. Add section on Loops.so integration setup
3. Include user-secrets commands
4. Document how to test locally
5. List prerequisites (API key)

**Verification**:
- New developers can set up integration
- Instructions are clear

---

## DevOps Tasks

### Task 24: Configure Development Environment

**Task ID**: TASK-024  
**Estimate**: 1 hour  
**Assignee**: DevOps Engineer

**Steps**:
1. Create shared development API key (if applicable)
2. Document in team wiki/secrets manager
3. Update .gitignore to exclude secrets
4. Create sample appsettings.Development.json

**Verification**:
- No secrets in repository
- Team can access dev API key

---

### Task 25: Configure Staging Environment

**Task ID**: TASK-025  
**Estimate**: 2 hours  
**Assignee**: DevOps Engineer

**Steps**:
1. Obtain staging Loops.so API key
2. Store in environment secrets (Azure Key Vault, AWS Secrets Manager, etc.)
3. Set environment variables in staging app service
4. Configure Loops:Enabled to true
5. Deploy and verify

**Verification**:
- Staging environment syncs correctly
- Secrets are secure

---

### Task 26: Configure Production Environment

**Task ID**: TASK-026  
**Estimate**: 2 hours  
**Assignee**: DevOps Engineer

**Steps**:
1. Obtain production Loops.so API key
2. Store in production secrets manager
3. Set environment variables in production app service
4. Configure Loops:Enabled to true
5. Set appropriate timeout (may differ from staging)
6. Deploy with feature flag initially disabled
7. Enable after verification

**Verification**:
- Production environment ready
- Feature can be toggled without redeployment

---

### Task 27: Set Up Monitoring and Alerts

**Task ID**: TASK-027  
**Estimate**: 3 hours  
**Assignee**: DevOps Engineer

**Steps**:
1. Configure log aggregation (Application Insights, ELK, etc.)
2. Create dashboard for Loops.so metrics:
   - Total sync attempts
   - Success rate
   - Error rate by type
   - Average response time
3. Set up alerts:
   - Sync failure rate > 10% for 1 hour
   - Consecutive failures > 5
   - No successful syncs for 1 hour
4. Document alerting procedures

**Verification**:
- Dashboards show real-time data
- Alerts trigger correctly in test scenarios

---

### Task 28: Create Rollback Plan

**Task ID**: TASK-028  
**Estimate**: 1 hour  
**Assignee**: DevOps Engineer / Tech Lead

**Steps**:
1. Document rollback procedure:
   - Set Loops:Enabled to false via environment variable
   - Restart application (or wait for config reload)
   - Verify clients can still be created
2. Test rollback in staging
3. Share with team

**Verification**:
- Rollback procedure tested
- Can disable integration without code deployment

---

## Definition of Done

A story is considered **Done** when:

### Code Quality
- [ ] Code is written following C# coding standards
- [ ] All compiler warnings resolved
- [ ] No code smells or technical debt introduced
- [ ] SOLID principles followed
- [ ] Error handling is comprehensive

### Testing
- [ ] Unit tests written and passing (>95% coverage)
- [ ] Integration tests written and passing
- [ ] Manual testing completed successfully
- [ ] Performance testing shows no degradation
- [ ] Edge cases and error scenarios tested

### Documentation
- [ ] XML documentation comments on all public methods
- [ ] README/setup guide updated if needed
- [ ] API documentation (Swagger) updated
- [ ] Inline comments for complex logic

### Code Review
- [ ] Pull request created
- [ ] Code reviewed by at least one senior developer
- [ ] All review comments addressed
- [ ] Approved by reviewer(s)

### Integration
- [ ] Code merged to main branch
- [ ] Build pipeline passes (CI)
- [ ] Deployed to development environment
- [ ] Smoke tests pass in development

### Sign-off
- [ ] Product owner accepts functionality
- [ ] QA sign-off received
- [ ] No critical or high-priority bugs

---

## Sprint Planning

### Sprint 1 (Stories LOOP-001 to LOOP-005)
**Goal**: Core implementation of Loops.so integration

**Stories**:
- LOOP-001: Configuration (3 SP)
- LOOP-002: Service Implementation (8 SP)
- LOOP-003: Service Registration (2 SP)
- LOOP-004: Controller Integration (5 SP)
- LOOP-005: Comprehensive Logging (3 SP)

**Total**: 21 Story Points  
**Duration**: 1 week (5 working days)  
**Team**: 2 Backend Developers

### Sprint 2 (Stories LOOP-006 to LOOP-007)
**Goal**: Comprehensive testing and validation

**Stories**:
- LOOP-006: Unit Tests (5 SP)
- LOOP-007: Integration Tests (4 SP)
- Documentation tasks
- DevOps tasks

**Total**: 9 Story Points + Tasks  
**Duration**: 1 week (5 working days)  
**Team**: 1 Backend Developer, 1 QA Engineer, 1 DevOps Engineer

---

## Risk Management

### Risk 1: Loops.so API Rate Limiting
**Probability**: Medium  
**Impact**: Medium  
**Mitigation**: 
- Monitor sync rate in production
- Implement queue-based processing if needed
- Document rate limits from Loops.so

### Risk 2: API Key Exposure
**Probability**: Low  
**Impact**: High  
**Mitigation**:
- Use secrets management (never commit to repo)
- Rotate keys regularly
- Audit access to secrets

### Risk 3: Loops.so Service Downtime
**Probability**: Low  
**Impact**: Low  
**Mitigation**:
- Fire-and-forget pattern prevents client creation failures
- Comprehensive error logging
- Feature flag for quick disable

### Risk 4: Performance Degradation
**Probability**: Low  
**Impact**: Medium  
**Mitigation**:
- Fire-and-forget ensures no blocking
- Load testing before production release
- Monitor response times

### Risk 5: Data Privacy Concerns
**Probability**: Medium  
**Impact**: High  
**Mitigation**:
- Verify Loops.so GDPR compliance
- Document data sharing in privacy policy
- Allow users to opt-out if required

---

## Success Metrics

### Development Metrics
- [ ] All 7 user stories completed
- [ ] 100% of acceptance criteria met
- [ ] >95% code coverage for LoopsService
- [ ] Zero critical bugs in production

### Performance Metrics
- [ ] API response time unchanged (<150ms for client creation)
- [ ] 95%+ sync success rate
- [ ] <1% error rate for Loops.so calls

### Operational Metrics
- [ ] Zero production incidents related to integration
- [ ] <5 minutes average time to diagnose issues (via logs)
- [ ] Feature flag toggle works instantly

### Business Metrics
- [ ] 100% of new clients automatically synced
- [ ] Reduced manual data entry time
- [ ] Email campaigns can start immediately after client creation

---

## Appendix

### A. API Request/Response Examples

**Request to Loops.so**:
```json
POST https://app.loops.so/api/v1/contacts/create
Authorization: Bearer YOUR_API_KEY
Content-Type: application/json

{
  "email": "john.smith@example.com",
  "firstName": "John",
  "lastName": "Smith",
  "source": "CRM API",
  "subscribed": true,
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Success Response**:
```json
{
  "success": true,
  "id": "clxyz123abc456",
  "message": "Contact created successfully"
}
```

**Error Response**:
```json
{
  "success": false,
  "message": "Contact with this email already exists"
}
```

### B. Configuration Example

**appsettings.json**:
```json
{
  "Loops": {
    "ApiKey": "",
    "BaseUrl": "https://app.loops.so/api/v1",
    "Enabled": false,
    "DefaultSource": "CRM API",
    "TimeoutSeconds": 30
  }
}
```

**appsettings.Development.json**:
```json
{
  "Loops": {
    "ApiKey": "loops_dev_test_key",
    "Enabled": true
  }
}
```

**User Secrets (Development)**:
```bash
dotnet user-secrets set "Loops:ApiKey" "your_actual_api_key_here"
```

**Environment Variables (Production)**:
```bash
LOOPS__APIKEY=your_production_api_key
LOOPS__ENABLED=true
```

### C. Logging Examples

**Debug - Integration Disabled**:
```
[DBG] Loops.so integration is disabled. Skipping contact creation for john.smith@example.com
```

**Warning - Missing API Key**:
```
[WRN] Loops.so API key is not configured. Cannot create contact for john.smith@example.com
```

**Information - Successful Sync**:
```
[INF] Creating contact in Loops.so for john.smith@example.com
[INF] Successfully created contact in Loops.so for john.smith@example.com. Response: Contact created successfully
```

**Warning - HTTP Error**:
```
[WRN] Failed to create contact in Loops.so for john.smith@example.com. Status: 400, Response: Invalid email format
```

**Error - Network Failure**:
```
[ERR] HTTP error while creating contact in Loops.so for john.smith@example.com
System.Net.Http.HttpRequestException: Connection refused
   at System.Net.Http.HttpClient.SendAsync(...)
```

### D. Useful Commands

**Set User Secret**:
```bash
cd src/Api
dotnet user-secrets set "Loops:ApiKey" "your_key_here"
```

**List User Secrets**:
```bash
dotnet user-secrets list
```

**Run Tests**:
```bash
dotnet test --filter "FullyQualifiedName~LoopsService"
```

**Run with Coverage**:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**Check Configuration**:
```bash
dotnet run --environment Development
# Check logs for "Loops.so integration is disabled" or API calls
```

---

## Document Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-16 | Development Team | Initial creation of stories, tasks, and AC |

---

**End of Document**
