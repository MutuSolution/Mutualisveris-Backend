//using Application.Services;
//using AutoMapper;
//using Common.Responses.Pagination;
//using Common.Responses.Products;
//using Common.Responses.Wrappers;
//using MediatR;

//namespace Application.Features.Products.Queries;

//public class GetPagedProductsQuery : IRequest<IResponseWrapper<PaginationResult<ProductResponse>>>
//{
//    public ProductParameters Parameters { get; set; } = new ProductParameters();
//}

//public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, IResponseWrapper<PaginationResult<ProductResponse>>>
//{
//    private readonly IProductService _productService;
//    private readonly IMapper _mapper;

//    public GetPagedProductsQueryHandler(IProductService productService, IMapper mapper)
//    {
//        _productService = productService;
//        _mapper = mapper;
//    }

//    async Task<IResponseWrapper<PaginationResult<ProductResponse>>> IRequestHandler<GetPagedProductsQuery, IResponseWrapper<PaginationResult<ProductResponse>>>.Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
//    {
//        var pagedResult = await _productService.GetPagedProductsAsync(request.Parameters);
//        var mappedItems = _mapper.Map<IEnumerable<ProductResponse>>(pagedResult.Items);
//        return await ResponseWrapper<PaginationResult<ProductResponse>>
//        .SuccessAsync(new PaginationResult<ProductResponse>(
//            mappedItems,
//            pagedResult.TotalCount,
//            pagedResult.TotalPage,
//            pagedResult.Page,
//            pagedResult.ItemsPerPage
//        ));

//    }
//}
