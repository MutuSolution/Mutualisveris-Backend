namespace Common.Responses.Pagination;

public class CategoryParameters : PaginationParams
{
    public bool? IsVisible { get; set; }

    public CategoryParameters()
    {
        OrderBy = "id";
    }
}
