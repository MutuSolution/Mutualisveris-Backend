using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Requests.Products;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public CreateProductRequest CreateProductRequest { get; set; }
}

public class CreateProductCommandHandler :
    IRequestHandler<CreateProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var mappedProduct = _mapper.Map<Product>(request.CreateProductRequest);
        var newProduct = await _productService.CreateProductAsync(mappedProduct);
        if (newProduct.Id > 0)
        {
            var mappedNewProduct = _mapper.Map<ProductResponse>(newProduct);
            return await ResponseWrapper<ProductResponse>
                .SuccessAsync(mappedNewProduct, "[ML18] Product created successfully.");
        }
        return await ResponseWrapper.FailAsync("[ML19] Failed to create product entry.");
    }
}