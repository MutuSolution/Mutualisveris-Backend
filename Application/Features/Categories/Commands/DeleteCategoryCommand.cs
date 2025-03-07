using Application.Pipelines;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Categories.Commands;
public class DeleteCategoryCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public int CategoryId { get; set; }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, IResponseWrapper>
{
    private readonly ICategoryService _categoryService;

    public DeleteCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IResponseWrapper> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryService.DeleteCategoryAsync(request.CategoryId);
    }
}
