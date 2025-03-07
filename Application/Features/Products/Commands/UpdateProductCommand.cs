using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Requests.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Commands;

public class UpdateProductCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public UpdateProductRequest UpdateProduct { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        return await _productService.UpdateProductAsync(request.UpdateProduct);
    }
}
