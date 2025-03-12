using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<IResponseWrapper>
{
    public int ProductId { get; set; }
}

public class GetProductByIdQueryHandler :
    IRequestHandler<GetProductByIdQuery, IResponseWrapper>
{
    private readonly IProductService _productService;

    public GetProductByIdQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IResponseWrapper> Handle(GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _productService.GetProductByIdAsync(request.ProductId);
    }
}
