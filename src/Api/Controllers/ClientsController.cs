using Api.Data;
using Api.Domain;
using Api.Dtos;
using Api.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateClientDto> _createValidator;
    private readonly IValidator<UpdateClientDto> _updateValidator;
    private readonly ILoopsService _loopsService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(
        AppDbContext context,
        IMapper mapper,
        IValidator<CreateClientDto> createValidator,
        IValidator<UpdateClientDto> updateValidator,
        ILoopsService loopsService,
        ILogger<ClientsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _loopsService = loopsService;
        _logger = logger;
    }

    /// <summary>
    /// Get all clients with search, sort, and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ClientListResponse>> GetClients(
        [FromQuery] string? query = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sort = "lastName",
        [FromQuery] string dir = "asc",
        [FromQuery] bool? isActive = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        IQueryable<Client> queryClients = _context.Clients;

        if (isActive.HasValue)
        {
            queryClients = queryClients.Where(c => c.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var searchTerm = query.ToLower();
            queryClients = queryClients.Where(c =>
                c.FirstName.ToLower().Contains(searchTerm) ||
                c.LastName.ToLower().Contains(searchTerm) ||
                c.Email.ToLower().Contains(searchTerm) ||
                (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                (c.Company != null && c.Company.ToLower().Contains(searchTerm)));
        }

        queryClients = sort.ToLower() switch
        {
            "firstname" => dir.ToLower() == "desc"
                ? queryClients.OrderByDescending(c => c.FirstName)
                : queryClients.OrderBy(c => c.FirstName),
            "lastname" => dir.ToLower() == "desc"
                ? queryClients.OrderByDescending(c => c.LastName)
                : queryClients.OrderBy(c => c.LastName),
            "email" => dir.ToLower() == "desc"
                ? queryClients.OrderByDescending(c => c.Email)
                : queryClients.OrderBy(c => c.Email),
            "company" => dir.ToLower() == "desc"
                ? queryClients.OrderByDescending(c => c.Company)
                : queryClients.OrderBy(c => c.Company),
            "createdat" => dir.ToLower() == "desc"
                ? queryClients.OrderByDescending(c => c.CreatedAt)
                : queryClients.OrderBy(c => c.CreatedAt),
            _ => dir.ToLower() == "desc"
                ? queryClients.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName)
                : queryClients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
        };

        var total = await queryClients.CountAsync();

        var clients = await queryClients
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ClientListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

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

    /// <summary>
    /// Get a client by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDetailDto>> GetClient(Guid id)
    {
        var client = await _context.Clients
            .ProjectTo<ClientDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
        {
            return NotFound(new { message = $"Client with ID {id} not found." });
        }

        return Ok(client);
    }

    /// <summary>
    /// Create a new client
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ClientDetailDto>> CreateClient([FromBody] CreateClientDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.ToDictionary() });
        }

        var existingClient = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == createDto.Email);
        if (existingClient != null)
        {
            return Conflict(new { message = "A client with this email already exists." });
        }

        var client = _mapper.Map<Client>(createDto);
        client.Id = Guid.NewGuid();
        client.CreatedAt = DateTimeOffset.UtcNow;
        client.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Sync contact to Loops.so asynchronously (non-blocking)
        _ = Task.Run(async () =>
        {
            try
            {
                await _loopsService.CreateContactAsync(
                    email: client.Email,
                    firstName: client.FirstName,
                    lastName: client.LastName,
                    userId: client.Id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync contact to Loops.so for client {ClientId}", client.Id);
            }
        });

        var resultDto = _mapper.Map<ClientDetailDto>(client);
        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, resultDto);
    }

    /// <summary>
    /// Update an existing client
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDetailDto>> UpdateClient(Guid id, [FromBody] UpdateClientDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.ToDictionary() });
        }

        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return NotFound(new { message = $"Client with ID {id} not found." });
        }

        if (client.Email != updateDto.Email)
        {
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == updateDto.Email && c.Id != id);
            if (existingClient != null)
            {
                return Conflict(new { message = "A client with this email already exists." });
            }
        }

        _mapper.Map(updateDto, client);
        client.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Clients.Update(client);
        await _context.SaveChangesAsync();

        var resultDto = _mapper.Map<ClientDetailDto>(client);
        return Ok(resultDto);
    }

    /// <summary>
    /// Delete a client
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteClient(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return NotFound(new { message = $"Client with ID {id} not found." });
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
