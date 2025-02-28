namespace Common.Requests.Products;

public class UpdateProductRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string UserName { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsDeleted { get; set; }
    public int LikeCount { get; set; }
}
