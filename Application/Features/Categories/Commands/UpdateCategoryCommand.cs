using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Request.Category;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Categories.Commands;

public class UpdateCategoryCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public UpdateCategoryRequest UpdateCategory { get; set; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResponseWrapper>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryService.UpdateCategoryAsync(request.UpdateCategory);
    }
}
