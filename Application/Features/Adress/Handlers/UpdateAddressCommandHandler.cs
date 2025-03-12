using Application.Features.Address.Commands;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Adress.Handlers;
public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, IResponseWrapper>
{
    private readonly IAddressService _addressService;

    public UpdateAddressCommandHandler(IAddressService addressService)
    {
        _addressService = addressService;
    }

    public async Task<IResponseWrapper> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        return await _addressService.UpdateAddressAsync(request.Request);
    }
}
