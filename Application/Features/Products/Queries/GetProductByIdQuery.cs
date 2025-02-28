using Application.Services;
using AutoMapper;
using Common.Responses.Products;
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
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var productInDb = await _productService.GetProductByIdAsync(request.ProductId);
        if (productInDb is null)
            return await ResponseWrapper.SuccessAsync("[ML24] Product does not exist.");

        var mappedProduct = _mapper.Map<ProductResponse>(productInDb);
        return await ResponseWrapper<ProductResponse>.SuccessAsync(mappedProduct);
    }
}
