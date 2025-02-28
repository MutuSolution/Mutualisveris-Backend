using Application.Services;
using AutoMapper;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Products.Queries;

public class GetPagedLikesByUserNameQuery : IRequest<IResponseWrapper<PaginationResult<ProductResponse>>>
{
    public LikesByUserNameParameters Parameters { get; set; } = new LikesByUserNameParameters();
}

public class GetPagedLikesByUserNameQueryHandler : IRequestHandler<GetPagedLikesByUserNameQuery, IResponseWrapper<PaginationResult<ProductResponse>>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public GetPagedLikesByUserNameQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    async Task<IResponseWrapper<PaginationResult<ProductResponse>>> IRequestHandler<GetPagedLikesByUserNameQuery, IResponseWrapper<PaginationResult<ProductResponse>>>.Handle(GetPagedLikesByUserNameQuery request, CancellationToken cancellationToken)
    {
        if (request.Parameters.UserName == null)
            return await ResponseWrapper<PaginationResult<ProductResponse>>
                .FailAsync("[ML84] Like does not exist.");

        var pagedResult = await _productService.GetPagedLikesByUserNameAsync(request.Parameters);
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