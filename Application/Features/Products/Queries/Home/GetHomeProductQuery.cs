using Application.Services;
using AutoMapper;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries.Home;

public class GetHomeProductQuery : IRequest<IResponseWrapper>
{

}

public class GetHomeProductQueryHandler : IRequestHandler<GetHomeProductQuery, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public GetHomeProductQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    public async Task<IResponseWrapper> Handle(GetHomeProductQuery request, CancellationToken cancellationToken)
    {
        var productList = await _productService.GetHomeProductListAsync();
        if (productList.Count > 0)
        {
            var mappedProductList = _mapper.Map<List<ProductResponse>>(productList);
            return await ResponseWrapper<List<ProductResponse>>
                .SuccessAsync(mappedProductList);
        }
        return await ResponseWrapper.FailAsync("[ML96] No products were found.");
    }
}
