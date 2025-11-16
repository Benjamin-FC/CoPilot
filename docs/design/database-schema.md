# Database Schema Design

## Document Information
- **Version**: 1.0
- **Last Updated**: November 16, 2025
- **Database**: SQL Server / In-Memory

## Overview

The CRM API uses a simple, normalized relational database schema centered around the Client entity. The design prioritizes data integrity, query performance, and scalability.

## Database Technologies

### Current Implementation
- **Development**: Entity Framework Core In-Memory Database
- **Production-Ready**: SQL Server, PostgreSQL, MySQL

### ORM
- **Entity Framework Core 8.0**
- Code-First approach
- Migration support

## Schema Diagram

```
┌─────────────────────────────────────────────────────────┐
│                        Clients                          │
├─────────────────────────────────────────────────────────┤
│ PK  Id              uniqueidentifier (GUID)             │
│     FirstName       nvarchar(100)     NOT NULL          │
│     LastName        nvarchar(100)     NOT NULL          │
│ UK  Email           nvarchar(255)     NOT NULL          │
│     Phone           nvarchar(20)      NULL              │
│     Company         nvarchar(255)     NULL              │
│     AddressLine1    nvarchar(255)     NULL              │
│     AddressLine2    nvarchar(255)     NULL              │
│     City            nvarchar(100)     NULL              │
│     State           nvarchar(100)     NULL              │
│     PostalCode      nvarchar(20)      NULL              │
│     Country         nvarchar(100)     NULL              │
│     IsActive        bit               NOT NULL (1)      │
│     CreatedAt       datetimeoffset    NOT NULL          │
│     UpdatedAt       datetimeoffset    NOT NULL          │
└─────────────────────────────────────────────────────────┘

Indexes:
  PK_Clients               PRIMARY KEY (Id)
  IX_Clients_Email         UNIQUE INDEX (Email)
  IX_Clients_Name          INDEX (FirstName, LastName)
  IX_Clients_IsActive      INDEX (IsActive)
  IX_Clients_CreatedAt     INDEX (CreatedAt)
```

## Entity: Client

### Table Name
`Clients`

### Columns

| Column | Type | Length | Nullable | Default | Description |
|--------|------|--------|----------|---------|-------------|
| Id | uniqueidentifier | - | NO | - | Primary key, GUID |
| FirstName | nvarchar | 100 | NO | - | Client's first name |
| LastName | nvarchar | 100 | NO | - | Client's last name |
| Email | nvarchar | 255 | NO | - | Email address (unique) |
| Phone | nvarchar | 20 | YES | NULL | Phone number |
| Company | nvarchar | 255 | YES | NULL | Company name |
| AddressLine1 | nvarchar | 255 | YES | NULL | Address line 1 |
| AddressLine2 | nvarchar | 255 | YES | NULL | Address line 2 |
| City | nvarchar | 100 | YES | NULL | City |
| State | nvarchar | 100 | YES | NULL | State/Province |
| PostalCode | nvarchar | 20 | YES | NULL | Postal/ZIP code |
| Country | nvarchar | 100 | YES | NULL | Country |
| IsActive | bit | - | NO | 1 | Active status flag |
| CreatedAt | datetimeoffset | - | NO | - | Record creation timestamp (UTC) |
| UpdatedAt | datetimeoffset | - | NO | - | Last update timestamp (UTC) |

### Constraints

#### Primary Key
```sql
CONSTRAINT PK_Clients PRIMARY KEY (Id)
```

#### Unique Constraint
```sql
CONSTRAINT UK_Clients_Email UNIQUE (Email)
```

Ensures email uniqueness across all clients.

#### Check Constraints (Recommended)
```sql
-- Email format validation (basic)
CONSTRAINT CK_Clients_Email CHECK (Email LIKE '%@%')

-- Phone format validation (if applicable)
CONSTRAINT CK_Clients_Phone CHECK (Phone IS NULL OR Phone LIKE '[0-9][0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]')

-- Timestamps
CONSTRAINT CK_Clients_UpdatedAt CHECK (UpdatedAt >= CreatedAt)
```

