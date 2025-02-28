namespace Common.Responses.Pagination;

public class ProductsByUserNameParameters : PaginationParams
{
    public int MinLikeCount { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public string UserName { get; set; }
    public string IsPublic { get; set; }
    public ProductsByUserNameParameters()
    {
        OrderBy = "id";
    }
}