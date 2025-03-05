using Application.Extensions;
using Application.Services;
using Application.Services.Identity;
using AutoMapper;
using Common.Request.Category;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CategoryService(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            if (request == null)
                return await ResponseWrapper<CategoryResponse>.FailAsync("İstek bilgisi boş olamaz.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return await ResponseWrapper<CategoryResponse>.FailAsync("Kategori adı gereklidir.");

            if (request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value);
                if (parentCategory == null)
                    return await ResponseWrapper<CategoryResponse>.FailAsync("Geçersiz üst kategori.");
            }

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if (existingCategory != null)
                return await ResponseWrapper<CategoryResponse>.FailAsync("[ML63] Kategori zaten var.");

            var newCategory = new Category
            {
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId,
                Description = request.Description,
                IsVisible = request.IsVisible
            };

            try
            {
                await _context.Categories.AddAsync(newCategory);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    // Include ile ilişkili veriyi çekiyoruz
                    var createdCategory = await _context.Categories
                        .Include(c => c.ParentCategory)
                        .FirstOrDefaultAsync(c => c.Id == newCategory.Id);
                    var mappedCategory = _mapper.Map<CategoryResponse>(createdCategory);
                    return await ResponseWrapper<CategoryResponse>.SuccessAsync(mappedCategory, "[ML65] Kategori başarıyla eklendi.");
                }
                else
                {
                    return await ResponseWrapper<CategoryResponse>.FailAsync("Kategori kaydedilirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                var message = "Kategori oluşturulurken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<CategoryResponse>.FailAsync(message);
            }
        }

        public async Task<IResponseWrapper<CategoryResponse>> UpdateCategoryAsync(UpdateCategoryRequest request)
        {
            if (request == null)
                return await ResponseWrapper<CategoryResponse>.FailAsync("Güncelleme isteği boş olamaz.");

            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (categoryInDb == null)
                return await ResponseWrapper<CategoryResponse>.FailAsync("[ML63] Kategori bulunamadı.");

            if (!string.Equals(categoryInDb.Name, request.Name, StringComparison.OrdinalIgnoreCase))
            {
                var duplicateCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
                if (duplicateCategory != null)
                    return await ResponseWrapper<CategoryResponse>.FailAsync("[ML63] Bu isimde başka bir kategori mevcut.");
            }

            if (request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value);
                if (parentCategory == null)
                    return await ResponseWrapper<CategoryResponse>.FailAsync("Geçersiz üst kategori.");
            }

            // Güncelleme işlemi
            categoryInDb.Name = request.Name;
            categoryInDb.ParentCategoryId = request.ParentCategoryId;
            categoryInDb.Description = string.IsNullOrEmpty(request.Description) ? null : request.Description;
            categoryInDb.IsVisible = request.IsVisible;

            try
            {
                _context.Categories.Update(categoryInDb);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    var updatedCategory = await _context.Categories
                        .Include(c => c.ParentCategory)
                        .FirstOrDefaultAsync(c => c.Id == categoryInDb.Id);
                    var mappedCategory = _mapper.Map<CategoryResponse>(updatedCategory);
                    return await ResponseWrapper<CategoryResponse>.SuccessAsync(mappedCategory, "[ML65] Kategori başarıyla güncellendi.");
                }
                else
                {
                    return await ResponseWrapper<CategoryResponse>.FailAsync("Kategori güncellenirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                var message = "Kategori güncellenirken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<CategoryResponse>.FailAsync(message);
            }
        }

        public async Task<IResponseWrapper<CategoryResponse>> DeleteCategoryAsync(int id)
        {
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (categoryInDb == null)
                return await ResponseWrapper<CategoryResponse>.FailAsync("[ML63] Kategori bulunamadı.");

            try
            {
                await RemoveParentRelationshipRecursive(id);

                // Mapleme için mevcut bilgiyi alıyoruz
                var mappedCategory = _mapper.Map<CategoryResponse>(categoryInDb);

                _context.Categories.Remove(categoryInDb);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                    return await ResponseWrapper<CategoryResponse>.SuccessAsync(mappedCategory, "[ML65] Kategori başarıyla silindi.");
                else
                    return await ResponseWrapper<CategoryResponse>.FailAsync("Kategori silinirken bir hata oluştu.");
            }
            catch (Exception ex)
            {
                var message = "Kategori silinirken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<CategoryResponse>.FailAsync(message);
            }
        }

        private async Task RemoveParentRelationshipRecursive(int parentId)
        {
            var childCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync();

            foreach (var child in childCategories)
            {
                child.ParentCategoryId = null;
                await RemoveParentRelationshipRecursive(child.Id);
            }
        }

        public async Task<IResponseWrapper<CategoryResponse>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var categoryInDb = await _context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.SubCategories)
                        .ThenInclude(sc => sc.SubCategories)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (categoryInDb == null)
                    return await ResponseWrapper<CategoryResponse>.FailAsync("[ML63] Kategori bulunamadı.");

                var mappedCategory = _mapper.Map<CategoryResponse>(categoryInDb);
                return await ResponseWrapper<CategoryResponse>.SuccessAsync(mappedCategory, "[ML65] Kategori başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return await ResponseWrapper<CategoryResponse>
                    .FailAsync("Kategori getirilirken bir hata meydana geldi: " + ex.Message);
            }
        }

        public async Task<IResponseWrapper<PaginationResult<CategoryResponse>>> GetCategoriesAsync(CategoryParameters parameters)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(searchTerm) ||
                    x.Description.ToLower().Contains(searchTerm));
            }

            if (parameters.IsVisible.HasValue)
                query = query.Where(x => x.IsVisible == parameters.IsVisible.Value);

            query = query.SortById(parameters.OrderBy);

            var totalCount = await query.CountAsync();
            var totalPage = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / parameters.ItemsPerPage) : 0;

            var items = await query
                .Skip(parameters.Skip)
                .Take(parameters.ItemsPerPage)
                .ToListAsync();

            var mappedItems = _mapper.Map<IEnumerable<CategoryResponse>>(items);

            var paginationResult = new PaginationResult<CategoryResponse>(
                mappedItems,
                totalCount,
                totalPage,
                parameters.Page,
                parameters.ItemsPerPage);

            return ResponseWrapper<PaginationResult<CategoryResponse>>.Success(paginationResult, "[ML65] Kategoriler başarıyla getirildi.");
        }

        public async Task<IResponseWrapper<CategoryResponse>> SoftDeleteCategory(int id)
        {
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (categoryInDb == null)
                return await ResponseWrapper<CategoryResponse>.FailAsync("[ML63] Kategori bulunamadı.");

            categoryInDb.IsVisible = false;
            await SoftDeleteDescendants(categoryInDb.Id);

            try
            {
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult > 0)
                {
                    var updatedCategory = await _context.Categories
                        .Include(c => c.ParentCategory)
                        .FirstOrDefaultAsync(c => c.Id == categoryInDb.Id);
                    var mappedCategory = _mapper.Map<CategoryResponse>(updatedCategory);
                    return await ResponseWrapper<CategoryResponse>.SuccessAsync(mappedCategory, "[ML65] Kategori başarıyla silindi.");
                }
                else
                {
                    return await ResponseWrapper<CategoryResponse>.FailAsync("Kategori silinirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                var message = "Kategori silinirken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<CategoryResponse>.FailAsync(message);
            }
        }

        private async Task SoftDeleteDescendants(int parentId)
        {
            var subCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync();

            foreach (var subCategory in subCategories)
            {
                subCategory.IsVisible = false;
                await SoftDeleteDescendants(subCategory.Id);
            }
        }
    }
}
