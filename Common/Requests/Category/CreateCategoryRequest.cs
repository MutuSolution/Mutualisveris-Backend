namespace Common.Requests.Category;

public record CreateCategoryRequest
{
    public string Name { get; init; }
    public string? Description { get; init; }  // Boş olabilir, null olmasına izin ver
    public int Level { get; init; }  // Boş olabilir, null olmasına izin ver
    public int? ParentCategoryId { get; init; }
    public bool IsVisible { get; init; } = true;
}
