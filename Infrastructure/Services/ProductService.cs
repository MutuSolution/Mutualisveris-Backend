using Application.Services;
using Application.Services.Identity;
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
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ProductService(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper<ProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        if (request == null)
            return await ResponseWrapper<ProductResponse>.FailAsync("İstek bilgisi boş olamaz.");
        if (string.IsNullOrWhiteSpace(request.Name))
            return await ResponseWrapper<ProductResponse>.FailAsync("Ürün adı gereklidir.");
        if (string.IsNullOrWhiteSpace(request.Description))
            return await ResponseWrapper<ProductResponse>.FailAsync("Ürün açıklaması gereklidir.");

        var categoryInDb = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);
        if (categoryInDb == null)
            return await ResponseWrapper<ProductResponse>.FailAsync("Geçersiz kategori.");

        var existingProduct = await _context.Products
                .FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
        if (existingProduct != null)
            return await ResponseWrapper<ProductResponse>.FailAsync("Ürün zaten var.");

        var existingSKU = await _context.Products
        .FirstOrDefaultAsync(x => x.SKU.ToLower() == request.SKU.ToLower());
        if (existingSKU != null)
            return await ResponseWrapper<ProductResponse>.FailAsync("SKU zaten var.");

        var newProduct = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            SKU = request.SKU,
            CategoryId = request.CategoryId,
            IsPublic = request.IsPublic
        };

        try
        {
            await _context.Products.AddAsync(newProduct);
            var saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                var mappedProduct = _mapper.Map<ProductResponse>(newProduct);
                return await ResponseWrapper<ProductResponse>
                    .SuccessAsync(mappedProduct, "Ürün başarıyla eklendi.");
            }
            else
            {
                return await ResponseWrapper<ProductResponse>
                    .FailAsync("Ürün kaydedilirken bir hata oluştu.");
            }
        }
        catch (Exception ex)
        {
            var message = "Ürün oluşturulurken bir hata meydana geldi: " + ex.Message;
            return await ResponseWrapper<ProductResponse>.FailAsync(message);
        }
    }

    public Task<IResponseWrapper<ProductResponse>> DeleteProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteProductAsync(IResponseWrapper<ProductResponse> productInDb)
    {
        throw new NotImplementedException();
    }

    public Task<IResponseWrapper<ProductResponse>> GetProductByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IResponseWrapper<PaginationResult<ProductResponse>>> GetProductsAsync(ProductParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<IResponseWrapper<ProductResponse>> SoftDeleteProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IResponseWrapper<ProductResponse>> UpdateProductAsync(UpdateProductRequest request)
    {
        throw new NotImplementedException();
    }
}
