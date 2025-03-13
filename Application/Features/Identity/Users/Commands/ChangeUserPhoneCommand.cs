using Application.Services.Identity;
using Common.Request.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class ChangeUserPhoneCommand : IRequest<IResponseWrapper>
{
    public ChangeUserPhoneRequest ChangeUserPhoneRequest { get; set; }
}

public class ChangeUserPhoneCommandHandler : IRequestHandler<ChangeUserPhoneCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public ChangeUserPhoneCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(ChangeUserPhoneCommand request, CancellationToken cancellationToken)
    {
        return await _userService
            .UpdateUserPhoneNumberAsync(request.ChangeUserPhoneRequest.UserId, request.ChangeUserPhoneRequest.PhoneNumber);
    }
}

