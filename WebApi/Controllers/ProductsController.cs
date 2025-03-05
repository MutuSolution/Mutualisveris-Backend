using Application.Features.Products.Commands;
using Common.Authorization;
using Common.Requests.Products;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Route("[controller]")]
public class ProductsController : MyBaseController<ProductsController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Products, AppAction.Create)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest createProduct)
    {
        var response = await MediatorSender
            .Send(new CreateProductCommand { CreateProduct = createProduct });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    //[HttpPut]
    //[MustHavePermission(AppFeature.Products, AppAction.Update)]
    //public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest updateProduct)
    //{
    //    var response = await MediatorSender
    //        .Send(new UpdateProductCommand { UpdateProductRequest = updateProduct });
    //    if (response.IsSuccessful) return Ok(response);
    //    return BadRequest(response);
    //}



    //[HttpGet("{productId:int}")]
    //[MustHavePermission(AppFeature.Products, AppAction.Read)]
    //public async Task<IActionResult> GetProductById(int productId)
    //{
    //    var response = await MediatorSender.Send(new GetProductByIdQuery { ProductId = productId });
    //    if (response.IsSuccessful) return Ok(response);
    //    return NotFound(response);
    //}

    //[HttpGet("all")]
    //[MustHavePermission(AppFeature.Products, AppAction.Read)]
    //public async Task<IActionResult> GetProductsAll([FromQuery] ProductParameters parameters)
    //{
    //    var query = new GetPagedProductsQuery { Parameters = parameters };
    //    var result = await MediatorSender.Send(query);
    //    if (result.IsSuccessful) return Ok(result);
    //    return NotFound(result);
    //}

    //[HttpPut("soft-delete")]
    //[MustHavePermission(AppFeature.Products, AppAction.Update)]
    //public async Task<IActionResult> SoftDelete([FromBody] SoftDeleteProductRequest request)
    //{
    //    var response = await MediatorSender.Send(new SoftDeleteProductCommand { SoftDeleteProductRequest = request });
    //    if (response.IsSuccessful) return Ok(response);
    //    return NotFound(response);
    //}

    //[HttpDelete("{productId:int}")]
    //[MustHavePermission(AppFeature.Products, AppAction.Delete)]
    //public async Task<IActionResult> DeleteProduct(int productId)
    //{
    //    var response = await MediatorSender
    //        .Send(new DeleteProductCommand { ProductId = productId });
    //    if (response.IsSuccessful) return Ok(response);
    //    return NotFound(response);
    //}
}