using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands;

public class SoftDeleteCategoryCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public int CategoryId { get; set; }
}

public class SoftDeleteCategoryCommandHandler : IRequestHandler<SoftDeleteCategoryCommand, IResponseWrapper>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public SoftDeleteCategoryCommandHandler(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(SoftDeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryService.SoftDeleteCategory(request.CategoryId);
    }
}
