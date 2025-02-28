using Application.Services;
using AutoMapper;
using Common.Requests.Product.Report;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Commands.Report;
public class UpdateReportProductCommand : IRequest<IResponseWrapper>
{
    public int ReportId { get; set; }
    public bool IsChecked { get; set; }
}
public class UpdateReportProductCommandHandler : IRequestHandler<UpdateReportProductCommand, IResponseWrapper>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    public UpdateReportProductCommandHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    public async Task<IResponseWrapper> Handle(UpdateReportProductCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new ProductReportIsCheckedRequest
        {
            ReportId = request.ReportId,
            IsChecked = request.IsChecked
        };
        var updatedProduct = await _productService.UpdateReportProductAsync(updateRequest);
        if (updatedProduct.ProductId > 0)
        {
            var mappedUpdatedProduct = _mapper.Map<ProductReportResponse>(updatedProduct);
            return await ResponseWrapper<ProductReportResponse>
                .SuccessAsync(mappedUpdatedProduct, "[ML114] Product report updated successfully.");
        }
        return await ResponseWrapper
            .FailAsync("[ML115] Failed to update product report entry.");
    }
}
