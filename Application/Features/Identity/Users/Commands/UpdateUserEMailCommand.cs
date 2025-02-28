using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class UpdateUserEMailCommand : IRequest<IResponseWrapper>
{
    public ChangeEmailRequest request { get; set; }
}

public class UpdateUserEMailCommandHandler : IRequestHandler<UpdateUserEMailCommand, IResponseWrapper>
{
    private readonly IUserService _userService;
    public UpdateUserEMailCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(UpdateUserEMailCommand request, CancellationToken cancellationToken)
    {
        return await _userService.ChangeUserEmailAsync(request.request);
    }
}

