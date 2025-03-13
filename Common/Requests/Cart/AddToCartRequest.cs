namespace Common.Requests.Cart
{
    public record AddToCartRequest
    {
        public string UserId { get; init; }
        public int ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
