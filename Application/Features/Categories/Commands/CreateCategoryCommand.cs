using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Request.Category;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands;

public class CreateCategoryCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public CreateCategoryRequest Request { get; set; }
}
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResponseWrapper>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var mappedCategory = _mapper.Map<Category>(request.Request);
        var newCategory = await _categoryService.CreateCategoryAsync(mappedCategory);
        if (newCategory.Id > 0)
        {
            var mappedNewCategory = _mapper.Map<CategoryResponse>(newCategory);
            return await ResponseWrapper<CategoryResponse>
                .SuccessAsync(mappedNewCategory, "[ML18] Category created successfully.");
        }
        return await ResponseWrapper<string>.FailAsync("[ML19] Failed to create category entry.");

    }
}