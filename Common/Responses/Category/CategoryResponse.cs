namespace Domain.Responses;

public record CategoryResponse
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int Level { get; init; }
    public int? ParentCategoryId { get; init; }
    public bool IsVisible { get; init; }

    public string ParentCategoryName { get; init; } = string.Empty; 
    // ❗ Eğer ParentCategory varsa isim al, yoksa boş string ata
    public List<CategoryResponse> SubCategories { get; init; } = new();
    public int? ProductCount { get; init; }  // Nullable olabilir
}
