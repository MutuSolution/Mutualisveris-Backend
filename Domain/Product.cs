namespace Domain;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string UserName { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int LikeCount { get; set; } = 0;

    // Navigation Property
    public ICollection<Like> Likes { get; set; }
}
