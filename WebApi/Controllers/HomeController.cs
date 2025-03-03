using Application.Features.Products.Queries.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("[controller]")]
public class HomeController : MyBaseController<HomeController>
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetHomeProductListAsync()
    {
        var response = await MediatorSender.Send(new GetHomeProductQuery());
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpGet("username/{userName:required}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProductWithUsernameListAsync(string userName)
    {
        var response = await MediatorSender.Send(new GetPublicProductWithUsernameQuery { UserName = userName });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }
}
