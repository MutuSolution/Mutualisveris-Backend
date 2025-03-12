using Common.Requests.Addresses;
using Common.Responses.Addresses;
using Common.Responses.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services;

public interface IAddressService
{
    Task<IResponseWrapper<AddressResponse>> AddAddressAsync(CreateAddressRequest request);
    Task<IResponseWrapper<AddressResponse>> UpdateAddressAsync(UpdateAddressRequest request);
    Task<IResponseWrapper<bool>> DeleteAddressAsync(int addressId);
    Task<IResponseWrapper<AddressResponse>> GetAddressByIdAsync(int addressId);
    Task<IResponseWrapper<List<AddressResponse>>> GetUserAddressesAsync(string userId);
}
