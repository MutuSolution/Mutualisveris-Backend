using Application.Features.Categories.Commands;
using Application.Features.Categories.Queries;
using Common.Authorization;
using Common.Request.Category;
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
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest createCategory)
    {
        var response = await MediatorSender
            .Send(new CreateCategoryCommand { CreateCategory = createCategory });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    [HttpPut]
    [MustHavePermission(AppFeature.Categories, AppAction.Update)]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest updateCategory)
    {
        var response = await MediatorSender
            .Send(new UpdateCategoryCommand { UpdateCategory = updateCategory });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    [HttpPut("hard-delete")]
    [MustHavePermission(AppFeature.Categories, AppAction.Delete)]
    public async Task<IActionResult> HardDeleteCategory(int id)
    {
        var response = await MediatorSender
           .Send(new DeleteCategoryCommand { CategoryId = id });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    [HttpPut("soft-delete")]
    [MustHavePermission(AppFeature.Categories, AppAction.Delete)]
    public async Task<IActionResult> SoftDeleteCategory(int id)
    {
        var response = await MediatorSender
           .Send(new SoftDeleteCategoryCommand { CategoryId = id });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    [HttpGet("id/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var response = await MediatorSender
           .Send(new GetCategoryByIdQuery { CategoryID = id });
        return response.IsSuccessful ? Ok(response) : NotFound(response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryParameters parameters)
    {
        var query = new GetCategoriesQuery { Parameters = parameters };
        var result = await MediatorSender.Send(query);
              return result.IsSuccessful ? Ok(result) : NotFound(result);
    }
}
