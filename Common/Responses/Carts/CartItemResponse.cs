namespace Common.Responses.Carts;

public record CartItemResponse
(
    int Id,
    int Quantity,
    int ProductId,
    string ProductName // Ürünün adını eklemek faydalı olabilir
);
