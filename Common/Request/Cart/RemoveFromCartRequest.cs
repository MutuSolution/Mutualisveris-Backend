namespace Common.Request.Cart
{
    public record RemoveFromCartRequest
    {
        public string UserId { get; init; }
        public int ProductId { get; init; }
    }
}
