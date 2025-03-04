using Application.Features.Categories.Commands;
using Application.Features.Products.Commands;
using Common.Authorization;
using Common.Request.Category;
using Common.Requests.Products;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Route("[controller]")]
public class CategoryController : MyBaseController<CategoryController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Categories, AppAction.Create)]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest createCategory)
    {
        var response = await MediatorSender
            .Send(new CreateCategoryCommand { CreateCategory = createCategory });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpPut]
    [MustHavePermission(AppFeature.Categories, AppAction.Update)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryRequest updateCategory)
    {
        var response = await MediatorSender
            .Send(new UpdateCategoryCommand { UpdateCategory = updateCategory });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

}
