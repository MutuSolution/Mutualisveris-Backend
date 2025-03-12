using Domain.Enums;

namespace Common.Responses.Addresses;

public record AddressResponse
{
    public int Id { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string Country { get; init; }
    public string ZipCode { get; init; }
    public AddressType Type { get; init; }
    public string PhoneNumber { get; init; }
}
