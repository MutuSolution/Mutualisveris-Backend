using Application.Features.Address.Queries;
using Application.Services;
using Common.Responses.Addresses;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Adress.Handlers;
public class GetUserAddressesQueryHandler : IRequestHandler<GetUserAddressesQuery, IResponseWrapper<List<AddressResponse>>>
{
    private readonly IAddressService _addressService;

    public GetUserAddressesQueryHandler(IAddressService addressService)
    {
        _addressService = addressService;
    }

    public async Task<IResponseWrapper<List<AddressResponse>>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
    {
        return await _addressService.GetUserAddressesAsync(request.UserId);
    }
}
