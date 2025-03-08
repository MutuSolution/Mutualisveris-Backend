using Application.Pipelines;
using Application.Services;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain.Responses;
using MediatR;

namespace Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<IResponseWrapper<PaginationResult<CategoryResponse>>>, IValidateMe
    {
        public CategoryParameters Parameters { get; set; } = new CategoryParameters();
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IResponseWrapper<PaginationResult<CategoryResponse>>>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IResponseWrapper<PaginationResult<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _categoryService.GetCategoriesAsync(request.Parameters);
        }
    }
}
