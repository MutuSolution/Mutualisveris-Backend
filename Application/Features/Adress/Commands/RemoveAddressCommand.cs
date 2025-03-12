using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Address.Commands;

public class RemoveAddressCommand : IRequest<IResponseWrapper>
{
    public int AddressId { get; set; }
}