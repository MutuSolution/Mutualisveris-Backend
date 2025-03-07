using Application.Services;
using Common.Requests.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Commands;

public class SoftDeleteProductCommand : IRequest<IResponseWrapper>
{
    public int ProductId { get; set; }
}

public class SoftDeleteProductCommandHandler : IRequestHandler<SoftDeleteProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;

    public SoftDeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IResponseWrapper> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.SoftDeleteProductAsync(request.ProductId);
    }
}
