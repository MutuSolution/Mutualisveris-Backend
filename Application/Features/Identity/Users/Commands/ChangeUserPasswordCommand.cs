using Application.Pipelines;
using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class ChangeUserPasswordCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public ChangePasswordRequest ChangePassword { get; set; }
}

public class ChangeUserPasswordCommandHandler :
    IRequestHandler<ChangeUserPasswordCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public ChangeUserPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(ChangeUserPasswordCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.ChangeUserPasswordAsync(request.ChangePassword);
    }
}
