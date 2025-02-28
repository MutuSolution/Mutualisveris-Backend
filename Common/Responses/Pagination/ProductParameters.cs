namespace Common.Responses.Pagination;

public class ProductParameters : PaginationParams
{
    public int MinLikeCount { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public bool? IsPublic { get; set; }

    public ProductParameters()
    {
        OrderBy = "id";
    }
}