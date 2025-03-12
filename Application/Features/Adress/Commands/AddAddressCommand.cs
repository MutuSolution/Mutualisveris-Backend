using Common.Requests.Addresses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Address.Commands;

public class AddAddressCommand : IRequest<IResponseWrapper>
{
    public CreateAddressRequest Request { get; set; }
}