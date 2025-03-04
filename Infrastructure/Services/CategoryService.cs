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
            categoryInDb.Name = request.Name;
            categoryInDb.Description = request.Description;
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

        public Task<IResponseWrapper> DeleteCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IResponseWrapper> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginationResult<Category>> GetPagedCategoriesAsync(CategoryParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IResponseWrapper> SoftDeleteCategory(int id)
        {
            throw new NotImplementedException();
        }
    }
}
