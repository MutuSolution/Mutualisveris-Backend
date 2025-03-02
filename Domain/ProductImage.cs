namespace Domain;
public class ProductImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public bool IsMain { get; set; }

    // Relationships
    public int ProductId { get; set; }
    public Product Product { get; set; }
}