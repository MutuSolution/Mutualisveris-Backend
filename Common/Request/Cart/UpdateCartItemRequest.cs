namespace Common.Request.Cart
{
    public record UpdateCartItemRequest
    {
        public string UserId { get; init; }
        public int ProductId { get; init; }
        public int NewQuantity { get; init; }
    }
}
