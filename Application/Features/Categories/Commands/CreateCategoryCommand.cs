using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Request.Category;
using Common.Responses.Wrappers;
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

    public Task<IResponseWrapper> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}