namespace Common.Request.Category;

public record UpdateCategoryRequest
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int? ParentCategoryId { get; init; }
    public bool IsVisible { get; init; } = true;
}
