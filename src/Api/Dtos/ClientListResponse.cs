namespace Api.Dtos;

public class ClientListResponse
{
    public List<ClientListItemDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string Sort { get; set; } = "lastName";
    public string Dir { get; set; } = "asc";
}
