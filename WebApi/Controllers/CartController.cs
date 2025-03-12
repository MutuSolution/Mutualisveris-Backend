using Application.Features.Cart.Commands;
using Application.Features.Cart.Queries;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Common.Request.Cart;
using Common.Authorization;
using WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    //sepet
    [Route("cart")]
    public class CartController : MyBaseController<CartController>
    {
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var response = await MediatorSender.Send(new AddToCartCommand { Request = request });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpPost("update")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            var response = await MediatorSender.Send(new UpdateCartItemCommand { Request = request });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpPost("remove")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            var response = await MediatorSender.Send(new RemoveFromCartCommand { Request = request });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCart(string userId)
        {
            var response = await MediatorSender.Send(new GetCartQuery { UserId = userId });
            return response.IsSuccessful ? Ok(response) : NotFound(response);
        }

        [HttpGet("{userId}/item/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCartItem(string userId, int productId)
        {
            var response = await MediatorSender.Send(new GetCartItemQuery { UserId = userId, ProductId = productId });
            return response.IsSuccessful ? Ok(response) : NotFound(response);
        }
    }
}
