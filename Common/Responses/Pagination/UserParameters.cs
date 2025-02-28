namespace Common.Responses.Pagination;

public class UserParameters : PaginationParams
{
    public bool? IsActive { get; set; }

    public UserParameters()
    {
        OrderBy = "UserName";
    }
}
