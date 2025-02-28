using Common.Requests.Product.Report;
using Common.Requests.Products;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;

namespace Application.Services;

public interface IProductService
{

    Task<Product> CreateProductAsync(Product product);
    Task<List<ProductResponse>> GetProductListAsync();
    Task<List<Product>> GetHomeProductListAsync();
    Task<List<Product>> GetPublicProductWithUsernameAsync(string userName);
    Task<ProductResponse> GetProductByIdAsync(int id);
    Task<Product> UpdateProductAsync(ProductResponse product);
    Task<int> DeleteProductAsync(ProductResponse product);
    Task<IResponseWrapper> SoftDeleteProduct(SoftDeleteProductRequest request);
    Task<IResponseWrapper> LikeProductAsync(LikeProductRequest request, CancellationToken cancellationToken);
    Task<PaginationResult<ProductResponse>> GetPagedProductsAsync(ProductParameters parameters);
    Task<PaginationResult<ProductResponse>> GetPagedProductsByUserNameAsync(ProductsByUserNameParameters parameters);
    Task<PaginationResult<ProductResponse>> GetPagedLikesByUserNameAsync(LikesByUserNameParameters parameters);

    //report
    Task<ProductReport> ReportProductAsync(ProductReport productReport);
    Task<List<ProductReportResponse>> GetProductReportsAsync();
    Task<ProductReportResponse> UpdateReportProductAsync(ProductReportIsCheckedRequest request);


}
