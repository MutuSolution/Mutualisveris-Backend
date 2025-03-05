using Application.Services.Identity;
using AutoMapper;
using Common.Responses.Identity;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Queries
{
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

        public async Task<IResponseWrapper<PaginationResult<UserResponse>>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
        {
            // Servisten sayfalı kullanıcı verisi alınıyor
            var pagedResultWrapper = await _userService.GetPagedUsersAsync(request.Parameters);

            // Servisten hata döndüyse direkt hata mesajını iletelim
            if (!pagedResultWrapper.IsSuccessful)
            {
                return ResponseWrapper<PaginationResult<UserResponse>>.Fail(pagedResultWrapper.Messages);
            }

            // Gelen verideki Items'ı DTO'ya eşliyoruz
            var mappedItems = _mapper.Map<IEnumerable<UserResponse>>(pagedResultWrapper.ResponseData.Items);

            var paginationResult = new PaginationResult<UserResponse>(
                mappedItems,
                pagedResultWrapper.ResponseData.TotalCount,
                pagedResultWrapper.ResponseData.TotalPage,
                pagedResultWrapper.ResponseData.Page,
                pagedResultWrapper.ResponseData.ItemsPerPage
            );

            // Başarılı sonuç, mesaj ile birlikte döndürülüyor
            return await ResponseWrapper<PaginationResult<UserResponse>>.SuccessAsync(paginationResult, "Sayfalı kullanıcı listesi başarıyla getirildi.");
        }
    }
}
