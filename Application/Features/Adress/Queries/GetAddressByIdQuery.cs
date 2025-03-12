using Application.Services;
using Common.Responses.Addresses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Address.Queries;

public class GetAddressByIdQuery : IRequest<IResponseWrapper<AddressResponse>>
{
    public int AddressId { get; set; }
}