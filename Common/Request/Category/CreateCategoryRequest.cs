namespace Common.Request.Category;

public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public bool IsVisible { get; set; } = true;
}
