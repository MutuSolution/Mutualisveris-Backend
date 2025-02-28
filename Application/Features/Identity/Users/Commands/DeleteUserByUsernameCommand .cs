using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;

public class DeleteUserByUsernameCommand : IRequest<IResponseWrapper>
{
    public DeleteUserByUsernameRequest DeleteUserByUsername { get; set; }

}
public class DeleteUserByUsernameCommandHandler : IRequestHandler<DeleteUserByUsernameCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public DeleteUserByUsernameCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(DeleteUserByUsernameCommand request, CancellationToken cancellationToken)
    {
        return await _userService.DeleteAsync(request.DeleteUserByUsername);

    }
}
