namespace WebApi.Controllers;

using Application.Features.Products.Commands;
using Application.Features.Products.Queries;
using Common.Authorization;
using Common.Requests.Products;
using Common.Responses.Pagination;
using global::WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class LikesController : MyBaseController<LikesController>
{

    [HttpPost]
    [MustHavePermission(AppFeature.Products, AppAction.Update)]
    public async Task<IActionResult> DoLikeAsync([FromBody] LikeProductRequest request)
    {
        var response = await MediatorSender.Send(new LikeCommand { LikeRequest = request });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }



    [HttpGet("one-user")]
    [MustHavePermission(AppFeature.Products, AppAction.Read)]
    public async Task<IActionResult> GetLikesByUserNameAsync([FromQuery] LikesByUserNameParameters parameters)
    {
        var query = new GetPagedLikesByUserNameQuery { Parameters = parameters };
        var result = await MediatorSender.Send(query);
        if (result.IsSuccessful) return Ok(result);
        return NotFound(result);
    }
}