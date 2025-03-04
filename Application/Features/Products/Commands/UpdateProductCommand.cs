using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Requests.Products;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Commands;

public class UpdateProductCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public UpdateProductRequest UpdateProductRequest { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var productInDb = await _productService
            .GetProductByIdAsync(request.UpdateProductRequest.Id);
        if (productInDb is not null)
        {
            var updatedProduct = new UpdateProductRequest
            {
                Id = productInDb.Id,
                Title = request.UpdateProductRequest.Title,
                Url = request.UpdateProductRequest.Url,
                UserName = request.UpdateProductRequest.UserName,
                Description = request.UpdateProductRequest.Description,
                IsPublic = request.UpdateProductRequest.IsPublic,
                IsDeleted = request.UpdateProductRequest.IsDeleted,
                LikeCount = request.UpdateProductRequest.LikeCount
            };

            var result = await _productService.UpdateProductAsync(updatedProduct);
            var mappedProduct = _mapper.Map<ProductResponse>(result);

            return await ResponseWrapper<ProductResponse>
                .SuccessAsync(mappedProduct, "[ML22] Product updated successfully");
        }
        return await ResponseWrapper<ProductResponse>
            .FailAsync("[ML23] Product does not exist.");
    }
}
