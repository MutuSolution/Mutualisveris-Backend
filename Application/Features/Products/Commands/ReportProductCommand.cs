using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Requests.Products;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Products.Commands;

public class ReportProductCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public ProductReportRequest ProductReportRequest { get; set; }
}
public class ReportProductCommandHandler :
    IRequestHandler<ReportProductCommand, IResponseWrapper>
{

    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ReportProductCommandHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    public async Task<IResponseWrapper> Handle(ReportProductCommand request, CancellationToken cancellationToken)
    {
        var mappedProductReport = _mapper.Map<ProductReport>(request.ProductReportRequest);
        var newProductReport = await _productService.ReportProductAsync(mappedProductReport);
        if (newProductReport.Id > 0)
        {
            var mappedNewProductReport = _mapper.Map<ProductReportResponse>(newProductReport);
            return await ResponseWrapper<ProductReportResponse>
                .SuccessAsync(mappedNewProductReport, "[ML111] Product report created successfully.");
        }
        return await ResponseWrapper.FailAsync("[ML112] Failed to create product report entry.");
    }
}