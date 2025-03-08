using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Requests.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Commands;

public class UpdateProductCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public UpdateProductRequest UpdateProduct { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;
    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IResponseWrapper> Handle(UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        return await _productService.UpdateProductAsync(request.UpdateProduct);
    }
}
