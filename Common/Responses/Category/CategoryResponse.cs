namespace Domain.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsVisible { get; set; }
        public string ParentCategoryName { get; set; }
        public List<CategoryResponse> SubCategories { get; set; }
            = new List<CategoryResponse>();
        public int ProductCount { get; set; }
    }
}
