using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;
//RegisterUserByAdminAsync
public class UserAddingByAdminCommand : IRequest<IResponseWrapper>
{
    public UserRegistrationRequest UserRegistration { get; set; }
}

public class UserAddingByAdminCommandHandler : IRequestHandler<UserAddingByAdminCommand, IResponseWrapper>
{
    private readonly IUserService _userService;
    public UserAddingByAdminCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(UserAddingByAdminCommand request, CancellationToken cancellationToken)
    {
        return await _userService.RegisterUserByAdminAsync(request.UserRegistration);
    }
}






