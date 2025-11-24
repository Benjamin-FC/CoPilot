namespace Api.Services;

public interface ILoopsService
{
    /// <summary>
    /// Creates a contact in Loops.so
    /// </summary>
    /// <param name="email">Contact email address (required)</param>
    /// <param name="firstName">Contact first name (optional)</param>
    /// <param name="lastName">Contact last name (optional)</param>
    /// <param name="userId">External user ID (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> CreateContactAsync(
        string email, 
        string? firstName = null, 
        string? lastName = null,
        string? userId = null,
        CancellationToken cancellationToken = default);
}
