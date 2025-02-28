using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Queries;

public class GetUserByUserNameQuery : IRequest<IResponseWrapper>
{
    public string UserName { get; set; }
}

public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetUserByUserNameQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByUserNameAsync(request.UserName);
    }
}
