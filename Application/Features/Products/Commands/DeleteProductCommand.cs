namespace Application.Features.Products.Commands;

using Application.Pipelines;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

public class DeleteProductCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public int ProductId { get; set; }
}

public class DeleteProductCommandHandler :
    IRequestHandler<DeleteProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;

    public DeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IResponseWrapper> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.DeleteProductAsync(request.ProductId);
    }
}
