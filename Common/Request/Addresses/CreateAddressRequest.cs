using Domain;
using Domain.Enums;

namespace Common.Requests.Addresses;

public record CreateAddressRequest
{
    public string UserId { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string Country { get; init; }
    public string ZipCode { get; init; }
    public AddressType Type { get; init; }
}
