namespace Common.Responses.Pagination;

public class ProductParameters : PaginationParams
{
    public int? CategoryId { get; set; }
    public bool? IsPublic { get; set; }

    public ProductParameters()
    {
        OrderBy = "id";
    }
}