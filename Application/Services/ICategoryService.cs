using Common.Request.Category;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;

namespace Application.Services;

public interface ICategoryService
{
    Task<IResponseWrapper> CreateCategoryAsync(CreateCategoryRequest request);
    Task<IResponseWrapper> GetCategoryByIdAsync(int id);
    Task<IResponseWrapper> UpdateCategoryAsync(UpdateCategoryRequest request);
    Task<IResponseWrapper> SoftDeleteCategory(int id);
    Task<IResponseWrapper> DeleteCategoryAsync(int id);
    Task<PaginationResult<Category>> GetPagedCategoriesAsync(CategoryParameters parameters);

}
