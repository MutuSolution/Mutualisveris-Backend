using Application.Pipelines;
using Application.Services;
using Common.Requests.Category;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Categories.Commands;

public class CreateCategoryCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public CreateCategoryRequest CreateCategory { get; set; }
}
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResponseWrapper>
{
    private readonly ICategoryService _categoryService;

    public CreateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;

    }

    public async Task<IResponseWrapper> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryService.CreateCategoryAsync(request.CreateCategory);
    }
}