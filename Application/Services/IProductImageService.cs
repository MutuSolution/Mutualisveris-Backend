using Common.Responses.Products;
using Common.Responses.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IProductImageService
{
    Task<IResponseWrapper<ProductImageResponse>> AddProductImageAsync(int productId, IFormFile formFile);
    Task<IResponseWrapper<IEnumerable<ProductImageResponse>>> GetProductImagesAsync(int productId);
    Task<IResponseWrapper<string>> DeleteProductImageAsync(int imageId);
    Task<IResponseWrapper<ProductImageResponse>> SetMainImageAsync(int imageId);
}
