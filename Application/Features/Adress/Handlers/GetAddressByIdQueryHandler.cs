using Application.Features.Address.Queries;
using Application.Services;
using Common.Responses.Addresses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Adress.Handlers;
public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, IResponseWrapper<AddressResponse>>
{
    private readonly IAddressService _addressService;

    public GetAddressByIdQueryHandler(IAddressService addressService)
    {
        _addressService = addressService;
    }

    public async Task<IResponseWrapper<AddressResponse>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        return await _addressService.GetAddressByIdAsync(request.AddressId);
    }
}
