using Application.Features.Categories.Commands;
using Common.Authorization;
using Common.Request.Category;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Route("[controller]")]
public class CategoryController : MyBaseController<CategoryController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Categories, AppAction.Create)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var response = await MediatorSender
            .Send(new CreateCategoryCommand { Request = request });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }
}