### Indexes

#### 1. Primary Key Index (Clustered)
```sql
CREATE UNIQUE CLUSTERED INDEX PK_Clients 
ON Clients (Id);
```
**Purpose**: Primary key enforcement, fast lookups by ID

#### 2. Email Index (Unique, Non-Clustered)
```sql
CREATE UNIQUE NONCLUSTERED INDEX IX_Clients_Email 
ON Clients (Email);
```
**Purpose**: 
- Enforce email uniqueness
- Fast duplicate email checks during create/update
- Fast lookups by email

**Query Patterns**:
```sql
-- Check if email exists
SELECT * FROM Clients WHERE Email = 'john@example.com';
```

#### 3. Name Composite Index (Non-Clustered)
```sql
CREATE NONCLUSTERED INDEX IX_Clients_Name 
ON Clients (FirstName, LastName);
```
**Purpose**:
- Fast searching by name
- Efficient sorting by name

**Query Patterns**:
```sql
-- Search by name
SELECT * FROM Clients 
WHERE FirstName LIKE '%John%' OR LastName LIKE '%Smith%';

-- Sort by name
SELECT * FROM Clients 
ORDER BY FirstName, LastName;
```

#### 4. IsActive Index (Non-Clustered)
```sql
CREATE NONCLUSTERED INDEX IX_Clients_IsActive 
ON Clients (IsActive);
```
**Purpose**:
- Filter active/inactive clients efficiently
- Support common queries filtering by status

**Query Patterns**:
```sql
-- Get only active clients
SELECT * FROM Clients WHERE IsActive = 1;
```

#### 5. CreatedAt Index (Non-Clustered)
```sql
CREATE NONCLUSTERED INDEX IX_Clients_CreatedAt 
ON Clients (CreatedAt DESC);
```
**Purpose**:
- Sort by creation date (newest first)
- Support time-based queries

**Query Patterns**:
```sql
-- Get recently created clients
SELECT * FROM Clients 
ORDER BY CreatedAt DESC;

-- Get clients created in date range
SELECT * FROM Clients 
WHERE CreatedAt BETWEEN '2025-01-01' AND '2025-12-31';
```

## Data Types Rationale

### uniqueidentifier (GUID) for Id
**Advantages**:
- Globally unique across distributed systems
- No collision risk
- Generated client-side (reduces database round-trips)
- Suitable for replication and merging

**Disadvantages**:
- Larger storage (16 bytes vs 4 bytes for int)
- Non-sequential (index fragmentation)

**Mitigation**: Use clustered index on GUID for performance

### nvarchar for Text Fields
**Rationale**:
- Unicode support (international characters)
- Variable length (storage efficiency)
- Compatible with .NET string type

### datetimeoffset for Timestamps
**Rationale**:
- Stores UTC offset (timezone-aware)
- Precision: 100 nanoseconds
- Better for distributed systems
- ISO 8601 compatible

### bit for Boolean (IsActive)
**Rationale**:
- Smallest storage (1 byte)
- Native boolean type in SQL Server
- Direct mapping to C# bool

## Data Integrity

### Entity Framework Configuration
```csharp
modelBuilder.Entity<Client>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // Required fields
    entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
    entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
    entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
    
    // Optional fields with max length
    entity.Property(e => e.Phone).HasMaxLength(20);
    entity.Property(e => e.Company).HasMaxLength(255);
    entity.Property(e => e.AddressLine1).HasMaxLength(255);
    entity.Property(e => e.AddressLine2).HasMaxLength(255);
    entity.Property(e => e.City).HasMaxLength(100);
    entity.Property(e => e.State).HasMaxLength(100);
    entity.Property(e => e.PostalCode).HasMaxLength(20);
    entity.Property(e => e.Country).HasMaxLength(100);
    
    // Indexes
    entity.HasIndex(e => e.Email).IsUnique();
    entity.HasIndex(e => new { e.FirstName, e.LastName });
    entity.HasIndex(e => e.IsActive);
    entity.HasIndex(e => e.CreatedAt);
});
```

