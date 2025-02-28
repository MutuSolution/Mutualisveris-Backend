using Application.Services;
using AutoMapper;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries;

public class GetProductQuery : IRequest<IResponseWrapper>
{

}

public class GetProductsQueryHandler : IRequestHandler<GetProductQuery, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(GetProductQuery request,
        CancellationToken cancellationToken)
    {
        var productList = await _productService.GetProductListAsync();
        if (productList.Count > 0)
        {
            var mappedProductList = _mapper.Map<List<ProductResponse>>(productList);
            return await ResponseWrapper<List<ProductResponse>>
                .SuccessAsync(mappedProductList);
        }
        return await ResponseWrapper.FailAsync("[ML25] No products were found.");
    }
}
