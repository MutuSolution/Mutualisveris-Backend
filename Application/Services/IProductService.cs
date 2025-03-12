using Common.Requests.Products;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;

namespace Application.Services;

public interface IProductService
{
    Task<IResponseWrapper<ProductResponse>> CreateProductAsync(CreateProductRequest request);
    Task<IResponseWrapper<ProductResponse>> UpdateProductAsync(UpdateProductRequest request);
    Task<IResponseWrapper<ProductResponse>> GetProductByIdAsync(int id);
    Task<IResponseWrapper<ProductResponse>> SoftDeleteProductAsync(int id);
    Task<IResponseWrapper<ProductResponse>> DeleteProductAsync(int id);
    Task<IResponseWrapper<PaginationResult<ProductResponse>>> GetProductsAsync(ProductParameters parameters);
}
