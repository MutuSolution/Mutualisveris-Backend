using Application.Pipelines;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Categories.Queries;

public class GetCategoryByIdQuery : IRequest<IResponseWrapper>, IValidateMe
{
    public int CategoryID { get; set; }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, IResponseWrapper>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryByIdQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IResponseWrapper> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.GetCategoryByIdAsync(request.CategoryID);
    }
}
