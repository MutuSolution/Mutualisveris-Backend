namespace Domain;

public class ProductReport
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? Message { get; set; }
    public bool IsChecked { get; set; } = false;
}
