using Common.Requests.Addresses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Address.Commands;

public class UpdateAddressCommand : IRequest<IResponseWrapper>
{
    public UpdateAddressRequest Request { get; set; }
}