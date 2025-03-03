using Common.Requests.Products;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
