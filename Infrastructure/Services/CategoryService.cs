using Application.Services;
using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Request.Category;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
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

        public async Task<IResponseWrapper> CreateCategoryAsync(CreateCategoryRequest request)
        {
            // İstek nesnesinin ve gerekli alanların kontrolü
            if (request == null)
                return await ResponseWrapper<string>.FailAsync("İstek bilgisi boş olamaz.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return await ResponseWrapper<string>.FailAsync("Kategori adı gereklidir.");

            // Parent kategori kontrolü (varsa)
            if (request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value);
                if (parentCategory == null)
                    return await ResponseWrapper<string>.FailAsync("Geçersiz üst kategori.");
            }

            // Aynı isimde kategori var mı kontrolü
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if (existingCategory != null)
                return await ResponseWrapper<string>.FailAsync("[ML63] Kategori zaten var.");

            // Yeni kategori nesnesi oluşturuluyor
            var newCategory = new Category
            {
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId,
                Description = request.Description,
                isVisible = request.IsVisible
            };

            try
            {
                await _context.Categories.AddAsync(newCategory);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    var message = "[ML65] Kategori başarıyla eklendi";
                    return await ResponseWrapper<string>.SuccessAsync(message);
                }
                else
                {
                    return await ResponseWrapper<string>.FailAsync("Kategori kaydedilirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                var message = "Kategori oluşturulurken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<string>.FailAsync(message);
            }
        }

        public async Task<IResponseWrapper> UpdateCategoryAsync(UpdateCategoryRequest request)
        {
            if (request == null)
                return await ResponseWrapper<string>.FailAsync("Güncelleme isteği boş olamaz.");

            // Kategori id üzerinden getirilir
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (categoryInDb == null)
                return await ResponseWrapper<string>.FailAsync("[ML63] Kategori bulunamadı.");

            // Eğer isim değişiyorsa, aynı isimde başka kategori var mı kontrol ediliyor
            if (!string.Equals(categoryInDb.Name, request.Name, StringComparison.OrdinalIgnoreCase))
            {
                var duplicateCategory = await _context.Categories
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
                if (duplicateCategory != null)
                    return await ResponseWrapper<string>.FailAsync("[ML63] Bu isimde başka bir kategori mevcut.");
            }

            // Parent kategori kontrolü (varsa)
            if (request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value);
                if (parentCategory == null)
                    return await ResponseWrapper<string>.FailAsync("Geçersiz üst kategori.");
            }

            // Kategori bilgileri güncelleniyor
            if(string.IsNullOrEmpty(request.Description))
                categoryInDb.Description = null;
            else
                categoryInDb.Description = request.Description;
            categoryInDb.Name = request.Name;
            categoryInDb.ParentCategoryId = request.ParentCategoryId;
            categoryInDb.isVisible = request.IsVisible;

            try
            {
                _context.Categories.Update(categoryInDb);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    var message = "[ML65] Kategori başarıyla güncellendi.";
                    return await ResponseWrapper<string>.SuccessAsync(message);
                }
                else
                {
                    return await ResponseWrapper<string>.FailAsync("Kategori güncellenirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                var message = "Kategori güncellenirken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<string>.FailAsync(message);
            }
        }

        public async Task<IResponseWrapper> DeleteCategoryAsync(int id)
        {
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (categoryInDb == null)
                return await ResponseWrapper<string>.FailAsync("[ML63] Kategori bulunamadı.");

            try
            {
                // Silinecek kategoriye bağlı tüm alt ilişkileri rekurziv olarak kaldırıyoruz.
                await RemoveParentRelationshipRecursive(id);

                // Silme işlemi
                _context.Categories.Remove(categoryInDb);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    var message = "[ML65] Kategori başarıyla silindi.";
                    return await ResponseWrapper<string>.SuccessAsync(message);
                }
                else
                {
                    return await ResponseWrapper<string>.FailAsync("Kategori silinirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                var message = "Kategori silinirken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<string>.FailAsync(message);
            }
        }

        /// <summary>
        /// Verilen parentId'ye sahip olan tüm alt kategorilerin ParentCategoryId değerini null yapar.
        /// Bu metod rekurziv olarak, alt kategorilerinin altındaki kategorileri de günceller.
        /// </summary>
        /// <param name="parentId">Silinecek kategorinin Id değeri</param>
        private async Task RemoveParentRelationshipRecursive(int parentId)
        {
            var childCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync();

            foreach (var child in childCategories)
            {
                // Parent ilişkiyi kaldırıyoruz
                child.ParentCategoryId = null;
                // Altındaki alt kategoriler için de aynı işlemi uyguluyoruz
                await RemoveParentRelationshipRecursive(child.Id);
            }
        }



        public Task<IResponseWrapper> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginationResult<Category>> GetPagedCategoriesAsync(CategoryParameters parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<IResponseWrapper> SoftDeleteCategory(int id)
        {
            // Silinecek kategori veritabanından getiriliyor
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (categoryInDb == null)
                return await ResponseWrapper<string>.FailAsync("[ML63] Kategori bulunamadı.");

            // Ana kategori soft delete ediliyor
            categoryInDb.isVisible = false;

            // Alt kategoriler rekurziv olarak soft delete ediliyor
            await SoftDeleteDescendants(categoryInDb.Id);

            try
            {
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    var message = "[ML65] Kategori başarıyla silindi.";
                    return await ResponseWrapper<string>.SuccessAsync(message);
                }
                else
                {
                    return await ResponseWrapper<string>.FailAsync("Kategori silinirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                var message = "Kategori silinirken bir hata meydana geldi: " + ex.Message;
                return await ResponseWrapper<string>.FailAsync(message);
            }
        }

        private async Task SoftDeleteDescendants(int parentId)
        {
            // Veritabanından parentId'si verilen kategorinin alt kategorileri getiriliyor
            var subCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync();

            foreach (var subCategory in subCategories)
            {
                // Her alt kategori soft delete ediliyor
                subCategory.isVisible = false;
                // Alt kategorilerinin altında da alt kategori varsa, rekurziv olarak devam ediliyor
                await SoftDeleteDescendants(subCategory.Id);
            }
        }

    }
}
