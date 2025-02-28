using Application.Services.Identity;
using AutoMapper;
using Common.Responses.Identity;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Queries;

public class GetPagedUsersQuery : IRequest<IResponseWrapper<PaginationResult<UserResponse>>>
{
    public UserParameters Parameters { get; set; } = new UserParameters();
}

public class GetPagedUsersQueryHandler : IRequestHandler<GetPagedUsersQuery, IResponseWrapper<PaginationResult<UserResponse>>>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetPagedUsersQueryHandler(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    async Task<IResponseWrapper<PaginationResult<UserResponse>>> IRequestHandler<GetPagedUsersQuery, IResponseWrapper<PaginationResult<UserResponse>>>.Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
    {
        var pagedResult = await _userService.GetPagedUsersAsync(request.Parameters);
        var mappedItems = _mapper.Map<IEnumerable<UserResponse>>(pagedResult.Items);
        return await ResponseWrapper<PaginationResult<UserResponse>>
        .SuccessAsync(new PaginationResult<UserResponse>(
            mappedItems,
            pagedResult.TotalCount,
            pagedResult.TotalPage,
            pagedResult.Page,
            pagedResult.ItemsPerPage
        ));

    }
}