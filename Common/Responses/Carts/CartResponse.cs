namespace Common.Responses.Carts;

public record CartResponse
(
    int Id,
    DateTime CreatedAt,
    string UserId,
    List<CartItemResponse> Items
);
