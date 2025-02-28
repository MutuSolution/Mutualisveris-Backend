using Application.Services;
using AutoMapper;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries;

public class GetPagedProductsByUserNameQuery : IRequest<IResponseWrapper<PaginationResult<ProductResponse>>>
{
    public ProductsByUserNameParameters Parameters { get; set; } = new ProductsByUserNameParameters();
}

public class GetPagedProductsByUserNameQueryHandler : IRequestHandler<GetPagedProductsByUserNameQuery, IResponseWrapper<PaginationResult<ProductResponse>>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public GetPagedProductsByUserNameQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    async Task<IResponseWrapper<PaginationResult<ProductResponse>>> IRequestHandler<GetPagedProductsByUserNameQuery, IResponseWrapper<PaginationResult<ProductResponse>>>.Handle(GetPagedProductsByUserNameQuery request, CancellationToken cancellationToken)
    {
        if (request.Parameters.UserName == null)
            return await ResponseWrapper<PaginationResult<ProductResponse>>
                .FailAsync("[ML26] User does not exist.");

        var pagedResult = await _productService.GetPagedProductsByUserNameAsync(request.Parameters);
        var mappedItems = _mapper.Map<IEnumerable<ProductResponse>>(pagedResult.Items);

        return await ResponseWrapper<PaginationResult<ProductResponse>>
        .SuccessAsync(new PaginationResult<ProductResponse>(
            mappedItems,
            pagedResult.TotalCount,
            pagedResult.TotalPage,
            pagedResult.Page,
            pagedResult.ItemsPerPage
        ));

    }
}