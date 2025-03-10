using Application.Services;
using Microsoft.AspNetCore.Mvc;

[Route("product-image")]
[ApiController]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _productImageService;

    public ProductImagesController(IProductImageService productImageService)
    {
        _productImageService = productImageService;
    }

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> AddProductImage(int productId, IFormFile file)
    {
        var response = await _productImageService.AddProductImageAsync(productId, file);
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProductImages(int productId)
    {
        var response = await _productImageService.GetProductImagesAsync(productId);
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    [HttpPut("delete/{imageId:int}")]
    public async Task<IActionResult> DeleteProductImage(int imageId)
    {
        var response = await _productImageService.DeleteProductImageAsync(imageId);
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }

    [HttpPut("set-main/{imageId:int}")]
    public async Task<IActionResult> SetMainImage(int imageId)
    {
        var response = await _productImageService.SetMainImageAsync(imageId);
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }
}
