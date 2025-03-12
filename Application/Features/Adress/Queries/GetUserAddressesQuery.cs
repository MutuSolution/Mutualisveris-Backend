using Application.Services;
using Common.Responses.Addresses;
using Common.Responses.Wrappers;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Address.Queries;

public class GetUserAddressesQuery : IRequest<IResponseWrapper<List<AddressResponse>>>
{
    public string UserId { get; set; }
}