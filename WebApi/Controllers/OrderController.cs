using Application.Features.Orders.Commands;
using Application.Features.Orders.Queries;
using Common.Authorization;
using Common.Requests.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    // ✅ Sipariş İşlemleri
    [Route("order")]
    public class OrderController : MyBaseController<OrderController>
    {
        // ✅ 🛍 Yeni Sipariş Oluştur
        [HttpPost("create")]
        [MustHavePermission(AppFeature.Orders, AppAction.Create)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var response = await MediatorSender.Send(new CreateOrderCommand { Request = request });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        // ✅ 📦 Sipariş Güncelleme
        [HttpPut("update")]
        [MustHavePermission(AppFeature.Orders, AppAction.Update)]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest request)
        {
            var response = await MediatorSender.Send(new UpdateOrderCommand { Request = request });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        // ✅ ❌ Sipariş Silme
        [HttpDelete("remove/{orderId:int}")]
        [MustHavePermission(AppFeature.Orders, AppAction.Delete)]
        public async Task<IActionResult> RemoveOrder(int orderId)
        {
            var response = await MediatorSender.Send(new RemoveOrderCommand { OrderId = orderId });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        // ✅ 📌 Belirli Bir Siparişi Getir
        [HttpGet("{orderId:int}")]
        [MustHavePermission(AppFeature.Orders, AppAction.Read)]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var response = await MediatorSender.Send(new GetOrderByIdQuery { OrderId = orderId });
            return response.IsSuccessful ? Ok(response) : NotFound(response);
        }

        // ✅ 🔄 Kullanıcının Tüm Siparişlerini Getir
        [HttpGet("user/{userId}")]
        [MustHavePermission(AppFeature.Orders, AppAction.Read)]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            var response = await MediatorSender.Send(new GetUserOrdersQuery { UserId = userId });
            return response.IsSuccessful ? Ok(response) : NotFound(response);
        }

        // ✅ 📊 Tüm Siparişleri Getir (Admin)
        [HttpGet]
        [MustHavePermission(AppFeature.Orders, AppAction.Read)]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await MediatorSender.Send(new GetAllOrdersQuery());
            return response.IsSuccessful ? Ok(response) : NotFound(response);
        }
    }
}
