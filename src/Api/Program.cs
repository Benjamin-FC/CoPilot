using Api.Configuration;
using Api.Data;
using Api.Dtos;
using Api.Mapping;
using Api.Services;
using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ClientsDb"));

// Configure AutoMapper with the assembly containing the ClientProfile
builder.Services.AddAutoMapper(typeof(ClientProfile).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<CreateClientValidator>();

// Configure Loops.so integration
builder.Services.Configure<LoopsOptions>(
    builder.Configuration.GetSection(LoopsOptions.SectionName));

builder.Services.AddHttpClient<ILoopsService, LoopsService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<LoopsOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CRM API",
        Version = "v1",
        Description = "Client Relationship Management API",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@example.com"
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("Development");
app.UseAuthorization();
app.MapControllers();

app.Run();
