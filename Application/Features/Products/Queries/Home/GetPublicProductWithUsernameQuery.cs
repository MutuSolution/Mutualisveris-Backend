using Application.Services;
using AutoMapper;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries.Home;

public class GetPublicProductWithUsernameQuery : IRequest<IResponseWrapper>
{
    public string UserName { get; set; }
}

public class GetPublicProductWithUsernameQueryHandler : IRequestHandler<GetPublicProductWithUsernameQuery, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    public GetPublicProductWithUsernameQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    public async Task<IResponseWrapper> Handle(GetPublicProductWithUsernameQuery request, CancellationToken cancellationToken)
    {
        var productList = await _productService.GetPublicProductWithUsernameAsync(request.UserName);
        if (productList.Count > 0)
        {
            var mappedProductList = _mapper.Map<List<ProductResponse>>(productList);
            return await ResponseWrapper<List<ProductResponse>>
                .SuccessAsync(mappedProductList);
        }
        return await ResponseWrapper.FailAsync("[ML97] No products were found.");
    }
}
