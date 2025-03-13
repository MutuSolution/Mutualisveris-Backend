using Application.Features.Address.Commands;
using Application.Features.Address.Queries;
using Common.Authorization;
using Common.Requests.Addresses;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    // Adres işlemleri
    [Route("address")]
    public class AddressController : MyBaseController<AddressController>
    {

        [HttpPost("add")]
        [MustHavePermission(AppFeature.Addresses, AppAction.Create)]
        public async Task<IActionResult> AddAddress([FromBody] CreateAddressRequest request)
        {
            var response = await MediatorSender.Send(new AddAddressCommand { Request = request });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpPut("update")]
        [MustHavePermission(AppFeature.Addresses, AppAction.Update)]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request)
        {
            var response = await MediatorSender.Send(new UpdateAddressCommand { Request = request });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("remove/{addressId:int}")]
        [MustHavePermission(AppFeature.Addresses, AppAction.Delete)]
        public async Task<IActionResult> RemoveAddress(int addressId)
        {
            var response = await MediatorSender.Send(new RemoveAddressCommand { AddressId = addressId });
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{addressId:int}")]
        [MustHavePermission(AppFeature.Addresses, AppAction.Read)]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            var response = await MediatorSender.Send(new GetAddressByIdQuery { AddressId = addressId });
            return response.IsSuccessful ? Ok(response) : NotFound(response);
        }

        [HttpGet("user/{userId}")]
        [MustHavePermission(AppFeature.Addresses, AppAction.Read)]
        public async Task<IActionResult> GetUserAddresses(string userId)
        {
            var response = await MediatorSender.Send(new GetUserAddressesQuery { UserId = userId });
            return response.IsSuccessful ? Ok(response) : NotFound(response);
        }
    }
}
