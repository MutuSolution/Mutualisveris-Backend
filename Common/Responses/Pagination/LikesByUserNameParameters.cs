namespace Common.Responses.Pagination;

public class LikesByUserNameParameters : PaginationParams
{
    public string UserName { get; set; }
    public LikesByUserNameParameters()
    {
        OrderBy = "id";
    }
}
