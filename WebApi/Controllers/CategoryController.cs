using Application.Features.Products.Commands;
using Common.Authorization;
using Common.Request.Category;
using Common.Requests.Products;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class CategoryController : MyBaseController<CategoryController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Categories, AppAction.Create)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        //var response = await MediatorSender
        //    .Send(new CreateProductCommand { CreateProductRequest = createProduct });
        //if (response.IsSuccessful) return Ok(response);
        return BadRequest(/*response*/);
    }
}