### Application-Level Validation
- FluentValidation rules enforce constraints before database operations
- Email format validation
- Phone format validation
- Required field checks

## Query Performance Analysis

### Common Query Patterns

#### 1. Get Client by ID
```sql
SELECT * FROM Clients WHERE Id = @Id;
```
**Index Used**: PK_Clients (clustered)
**Performance**: O(log n) - very fast

#### 2. Get Client by Email
```sql
SELECT * FROM Clients WHERE Email = @Email;
```
**Index Used**: IX_Clients_Email (unique)
**Performance**: O(log n) - very fast

#### 3. Search Clients
```sql
SELECT * FROM Clients 
WHERE FirstName LIKE '%John%' 
   OR LastName LIKE '%Smith%' 
   OR Email LIKE '%john%'
   OR Company LIKE '%Acme%';
```
**Index Used**: IX_Clients_Name (partial), IX_Clients_Email (partial)
**Performance**: O(n) - full/partial scan (LIKE with leading wildcard)
**Improvement**: Full-text search index for better performance

#### 4. Paginated List with Sorting
```sql
SELECT * FROM Clients 
WHERE IsActive = 1
ORDER BY LastName, FirstName
OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
```
**Index Used**: IX_Clients_IsActive, IX_Clients_Name
**Performance**: O(log n + m) where m = pageSize

#### 5. Count Total Clients
```sql
SELECT COUNT(*) FROM Clients WHERE IsActive = 1;
```
**Index Used**: IX_Clients_IsActive
**Performance**: Index scan - fast

## Sample Data Seeding

### Purpose
- Pre-populated database for development/testing
- 150 sample clients with realistic data

### Seed Data Characteristics
- Random combinations of first names, last names
- Unique emails (sequential numbering)
- Random companies, phone numbers (optional)
- Random addresses across US cities
- 90% active, 10% inactive
- Creation dates spread over past year
- Update dates within past 30 days

### Seed Method
```csharp
private static List<Client> GenerateSampleClients()
{
    // Generate 150 clients with:
    // - 10 first names
    // - 10 last names
    // - 10 companies
    // - 10 cities/states
    // - Random optional fields
    // - Deterministic seed (42) for consistency
}
```

## Migration Strategy

### Initial Migration
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Future Migrations (Examples)

#### Add Client Notes
```csharp
public partial class AddClientNotes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Notes",
            table: "Clients",
            type: "nvarchar(max)",
            nullable: true);
    }
}
```

#### Add Client Tags (Many-to-Many)
```csharp
public partial class AddClientTags : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Tags",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 50, nullable: false)
            });
            
        migrationBuilder.CreateTable(
            name: "ClientTags",
            columns: table => new
            {
                ClientId = table.Column<Guid>(nullable: false),
                TagId = table.Column<int>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClientTags", x => new { x.ClientId, x.TagId });
                table.ForeignKey(
                    name: "FK_ClientTags_Clients",
                    column: x => x.ClientId,
                    principalTable: "Clients",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ClientTags_Tags",
                    column: x => x.TagId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }
}
```

## Backup and Recovery

### Recommended Backup Strategy

#### Development
- In-memory database: No persistence required
- Seed data regenerates on startup

#### Production
- **Full Backup**: Daily
- **Differential Backup**: Every 6 hours
- **Transaction Log Backup**: Every 15 minutes
- **Point-in-Time Recovery**: Enabled
- **Retention**: 30 days

