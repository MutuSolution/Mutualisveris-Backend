using Application.Services;
using AutoMapper;
using Common.Requests.Products;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// ✅ **Ürün Ekleme**
    public async Task<IResponseWrapper<ProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        // 1️⃣ Kategori kontrolü
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            return ResponseWrapper<ProductResponse>.Fail("Geçersiz kategori.");

        // 2️⃣ Aynı SKU var mı kontrolü
        var skuExists = await _context.Products.AnyAsync(p => p.SKU == request.SKU);
        if (skuExists)
            return ResponseWrapper<ProductResponse>.Fail("Bu SKU zaten kullanılıyor.");

        // 3️⃣ Yeni ürün oluştur
        var newProduct = _mapper.Map<Product>(request);
        newProduct.CreatedAt = DateTime.UtcNow;

        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();

        return ResponseWrapper<ProductResponse>.Success(_mapper.Map<ProductResponse>(newProduct), "Ürün başarıyla eklendi.");
    }

    /// ✅ **Ürün Güncelleme**
    public async Task<IResponseWrapper<ProductResponse>> UpdateProductAsync(UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null)
            return ResponseWrapper<ProductResponse>.Fail("Ürün bulunamadı.");

        // 1️⃣ Aynı SKU başka bir ürüne ait mi?
        var skuExists = await _context.Products
            .AnyAsync(p => p.SKU == request.SKU && p.Id != request.Id);
        if (skuExists)
            return ResponseWrapper<ProductResponse>.Fail("Bu SKU başka bir ürüne ait.");

        // 2️⃣ Kategori kontrolü
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            return ResponseWrapper<ProductResponse>.Fail("Geçersiz kategori.");

        // 3️⃣ Güncellemeleri uygula
        _mapper.Map(request, product);
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ResponseWrapper<ProductResponse>.Success(_mapper.Map<ProductResponse>(product), "Ürün başarıyla güncellendi.");
    }

    /// ✅ **Ürün Detayını Getirme**
    public async Task<IResponseWrapper<ProductResponse>> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.OrderItems)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product == null
            ? ResponseWrapper<ProductResponse>.Fail("Ürün bulunamadı.")
            : ResponseWrapper<ProductResponse>.Success(_mapper.Map<ProductResponse>(product), "Ürün başarıyla getirildi.");
    }

    /// ✅ **Ürünleri Listeleme (Pagination ile)**
    public async Task<IResponseWrapper<PaginationResult<ProductResponse>>> GetProductsAsync(ProductParameters parameters)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchTerm) ||
                x.Description.ToLower().Contains(searchTerm));
        }

        if (parameters.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == parameters.CategoryId.Value);

        if (parameters.IsPublic.HasValue)
            query = query.Where(x => x.IsPublic == parameters.IsPublic.Value);

        query = query.OrderBy(x => x.CreatedAt);

        var totalCount = await query.CountAsync();
        var totalPage = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / parameters.ItemsPerPage) : 0;

        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.ItemsPerPage)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .ToListAsync();

        var mappedItems = _mapper.Map<IEnumerable<ProductResponse>>(items);

        return ResponseWrapper<PaginationResult<ProductResponse>>.Success(
            new PaginationResult<ProductResponse>(mappedItems, totalCount, totalPage, parameters.Page, parameters.ItemsPerPage),
            "Ürünler başarıyla getirildi.");
    }

    /// ✅ **Ürün Soft Delete**
    public async Task<IResponseWrapper<ProductResponse>> SoftDeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return ResponseWrapper<ProductResponse>.Fail("Ürün bulunamadı.");

        product.IsDeleted = true;
        await _context.SaveChangesAsync();

        return ResponseWrapper<ProductResponse>.Success("Ürün başarıyla silindi.");
    }

    /// ✅ **Ürün Tamamen Silme**
    public async Task<IResponseWrapper<ProductResponse>> DeleteProductAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .Include(p => p.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return ResponseWrapper<ProductResponse>.Fail("Ürün bulunamadı.");

        if (product.OrderItems.Any())
            return ResponseWrapper<ProductResponse>.Fail("Bu ürüne ait siparişler olduğu için silinemez.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return ResponseWrapper<ProductResponse>.Success("Ürün başarıyla silindi.");
    }
}
