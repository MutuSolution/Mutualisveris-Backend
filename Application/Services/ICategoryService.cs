using Common.Request.Category;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain.Responses;

namespace Application.Services;

public interface ICategoryService
{
    Task<IResponseWrapper<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<IResponseWrapper<CategoryResponse>> UpdateCategoryAsync(UpdateCategoryRequest request);
    Task<IResponseWrapper<CategoryResponse>> SoftDeleteCategory(int id);
    Task<IResponseWrapper<CategoryResponse>> DeleteCategoryAsync(int id);
    Task<IResponseWrapper<CategoryResponse>> GetCategoryByIdAsync(int id);
    Task<IResponseWrapper<PaginationResult<CategoryResponse>>> GetCategoriesAsync(CategoryParameters parameters);

}