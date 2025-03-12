using Application.Services;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries;

public class GetProductsQuery : IRequest<IResponseWrapper<PaginationResult<ProductResponse>>>
{
    public ProductParameters Parameters { get; set; } = new ProductParameters();
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IResponseWrapper<PaginationResult<ProductResponse>>>
{
    private readonly IProductService _productService;

    public GetProductsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IResponseWrapper<PaginationResult<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductsAsync(request.Parameters);
    }
}
