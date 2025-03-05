using Application.Features.Categories.Commands;
using Application.Features.Categories.Queries;
using Application.Features.Identity.Users.Queries;
using Application.Features.Products.Commands;
using Common.Authorization;
using Common.Request.Category;
using Common.Requests.Products;
using Common.Responses.Pagination;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPut("hard-delete")]
    [MustHavePermission(AppFeature.Categories, AppAction.Delete)]
    public async Task<IActionResult> HardDeleteCategoryAsync(int id)
    {
        var response = await MediatorSender
           .Send(new DeleteCategoryCommand { CategoryId = id });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpPut("soft-delete")]
    [MustHavePermission(AppFeature.Categories, AppAction.Delete)]
    public async Task<IActionResult> SoftDeleteCategoryAsync(int id)
    {
        var response = await MediatorSender
           .Send(new SoftDeleteCategoryCommand { CategoryId = id });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("id/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryByIdAsync(int id)
    {
        var response = await MediatorSender
           .Send(new GetCategoryByIdQuery { CategoryID = id });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoriesAsync([FromQuery] CategoryParameters parameters)
    {
        var query = new GetCategoriesQuery { Parameters = parameters };
        var result = await MediatorSender.Send(query);
        if (result.IsSuccessful) return Ok(result);
        return NotFound(result);
    }
}
