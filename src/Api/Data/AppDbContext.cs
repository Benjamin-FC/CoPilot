using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { 
    }

    public DbSet<Client> Clients { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Client entity
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
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

        // Seed data
        var clients = GenerateSampleClients();
        modelBuilder.Entity<Client>().HasData(clients);
    }

    private static List<Client> GenerateSampleClients()
    {
        var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emma", "Robert", "Lisa", "James", "Mary" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
        var companies = new[] { "Acme Corp", "TechStart Inc", "Global Solutions", "Digital Ventures", "Innovate Labs", "Future Systems", "Smart Tech", "Data Dynamics", "Cloud Nine", "Quantum Software" };
        var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
        var states = new[] { "NY", "CA", "IL", "TX", "AZ", "PA", "TX", "CA", "TX", "CA" };

        var clients = new List<Client>();
        var random = new Random(42);

        for (int i = 1; i <= 150; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@example.com";

            var client = new Client
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = random.Next(2) == 0 ? $"{random.Next(200, 999)}-{random.Next(200, 999)}-{random.Next(1000, 9999)}" : null,
                Company = random.Next(2) == 0 ? companies[random.Next(companies.Length)] : null,
                AddressLine1 = $"{random.Next(1, 9999)} Main Street",
                AddressLine2 = random.Next(2) == 0 ? $"Suite {random.Next(100, 999)}" : null,
                City = cities[random.Next(cities.Length)],
                State = states[random.Next(states.Length)],
                PostalCode = random.Next(10000, 99999).ToString(),
                Country = "USA",
                IsActive = random.Next(10) > 1,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-random.Next(365)),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-random.Next(30))
            };

            clients.Add(client);
        }

        return clients;
    }
}
