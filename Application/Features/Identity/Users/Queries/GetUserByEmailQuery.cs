using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Queries;

public class GetUserByEmailQuery : IRequest<IResponseWrapper>
{
    public string Email { get; set; }
}

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetUserByEmailQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByEmailAsync(request.Email);
    }
}