### Backup Commands (SQL Server)
```sql
-- Full backup
BACKUP DATABASE CrmDb 
TO DISK = 'D:\Backups\CrmDb_Full.bak' 
WITH FORMAT, COMPRESSION;

-- Differential backup
BACKUP DATABASE CrmDb 
TO DISK = 'D:\Backups\CrmDb_Diff.bak' 
WITH DIFFERENTIAL, COMPRESSION;

-- Transaction log backup
BACKUP LOG CrmDb 
TO DISK = 'D:\Backups\CrmDb_Log.trn' 
WITH COMPRESSION;
```

## Scalability Considerations

### Horizontal Scaling
- **Read Replicas**: Route read queries to replicas
- **Connection Pooling**: Managed by EF Core
- **Sharding**: Partition by client ID ranges (future)

### Vertical Scaling
- Increase database server resources (CPU, RAM, storage)
- Upgrade to higher-tier database service (Azure SQL, AWS RDS)

### Performance Tuning
- Monitor index usage and fragmentation
- Update statistics regularly
- Query execution plan analysis
- Add covering indexes for frequent queries

### Monitoring Metrics
- Query execution time
- Index fragmentation level
- Database size growth
- Connection pool usage
- Lock contention

## Archival Strategy (Future)

### Inactive Client Archival
- Move clients with `IsActive = false` and no activity for 2+ years to archive table
- Maintain foreign key relationships
- Periodic archival job (monthly)

### Archive Table
```sql
CREATE TABLE ArchivedClients (
    -- Same schema as Clients
    -- Plus:
    ArchivedAt datetimeoffset NOT NULL,
    ArchivedReason nvarchar(500)
);
```

## Data Privacy & Compliance

### GDPR Considerations
- **Right to Access**: Query client by email/ID
- **Right to Rectification**: Update client endpoint
- **Right to Erasure**: Delete client endpoint (hard delete)
- **Data Portability**: Export client data (future)

### PII (Personally Identifiable Information)
Fields containing PII:
- FirstName, LastName, Email, Phone
- AddressLine1, AddressLine2, City, State, PostalCode, Country

Recommendations:
- Encrypt PII at rest (Transparent Data Encryption)
- Encrypt PII in transit (TLS)
- Audit access to PII (database audit logs)
- Data retention policy (auto-delete after N years)

## Future Schema Enhancements

### 1. Audit Trail
```sql
CREATE TABLE ClientAudit (
    Id bigint IDENTITY PRIMARY KEY,
    ClientId uniqueidentifier NOT NULL,
    Action varchar(50) NOT NULL, -- INSERT, UPDATE, DELETE
    ChangedBy varchar(255),
    ChangedAt datetimeoffset NOT NULL,
    OldValue nvarchar(max), -- JSON
    NewValue nvarchar(max)  -- JSON
);
```

### 2. Client Activities
```sql
CREATE TABLE ClientActivities (
    Id bigint IDENTITY PRIMARY KEY,
    ClientId uniqueidentifier NOT NULL,
    ActivityType varchar(100) NOT NULL,
    Description nvarchar(max),
    ActivityDate datetimeoffset NOT NULL,
    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);
```

### 3. Client Tags (Many-to-Many)
See Migration Strategy section above.

### 4. Client Files/Attachments
```sql
CREATE TABLE ClientFiles (
    Id bigint IDENTITY PRIMARY KEY,
    ClientId uniqueidentifier NOT NULL,
    FileName varchar(255) NOT NULL,
    FileSize bigint NOT NULL,
    ContentType varchar(100),
    StorageUrl varchar(1000), -- Azure Blob, S3, etc.
    UploadedAt datetimeoffset NOT NULL,
    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);
```

## References

- [Entity Framework Core Documentation](https://learn.microsoft.com/ef/core/)
- [SQL Server Indexing Best Practices](https://learn.microsoft.com/sql/relational-databases/indexes/)
- [Database Design Principles](https://www.sqlshack.com/database-design-best-practices/)
