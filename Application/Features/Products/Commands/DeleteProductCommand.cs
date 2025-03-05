//using Application.Services;
//using Common.Responses.Wrappers;
//using MediatR;

//namespace Application.Features.Products.Commands;

//public class DeleteProductCommand : IRequest<IResponseWrapper>
//{
//    public int ProductId { get; set; }
//}

//public class DeleteProductCommandHandler :
//    IRequestHandler<DeleteProductCommand, IResponseWrapper>
//{
//    private readonly IProductService _productService;

//    public DeleteProductCommandHandler(IProductService productService)
//    {
//        _productService = productService;
//    }

//    public async Task<IResponseWrapper> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
//    {
//        var productInDb = await _productService.GetProductByIdAsync(request.ProductId);
//        if (productInDb is not null)
//        {
//            var productId = await _productService.DeleteProductAsync(productInDb);
//            return await ResponseWrapper<int>.SuccessAsync(productId, "[ML20] product entry deleted successfully.");
//        }
//        else
//        {
//            return await ResponseWrapper.FailAsync("[ML21] Product does not exist.");
//        }
//    }
//}
