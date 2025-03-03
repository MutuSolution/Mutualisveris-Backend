using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;

namespace Application.Services;

public interface ICategoryService
{
    Task<Category> CreateCategoryAsync(Category category);
    Task<CategoryResponse> GetCategoryByIdAsync(int id);
    Task<Category> UpdateCategoryAsync(CategoryResponse category);
    Task<IResponseWrapper> SoftDeleteCategory(int id);
    Task<int> DeleteProductAsync(CategoryResponse category);
    Task<PaginationResult<CategoryResponse>> GetPagedCategoriesAsync(
        CategoryParameters parameters);

}
