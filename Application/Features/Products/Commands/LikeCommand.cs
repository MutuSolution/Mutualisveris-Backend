using Application.Services;
using Common.Requests.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Commands;

public class LikeCommand : IRequest<IResponseWrapper>
{
    public LikeProductRequest LikeRequest { get; set; }
}

public class LikeCommandHandler : IRequestHandler<LikeCommand, IResponseWrapper>
{
    private readonly IProductService _productService;

    public LikeCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IResponseWrapper> Handle(LikeCommand request, CancellationToken cancellationToken)
    {
        return await _productService.LikeProductAsync(request.LikeRequest, cancellationToken);
    }
}
