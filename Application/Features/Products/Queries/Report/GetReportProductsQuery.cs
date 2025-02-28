using Application.Services;
using AutoMapper;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries.Report;

public class GetReportProductsQuery : IRequest<IResponseWrapper>
{
}

public class GetReportProductsQueryHandler : IRequestHandler<GetReportProductsQuery, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public GetReportProductsQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    public async Task<IResponseWrapper> Handle(GetReportProductsQuery request, CancellationToken cancellationToken)
    {
        var productList = await _productService.GetProductReportsAsync();
        if (productList.Count > 0)
        {
            var mappedProductList = _mapper.Map<List<ProductReportResponse>>(productList);
            return await ResponseWrapper<List<ProductReportResponse>>
                .SuccessAsync(mappedProductList);
        }
        return await ResponseWrapper.FailAsync("[ML113] No products were found.");
    }
}