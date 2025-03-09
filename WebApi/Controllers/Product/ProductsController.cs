using Application.Features.Products.Commands;
using Application.Features.Products.Queries;
using Common.Authorization;
using Common.Requests.Products;
using Common.Responses.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Product;

[Route("api/products")]
[ApiController]
public class ProductsController : MyBaseController<ProductsController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Products, AppAction.Create)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest createProduct)
    {
        var response = await MediatorSender.Send(new CreateProductCommand { CreateProduct = createProduct });
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    [MustHavePermission(AppFeature.Products, AppAction.Update)]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest updateProduct)
    {
        var response = await MediatorSender.Send(new UpdateProductCommand { UpdateProduct = updateProduct });
        return response.IsSuccessful ? Ok(response) : BadRequest(response);
    }

    /// ✅ **Ürün detayını ID'ye göre getirme**
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(int id)
    {
        var response = await MediatorSender.Send(new GetProductByIdQuery { ProductId = id });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    /// ✅ **Tüm ürünleri listeleme (pagination destekli)**
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts([FromQuery] ProductParameters parameters)
    {
        var response = await MediatorSender.Send(new GetProductsQuery { Parameters = parameters });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    /// ✅ **Ürünü Soft Delete ile gizleme**
    [HttpPut("soft-delete/{id:int}")]
    [MustHavePermission(AppFeature.Products, AppAction.Update)]
    public async Task<IActionResult> SoftDeleteProduct(int id)
    {
        var response = await MediatorSender.Send(new SoftDeleteProductCommand { ProductId = id });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    /// ✅ **Ürünü tamamen silme**
    [HttpPut("hard-delete/{id:int}")]
    [MustHavePermission(AppFeature.Products, AppAction.Delete)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var response = await MediatorSender.Send(new DeleteProductCommand { ProductId = id });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }
}
