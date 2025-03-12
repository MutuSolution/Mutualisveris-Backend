using Application.Services;
using Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

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
    [MustHavePermission(AppFeature.Products, AppAction.Create)]
    public async Task<IActionResult> AddProductImage(int productId, IFormFile file)
    {
        var response = await _productImageService.AddProductImageAsync(productId, file);
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{productId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductImages(int productId)
    {
        var response = await _productImageService.GetProductImagesAsync(productId);
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    [HttpDelete("delete/{imageId:int}")]
    [MustHavePermission(AppFeature.Products, AppAction.Delete)]

    public async Task<IActionResult> DeleteProductImage(int imageId)
    {
        var response = await _productImageService.DeleteProductImageAsync(imageId);
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }

    [HttpPut("set-main/{imageId:int}")]
    [MustHavePermission(AppFeature.Products, AppAction.Update)]

    public async Task<IActionResult> SetMainImage(int imageId)
    {
        var response = await _productImageService.SetMainImageAsync(imageId);
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }
}
