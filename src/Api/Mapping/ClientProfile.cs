using Api.Domain;
using Api.Dtos;
using AutoMapper;

namespace Api.Mapping;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<Client, ClientListItemDto>();
        CreateMap<Client, ClientDetailDto>();
        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();
    }
}
