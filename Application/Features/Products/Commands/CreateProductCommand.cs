using Application.Pipelines;
using Application.Services;
using Common.Requests.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public CreateProductRequest CreateProduct { get; set; }
}

public class CreateProductCommandHandler :
    IRequestHandler<CreateProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;

    public CreateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IResponseWrapper> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.CreateProductAsync(request.CreateProduct);
    }
}