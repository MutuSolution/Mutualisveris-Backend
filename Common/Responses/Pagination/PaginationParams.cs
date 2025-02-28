namespace Common.Responses.Pagination;
public class PaginationParams
{
    public int Page { get; set; } = 1; // Varsayılan 1. sayfa
    public int ItemsPerPage { get; set; } = 10; // Varsayılan 10 kayıt
    public int Skip => (Page - 1) * ItemsPerPage;
    public string? SearchTerm { get; set; }
    public string? OrderBy { get; set; }
}
