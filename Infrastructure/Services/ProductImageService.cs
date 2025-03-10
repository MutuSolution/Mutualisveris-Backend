using Application.Services;
using AutoMapper;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public sealed class ProductImageService : IProductImageService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly string _uploadsFolder;

    public ProductImageService(ApplicationDbContext context, IMapper mapper, IHostEnvironment env)
    {
        _context = context;
        _mapper = mapper;

        // wwwroot/uploads klasörüne kaydetmek için yol belirleme
        _uploadsFolder = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");

        // Eğer klasör yoksa oluştur
        if (!Directory.Exists(_uploadsFolder))
            Directory.CreateDirectory(_uploadsFolder);
    }

    /// ✅ **Ürüne Resim Ekleme (Dosya Yükleme Dahil)**
    public async Task<IResponseWrapper<ProductImageResponse>> AddProductImageAsync(int productId, IFormFile formFile)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
            return ResponseWrapper<ProductImageResponse>.Fail("Ürün bulunamadı.");

        // 📌 Resim yükleme işlemi
        string uploadedFileUrl;
        try
        {
            uploadedFileUrl = await FileLoaderAsync(formFile);
        }
        catch (Exception ex)
        {
            return ResponseWrapper<ProductImageResponse>.Fail($"Dosya yüklenirken hata oluştu: {ex.Message}");
        }

        // 📌 İlk resimse `IsMain = true`
        bool isFirstImage = !await _context.ProductImages.AnyAsync(i => i.ProductId == productId);

        var newImage = new ProductImage
        {
            ProductId = productId,
            ImageUrl = uploadedFileUrl,
            IsMain = isFirstImage
        };

        _context.ProductImages.Add(newImage);
        await _context.SaveChangesAsync();

        return ResponseWrapper<ProductImageResponse>
            .Success(_mapper.Map<ProductImageResponse>(newImage), "Resim başarıyla eklendi.");
    }

    /// ✅ **Belirli Bir Ürünün Tüm Resimlerini Getirme**
    public async Task<IResponseWrapper<IEnumerable<ProductImageResponse>>> GetProductImagesAsync(int productId)
    {
        var images = await _context.ProductImages.Where(i => i.ProductId == productId).ToListAsync();
        if (!images.Any())
            return ResponseWrapper<IEnumerable<ProductImageResponse>>.Fail("Bu ürüne ait resim bulunamadı.");

        return ResponseWrapper<IEnumerable<ProductImageResponse>>
            .Success(_mapper.Map<IEnumerable<ProductImageResponse>>(images), "Resimler getirildi.");
    }

    /// ✅ **Belirli Bir Resmi Silme (Dosya Silme Dahil)**
    public async Task<IResponseWrapper<string>> DeleteProductImageAsync(int imageId)
    {
        var image = await _context.ProductImages.Where(i => i.ProductId == imageId).FirstOrDefaultAsync();
        if (image is null)
            return ResponseWrapper<string>.Fail("Resim bulunamadı.");

        // 📌 Önce dosyayı fiziksel olarak sil
        try
        {
            FileRemover(image.ImageUrl);
        }
        catch (Exception ex)
        {
            return ResponseWrapper<string>.Fail($"Dosya silinirken hata oluştu: {ex.Message}");
        }

        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();

        return ResponseWrapper<string>.Success("Resim başarıyla silindi.");
    }

    /// ✅ **Bir Resmi Ana Resim Yapma**
    public async Task<IResponseWrapper<ProductImageResponse>> SetMainImageAsync(int imageId)
    {
        var image = await _context.ProductImages.FindAsync(imageId);
        if (image == null)
            return ResponseWrapper<ProductImageResponse>.Fail("Resim bulunamadı.");

        // ✅ Önce bu ürüne ait tüm resimleri "Ana Resim Değil" olarak güncelle
        await _context.ProductImages
            .Where(i => i.ProductId == image.ProductId)
            .ForEachAsync(i => i.IsMain = false);

        image.IsMain = true;
        await _context.SaveChangesAsync();

        return ResponseWrapper<ProductImageResponse>
            .Success(_mapper.Map<ProductImageResponse>(image), "Ana resim başarıyla güncellendi.");
    }

    /// 📌 **Dosya Yükleme Metodu (Geliştirilmiş)**
    private async Task<string> FileLoaderAsync(IFormFile file)
    {
        // ✅ Geçerli resim formatları
        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        string fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
            throw new Exception("Sadece JPG, JPEG veya PNG formatındaki dosyalar yüklenebilir.");

        // ✅ Dosya boyutu kontrolü (5MB Max)
        if (file.Length > 5 * 1024 * 1024)
            throw new Exception("Dosya boyutu maksimum 5MB olabilir.");

        // ✅ Benzersiz dosya adı oluştur
        string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        string filePath = Path.Combine(_uploadsFolder, uniqueFileName);

        // ✅ Dosyayı kaydet
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        // ✅ Kaydedilen dosyanın URL'sini oluştur
        return $"/uploads/{uniqueFileName}";
    }

    /// 📌 **Dosya Silme Metodu**
    private void FileRemover(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;

        string filePath = Path.Combine(_uploadsFolder, Path.GetFileName(imageUrl));

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
