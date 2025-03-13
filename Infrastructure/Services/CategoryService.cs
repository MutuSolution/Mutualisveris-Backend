using Application.Services;
using AutoMapper;
using Common.Requests.Category;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        // Eğer ParentCategoryId belirtilmişse, önce veritabanında olup olmadığını kontrol et.
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.ParentCategoryId.Value);

            if (!parentCategoryExists)
                return ResponseWrapper<CategoryResponse>.Fail("Üst kategori bulunamadı.");
        }

        var isDuplicate = await _context.Categories
            .AnyAsync(x => x.Name.ToLower() == request.Name.ToLower());

        if (isDuplicate)
            return ResponseWrapper<CategoryResponse>.Fail("Bu isimde kategori zaten var.");

        var newCategory = _mapper.Map<Category>(request);

        using var transaction = await _context.Database.BeginTransactionAsync();
        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var mappedCategory = _mapper.Map<CategoryResponse>(newCategory);
        return ResponseWrapper<CategoryResponse>.Success(mappedCategory, "Kategori başarıyla oluşturuldu.");
    }

    public async Task<IResponseWrapper<CategoryResponse>> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name))
            return ResponseWrapper<CategoryResponse>.Fail("Güncelleme isteği geçersiz.");

        var category = await _context.Categories.FindAsync(request.Id);
        if (category == null)
            return ResponseWrapper<CategoryResponse>.Fail("Kategori bulunamadı.");

        // Eğer isim değiştiriliyorsa, çakışma kontrolü yap
        if (!category.Name.ToLower().Equals(request.Name.ToLower()))
        {
            var isDuplicate = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower() && c.Id != request.Id);
            if (isDuplicate)
                return ResponseWrapper<CategoryResponse>.Fail("Bu isimde başka bir kategori mevcut.");
        }

        // Üst kategori değişimi kontrolü
        if (request.ParentCategoryId.HasValue && request.ParentCategoryId.Value != category.ParentCategoryId)
        {
            var parentExists = await _context.Categories.AnyAsync(c => c.Id == request.ParentCategoryId.Value);
            if (!parentExists)
                return ResponseWrapper<CategoryResponse>.Fail("Geçersiz üst kategori.");
        }

        _mapper.Map(request, category);
        await _context.SaveChangesAsync();

        var mappedCategory = _mapper.Map<CategoryResponse>(category);
        return ResponseWrapper<CategoryResponse>.Success(mappedCategory, "Kategori başarıyla güncellendi.");
    }


    public async Task<IResponseWrapper<CategoryResponse>> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(x => x.Id == id);

        return category == null
            ? ResponseWrapper<CategoryResponse>.Fail("Kategori bulunamadı.")
            : ResponseWrapper<CategoryResponse>.Success(_mapper.Map<CategoryResponse>(category), "Kategori başarıyla getirildi.");
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

        query = query.OrderBy(x => x.Id);

        var totalCount = await query.CountAsync();
        var totalPage = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / parameters.ItemsPerPage) : 0;

        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.ItemsPerPage)
            .ToListAsync();

        var mappedItems = _mapper.Map<IEnumerable<CategoryResponse>>(items);

        return ResponseWrapper<PaginationResult<CategoryResponse>>.Success(
            new PaginationResult<CategoryResponse>(mappedItems, totalCount, totalPage, parameters.Page, parameters.ItemsPerPage),
            "Kategoriler başarıyla getirildi.");
    }

    public async Task<IResponseWrapper<CategoryResponse>> SoftDeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return ResponseWrapper<CategoryResponse>.Fail("Kategori bulunamadı.");

        category.IsVisible = false;
        await SoftDeleteDescendants(category.Id);
        await _context.SaveChangesAsync();

        return ResponseWrapper<CategoryResponse>.Success(_mapper.Map<CategoryResponse>(category), "Kategori başarıyla gizlendi.");
    }

    private async Task SoftDeleteDescendants(int parentId)
    {
        var subCategories = await _context.Categories
            .Where(c => c.ParentCategoryId == parentId)
            .ToListAsync();

        foreach (var subCategory in subCategories)
        {
            subCategory.IsVisible = false; // Alt kategoriyi de gizle
            await SoftDeleteDescendants(subCategory.Id);
        }
    }


    public async Task<IResponseWrapper<CategoryResponse>> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products) // Kategoriye bağlı ürünleri kontrol et
            .Include(c => c.SubCategories) // Alt kategorileri kontrol et
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
            return ResponseWrapper<CategoryResponse>.Fail("Kategori bulunamadı.");

        // ✅ Eğer bu kategoriye bağlı ürünler varsa, ürünlerin CategoryId'sini NULL yap
        await _context.Products
            .Where(p => p.CategoryId == id)
            .ForEachAsync(p => p.CategoryId = 14);

        // ✅ Eğer bu kategoriye bağlı alt kategoriler varsa, onların ParentCategoryId'sini NULL yap
        await _context.Categories
            .Where(c => c.ParentCategoryId == id)
            .ForEachAsync(c => c.ParentCategoryId = null);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return ResponseWrapper<CategoryResponse>.Success("Kategori başarıyla silindi.");
    }

}
