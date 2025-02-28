namespace Domain;

public class Like
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Property
    public Product Products { get; set; }
}

