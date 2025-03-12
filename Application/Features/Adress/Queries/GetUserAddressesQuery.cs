using Common.Responses.Addresses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Address.Queries;

public class GetUserAddressesQuery : IRequest<IResponseWrapper<List<AddressResponse>>>
{
    public string UserId { get; set; }
}