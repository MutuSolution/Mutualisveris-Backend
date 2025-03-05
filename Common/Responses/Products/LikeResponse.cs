namespace Common.Responses.Products;
public record LikeResponse
(
     string LikeId,
     string ProductId,
     bool IsLiked
);
