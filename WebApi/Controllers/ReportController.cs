using Application.Features.Products.Commands;
using Application.Features.Products.Commands.Report;
using Application.Features.Products.Queries.Report;
using Common.Authorization;
using Common.Requests.Product.Report;
using Common.Requests.Products;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;


namespace WebApi.Controllers;

[Route("api/[controller]")]

public class ReportController : MyBaseController<ReportController>
{
    [HttpPost("product")]
    [MustHavePermission(AppFeature.Products, AppAction.Read)]
    public async Task<IActionResult> ProductReport([FromBody] ProductReportRequest request)
    {
        var response = await MediatorSender
            .Send(new ReportProductCommand { ProductReportRequest = request });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("product")]
    [MustHavePermission(AppFeature.Products, AppAction.Read)]
    public async Task<IActionResult> GetProductReportList()
    {
        var response = await MediatorSender.Send(new GetReportProductsQuery());
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpPut("product")]
    [MustHavePermission(AppFeature.Products, AppAction.Update)]
    public async Task<IActionResult> UpdateProductReport([FromBody] ProductReportIsCheckedRequest request)
    {
        var response = await MediatorSender
            .Send(new UpdateReportProductCommand { ReportId = request.ReportId, IsChecked = request.IsChecked });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

}
