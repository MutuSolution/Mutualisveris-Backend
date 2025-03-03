namespace Common.Responses.Pagination;

public class UserParameters : PaginationParams
{
    public bool? IsActive { get; set; } = true;

    public UserParameters()
    {
        OrderBy = "UserName";
    }
}
