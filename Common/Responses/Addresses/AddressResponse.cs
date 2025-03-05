namespace Common.Responses.Addresses;

public record AddressResponse
(
    int Id,
    string Street,
    string City,
    string Country,
    string ZipCode,
    string Type // Örneğin "Billing" veya "Shipping" olarak string dönebilirsiniz, ya da enum'ı string'e çevirerek
);
